using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SqlSugarScopeProvider:ISqlSugarClient
    {
        internal  SqlSugarProvider conn;
        StackFrame[] frames;

        public SqlSugarScopeProvider(SqlSugarProvider conn)
        {
            this.conn = conn;
            var key = GetKey();
            this.GetContext(true);
        }
        public SqlSugarProvider ScopedContext { get { return GetContext(); } }
        private SqlSugarProvider GetAsyncContext(bool isInit=false)
        {
            if (isInit)
            {
                CallContextAsync<SqlSugarProvider>.SetData(GetKey(), this.conn);
                isInit = false;
                return conn;
            }
            else
            {
                SqlSugarProvider result = CallContextAsync<SqlSugarProvider>.GetData(GetKey());
                if (result == null)
                {
                    SqlSugarProvider db=new SqlSugarProvider(this.conn.CurrentConnectionConfig);
                    CallContextAsync<SqlSugarProvider>.SetData(GetKey(), db);
                    return db;
                }
                else
                {

                    return result;
                }
            }
        }
        private SqlSugarProvider GetThreadContext(bool isInit = false)
        {
            if (isInit)
            {
                CallContextThread<SqlSugarProvider>.SetData(GetKey(), this.conn);
                isInit = false;
                return conn;
            }
            else
            {
                SqlSugarProvider result = CallContextThread<SqlSugarProvider>.GetData(GetKey());
                if (result == null)
                {
                    SqlSugarProvider db = new SqlSugarProvider(this.conn.CurrentConnectionConfig);
                    CallContextThread<SqlSugarProvider>.SetData(GetKey(), db);
                    return db;
                }
                else
                {

                    return result;
                }
            }
        }
        protected virtual SqlSugarProvider GetContext(bool isInit = false)
        {
            SqlSugarProvider result = null;
            var key = GetKey(); ;
            StackTrace st = new StackTrace(true);
            var methods = st.GetFrames();
            var isAsync = UtilMethods.IsAnyAsyncMethod(methods);
            if (isAsync)
            {
                result= GetAsyncContext(isInit);
            }
            else
            {
                result= GetThreadContext(isInit);
            }
            return result;
        }
        private  dynamic GetKey()
        {
            var key= "SqlSugarProviderScope_" + conn.CurrentConnectionConfig.ConfigId;
            if (frames == null)
            {
                frames = new StackTrace(true).GetFrames();
            }
            if (frames.Length >= 0)
            {
                foreach (var method in frames.Take(15))
                {
                    var refType = method.GetMethod()?.ReflectedType;
                    if (refType != null)
                    {
                        var getInterfaces = refType.Name.StartsWith("<") ? refType?.ReflectedType?.GetInterfaces() : refType?.GetInterfaces();
                        if (getInterfaces != null && getInterfaces.Any(it => it.Name.IsIn("IJob")))
                        {
                            key = $"{key}IJob";
                            break;
                        }
                    }
                }
            }
            return key;
        }

        #region  API
        public Task<SugarAsyncLock> AsyncLock(int timeOutSeconds = 30)
        {
            return ScopedContext.AsyncLock(timeOutSeconds);
        }
        public SugarActionType SugarActionType { get => ScopedContext.SugarActionType; set => ScopedContext.SugarActionType = value; }
        public MappingTableList MappingTables { get => ScopedContext.MappingTables; set => ScopedContext.MappingTables = value; }
        public MappingColumnList MappingColumns { get => ScopedContext.MappingColumns; set => ScopedContext.MappingColumns = value; }
        public IgnoreColumnList IgnoreColumns { get => ScopedContext.IgnoreColumns; set => ScopedContext.IgnoreColumns = value; }
        public IgnoreColumnList IgnoreInsertColumns { get => ScopedContext.IgnoreInsertColumns; set => ScopedContext.IgnoreInsertColumns = value; }
        public Dictionary<string, object> TempItems { get => ScopedContext.TempItems; set => ScopedContext.TempItems = value; }
        public ConfigQuery ConfigQuery { get => ScopedContext.ConfigQuery; set => ScopedContext.ConfigQuery = value; }

        public bool IsSystemTablesConfig => ScopedContext.IsSystemTablesConfig;

        public Guid ContextID { get => ScopedContext.ContextID; set => ScopedContext.ContextID = value; }
        public ConnectionConfig CurrentConnectionConfig { get => ScopedContext.CurrentConnectionConfig; set => ScopedContext.CurrentConnectionConfig = value; }

        public IAdo Ado => ScopedContext.Ado;

        public AopProvider Aop => ScopedContext.Aop;

        public ICodeFirst CodeFirst => ScopedContext.CodeFirst;

        public IDbFirst DbFirst => ScopedContext.DbFirst;

        public IDbMaintenance DbMaintenance => ScopedContext.DbMaintenance;

        public EntityMaintenance EntityMaintenance { get => ScopedContext.EntityMaintenance; set => ScopedContext.EntityMaintenance = value; }
        public QueryFilterProvider QueryFilter { get => ScopedContext.QueryFilter; set => ScopedContext.QueryFilter = value; }
        public IContextMethods Utilities { get => ScopedContext.Utilities; set => ScopedContext.Utilities = value; }
        public QueueList Queues { get => ScopedContext.Queues; set => ScopedContext.Queues = value; }

        public SugarCacheProvider DataCache => ScopedContext.DataCache;

         
        public void AddQueue(string sql, object parsmeters = null)
        {
            ScopedContext.AddQueue(sql, parsmeters);
        }

        public void AddQueue(string sql, List<SugarParameter> parsmeters)
        {
            ScopedContext.AddQueue(sql, parsmeters);
        }

        public void AddQueue(string sql, SugarParameter parsmeter)
        {
            ScopedContext.AddQueue(sql, parsmeter);
        }

      
        public void Close()
        {
            ScopedContext.Close();
        }
        public DeleteMethodInfo DeleteableByObject(object singleEntityObjectOrListObject)
        {
            return ScopedContext.DeleteableByObject(singleEntityObjectOrListObject);
        }
        public IDeleteable<T> Deleteable<T>() where T : class, new()
        {
            return ScopedContext.Deleteable<T>();
        }

        public IDeleteable<T> Deleteable<T>(dynamic primaryKeyValue) where T : class, new()
        {
            return ScopedContext.Deleteable<T>(primaryKeyValue);
        }

        public IDeleteable<T> Deleteable<T>(dynamic[] primaryKeyValues) where T : class, new()
        {
            return ScopedContext.Deleteable<T>(primaryKeyValues);
        }

        public IDeleteable<T> Deleteable<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return ScopedContext.Deleteable(expression);
        }

        public IDeleteable<T> Deleteable<T>(List<dynamic> pkValue) where T : class, new()
        {
            return ScopedContext.Deleteable<T>(pkValue);
        }

        public IDeleteable<T> Deleteable<T>(List<T> deleteObjs) where T : class, new()
        {
            return ScopedContext.Deleteable(deleteObjs);
        }

        public IDeleteable<T> Deleteable<T>(T deleteObj) where T : class, new()
        {
            return ScopedContext.Deleteable(deleteObj);
        }

        public void Dispose()
        {
            ScopedContext.Dispose();
        }
 
        public DateTime GetDate()
        {
            return ScopedContext.GetDate();
        }
        public T CreateContext<T>(bool isTran) where T : SugarUnitOfWork, new()
        {
            Check.ExceptionEasy(" var childDb=Db.GetConnection(configId);  use Db.CreateContext ", " 例如 var childDb=Db.GetConnection(configId);其中Db才能使用CreateContext，childDb不能使用");
            return null;
        }
        public SugarUnitOfWork CreateContext(bool isTran = true)
        {
            Check.ExceptionEasy(" var childDb=Db.GetConnection(configId);  use Db.CreateContext ", " 例如 var childDb=Db.GetConnection(configId);其中Db才能使用CreateContext，childDb不能使用");
            return null;
        }
        public SimpleClient<T> GetSimpleClient<T>() where T : class, new()
        {
            return ScopedContext.GetSimpleClient<T>();
        }
        public RepositoryType GetRepository<RepositoryType>() where RepositoryType : ISugarRepository, new()
        {
            return ScopedContext.GetRepository<RepositoryType>();
        }
        public void InitMappingInfo(Type type)
        {
            ScopedContext.InitMappingInfo(type);
        }

        public void InitMappingInfo<T>()
        {
            ScopedContext.InitMappingInfo<T>();
        }
        public IInsertable<Dictionary<string, object>> InsertableByDynamic(object insertDynamicObject)
        {
            return ScopedContext.InsertableByDynamic(insertDynamicObject);
        }
        public InsertMethodInfo InsertableByObject(object singleEntityObjectOrListObject)
        {
            return ScopedContext.InsertableByObject(singleEntityObjectOrListObject);
        }
        public IInsertable<T> Insertable<T>(Dictionary<string, object> columnDictionary) where T : class, new()
        {
            return ScopedContext.Insertable<T>(columnDictionary);
        }

        public IInsertable<T> Insertable<T>(dynamic insertDynamicObject) where T : class, new()
        {
            return ScopedContext.Insertable<T>((object)insertDynamicObject);
        }

        public IInsertable<T> Insertable<T>(List<T> insertObjs) where T : class, new()
        {
            return ScopedContext.Insertable(insertObjs);
        }

        public IInsertable<T> Insertable<T>(T insertObj) where T : class, new()
        {
            return ScopedContext.Insertable(insertObj);
        }

        public IInsertable<T> Insertable<T>(T[] insertObjs) where T : class, new()
        {
            return ScopedContext.Insertable(insertObjs);
        }

        public void Open()
        {
            ScopedContext.Open();
        }
        public ISugarQueryable<T> SlaveQueryable<T>() 
        {
            return ScopedContext.SlaveQueryable<T>();
        }
        public ISugarQueryable<T> MasterQueryable<T>()
        {
            return ScopedContext.MasterQueryable<T>();
        }
        public ISugarQueryable<ExpandoObject> Queryable(string tableName, string shortName)
        {
            return ScopedContext.Queryable(tableName, shortName);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression) where T : class, new()
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, JoinQueryInfos>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object[]>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression) where T : class, new()
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, JoinQueryInfos>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object[]>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression) where T : class, new()
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, JoinQueryInfos>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression) where T : class, new()
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, JoinQueryInfos>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression) where T : class, new()
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, JoinQueryInfos>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression) where T : class, new()
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, JoinQueryInfos>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression) where T : class, new()
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, JoinQueryInfos>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression) where T : class, new()
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, JoinQueryInfos>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression) where T : class, new()
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, JoinQueryInfos>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, bool>> joinExpression) where T : class, new()
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, JoinQueryInfos>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, bool>> joinExpression) where T : class, new()
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, JoinQueryInfos>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression)
        {
            return ScopedContext.Queryable(joinExpression);
        }

        public ISugarQueryable<T, T2> Queryable<T, T2>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, Expression<Func<T, T2, bool>> joinExpression)
            where T : class, new()
            where T2 : class, new()
        {
            return ScopedContext.Queryable(joinQueryable1, joinQueryable2, joinExpression);
        }

        public ISugarQueryable<T, T2> Queryable<T, T2>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, JoinType joinType, Expression<Func<T, T2, bool>> joinExpression)
            where T : class, new()
            where T2 : class, new()
        {
            return ScopedContext.Queryable(joinQueryable1, joinQueryable2, joinType, joinExpression);
        }

        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, ISugarQueryable<T3> joinQueryable3, JoinType joinType1, Expression<Func<T, T2, T3, bool>> joinExpression1, JoinType joinType2, Expression<Func<T, T2, T3, bool>> joinExpression2)
            where T : class, new()
            where T2 : class, new()
            where T3 : class, new()
        {
            return ScopedContext.Queryable(joinQueryable1, joinQueryable2, joinQueryable3, joinType1, joinExpression1, joinType2, joinExpression2);
        }
        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, ISugarQueryable<T3> joinQueryable3, ISugarQueryable<T4> joinQueryable4, JoinType joinType1, Expression<Func<T, T2, T3, T4, bool>> joinExpression1, JoinType joinType2, Expression<Func<T, T2, T3, T4, bool>> joinExpression2, JoinType joinType3, Expression<Func<T, T2, T3, T4, bool>> joinExpression3)
            where T : class, new()
            where T2 : class, new()
            where T3 : class, new()
             where T4 : class, new()
        {
            return ScopedContext.Queryable(joinQueryable1, joinQueryable2, joinQueryable3, joinQueryable4, joinType1, joinExpression1, joinType2, joinExpression2, joinType3, joinExpression3);
        }
        public ISugarQueryable<T> Queryable<T>()
        {
            return ScopedContext.Queryable<T>();
        }

        public ISugarQueryable<T> Queryable<T>(ISugarQueryable<T> queryable)  
        {
            return ScopedContext.Queryable(queryable);
        }

        public ISugarQueryable<T> Queryable<T>(string shortName)
        {
            return ScopedContext.Queryable<T>(shortName);
        }

        public IReportable<T> Reportable<T>(T data)
        {
            return ScopedContext.Reportable(data);
        }

        public IReportable<T> Reportable<T>(List<T> list)
        {
            return ScopedContext.Reportable(list);
        }

        public IReportable<T> Reportable<T>(T[] array)
        {
            return ScopedContext.Reportable(array);
        }

        public int SaveQueues(bool isTran = true)
        {
            return ScopedContext.SaveQueues(isTran);
        }

        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>> SaveQueues<T, T2, T3, T4, T5, T6, T7>(bool isTran = true)
        {
            return ScopedContext.SaveQueues<T, T2, T3, T4, T5, T6, T7>(isTran);
        }

        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>> SaveQueues<T, T2, T3, T4, T5, T6>(bool isTran = true)
        {
            return ScopedContext.SaveQueues<T, T2, T3, T4, T5, T6>(isTran);
        }

        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>> SaveQueues<T, T2, T3, T4, T5>(bool isTran = true)
        {
            return ScopedContext.SaveQueues<T, T2, T3, T4, T5>(isTran);
        }

        public Tuple<List<T>, List<T2>, List<T3>, List<T4>> SaveQueues<T, T2, T3, T4>(bool isTran = true)
        {
            return ScopedContext.SaveQueues<T, T2, T3, T4>(isTran);
        }

        public Tuple<List<T>, List<T2>, List<T3>> SaveQueues<T, T2, T3>(bool isTran = true)
        {
            return ScopedContext.SaveQueues<T, T2, T3>(isTran);
        }

        public Tuple<List<T>, List<T2>> SaveQueues<T, T2>(bool isTran = true)
        {
            return ScopedContext.SaveQueues<T, T2>(isTran);
        }

        public List<T> SaveQueues<T>(bool isTran = true)
        {
            return ScopedContext.SaveQueues<T>(isTran);
        }

        public Task<int> SaveQueuesAsync(bool isTran = true)
        {
            return ScopedContext.SaveQueuesAsync(isTran);
        }

        public Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>>> SaveQueuesAsync<T, T2, T3, T4, T5, T6, T7>(bool isTran = true)
        {
            return ScopedContext.SaveQueuesAsync<T, T2, T3, T4, T5, T6, T7>(isTran);
        }

        public Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>>> SaveQueuesAsync<T, T2, T3, T4, T5, T6>(bool isTran = true)
        {
            return ScopedContext.SaveQueuesAsync<T, T2, T3, T4, T5, T6>(isTran);
        }

        public Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>>> SaveQueuesAsync<T, T2, T3, T4, T5>(bool isTran = true)
        {
            return ScopedContext.SaveQueuesAsync<T, T2, T3, T4, T5>(isTran);
        }

        public Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>>> SaveQueuesAsync<T, T2, T3, T4>(bool isTran = true)
        {
            return ScopedContext.SaveQueuesAsync<T, T2, T3, T4>(isTran);
        }

        public Task<Tuple<List<T>, List<T2>, List<T3>>> SaveQueuesAsync<T, T2, T3>(bool isTran = true)
        {
            return ScopedContext.SaveQueuesAsync<T, T2, T3>(isTran);
        }

        public Task<Tuple<List<T>, List<T2>>> SaveQueuesAsync<T, T2>(bool isTran = true)
        {
            return ScopedContext.SaveQueuesAsync<T, T2>(isTran);
        }

        public Task<List<T>> SaveQueuesAsync<T>(bool isTran = true)
        {
            return ScopedContext.SaveQueuesAsync<T>(isTran);
        }

        public ISugarQueryable<T> SqlQueryable<T>(string sql) where T : class, new()
        {
            return ScopedContext.SqlQueryable<T>(sql);
        }
        public IStorageable<T> Storageable<T>(T[] dataList) where T : class, new()
        {
            return ScopedContext.Storageable(dataList);
        }
        public StorageableDataTable Storageable(List<Dictionary<string, object>> dictionaryList, string tableName)
        {
            return ScopedContext.Storageable(dictionaryList, tableName);
        }
        public StorageableDataTable Storageable(Dictionary<string, object> dictionary, string tableName)
        {
            return ScopedContext.Storageable(dictionary, tableName);
        }

        public IStorageable<T> Storageable<T>(List<T> dataList) where T : class, new()
        {
            return ScopedContext.Storageable(dataList);
        }

        public IStorageable<T> Storageable<T>(T data) where T : class, new()
        {
            return ScopedContext.Storageable(data);
        }
        public StorageableDataTable Storageable(DataTable data)
        {
            return ScopedContext.Storageable(data);
        }
        public StorageableMethodInfo StorageableByObject(object singleEntityObjectOrListObject) 
        {
            return ScopedContext.StorageableByObject(singleEntityObjectOrListObject);
        }

        public ISugarQueryable<T> Union<T>(List<ISugarQueryable<T>> queryables) where T : class, new()
        {
            return ScopedContext.Union(queryables);
        }

        public ISugarQueryable<T> Union<T>(params ISugarQueryable<T>[] queryables) where T : class, new()
        {
            return ScopedContext.Union(queryables);
        }

        public ISugarQueryable<T> UnionAll<T>(List<ISugarQueryable<T>> queryables) where T : class, new()
        {
            return ScopedContext.UnionAll(queryables);
        }

        public ISugarQueryable<T> UnionAll<T>(params ISugarQueryable<T>[] queryables) where T : class, new()
        {
            return ScopedContext.UnionAll(queryables);
        }
        public UpdateMethodInfo UpdateableByObject(object singleEntityObjectOrListObject)
        {
            return ScopedContext.UpdateableByObject(singleEntityObjectOrListObject);
        }
        public IUpdateable<Dictionary<string, object>> UpdateableByDynamic(object updateDynamicObject)
        {
            return ScopedContext.UpdateableByDynamic(updateDynamicObject);
        }

        public IUpdateable<T> Updateable<T>() where T : class, new()
        {
            return ScopedContext.Updateable<T>();
        }

        public IUpdateable<T> Updateable<T>(Dictionary<string, object> columnDictionary) where T : class, new()
        {
            return ScopedContext.Updateable<T>(columnDictionary);
        }

        public IUpdateable<T> Updateable<T>(dynamic updateDynamicObject) where T : class, new()
        {
            return ScopedContext.Updateable<T>((object)updateDynamicObject);
        }

        public IUpdateable<T> Updateable<T>(Expression<Func<T, bool>> columns) where T : class, new()
        {
            return ScopedContext.Updateable(columns);
        }

        public IUpdateable<T> Updateable<T>(Expression<Func<T, T>> columns) where T : class, new()
        {
            return ScopedContext.Updateable(columns);
        }

        public IUpdateable<T> Updateable<T>(List<T> UpdateObjs) where T : class, new()
        {
            return ScopedContext.Updateable(UpdateObjs);
        }

        public IUpdateable<T> Updateable<T>(T UpdateObj) where T : class, new()
        {
            return ScopedContext.Updateable(UpdateObj);
        }

        public IUpdateable<T> Updateable<T>(T[] UpdateObjs) where T : class, new()
        {
            return ScopedContext.Updateable(UpdateObjs);
        }
        public SplitTableContext SplitHelper<T>() where T : class, new()
        {
            return ScopedContext.SplitHelper<T>();
        }
        public SplitTableContext SplitHelper(Type entityType)
        {
            return ScopedContext.SplitHelper(entityType);
        }
        public SplitTableContextResult<T> SplitHelper<T>(T data) where T : class, new()
        {
            return ScopedContext.SplitHelper(data);
        }
        public SplitTableContextResult<T> SplitHelper<T>(List<T> dataList) where T : class, new()
        {
            return ScopedContext.SplitHelper(dataList);
        }
        public IFastest<T> Fastest<T>() where T : class, new()
        {
            return ScopedContext.Fastest<T>();
        }

        public void ThenMapper<T>(IEnumerable<T> list, Action<T> action)
        {
            ScopedContext.ThenMapper(list, action);
        }

        public Task ThenMapperAsync<T>(IEnumerable<T> list, Func<T, Task> action)
        {
            return ScopedContext.ThenMapperAsync(list, action);
        }

        public ITenant AsTenant()
        {
            return ScopedContext.AsTenant();
        }

        public ISaveable<T> Saveable<T>(List<T> saveObjects) where T : class, new()
        {
            return ScopedContext.Saveable(saveObjects);
        }

        public ISaveable<T> Saveable<T>(T saveObject) where T : class, new()
        {
            return ScopedContext.Saveable(saveObject);
        }
        public InsertNavTaskInit<T, T> InsertNav<T>(T data) where T : class, new()
        {
            return ScopedContext.InsertNav(data);
        }
        public InsertNavTaskInit<T, T> InsertNav<T>(List<T> datas) where T : class, new()
        {
            return ScopedContext.InsertNav(datas);
        }
        public InsertNavTaskInit<T, T> InsertNav<T>(T data,InsertNavRootOptions rootOptions) where T : class, new()
        {
            return ScopedContext.InsertNav(data,rootOptions);
        }
        public InsertNavTaskInit<T, T> InsertNav<T>(List<T> datas, InsertNavRootOptions rootOptions) where T : class, new()
        {
            return ScopedContext.InsertNav(datas,rootOptions);
        }
        public DeleteNavTaskInit<T, T> DeleteNav<T>(T data) where T : class, new()
        {
            return ScopedContext.DeleteNav(data);
        }
        public DeleteNavTaskInit<T, T> DeleteNav<T>(List<T> datas) where T : class, new()
        {
            return ScopedContext.DeleteNav(datas);
        }
        public DeleteNavTaskInit<T, T> DeleteNav<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return ScopedContext.DeleteNav(whereExpression);
        }
        public DeleteNavTaskInit<T, T> DeleteNav<T>(T data, DeleteNavRootOptions options) where T : class, new()
        {
            return ScopedContext.DeleteNav(data, options);
        }
        public DeleteNavTaskInit<T, T> DeleteNav<T>(List<T> datas, DeleteNavRootOptions options) where T : class, new()
        {
            return ScopedContext.DeleteNav(datas, options);
        }
        public DeleteNavTaskInit<T, T> DeleteNav<T>(Expression<Func<T, bool>> whereExpression, DeleteNavRootOptions options) where T : class, new()
        {
            return ScopedContext.DeleteNav(whereExpression, options);
        }
        public UpdateNavTaskInit<T, T> UpdateNav<T>(T data) where T : class, new()
        {
            return ScopedContext.UpdateNav(data);
        }
        public UpdateNavTaskInit<T, T> UpdateNav<T>(List<T> datas) where T : class, new()
        {
            return ScopedContext.UpdateNav(datas);
        }
        public UpdateNavTaskInit<T, T> UpdateNav<T>(List<T> datas, UpdateNavRootOptions rootOptions) where T : class, new()
        {
            return this.ScopedContext.UpdateNav(datas, rootOptions);
        }
        public UpdateNavTaskInit<T, T> UpdateNav<T>(T data, UpdateNavRootOptions rootOptions) where T : class, new()
        {
            return this.ScopedContext.UpdateNav(data, rootOptions);
        }
        public SqlSugarClient CopyNew()
        {
            var result= new SqlSugarClient(UtilMethods.CopyConfig(this.Ado.Context.CurrentConnectionConfig));
            result.QueryFilter = this.QueryFilter;
            return result;
        }
        public DynamicBuilder DynamicBuilder()
        {
           return ScopedContext.DynamicBuilder();
        }
        public void Tracking<T>(T data) where T : class, new()
        {
            ScopedContext.Tracking(data);
        }
        public void Tracking<T>(List<T> datas) where T : class, new()
        {
            ScopedContext.Tracking(datas);
        }
        public QueryMethodInfo QueryableByObject(Type entityType)
        {
            return ScopedContext.QueryableByObject(entityType);
        }
        public QueryMethodInfo QueryableByObject(Type entityType, string shortName)
        {
            return ScopedContext.QueryableByObject(entityType, shortName);
        }
        public GridSaveProvider<T> GridSave<T>(List<T> oldList, List<T> saveList) where T : class, new()
        {
            return ScopedContext.GridSave(oldList, saveList);
        }
        public GridSaveProvider<T> GridSave<T>(List<T> saveList) where T : class, new()
        {
            return ScopedContext.GridSave(saveList);
        }
        public void ClearTracking()
        {
            ScopedContext.ClearTracking();
        }
        #endregion
    }
}