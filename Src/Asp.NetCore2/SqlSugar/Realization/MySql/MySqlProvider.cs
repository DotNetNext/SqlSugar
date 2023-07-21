using MySqlConnector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class MySqlProvider : AdoProvider
    {
        public MySqlProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var mySqlConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        Check.ExceptionEasy(String.IsNullOrEmpty(mySqlConnectionString), "ConnectionString is not null", "连接字符串ConnectionString不能为Null");
                        if (!mySqlConnectionString.ToLower().Contains("charset")&& !mySqlConnectionString.ToLower().Contains("character"))
                        {
                            mySqlConnectionString = mySqlConnectionString.Trim().TrimEnd(';') + ";charset=utf8;";
                        }
                        //if (!mySqlConnectionString.ToLower().Contains("min"))
                        //{
                        //    mySqlConnectionString = mySqlConnectionString.Trim().TrimEnd(';') + ";min pool size=1";
                        //}
                        base._DbConnection = new MySqlConnection(mySqlConnectionString);
                    }
                    catch (Exception ex)
                    {
                        if (ex is SqlSugarException)
                        {
                            throw ex;
                        }
                        else
                        {
                            Check.Exception(true, ErrorMessage.ConnnectionOpen, ex.Message);
                        }
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
            return new MySqlDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            MySqlCommand sqlCommand = new MySqlCommand(sql, (MySqlConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (MySqlTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((MySqlParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((MySqlDataAdapter)dataAdapter).SelectCommand = (MySqlCommand)command;
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
            MySqlParameter[] result = new MySqlParameter[parameters.Length];
            int index = 0;
            var isVarchar = this.Context.IsVarchar();
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new MySqlParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                if (parameter.Direction == 0)
                {
                    parameter.Direction = ParameterDirection.Input; 
                }
                sqlParameter.Direction = parameter.Direction;
                //if (sqlParameter.Direction == 0)
                //{
                //    sqlParameter.Direction = ParameterDirection.Input;
                //}
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
                else if (parameter.DbType== System.Data.DbType.DateTimeOffset) 
                {
                    if(sqlParameter.Value != DBNull.Value)
                       sqlParameter.Value = UtilMethods.ConvertFromDateTimeOffset((DateTimeOffset)sqlParameter.Value);
                    sqlParameter.DbType = System.Data.DbType.DateTime;
                }
                if (sqlParameter.Value is DateTime&&sqlParameter.Value.ObjToDate()==DateTime.MinValue)
                {
                    var date = Convert.ToDateTime(sqlParameter.Value);
                    if (date == DateTime.MinValue)
                    {
                        sqlParameter.Value = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
                    }
                }
                if (parameter.IsJson == false&& sqlParameter.Value!=null&& sqlParameter.Value is JArray) 
                {
                    sqlParameter.Value = sqlParameter.Value.ToString();
                }
                ++index;
            }
            return result;
        }
        protected override void SugarCatch(Exception ex, string sql, SugarParameter[] parameters)
        {
            base.SugarCatch(ex, sql, parameters);

            if (ex is NullReferenceException&&SugarCompatible.IsFramework) 
            {
                Check.ExceptionEasy($"To upgrade the MySql.Data. Error:{ex.Message}", $" 请先升级MySql.Data 。 详细错误:{ex.Message}");
            }
        }

        #region async
        public async Task<DbCommand> GetCommandAsync(string sql, SugarParameter[] parameters)
        {
            MySqlCommand sqlCommand = new MySqlCommand(sql, (MySqlConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (MySqlTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((MySqlParameter[])ipars);
            }
            if (this.Connection.State != ConnectionState.Open)
            {
                try
                {
                    await (this.Connection as MySqlConnection).OpenAsync();
                }
                catch (Exception ex)
                {
                    Check.Exception(true, ex.Message);
                }
            }
            return sqlCommand;
        }
        public override async Task<int> ExecuteCommandAsync(string sql, params SugarParameter[] parameters)
        {
            if (this.Context.CurrentConnectionConfig?.SqlMiddle?.IsSqlMiddle == true)
                return  await base.ExecuteCommandAsync(sql,parameters);
            try
            {
                Async();
                InitParameters(ref sql, parameters);
                if (FormatSql != null)
                    sql = FormatSql(sql);
                SetConnectionStart(sql);
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, ref parameters);
                ExecuteBefore(sql, parameters);
                var sqlCommand = await GetCommandAsync(sql, parameters);
                int count;
                if (this.CancellationToken == null)
                    count = await sqlCommand.ExecuteNonQueryAsync();
                else
                    count = await sqlCommand.ExecuteNonQueryAsync(this.CancellationToken.Value);
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
                sqlCommand.Dispose();
                return count;
            }
            catch (Exception ex)
            {
                CommandType = CommandType.Text;
                if (ErrorEvent != null)
                    ExecuteErrorEvent(sql, parameters, ex);
                throw ex;
            }
            finally
            {
                if (this.IsAutoClose())
                {
                    await this.CloseAsync();
                }
                SetConnectionEnd(sql);
            }
        }
        public override async Task<IDataReader> GetDataReaderAsync(string sql, params SugarParameter[] parameters)
        {
            if (this.Context.CurrentConnectionConfig?.SqlMiddle?.IsSqlMiddle == true)
                return await base.GetDataReaderAsync(sql, parameters);
            try
            {
                Async();
                InitParameters(ref sql, parameters);
                if (FormatSql != null)
                    sql = FormatSql(sql);
                SetConnectionStart(sql);
                var isSp = this.CommandType == CommandType.StoredProcedure;
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, ref parameters);
                ExecuteBefore(sql, parameters);
                var sqlCommand = await GetCommandAsync(sql, parameters);
                DbDataReader sqlDataReader;
                if (this.CancellationToken == null)
                    sqlDataReader = await sqlCommand.ExecuteReaderAsync(this.IsAutoClose() ? CommandBehavior.CloseConnection : CommandBehavior.Default);
                else
                    sqlDataReader = await sqlCommand.ExecuteReaderAsync(this.IsAutoClose() ? CommandBehavior.CloseConnection : CommandBehavior.Default, this.CancellationToken.Value);
                if (isSp)
                    DataReaderParameters = sqlCommand.Parameters;
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
                SetConnectionEnd(sql);
                if (SugarCompatible.IsFramework || this.Context.CurrentConnectionConfig.DbType != DbType.Sqlite)
                    sqlCommand.Dispose();
                return sqlDataReader;
            }
            catch (Exception ex)
            {
                CommandType = CommandType.Text;
                if (ErrorEvent != null)
                    ExecuteErrorEvent(sql, parameters, ex);
                throw ex;
            }
        }
        public override async Task<object> GetScalarAsync(string sql, params SugarParameter[] parameters)
        {
            if (this.Context.CurrentConnectionConfig?.SqlMiddle?.IsSqlMiddle == true)
                return await base.GetScalarAsync(sql, parameters);
            try
            {
                Async();
                InitParameters(ref sql, parameters);
                if (FormatSql != null)
                    sql = FormatSql(sql);
                SetConnectionStart(sql);
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, ref parameters);
                ExecuteBefore(sql, parameters);
                var sqlCommand = await GetCommandAsync(sql, parameters);
                object scalar;
                if (CancellationToken == null)
                    scalar = await sqlCommand.ExecuteScalarAsync();
                else
                    scalar = await sqlCommand.ExecuteScalarAsync(this.CancellationToken.Value);
                //scalar = (scalar == null ? 0 : scalar);
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
                sqlCommand.Dispose();
                return scalar;
            }
            catch (Exception ex)
            {
                CommandType = CommandType.Text;
                if (ErrorEvent != null)
                    ExecuteErrorEvent(sql, parameters, ex);
                throw ex;
            }
            finally
            {
                if (this.IsAutoClose())
                {
                    await this.CloseAsync();
                }
                SetConnectionEnd(sql);
            }
        }
        #endregion
    }
}
