﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface ISqlSugarClient : IDisposable
    {
        MappingTableList MappingTables { get; set; }
        MappingColumnList MappingColumns { get; set; }
        IgnoreColumnList IgnoreColumns { get; set; }
        IgnoreColumnList IgnoreInsertColumns { get; set; }
        Dictionary<string, object> TempItems { get; set; }
        ConfigQuery ConfigQuery { get; set; }

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
        SugarActionType SugarActionType { get; set; }

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
        void Tracking<T>(T  data) where T : class, new();
        void Tracking<T>(List<T> data) where T : class, new();
        SqlSugarClient CopyNew();
        T CreateContext<T>(bool isTran=true) where T : SugarUnitOfWork, new();
        SugarUnitOfWork CreateContext(bool isTran = true);
        SplitTableContext SplitHelper(Type entityType);
        SplitTableContext SplitHelper<T>() where T : class, new();
        SplitTableContextResult<T> SplitHelper<T>(T data) where T : class, new();
        SplitTableContextResult<T> SplitHelper<T>(List<T> data) where T : class, new();
        DateTime GetDate();
        //SimpleClient GetSimpleClient();
        SimpleClient<T> GetSimpleClient<T>() where T : class, new();
        RepositoryType GetRepository<RepositoryType>() where RepositoryType : ISugarRepository, new();
        void InitMappingInfo(Type type);
        void InitMappingInfo<T>();
        void Open();
        void Close();
        ITenant AsTenant();
        #endregion

        #region Insertable
        IInsertable<T> Insertable<T>(Dictionary<string, object> columnDictionary) where T : class, new();
        IInsertable<T> Insertable<T>(dynamic insertDynamicObject) where T : class, new();
        IInsertable<T> Insertable<T>(List<T> insertObjs) where T : class, new();
        IInsertable<T> Insertable<T>(T insertObj) where T : class, new();
        IInsertable<T> Insertable<T>(T[] insertObjs) where T : class, new();
        InsertMethodInfo InsertableByObject(object singleEntityObjectOrListObject);
        IInsertable<Dictionary<string, object>> InsertableByDynamic(object insertDynamicObject);
        #endregion

        #region Queryable
        ISugarQueryable<T> MasterQueryable<T>();
        ISugarQueryable<T> SlaveQueryable<T>();
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

        ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, ISugarQueryable<T3> joinQueryable3,
          JoinType joinType1, Expression<Func<T, T2, T3, bool>> joinExpression1,
          JoinType joinType2, Expression<Func<T, T2, T3, bool>> joinExpression2)
           where T : class, new()
           where T2 : class, new()
           where T3 : class, new();

        ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, ISugarQueryable<T3> joinQueryable3, ISugarQueryable<T4> joinQueryable4,
            JoinType joinType1, Expression<Func<T, T2, T3, T4, bool>> joinExpression1,
            JoinType joinType2, Expression<Func<T, T2, T3, T4, bool>> joinExpression2,
            JoinType joinType3, Expression<Func<T, T2, T3, T4, bool>> joinExpression4)
             where T : class, new()
             where T2 : class, new()
             where T3 : class, new()
             where T4 : class, new();
        ISugarQueryable<T> Queryable<T>();
        ISugarQueryable<T> Queryable<T>(ISugarQueryable<T> queryable);  
        ISugarQueryable<T> Queryable<T>(string shortName);
        #endregion

        #region Saveable
         StorageableDataTable Storageable(List<Dictionary<string, object>> dictionaryList, string tableName);
         StorageableDataTable Storageable(Dictionary<string, object> dictionary, string tableName);
        IStorageable<T> Storageable<T>(List<T> dataList) where T : class, new();
        IStorageable<T> Storageable<T>(T data) where T : class, new();
        StorageableDataTable Storageable(DataTable data);
        [Obsolete("use Storageable")]
        ISaveable<T> Saveable<T>(List<T> saveObjects) where T : class, new();
        [Obsolete("use Storageable")]
        ISaveable<T> Saveable<T>(T saveObject) where T : class, new();

        StorageableMethodInfo StorageableByObject(object singleEntityObjectOrListObject);

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
        IUpdateable<Dictionary<string, object>> UpdateableByDynamic(object updateDynamicObject);
        #endregion

        #region Reportable
        IReportable<T> Reportable<T>(T data);
        IReportable<T> Reportable<T>(List<T> list);
        IReportable<T> Reportable<T>(T[] array);
        #endregion

        #region Cache
        SugarCacheProvider DataCache { get; }
        #endregion

        #region Fastest
        IFastest<T> Fastest<T>() where T : class, new();
        #endregion

        #region ThenMapper
        void ThenMapper<T>(IEnumerable<T> list, Action<T> action);
        Task ThenMapperAsync<T>(IEnumerable<T> list, Func<T,Task> action);
        #endregion

        #region  Nav CUD
        InsertNavTaskInit<T, T> InsertNav<T>(T data) where T : class, new();
        InsertNavTaskInit<T, T> InsertNav<T>(List<T> datas) where T : class, new();
        InsertNavTaskInit<T, T> InsertNav<T>(T data,InsertNavRootOptions rootOptions) where T : class, new();
        InsertNavTaskInit<T, T> InsertNav<T>(List<T> datas, InsertNavRootOptions rootOptions) where T : class, new();
        DeleteNavTaskInit<T, T> DeleteNav<T>(T data) where T : class, new();
        DeleteNavTaskInit<T, T> DeleteNav<T>(List<T> datas) where T : class, new();
        DeleteNavTaskInit<T, T> DeleteNav<T>(Expression<Func<T,bool>> whereExpression) where T : class, new();
        UpdateNavTaskInit<T, T> UpdateNav<T>(T data) where T : class, new ();
        UpdateNavTaskInit<T, T> UpdateNav<T>(List<T> datas) where T : class, new ();
        UpdateNavTaskInit<T, T> UpdateNav<T>(T data,UpdateNavRootOptions rootOptions) where T : class, new();
        UpdateNavTaskInit<T, T> UpdateNav<T>(List<T> datas, UpdateNavRootOptions rootOptions) where T : class, new();
        #endregion

    }
}