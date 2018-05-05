using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class SqliteProvider : AdoProvider
    {
        public SqliteProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                try
                {
                    if (base._DbConnection == null)
                    {
                        var SqliteConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new SqliteConnection(SqliteConnectionString);
                    }
                }
                catch (Exception ex)
                {
                    Check.Exception(true, ErrorMessage.ConnnectionOpen, ex.Message);
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
            return new SqliteDataAdapter();
        }
        public override IDbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            SqliteCommand sqlCommand = new SqliteCommand(sql, (SqliteConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (SqliteTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((SqliteParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, IDbCommand command)
        {
            ((SqliteDataAdapter)dataAdapter).SelectCommand = (SqliteCommand)command;
        }
        /// <summary>
        /// if Sqlite return SqliteParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            SqliteParameter[] result = new SqliteParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new SqliteParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                result[index] = sqlParameter;
                if (sqlParameter.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput,ParameterDirection.ReturnValue))
                {
                    if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                    this.OutputParameters.RemoveAll(it => it.ParameterName == sqlParameter.ParameterName);
                    this.OutputParameters.Add(sqlParameter);
                }
                if (sqlParameter.DbType == System.Data.DbType.Guid) {
                    sqlParameter.DbType = System.Data.DbType.String;
                    sqlParameter.Value = sqlParameter.Value.ObjToString();
                }
                ++index;
            }
            return result;
        }
    }
}
