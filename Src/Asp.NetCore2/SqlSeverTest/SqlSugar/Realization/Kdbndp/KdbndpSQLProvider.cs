using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kdbndp;
using KdbndpTypes;

namespace SqlSugar
{
    public partial class KdbndpProvider : AdoProvider
    {
        public KdbndpProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var npgsqlConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new KdbndpConnection(npgsqlConnectionString);
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
            return new KdbndpDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            KdbndpCommand sqlCommand = new KdbndpCommand(sql, (KdbndpConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (KdbndpTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((KdbndpParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((KdbndpDataAdapter)dataAdapter).SelectCommand = (KdbndpCommand)command;
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
            KdbndpParameter[] result = new KdbndpParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new KdbndpParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                sqlParameter.Direction = parameter.Direction;
                if (parameter.IsJson)
                {
                    sqlParameter.KdbndpDbType = KdbndpDbType.Json;
                }
                if (parameter.IsArray)
                {
                    //    sqlParameter.Value = this.Context.Utilities.SerializeObject(sqlParameter.Value);
                    var type = sqlParameter.Value.GetType();
                    if (ArrayMapping.ContainsKey(type))
                    {
                        sqlParameter.KdbndpDbType = ArrayMapping[type] | KdbndpDbType.Array;
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
                ++index;
            }
            return result;
        }


        static readonly Dictionary<Type, KdbndpDbType> ArrayMapping = new Dictionary<Type, KdbndpDbType>()
        {
            { typeof(int[]),KdbndpDbType.Integer},
            { typeof(short[]),KdbndpDbType.Smallint},
            { typeof(long[]),KdbndpDbType.Bigint},
            { typeof(decimal[]),KdbndpDbType.Numeric},
            { typeof(char[]),KdbndpDbType.Text},
            { typeof(byte[]),KdbndpDbType.Bytea},
            { typeof(bool[]),KdbndpDbType.Boolean},
            {typeof(DateTime[]),KdbndpDbType.Date},


            { typeof(int?[]),KdbndpDbType.Integer},
            { typeof(short?[]),KdbndpDbType.Smallint},
            { typeof(long?[]),KdbndpDbType.Bigint},
            { typeof(decimal?[]),KdbndpDbType.Numeric},
            { typeof(char?[]),KdbndpDbType.Text},
            { typeof(byte?[]),KdbndpDbType.Bytea},
            { typeof(bool?[]),KdbndpDbType.Boolean},
            {typeof(DateTime?[]),KdbndpDbType.Date},


             { typeof(string[]), KdbndpDbType.Text},
        };
    }
}
