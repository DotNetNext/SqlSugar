using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;

namespace SqlSugar
{
    public class SqlSugarClient : SqlHelper
    {

        public SqlSugarClient(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// 创建sql的对象
        /// </summary>
        public Sqlable Sqlable = new Sqlable();

        public List<T> SqlQuery<T>(string sql,object whereObj=null)
        {
            DataTable dt = null;
            if (whereObj != null)
            {
                dt = GetDataTable(sql,SqlTool.GetObjectToParameters(whereObj));
            }
            else {
                dt = GetDataTable(sql);
            }
            var reval = SqlTool.List<T>(dt);
            return reval;
        }

        /// <summary>
        /// 插入
        /// 使用说明:sqlSugar.Insert(entity);
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">插入对象</param>
        /// <param name="isIdentity">主键是否为自增长,true可以不填,false必填</param>
        /// <returns></returns>
        public object Insert<TEntity>(TEntity entity, bool isIdentity = true) where TEntity : class
        {

            Type type = entity.GetType();
            StringBuilder sbInsertSql = new StringBuilder();
            List<SqlParameter> pars = new List<SqlParameter>();

            //sql语句缓存
            string cacheSqlKey = "db.Insert." + type.Name;
            var cacheSqlManager = CacheManager<StringBuilder>.GetInstance();

            //属性缓存
            string cachePropertiesKey = "db." + type.Name + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();


            PropertyInfo[] props = null;
            if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
            {
                props = cachePropertiesManager[cachePropertiesKey];
            }
            else
            {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }

            if (cacheSqlManager.ContainsKey(cacheSqlKey))
            {
                sbInsertSql = cacheSqlManager[cacheSqlKey];
            }
            else
            {

                var primaryKeyName = string.Empty;

                //2.获得实体的属性集合 


                //实例化一个StringBuilder做字符串的拼接 


                sbInsertSql.Append("insert into " + type.Name + " (");

                //3.遍历实体的属性集合 
                foreach (PropertyInfo prop in props)
                {
                    if (props.First() == prop)
                    {
                        primaryKeyName = prop.Name;
                    }

                    //EntityState,@EntityKey
                    if (isIdentity == false || (isIdentity && prop.Name != primaryKeyName))
                    {
                        //4.将属性的名字加入到字符串中 
                        sbInsertSql.Append(prop.Name + ",");
                    }
                }
                //**去掉最后一个逗号 
                sbInsertSql.Remove(sbInsertSql.Length - 1, 1);
                sbInsertSql.Append(" ) values(");

            }

            //5.再次遍历，形成参数列表"(@xx,@xx@xx)"的形式 
            foreach (PropertyInfo prop in props)
            {
                //EntityState,@EntityKey
                if (isIdentity == false || (isIdentity && prop.Name != props[0].Name))
                {
                    if (!cacheSqlManager.ContainsKey(cacheSqlKey))
                        sbInsertSql.Append("@" + prop.Name + ",");
                    object val = prop.GetValue(entity, null);
                    if (val == null)
                        val = DBNull.Value;
                    pars.Add(new SqlParameter("@" + prop.Name, val));
                }
            }
            if (!cacheSqlManager.ContainsKey(cacheSqlKey))
            {
                //**去掉最后一个逗号 
                sbInsertSql.Remove(sbInsertSql.Length - 1, 1);
                sbInsertSql.Append(");select @@identity;");
                cacheSqlManager.Add(cacheSqlKey, sbInsertSql, cacheSqlManager.Day);
            }
            var sql = sbInsertSql.ToString();
            var lastInsertRowId = GetScalar(sql, pars.ToArray());
            return lastInsertRowId;

        }

        /// <summary>
        /// 更新
        /// 使用说明:sqlSugar.Update<T>(rowObj,whereObj});
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="rowObj">new {name="张三",sex="男"}</param>
        /// <param name="whereObj">new {id=100}</param>
        /// <returns></returns>
        public bool Update<TEntity>(object rowObj, object whereObj) where TEntity : class
        {
            if (rowObj == null) { throw new ArgumentNullException("SqlSugarClient.Update.rowObj"); }
            if (whereObj == null) { throw new ArgumentNullException("SqlSugarClient.Update.whereObj"); }


            Type type = typeof(TEntity);
            string sql = string.Format(" update {0} set ", type.Name);
            Dictionary<string, string> rows = SqlTool.GetObjectToDictionary(rowObj);
            foreach (var r in rows)
            {
                sql += string.Format(" {0} =@{0}  ,", r.Key);
            }
            sql = sql.TrimEnd(',');
            sql += string.Format(" where  1=1  ");
            Dictionary<string, string> wheres = SqlTool.GetObjectToDictionary(whereObj);
            foreach (var r in wheres)
            {
                sql += string.Format(" and {0} =@{0}", r.Key);
            }
            List<SqlParameter> parsList = new List<SqlParameter>();
            parsList.AddRange(rows.Select(c => new SqlParameter("@" + c.Key, c.Value)));
            parsList.AddRange(wheres.Select(c => new SqlParameter("@" + c.Key, c.Value)));
            var updateRowCount = ExecuteCommand(sql, parsList.ToArray());
            return updateRowCount > 0;
        }

        /// <summary>
        /// 批量删除(注意：where field 为class中的第一个属性)
        /// 用法:
        /// Delete<T>(new int[]{1,2,3})
        /// 或者
        /// Delete<T>(3)
        /// </summary>
        /// <param name="oc"></param>
        /// <param name="whereIn">in的集合</param>
        /// <param name="whereIn">主键为实体的第一个属性</param>
        public bool Delete<TEntity>(params dynamic[] whereIn)
        {
            Type type = typeof(TEntity);
            //属性缓存
            string cachePropertiesKey = "db." + type.Name + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
            PropertyInfo[] props = null;
            if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
            {
                props = cachePropertiesManager[cachePropertiesKey];
            }
            else
            {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }
            string key = type.FullName;
            bool isSuccess = false;
            if (whereIn != null && whereIn.Length > 0)
            {
                string sql = string.Format("delete from {0} where {1} in ({2})", type.Name, props[0].Name, SqlTool.ToJoinSqlInVal(whereIn));
                int deleteRowCount = ExecuteCommand(sql);
                isSuccess = deleteRowCount > 0;
            }
            return isSuccess;
        }


        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void RemoveAllCache()
        {
            CacheManager<int>.GetInstance().RemoveAll(c => c.Contains("SqlSugar."));
        }

    }
    /// <summary>
    /// sql查询类
    /// </summary>
    public class Queryable<T> where T : new()
    {
        public SqlHelper SqlHelper { get; set; }
    }
    /// <summary>
    /// sql创建类
    /// </summary>
    public class Sqlable
    {
        /// <summary>
        /// sql
        /// </summary>
        public StringBuilder Sql { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public MappingCurrentState? MappingCurrentState { get; set; }
        /// <summary>
        /// 是否允许脏读
        /// </summary>
        public bool IsNoLock { get; set; }
    }

    public enum MappingCurrentState
    {
        MappingTable = 0,
        Where = 1,
        Select = 2,
        SingleTable = 3
    }

}
