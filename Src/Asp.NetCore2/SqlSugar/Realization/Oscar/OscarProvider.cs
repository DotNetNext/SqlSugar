using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OscarClient;

namespace SqlSugar
{
    public partial class OscarProvider : AdoProvider
    {
        public OscarProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var OscarConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new OscarConnection(OscarConnectionString);
                    }
                    catch (Exception ex)
                    {
                        Check.Exception(true, ErrorMessage.ConnnectionOpen, ex.Message);
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
            return new OscarDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            OscarCommand sqlCommand = new OscarCommand(sql, (OscarConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (OscarTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((OscarParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((OscarDataAdapter)dataAdapter).SelectCommand = (OscarCommand)command;
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
            OscarParameter[] result = new OscarParameter[parameters.Length];
            int index = 0;
            var isVarchar = this.Context.IsVarchar();
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new OscarParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                sqlParameter.Direction = parameter.Direction;
                if (parameter.IsJson)
                {
                    sqlParameter.OscarDbType = OscarDbType.Text;
                }
                if (parameter.IsArray)
                {
                    //    sqlParameter.Value = this.Context.Utilities.SerializeObject(sqlParameter.Value);
                    var type = sqlParameter.Value.GetType();
                    if (ArrayMapping.ContainsKey(type))
                    {
                        sqlParameter.OscarDbType = ArrayMapping[type] | OscarDbType.Array;
                    }
                    else
                    {
                        Check.Exception(true, sqlParameter.Value.GetType().Name + " No Support");
                    }
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
                ++index;
            }
            return result;
        }


        static readonly Dictionary<Type, OscarDbType> ArrayMapping = new Dictionary<Type, OscarDbType>()
        {
            { typeof(int[]),OscarDbType.Integer},
            { typeof(short[]),OscarDbType.SmallInt},
            { typeof(long[]),OscarDbType.BigInt},
            { typeof(decimal[]),OscarDbType.Numeric},
            { typeof(char[]),OscarDbType.Text},
            { typeof(byte[]),OscarDbType.Bytea},
            { typeof(bool[]),OscarDbType.Boolean},
            {typeof(DateTime[]),OscarDbType.Date},
            {typeof(float[]),OscarDbType.Real},


            { typeof(int?[]),OscarDbType.Integer},
            { typeof(short?[]),OscarDbType.SmallInt},
            { typeof(long?[]),OscarDbType.BigInt},
            { typeof(decimal?[]),OscarDbType.Numeric},
            { typeof(char?[]),OscarDbType.Text},
            { typeof(byte?[]),OscarDbType.Bytea},
            { typeof(bool?[]),OscarDbType.Boolean},
            {typeof(DateTime?[]),OscarDbType.Date},


             { typeof(string[]), OscarDbType.Text},
             {typeof(float?[]),OscarDbType.Real},
        };
    }
}
