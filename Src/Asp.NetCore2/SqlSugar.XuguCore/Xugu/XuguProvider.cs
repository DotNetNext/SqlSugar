using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XuguClient;

namespace SqlSugar.Xugu
{
    /// <summary>
    /// 虚谷 ADO 提供器
    /// </summary>
    public class XuguProvider : AdoProvider
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public XuguProvider() { }
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        base._DbConnection = new XGConnection(base.Context.CurrentConnectionConfig.ConnectionString);
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
        public override string SqlParameterKeyWord => ":";
        public string SplitCommandTag => UtilConstants.ReplaceCommaKey.Replace("{", "").Replace("}", "");
        public override object GetScalar(string sql, params SugarParameter[] parameters)
        {
            if (this.Context.Ado.Transaction != null) return _GetScalar(sql, parameters);
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
        public override async Task<object> GetScalarAsync(string sql, params SugarParameter[] parameters)
        {
            if (this.Context.Ado.Transaction != null) return await _GetScalarAsync(sql, parameters);
            try
            {
                this.Context.Ado.BeginTran();
                var result = await _GetScalarAsync(sql, parameters);
                this.Context.Ado.CommitTran();
                return result;
            }
            catch (Exception ex)
            {
                this.Context.Ado.RollbackTran();
                throw ex;
            }
        }
        private void CheckSqlNull(string sql) { if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException("sql", "SQL语句不能为空"); }
        private object _GetScalar(string sql, SugarParameter[] parameters)
        {
            CheckSqlNull(sql);
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                object result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "") result = base.GetScalar(item, parameters);
                }
                return result;
            }
            else return base.GetScalar(sql, parameters);
        }
        private async Task<object> _GetScalarAsync(string sql, SugarParameter[] parameters)
        {
            CheckSqlNull(sql);
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                object result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "") result = await base.GetScalarAsync(item, parameters);
                }
                return result;
            }
            else return await base.GetScalarAsync(sql, parameters);
        }

        public override int ExecuteCommand(string sql, SugarParameter[] parameters)
        {
            CheckSqlNull(sql);
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                int result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "") result += base.ExecuteCommand(item, parameters);
                }
                return result;
            }
            else return base.ExecuteCommand(sql, parameters);
        }
        public override async Task<int> ExecuteCommandAsync(string sql, SugarParameter[] parameters)
        {
            CheckSqlNull(sql);
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                int result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "") result += await base.ExecuteCommandAsync(item, parameters);
                }
                return result;
            }
            else return await base.ExecuteCommandAsync(sql, parameters);
        }

        public override void BeginTran(string transactionName)
        {
            CheckConnection();
            base.Transaction = ((XGConnection)this.Connection).BeginTransaction();
        }

        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            CheckConnection();
            base.Transaction = ((XGConnection)this.Connection).BeginTransaction(iso);
        }

        public override IDataAdapter GetAdapter()=> new XuguDataAdapter();
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            CheckSqlNull(sql);
            var helper = new XuguInsertBuilder();
            helper.Context = this.Context;
            //if (parameters != null)
            //{
            //    foreach (var param in parameters.OrderByDescending(it => it.ParameterName.Length))
            //    {
            //        sql = sql.Replace(param.ParameterName,  "?");//helper.FormatValue(param.Value)
            //    }
            //}
            XGCommand sqlCommand = new XGCommand(sql, (XGConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            if (parameters != null && parameters.Length > 0) sqlCommand.Parameters.AddRange(GetSqlParameter(parameters));
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null) sqlCommand.Transaction = (XGTransaction)this.Transaction;
            //if (parameters.HasValue())
            //{
            //    OdbcParameter[] ipars = GetSqlParameter(parameters);
            //    sqlCommand.Parameters.AddRange(ipars);
            //}
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)=> ((XuguDataAdapter)dataAdapter).SelectCommand = (XGCommand)command;
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)=> GetSqlParameter(parameters);
        private int bufferSize = 1024000;
        public XGParameters[] GetSqlParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            XGParameters[] result = new XGParameters[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new XGParameters();
                sqlParameter.ParameterName = parameter.ParameterName.Trim(this.SqlParameterKeyWord.ToCharArray());
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = GetDbType(parameter);
                if (parameter.DbType == System.Data.DbType.Time)
                {
                    sqlParameter.Value = DateTime.Parse(parameter.Value?.ToString()).TimeOfDay;
                }
                if (parameter.Value is byte[] binary)
                {
                    sqlParameter = new XGParameters(sqlParameter.ParameterName, XGDbType.Binary);
                    sqlParameter.Value = binary;

                    int num = binary.Length / bufferSize;
                    int mod = binary.Length % bufferSize;
                    if (parameter.DbType == System.Data.DbType.AnsiString)//大概是 BLOB
                    {
                        sqlParameter = new XGParameters(sqlParameter.ParameterName, XGDbType.LongVarBinary);
                        var blob_obj = new XGBlob();
                        blob_obj.BeginChunkWrite();
                        for (int i = 0; i < num + (mod > 0 ? 1 : 0); i++) blob_obj.write(binary, i * bufferSize, i == num ? mod : bufferSize);
                        blob_obj.EndChunkWrite();
                        sqlParameter.Value = blob_obj;
                    }
                    //这个基本没什么用。
                    if (parameter.DbType == System.Data.DbType.String)//大概是 CLOB
                    {
                        sqlParameter = new XGParameters(sqlParameter.ParameterName, XGDbType.LongVarChar);
                        var clob_obj = new XGClob();
                        clob_obj.BeginChunkWrite();
                        for (int i = 0; i < num + (mod > 0 ? 1 : 0); i++) clob_obj.write(binary, i * bufferSize, i == num ? mod : bufferSize);
                        clob_obj.EndChunkWrite();
                        sqlParameter.Value = clob_obj;
                    }
                }
                if (parameter.Value is XGClob clob)
                {
                    sqlParameter = new XGParameters(sqlParameter.ParameterName, XGDbType.LongVarChar);
                    sqlParameter.Value = clob.Length == 0 ? null : parameter.Value;
                }
                if (parameter.Value is XGBlob blob)
                {
                    sqlParameter = new XGParameters(sqlParameter.ParameterName, XGDbType.LongVarBinary);
                    sqlParameter.Value = blob.Length == 0 ? null : parameter.Value;
                }
                if (parameter.Value is TimeSpan)
                {
                    sqlParameter.DbType = System.Data.DbType.String;
                    sqlParameter.Value = parameter.Value?.ToString();
                }
                if (parameter.Value != null && parameter.Value != DBNull.Value && parameter.DbType == System.Data.DbType.DateTime)
                {
                    var date = Convert.ToDateTime(parameter.Value);
                    if (date == DateTime.MinValue)
                    {
                        sqlParameter.Value = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
                    }
                }
                if (parameter.Value is DateTimeOffset dateTime)
                {
                    //sqlParameter.DbType = System.Data.DbType.DateTime;
                    //sqlParameter.Value = UtilMethods.ConvertFromDateTimeOffset(dateTime);
                    sqlParameter.DbType = System.Data.DbType.String;
                    sqlParameter.Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
                }
                if (parameter.Direction == 0)
                {
                    parameter.Direction = ParameterDirection.Input;
                }
                sqlParameter.Direction = parameter.Direction;
                result[index] = sqlParameter;
                if (sqlParameter.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput, ParameterDirection.ReturnValue))
                {
                    this.OutputParameters ??= new List<IDataParameter>();
                    this.OutputParameters.RemoveAll(it => it.ParameterName == sqlParameter.ParameterName);
                    this.OutputParameters.Add(sqlParameter);
                }

                ++index;
            }
            return result;
        }
        private static System.Data.DbType GetDbType(SugarParameter parameter)
        {
            if (parameter.DbType == System.Data.DbType.UInt16) return System.Data.DbType.Int16;
            else if (parameter.DbType == System.Data.DbType.UInt32) return System.Data.DbType.Int32;
            else if (parameter.DbType == System.Data.DbType.UInt64) return System.Data.DbType.Int64;
            else return parameter.DbType;
        }
    }
}
