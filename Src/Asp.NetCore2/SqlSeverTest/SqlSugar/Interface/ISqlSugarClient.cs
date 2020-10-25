using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface ISqlSugarClient: IDisposable
    {
        MappingTableList MappingTables { get; set; }
        MappingColumnList MappingColumns { get; set; }
        IgnoreColumnList IgnoreColumns { get; set; }
        IgnoreColumnList IgnoreInsertColumns { get; set; }
        Dictionary<string, object> TempItems { get; set; }


        bool IsSystemTablesConfig { get; }
        Guid ContextID { get; set; }
        ConnectionConfig CurrentConnectionConfig { get; set; }


        IAdo Ado { get; }
        AopProvider Aop { get; }
        ICodeFirst CodeFirst { get; }

 
        IDbFirst DbFirst { get; }
        IDbMaintenance DbMaintenance { get; }
        EntityMaintenance EntityMaintenance { get; set; }
        QueryFilterProvider QueryFilter { get; set; }
        IContextMethods Utilities { get; set; }


        #region Deleteable
        IDeleteable<T> Deleteable<T>() where T : class, new();
        IDeleteable<T> Deleteable<T>(dynamic primaryKeyValue) where T : class, new();
        IDeleteable<T> Deleteable<T>(dynamic[] primaryKeyValues) where T : class, new();
        IDeleteable<T> Deleteable<T>(Expression<Func<T, bool>> expression) where T : class, new();
        IDeleteable<T> Deleteable<T>(List<dynamic> pkValue) where T : class, new();
        IDeleteable<T> Deleteable<T>(List<T> deleteObjs) where T : class, new();
        IDeleteable<T> Deleteable<T>(T deleteObj) where T : class, new();
        #endregion

        #region Other methods
        DateTime GetDate();
        //SimpleClient GetSimpleClient();
        SimpleClient<T> GetSimpleClient<T>() where T : class, new();
        void InitMappingInfo(Type type);
        void InitMappingInfo<T>();
        void Open();
        void Close(); 
        #endregion

        #region Insertable
        IInsertable<T> Insertable<T>(Dictionary<string, object> columnDictionary) where T : class, new();
        IInsertable<T> Insertable<T>(dynamic insertDynamicObject) where T : class, new();
        IInsertable<T> Insertable<T>(List<T> insertObjs) where T : class, new();
        IInsertable<T> Insertable<T>(T insertObj) where T : class, new();
        IInsertable<T> Insertable<T>(T[] insertObjs) where T : class, new();
        #endregion

        #region Queryable
        ISugarQueryable<T> SqlQueryable<T>(string sql) where T : class, new();
        ISugarQueryable<ExpandoObject> Queryable(string tableName, string shortName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression) where T : class, new();
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, JoinQueryInfos>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object[]>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression) where T : class, new();
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, JoinQueryInfos>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object[]>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression) where T : class, new();
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, JoinQueryInfos>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression) where T : class, new();
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, JoinQueryInfos>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression) where T : class, new();
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, JoinQueryInfos>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression) where T : class, new();
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, JoinQueryInfos>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression) where T : class, new();
        ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, JoinQueryInfos>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression) where T : class, new();
        ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, JoinQueryInfos>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression);
        ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression) where T : class, new();
        ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, JoinQueryInfos>> joinExpression);
        ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression);
        ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, bool>> joinExpression) where T : class, new();
        ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, JoinQueryInfos>> joinExpression);
        ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression);
        ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, bool>> joinExpression) where T : class, new();
        ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, JoinQueryInfos>> joinExpression);
        ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression);
        ISugarQueryable<T, T2> Queryable<T, T2>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, Expression<Func<T, T2, bool>> joinExpression)
            where T : class, new()
            where T2 : class, new();
        ISugarQueryable<T, T2> Queryable<T, T2>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, JoinType joinType, Expression<Func<T, T2, bool>> joinExpression)
            where T : class, new()
            where T2 : class, new();
        ISugarQueryable<T> Queryable<T>();
        ISugarQueryable<T> Queryable<T>(ISugarQueryable<T> queryable) where T : class, new();
        ISugarQueryable<T> Queryable<T>(string shortName);
        #endregion

        #region Saveable
        ISaveable<T> Saveable<T>(List<T> saveObjects) where T : class, new();
        ISaveable<T> Saveable<T>(T saveObject) where T : class, new();
        #endregion

        #region Queue
        QueueList Queues { get; set; }
        void AddQueue(string sql, object parsmeters = null);
        void AddQueue(string sql, List<SugarParameter> parsmeters);
        void AddQueue(string sql, SugarParameter parsmeter);
        int SaveQueues(bool isTran = true);
        Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>> SaveQueues<T, T2, T3, T4, T5, T6, T7>(bool isTran = true);
        Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>> SaveQueues<T, T2, T3, T4, T5, T6>(bool isTran = true);
        Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>> SaveQueues<T, T2, T3, T4, T5>(bool isTran = true);
        Tuple<List<T>, List<T2>, List<T3>, List<T4>> SaveQueues<T, T2, T3, T4>(bool isTran = true);
        Tuple<List<T>, List<T2>, List<T3>> SaveQueues<T, T2, T3>(bool isTran = true);
        Tuple<List<T>, List<T2>> SaveQueues<T, T2>(bool isTran = true);
        List<T> SaveQueues<T>(bool isTran = true);
        Task<int> SaveQueuesAsync(bool isTran = true);
        Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>>> SaveQueuesAsync<T, T2, T3, T4, T5, T6, T7>(bool isTran = true);
        Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>>> SaveQueuesAsync<T, T2, T3, T4, T5, T6>(bool isTran = true);
        Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>>> SaveQueuesAsync<T, T2, T3, T4, T5>(bool isTran = true);
        Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>>> SaveQueuesAsync<T, T2, T3, T4>(bool isTran = true);
        Task<Tuple<List<T>, List<T2>, List<T3>>> SaveQueuesAsync<T, T2, T3>(bool isTran = true);
        Task<Tuple<List<T>, List<T2>>> SaveQueuesAsync<T, T2>(bool isTran = true);
        Task<List<T>> SaveQueuesAsync<T>(bool isTran = true); 
        #endregion

        #region Union 
        ISugarQueryable<T> Union<T>(List<ISugarQueryable<T>> queryables) where T : class, new();
        ISugarQueryable<T> Union<T>(params ISugarQueryable<T>[] queryables) where T : class, new();
        ISugarQueryable<T> UnionAll<T>(List<ISugarQueryable<T>> queryables) where T : class, new();
        ISugarQueryable<T> UnionAll<T>(params ISugarQueryable<T>[] queryables) where T : class, new();
        #endregion

        #region Updateable
        IUpdateable<T> Updateable<T>() where T : class, new();
        IUpdateable<T> Updateable<T>(Dictionary<string, object> columnDictionary) where T : class, new();
        IUpdateable<T> Updateable<T>(dynamic updateDynamicObject) where T : class, new();
        IUpdateable<T> Updateable<T>(Expression<Func<T, bool>> columns) where T : class, new();
        IUpdateable<T> Updateable<T>(Expression<Func<T, T>> columns) where T : class, new();
        IUpdateable<T> Updateable<T>(List<T> UpdateObjs) where T : class, new();
        IUpdateable<T> Updateable<T>(T UpdateObj) where T : class, new();
        IUpdateable<T> Updateable<T>(T[] UpdateObjs) where T : class, new(); 
        #endregion

        #region Obsolete
        [Obsolete("use Utilities")]
        IContextMethods RewritableMethods { get; set; }
        [Obsolete("use GetSimpleClient()")]
        SimpleClient SimpleClient { get; }
        [Obsolete("use EntityMaintenance")]
        EntityMaintenance EntityProvider { get; set; }
        #endregion
    }
}