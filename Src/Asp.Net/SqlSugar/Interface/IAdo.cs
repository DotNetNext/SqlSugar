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
    public partial interface IAdo
    {
        string SqlParameterKeyWord { get; }
        IDbConnection Connection { get; set; }
        IDbTransaction Transaction { get; set; }
        IDataParameter[] ToIDbDataParameter(params SugarParameter[] pars);
        SugarParameter[] GetParameters(object obj, PropertyInfo[] propertyInfo = null);
        SqlSugarClient Context { get; set; }
        void ExecuteBefore(string sql, SugarParameter[] pars);
        void ExecuteAfter(string sql, SugarParameter[] pars);

        IDataParameterCollection DataReaderParameters { get; set; }
        CommandType CommandType { get; set; }
        bool IsEnableLogEvent { get; set; }
        Action<string, SugarParameter []> LogEventStarting { get; set; }
        Action<string, SugarParameter []> LogEventCompleted { get; set; }
        Func<string, SugarParameter[], KeyValuePair<string, SugarParameter[]>> ProcessingEventStartingSQL { get; set; }
        Action<Exception> ErrorEvent { get; set; }
        bool IsClearParameters { get; set; }
        int CommandTimeOut { get; set; }
        IDbBind DbBind { get; }
        void SetCommandToAdapter(IDataAdapter adapter, IDbCommand command);
        IDataAdapter GetAdapter();
        IDbCommand GetCommand(string sql, SugarParameter[] parameters);
        DataTable GetDataTable(string sql, object parameters);
        DataTable GetDataTable(string sql, params SugarParameter[] parameters);
        DataTable GetDataTable(string sql, List<SugarParameter> parameters);
        DataSet GetDataSetAll(string sql, object parameters);
        DataSet GetDataSetAll(string sql, params SugarParameter[] parameters);
        DataSet GetDataSetAll(string sql, List<SugarParameter> parameters);
        IDataReader GetDataReader(string sql, object parameters);
        IDataReader GetDataReader(string sql, params SugarParameter[] parameters);
        IDataReader GetDataReader(string sql, List<SugarParameter> parameters);
        object GetScalar(string sql, object parameters);
        object GetScalar(string sql, params SugarParameter[] parameters);
        object GetScalar(string sql, List<SugarParameter> parameters);
        int ExecuteCommand(string sql, object parameters);
        int ExecuteCommand(string sql, params SugarParameter[] parameters);
        int ExecuteCommand(string sql, List<SugarParameter> parameters);
        string GetString(string sql, object parameters);
        string GetString(string sql, params SugarParameter[] parameters);
        string GetString(string sql, List<SugarParameter> parameters);
        int GetInt(string sql, object pars);
        int GetInt(string sql, params SugarParameter[] parameters);
        int GetInt(string sql, List<SugarParameter> parameters);
        Double GetDouble(string sql, object parameters);
        Double GetDouble(string sql, params SugarParameter[] parameters);
        Double GetDouble(string sql, List<SugarParameter> parameters);
        decimal GetDecimal(string sql, object parameters);
        decimal GetDecimal(string sql, params SugarParameter[] parameters);
        decimal GetDecimal(string sql, List<SugarParameter> parameters);
        DateTime GetDateTime(string sql, object parameters);
        DateTime GetDateTime(string sql, params SugarParameter[] parameters);
        DateTime GetDateTime(string sql, List<SugarParameter> parameters);
        List<T> SqlQuery<T>(string sql, object parameters = null);
        List<T> SqlQuery<T>(string sql, params SugarParameter[] parameters);
        List<T> SqlQuery<T>(string sql, List<SugarParameter> parameters);
        T SqlQuerySingle<T>(string sql, object whereObj = null);
        T SqlQuerySingle<T>(string sql, params SugarParameter[] parameters);
        T SqlQuerySingle<T>(string sql, List<SugarParameter> parameters);
        dynamic SqlQueryDynamic(string sql, object whereObj = null);
        dynamic SqlQueryDynamic(string sql, params SugarParameter[] parameters);
        dynamic SqlQueryDynamic(string sql, List<SugarParameter> parameters);
        void Dispose();
        void Close();
        void Open();
        void CheckConnection();

        void BeginTran();
        void BeginTran(IsolationLevel iso);
        void BeginTran(string transactionName);
        void BeginTran(IsolationLevel iso, string transactionName);
        void RollbackTran();
        void CommitTran();

        DbResult<bool> UseTran(Action action);
        DbResult<T> UseTran<T>(Func<T> action);
        void UseStoredProcedure(Action action);
        T UseStoredProcedure<T>(Func<T> action);
        IAdo UseStoredProcedure();
    }
}
