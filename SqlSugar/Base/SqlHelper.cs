using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：底层SQL辅助函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public abstract class SqlHelper : IDisposable
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        protected SqlConnection _sqlConnection;
        /// <summary>
        /// 事务对象
        /// </summary>
        protected SqlTransaction _tran = null;
        /// <summary>
        /// 如何解释命令字符串 默认为Text 
        /// </summary>
        public CommandType CommandType = CommandType.Text;
        /// <summary>
        /// 是否启用日志事件(默认为:false)
        /// </summary>
        public bool IsEnableLogEvent = false;
        /// <summary>
        /// 执行访数据库前的回调函数  (sql,pars)=>{}
        /// </summary>
        public Action<string, string> LogEventStarting = null;
        /// <summary>
        /// 执行访数据库后的回调函数  (sql,pars)=>{}
        /// </summary>
        public Action<string, string> LogEventCompleted = null;
        /// <summary>
        /// 是否清空SqlParameters
        /// </summary>
        public bool IsClearParameters = true;
        /// <summary>
        /// 设置在终止执行命令的尝试并生成错误之前的等待时间。（单位：秒）
        /// </summary>
        public int CommandTimeOut = 30000;
        /// <summary>
        /// 将页面参数自动填充到SqlParameter []，无需在程序中指定参数
        /// 例如：
        ///     var list = db.Queryable&lt;Student&gt;().Where("id=@id").ToList();
        ///     以前写法
        ///     var list = db.Queryable&lt;Student&gt;().Where("id=@id", new { id=Request["id"] }).ToList();
        /// </summary>
        public bool IsGetPageParas = false;


        /// <summary>
        /// 初始化 SqlHelper 类的新实例
        /// </summary>
        /// <param name="connectionString"></param>
        public  SqlHelper(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 主连接
        /// </summary>
        protected SqlConnection _masterConnection = null;
        /// <summary>
        /// 从连接
        /// </summary>
        protected List<SqlConnection> _slaveConnections = null;
        /// <summary>
        /// 初始化 SqlHelper 类的新实例
        /// </summary>
        /// <param name="masterConnectionString"></param>
        /// <param name="slaveConnectionStrings"></param>
        public  SqlHelper(string masterConnectionString, params string[] slaveConnectionStrings)
        {
            _masterConnection = new SqlConnection(masterConnectionString);
            if (slaveConnectionStrings == null || slaveConnectionStrings.Length == 0)
            {
                _slaveConnections = new List<SqlConnection>()
                {
                    _masterConnection
                };
            }
            else
            {
                _slaveConnections = new List<SqlConnection>();
                foreach (var item in slaveConnectionStrings)
                {
                    _slaveConnections.Add(new SqlConnection(item));
                }
            }
        }
        /// <summary>
        /// 设置当前主从连接对象
        /// </summary>
        /// <param name="isMaster"></param>
        public virtual void SetCurrentConnection(bool isMaster)
        {
            if (_slaveConnections != null && _slaveConnections.Count > 0)//开启主从模式
            {
                if (isMaster || _tran != null)
                {
                    _sqlConnection = _masterConnection;
                }
                else
                {
                    var count = _slaveConnections.Count;
                    _sqlConnection = _slaveConnections[new Random().Next(0, count - 1)];
                }
            }
        }


        /// <summary>
        /// 获取当前数据库连接对象
        /// </summary>
        /// <returns></returns>
        public virtual SqlConnection GetConnection()
        {
            return _sqlConnection;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public virtual void BeginTran()
        {
            SetCurrentConnection(true);
            CheckConnect();
            _tran = _sqlConnection.BeginTransaction();
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="iso">指定事务行为</param>
        public virtual void BeginTran(IsolationLevel iso)
        {
            SetCurrentConnection(true);
            CheckConnect();
            _tran = _sqlConnection.BeginTransaction(iso);
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="transactionName"></param>
        public virtual void BeginTran(string transactionName)
        {
            SetCurrentConnection(true);
            CheckConnect();
            _tran = _sqlConnection.BeginTransaction(transactionName);
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="iso">指定事务行为</param>
        /// <param name="transactionName"></param>
        public virtual void BeginTran(IsolationLevel iso, string transactionName)
        {
            SetCurrentConnection(true);
            CheckConnect();
            _tran = _sqlConnection.BeginTransaction(iso, transactionName);
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public virtual void RollbackTran()
        {
            SetCurrentConnection(true);
            CheckConnect();
            if (_tran != null)
            {
                _tran.Rollback();
                _tran = null;
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public virtual void CommitTran()
        {
            SetCurrentConnection(true);
            CheckConnect();
            if (_tran != null)
            {
                _tran.Commit();
                _tran = null;
            }
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual string GetString(string sql, object pars)
        {
            return GetString(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual string GetString(string sql, params SqlParameter[] pars)
        {
            return Convert.ToString(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual int GetInt(string sql, object pars)
        {
            return GetInt(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual int GetInt(string sql, params SqlParameter[] pars)
        {
            return Convert.ToInt32(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual Double GetDouble(string sql, params SqlParameter[] pars)
        {
            return Convert.ToDouble(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual decimal GetDecimal(string sql, params SqlParameter[] pars)
        {
            return Convert.ToDecimal(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual DateTime GetDateTime(string sql, params SqlParameter[] pars)
        {
            return Convert.ToDateTime(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual object GetScalar(string sql, object pars)
        {
            return GetScalar(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual object GetScalar(string sql, params SqlParameter[] pars)
        {
            SetCurrentConnection(true);
            ExecLogEvent(sql, pars, true);
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            sqlCommand.CommandType = CommandType;
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (pars != null)
                sqlCommand.Parameters.AddRange(pars);
            if (this.IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            CheckConnect();
            object scalar = sqlCommand.ExecuteScalar();
            scalar = (scalar == null ? 0 : scalar);
            if (this.IsClearParameters)
                sqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return scalar;
        }

        /// <summary>
        /// 执行SQL返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual int ExecuteCommand(string sql, object pars)
        {
            return ExecuteCommand(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 执行SQL返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual int ExecuteCommand(string sql, params SqlParameter[] pars)
        {
            SetCurrentConnection(true);
            ExecLogEvent(sql, pars, true);
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            if (pars != null)
                sqlCommand.Parameters.AddRange(pars);
            if (this.IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            CheckConnect();
            int count = sqlCommand.ExecuteNonQuery();
            if (this.IsClearParameters)
                sqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return count;
        }

        /// <summary>
        /// 获取DataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual SqlDataReader GetReader(string sql, object pars)
        {
            return GetReader(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取DataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual SqlDataReader GetReader(string sql, params SqlParameter[] pars)
        {
            SetCurrentConnection(false);
            ExecLogEvent(sql, pars, true);
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            if (pars != null)
                sqlCommand.Parameters.AddRange(pars);
            if (this.IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            CheckConnect();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            if (this.IsClearParameters)
                sqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return sqlDataReader;
        }

        /// <summary>
        /// 根据SQL获取T的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string sql, object pars)
        {
            return GetList<T>(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 根据SQL获取T的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string sql, params SqlParameter[] pars)
        {
            var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), GetReader(sql, pars), null);
            return reval;
        }

        /// <summary>
        /// 根据SQL获取T
        /// </summary>
        /// <typeparam name="T">可以是int、string等，也可以是类或者数组、字典</typeparam>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual T GetSingle<T>(string sql, object pars)
        {
            return GetSingle<T>(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 根据SQL获取T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual T GetSingle<T>(string sql, params SqlParameter[] pars)
        {
            var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), GetReader(sql, pars), null).Single();
            return reval;
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string sql, object pars)
        {
            return GetDataTable(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string sql, params SqlParameter[] pars)
        {
            SetCurrentConnection(false);
            ExecLogEvent(sql, pars, true);
            SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            _sqlDataAdapter.SelectCommand.CommandType = this.CommandType;
            if (pars != null)
                _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            if (this.IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(_sqlDataAdapter.SelectCommand.Parameters);
            }
            _sqlDataAdapter.SelectCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            CheckConnect();
            DataTable dt = new DataTable();
            _sqlDataAdapter.Fill(dt);
            if (this.IsClearParameters)
                _sqlDataAdapter.SelectCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return dt;
        }
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual DataSet GetDataSetAll(string sql, object pars)
        {
            return GetDataSetAll(sql, SqlSugarTool.GetParameters(pars));
        }
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual DataSet GetDataSetAll(string sql, params SqlParameter[] pars)
        {
            SetCurrentConnection(false);
            ExecLogEvent(sql, pars, true);
            SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            if (this.IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(_sqlDataAdapter.SelectCommand.Parameters);
            }
            _sqlDataAdapter.SelectCommand.CommandTimeout = this.CommandTimeOut;
            _sqlDataAdapter.SelectCommand.CommandType = this.CommandType;
            if (pars != null)
                _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            CheckConnect();
            DataSet ds = new DataSet();
            _sqlDataAdapter.Fill(ds);
            if (this.IsClearParameters)
                _sqlDataAdapter.SelectCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return ds;
        }
        /// <summary>
        /// 执行日志
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <param name="isStarting"></param>
        protected virtual void ExecLogEvent(string sql, SqlParameter[] pars, bool isStarting = true)
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
                        action(sql, JsonConverter.Serialize(pars.Select(it => new { key = it.ParameterName, value = it.Value.ObjToString() })));
                    }
                }
            }
        }
        /// <summary>
        /// 释放数据库连接对象
        /// </summary>
        public virtual void Dispose()
        {
            if (_sqlConnection != null)
            {
                if (_sqlConnection.State != ConnectionState.Closed)
                {
                    if (_tran != null)
                        _tran.Commit();
                    _sqlConnection.Close();
                }
                _sqlConnection = null;
            }
        }
        /// <summary>
        /// 检查数据库连接，若未连接，连接数据库
        /// </summary>
        protected virtual void CheckConnect()
        {
            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection.Open();
            }
        }
    }
}
