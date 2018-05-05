using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        public virtual Action<Exception> ErrorEvent { get; set; }
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
                foreach (var slaveConnection in this.SlaveConnections)
                {
                    if (slaveConnection != null && slaveConnection.State == ConnectionState.Open)
                    {
                        slaveConnection.Dispose();
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
        public DbResult<bool> UseTran(Action action)
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
            }
            return result;
        }
        public DbResult<T> UseTran<T>(Func<T> action)
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
            }
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
                if (ErrorEvent != null)
                    ErrorEvent(ex);
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
                if (ErrorEvent != null)
                    ErrorEvent(ex);
                throw ex;
            }
        }
        public virtual DataSet GetDataSetAll(string sql, params SugarParameter[] parameters)
        {
            try
            {
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
                if (ErrorEvent != null)
                    ErrorEvent(ex);
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
                if (ErrorEvent != null)
                    ErrorEvent(ex);
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
        private void ExecuteProcessingSQL(ref string sql, SugarParameter[] parameters)
        {
            var result = this.ProcessingEventStartingSQL(sql, parameters);
            sql = result.Key;
            parameters = result.Value;
        }
        public virtual void ExecuteBefore(string sql, SugarParameter[] parameters)
        {
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
                return this.Context.CurrentConnectionConfig.SlaveConnectionConfigs.HasValue();
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
        #endregion
    }
}
