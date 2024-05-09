using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial interface IInsertable<T> where T :class,new()
    {
        InsertBuilder InsertBuilder { get; set; }
        int ExecuteCommand();
        Task<int> ExecuteCommandAsync();
        Task<int> ExecuteCommandAsync(CancellationToken token);
        List<Type> ExecuteReturnPkList<Type>();
        Task<List<Type>> ExecuteReturnPkListAsync<Type>();
        long ExecuteReturnSnowflakeId();
        List<long> ExecuteReturnSnowflakeIdList();
        Task<long> ExecuteReturnSnowflakeIdAsync();
        Task<long> ExecuteReturnSnowflakeIdAsync(CancellationToken token);
        Task<List<long>> ExecuteReturnSnowflakeIdListAsync();
        Task<List<long>> ExecuteReturnSnowflakeIdListAsync(CancellationToken token);
        int ExecuteReturnIdentity();
        Task<int> ExecuteReturnIdentityAsync();
        Task<int> ExecuteReturnIdentityAsync(CancellationToken token);
        T ExecuteReturnEntity();
        T ExecuteReturnEntity(bool isIncludesAllFirstLayer);
        Task<T> ExecuteReturnEntityAsync();
        Task<T> ExecuteReturnEntityAsync(bool isIncludesAllFirstLayer);
        bool ExecuteCommandIdentityIntoEntity();
        Task<bool> ExecuteCommandIdentityIntoEntityAsync();
        long ExecuteReturnBigIdentity();
        Task<long> ExecuteReturnBigIdentityAsync();
        Task<long> ExecuteReturnBigIdentityAsync(CancellationToken token);
        IInsertable<T> AS(string tableName);
        IInsertable<T> AsType(Type tableNameType);
        IInsertable<T> With(string lockString);
        IInsertable<T> InsertColumns(Expression<Func<T, object>> columns);
        IInsertable<T> InsertColumns(params string[] columns);

        IInsertable<T> IgnoreColumns(Expression<Func<T, object>> columns);
        IInsertable<T> IgnoreColumns(params string[]columns);
        IInsertable<T> IgnoreColumns(bool ignoreNullColumn, bool isOffIdentity = false);
        IInsertable<T> IgnoreColumnsNull(bool isIgnoreNull = true);

        ISubInsertable<T> AddSubList(Expression<Func<T, object>> subForeignKey);
        ISubInsertable<T> AddSubList(Expression<Func<T, SubInsertTree>> tree);
        IParameterInsertable<T> UseParameter();
        IInsertable<T> CallEntityMethod(Expression<Action<T>> method);

        IInsertable<T> EnableDiffLogEvent(object businessData = null);
        IInsertable<T> EnableDiffLogEventIF(bool isDiffLogEvent, object businessData=null);
        IInsertable<T> RemoveDataCache();
        IInsertable<T> RemoveDataCache(string likeString);
        KeyValuePair<string, List<SugarParameter>> ToSql();
        string ToSqlString();
        SqlServerBlukCopy UseSqlServer();
        MySqlBlukCopy<T> UseMySql();
        OracleBlukCopy UseOracle();

        SplitInsertable<T> SplitTable();
        SplitInsertable<T> SplitTable(SplitType splitType);
        void AddQueue();
        IInsertable<T> MySqlIgnore();
        IInsertable<T> OffIdentity();
        IInsertable<T> OffIdentity(bool isSetOn);
        InsertablePage<T> PageSize(int pageSize);
    }
}
