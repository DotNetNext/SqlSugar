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
    public class SqlHelper : IDisposable
    {
        SqlConnection _sqlConnection;
        SqlTransaction _tran = null;
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
        public SqlHelper(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }
        /// <summary>
        /// 获取当前数据库连接对象
        /// </summary>
        /// <returns></returns>

        public SqlConnection GetConnection()
        {
            return _sqlConnection;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public void BeginTran()
        {
            _tran = _sqlConnection.BeginTransaction();
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="iso">指定事务行为</param>
        public void BeginTran(IsolationLevel iso)
        {
            _tran = _sqlConnection.BeginTransaction(iso);
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="transactionName"></param>
        public void BeginTran(string transactionName)
        {
            _tran = _sqlConnection.BeginTransaction(transactionName);
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="iso">指定事务行为</param>
        /// <param name="transactionName"></param>
        public void BeginTran(IsolationLevel iso, string transactionName)
        {
            _tran = _sqlConnection.BeginTransaction(iso, transactionName);
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            if (_tran != null)
            {
                _tran.Rollback();
                _tran = null;
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
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
        public string GetString(string sql, object pars)
        {
            return GetString(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public string GetString(string sql, params SqlParameter[] pars)
        {
            return Convert.ToString(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public int GetInt(string sql, object pars)
        {
            return GetInt(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public int GetInt(string sql, params SqlParameter[] pars)
        {
            return Convert.ToInt32(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public Double GetDouble(string sql, params SqlParameter[] pars)
        {
            return Convert.ToDouble(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public decimal GetDecimal(string sql, params SqlParameter[] pars)
        {
            return Convert.ToDecimal(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public DateTime GetDateTime(string sql, params SqlParameter[] pars)
        {
            return Convert.ToDateTime(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public object GetScalar(string sql, object pars)
        {
            return GetScalar(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public object GetScalar(string sql, params SqlParameter[] pars)
        {
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
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            object scalar = sqlCommand.ExecuteScalar();
            scalar = (scalar == null ? 0 : scalar);
            if (IsClearParameters)
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
        public int ExecuteCommand(string sql, object pars)
        {
            return ExecuteCommand(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 执行SQL返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public int ExecuteCommand(string sql, params SqlParameter[] pars)
        {
            ExecLogEvent(sql, pars, true);
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            sqlCommand.CommandType = CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            if (pars != null)
                sqlCommand.Parameters.AddRange(pars);
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            int count = sqlCommand.ExecuteNonQuery();
            if (IsClearParameters)
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
        public SqlDataReader GetReader(string sql, object pars)
        {
            return GetReader(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取DataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public SqlDataReader GetReader(string sql, params SqlParameter[] pars)
        {
            ExecLogEvent(sql, pars, true);
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            sqlCommand.CommandType = CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            if (pars != null)
                sqlCommand.Parameters.AddRange(pars);
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            if (IsClearParameters)
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
        public List<T> GetList<T>(string sql, object pars)
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
        public List<T> GetList<T>(string sql, params SqlParameter[] pars)
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
        public T GetSingle<T>(string sql, object pars)
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
        public T GetSingle<T>(string sql, params SqlParameter[] pars)
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
        public DataTable GetDataTable(string sql, object pars)
        {
            return GetDataTable(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, params SqlParameter[] pars)
        {
            ExecLogEvent(sql, pars, true);
            SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            _sqlDataAdapter.SelectCommand.CommandType = CommandType;
            _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(_sqlDataAdapter.SelectCommand.Parameters);
            }
            _sqlDataAdapter.SelectCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            DataTable dt = new DataTable();
            _sqlDataAdapter.Fill(dt);
            if (IsClearParameters)
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
        public DataSet GetDataSetAll(string sql, object pars)
        {
            return GetDataSetAll(sql, SqlSugarTool.GetParameters(pars));
        }
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public DataSet GetDataSetAll(string sql, params SqlParameter[] pars)
        {
            ExecLogEvent(sql, pars, true);
            SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(_sqlDataAdapter.SelectCommand.Parameters);
            }
            _sqlDataAdapter.SelectCommand.CommandTimeout = this.CommandTimeOut;
            _sqlDataAdapter.SelectCommand.CommandType = CommandType;
            _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            DataSet ds = new DataSet();
            _sqlDataAdapter.Fill(ds);
            if (IsClearParameters)
                _sqlDataAdapter.SelectCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return ds;
        }

        private void ExecLogEvent(string sql, SqlParameter[] pars, bool isStarting = true)
        {
            if (IsEnableLogEvent)
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
                        action(sql, JsonConverter.Serialize(pars.Select(it => new { key = it.ParameterName, value = it.Value })));
                    }
                }
            }
        }
        /// <summary>
        /// 释放数据库连接对象
        /// </summary>
        public void Dispose()
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
    }
}
