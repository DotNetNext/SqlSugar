using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar.MySqlConnector
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
                        if (!mySqlConnectionString.ToLower().Contains("charset") && !mySqlConnectionString.ToLower().Contains("character"))
                        {
                            mySqlConnectionString = mySqlConnectionString.Trim().TrimEnd(';') + ";charset=utf8;";
                        }
                        base._DbConnection = new MySqlConnection(mySqlConnectionString);
                    }
                    catch (Exception ex)
                    {
                        Check.Exception(true, ex.Message);
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
            return new MySqlConnectorDataAdapter();
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
            ((MySqlConnectorDataAdapter)dataAdapter).SelectCommand = (MySqlCommand)command;
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
            var isVarchar =IsVarchar();
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


        #region async
        public async Task CloseAsync()
        {
            if (this.Transaction != null)
            {
                this.Transaction = null;
            }
            if (this.Connection != null && this.Connection.State == ConnectionState.Open)
            {
                await (this.Connection as MySqlConnection).CloseAsync();
            }
            if (this.IsMasterSlaveSeparation && this.SlaveConnections.HasValue())
            {
                foreach (var slaveConnection in this.SlaveConnections)
                {
                    if (slaveConnection != null && slaveConnection.State == ConnectionState.Open)
                    {
                        await (slaveConnection as MySqlConnection).CloseAsync();
                    }
                }
            }
        }
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
            try
            {
                Async();
                InitParameters(ref sql, parameters);
                if (FormatSql != null)
                    sql = FormatSql(sql);
                SetConnectionStart(sql);
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql,ref parameters);
                ExecuteBefore(sql, parameters);
                var sqlCommand =await GetCommandAsync(sql, parameters);
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
            try
            {
                Async();
                InitParameters(ref sql, parameters);
                if (FormatSql != null)
                    sql = FormatSql(sql);
                SetConnectionStart(sql);
                var isSp = this.CommandType == CommandType.StoredProcedure;
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql,ref parameters);
                ExecuteBefore(sql, parameters);
                var sqlCommand =   await GetCommandAsync(sql, parameters);
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
            try
            {
                Async();
                InitParameters(ref sql, parameters);
                if (FormatSql != null)
                    sql = FormatSql(sql);
                SetConnectionStart(sql);
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql,ref parameters);
                ExecuteBefore(sql, parameters);
                var sqlCommand = await GetCommandAsync(sql, parameters) ;
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
