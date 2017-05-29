﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
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
        public virtual string SqlParameterKeyWord { get { return "@"; } }
        public IDbTransaction Transaction { get; set; }
        public virtual SqlSugarClient Context { get; set; }
        public virtual ConnectionConfig MasterConnectionConfig { get; set; }
        public virtual List<ConnectionConfig> SlaveConnectionConfigs { get; set; }
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
        public virtual Action<string, string> LogEventStarting { get; set; }
        public virtual Action<string, string> LogEventCompleted { get; set; }
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
            if (this.Connection != null && this.Connection.State != ConnectionState.Open)
            {
                this.Connection.Close();
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
        }
        public virtual void CheckConnection()
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                this.Connection.Open();
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

        #region Core
        public virtual int ExecuteCommand(string sql, params SugarParameter[] pars)
        {
            base.SetParamterSize(pars);
            ExecLogEvent(sql, pars, true);
            IDbCommand sqlCommand = GetCommand(sql, pars);
            int count = sqlCommand.ExecuteNonQuery();
            if (this.IsClearParameters)
                sqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection && this.Transaction == null) this.Close();
            return count;
        }

        #region Use
        public SugarMessageResult<bool> UseTran(Action action)
        {
            var result = new SugarMessageResult<bool>();
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
                result.Exception = ex;
                result.Messaage = ex.Message;
                result.IsSuccess = false;
                this.RollbackTran();
            }
            return result;
        }
        public SugarMessageResult<T> UseTran<T>(Func<T> action)
        {
            var result = new SugarMessageResult<T>();
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
                result.Exception = ex;
                result.Messaage = ex.Message;
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
        #endregion

        public virtual IDataReader GetDataReader(string sql, params SugarParameter[] pars)
        {
            base.SetParamterSize(pars);
            ExecLogEvent(sql, pars, true);
            IDbCommand sqlCommand = GetCommand(sql, pars);
            IDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            if (this.IsClearParameters)
                sqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return sqlDataReader;
        }
        public virtual DataSet GetDataSetAll(string sql, params SugarParameter[] pars)
        {
            base.SetParamterSize(pars);
            ExecLogEvent(sql, pars, true);
            IDataAdapter dataAdapter = this.GetAdapter();
            IDbCommand sqlCommand = GetCommand(sql, pars);
            this.SetCommandToAdapter(dataAdapter, sqlCommand);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            if (this.IsClearParameters)
                sqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection && this.Transaction == null) this.Close();
            return ds;
        }
        public virtual object GetScalar(string sql, params SugarParameter[] pars)
        {
            base.SetParamterSize(pars);
            ExecLogEvent(sql, pars, true);
            IDbCommand sqlCommand = GetCommand(sql, pars);
            object scalar = sqlCommand.ExecuteScalar();
            scalar = (scalar == null ? 0 : scalar);
            if (this.IsClearParameters)
                sqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection && this.Transaction == null) this.Close();
            return scalar;
        }
        #endregion

        #region Methods

        public virtual string GetString(string sql, object pars)
        {
            return GetString(sql, this.GetParameters(pars));
        }
        public virtual string GetString(string sql, params SugarParameter[] pars)
        {
            return Convert.ToString(GetScalar(sql, pars));
        }
        public virtual string GetString(string sql, List<SugarParameter> pars)
        {
            if (pars == null)
            {
                return GetString(sql);
            }
            else
            {
                return GetString(sql, pars.ToArray());
            }
        }
        public virtual int GetInt(string sql, object pars)
        {
            return GetInt(sql, this.GetParameters(pars));
        }
        public virtual int GetInt(string sql, params SugarParameter[] pars)
        {
            return GetScalar(sql, pars).ObjToInt();
        }
        public virtual int GetInt(string sql, List<SugarParameter> pars)
        {
            if (pars == null)
            {
                return GetInt(sql);
            }
            else
            {
                return GetInt(sql, pars.ToArray());
            }
        }
        public virtual Double GetDouble(string sql, object pars)
        {
            return GetDouble(sql, this.GetParameters(pars));
        }
        public virtual Double GetDouble(string sql, params SugarParameter[] pars)
        {
            return Convert.ToDouble(GetScalar(sql, pars));
        }
        public virtual Double GetDouble(string sql, List<SugarParameter> pars)
        {
            if (pars == null)
            {
                return GetDouble(sql);
            }
            else
            {
                return GetDouble(sql, pars.ToArray());
            }
        }
        public virtual decimal GetDecimal(string sql, object pars)
        {
            return GetDecimal(sql, this.GetParameters(pars));
        }
        public virtual decimal GetDecimal(string sql, params SugarParameter[] pars)
        {
            return Convert.ToDecimal(GetScalar(sql, pars));
        }
        public virtual decimal GetDecimal(string sql, List<SugarParameter> pars)
        {
            if (pars == null)
            {
                return GetDecimal(sql);
            }
            else
            {
                return GetDecimal(sql, pars.ToArray());
            }
        }
        public virtual DateTime GetDateTime(string sql, object pars)
        {
            return GetDateTime(sql, this.GetParameters(pars));
        }
        public virtual DateTime GetDateTime(string sql, params SugarParameter[] pars)
        {
            return Convert.ToDateTime(GetScalar(sql, pars));
        }
        public virtual DateTime GetDateTime(string sql, List<SugarParameter> pars)
        {
            if (pars == null)
            {
                return GetDateTime(sql);
            }
            else
            {
                return GetDateTime(sql, pars.ToArray());
            }
        }
        public virtual List<T> SqlQuery<T>(string sql, object whereObj = null)
        {
            var parameters = this.GetParameters(whereObj);
            return SqlQuery<T>(sql, parameters);
        }
        public virtual List<T> SqlQuery<T>(string sql, params SugarParameter[] parameters)
        {
            var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parameters != null && parameters.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parameters);
            using (var dataReader = this.GetDataReader(builder.SqlQueryBuilder.ToSqlString(), builder.SqlQueryBuilder.Parameters.ToArray()))
            {
                var reval = this.DbBind.DataReaderToList<T>(typeof(T), dataReader, builder.SqlQueryBuilder.Fields);
                if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection) this.Close();
                builder.SqlQueryBuilder.Clear();
                return reval;
            }
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
        public virtual T SqlQuerySingle<T>(string sql, object whereObj = null)
        {
            var result = SqlQuery<T>(sql, whereObj);
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
            return dt == null ? null : this.Context.RewritableMethods.DataTableToDynamic(dt);
        }
        public virtual dynamic SqlQueryDynamic(string sql, params SugarParameter[] parameters)
        {
            var dt = this.GetDataTable(sql, parameters);
            return dt == null ? null : this.Context.RewritableMethods.DataTableToDynamic(dt);
        }
        public dynamic SqlQueryDynamic(string sql, List<SugarParameter> parameters)
        {
            var dt = this.GetDataTable(sql, parameters);
            return dt == null ? null : this.Context.RewritableMethods.DataTableToDynamic(dt);
        }
        public virtual DataTable GetDataTable(string sql, params SugarParameter[] pars)
        {
            var ds = GetDataSetAll(sql, pars);
            if (ds.Tables.Count != 0 && ds.Tables.Count > 0) return ds.Tables[0];
            return new DataTable();
        }
        public virtual DataTable GetDataTable(string sql, object pars)
        {
            return GetDataTable(sql, this.GetParameters(pars));
        }
        public virtual DataTable GetDataTable(string sql, List<SugarParameter>[] pars)
        {
            if (pars == null)
            {
                return GetDataTable(sql);
            }
            else
            {
                return GetDataTable(sql, pars.ToArray());
            }
        }
        public virtual DataSet GetDataSetAll(string sql, object pars)
        {
            return GetDataSetAll(sql, this.GetParameters(pars));
        }
        public virtual DataSet GetDataSetAll(string sql, List<SugarParameter> pars)
        {
            if (pars == null)
            {
                return GetDataSetAll(sql);
            }
            else
            {
                return GetDataSetAll(sql, pars.ToArray());
            }
        }
        public virtual IDataReader GetDataReader(string sql, object pars)
        {
            return GetDataReader(sql, this.GetParameters(pars));
        }
        public virtual IDataReader GetDataReader(string sql, List<SugarParameter> pars)
        {
            if (pars == null)
            {
                return GetDataReader(sql);
            }
            else
            {
                return GetDataReader(sql, pars.ToArray());
            }
        }
        public virtual object GetScalar(string sql, object pars)
        {
            return GetScalar(sql, this.GetParameters(pars));
        }
        public virtual object GetScalar(string sql, List<SugarParameter> pars)
        {
            if (pars == null)
            {
                return GetScalar(sql);
            }
            else
            {
                return GetScalar(sql, pars.ToArray());
            }
        }
        public virtual int ExecuteCommand(string sql, object pars)
        {
            return ExecuteCommand(sql, GetParameters(pars));
        }
        public virtual int ExecuteCommand(string sql, List<SugarParameter> pars)
        {
            if (pars == null)
            {
                return ExecuteCommand(sql);
            }
            else
            {
                return ExecuteCommand(sql, pars.ToArray());
            }
        }
        #endregion

        #region  Helper

        public virtual void ExecLogEvent(string sql, SugarParameter[] pars, bool isStarting = true)
        {
            if (this.IsEnableLogEvent)
            {
                Action<string, string> action = isStarting ? LogEventStarting : LogEventCompleted;
                if (action != null)
                {
                    if (pars == null || pars.Length == 0)
                    {
                        action(sql, null);
                    }
                    else
                    {
                        action(sql, this.Context.RewritableMethods.SerializeObject(pars.Select(it => new { key = it.ParameterName, value = it.Value.ObjToString() })));
                    }
                }
            }
        }
        public virtual SugarParameter[] GetParameters(object obj, PropertyInfo[] propertyInfo = null)
        {
            if (obj == null) return null;
            return base.GetParameters(obj, propertyInfo, this.SqlParameterKeyWord);
        } 
        #endregion
    }
}
