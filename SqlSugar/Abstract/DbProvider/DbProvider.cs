using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public abstract partial class DbProvider : DbAccessory, IDb
    {
        public DbProvider()
        {
            this.IsEnableLogEvent = false;
            this.CommandType = CommandType.Text;
            this.IsClearParameters = true;
            this.CommandTimeOut = 30000;
        }
        public IDbTransaction Transaction { get; set; }
        public virtual SqlSugarClient Context { get; set; }
        public virtual IConnectionConfig MasterConnectionConfig { get; set; }
        public virtual List<IConnectionConfig> SlaveConnectionConfigs { get; set; }
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
        public virtual IDbFirst DbFirst
        {
            get
            {
                if (base._DbFirst == null)
                {
                    IDbFirst dbFirst = InstanceFactory.GetDbFirst(this.Context.CurrentConnectionConfig);
                    base._DbFirst = dbFirst;
                    dbFirst.Context = this.Context;
                }
                return base._DbFirst;
            }
        }
        public virtual ICodeFirst CodeFirst
        {
            get
            {
                if (base._CodeFirst == null)
                {
                    ICodeFirst codeFirst = InstanceFactory.GetCodeFirst(this.Context.CurrentConnectionConfig);
                    base._CodeFirst = codeFirst;
                    codeFirst.Context = this.Context;
                }
                return base._CodeFirst;
            }
        }
        public virtual IDbMaintenance DbMaintenance
        {
            get
            {
                if (base._DbMaintenance == null)
                {
                    IDbMaintenance maintenance = InstanceFactory.GetDbMaintenance(this.Context.CurrentConnectionConfig);
                    base._DbMaintenance = maintenance;
                    maintenance.Context = this.Context;
                }
                return base._DbMaintenance;
            }
        }

        public virtual int CommandTimeOut { get; set; }
        public virtual CommandType CommandType { get; set; }
        public virtual bool IsEnableLogEvent { get; set; }
        public virtual bool IsClearParameters { get; set; }
        public virtual Action<string, string> LogEventStarting { get; set; }
        public virtual Action<string, string> LogEventCompleted { get; set; }


        public virtual void Close()
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

        public virtual string GetString(string sql, object pars)
        {
            return GetString(sql, this.GetParameters(pars));
        }
        public virtual string GetString(string sql, params SugarParameter[] pars)
        {
            return Convert.ToString(GetScalar(sql, pars));
        }
        public virtual int GetInt(string sql, object pars)
        {
            return GetInt(sql, this.GetParameters(pars));
        }
        public virtual int GetInt(string sql, params SugarParameter[] pars)
        {
            return Convert.ToInt32(GetScalar(sql, pars));
        }
        public virtual Double GetDouble(string sql, params SugarParameter[] pars)
        {
            return Convert.ToDouble(GetScalar(sql, pars));
        }
        public virtual decimal GetDecimal(string sql, params SugarParameter[] pars)
        {
            return Convert.ToDecimal(GetScalar(sql, pars));
        }
        public virtual DateTime GetDateTime(string sql, params SugarParameter[] pars)
        {
            return Convert.ToDateTime(GetScalar(sql, pars));
        }

        public virtual SugarParameter[] GetParameters(object obj, PropertyInfo[] propertyInfo = null)
        {
            if (obj == null) return null;
            return base.GetParameters(obj, propertyInfo,this.Context.SqlBuilder.SqlParameterKeyWord);
        }

        public virtual void BeginTran()
        {
            this.Connection.BeginTransaction();
        }
        public virtual void BeginTran(IsolationLevel iso)
        {
            this.Connection.BeginTransaction(iso);
        }
        public virtual void RollbackTran()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
                this.Transaction = null;
            }
        }
        public virtual void CommitTran()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
                this.Transaction = null;
            }
        }
        public abstract IDataParameter[] ToIDbDataParameter(params SugarParameter[] pars);
        public abstract void SetCommandToAdapter(IDataAdapter adapter, IDbCommand command);
        public abstract IDataAdapter GetAdapter();
        public abstract IDbCommand GetCommand(string sql, SugarParameter[] pars);
        public abstract IDbConnection Connection { get; set; }
        public abstract void BeginTran(string transactionName);//Only SqlServer
        public abstract void BeginTran(IsolationLevel iso, string transactionName);//Only SqlServer

        #region Core
        public virtual int ExecuteCommand(string sql, params SugarParameter[] pars)
        {
            base.SetParSize(pars);
            ExecLogEvent(sql, pars, true);
            IDbCommand sqlCommand = GetCommand(sql, pars);
            int count = sqlCommand.ExecuteNonQuery();
            if (this.IsClearParameters)
                sqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return count;
        }
        public virtual IDataReader GetDataReader(string sql, params SugarParameter[] pars)
        {
            base.SetParSize(pars);
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
            base.SetParSize(pars);
            ExecLogEvent(sql, pars, true);
            IDataAdapter dataAdapter = this.GetAdapter();
            IDbCommand sqlCommand = GetCommand(sql, pars);
            this.SetCommandToAdapter(dataAdapter, sqlCommand);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            if (this.IsClearParameters)
                sqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return ds;
        }
        public virtual object GetScalar(string sql, params SugarParameter[] pars)
        {
            base.SetParSize(pars);
            ExecLogEvent(sql, pars, true);
            IDbCommand sqlCommand = GetCommand(sql, pars);
            object scalar = sqlCommand.ExecuteScalar();
            scalar = (scalar == null ? 0 : scalar);
            if (this.IsClearParameters)
                sqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return scalar;
        }
        #endregion

        public virtual DataTable GetDataTable(string sql, params SugarParameter[] pars)
        {
            var ds = GetDataSetAll(sql, pars);
            if (ds.Tables.Count != 0 && ds.Tables.Count > 0) return ds.Tables[0];
            return new DataTable();
        }
        public DataTable GetDataTable(string sql, object pars)
        {
            return GetDataTable(sql, this.GetParameters(pars));
        }
        public DataSet GetDataSetAll(string sql, object pars)
        {
            return GetDataSetAll(sql, this.GetParameters(pars));
        }
        public IDataReader GetDataReader(string sql, object pars)
        {
            return GetDataReader(sql, this.GetParameters(pars));
        }
        public virtual object GetScalar(string sql, object pars)
        {
            return GetScalar(sql, this.GetParameters(pars));
        }
        public virtual int ExecuteCommand(string sql, object pars)
        {
            return ExecuteCommand(sql, GetParameters(pars));
        }
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
                        action(sql, JsonConvert.SerializeObject(pars.Select(it => new { key = it.ParameterName, value = it.Value.ObjToString() })));
                    }
                }
            }
        }
        public virtual void Open()
        {
            CheckConnection();
        }
    }
}
