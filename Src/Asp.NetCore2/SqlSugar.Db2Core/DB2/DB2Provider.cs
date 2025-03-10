using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.Db2;

namespace SqlSugar.DB2
{
    public partial class DB2Provider : AdoProvider
    {
        public DB2Provider()
        {

        }

        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var connectionString = base.Context.CurrentConnectionConfig.ConnectionString;

                        var connection = new DB2Connection(connectionString);
                        base._DbConnection = connection;
                    }
                    catch (Exception ex)
                    {
                        throw ex;

                    }
                }
                return base._DbConnection;
            }
            set
            {
                base._DbConnection = value;
            }
        }

        public override void BeginTran(string transactionName)
        {
            base.BeginTran();
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            base.BeginTran(iso);
        }
        public override IDataAdapter GetAdapter()
        {
            return new DB2DataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            DB2Command sqlCommand = new DB2Command(sql, (DB2Connection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (DB2Transaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((DB2Parameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((DB2DataAdapter)dataAdapter).SelectCommand = (DB2Command)command;
        }
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            DB2Parameter[] result = new DB2Parameter[parameters.Length];
            int index = 0;
            var isVarchar = this.Context.IsVarchar();
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                if (parameter.Value is System.Data.SqlTypes.SqlDateTime && parameter.DbType == System.Data.DbType.AnsiString)
                {
                    parameter.DbType = System.Data.DbType.DateTime;
                    parameter.Value = DBNull.Value;
                }
                UNumber(parameter);
                var sqlParameter = new DB2Parameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                sqlParameter.Direction = ParameterDirection.InputOutput;
                if (parameter.IsJson)
                {
                    sqlParameter.DB2Type = DB2Type.DbClob;
                }
                if (parameter.IsArray)
                {
                    Array(parameter, sqlParameter);
                }
                if (sqlParameter.Direction == 0)
                {
                    sqlParameter.Direction = ParameterDirection.Input;
                }
                result[index] = sqlParameter;
                if (sqlParameter.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput, ParameterDirection.ReturnValue))
                {
                    if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                    this.OutputParameters.RemoveAll(it => it.ParameterName == sqlParameter.ParameterName);
                    this.OutputParameters.Add(sqlParameter);
                }
                if (isVarchar && sqlParameter.DbType == System.Data.DbType.String)
                {
                    sqlParameter.DbType = System.Data.DbType.AnsiString;
                }
                else if (sqlParameter.Value is DateTime && sqlParameter.DbType == System.Data.DbType.AnsiString)
                {
                    sqlParameter.DbType = System.Data.DbType.DateTime;
                }
                ++index;
                if (parameter.CustomDbType != null && parameter.CustomDbType is DB2Type)
                {
                    sqlParameter.DB2Type = ((DB2Type)parameter.CustomDbType);
                }
            }
            return result;
        }

        private static void Array(SugarParameter parameter, DB2Parameter sqlParameter)
        {
            //    sqlParameter.Value = this.Context.Utilities.SerializeObject(sqlParameter.Value);
            var type = sqlParameter.Value.GetType();
            if (ArrayMapping.ContainsKey(type))
            {
                sqlParameter.DB2Type = ArrayMapping[type] | DB2Type.DynArray;
            }
            else if (type == DBNull.Value.GetType())
            {
                DbNullParametrerArray(parameter, sqlParameter);

            }
            else
            {
                Check.Exception(true, sqlParameter.Value.GetType().Name + " No Support");
            }
        }

        private static void DbNullParametrerArray(SugarParameter parameter, DB2Parameter sqlParameter)
        {
            if (parameter.DbType.IsIn(System.Data.DbType.Int32))
            {
                sqlParameter.DB2Type = DB2Type.Integer | DB2Type.DynArray;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Int16))
            {
                sqlParameter.DB2Type = DB2Type.SmallInt | DB2Type.DynArray;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Int64))
            {
                sqlParameter.DB2Type = DB2Type.BigInt | DB2Type.DynArray;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Guid))
            {
                sqlParameter.DB2Type = DB2Type.VarChar | DB2Type.DynArray;
            }
            else
            {
                sqlParameter.DB2Type = DB2Type.Text | DB2Type.DynArray;
            }
        }

        private static void UNumber(SugarParameter parameter)
        {
            if (parameter.DbType == System.Data.DbType.UInt16)
            {
                parameter.DbType = System.Data.DbType.Int16;
                parameter.Value = Convert.ToInt16(parameter.Value);
            }
            else if (parameter.DbType == System.Data.DbType.UInt32)
            {
                parameter.DbType = System.Data.DbType.Int32;
                parameter.Value = Convert.ToInt32(parameter.Value);
            }
            else if (parameter.DbType == System.Data.DbType.UInt64)
            {
                parameter.DbType = System.Data.DbType.Int64;
                parameter.Value = Convert.ToInt64(parameter.Value);
            }
        }

        static readonly Dictionary<Type, DB2Type> ArrayMapping = new Dictionary<Type, DB2Type>()
        {
            { typeof(int[]),DB2Type.Integer},
            { typeof(short[]),DB2Type.SmallInt},
            { typeof(long[]),DB2Type.BigInt},
            { typeof(decimal[]),DB2Type.Numeric},
            { typeof(char[]),DB2Type.Clob},
            { typeof(byte[]),DB2Type.Byte},
            { typeof(bool[]),DB2Type.Boolean},
            {typeof(DateTime[]),DB2Type.Date},
            {typeof(float[]),DB2Type.Real},
            {typeof(Guid[]),DB2Type.VarChar},


            { typeof(int?[]),DB2Type.Integer},
            { typeof(short?[]),DB2Type.SmallInt},
            { typeof(long?[]),DB2Type.BigInt},
            { typeof(decimal?[]),DB2Type.Numeric},
            { typeof(char?[]),DB2Type.Text},
            { typeof(byte?[]),DB2Type.Byte},
            { typeof(bool?[]),DB2Type.Boolean},
            {typeof(DateTime?[]),DB2Type.Date},
            {typeof(Guid?[]),DB2Type.VarChar},


             { typeof(string[]), DB2Type.Text},
             {typeof(float?[]),DB2Type.Real},
        };
    }
}
