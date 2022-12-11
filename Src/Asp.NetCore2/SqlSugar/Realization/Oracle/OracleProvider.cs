using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class OracleProvider : AdoProvider
    {
        public OracleProvider()
        {
            this.FormatSql = sql =>
            {
                sql = sql.Replace("+@", "+:");
                if (sql.HasValue()&&sql.Contains("@")) {
                    var exceptionalCaseInfo = Regex.Matches(sql, @"\'[^\=]*?\@.*?\'|[\.,\w]+\@[\.,\w]+ | [\.,\w]+\@[\.,\w]+|[\.,\w]+\@[\.,\w]+ |\d+\@\d|\@\@");
                    if (exceptionalCaseInfo != null) {
                        foreach (var item in exceptionalCaseInfo.Cast<Match>())
                        {
                            if (item.Value != null && item.Value.IndexOf(",") == 1&&Regex.IsMatch(item.Value, @"^ \,\@\w+$")) 
                            {
                                break;
                            }
                            else if (item.Value != null &&Regex.IsMatch(item.Value.Trim(), @"^\w+\,\@\w+\,$"))
                            {
                                break;
                            }
                            sql = sql.Replace(item.Value, item.Value.Replace("@", UtilConstants.ReplaceKey));
                        }
                    }
                    sql = sql .Replace("@",":");
                    sql = sql.Replace(UtilConstants.ReplaceKey, "@");
                }
                return sql;
            };
        }
        public override string SqlParameterKeyWord
        {
            get
            {
                return ":";
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
                        base._DbConnection = new OracleConnection(base.Context.CurrentConnectionConfig.ConnectionString);
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
            ((OracleConnection)this.Connection).BeginTransaction();
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            ((OracleConnection)this.Connection).BeginTransaction(iso);
        }
        public override IDataAdapter GetAdapter()
        {
            return new MyOracleDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            sql = ReplaceKeyWordParameterName(sql, parameters);
            OracleCommand sqlCommand = new OracleCommand(sql, (OracleConnection)this.Connection);
            sqlCommand.BindByName = true;
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            sqlCommand.InitialLONGFetchSize = -1;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (OracleTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((OracleParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }

        private static string ReplaceKeyWordParameterName(string sql, SugarParameter[] parameters)
        {
            if (parameters.HasValue())
            {
                foreach (var Parameter in parameters)
                {
                    if (Parameter.ParameterName != null && Parameter.ParameterName.ToLower().IsIn("@order",":order","@user", "@level",  ":user", ":level"))
                    {
                        if (parameters.Count(it => it.ParameterName.StartsWith(Parameter.ParameterName)) == 1)
                        {
                            var newName = Parameter.ParameterName + "_01";
                            sql = sql.Replace(Parameter.ParameterName, newName);
                            Parameter.ParameterName = newName;
                        }
                        else
                        {
                            Check.ExceptionEasy($" {Parameter.ParameterName} is key word", $"{Parameter.ParameterName}是关键词");
                        }
                    }
                }
            }

            return sql;
        }
        public override Action<SqlSugarException> ErrorEvent => it => {

            if (it.Message != null && it.Message.Contains("无效的主机/绑定变量名"))
            {
                Check.ExceptionEasy(it.Message, $"错误：{it.Message}，出现这个错的原因： 1.可能是参数名为关键词（例如 @user ）2. SQL错误。");
            }
        };

        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((MyOracleDataAdapter)dataAdapter).SelectCommand = (OracleCommand)command;
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
            OracleParameter[] result = new OracleParameter[parameters.Length];
            int index = 0;
            var isVarchar = this.Context.IsVarchar();
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new OracleParameter();
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
                if (parameter.IsRefCursor)
                {
                    sqlParameter.OracleDbType = OracleDbType.RefCursor;
                }
                if (parameter.IsNvarchar2&& parameter.DbType==System.Data.DbType.String)
                {
                    sqlParameter.OracleDbType = OracleDbType.NVarchar2;
                }
                if (parameter.IsClob)
                {
                    sqlParameter.OracleDbType = OracleDbType.Clob;
                    sqlParameter.Value = parameter.Value;
                }
                if (parameter.IsArray)
                {
                    sqlParameter.OracleDbType = OracleDbType.Varchar2;
                    sqlParameter.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                }
                if (sqlParameter.DbType == System.Data.DbType.Guid)
                {
                    sqlParameter.DbType = System.Data.DbType.String;
                    sqlParameter.Value = sqlParameter.Value.ObjToString();
                }
                else if (parameter.DbType == System.Data.DbType.DateTimeOffset)
                {
                    if (parameter.Value != DBNull.Value)
                        sqlParameter.Value = UtilMethods.ConvertFromDateTimeOffset((DateTimeOffset)parameter.Value);
                    sqlParameter.DbType = System.Data.DbType.DateTime;
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
                else if (parameter.DbType == System.Data.DbType.DateTime)
                {
                    sqlParameter.Value = parameter.Value;
                    sqlParameter.DbType = System.Data.DbType.DateTime;
                }
                else if (parameter.DbType == System.Data.DbType.Date)
                {
                    sqlParameter.Value = parameter.Value;
                    sqlParameter.DbType = System.Data.DbType.Date;
                }
                else if (parameter.DbType == System.Data.DbType.AnsiStringFixedLength)
                {
                    sqlParameter.DbType = System.Data.DbType.AnsiStringFixedLength;
                    sqlParameter.Value = parameter.Value;
                }
                else if (parameter.DbType == System.Data.DbType.AnsiString)
                {
                    sqlParameter.DbType = System.Data.DbType.AnsiString;
                    sqlParameter.Value = parameter.Value;
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
    }
}
