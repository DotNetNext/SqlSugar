using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nhgdb;
using NhgdbTypes;

namespace SqlSugar.HG
{
    public partial class HGProvider : AdoProvider
    {
        public HGProvider() 
        {
            
            AppContext.SetSwitch("Nhgdb.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Nhgdb.DisableDateTimeInfinityConversions", true);
         
        }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var NhgdbConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new NhgdbConnection(NhgdbConnectionString);
                    }
                    catch (Exception)
                    {
                        throw;

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
            return new NhgdbDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            NhgdbCommand sqlCommand = new NhgdbCommand(sql, (NhgdbConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (NhgdbTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((NhgdbParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((NhgdbDataAdapter)dataAdapter).SelectCommand = (NhgdbCommand)command;
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
            NhgdbParameter[] result = new NhgdbParameter[parameters.Length];
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
                var sqlParameter = new NhgdbParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                sqlParameter.Direction = parameter.Direction;
                if (parameter.IsJson)
                {
                    sqlParameter.NhgdbDbType = NhgdbDbType.Json;
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
                if (parameter.CustomDbType != null&& parameter.CustomDbType is NhgdbDbType) 
                {
                    sqlParameter.NhgdbDbType =((NhgdbDbType)parameter.CustomDbType);
                }
            }
            return result;
        }

        private static void Array(SugarParameter parameter, NhgdbParameter sqlParameter)
        {
            //    sqlParameter.Value = this.Context.Utilities.SerializeObject(sqlParameter.Value);
            var type = sqlParameter.Value.GetType();
            if (ArrayMapping.ContainsKey(type))
            {
                sqlParameter.NhgdbDbType = ArrayMapping[type] | NhgdbDbType.Array;
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

        private static void DbNullParametrerArray(SugarParameter parameter, NhgdbParameter sqlParameter)
        {
            if (parameter.DbType.IsIn(System.Data.DbType.Int32))
            {
                sqlParameter.NhgdbDbType = NhgdbDbType.Integer | NhgdbDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Int16))
            {
                sqlParameter.NhgdbDbType = NhgdbDbType.Smallint | NhgdbDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Int64))
            {
                sqlParameter.NhgdbDbType = NhgdbDbType.Bigint | NhgdbDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Guid))
            {
                sqlParameter.NhgdbDbType = NhgdbDbType.Uuid | NhgdbDbType.Array;
            }
            else
            {
                sqlParameter.NhgdbDbType = NhgdbDbType.Text | NhgdbDbType.Array;
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

        static readonly Dictionary<Type, NhgdbDbType> ArrayMapping = new Dictionary<Type, NhgdbDbType>()
        {
            { typeof(int[]),NhgdbDbType.Integer},
            { typeof(short[]),NhgdbDbType.Smallint},
            { typeof(long[]),NhgdbDbType.Bigint},
            { typeof(decimal[]),NhgdbDbType.Numeric},
            { typeof(char[]),NhgdbDbType.Text},
            { typeof(byte[]),NhgdbDbType.Bytea},
            { typeof(bool[]),NhgdbDbType.Boolean},
            {typeof(DateTime[]),NhgdbDbType.Date},
            {typeof(float[]),NhgdbDbType.Real},
            {typeof(Guid[]),NhgdbDbType.Uuid},


            { typeof(int?[]),NhgdbDbType.Integer},
            { typeof(short?[]),NhgdbDbType.Smallint},
            { typeof(long?[]),NhgdbDbType.Bigint},
            { typeof(decimal?[]),NhgdbDbType.Numeric},
            { typeof(char?[]),NhgdbDbType.Text},
            { typeof(byte?[]),NhgdbDbType.Bytea},
            { typeof(bool?[]),NhgdbDbType.Boolean},
            {typeof(DateTime?[]),NhgdbDbType.Date},
            {typeof(Guid?[]),NhgdbDbType.Uuid},


             { typeof(string[]), NhgdbDbType.Text},
             {typeof(float?[]),NhgdbDbType.Real},
        };
    }
}
