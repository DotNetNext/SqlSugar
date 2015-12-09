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
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public partial class CloudClient : IDisposable, IClient
    {
        private Object tranLock = new Object();
        private List<SqlSugarClient> dbs = new List<SqlSugarClient>();

        /// <summary>
        /// 分布式事务
        /// </summary>
        public CommittableTransaction Tran = null;

        /// <summary>
        /// 内存中处理数据的最大值（默认：1000）
        /// </summary>
        public int PageMaxHandleNumber = 1000;
        private CloudClient()
        {

        }


        private List<CloudConnectionConfig> configList { get; set; }

        /// <summary>
        /// 实例
        /// </summary>
        /// <param name="configList">云计算连接配置</param>
        public CloudClient(List<CloudConnectionConfig> configList)
        {
            this.configList = configList;
        }

        #region insert

        /// <summary>
        /// 批量插入
        /// 使用说明:sqlSugar.Insert(List《entity》);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">插入对象</param>
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

        #region delete
        /// <summary>
        /// 批量删除
        /// 注意：whereIn 主键集合  
        /// 使用说明:Delete《T》(new int[]{1,2,3}) 或者  Delete《T》(3)
        /// </summary>
        /// <param name="whereIn"> delete ids </param>
        public bool Delete<T, FiledType>(params FiledType[] whereIn)
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
        /// 删除,根据表达示
        /// 使用说明:
        /// Delete《T》(it=>it.id=100) 或者Delete《T》(3)
        /// </summary>
        /// <param name="expression">筛选表达示</param>
        public bool Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression)
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
        /// <param name="whereIn"> delete ids </param>
        public bool FalseDelete<T, FiledType>(string field, params FiledType[] whereIn)
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
        /// 假删除，根据表达示
        /// 使用说明::
        /// FalseDelete《T》(new int[]{1,2,3})或者Delete《T》(3)
        /// </summary>
        /// <param name="field">更新删除状态字段</param>
        /// <param name="expression">筛选表达示</param>
        public bool FalseDelete<T>(string field, System.Linq.Expressions.Expression<Func<T, bool>> expression)
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
                        obj = Convert.ChangeType(obj, typeof(T));
                        itemReval.Value = (T)obj;
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
        #region page

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        ///<typeparam name="unqField">数据库中数据唯一的列（建议：主键GUID）</typeparam>
        /// <param name="sql">不包含分页逻辑,Order by如果只有一个字段不能写在该参数里面,应该写在Order By参数里面，如果要实现多级排序可以写成这样 ：string sql="select top "+int.MaxValue+" from table order by 二级排序字段 desc "</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderByField">主排序字段名要和实体类一致,区分大小写</param>
        /// <param name="orderByType">排序类型</param>
        /// <param name="whereObj">参数 例如: new { id="1",name="张三"}</param>
        /// <returns></returns>
        public List<T> TaskableWithPage<T>(string unqueField, string sql, int pageIndex, int pageSize, ref int pageCount, string orderByField, OrderByType orderByType, object whereObj = null) where T : class
        {
            if (pageIndex == 0)
                pageIndex = 1;
            /***count***/
            int configCount = configList.Count;
            string sqlCount = string.Format("SELECT COUNT(*) FROM ({0}) t ", sql);
            pageCount = Taskable<int>(sqlCount, whereObj).Count();
            int totalPage = (pageCount + pageSize - 1) / pageSize;
            string fullOrderByString = orderByField + " " + orderByType.ToString();
            string fullOrderByStringReverse = orderByField + " " + (orderByType == OrderByType.asc ? OrderByType.desc : OrderByType.asc);

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
                                                         ", sql, orderByField + " " + orderByType.ToString(), (pageIndex - 1) * pageSize + 1, pageSize * pageIndex);
                var list = db.SqlQuery<T>(sql, whereObj);
                return list.OrderBy(orderByField, orderByType).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            #endregion

            /***small data***/
            #region small data
            var isSmallData = pageCount <= this.PageMaxHandleNumber || int.MaxValue == pageSize;//page size等于int.MaxValue不需要分页
            if (isSmallData)
            {
                var tasks = Taskable<T>(sql + " ORDER BY " + orderByField, whereObj);
                return tasks.Tasks.SelectMany(it => it.Result.Entities).OrderBy(orderByField, orderByType).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

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
                var tasks = Taskable<T>(sql, whereObj);
                return tasks.Tasks.SelectMany(it => it.Result.Entities).OrderBy(orderByField, orderByType).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            #endregion

            /***small index  by reverse***/
            #region small index  by reverse
            var lastPage = (totalPage - pageIndex) + 1;
            var isSmallPageIndexByReverse = CloudPubMethod.GetIsSmallPageIndexByReverse(totalPage, pageIndex, pageSize, configCount, this.PageMaxHandleNumber);
            if (isSmallPageIndexByReverse)
            {

                var sqlPage = string.Format(@"SELECT * FROM (
                                                                                        SELECT *,ROW_NUMBER()OVER(ORDER BY {1}) AS  ROWINDEX  FROM ({0}) as sqlstr ) t WHERE t.rowIndex BETWEEN {2} AND {3}
                                                                                        ", sql, fullOrderByStringReverse, 1, lastPage * configCount);
                var tasks = Taskable<T>(sql, whereObj);
                var lastPageSize = pageCount % pageSize;
                if (lastPageSize == 0) lastPageSize = pageSize;

                var list = tasks.Tasks.SelectMany(it => it.Result.Entities).OrderBy(orderByField, orderByType == OrderByType.asc ? OrderByType.desc : OrderByType.asc);
                var isLast = totalPage == pageIndex;
                if (isLast)
                {
                    return list.Skip(0).Take(lastPageSize).OrderBy(orderByField, orderByType).ToList();
                }
                else
                {
                    return list.Skip((lastPage - 1) * pageSize - lastPageSize - 1).Take(pageSize).OrderBy(orderByField, orderByType).ToList();
                }
            }
            #endregion

            /***other***/
            #region other
            //单节点最大索引
            var maxDataIndex = pageIndex * pageSize * configCount;
            //节点间距
            var nodeSPacing = 0;
            //分页最大索引
            var pageMaxIndex = pageIndex * pageSize;

            var dataSamplelList = new List<DataRow>();
            GetListByPage_GetDataSampleList<T>(dataSamplelList, unqueField, sql, pageMaxIndex, maxDataIndex, 0, pageIndex, pageSize, nodeSPacing, orderByField, whereObj, configCount, fullOrderByString, orderByType);
            var reval = GetListByPage_GetReval<T>(unqueField, sql, pageSize, pageIndex, orderByField, orderByType, whereObj, ref dataSamplelList);
            reval = reval.OrderBy(orderByField, orderByType).ThenBy(unqueField, OrderByType.desc).ToList();
            var revalCount = reval.Count;
            var revalSkipIndex = pageSize;
            reval = reval.Skip(reval.Count - pageSize).ToList();
            return reval; 
            #endregion
        }

        private List<T> GetListByPage_GetReval<T>(string unqField, string sql, int pageSize, int pageIndex, string orderByField, OrderByType orderByType, object whereObj, ref List<DataRow> dataSamplelList) where T : class
        {
            var isAsc = OrderByType.asc == orderByType;

            dataSamplelList = dataSamplelList.OrderByDataRow("OrderByField", orderByType).ThenByDataRow(unqField, OrderByType.desc).ToList();

            string fullOrderByString = orderByType == OrderByType.asc ? string.Format(" {0} {1} ", orderByField, OrderByType.desc) : string.Format(" {0} {1} ", orderByField, OrderByType.asc);
            var maxRow = isAsc ? dataSamplelList.OrderByDataRow("OrderByField", OrderByType.desc).ThenByDataRow(unqField, OrderByType.desc).First() : dataSamplelList.OrderByDataRow("OrderByField", OrderByType.asc).ThenByDataRow(unqField, OrderByType.desc).First();
            if (!sql.ToLower().Contains("where"))
            {
                sql += " WHERE 1=1 ";
            }
            sql = string.Format("{0} AND {1}<'{2}'  OR ({1}='{2}' AND  {3}<='{4}'  )", sql, orderByField, maxRow["OrderByField"], unqField, maxRow[unqField]);
            var connectionStringList = dataSamplelList.Skip(dataSamplelList.Count - 10).Take(10).Select(it => it["ConnectionString"].ToString()).ToList();
            sql = string.Format(@"SELECT * FROM (
                                                                                        SELECT *,ROW_NUMBER()OVER(ORDER BY {1}) AS  ROWINDEX  FROM ({0}) as sqlstr ) t WHERE t.rowIndex BETWEEN {2} AND {3}
                                                                                        ", sql, fullOrderByString, 1, pageSize * dataSamplelList.Count);
            var reval = Taskable<T>(sql, whereObj).MergeEntities().ToList();
            return reval;
        }
        /// <summary>
        /// 获取样品节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pageMaxIndex"></param>
        /// <param name="maxDataIndex"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="nodeSpacing"></param>
        /// <param name="orderByField"></param>
        /// <param name="whereObj"></param>
        /// <param name="configCount"></param>
        /// <param name="fullOrderByString"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        private List<DataRow> GetListByPage_GetDataSampleList<T>(List<DataRow> dataSamplelList, string unqField, string sql, int pageMaxIndex, int maxDataIndex, int minDataIndex, int pageIndex, int pageSize, int nodeSpacing, string orderByField, object whereObj, int configCount, string fullOrderByString, OrderByType orderByType) where T : class
        {

            if (nodeSpacing == 0)
            {
                nodeSpacing = pageSize;
                //保证节点间距样品数在10条以内
                nodeSpacing = GetListByPage_GetNodeSpacing(maxDataIndex, nodeSpacing);
            }
            //根据节点间距得到间距集合
            var nodeIndexList = GetListByPage_GetNodeIndexList(maxDataIndex, nodeSpacing, minDataIndex);
            //where in nodeIndexList
            var sqlPage = string.Format(@"SELECT {4},RowIndex,{3} as OrderByField,'$:->connectionString<-:$' as ConnectionString  FROM (
                                                                                        SELECT *,ROW_NUMBER()OVER(ORDER BY {1},{4}) AS  ROWINDEX  FROM ({0}) as sqlstr ) t WHERE t.rowIndex IN ({2})
                                                                                        ", sql, fullOrderByString, string.Join(",", nodeIndexList), orderByField, unqField);
            var innerDataSampleList = Taskable<DataTable>(sqlPage, whereObj).MergeTable().ToList();
            var isAsc = OrderByType.asc == orderByType;
            innerDataSampleList = innerDataSampleList.OrderByDataRow("OrderByField", orderByType).ThenByDataRow(unqField, OrderByType.desc).ToList();

            var sampleListTakeIndex = pageMaxIndex / nodeSpacing;
            var isSmallSpacing = nodeSpacing == pageSize;
            if (!isSmallSpacing && nodeSpacing != 1)//不是最小间距
            {
                if (pageMaxIndex % nodeSpacing == 0)
                {
                    sampleListTakeIndex = sampleListTakeIndex - nodeSpacing;
                }
                innerDataSampleList = innerDataSampleList.Skip(0).Take(sampleListTakeIndex).ToList();
                var connections = innerDataSampleList.GroupBy(it => it["ConnectionString"]).Select(it => it.Key.ToString()).ToList();
                int dataSampleCount = innerDataSampleList.Count();
                nodeSpacing = nodeSpacing / 10;
                dataSamplelList.AddRange(innerDataSampleList);
                GetListByPage_GetDataSampleList<T>(dataSamplelList, unqField, sql, pageMaxIndex, maxDataIndex, sampleListTakeIndex, pageIndex, pageSize, nodeSpacing, orderByField, whereObj, configCount, fullOrderByString, orderByType);
            }
            else if (isSmallSpacing)
            {
                if (pageMaxIndex % nodeSpacing == 0)
                {
                    sampleListTakeIndex = sampleListTakeIndex - nodeSpacing;
                }
                innerDataSampleList = innerDataSampleList.Skip(0).Take(sampleListTakeIndex).ToList();
                var connections = innerDataSampleList.GroupBy(it => it["ConnectionString"]).Select(it => it.Key.ToString()).ToList();
                int dataSampleCount = innerDataSampleList.Count();
                nodeSpacing = 1;
                dataSamplelList.AddRange(innerDataSampleList);
                GetListByPage_GetDataSampleList<T>(dataSamplelList, unqField, sql, pageMaxIndex, maxDataIndex, sampleListTakeIndex, pageIndex, pageSize, nodeSpacing, orderByField, whereObj, configCount, fullOrderByString, orderByType);
            }
            else if (nodeSpacing == 1)
            {//是最小间距

                innerDataSampleList = innerDataSampleList.OrderByDataRow("OrderByField", orderByType).ThenByDataRow(unqField, OrderByType.desc).Skip(0).Take(sampleListTakeIndex).ToList();
                dataSamplelList.AddRange(innerDataSampleList);
            }
            //已经获得所有样品节点
            return dataSamplelList;
        }
        /// <summary>
        /// 获取样品节点索引数组
        /// </summary>
        /// <param name="maxDataIndex"></param>
        /// <param name="nodeSPacing"></param>
        /// <param name="minDataIndex"></param>
        /// <returns></returns>
        private static List<int> GetListByPage_GetNodeIndexList(int maxDataIndex, int nodeSPacing, int minDataIndex)
        {
            List<int> reval = new List<int>();
            var oldNodeSPacing = nodeSPacing;
            if (minDataIndex == 0)
            {
                oldNodeSPacing = nodeSPacing;
                reval.Add(nodeSPacing);
            }
            else
            {
                nodeSPacing = minDataIndex;
            }
            while (nodeSPacing <= maxDataIndex)
            {
                nodeSPacing += oldNodeSPacing;
                reval.Add(nodeSPacing);

            }
            return reval.Distinct().ToList();
        }
        /// <summary>
        /// 获取节点间距
        /// </summary>
        /// <param name="maxDataIndex"></param>
        /// <param name="nodeSPacing"></param>
        /// <returns></returns>
        private static int GetListByPage_GetNodeSpacing(int maxDataIndex, int nodeSPacing)
        {

            if (maxDataIndex / nodeSPacing > 10)
            {
                nodeSPacing *= 10;
                return GetListByPage_GetNodeSpacing(maxDataIndex, nodeSPacing);
            }
            return nodeSPacing;
        }
        #endregion 
        #endregion

        #region update
        /// <summary>
        /// 更新
        /// 注意：rowObj为T类型将更新该实体的非主键所有列，如果rowObj类型为匿名类将更新指定列
        /// 使用说明:sqlSugar.Update《T》(rowObj,whereObj);
        /// </summary>
        /// <typeparam name="T"></typeparam>
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

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
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


        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void RemoveAllCache()
        {
            var connName = configList[0].ConnectionString;
            var db = new SqlSugarClient(connName);
            db.RemoveAllCache();
        }
        public void TranDispose()
        {
            Tran = null;
        }

        /// <summary>
        /// 设置连接
        /// </summary>
        /// <param name="db"></param>
        private void SettingConnection(SqlSugarClient db)
        {

            if (Tran != null)
            {
                try
                {
                    lock (this.tranLock)
                    {
                        lock (this.Tran)
                        {
                            db.GetConnection().EnlistTransaction(Tran);
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

            }
            lock (dbs)
            {
                dbs.Add(db);
            }
        }
    }
}
