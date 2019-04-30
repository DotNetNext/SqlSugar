using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
namespace SqlSugar
{
    ///<summary>
    /// ** description：ActiveX Data Objects
    /// ** author：sunkaixuan
    /// ** date：2017/1/2
    /// ** email:610262374@qq.com
    /// </summary>
    public abstract partial class AdoProvider : AdoAccessory, IAdo
    {
        #region Constructor
        public AdoProvider()
        {
            this.IsEnableLogEvent = false;
            this.CommandType = CommandType.Text;
            this.IsClearParameters = true;
            this.CommandTimeOut = 30000;
        }
        #endregion

        #region Properties
        protected List<IDataParameter> OutputParameters { get; set; }
        public virtual string SqlParameterKeyWord { get { return "@"; } }
        public IDbTransaction Transaction { get; set; }
        public virtual SqlSugarClient Context { get; set; }
        internal CommandType OldCommandType { get; set; }
        internal bool OldClearParameters { get; set; }
        public IDataParameterCollection DataReaderParameters { get; set; }
        public TimeSpan SqlExecutionTime { get { return AfterTime - BeforeTime; } }
        public bool IsDisableMasterSlaveSeparation { get; set; }
        internal DateTime BeforeTime = DateTime.MinValue;
        internal DateTime AfterTime = DateTime.MinValue;
        public virtual IDbBind DbBind
        {
            get
            {
                if (base._DbBind == null)
                {
                    IDbBind bind = InstanceFactory.GetDbBind(this.Context.CurrentConnectionConfig);
                    base._DbBind = bind;
                    bind.Context = this.Context;
                }
                return base._DbBind;
            }
        }
        public virtual int CommandTimeOut { get; set; }
        public virtual CommandType CommandType { get; set; }
        public virtual bool IsEnableLogEvent { get; set; }
        public virtual bool IsClearParameters { get; set; }
        public virtual Action<string, SugarParameter[]> LogEventStarting { get; set; }
        public virtual Action<string, SugarParameter[]> LogEventCompleted { get; set; }
        public virtual Func<string, SugarParameter[], KeyValuePair<string, SugarParameter[]>> ProcessingEventStartingSQL { get; set; }
        protected virtual Func<string,string> FormatSql { get; set; }
        public virtual Action<SqlSugarException> ErrorEvent { get; set; }
        public virtual Action<DiffLogModel> DiffLogEvent { get; set; }
        public virtual List<IDbConnection> SlaveConnections { get; set; }
        public virtual IDbConnection MasterConnection { get; set; }
        #endregion

        #region Connection
        public virtual void Open()
        {
            CheckConnection();
        }
        public virtual void Close()
        {
            if (this.Transaction != null)
            {
                this.Transaction = null;
            }
            if (this.Connection != null && this.Connection.State == ConnectionState.Open)
            {
                this.Connection.Close();
            }
            if (this.IsMasterSlaveSeparation && this.SlaveConnections.HasValue())
            {
                foreach (var slaveConnection in this.SlaveConnections)
                {
                    if (slaveConnection != null && slaveConnection.State == ConnectionState.Open)
                    {
                        slaveConnection.Close();
                    }
                }
            }
        }
        public virtual void Dispose()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
                this.Transaction = null;
            }
            if (this.Connection != null && this.Connection.State != ConnectionState.Open)
            {
                this.Connection.Close();
            }
            if (this.Connection != null)
            {
                this.Connection.Dispose();
            }
            this.Connection = null;

            if (this.IsMasterSlaveSeparation)
            {
                if (this.SlaveConnections != null)
                {
                    foreach (var slaveConnection in this.SlaveConnections)
                    {
                        if (slaveConnection != null && slaveConnection.State == ConnectionState.Open)
                        {
                            slaveConnection.Dispose();
                        }
                    }
                }
            }
        }
        public virtual void CheckConnection()
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                try
                {
                    this.Connection.Open();
                }
                catch (Exception ex)
                {
                    Check.Exception(true, ErrorMessage.ConnnectionOpen, ex.Message);
                }
            }
        }

        #endregion

        #region Transaction
        public virtual void BeginTran()
        {
            CheckConnection();
            this.Transaction = this.Connection.BeginTransaction();
        }
        public virtual void BeginTran(IsolationLevel iso)
        {
            CheckConnection();
            this.Transaction = this.Connection.BeginTransaction(iso);
        }
        public virtual void RollbackTran()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Rollback();
                this.Transaction = null;
                if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection) this.Close();
            }
        }
        public virtual void CommitTran()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
                this.Transaction = null;
                if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection) this.Close();
            }
        }
        #endregion

        #region abstract
        public abstract IDataParameter[] ToIDbDataParameter(params SugarParameter[] pars);
        public abstract void SetCommandToAdapter(IDataAdapter adapter, IDbCommand command);
        public abstract IDataAdapter GetAdapter();
        public abstract IDbCommand GetCommand(string sql, SugarParameter[] pars);
        public abstract IDbConnection Connection { get; set; }
        public abstract void BeginTran(string transactionName);//Only SqlServer
        public abstract void BeginTran(IsolationLevel iso, string transactionName);//Only SqlServer 
        #endregion

        #region Use
        public DbResult<bool> UseTran(Action action, Action<Exception> errorCallBack=null)
        {
            var result = new DbResult<bool>();
            try
            {
                this.BeginTran();
                if (action != null)
                    action();
                this.CommitTran();
                result.Data = result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorException = ex;
                result.ErrorMessage = ex.Message;
                result.IsSuccess = false;
                this.RollbackTran();
                if (errorCallBack != null)
                {
                    errorCallBack(ex);
                }
            }
            return result;
        }

        public Task<DbResult<bool>> UseTranAsync(Action action, Action<Exception> errorCallBack = null)
        {
            Task<DbResult<bool>> result = new Task<DbResult<bool>>(() =>
            {
                return UseTran(action,errorCallBack);
            });
            TaskStart(result);
            return result;
        }

        public DbResult<T> UseTran<T>(Func<T> action, Action<Exception> errorCallBack = null)
        {
            var result = new DbResult<T>();
            try
            {
                this.BeginTran();
                if (action != null)
                    result.Data = action();
                this.CommitTran();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorException = ex;
                result.ErrorMessage = ex.Message;
                result.IsSuccess = false;
                this.RollbackTran();
                if (errorCallBack != null)
                {
                    errorCallBack(ex);
                }
            }
            return result;
        }

        public Task<DbResult<T>> UseTranAsync<T>(Func<T> action, Action<Exception> errorCallBack = null)
        {
            Task<DbResult<T>> result = new Task<DbResult<T>>(() =>
            {
                return UseTran(action,errorCallBack);
            });
            TaskStart(result);
            return result;
        }

        public void UseStoredProcedure(Action action)
        {
            var oldCommandType = this.CommandType;
            this.CommandType = CommandType.StoredProcedure;
            this.IsClearParameters = false;
            if (action != null)
            {
                action();
            }
            this.CommandType = oldCommandType;
            this.IsClearParameters = true;
        }
        public T UseStoredProcedure<T>(Func<T> action)
        {
            T result = default(T);
            var oldCommandType = this.CommandType;
            this.CommandType = CommandType.StoredProcedure;
            this.IsClearParameters = false;
            if (action != null)
            {
                result = action();
            }
            this.CommandType = oldCommandType;
            this.IsClearParameters = true;
            return result;
        }
        public IAdo UseStoredProcedure()
        {
            this.OldCommandType = this.CommandType;
            this.OldClearParameters = this.IsClearParameters;
            this.CommandType = CommandType.StoredProcedure;
            this.IsClearParameters = false;
            return this;
        }
        #endregion

        #region Core
        public virtual int ExecuteCommand(string sql, params SugarParameter[] parameters)
        {
            try
            {
                InitParameters(ref sql, parameters);
                if (FormatSql != null) 
                    sql = FormatSql(sql);
                SetConnectionStart(sql);
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, parameters);
                ExecuteBefore(sql, parameters);
                IDbCommand sqlCommand = GetCommand(sql, parameters);
                int count = sqlCommand.ExecuteNonQuery();
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
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
                if (this.IsAutoClose()) this.Close();
                SetConnectionEnd(sql);
            }
        }
        public virtual IDataReader GetDataReader(string sql, params SugarParameter[] parameters)
        {
            try
            {
                InitParameters(ref sql, parameters);
                if (FormatSql != null)
                    sql = FormatSql(sql);
                SetConnectionStart(sql);
                var isSp = this.CommandType == CommandType.StoredProcedure;
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, parameters);
                ExecuteBefore(sql, parameters);
                IDbCommand sqlCommand = GetCommand(sql, parameters);
                IDataReader sqlDataReader = sqlCommand.ExecuteReader(this.IsAutoClose() ? CommandBehavior.CloseConnection : CommandBehavior.Default);
                if (isSp)
                    DataReaderParameters = sqlCommand.Parameters;
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
                SetConnectionEnd(sql);
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
        public virtual IDataReader GetDataReaderNoClose(string sql, params SugarParameter[] parameters)
        {
            try
            {
                InitParameters(ref sql, parameters);
                if (FormatSql != null)
                    sql = FormatSql(sql);
                SetConnectionStart(sql);
                var isSp = this.CommandType == CommandType.StoredProcedure;
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, parameters);
                ExecuteBefore(sql, parameters);
                IDbCommand sqlCommand = GetCommand(sql, parameters);
                IDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (isSp)
                    DataReaderParameters = sqlCommand.Parameters;
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
                SetConnectionEnd(sql);
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
        public virtual DataSet GetDataSetAll(string sql, params SugarParameter[] parameters)
        {
            try
            {
                InitParameters(ref sql, parameters);
                if (FormatSql != null)
                    sql = FormatSql(sql);
                SetConnectionStart(sql);
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, parameters);
                ExecuteBefore(sql, parameters);
                IDataAdapter dataAdapter = this.GetAdapter();
                IDbCommand sqlCommand = GetCommand(sql, parameters);
                this.SetCommandToAdapter(dataAdapter, sqlCommand);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
                return ds;
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
                if (this.IsAutoClose()) this.Close();
                SetConnectionEnd(sql);
            }
        }
        public virtual object GetScalar(string sql, params SugarParameter[] parameters)
        {
            try
            {
                InitParameters(ref sql,parameters);
                if (FormatSql != null)
                    sql = FormatSql(sql);
                SetConnectionStart(sql);
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, parameters);
                ExecuteBefore(sql, parameters);
                IDbCommand sqlCommand = GetCommand(sql, parameters);
                object scalar = sqlCommand.ExecuteScalar();
                //scalar = (scalar == null ? 0 : scalar);
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
                return scalar;
            }
            catch (Exception ex)
            {
                CommandType = CommandType.Text;
                if (ErrorEvent != null)
                    ExecuteErrorEvent(sql,parameters,ex);
                throw ex;
            }
            finally
            {
                if (this.IsAutoClose()) this.Close();
                SetConnectionEnd(sql);
            }
        }
        #endregion

        #region Methods

        public virtual string GetString(string sql, object parameters)
        {
            return GetString(sql, this.GetParameters(parameters));
        }
        public virtual string GetString(string sql, params SugarParameter[] parameters)
        {
            return Convert.ToString(GetScalar(sql, parameters));
        }
        public virtual string GetString(string sql, List<SugarParameter> parameters)
        {
            if (parameters == null)
            {
                return GetString(sql);
            }
            else
            {
                return GetString(sql, parameters.ToArray());
            }
        }
        public virtual int GetInt(string sql, object parameters)
        {
            return GetInt(sql, this.GetParameters(parameters));
        }
        public virtual long GetLong(string sql, object parameters)
        {
            return Convert.ToInt64(GetScalar(sql, GetParameters(parameters)));
        }
        public virtual int GetInt(string sql, params SugarParameter[] parameters)
        {
            return GetScalar(sql, parameters).ObjToInt();
        }
        public virtual int GetInt(string sql, List<SugarParameter> parameters)
        {
            if (parameters == null)
            {
                return GetInt(sql);
            }
            else
            {
                return GetInt(sql, parameters.ToArray());
            }
        }
        public virtual Double GetDouble(string sql, object parameters)
        {
            return GetDouble(sql, this.GetParameters(parameters));
        }
        public virtual Double GetDouble(string sql, params SugarParameter[] parameters)
        {
            return GetScalar(sql, parameters).ObjToMoney();
        }
        public virtual Double GetDouble(string sql, List<SugarParameter> parameters)
        {
            if (parameters == null)
            {
                return GetDouble(sql);
            }
            else
            {
                return GetDouble(sql, parameters.ToArray());
            }
        }
        public virtual decimal GetDecimal(string sql, object parameters)
        {
            return GetDecimal(sql, this.GetParameters(parameters));
        }
        public virtual decimal GetDecimal(string sql, params SugarParameter[] parameters)
        {
            return GetScalar(sql, parameters).ObjToDecimal();
        }
        public virtual decimal GetDecimal(string sql, List<SugarParameter> parameters)
        {
            if (parameters == null)
            {
                return GetDecimal(sql);
            }
            else
            {
                return GetDecimal(sql, parameters.ToArray());
            }
        }
        public virtual DateTime GetDateTime(string sql, object parameters)
        {
            return GetDateTime(sql, this.GetParameters(parameters));
        }
        public virtual DateTime GetDateTime(string sql, params SugarParameter[] parameters)
        {
            return GetScalar(sql, parameters).ObjToDate();
        }
        public virtual DateTime GetDateTime(string sql, List<SugarParameter> parameters)
        {
            if (parameters == null)
            {
                return GetDateTime(sql);
            }
            else
            {
                return GetDateTime(sql, parameters.ToArray());
            }
        }
        public virtual List<T> SqlQuery<T>(string sql, object parameters = null)
        {
            var sugarParameters = this.GetParameters(parameters);
            return SqlQuery<T>(sql, sugarParameters);
        }
        public virtual List<T> SqlQuery<T>(string sql, params SugarParameter[] parameters)
        {
            this.Context.InitMppingInfo<T>();
            var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parameters != null && parameters.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parameters);
            var dataReader = this.GetDataReader(builder.SqlQueryBuilder.ToSqlString(), builder.SqlQueryBuilder.Parameters.ToArray());
            List<T> result = this.DbBind.DataReaderToList<T>(typeof(T), dataReader);
            builder.SqlQueryBuilder.Clear();
            if (this.Context.Ado.DataReaderParameters != null)
            {
                foreach (IDataParameter item in this.Context.Ado.DataReaderParameters)
                {
                    var parameter = parameters.FirstOrDefault(it => item.ParameterName.Substring(1) == it.ParameterName.Substring(1));
                    if (parameter != null)
                    {
                        parameter.Value = item.Value;
                    }
                }
                this.Context.Ado.DataReaderParameters = null;
            }
            return result;
        }

        public virtual List<T> SqlQuery<T>(string sql, List<SugarParameter> parameters)
        {
            if (parameters != null)
            {
                return SqlQuery<T>(sql, parameters.ToArray());
            }
            else
            {
                return SqlQuery<T>(sql);
            }
        }
        public Tuple<List<T>, List<T2>> SqlQuery<T, T2>(string sql, object parameters = null)
        {
            var parsmeterArray = this.GetParameters(parameters);
            this.Context.InitMppingInfo<T>();
            var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parsmeterArray != null && parsmeterArray.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parsmeterArray);
            using (var dataReader = this.GetDataReaderNoClose(builder.SqlQueryBuilder.ToSqlString(), builder.SqlQueryBuilder.Parameters.ToArray()))
            {
                List<T> result = this.DbBind.DataReaderToListNoUsing<T>(typeof(T), dataReader);
                NextResult(dataReader);
                List<T2> result2 = this.DbBind.DataReaderToListNoUsing<T2>(typeof(T2), dataReader);
                builder.SqlQueryBuilder.Clear();
                if (this.Context.Ado.DataReaderParameters != null)
                {
                    foreach (IDataParameter item in this.Context.Ado.DataReaderParameters)
                    {
                        var parameter = parsmeterArray.FirstOrDefault(it => item.ParameterName.Substring(1) == it.ParameterName.Substring(1));
                        if (parameter != null)
                        {
                            parameter.Value = item.Value;
                        }
                    }
                    this.Context.Ado.DataReaderParameters = null;
                }
                return Tuple.Create<List<T>, List<T2>>(result, result2);
            }
        }

        public Tuple<List<T>, List<T2>, List<T3>> SqlQuery<T, T2, T3>(string sql, object parameters = null)
        {
            var parsmeterArray = this.GetParameters(parameters);
            this.Context.InitMppingInfo<T>();
            var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parsmeterArray != null && parsmeterArray.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parsmeterArray);
            using (var dataReader = this.GetDataReaderNoClose(builder.SqlQueryBuilder.ToSqlString(), builder.SqlQueryBuilder.Parameters.ToArray()))
            {
                List<T> result = this.DbBind.DataReaderToListNoUsing<T>(typeof(T), dataReader);
                NextResult(dataReader);
                List<T2> result2 = this.DbBind.DataReaderToListNoUsing<T2>(typeof(T2), dataReader);
                NextResult(dataReader);
                List<T3> result3 = this.DbBind.DataReaderToListNoUsing<T3>(typeof(T3), dataReader);
                builder.SqlQueryBuilder.Clear();
                if (this.Context.Ado.DataReaderParameters != null)
                {
                    foreach (IDataParameter item in this.Context.Ado.DataReaderParameters)
                    {
                        var parameter = parsmeterArray.FirstOrDefault(it => item.ParameterName.Substring(1) == it.ParameterName.Substring(1));
                        if (parameter != null)
                        {
                            parameter.Value = item.Value;
                        }
                    }
                    this.Context.Ado.DataReaderParameters = null;
                }
                return Tuple.Create<List<T>, List<T2>, List<T3>>(result, result2, result3);
            }
        }

        public Tuple<List<T>, List<T2>, List<T3>, List<T4>> SqlQuery<T, T2, T3, T4>(string sql, object parameters = null)
        {
            var parsmeterArray = this.GetParameters(parameters);
            this.Context.InitMppingInfo<T>();
            var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parsmeterArray != null && parsmeterArray.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parsmeterArray);
            using (var dataReader = this.GetDataReaderNoClose(builder.SqlQueryBuilder.ToSqlString(), builder.SqlQueryBuilder.Parameters.ToArray()))
            {
                List<T> result = this.DbBind.DataReaderToListNoUsing<T>(typeof(T), dataReader);
                NextResult(dataReader);
                List<T2> result2 = this.DbBind.DataReaderToListNoUsing<T2>(typeof(T2), dataReader);
                NextResult(dataReader);
                List<T3> result3 = this.DbBind.DataReaderToListNoUsing<T3>(typeof(T3), dataReader);
                NextResult(dataReader);
                List<T4> result4 = this.DbBind.DataReaderToListNoUsing<T4>(typeof(T4), dataReader);
                builder.SqlQueryBuilder.Clear();
                if (this.Context.Ado.DataReaderParameters != null)
                {
                    foreach (IDataParameter item in this.Context.Ado.DataReaderParameters)
                    {
                        var parameter = parsmeterArray.FirstOrDefault(it => item.ParameterName.Substring(1) == it.ParameterName.Substring(1));
                        if (parameter != null)
                        {
                            parameter.Value = item.Value;
                        }
                    }
                    this.Context.Ado.DataReaderParameters = null;
                }
                return Tuple.Create<List<T>, List<T2>, List<T3>, List<T4>>(result, result2, result3, result4);
            }
        }
        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>> SqlQuery<T, T2, T3, T4, T5>(string sql, object parameters = null)
        {
            var parsmeterArray = this.GetParameters(parameters);
            this.Context.InitMppingInfo<T>();
            var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parsmeterArray != null && parsmeterArray.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parsmeterArray);
            using (var dataReader = this.GetDataReaderNoClose(builder.SqlQueryBuilder.ToSqlString(), builder.SqlQueryBuilder.Parameters.ToArray()))
            {
                List<T> result = this.DbBind.DataReaderToListNoUsing<T>(typeof(T), dataReader);
                NextResult(dataReader);
                List<T2> result2 = this.DbBind.DataReaderToListNoUsing<T2>(typeof(T2), dataReader);
                NextResult(dataReader);
                List<T3> result3 = this.DbBind.DataReaderToListNoUsing<T3>(typeof(T3), dataReader);
                NextResult(dataReader);
                List<T4> result4 = this.DbBind.DataReaderToListNoUsing<T4>(typeof(T4), dataReader);
                NextResult(dataReader);
                List<T5> result5 = this.DbBind.DataReaderToListNoUsing<T5>(typeof(T5), dataReader);
                builder.SqlQueryBuilder.Clear();
                if (this.Context.Ado.DataReaderParameters != null)
                {
                    foreach (IDataParameter item in this.Context.Ado.DataReaderParameters)
                    {
                        var parameter = parsmeterArray.FirstOrDefault(it => item.ParameterName.Substring(1) == it.ParameterName.Substring(1));
                        if (parameter != null)
                        {
                            parameter.Value = item.Value;
                        }
                    }
                    this.Context.Ado.DataReaderParameters = null;
                }
                return Tuple.Create<List<T>, List<T2>, List<T3>, List<T4>, List<T5>>(result, result2, result3, result4, result5);
            }
        }

        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>> SqlQuery<T, T2, T3, T4, T5, T6>(string sql, object parameters = null)
        {
            var parsmeterArray = this.GetParameters(parameters);
            this.Context.InitMppingInfo<T>();
            var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parsmeterArray != null && parsmeterArray.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parsmeterArray);
            using (var dataReader = this.GetDataReaderNoClose(builder.SqlQueryBuilder.ToSqlString(), builder.SqlQueryBuilder.Parameters.ToArray()))
            {
                List<T> result = this.DbBind.DataReaderToListNoUsing<T>(typeof(T), dataReader);
                NextResult(dataReader);
                List<T2> result2 = this.DbBind.DataReaderToListNoUsing<T2>(typeof(T2), dataReader);
                NextResult(dataReader);
                List<T3> result3 = this.DbBind.DataReaderToListNoUsing<T3>(typeof(T3), dataReader);
                NextResult(dataReader);
                List<T4> result4 = this.DbBind.DataReaderToListNoUsing<T4>(typeof(T4), dataReader);
                NextResult(dataReader);
                List<T5> result5 = this.DbBind.DataReaderToListNoUsing<T5>(typeof(T5), dataReader);
                NextResult(dataReader);
                List<T6> result6 = this.DbBind.DataReaderToListNoUsing<T6>(typeof(T6), dataReader);
                builder.SqlQueryBuilder.Clear();
                if (this.Context.Ado.DataReaderParameters != null)
                {
                    foreach (IDataParameter item in this.Context.Ado.DataReaderParameters)
                    {
                        var parameter = parsmeterArray.FirstOrDefault(it => item.ParameterName.Substring(1) == it.ParameterName.Substring(1));
                        if (parameter != null)
                        {
                            parameter.Value = item.Value;
                        }
                    }
                    this.Context.Ado.DataReaderParameters = null;
                }
                return Tuple.Create<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>>(result, result2, result3, result4, result5, result6);
            }
        }

        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>> SqlQuery<T, T2, T3, T4, T5, T6, T7>(string sql, object parameters = null)
        {
            var parsmeterArray = this.GetParameters(parameters);
            this.Context.InitMppingInfo<T>();
            var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parsmeterArray != null && parsmeterArray.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parsmeterArray);
            using (var dataReader = this.GetDataReaderNoClose(builder.SqlQueryBuilder.ToSqlString(), builder.SqlQueryBuilder.Parameters.ToArray()))
            {
                List<T> result = this.DbBind.DataReaderToListNoUsing<T>(typeof(T), dataReader);
                NextResult(dataReader);
                List<T2> result2 = this.DbBind.DataReaderToListNoUsing<T2>(typeof(T2), dataReader);
                NextResult(dataReader);
                List<T3> result3 = this.DbBind.DataReaderToListNoUsing<T3>(typeof(T3), dataReader);
                NextResult(dataReader);
                List<T4> result4 = this.DbBind.DataReaderToListNoUsing<T4>(typeof(T4), dataReader);
                NextResult(dataReader);
                List<T5> result5 = this.DbBind.DataReaderToListNoUsing<T5>(typeof(T5), dataReader);
                NextResult(dataReader);
                List<T6> result6 = this.DbBind.DataReaderToListNoUsing<T6>(typeof(T6), dataReader);
                NextResult(dataReader);
                List<T7> result7 = this.DbBind.DataReaderToListNoUsing<T7>(typeof(T7), dataReader);
                builder.SqlQueryBuilder.Clear();
                if (this.Context.Ado.DataReaderParameters != null)
                {
                    foreach (IDataParameter item in this.Context.Ado.DataReaderParameters)
                    {
                        var parameter = parsmeterArray.FirstOrDefault(it => item.ParameterName.Substring(1) == it.ParameterName.Substring(1));
                        if (parameter != null)
                        {
                            parameter.Value = item.Value;
                        }
                    }
                    this.Context.Ado.DataReaderParameters = null;
                }
                return Tuple.Create<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>>(result, result2, result3, result4, result5, result6, result7);
            }
        }

        private static void NextResult(IDataReader dataReader)
        {
            try
            {
                dataReader.NextResult();
            }
            catch
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage("Please reduce the number of T. Save Queue Changes queries don't have so many results", "请减少T的数量，SaveQueueChanges 查询没有这么多结果"));
            }
        }
        public virtual T SqlQuerySingle<T>(string sql, object parameters = null)
        {
            var result = SqlQuery<T>(sql, parameters);
            return result == null ? default(T) : result.FirstOrDefault();
        }
        public virtual T SqlQuerySingle<T>(string sql, params SugarParameter[] parameters)
        {
            var result = SqlQuery<T>(sql, parameters);
            return result == null ? default(T) : result.FirstOrDefault();
        }
        public virtual T SqlQuerySingle<T>(string sql, List<SugarParameter> parameters)
        {
            var result = SqlQuery<T>(sql, parameters);
            return result == null ? default(T) : result.FirstOrDefault();
        }
        public virtual dynamic SqlQueryDynamic(string sql, object parameters = null)
        {
            var dt = this.GetDataTable(sql, parameters);
            return dt == null ? null : this.Context.Utilities.DataTableToDynamic(dt);
        }
        public virtual dynamic SqlQueryDynamic(string sql, params SugarParameter[] parameters)
        {
            var dt = this.GetDataTable(sql, parameters);
            return dt == null ? null : this.Context.Utilities.DataTableToDynamic(dt);
        }
        public dynamic SqlQueryDynamic(string sql, List<SugarParameter> parameters)
        {
            var dt = this.GetDataTable(sql, parameters);
            return dt == null ? null : this.Context.Utilities.DataTableToDynamic(dt);
        }
        public virtual DataTable GetDataTable(string sql, params SugarParameter[] parameters)
        {
            var ds = GetDataSetAll(sql, parameters);
            if (ds.Tables.Count != 0 && ds.Tables.Count > 0) return ds.Tables[0];
            return new DataTable();
        }
        public virtual DataTable GetDataTable(string sql, object parameters)
        {
            return GetDataTable(sql, this.GetParameters(parameters));
        }
        public virtual DataTable GetDataTable(string sql, List<SugarParameter> parameters)
        {
            if (parameters == null)
            {
                return GetDataTable(sql);
            }
            else
            {
                return GetDataTable(sql, parameters.ToArray());
            }
        }
        public virtual DataSet GetDataSetAll(string sql, object parameters)
        {
            return GetDataSetAll(sql, this.GetParameters(parameters));
        }
        public virtual DataSet GetDataSetAll(string sql, List<SugarParameter> parameters)
        {
            if (parameters == null)
            {
                return GetDataSetAll(sql);
            }
            else
            {
                return GetDataSetAll(sql, parameters.ToArray());
            }
        }
        public virtual IDataReader GetDataReader(string sql, object parameters)
        {
            return GetDataReader(sql, this.GetParameters(parameters));
        }
        public virtual IDataReader GetDataReader(string sql, List<SugarParameter> parameters)
        {
            if (parameters == null)
            {
                return GetDataReader(sql);
            }
            else
            {
                return GetDataReader(sql, parameters.ToArray());
            }
        }
        public virtual object GetScalar(string sql, object parameters)
        {
            return GetScalar(sql, this.GetParameters(parameters));
        }
        public virtual object GetScalar(string sql, List<SugarParameter> parameters)
        {
            if (parameters == null)
            {
                return GetScalar(sql);
            }
            else
            {
                return GetScalar(sql, parameters.ToArray());
            }
        }
        public virtual int ExecuteCommand(string sql, object parameters)
        {
            return ExecuteCommand(sql, GetParameters(parameters));
        }
        public virtual int ExecuteCommand(string sql, List<SugarParameter> parameters)
        {
            if (parameters == null)
            {
                return ExecuteCommand(sql);
            }
            else
            {
                return ExecuteCommand(sql, parameters.ToArray());
            }
        }
        #endregion

        #region  Helper
        private void TaskStart<Type>(Task<Type> result)
        {
            if (this.Context.CurrentConnectionConfig.IsShardSameThread)
            {
                Check.Exception(true, "IsShardSameThread=true can't be used async method");
            }
            result.Start();
        }
        private void ExecuteProcessingSQL(ref string sql, SugarParameter[] parameters)
        {
            var result = this.ProcessingEventStartingSQL(sql, parameters);
            sql = result.Key;
            parameters = result.Value;
        }
        public virtual void ExecuteBefore(string sql, SugarParameter[] parameters)
        {
            if (this.Context.IsAsyncMethod==false&&this.Context.CurrentConnectionConfig.Debugger != null && this.Context.CurrentConnectionConfig.Debugger.EnableThreadSecurityValidation == true) {

                var contextId =this.Context.ContextID.ToString();
                var processId = Thread.CurrentThread.ManagedThreadId.ToString();
                var cache = new ReflectionInoCacheService();
                if (!cache.ContainsKey<string>(contextId))
                {
                    cache.Add(contextId, processId);
                }
                else {
                    var cacheValue = cache.Get<string>(contextId);
                    if (processId != cacheValue) {
                       throw new SqlSugarException(this.Context,ErrorMessage.GetThrowMessage("Detection of SqlSugarClient cross-threading usage,a thread needs a new one", "检测到声名的SqlSugarClient跨线程使用，请检查是否静态、是否单例、或者IOC配置错误引起的，保证一个线程new出一个对象 ，具本Sql:")+sql,parameters);
                    }
                }
            }
            this.BeforeTime = DateTime.Now;
            if (this.IsEnableLogEvent)
            {
                Action<string, SugarParameter[]> action = LogEventStarting;
                if (action != null)
                {
                    if (parameters == null || parameters.Length == 0)
                    {
                        action(sql, new SugarParameter[] { });
                    }
                    else
                    {
                        action(sql, parameters);
                    }
                }
            }
        }
        public virtual void ExecuteAfter(string sql, SugarParameter[] parameters)
        {
            this.AfterTime = DateTime.Now;
            var hasParameter = parameters.HasValue();
            if (hasParameter)
            {
                foreach (var outputParameter in parameters.Where(it => it.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput,ParameterDirection.ReturnValue)))
                {
                    var gobalOutputParamter = this.OutputParameters.FirstOrDefault(it => it.ParameterName == outputParameter.ParameterName);
                    if (gobalOutputParamter == null) {//Oracle bug
                        gobalOutputParamter=this.OutputParameters.FirstOrDefault(it => it.ParameterName == outputParameter.ParameterName.TrimStart(outputParameter.ParameterName.First()));
                    }
                    outputParameter.Value = gobalOutputParamter.Value;
                    this.OutputParameters.Remove(gobalOutputParamter);
                }
            }
            if (this.IsEnableLogEvent)
            {
                Action<string, SugarParameter[]> action = LogEventCompleted;
                if (action != null)
                {
                    if (parameters == null || parameters.Length == 0)
                    {
                        action(sql, new SugarParameter[] { });
                    }
                    else
                    {
                        action(sql, parameters);
                    }
                }
            }
            if (this.OldCommandType != 0)
            {
                this.CommandType = this.OldCommandType;
                this.IsClearParameters = this.OldClearParameters;
                this.OldCommandType = 0;
                this.OldClearParameters = false;
            }
        }
        public virtual SugarParameter[] GetParameters(object parameters, PropertyInfo[] propertyInfo = null)
        {
            if (parameters == null) return null;
            return base.GetParameters(parameters, propertyInfo, this.SqlParameterKeyWord);
        }
        private bool IsAutoClose()
        {
            return this.Context.CurrentConnectionConfig.IsAutoCloseConnection && this.Transaction == null;
        }
        private bool IsMasterSlaveSeparation
        {
            get
            {
                return this.Context.CurrentConnectionConfig.SlaveConnectionConfigs.HasValue()&& this.IsDisableMasterSlaveSeparation==false;
            }
        }
        private void SetConnectionStart(string sql)
        {
            if (this.Transaction==null&&this.IsMasterSlaveSeparation && IsRead(sql))
            {
                if (this.MasterConnection == null)
                {
                    this.MasterConnection = this.Connection;
                }
                var saves = this.Context.CurrentConnectionConfig.SlaveConnectionConfigs.Where(it => it.HitRate > 0).ToList();
                var currentIndex = UtilRandom.GetRandomIndex(saves.ToDictionary(it => saves.ToList().IndexOf(it), it => it.HitRate));
                var currentSaveConnection = saves[currentIndex];
                this.Connection = null;
                this.Context.CurrentConnectionConfig.ConnectionString = currentSaveConnection.ConnectionString;
                this.Connection = this.Connection;
                if (this.SlaveConnections.IsNullOrEmpty() || !this.SlaveConnections.Any(it => EqualsConnectionString(it.ConnectionString, this.Connection.ConnectionString)))
                {
                    if (this.SlaveConnections == null) this.SlaveConnections = new List<IDbConnection>();
                    this.SlaveConnections.Add(this.Connection);
                }
            }
        }

        private bool EqualsConnectionString(string connectionString1, string connectionString2)
        {
            var connectionString1Array = connectionString1.Split(';');
            var connectionString2Array = connectionString2.Split(';');
            var result = connectionString1Array.Except(connectionString2Array);
            return result.Count() == 0;
        }

        private void SetConnectionEnd(string sql)
        {
            if (this.IsMasterSlaveSeparation && IsRead(sql)&&this.Transaction==null)
            {
                this.Connection = this.MasterConnection;
                this.Context.CurrentConnectionConfig.ConnectionString = this.MasterConnection.ConnectionString;
            }
        }

        private bool IsRead(string sql)
        {
            var sqlLower = sql.ToLower();
            var result = Regex.IsMatch(sqlLower, "[ ]*select[ ]") && !Regex.IsMatch(sqlLower, "[ ]*insert[ ]|[ ]*update[ ]|[ ]*delete[ ]");
            return result;
        }

        private void ExecuteErrorEvent(string sql, SugarParameter[] parameters, Exception ex)
        {
            ErrorEvent(new SqlSugarException(this.Context,ex, sql, parameters));
        }
        private  void InitParameters(ref string sql, SugarParameter[] parameters)
        {
            if (parameters.HasValue())
            {
                foreach (var item in parameters)
                {
                    if (item.Value != null)
                    {
                        var type = item.Value.GetType();
                        if ((type != UtilConstants.ByteArrayType && type.IsArray) || type.FullName.IsCollectionsList())
                        {
                            var newValues = new List<string>();
                            foreach (var inValute in item.Value as IEnumerable)
                            {
                                newValues.Add(inValute.ObjToString());
                            }
                            if (newValues.IsNullOrEmpty())
                            {
                                newValues.Add("-1");
                            }
                            if (item.ParameterName.Substring(0, 1) == ":")
                            {
                                sql = sql.Replace("@"+item.ParameterName.Substring(1), newValues.ToArray().ToJoinSqlInVals());
                            }
                            sql = sql.Replace(item.ParameterName, newValues.ToArray().ToJoinSqlInVals());
                            item.Value = DBNull.Value;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
