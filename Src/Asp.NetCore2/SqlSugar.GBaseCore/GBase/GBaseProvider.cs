using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Data.Odbc;
using System.Text.RegularExpressions;

namespace SqlSugar.GBase
{
    public class GBaseProvider : AdoProvider
    {
        public GBaseProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        base._DbConnection = new OdbcConnection(base.Context.CurrentConnectionConfig.ConnectionString);
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

        public string SplitCommandTag => UtilConstants.ReplaceCommaKey.Replace("{", "").Replace("}", "");

       
        public override object GetScalar(string sql, params SugarParameter[] parameters)
        {
            if (this.Context.Ado.Transaction != null)
            {
                return _GetScalar(sql, parameters);
            }
            else 
            {
                try
                {
                    this.Context.Ado.BeginTran();
                    var result = _GetScalar(sql, parameters);
                    this.Context.Ado.CommitTran();
                    return result;
                }
                catch (Exception ex)
                {
                    this.Context.Ado.RollbackTran();
                    throw ex;
                }
            }
        }

        public override async Task<object> GetScalarAsync(string sql, params SugarParameter[] parameters)
        {
            if (this.Context.Ado.Transaction != null)
            {
                return await GetScalarAsync(sql, parameters);
            }
            else
            {
                try
                {
                    this.Context.Ado.BeginTran();
                    var result =await GetScalarAsync(sql, parameters);
                    this.Context.Ado.CommitTran();
                    return result;
                }
                catch (Exception ex)
                {
                    this.Context.Ado.RollbackTran();
                    throw ex;
                }
            }
        }
        private object _GetScalar(string sql, SugarParameter[] parameters)
        {
            if (sql == null) throw new Exception("sql is null");
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                object result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "")
                    {
                        result = base.GetScalar(item, parameters);
                    }
                }
                return result;
            }
            else
            {
                return base.GetScalar(sql, parameters);
            }
        }
        private async Task<object> _GetScalarAsync(string sql, SugarParameter[] parameters)
        {
            if (sql == null) throw new Exception("sql is null");
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                object result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "")
                    {
                        result = await base.GetScalarAsync(item, parameters);
                    }
                }
                return result;
            }
            else
            {
                return await base.GetScalarAsync(sql, parameters);
            }
        }

        public override int ExecuteCommand(string sql, SugarParameter [] parameters)
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
                return base.ExecuteCommand(sql, parameters);
            }
        }
        public override async Task<int> ExecuteCommandAsync(string sql, SugarParameter [] parameters)
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
                        result +=await base.ExecuteCommandAsync(item, parameters);
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
        /// Only GBase
        /// </summary>
        /// <param name="transactionName"></param>
        public override void BeginTran(string transactionName)
        {
            CheckConnection();
            base.Transaction = ((OdbcConnection)this.Connection).BeginTransaction();
        }
        /// <summary>
        /// Only GBase
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            CheckConnection();
            base.Transaction = ((OdbcConnection)this.Connection).BeginTransaction(iso);
        }
     
        public override IDataAdapter GetAdapter()
        {
            return new GBaseDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            var helper = new GBaseInsertBuilder();
            helper.Context = this.Context;
            if (parameters != null)
            {
                foreach (var param in parameters.OrderByDescending(it => it.ParameterName.Length))
                {
                    sql = sql.Replace(param.ParameterName, helper.FormatValue(param.Value) + "");
                }
            }
            OdbcCommand sqlCommand = new OdbcCommand(sql, (OdbcConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (OdbcTransaction)this.Transaction;
            }
            //if (parameters.HasValue())
            //{
            //    OdbcParameter[] ipars = GetSqlParameter(parameters);
            //    sqlCommand.Parameters.AddRange(ipars);
            //}
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((GBaseDataAdapter)dataAdapter).SelectCommand = (OdbcCommand)command;
        }
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return new OdbcParameter[] { };
            OdbcParameter[] result = new OdbcParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new OdbcParameter();
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
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public OdbcParameter[] GetSqlParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            OdbcParameter[] result = new OdbcParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new OdbcParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                //sqlParameter.UdtTypeName = parameter.UdtTypeName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = GetDbType(parameter);
                var isTime = parameter.DbType == System.Data.DbType.Time;
                if (isTime)
                {
                    sqlParameter.Value = DateTime.Parse(parameter.Value?.ToString()).TimeOfDay;
                }
                if (sqlParameter.Value != null && sqlParameter.Value != DBNull.Value && sqlParameter.DbType == System.Data.DbType.DateTime)
                {
                    var date = Convert.ToDateTime(sqlParameter.Value);
                    if (date == DateTime.MinValue)
                    {
                        sqlParameter.Value = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
                    }
                }
                if (parameter.Direction == 0)
                {
                    parameter.Direction = ParameterDirection.Input;
                }
                sqlParameter.Direction = parameter.Direction;
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

        private static System.Data.DbType GetDbType(SugarParameter parameter)
        {
            if (parameter.DbType==System.Data.DbType.UInt16)
            {
                return System.Data.DbType.Int16;
            }
            else if (parameter.DbType == System.Data.DbType.UInt32)
            {
                return System.Data.DbType.Int32;
            }
            else if (parameter.DbType == System.Data.DbType.UInt64)
            {
                return System.Data.DbType.Int64;
            }
            else
            {
                return parameter.DbType;
            }
        }
    }
}
