using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Text.RegularExpressions;
using System.Transactions;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：SQL糖 ORM 核心类升级版 分布式存储和云计算框架
    /// ** 创始时间：2015-12-14
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public partial class CloudClient : IDisposable
    {
        #region private variables
        private object tranLock = new object();
        private object dbsLock = new object();
        private List<SqlSugarClient> dbs = new List<SqlSugarClient>();
        private List<CloudConnectionConfig> configList { get; set; }
        #endregion

        #region public variables
        /// <summary>
        /// 分布式事务
        /// </summary>
        public CommittableTransaction Tran = null;
        /// <summary>
        /// 内存中处理数据的最大值（默认：1000）
        /// </summary>
        public int PageMaxHandleNumber = 1000;
        /// <summary>
        /// 将时间格式化成数据库格式,如果数据库格式和默认值不相同需要重新设置(默认:yyyy-MM-dd hh:mm:ss.ms)
        /// </summary>
        public Func<object, object> FormatTimeByDataBase = (object fieldValue) =>
        {
            if (fieldValue.GetType().Name == "DateTime")
            {
                if (fieldValue != DBNull.Value)
                {
                    fieldValue = Convert.ToDateTime(fieldValue).ToString("yyyy-MM-dd HH:mm:ss") + "." + Convert.ToDateTime(fieldValue).Millisecond;
                }
            }
            return fieldValue;
        };
        #endregion

        #region insert
        /// <summary>
        /// 批量插入
        /// 使用说明:sqlSugar.Insert(List《entity》);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities">插入对象</param>
        /// <param name="isIdentity">主键是否为自增长,true可以不填,false必填</param>
        /// <returns></returns>
        public List<object> InsertRange<T>(List<T> entities, bool isIdentity = true) where T : class
        {
            List<object> reval = new List<object>();
            foreach (var it in entities)
            {
                reval.Add(Insert<T>(it, isIdentity));
            }
            return reval;
        }
        /// <summary>
        /// 插入
        /// 使用说明:sqlSugar.Insert(entity);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">插入对象</param>
        /// <param name="isIdentity">主键是否为自增长,true可以不填,false必填</param>
        /// <returns></returns>
        public object Insert<T>(T entity, bool isIdentity = true) where T : class
        {

            var connName = CloudPubMethod.GetConnection(this.configList);
            var db = new SqlSugarClient(connName);
            SettingConnection(db);
            return db.Insert<T>(entity, isIdentity);
        }
        #endregion

        #region constructor
        /// <summary>
        /// 私有禁止无参实例
        /// </summary>
        private CloudClient()
        {

        }
        /// <summary>
        /// ** 描述：SQL糖 ORM 核心类升级版 分布式存储和云计算框架
        /// ** 创始时间：2015-12-14
        /// ** 修改时间：-
        /// ** 作者：sunkaixuan
        /// ** 使用说明：
        /// </summary>
        /// <param name="configList">云计算连接配置</param>
        public CloudClient(List<CloudConnectionConfig> configList)
        {
            this.configList = configList;
        }
        #endregion

        #region delete
        /// <summary>
        /// 批量删除
        /// 注意：whereIn 主键集合  
        /// 使用说明:Delete《T》(new int[]{1,2,3}) 或者  Delete《T》(3)
        /// </summary>
        /// <param name="whereIn"> delete ids </param>
        public bool Delete<T, FiledType>(params FiledType[] whereIn) where T:class
        {
            var tasks = new Task<bool>[configList.Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<bool>(ti =>
                {
                    var connName = configList[ti].ConnectionString;
                    var db = new SqlSugarClient(connName);
                    SettingConnection(db);
                    return db.Delete<T, FiledType>(whereIn);

                }, tasks, i);
            }
            Task.WaitAll(tasks);
            return tasks.Any(it => it.Result);
        }
        /// <summary>
        /// 删除,根据表达式
        /// 使用说明:
        /// Delete《T》(it=>it.id=100) 或者Delete《T》(3)
        /// </summary>
        /// <param name="expression">筛选表达式</param>
        public bool Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression)where T:class
        {
            var tasks = new Task<bool>[configList.Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<bool>(ti =>
                {
                    var connName = configList[ti].ConnectionString;
                    var db = new SqlSugarClient(connName);
                    SettingConnection(db);
                    return db.Delete<T>(expression);


                }, tasks, i);
            }
            Task.WaitAll(tasks);
            return tasks.Any(it => it.Result);
        }
        /// <summary>
        /// 批量删除
        /// 注意：whereIn 主键集合  
        /// 使用说明:Delete《T》(new int[]{1,2,3}) 或者  Delete《T》(3)
        /// </summary>
        ///<param name="field">  </param>
        /// <param name="whereIn"> delete ids </param>
        public bool FalseDelete<T, FiledType>(string field, params FiledType[] whereIn)where T:class
        {
            var tasks = new Task<bool>[configList.Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<bool>(ti =>
                {
                    var connName = configList[ti].ConnectionString;
                    var db = new SqlSugarClient(connName);
                    SettingConnection(db); ;
                    return db.FalseDelete<T, FiledType>(field, whereIn);

                }, tasks, i);
            }
            Task.WaitAll(tasks);
            return tasks.Any(it => it.Result);
        }
        /// <summary>
        /// 假删除，根据表达式
        /// 使用说明::
        /// FalseDelete《T》(new int[]{1,2,3})或者Delete《T》(3)
        /// </summary>
        /// <param name="field">更新删除状态字段</param>
        /// <param name="expression">筛选表达式</param>
        public bool FalseDelete<T>(string field, System.Linq.Expressions.Expression<Func<T, bool>> expression) where T:class
        {
            var tasks = new Task<bool>[configList.Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<bool>(ti =>
                {
                    var connName = configList[ti].ConnectionString;
                    var db = new SqlSugarClient(connName);
                    SettingConnection(db);
                    return db.FalseDelete<T>(field, expression);
                }, tasks, i);
            }
            Task.WaitAll(tasks);
            return tasks.Any(it => it.Result);
        }
        #endregion

        #region search

        /// <summary>
        /// 多线程请求所有数据库节点，同步汇总结果
        /// </summary>
        /// <typeparam name="T">支持DataTable、实体类和值类型</typeparam>
        /// <param name="sql"></param>
        /// <param name="whereObj">参数 例如: new { id="1",name="张三"}</param>
        /// <returns></returns>
        public Taskable<T> Taskable<T>(string sql, object whereObj = null)
        {
            return Taskable<T>(sql, configList.Select(it => it.ConnectionString).ToList(), whereObj);
        }
        /// <summary>
        /// 多线程请求所有数据库节点，同步汇总结果
        /// </summary>
        /// <typeparam name="T">支持DataTable、实体类和值类型</typeparam>
        /// <param name="sql"></param>
        /// <param name="connectionStringList">连接字符串数组</param>
        /// <param name="whereObj">参数 例如: new { id="1",name="张三"}</param>
        /// <returns></returns>
        public Taskable<T> Taskable<T>(string sql, List<string> connectionStringList, object whereObj = null)
        {
            Taskable<T> reval = new Taskable<T>();
            reval.Sql = sql;
            reval.WhereObj = whereObj;
            var tasks = new Task<CloudSearchResult<T>>[connectionStringList.Count];

            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<CloudSearchResult<T>>(ti =>
                {
                    string innerSql = sql;
                    var connString = connectionStringList[ti];
                    var db = new SqlSugarClient(connString);
                    SettingConnection(db);
                    CloudSearchResult<T> itemReval = new CloudSearchResult<T>();
                    var isDataTable = typeof(T) == typeof(DataTable);
                    var isClass = typeof(T).IsClass;
                    if (innerSql.Contains("$:->connectionString<-:$"))
                        innerSql = innerSql.Replace("$:->connectionString<-:$", connString);
                    if (isDataTable)
                    {
                        itemReval.DataTable = db.GetDataTable(innerSql, whereObj);
                    }
                    else if (isClass)
                    {
                        itemReval.Entities = db.SqlQuery<T>(innerSql, whereObj);
                    }
                    else
                    {
                        var obj = db.GetScalar(sql, whereObj);
                        if (obj == DBNull.Value)
                        {
                            itemReval.Value = default(T);
                        }
                        else
                        {
                            obj = Convert.ChangeType(obj, typeof(T));
                            itemReval.Value = (T)obj;
                        }
                    }
                    itemReval.ConnectionString = connString;
                    return itemReval;
                }, tasks, i);
            }
            Task.WaitAll(tasks);
            reval.Tasks = tasks;
            return reval;
        }
        /// <summary>
        /// 多线程请求所有数据库节点，同步汇总结果
        /// </summary>
        /// <typeparam name="T">支持DataTable、实体类和值类型</typeparam>
        /// <param name="sqlSelect">sql from之前（例如： "select count(*)" ）</param>
        /// <param name="sqlEnd">sql from之后（例如： "from table where id=1" </param>
        /// <param name="whereObj">参数 例如: new { id="1",name="张三"}</param>
        /// <returns></returns>
        public TaskableWithCount<T> TaskableWithCount<T>(string sqlSelect, string sqlEnd, object whereObj = null)
        {
            TaskableWithCount<T> reval = new TaskableWithCount<T>();
            reval.Sql = sqlSelect + sqlEnd;
            reval.WhereObj = whereObj;
            var tasks = new Task<CloudSearchResult<T>>[configList.Count];

            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<CloudSearchResult<T>>(ti =>
                {
                    var connString = configList[ti].ConnectionString;
                    var db = new SqlSugarClient(connString);
                    SettingConnection(db);
                    CloudSearchResult<T> itemReval = new CloudSearchResult<T>();
                    var isDataTable = typeof(T) == typeof(DataTable);
                    var isClass = typeof(T).IsClass;
                    if (isClass)
                    {
                        itemReval.Entities = db.SqlQuery<T>(reval.Sql, whereObj);
                    }
                    else if (isDataTable)
                    {
                        itemReval.DataTable = db.GetDataTable(reval.Sql, whereObj);
                    }
                    else
                    {
                        var obj = db.GetScalar(reval.Sql, whereObj);
                        obj = Convert.ChangeType(obj, typeof(T));
                        itemReval.Value = (T)obj;
                    }
                    itemReval.Count = db.GetInt("SELECT COUNT(1)" + sqlEnd); ;
                    itemReval.ConnectionString = connString;
                    return itemReval;
                }, tasks, i);
            }
            Task.WaitAll(tasks);
            reval.Tasks = tasks;
            return reval;
        }
        /// <summary>
        /// 获取分页数据(注意：该函数不可以在事务内使用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unqueField">数据库中数据唯一的列（建议：主键GUID）</param>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderByField"></param>
        /// <param name="orderByType"></param>
        /// <param name="whereObj">参数 例如: new { id="1",name="张三"}</param>
        /// <returns></returns>
        public List<T> TaskableWithPage<T>(string unqueField, string sql, int pageIndex, int pageSize, ref int pageCount, string orderByField, OrderByType orderByType, object whereObj = null) where T : class
        {
            return TaskableWithPage<T>(unqueField, sql, pageIndex, pageSize, ref pageCount, new List<OrderByDictionary>() { new OrderByDictionary() { OrderByField = orderByField, OrderByType = orderByType } }, whereObj);
        }
        /// <summary>
        /// 获取分页数据(注意：该函数不可以在事务内使用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unqueField">数据库中数据唯一的列（建议：主键GUID）</param>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderByTypes">排序信息集合</param>
        /// <param name="whereObj">参数 例如: new { id="1",name="张三"}</param>
        /// <returns></returns>
        public List<T> TaskableWithPage<T>(string unqueField, string sql, int pageIndex, int pageSize, ref int pageCount, List<OrderByDictionary> orderByTypes, object whereObj = null) where T : class
        {
            if (orderByTypes == null || orderByTypes.Count == 0)
            {
                throw new ArgumentNullException("CloudClient.TaskableWithPage.orderByTypes");
            }
            if (pageIndex == 0)
                pageIndex = 1;
            int configCount = configList.Count;
            string sqlCount = string.Format("SELECT COUNT(*) FROM ({0}) t ", sql);
            pageCount = Taskable<int>(sqlCount, whereObj).Count();
            if (pageCount == 0)
            {
                return new List<T>();
            }
            int totalPage = (pageCount + pageSize - 1) / pageSize;
            var lastPage = (totalPage - pageIndex) + 1;
            var isLast = totalPage == pageIndex;

            string fullOrderByString = string.Join(",", orderByTypes.Select(it => it.OrderByString)) + "," + unqueField + " ASC ";
            string fullOrderByStringReverse = string.Join(",", orderByTypes.Select(it => it.OrderByStringReverse)) + "," + unqueField + " DESC ";
            string orderByFieldsString = string.Join(",", orderByTypes.Select(it => it.OrderByField));
            string[] orderByFieldArray = orderByTypes.Select(it => it.OrderByField).ToArray();

            string whereCompare = string.Join(" AND ", orderByTypes.Select(it => string.Format(" {0}{1}'$:->{0}<-:$' ", it.OrderByField, it.Symbol, it.Symbol)));

            /***one nodes***/
            #region one nodes
            var isOneNode = configCount == 1;
            if (isOneNode)
            {
                var connName = configList.Single().ConnectionString;
                var db = new SqlSugarClient(connName);
                SettingConnection(db);
                var sqlPage = string.Format(@"SELECT * FROM (
                                                                                    SELECT *,ROW_NUMBER()OVER(ORDER BY {1}) AS  ROWINDEX  FROM ({0}) as sqlstr ) t WHERE t.rowIndex BETWEEN {2} AND {3}
                                                         ", sql, fullOrderByString, (pageIndex - 1) * pageSize + 1, pageSize * pageIndex);
                var list = db.SqlQuery<T>(sql, whereObj);
                return list.OrderBy(orderByTypes).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            #endregion

            /***small data***/
            #region small data
            var isSmallData = pageCount <= this.PageMaxHandleNumber || int.MaxValue == pageSize;//page size等于int.MaxValue不需要分页
            if (isSmallData)
            {
                var tasks = Taskable<T>(sql + " ORDER BY " + fullOrderByString, whereObj);
                return tasks.Tasks.SelectMany(it => it.Result.Entities).OrderBy(orderByTypes).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            }
            #endregion

            /***small index***/
            #region small index
            var isSmallPageIndex = CloudPubMethod.GetIsSmallPageIndex(pageIndex, pageSize, configCount, this.PageMaxHandleNumber);
            if (isSmallPageIndex)
            {

                var sqlPage = string.Format(@"SELECT * FROM (
                                                                                        SELECT *,ROW_NUMBER()OVER(ORDER BY {1}) AS  ROWINDEX  FROM ({0}) as sqlstr ) t WHERE t.rowIndex BETWEEN {2} AND {3}
                                                                                        ", sql, fullOrderByString, 1, pageSize * configCount);
                var tasks = Taskable<T>(sqlPage, whereObj);
                return tasks.Tasks.SelectMany(it => it.Result.Entities).OrderBy(orderByTypes).ThenBy(unqueField, OrderByType.Asc).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            #endregion

            /***small index  by reverse***/
            #region small index  by reverse
            var isSmallPageIndexByReverse = CloudPubMethod.GetIsSmallPageIndexByReverse(totalPage, pageIndex, pageSize, configCount, this.PageMaxHandleNumber);
            if (isSmallPageIndexByReverse)
            {

                var sqlPage = string.Format(@"SELECT * FROM (
                                                                                        SELECT *,ROW_NUMBER()OVER(ORDER BY {1}) AS  ROWINDEX  FROM ({0}) as sqlstr ) t WHERE t.rowIndex BETWEEN {2} AND {3}
                                                                                        ", sql, fullOrderByStringReverse, 1, lastPage * configCount * pageSize);
                var tasks = Taskable<T>(sqlPage, whereObj);
                var lastPageSize = pageCount % pageSize;
                if (lastPageSize == 0) lastPageSize = pageSize;

                var list = tasks.Tasks.SelectMany(it => it.Result.Entities).OrderByReverse(orderByTypes).ThenBy(unqueField, OrderByType.Desc);
                if (isLast)
                {
                    return list.Skip(0).Take(lastPageSize).OrderBy(orderByTypes).ThenBy(unqueField, OrderByType.Asc).ToList();
                }
                else
                {
                    var skipIndex = (lastPage - 1) * pageSize + lastPageSize - pageSize;
                    return list.Skip(skipIndex).Take(pageSize).OrderBy(orderByTypes).ThenBy(unqueField, OrderByType.Asc).ToList();
                }
            }
            #endregion

            /***other***/
            #region other
            //单节点最大索引
            var maxDataIndex = pageIndex * pageSize * configCount;
            //分页最大索引
            var pageEnd = pageIndex * pageSize;
            var pageBegin = pageIndex * pageSize - pageSize;
            //节点间距
            var dataSampleIndex = pageBegin / configCount;

            string sqlOtherPage = GetSqlSampleRowSql(unqueField, sql, fullOrderByString, orderByFieldsString, dataSampleIndex);
            DataRow sampleRow = null;
            int sampleRowIndex;
            int sampleEachIndex;
            List<DataRow> innerDataSampleList;
            InitSampleRow(unqueField, sql, orderByTypes, whereObj, configCount, fullOrderByString, ref whereCompare, pageBegin, ref sqlOtherPage, ref sampleRow, out sampleRowIndex, out sampleEachIndex, out innerDataSampleList);
            sampleRow=innerDataSampleList[sampleEachIndex];
            //获取分页索引所需参数实体
            PageRowInnerParamsResultMultipleOrderBy beginEndRowParams = new PageRowInnerParamsResultMultipleOrderBy()
            {
                RowIndex = sampleRowIndex,
                Row = sampleRow,
                Begin = pageBegin,
                End = pageEnd,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Sql = sql,
                UnqueField = unqueField,
                isGreater = sampleRowIndex > pageBegin,
                UnqueValue = sampleRow[0],
                FullOrderByString = fullOrderByString,
                FullOrderByStringReverse = fullOrderByStringReverse,
                ConfigCount = configCount,
                orderByFieldsString = orderByFieldsString,
                OrderByTypes = orderByTypes,
                WhereObj = whereObj,
                Count = pageCount,
                SampleEachIndex=sampleEachIndex
            };

            PageRowInnerParamsResultMultipleOrderBy beginEndRow =null;
            var isBeginRow = (Math.Abs(sampleRowIndex - pageBegin) * configCount < PageMaxHandleNumber || Math.Abs(pageBegin - sampleRowIndex) * configCount < PageMaxHandleNumber);
            beginEndRow=isBeginRow?beginEndRowParams: GetListByPage_GetPageBeginRowMultipleOrderBy(beginEndRowParams);
            Dispose(false);
            var reval = GetListByPage_GetPageListMultipleOrderBy<T>(beginEndRow);
            Dispose(false);
            return reval;
            #endregion

        }


    

        #endregion

        #region update
        /// <summary>
        /// 更新
        /// 注意：rowObj为T类型将更新该实体的非主键所有列，如果rowObj类型为匿名类将更新指定列
        /// 使用说明:sqlSugar.Update《T》(rowObj,whereObj);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="FiledType">主键类型</typeparam>
        /// <param name="rowObj">new T(){name="张三",sex="男"}或者new {name="张三",sex="男"}</param>
        /// <param name="whereIn">new int[]{1,2,3}</param>
        /// <returns></returns>
        public bool Update<T, FiledType>(object rowObj, params FiledType[] whereIn) where T : class
        {
            var tasks = new Task<bool>[configList.Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<bool>(ti =>
                {
                    var connName = configList[ti].ConnectionString;
                    var db = new SqlSugarClient(connName);
                    SettingConnection(db);
                    return db.Update<T, FiledType>(rowObj, whereIn);
                }, tasks, i);
            }
            Task.WaitAll(tasks);
            return tasks.Any(it => it.Result);
        }

        /// <summary>
        /// 更新
        /// 注意：rowObj为T类型将更新该实体的非主键所有列，如果rowObj类型为匿名类将更新指定列
        /// 使用说明:sqlSugar.Update《T》(rowObj,whereObj);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowObj">new T(){name="张三",sex="男"}或者new {name="张三",sex="男"}</param>
        /// <param name="expression">it.id=100</param>
        /// <returns></returns>
        public bool Update<T>(object rowObj, System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class
        {
            var tasks = new Task<bool>[configList.Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<bool>(ti =>
                {
                    var connName = configList[ti].ConnectionString;
                    var db = new SqlSugarClient(connName);
                    SettingConnection(db);
                    return db.Update<T>(rowObj, expression);
                }, tasks, i);
            }
            Task.WaitAll(tasks);
            return tasks.Any(it => it.Result);
        }
        #endregion

        #region dispose
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            lock (this.dbsLock)
            {
                if (dbs != null)
                {
                    lock (dbs)
                    {
                        foreach (var db in dbs)
                        {
                            db.Dispose();
                        }
                    }
                }

                dbs = null;
                this.configList = null;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="isAll"></param>
        public void Dispose(bool isAll)
        {
            lock (this.dbsLock)
            {
                if (dbs != null)
                {
                    lock (dbs)
                    {
                        foreach (var db in dbs)
                        {
                            db.Dispose();
                        }
                    }
                }
                if (isAll)
                {
                    dbs = null;
                    this.configList = null;
                }
            }
        }
        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void RemoveAllCache<T>()
        {
            var connName = configList[0].ConnectionString;
            var db = new SqlSugarClient(connName);
            db.RemoveAllCache<T>();
        }

        /// <summary>
        /// 清空事务
        /// </summary>
        public void TranDispose()
        {
            lock (this.tranLock)
            {
                lock (this.Tran)
                {
                    Tran = null;
                }
            }
        }
        #endregion

        #region private methods

        #region setting
        /// <summary>
        /// 设置连接
        /// </summary>
        /// <param name="db"></param>
        private void SettingConnection(SqlSugarClient db)
        {
            try
            {
                lock (this.tranLock)
                {
                    if (Tran != null)
                    {
                        lock (this.Tran)
                        {
                            lock (db)
                            {
                                db.GetConnection().EnlistTransaction(Tran);
                            }
                        }
                    }
                }
            }
            catch (Exception)//BUG 实现找不到为什么锁了还有时会报被占用
            {

                try
                {
                    System.Threading.Thread.Sleep(10);
                    db.GetConnection().EnlistTransaction(Tran);
                }
                catch (Exception)
                {

                    try
                    {
                        System.Threading.Thread.Sleep(100);
                        db.GetConnection().EnlistTransaction(Tran);
                    }
                    catch (Exception)
                    {

                        try
                        {
                            System.Threading.Thread.Sleep(1000);
                            db.GetConnection().EnlistTransaction(Tran);
                        }
                        catch (Exception)
                        {

                            System.Threading.Thread.Sleep(10000);
                            db.GetConnection().EnlistTransaction(Tran);
                        }
                    }
                }
            }

            lock (this.dbsLock)
            {
                if (dbs != null)
                {
                    lock (dbs)
                    {
                        dbs.Add(db);
                    }
                }
            }
        }
        #endregion

        #region page order by many
        /// <summary>
        /// 获取Where比较条件
        /// </summary>
        /// <param name="whereCompare"></param>
        /// <param name="orderByTypes"></param>
        /// <param name="sampleRow"></param>
        /// <param name="unqueField"></param>
        /// <param name="unqueValue"></param>
        /// <param name="isReverse"></param>
        /// <param name="isEqual"></param>
        /// <returns></returns>
        private string GetWhereCompare(string whereCompare, List<OrderByDictionary> orderByTypes, DataRow sampleRow, string unqueField, string unqueValue, bool isReverse = false, bool isEqual = false)
        {
            List<string> reval = new List<string>();
            var ordersCount = orderByTypes.Count;
            /*
               排序筛选逻辑:
             * 例如:  num desc, createTime asc
             *  num>x
             *  num=x and createTime<xx
             *  num=x and createTime=xx and unqueField<unqueValue
             *  以此类推
             */
            //算出没有unqueField的部分
            for (int i = 0; i < ordersCount; i++)
            {
                List<string> revalChild = new List<string>();
                for (int j = 0; j <= i; j++)
                {
                    var isLastEach = j == i;
                    OrderByDictionary currentOrderType = orderByTypes[j];
                    var currentSymbol = isReverse ? currentOrderType.SymbolReverse : currentOrderType.Symbol;
                    var fieldValue = sampleRow[currentOrderType.OrderByField];
                    fieldValue = FormatTimeByDataBase(fieldValue);
                    if (isLastEach)
                    {
                        revalChild.Add(string.Format(" {0}{1}'{2}' ", currentOrderType.OrderByField, currentSymbol, fieldValue));
                    }
                    else
                    {
                        revalChild.Add(string.Format(" {0}='{1}' ", currentOrderType.OrderByField, fieldValue));
                    }
                }
                reval.Add(" ( " + string.Join(" AND ", revalChild) + " ) ");
            }
            //算出unqueField的部分
            List<string> revalChildLast = new List<string>();
            for (int i = 0; i < ordersCount; i++)
            {
                OrderByDictionary currentOrderType = orderByTypes[i];
                var fieldValue = sampleRow[currentOrderType.OrderByField];
                fieldValue = FormatTimeByDataBase(fieldValue);
                revalChildLast.Add(string.Format(" {0}='{1}' ", currentOrderType.OrderByField, fieldValue));
            }
            var isSymbol = isReverse ? ">" : "<";
            if (isEqual)
            {
                isSymbol = isReverse ? ">=" : "<=";
            }
            revalChildLast.Add(string.Format("{0}{1}'{2}' ", unqueField, isSymbol, unqueValue));
            reval.Add(" ( " + string.Join(" AND ", revalChildLast) + " ) ");


            //将unqueField部分和非unqueField部分进行OR拼接
            return string.Join(" OR ", reval);
        }


        private PageRowInnerParamsResultMultipleOrderBy GetListByPage_GetPageBeginRowMultipleOrderBy(PageRowInnerParamsResultMultipleOrderBy paras)
        {
            Dispose(false);
            string whereCompare = GetWhereCompare(string.Empty, paras.OrderByTypes, paras.Row, paras.UnqueField, paras.UnqueValue.ToString());
            string whereCompareReverse = GetWhereCompare(string.Empty, paras.OrderByTypes, paras.Row, paras.UnqueField, paras.UnqueValue.ToString(), true);
            string sql = string.Empty;
            var thisIndex = 0;
            //向前取样
            if (paras.isGreater)
            {

                #region 向前取样
                thisIndex = (paras.RowIndex - paras.Begin) / paras.ConfigCount;
                sql = string.Format(@"SELECT {0},RowIndex,{1}   FROM (
                                                                                                    SELECT *,ROW_NUMBER()OVER(ORDER BY {2}) AS  ROWINDEX  FROM ({3}) as sqlstr  WHERE  {4} ) t WHERE t.ROWINDEX={5}
             
                                                                                       ",
                                                                                             paras.UnqueField/*0*/,
                                                                                             paras.orderByFieldsString/*1*/,
                                                                                             paras.FullOrderByStringReverse/*2*/,
                                                                                             paras.Sql/*3*/,
                                                                                             whereCompare/*4*/,
                                                                                             thisIndex/*5*/);

                var rowList=Taskable<DataTable>(sql, paras.WhereObj).MergeTable().ToList();
                int sampleRowIndex=0;
                int i=-1;
                while (sampleRowIndex == 0)
                {
                    var isFirst = i == -1;
                    if (i == paras.SampleEachIndex) continue;
                    var row = rowList[isFirst?paras.SampleEachIndex:i];
                    paras.Row = row;
                    paras.UnqueValue = row[0];
                    whereCompare = GetWhereCompare(string.Empty, paras.OrderByTypes, paras.Row, paras.UnqueField, paras.UnqueValue.ToString());
                    whereCompareReverse = GetWhereCompare(string.Empty, paras.OrderByTypes, paras.Row, paras.UnqueField, paras.UnqueValue.ToString(), true);
                    sql = string.Format(@"SELECT  COUNT(1)  FROM (
                                                                                                    SELECT *,ROW_NUMBER()OVER(ORDER BY {0}) AS  ROWINDEX  FROM ({1}) as sqlstr ) t WHERE {2}
                                                                                                    ",
                                                                                             paras.FullOrderByString/*0*/,
                                                                                             paras.Sql/*1*/,
                                                                                             whereCompare/*2*/
                                                                                         );
                    var innerSampleRowIndex = Taskable<int>(sql, paras.WhereObj).Count();
                    var isLess=Math.Abs(innerSampleRowIndex - paras.Begin)<Math.Abs(paras.RowIndex - paras.Begin);
                    if (isLess)
                    {
                       sampleRowIndex=innerSampleRowIndex;
                    }
               
                }
                paras.RowIndex = sampleRowIndex;
                paras.isGreater = sampleRowIndex > paras.Begin;
                if (sampleRowIndex == paras.Begin) return paras;//如果相等返回BeginRow
                if (Math.Abs((sampleRowIndex - paras.Begin) * paras.ConfigCount) < PageMaxHandleNumber)
                {
                    return paras;
                }
                return GetListByPage_GetPageBeginRowMultipleOrderBy(paras);
                #endregion

            }
            else//向后取样
            {

                #region 向后取样
                thisIndex = (paras.Begin - paras.RowIndex) / paras.ConfigCount;
                if (thisIndex == 0)
                {
                    return paras;
                }

                sql = string.Format(@"SELECT {0},RowIndex,{1}   FROM (
                                                                                                    SELECT *,ROW_NUMBER()OVER(ORDER BY {2}) AS  ROWINDEX  FROM ({3}) as sqlstr WHERE {4}  ) t WHERE t.ROWINDEX={5}
             
                                                                                       ",
                                                                                        paras.UnqueField/*0*/,
                                                                                        paras.orderByFieldsString/*1*/,
                                                                                        paras.FullOrderByString/*2*/,
                                                                                        paras.Sql/*3*/,
                                                                                        whereCompareReverse/*4*/,
                                                                                        thisIndex/*5*/);
                var rowList = Taskable<DataTable>(sql, paras.WhereObj).MergeTable().OrderByDataRow(paras.OrderByTypes).ToList();
                var row = rowList[paras.ConfigCount-paras.SampleEachIndex-1];
                paras.Row = row;
                paras.UnqueValue = row[0];
                whereCompare = GetWhereCompare(string.Empty, paras.OrderByTypes, paras.Row, paras.UnqueField, paras.UnqueValue.ToString());
                whereCompareReverse = GetWhereCompare(string.Empty, paras.OrderByTypes, paras.Row, paras.UnqueField, paras.UnqueValue.ToString(), true);
                sql = string.Format(@"SELECT  COUNT(1)  FROM (
                                                                                                    SELECT *,ROW_NUMBER()OVER(ORDER BY {0}) AS  ROWINDEX  FROM ({1}) as sqlstr ) t WHERE  {2}
                                                                                                    ",
                                                                                         paras.FullOrderByString/*0*/,
                                                                                         paras.Sql/*1*/,
                                                                                         whereCompare/*2*/
                                                                                                                                                                                 );
                var maxRowIndex = Taskable<int>(sql, paras.WhereObj).Count();
                paras.RowIndex = maxRowIndex;
                paras.isGreater = maxRowIndex > paras.Begin;
                Dispose(false);
                if (maxRowIndex == paras.Begin) return paras;//如果相等返回BeginRow
                if (Math.Abs(((maxRowIndex - paras.Begin) * paras.ConfigCount)) < PageMaxHandleNumber)
                {
                    return paras;
                }
                return GetListByPage_GetPageBeginRowMultipleOrderBy(paras);
                #endregion
            }
        }
        private List<T> GetListByPage_GetPageListMultipleOrderBy<T>(PageRowInnerParamsResultMultipleOrderBy paras) where T : class
        {

            string whereCompareEqual = GetWhereCompare(string.Empty, paras.OrderByTypes, paras.Row, paras.UnqueField, paras.UnqueValue.ToString(), false, true);
            string whereCompareReverseEqual = GetWhereCompare(string.Empty, paras.OrderByTypes, paras.Row, paras.UnqueField, paras.UnqueValue.ToString(), true, true);
            string sql = null;
            if (paras.RowIndex == paras.Begin)
            { //如果相等
                string whereCompare = GetWhereCompare(string.Empty, paras.OrderByTypes, paras.Row, paras.UnqueField, paras.UnqueValue.ToString());
                string whereCompareReverse = GetWhereCompare(string.Empty, paras.OrderByTypes, paras.Row, paras.UnqueField, paras.UnqueValue.ToString(), true);
                sql = string.Format(@"SELECT  top {0}*  FROM (
                                                                                                    SELECT *,ROW_NUMBER()OVER(ORDER BY {1}) AS  ROWINDEX  FROM ({2}) as sqlstr ) t WHERE  ({3}) 
                                                                                                    ",
                                                                             paras.PageSize * paras.ConfigCount + paras.PageSize * paras.PageSize/*0*/,
                                                                             paras.FullOrderByString/*1*/,
                                                                             paras.Sql/*2*/,
                                                                             whereCompareReverse/*3*/,
                                                                             paras.UnqueValue/*4*/
                                                                             );
                return Taskable<T>(sql, paras.WhereObj).MergeEntities().OrderBy(paras.OrderByTypes).Skip(0).Take(paras.PageSize).ToList();
            }
            else if (paras.isGreater)
            { //大于
                string whereCompareReverse = GetWhereCompare(string.Empty, paras.OrderByTypes, paras.Row, paras.UnqueField, paras.UnqueValue.ToString(), true);
                string AnotherPartSql = null;
                var createrValue = (paras.RowIndex) - paras.Begin;
                sql = string.Format(@"SELECT TOP {0}  {1},{2} FROM ({3}) as  t WHERE {4}  ORDER BY {5}
                                                                       
                             ",
                                                             createrValue * paras.ConfigCount/*0*/,
                                                             paras.UnqueField/*1*/,
                                                             paras.orderByFieldsString/*2*/,
                                                             paras.Sql/*3*/,
                                                             whereCompareEqual/*4*/,
                                                             paras.FullOrderByStringReverse/*5*/
                                                            );
                if (createrValue < paras.PageSize)
                {

                    AnotherPartSql = string.Format(@"SELECT TOP {0}  {1},{2} FROM ({3}) as  t WHERE {4}   ORDER BY {5}
                                                                       
                             ",
                                                            (paras.PageSize - createrValue) * paras.ConfigCount/*0*/,
                                                            paras.UnqueField/*1*/,
                                                            paras.orderByFieldsString/*2*/,
                                                            paras.Sql/*3*/,
                                                            whereCompareReverse/*4*/,
                                                            paras.FullOrderByString/*5*/,
                                                            paras.UnqueValue
                                                            );
                }
                var rows = Taskable<DataTable>(sql, paras.WhereObj).MergeTable().ToList();
                if (createrValue < paras.PageSize)
                {
                    var rows2 = Taskable<DataTable>(AnotherPartSql, paras.WhereObj).MergeTable().ToList();
                    rows.AddRange(rows2);
                }
                rows = rows.OrderByDataRow(paras.OrderByTypes, new OrderByDictionary() { OrderByField = paras.UnqueField, OrderByType = OrderByType.Asc });
                var maxRowIndex = rows.IndexOf(rows.Single(it => it[0].ToString().ToLower() == paras.UnqueValue.ToString().ToLower()));
                var revalRows = rows.Skip(maxRowIndex - createrValue).Take(paras.PageSize).Select(it => it[0]).ToArray();
                sql = string.Format("SELECT * FROM ({0}) as  t WHERE {1} IN ({2})", paras.Sql, paras.UnqueField, revalRows.ToJoinSqlInVal());
                return Taskable<T>(sql, paras.WhereObj).MergeEntities().OrderBy(paras.OrderByTypes).ThenBy(paras.UnqueField, OrderByType.Asc).Take(paras.PageSize).ToList();

            }
            else
            { //小于

                var createrValue = paras.Begin - paras.RowIndex;
                sql = string.Format(@"SELECT TOP {0}  {1},{2} FROM ({3}) as  t WHERE  {4} ORDER BY {5}
                                                                                                    ",
                                                             createrValue * paras.ConfigCount + paras.PageSize/*0*/,
                                                             paras.UnqueField/*1*/,
                                                             paras.orderByFieldsString/*2*/,
                                                             paras.Sql/*3*/,
                                                             whereCompareReverseEqual/*4*/,
                                                             paras.FullOrderByString/*5*/
                                                             );
                var rows = Taskable<DataTable>(sql, paras.WhereObj).MergeTable().OrderByDataRow(paras.OrderByTypes, new OrderByDictionary() { OrderByField = paras.UnqueField, OrderByType = OrderByType.Asc });
                var maxRowIndex = rows.IndexOf(rows.Single(it => it[0].ToString().ToLower() == paras.UnqueValue.ToString().ToLower()));
                var revalRows = rows.Skip(maxRowIndex + createrValue).Take(paras.PageSize).Select(it => it[0]).ToArray();
                sql = string.Format("SELECT * FROM ({0}) as  t WHERE {1} IN ({2})", paras.Sql, paras.UnqueField, revalRows.ToJoinSqlInVal());
                return Taskable<T>(sql, paras.WhereObj).MergeEntities().OrderBy(paras.OrderByTypes).ThenBy(paras.UnqueField, OrderByType.Asc).Take(paras.PageSize).ToList();

            }
        }


        #endregion

        #region  get page Sample 

        /// <summary>
        /// 获取样品SQL
        /// </summary>
        /// <param name="unqueField"></param>
        /// <param name="sql"></param>
        /// <param name="fullOrderByString"></param>
        /// <param name="orderByFieldsString"></param>
        /// <param name="dataSampleIndex"></param>
        /// <returns></returns>
        private static string GetSqlSampleRowSql(string unqueField, string sql, string fullOrderByString, string orderByFieldsString, int dataSampleIndex)
        {
            string sqlOtherPage = string.Format(@"SELECT {4},RowIndex,{3}   FROM (
                                                                                                    SELECT *,ROW_NUMBER()OVER(ORDER BY {1}) AS  ROWINDEX  FROM ({0}) as sqlstr ) t WHERE t.rowIndex = ({2})
                                                                                                    ", sql/*0*/,
                                                                                                     fullOrderByString/*1*/,
                                                                                                     dataSampleIndex/*2*/,
                                                                                                     orderByFieldsString/*3*/,
                                                                                                    unqueField/*4*/);
            return sqlOtherPage;
        }
        /// <summary>
        /// 获取样品索引SQL
        /// </summary>
        /// <param name="unqueField"></param>
        /// <param name="sql"></param>
        /// <param name="fullOrderByString"></param>
        /// <param name="whereCompare"></param>
        /// <param name="sqlOtherPage"></param>
        /// <param name="sampleRow"></param>
        /// <returns></returns>
        private static string GetSampleIndexSql(string unqueField, string sql, string fullOrderByString, string whereCompare, string sqlOtherPage, DataRow sampleRow)
        {
            sqlOtherPage = string.Format(@"SELECT  COUNT(1)  FROM (
                                                                                                    SELECT *,ROW_NUMBER()OVER(ORDER BY {5}) AS  ROWINDEX  FROM ({2}) as sqlstr ) t WHERE {0}({3}) 
                                                                                                    ",
                                                                                                     null/*0*/,
                                                                                                     unqueField/*1*/,
                                                                                                     sql/*2*/,
                                                                                                     whereCompare/*3*/,
                                                                                                     sampleRow[0]/*4:UnqueValue*/,
                                                                                                     fullOrderByString/*5*/);
            return sqlOtherPage;
        }
        /// <summary>
        /// 初始化样品数据
        /// </summary>
        /// <param name="unqueField"></param>
        /// <param name="sql"></param>
        /// <param name="orderByTypes"></param>
        /// <param name="whereObj"></param>
        /// <param name="configCount"></param>
        /// <param name="fullOrderByString"></param>
        /// <param name="whereCompare"></param>
        /// <param name="pageBegin"></param>
        /// <param name="sqlOtherPage"></param>
        /// <param name="sampleRow"></param>
        /// <param name="sampleRowIndex"></param>
        /// <param name="sampleEachIndex"></param>
        /// <param name="innerDataSampleList"></param>
        private void InitSampleRow(string unqueField, string sql, List<OrderByDictionary> orderByTypes, object whereObj, int configCount, string fullOrderByString, ref string whereCompare, int pageBegin, ref string sqlOtherPage, ref DataRow sampleRow, out int sampleRowIndex, out int sampleEachIndex, out List<DataRow> innerDataSampleList)
        {
            sampleRowIndex = 0;
            sampleEachIndex = 0;
            int nodeSpacing = 1;
            innerDataSampleList = Taskable<DataTable>(sqlOtherPage, whereObj).MergeTable().OrderByDataRow(orderByTypes).ThenByDataRow(unqueField, OrderByType.Asc).ToList();

            for (int i = 0; i < configCount; i++)
            {
                if (configCount < nodeSpacing)
                {
                    if (i != configCount / 2) continue;
                }
                else if (i % nodeSpacing != 0)
                {
                    continue;
                }

                sampleRow = innerDataSampleList[i];

                whereCompare = GetWhereCompare(whereCompare, orderByTypes, sampleRow, unqueField, sampleRow[0].ToString());
                string whereCompareReverse = GetWhereCompare(null, orderByTypes, sampleRow, unqueField, sampleRow[0].ToString(), true);

                sqlOtherPage = GetSampleIndexSql(unqueField, sql, fullOrderByString, whereCompare, sqlOtherPage, sampleRow);
                var innerSampleRowIndex = Taskable<int>(sqlOtherPage, whereObj).Count();
                var isSet = sampleRowIndex == 0 || (Math.Abs(pageBegin - innerSampleRowIndex) < (Math.Abs(pageBegin - sampleRowIndex)));
                if (isSet)
                {
                    sampleRowIndex = innerSampleRowIndex;
                    sampleEachIndex = i;
                }
            }
        } 
        #endregion

     
        #endregion
    }
}
