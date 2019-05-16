using IBM.Data.DB2.Core;
using Oracle.ManagedDataAccess.Client;
using SqlSugar.Realization.DB2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class DB2Provider : AdoProvider
    {
        public DB2Provider()
        {
            this.FormatSql = sql =>
            {
                //if (sql.HasValue()&&sql.Contains("@")) {
                //    var exceptionalCaseInfo = Regex.Matches(sql,@"\'.*?\@.*?\'| \w+\@\w+ ");
                //    if (exceptionalCaseInfo != null) {
                //        foreach (var item in exceptionalCaseInfo.Cast<Match>())
                //        {
                //            sql = sql.Replace(item.Value, item.Value.Replace("?", UtilConstants.ReplaceKey));
                //        }
                //    }
                //    sql = sql .Replace("@","?");
                //    sql = sql.Replace(UtilConstants.ReplaceKey, "?");
                //}
                return sql;
            };
        }
        public override string SqlParameterKeyWord
        {
            get
            {
                return "?";
            }
        }
        public override IDbConnection Connection
        {
            get
            {
                try
                {
                    if (base._DbConnection == null)
                    {
                        base._DbConnection = new DB2Connection(base.Context.CurrentConnectionConfig.ConnectionString);
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
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="transactionName"></param>
        public override void BeginTran(string transactionName)
        {
            ((DB2Connection)this.Connection).BeginTransaction();
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            ((DB2Connection)this.Connection).BeginTransaction(iso);
        }
        public override IDataAdapter GetAdapter()
        {
            return new MyDB2DataAdapter();
        }
        public override IDbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            foreach (var item in parameters)
            {
                sql= sql.Replace(item.ParameterName, "?");
            }
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
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, IDbCommand command)
        {
            ((MyDB2DataAdapter)dataAdapter).SelectCommand = (DB2Command)command;
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
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new DB2Parameter();
                sqlParameter.Size = parameter.Size == -1 ? 0 : parameter.Size;
                sqlParameter.ParameterName = parameter.ParameterName;
                if (sqlParameter.ParameterName[0] == '@')
                {
                    sqlParameter.ParameterName = ':' + sqlParameter.ParameterName.Substring(1, sqlParameter.ParameterName.Length - 1);
                }
                if (this.CommandType == CommandType.StoredProcedure)
                {
                    sqlParameter.ParameterName = sqlParameter.ParameterName.TrimStart(':');
                }
                //if (parameter.IsRefCursor)
                //{
                //    sqlParameter.OracleDbType = OracleDbType.RefCursor;
                //}
                if (sqlParameter.DbType == System.Data.DbType.Guid)
                {
                    sqlParameter.DbType = System.Data.DbType.String;
                    sqlParameter.Value = sqlParameter.Value.ObjToString();
                }
                else if (parameter.DbType == System.Data.DbType.Boolean)
                {
                    sqlParameter.DbType = System.Data.DbType.Int16;
                    if (parameter.Value == DBNull.Value)
                    {
                        parameter.Value = 0;
                    }
                    else
                    {
                        sqlParameter.Value = (bool)parameter.Value ? 1 : 0;
                    }
                }
                else
                {
                    if (parameter.Value != null && parameter.Value.GetType() == UtilConstants.GuidType)
                    {
                        parameter.Value = parameter.Value.ToString();
                    }
                    sqlParameter.Value = parameter.Value;
                }
                if (parameter.Direction != 0)
                    sqlParameter.Direction = parameter.Direction;
                result[index] = sqlParameter;
                if (sqlParameter.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput,ParameterDirection.ReturnValue))
                {
                    if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                    this.OutputParameters.RemoveAll(it => it.ParameterName == sqlParameter.ParameterName);
                    this.OutputParameters.Add(sqlParameter);
                }

                ++index;
            }
            return result;
        }
    }
}
