using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class ScopedClient : ISqlSugarClient, ITenant
    {
        private SqlSugarClient db;
        private ScopedClient()
        {

        }
        public ScopedClient(SqlSugarClient context)
        {
            this.db = context;
        }
        public SqlSugarClient ScopedContext
        {
            get
            {
                var key = db.GetHashCode().ToString();
                SqlSugarClient result = CallContextAsync<SqlSugarClient>.GetData(key);
                if (result == null)
                {
                    CallContextAsync<SqlSugarClient>.SetData(key, new SqlSugarClient(db._allConfigs));
                    result = CallContextAsync<SqlSugarClient>.GetData(key);
                }
                return result;
            }
        }
        public MappingTableList MappingTables { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public MappingColumnList MappingColumns { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IgnoreColumnList IgnoreColumns { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IgnoreColumnList IgnoreInsertColumns { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<string, object> TempItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ConfigQuery ConfigQuery { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsSystemTablesConfig => throw new NotImplementedException();

        public Guid ContextID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ConnectionConfig CurrentConnectionConfig { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IAdo Ado => throw new NotImplementedException();

        public AopProvider Aop => throw new NotImplementedException();

        public ICodeFirst CodeFirst => throw new NotImplementedException();

        public IDbFirst DbFirst => throw new NotImplementedException();

        public IDbMaintenance DbMaintenance => throw new NotImplementedException();

        public EntityMaintenance EntityMaintenance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public QueryFilterProvider QueryFilter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IContextMethods Utilities { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public QueueList Queues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SugarCacheProvider DataCache => throw new NotImplementedException();

        public void AddConnection(ConnectionConfig connection)
        {
            throw new NotImplementedException();
        }

        public void AddQueue(string sql, object parsmeters = null)
        {
            throw new NotImplementedException();
        }

        public void AddQueue(string sql, List<SugarParameter> parsmeters)
        {
            throw new NotImplementedException();
        }

        public void AddQueue(string sql, SugarParameter parsmeter)
        {
            throw new NotImplementedException();
        }

        public void BeginTran()
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(dynamic configId)
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(Func<ConnectionConfig, bool> changeExpression)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void CommitTran()
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Deleteable<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Deleteable<T>(dynamic primaryKeyValue) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Deleteable<T>(dynamic[] primaryKeyValues) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Deleteable<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Deleteable<T>(List<dynamic> pkValue) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Deleteable<T>(List<T> deleteObjs) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Deleteable<T>(T deleteObj) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public SqlSugarProvider GetConnection(dynamic configId)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDate()
        {
            throw new NotImplementedException();
        }

        public SimpleClient<T> GetSimpleClient<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void InitMappingInfo(Type type)
        {
            throw new NotImplementedException();
        }

        public void InitMappingInfo<T>()
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> Insertable<T>(Dictionary<string, object> columnDictionary) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> Insertable<T>(dynamic insertDynamicObject) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> Insertable<T>(List<T> insertObjs) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> Insertable<T>(T insertObj) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> Insertable<T>(T[] insertObjs) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<ExpandoObject> Queryable(string tableName, string shortName)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, JoinQueryInfos>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object[]>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, JoinQueryInfos>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object[]>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, JoinQueryInfos>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, JoinQueryInfos>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, JoinQueryInfos>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, JoinQueryInfos>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, JoinQueryInfos>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, JoinQueryInfos>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, JoinQueryInfos>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, bool>> joinExpression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, JoinQueryInfos>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, bool>> joinExpression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, JoinQueryInfos>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2> Queryable<T, T2>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, Expression<Func<T, T2, bool>> joinExpression)
            where T : class, new()
            where T2 : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2> Queryable<T, T2>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, JoinType joinType, Expression<Func<T, T2, bool>> joinExpression)
            where T : class, new()
            where T2 : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, ISugarQueryable<T3> joinQueryable3, JoinType joinType1, Expression<Func<T, T2, T3, bool>> joinExpression1, JoinType joinType2, Expression<Func<T, T2, T3, bool>> joinExpression2)
            where T : class, new()
            where T2 : class, new()
            where T3 : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> Queryable<T>()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> Queryable<T>(ISugarQueryable<T> queryable) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> Queryable<T>(string shortName)
        {
            throw new NotImplementedException();
        }

        public IReportable<T> Reportable<T>(T data)
        {
            throw new NotImplementedException();
        }

        public IReportable<T> Reportable<T>(List<T> list)
        {
            throw new NotImplementedException();
        }

        public IReportable<T> Reportable<T>(T[] array)
        {
            throw new NotImplementedException();
        }

        public void RollbackTran()
        {
            throw new NotImplementedException();
        }

        public ISaveable<T> Saveable<T>(List<T> saveObjects) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISaveable<T> Saveable<T>(T saveObject) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public int SaveQueues(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>> SaveQueues<T, T2, T3, T4, T5, T6, T7>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>> SaveQueues<T, T2, T3, T4, T5, T6>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>> SaveQueues<T, T2, T3, T4, T5>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Tuple<List<T>, List<T2>, List<T3>, List<T4>> SaveQueues<T, T2, T3, T4>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Tuple<List<T>, List<T2>, List<T3>> SaveQueues<T, T2, T3>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Tuple<List<T>, List<T2>> SaveQueues<T, T2>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public List<T> SaveQueues<T>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveQueuesAsync(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>>> SaveQueuesAsync<T, T2, T3, T4, T5, T6, T7>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>>> SaveQueuesAsync<T, T2, T3, T4, T5, T6>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>>> SaveQueuesAsync<T, T2, T3, T4, T5>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>>> SaveQueuesAsync<T, T2, T3, T4>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<List<T>, List<T2>, List<T3>>> SaveQueuesAsync<T, T2, T3>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<List<T>, List<T2>>> SaveQueuesAsync<T, T2>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> SaveQueuesAsync<T>(bool isTran = true)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> SqlQueryable<T>(string sql) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IStorageable<T> Storageable<T>(List<T> dataList) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IStorageable<T> Storageable<T>(T data) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> Union<T>(List<ISugarQueryable<T>> queryables) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> Union<T>(params ISugarQueryable<T>[] queryables) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> UnionAll<T>(List<ISugarQueryable<T>> queryables) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> UnionAll<T>(params ISugarQueryable<T>[] queryables) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> Updateable<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> Updateable<T>(Dictionary<string, object> columnDictionary) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> Updateable<T>(dynamic updateDynamicObject) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> Updateable<T>(Expression<Func<T, bool>> columns) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> Updateable<T>(Expression<Func<T, T>> columns) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> Updateable<T>(List<T> UpdateObjs) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> Updateable<T>(T UpdateObj) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> Updateable<T>(T[] UpdateObjs) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public DbResult<bool> UseTran(Action action, Action<Exception> errorCallBack = null)
        {
            throw new NotImplementedException();
        }

        public DbResult<T> UseTran<T>(Func<T> action, Action<Exception> errorCallBack = null)
        {
            throw new NotImplementedException();
        }

        public Task<DbResult<bool>> UseTranAsync(Action action, Action<Exception> errorCallBack = null)
        {
            throw new NotImplementedException();
        }

        public Task<DbResult<T>> UseTranAsync<T>(Func<T> action, Action<Exception> errorCallBack = null)
        {
            throw new NotImplementedException();
        }
    }
}
