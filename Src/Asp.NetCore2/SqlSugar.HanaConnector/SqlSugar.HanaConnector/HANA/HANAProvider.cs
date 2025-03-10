﻿using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using SqlSugar.HanaConnector;
namespace SqlSugar.HANAConnector
{
    public class HANAProvider : AdoProvider
    {
        public HANAProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var hanaConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        //if (!hanaConnectionString.ToLower().Contains("charset") && !hanaConnectionString.ToLower().Contains("character"))
                        //{
                        //    hanaConnectionString = hanaConnectionString.Trim().TrimEnd(';') + ";charset=utf8;";
                        //}
                        base._DbConnection = new HanaConnection(hanaConnectionString);
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
            return new MyHanaDataAdapter(); //DataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            sql = ReplaceKeyWordParameterName(sql, parameters);
            HanaCommand sqlCommand = new HanaCommand(sql, (HanaConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (HanaTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((HanaParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((MyHanaDataAdapter)dataAdapter).SelectCommand = (HanaCommand)command;
        }
        /// <summary>
        /// if hana return HANAParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            HanaParameter[] result = new HanaParameter[parameters.Length];
            int index = 0;
            var isVarchar =IsVarchar();
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new HanaParameter();
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
                    if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>(); //IDbDataParameter, IDataParameter
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

        private static string[] KeyWord = new string[] {  };
        private static string ReplaceKeyWordParameterName(string sql, SugarParameter[] parameters)
        {
            sql = ReplaceKeyWordWithAd(sql, parameters);
            if (parameters.HasValue() && parameters.Count(it => it.ParameterName.ToLower().IsIn(KeyWord)) > 0)
            {
                int i = 0;
                foreach (var Parameter in parameters.OrderByDescending(it => it.ParameterName.Length))
                {
                    if (Parameter.ParameterName != null && Parameter.ParameterName.ToLower().IsContainsIn(KeyWord))
                    {
                        var newName = ":p" + i + 100;
                        sql =System.Text.RegularExpressions.Regex.Replace(sql, Parameter.ParameterName, newName, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        Parameter.ParameterName = newName;
                        i++;
                    }
                }
            }
            return sql;
        }
        private static string ReplaceKeyWordWithAd(string sql, SugarParameter[] parameters)
        {
            if (parameters != null && sql != null && sql.Contains("@"))
            {
                foreach (var item in parameters.OrderByDescending(it => it.ParameterName.Length))
                {
                    if (item.ParameterName.StartsWith("@"))
                    {
                        item.ParameterName = ":" + item.ParameterName.TrimStart('@');
                    }
                    sql = System.Text.RegularExpressions.Regex.Replace(sql, "@" + item.ParameterName.TrimStart(':'), item.ParameterName, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }
            }

            return sql;
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
                 (this.Connection as HanaConnection).Close();
            }
            if (this.IsMasterSlaveSeparation && this.SlaveConnections.HasValue())
            {
                foreach (var slaveConnection in this.SlaveConnections)
                {
                    if (slaveConnection != null && slaveConnection.State == ConnectionState.Open)
                    {
                         (slaveConnection as HanaConnection).Close();
                    }
                }
            }
        }
        public async Task<DbCommand> GetCommandAsync(string sql, SugarParameter[] parameters)
        {
            HanaCommand sqlCommand = new HanaCommand(sql, (HanaConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (HanaTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((HanaParameter[])ipars);
            }
            if (this.Connection.State != ConnectionState.Open)
            {
                try
                {
                    await (this.Connection as HanaConnection).OpenAsync();
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
