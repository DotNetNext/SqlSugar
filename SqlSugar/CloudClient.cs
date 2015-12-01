using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：SQL糖 ORM 核心类升级版 分布式存储，运算
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class CloudClient : IDisposable, IClient
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
                    return db.FalseDelete<T,FiledType>(field,whereIn);

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
                    return db.FalseDelete<T>(field,expression);

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

       
        public List<T> SqlQuery<T>(string sql, object whereObj = null)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
