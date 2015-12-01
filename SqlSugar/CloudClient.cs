using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class CloudClient : IDisposable, IClient
    {
        private CloudClient()
        {

        }
        public List<SqlSugarClient> sqlSugarClientList = new List<SqlSugarClient>();

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

        public bool Delete<T, FiledType>(params FiledType[] whereIn)
        {

            throw new NotImplementedException();
        }

        public bool Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public bool FalseDelete<T, FiledType>(string field, params FiledType[] whereIn)
        {
            throw new NotImplementedException();
        }

        public bool FalseDelete<T>(string field, System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public bool IsNoLock
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Queryable<T> Queryable<T>() where T : new()
        {
            throw new NotImplementedException();
        }

        public void RemoveAllCache()
        {
            throw new NotImplementedException();
        }

        public Sqlable Sqlable()
        {
            throw new NotImplementedException();
        }

        public List<T> SqlQuery<T>(string sql, object whereObj = null)
        {
            throw new NotImplementedException();
        }

        public bool Update<T, FiledType>(object rowObj, params FiledType[] whereIn) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(object rowObj, System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
