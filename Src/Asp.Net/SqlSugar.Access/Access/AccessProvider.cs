using SqlSugar.Access;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SqlSugar.Access
{
    public class AccessProvider : AdoProvider
    {
        public AccessProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        base._DbConnection = new OleDbConnection(base.Context.CurrentConnectionConfig.ConnectionString);
                    }
                    catch (Exception ex)
                    {
                        Check.Exception(true,ex.Message);
                    }
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
            CheckConnection();
            base.Transaction = ((OleDbConnection)this.Connection).BeginTransaction();
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            CheckConnection();
            base.Transaction = ((OleDbConnection)this.Connection).BeginTransaction(iso);
        }
        public override IDataAdapter GetAdapter()
        {
            return new OleDbDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            OleDbCommand sqlCommand = new OleDbCommand(sql, (OleDbConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (OleDbTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                OleDbParameter[] ipars = GetSqlParameter(parameters).Where(x=>sql.ToLower().Contains(x.ParameterName.ToLower())).ToArray();
                if (ipars != null)
                {
                    ipars = ipars.OrderBy(it => sql.IndexOf(it.ParameterName)).ToArray();
                }
                sqlCommand.Parameters.AddRange(ipars);
            
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((OleDbDataAdapter)dataAdapter).SelectCommand = (OleDbCommand)command;
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
            OleDbParameter[] result = new OleDbParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new OleDbParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                //sqlParameter.UdtTypeName = parameter.UdtTypeName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
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
        public string SplitCommandTag = UtilConstants.ReplaceCommaKey;
        public override int ExecuteCommand(string sql, SugarParameter[] parameters)
        {
            if (sql == null) throw new Exception("sql is null");
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                int result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "")
                    {
                        result += base.ExecuteCommand(item, parameters);
                    }
                }
                return result;
            }
            else
            {
                if (sql.TrimStart('\r').TrimStart('\n') != "")
                {
                    return base.ExecuteCommand(sql, parameters);
                }
                else
                {
                    return 0;
                }
            }
        }
        public override async Task<int> ExecuteCommandAsync(string sql, SugarParameter[] parameters)
        {
            if (sql == null) throw new Exception("sql is null");
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                int result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "")
                    {
                        result += await base.ExecuteCommandAsync(item, parameters);
                    }
                }
                return result;
            }
            else
            {
                return base.ExecuteCommand(sql, parameters);
            }
        }
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public OleDbParameter[] GetSqlParameter(params SugarParameter[] parameters)
        {
            var isVarchar =IsVarchar();
            if (parameters == null || parameters.Length == 0) return null;
            OleDbParameter[] result = new OleDbParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new OleDbParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                //sqlParameter.UdtTypeName = parameter.UdtTypeName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                var isTime = parameter.DbType == System.Data.DbType.DateTime;
                if (isTime)
                {
                    sqlParameter.DbType = System.Data.DbType.String;
                    if (sqlParameter.Value != DBNull.Value)
                    {
                        sqlParameter.Value = sqlParameter.Value.ToString();
                    }
                }
                if (sqlParameter.Value!=null&& sqlParameter.Value != DBNull.Value && sqlParameter.DbType == System.Data.DbType.DateTime)
                {
                    var date = Convert.ToDateTime(sqlParameter.Value);
                    if (date==DateTime.MinValue)
                    {
                        sqlParameter.Value = Convert.ToDateTime("1753/01/01");
                    }
                }
                if (parameter.Direction == 0) 
                {
                    parameter.Direction = ParameterDirection.Input;
                }
                sqlParameter.Direction = parameter.Direction;
                result[index] = sqlParameter;
                //if (parameter.TypeName.HasValue()) {
                //    sqlParameter.TypeName = parameter.TypeName;
                //    sqlParameter.SqlDbType = SqlDbType.Structured;
                //    sqlParameter.DbType = System.Data.DbType.Object;
                //}
                if (sqlParameter.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput, ParameterDirection.ReturnValue))
                {
                    if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                    this.OutputParameters.RemoveAll(it => it.ParameterName == sqlParameter.ParameterName);
                    this.OutputParameters.Add(sqlParameter);
                }

                if (isVarchar&&sqlParameter.DbType== System.Data.DbType.String)
                {
                    sqlParameter.DbType =System.Data.DbType.AnsiString;
                }

                ++index;
            }
            return result;
        }
        private bool IsVarchar()
        {
            if (this.Context.CurrentConnectionConfig.MoreSettings != null && this.Context.CurrentConnectionConfig.MoreSettings.DisableNvarchar)
            {
                return true;
            }
            return false;
        }

    }
}
