using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：SQL糖 ORM 核心类升级版 分布式存储，运算
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public partial class CloudClient : IDisposable, IClient
    {
        public bool IsNoLock { get; set; }
        private CloudClient()
        {

        }
        private List<SqlSugarClient> sqlSugarClientList = new List<SqlSugarClient>();

        private List<CloudConnectionConfig> configList { get; set; }

        /// <summary>
        /// 实例
        /// </summary>
        /// <param name="configList">云计算连接配置</param>
        public CloudClient(List<CloudConnectionConfig> configList)
        {
            this.configList = configList;
        }


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
            sqlSugarClientList.Add(db);
            return db.Insert<T>(entity);
        }


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
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
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
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
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
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
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
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
                    return db.FalseDelete<T>(field, expression);

                }, tasks, i);
            }
            Task.WaitAll(tasks);
            return tasks.Any(it => it.Result);
        }



        public void RemoveAllCache()
        {
            var connName = configList[0].ConnectionName;
            var db = new SqlSugarClient(connName);
            db.RemoveAllCache();
        }


        public T GetMax<T>(string sql, object whereObj = null)
        {
            var tasks = new Task<T>[configList.Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<T>(ti =>
                {
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
                    var obj = db.GetScalar(sql);
                    obj = Convert.ChangeType(obj, typeof(T));
                    return (T)obj;

                }, tasks, i);
            }
            Task.WaitAll(tasks);
            return tasks.Max(it => it.Result);
        }

        public T GetMin<T>(string sql, object whereObj = null)
        {
            var tasks = new Task<T>[configList.Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<T>(ti =>
                {
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
                    var obj = db.GetScalar(sql);
                    obj = Convert.ChangeType(obj, typeof(T));
                    return (T)obj;

                }, tasks, i);
            }
            Task.WaitAll(tasks);
            return tasks.Min(it => it.Result);
        }


        public int GetCount<T>(string sql, object whereObj = null)
        {
            var tasks = new Task<int>[configList.Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<int>(ti =>
                {
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
                    var obj = db.GetScalar(sql);
                    obj = Convert.ChangeType(obj, typeof(int));
                    return (int)obj;

                }, tasks, i);
            }
            Task.WaitAll(tasks);
            return tasks.Select(it => it.Result).Sum(); ;
        }




        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">不包含分页逻辑,Order by只有一个字段Order by逻辑不能写在该参数里面应该写在Order By参数里面，如果要实现多级排序可以写成这样 ：string sql="select top "+int.MaxValue+" from table order by 二级排序字段 desc "</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderByField">主排序字段名要和实体类一致区分大小写</param>
        /// <param name="orderByType">排序类型</param>
        /// <param name="whereObj">参数 例如: new { id="1",name="张三"}</param>
        /// <returns></returns>
        public List<T> GetListByPage<T>(string sql, int pageIndex, int pageSize, ref int pageCount, string orderByField, OrderByType orderByType, object whereObj = null)
        {

            if (pageIndex == 0)
                pageIndex = 1;

            /***count***/
            int configCount = configList.Count;
            var tasks = new Task<CloudSearch<T>>[configCount];
            GetListByPage_GetPageCount<T>(tasks);
            Task.WaitAll(tasks);
            pageCount = tasks.Sum(it => it.Result.Count);


            /***one nodes***/
            var isOneNode = configCount == 1;
            if (isOneNode)
            {
                var connName = configList[0].ConnectionName;
                var db = new SqlSugarClient(connName);
                var sqlPage = string.Format(@"SELECT * FROM (
                                                                        SELECT *,ROW_NUMBER()OVER(ORDER BY {1}) AS  ROWINDEX  FROM ({0}) as sqlstr ) t WHERE t.rowIndex BETWEEN {2} AND {3}
                                             ", sql, orderByField + " " + orderByType.ToString(), (pageIndex - 1) * pageSize + 1, pageSize * pageIndex);
                sqlSugarClientList.Add(db);
                return db.SqlQuery<T>(sql, whereObj);
            }

            string fullOrderByString = orderByField + " " + orderByType.ToString();
            /***small data***/
            var isSmallData = pageCount <= 1000;
            if (isSmallData)
            {
                tasks = new Task<CloudSearch<T>>[configCount];
                GetListByPage_SmallData<T>(sql, tasks, fullOrderByString, whereObj);
                Task.WaitAll(tasks);
                if (orderByType == OrderByType.asc)
                {
                    return tasks.SelectMany(it => it.Result.TEntities).OrderBy(it => SqlSugarTool.GetPropertyValue(it, orderByField)).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    return tasks.SelectMany(it => it.Result.TEntities).OrderByDescending(it => SqlSugarTool.GetPropertyValue(it, orderByField)).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
            }

            /***small index***/
            var isSmallPageIndex = CloudPubMethod.GetIsSmallPageIndex(pageIndex, pageSize, configCount);
            if (isSmallPageIndex)
            {
                tasks = new Task<CloudSearch<T>>[configCount];
                GetListByPage_SmallPageIndex<T>(sql, pageSize, fullOrderByString, whereObj, configCount, tasks);
                Task.WaitAll(tasks);
                if (orderByType == OrderByType.asc)
                {
                    return tasks.SelectMany(it => it.Result.TEntities).OrderBy(it => SqlSugarTool.GetPropertyValue(it, orderByField)).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    return tasks.SelectMany(it => it.Result.TEntities).OrderByDescending(it => SqlSugarTool.GetPropertyValue(it, orderByField)).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
            }

            /***other***/
            tasks = new Task<CloudSearch<T>>[configCount];
            GetListByPage_OtherOne<T>(sql, pageIndex, pageSize, orderByField, orderByType, whereObj, configCount, tasks);
            Task.WaitAll(tasks);
            var dataTable = tasks.SelectMany(it => it.Result.DataTable.AsEnumerable()).CopyToDataTable();
            var connectionList=dataTable.AsEnumerable().Skip(0).Take(pageSize).Select(it => it["CONNECTIONNAME"].ToString()).ToList();
            connectionList.Distinct();
            tasks = new Task<CloudSearch<T>>[connectionList.Count];


            return null;
        }


        private void GetListByPage_OtherOne<T>(string sql, int pageIndex, int pageSize, string orderByField, OrderByType orderByType, object whereObj, int configCount, Task<CloudSearch<T>>[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<CloudSearch<T>>(ti =>
                {
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
                    var sqlPage = string.Format(@"SELECT ROWINDEX,{4} AS ORDERBYVALUE,{5} CONNECTIONNAME FROM (
                                                                            SELECT *,ROW_NUMBER()OVER(ORDER BY {1}) AS  ROWINDEX  FROM ({0}) as sqlstr ) t WHERE t.rowIndex BETWEEN {2} AND {3}
                                                                            ", sql, orderByField == null ? " GETDATE()" : orderByField + " " + orderByType.ToString(), (pageIndex - 1) * pageSize + 1, pageIndex * pageSize, orderByField);
                    CloudSearch<T> reval = new CloudSearch<T>();
                    reval.DataTable = db.GetDataTable(sqlPage, whereObj);
                    var dv = reval.DataTable.AsDataView();
                    dv.Sort = orderByField + " " + orderByType.ToString();
                    reval.DataTable = dv.ToTable();
                    return reval;
                }, tasks, i);
            }
        }

        private void GetListByPage_SmallPageIndex<T>(string sql, int pageSize, string orderBy, object whereObj, int configCount, Task<CloudSearch<T>>[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<CloudSearch<T>>(ti =>
                {
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
                    var countPage = string.Format(@"SELECT * FROM (
                                                                            SELECT *,ROW_NUMBER()OVER(ORDER BY {1}) AS  ROWINDEX  FROM ({0}) as sqlstr ) t WHERE t.rowIndex BETWEEN {2} AND {3}
                                                                            ", sql, orderBy == null ? " GETDATE()" : orderBy, 1, pageSize * configCount);
                    //data
                    var list = db.SqlQuery<T>(sql, whereObj);
                    CloudSearch<T> reval = new CloudSearch<T>();
                    reval.TEntities = list;
                    return reval;

                }, tasks, i);
            }
        }

        private void GetListByPage_SmallData<T>(string sql, Task<CloudSearch<T>>[] tasks, string orderBy = null, object whereObj = null)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<CloudSearch<T>>(ti =>
                {
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
                    CloudSearch<T> reval = new CloudSearch<T>();
                    reval.TEntities = db.SqlQuery<T>(sql + " ORDER BY " + orderBy, whereObj);
                    return reval;

                }, tasks, i);
            }
        }

        private void GetListByPage_GetPageCount<T>(Task<CloudSearch<T>>[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                CloudPubMethod.TaskFactory<CloudSearch<T>>(ti =>
                {
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
                    var countSql = string.Format("SELECT COUNT(*) FROM ({0}) t ");
                    var count = Convert.ToInt32(db.GetScalar(countSql));
                    CloudSearch<T> reval = new CloudSearch<T>();
                    reval.Count = count;
                    return reval;

                }, tasks, i);
            }
        }




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
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
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
                    var connName = configList[ti].ConnectionName;
                    var db = new SqlSugarClient(connName);
                    sqlSugarClientList.Add(db);
                    return db.Update<T>(rowObj, expression);

                }, tasks, i);
            }
            Task.WaitAll(tasks);
            return tasks.Any(it => it.Result);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            //关闭连接
            for (int i = 0; i < sqlSugarClientList.Count; i++)
            {
                var it = sqlSugarClientList[i];
                it.Dispose();
                it = null;
            }
            this.configList = null;
            sqlSugarClientList = null;
        }
    }
}
