using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using System.Linq.Expressions;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：SQL糖 ORM 核心类
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：http://www.cnblogs.com/sunkaixuan/p/4649904.html
    /// </summary>
    public class SqlSugarClient : SqlHelper
    {

        public SqlSugarClient(string connectionString)
            : base(connectionString)
        {
            ConnectionString = connectionString;
            IsNoLock = false;
        }
        public string ConnectionString { get; set; }

        /// <summary>
        /// 查询是否允许脏读，（默认为:true）
        /// </summary>
        public bool IsNoLock { get; set; }

        /// <summary>
        /// 创建多表查询对象
        /// </summary>
        public Sqlable Sqlable()
        {
            return new Sqlable() { DB = this };
        }

        /// <summary>
        /// 创建单表查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Queryable<T> Queryable<T>() where T : new()
        {
            return new Queryable<T>() { DB = this };
        }

        /// <summary>
        /// 根据SQL语句将结果集映射到List《T》
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        public List<T> SqlQuery<T>(string sql, object whereObj = null)
        {
            SqlDataReader reader = null;
            var pars = SqlSugarTool.GetParameters(whereObj);
            var type = typeof(T);
            reader = GetReader(sql, pars);
            if (type.IsIn(SqlSugarTool.IntType, SqlSugarTool.StringType))
            {
                List<T> strReval = new List<T>();
                using (SqlDataReader re = reader)
                {
                    while (re.Read())
                    {
                        strReval.Add((T)Convert.ChangeType(re.GetValue(0), type));
                    }
                }
                return strReval;
            }
            string fields = sql;
            if (sql.Length > 51)
            {
                fields = sql.Substring(0, 50);
            }
            var reval = SqlSugarTool.DataReaderToList<T>(type, reader, fields);
            return reval;
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
                    var par = new SqlParameter("@" + prop.Name, val);
                    if (par.SqlDbType == SqlDbType.Udt)
                    {
                        par.UdtTypeName = "HIERARCHYID";
                    }
                    pars.Add(par);
                }
            }
            if (!cacheSqlManager.ContainsKey(cacheSqlKey))
            {
                //**去掉最后一个逗号 
                sbInsertSql.Remove(sbInsertSql.Length - 1, 1);
                if (isIdentity == false)
                {
                    sbInsertSql.Append(");select 'true';");
                }
                else
                {
                    sbInsertSql.Append(");select @@identity;");
                }
                cacheSqlManager.Add(cacheSqlKey, sbInsertSql, cacheSqlManager.Day);
            }
            var sql = sbInsertSql.ToString();
            var lastInsertRowId = GetScalar(sql, pars.ToArray());
            return lastInsertRowId;

        }


        /// <summary>
        /// 更新
        /// 注意：rowObj为T类型将更新该实体的非主键所有列，如果rowObj类型为匿名类将更新指定列
        /// 使用说明:sqlSugar.Update《T》(rowObj,whereObj);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowObj">new T(){name="张三",sex="男"}或者new {name="张三",sex="男"}</param>
        /// <param name="expression">new {id=100}</param>
        /// <returns></returns>
        public bool Update<T>(object rowObj, Expression<Func<T, bool>> expression) where T : class
        {
            if (rowObj == null) { throw new ArgumentNullException("SqlSugarClient.Update.rowObj"); }
            if (expression == null) { throw new ArgumentNullException("SqlSugarClient.Update.expression"); }


            Type type = typeof(T);
            StringBuilder sbSql = new StringBuilder(string.Format(" UPDATE {0} SET ", type.Name));
            Dictionary<string, string> rows = SqlSugarTool.GetObjectToDictionary(rowObj);
            string pkName = SqlSugarTool.GetPrimaryKeyByTableName(this, type.Name);
            foreach (var r in rows)
            {
                if (pkName == r.Key)
                {
                    if (rowObj.GetType() == type)
                    {
                        continue;
                    }
                }
                sbSql.Append(string.Format(" {0} =@{0}  ,", r.Key));
            }
            sbSql.Remove(sbSql.Length - 1, 1);
            sbSql.Append(" WHERE  1=1  ");
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);
            sbSql.Append(re.SqlWhere); ;

            List<SqlParameter> parsList = new List<SqlParameter>();
            parsList.AddRange(re.Paras);
            var pars = rows.Select(c => new SqlParameter("@" + c.Key, c.Value));
            if (pars != null)
            {
                foreach (var par in pars)
                {
                    if (par.SqlDbType == SqlDbType.Udt)
                    {
                        par.UdtTypeName = "HIERARCHYID";
                    }
                    parsList.Add(par);
                }
            }
            var updateRowCount = ExecuteCommand(sbSql.ToString(), parsList.ToArray());
            return updateRowCount > 0;
        }

        /// <summary>
        /// 删除,根据表达示
        /// 使用说明:
        /// Delete《T》(it=>it.id=100) 或者Delete《T》(3)
        /// </summary>
        /// <param name="expression">筛选表达示</param>
        public bool Delete<T>(Expression<Func<T, bool>> expression)
        {
            Type type = typeof(T);
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);
            string sql = string.Format("DELETE FROM {0} WHERE 1=1 {1}", type.Name, re.SqlWhere);
            bool isSuccess = ExecuteCommand(sql, re.Paras.ToArray()) > 0;
            return isSuccess;
        }

        /// <summary>
        /// 批量删除
        /// 注意：whereIn 主键集合  
        /// 使用说明:Delete《T》(new int[]{1,2,3}) 或者  Delete《T》(3)
        /// </summary>
        /// <param name="whereIn"> delete ids </param>
        public bool Delete<T>(params dynamic[] whereIn)
        {
            Type type = typeof(T);
            //属性缓存
            string cachePropertiesKey = "db." + type.Name + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
            PropertyInfo[] props = SqlSugarTool.GetGetPropertiesByCache(type, cachePropertiesKey, cachePropertiesManager);
            string key = type.FullName;
            bool isSuccess = false;
            if (whereIn != null && whereIn.Length > 0)
            {
                string sql = string.Format("DELETE FROM {0} WHERE {1} IN ({2})", type.Name, SqlSugarTool.GetPrimaryKeyByTableName(this, type.Name), whereIn.ToJoinSqlInVal());
                int deleteRowCount = ExecuteCommand(sql);
                isSuccess = deleteRowCount > 0;
            }
            return isSuccess;
        }
        /// <summary>
        /// 批量删除
        /// 注意：whereIn field 为class中的第一个属性
        /// 使用说明:Delete《T》(new int[]{1,2,3}) 或者  Delete《T》(3)
        /// </summary>
        /// <param name="whereIn"> delete ids </param>
        public bool Delete<T>(params int[] whereIn)
        {
            Type type = typeof(T);
            //属性缓存
            string cachePropertiesKey = "db." + type.Name + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
            PropertyInfo[] props = SqlSugarTool.GetGetPropertiesByCache(type, cachePropertiesKey, cachePropertiesManager);
            string key = type.FullName;
            bool isSuccess = false;
            if (whereIn != null && whereIn.Length > 0)
            {
                string sql = string.Format("DELETE FROM {0} WHERE {1} IN ({2})", type.Name, props[0].Name, whereIn.ToJoinSqlInVal());
                int deleteRowCount = ExecuteCommand(sql);
                isSuccess = deleteRowCount > 0;
            }
            return isSuccess;
        }
        /// <summary>
        /// 批量删除
        /// 注意：whereIn field 为class中的第一个属性
        /// 使用说明:Delete《T》(new int[]{1,2,3}) 或者  Delete《T》(3)
        /// </summary>
        /// <param name="whereIn"> delete ids </param>
        public bool Delete<T>(params Guid[] whereIn)
        {
            Type type = typeof(T);
            //属性缓存
            string cachePropertiesKey = "db." + type.Name + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
            PropertyInfo[] props = SqlSugarTool.GetGetPropertiesByCache(type, cachePropertiesKey, cachePropertiesManager);
            string key = type.FullName;
            bool isSuccess = false;
            if (whereIn != null && whereIn.Length > 0)
            {
                string sql = string.Format("DELETE FROM {0} WHERE {1} IN ({2})", type.Name, props[0].Name, whereIn.ToJoinSqlInVal());
                int deleteRowCount = ExecuteCommand(sql);
                isSuccess = deleteRowCount > 0;
            }
            return isSuccess;
        }

        /// <summary>
        /// 批量假删除
        /// 注意：whereIn field 为class中的第一个属性
        /// 使用说明::
        /// FalseDelete《T》("is_del",new int[]{1,2,3})或者Delete《T》("is_del",3)
        /// </summary>
        /// <param name="field">更新删除状态字段</param>
        /// <param name="whereIn">delete ids</param>
        public bool FalseDelete<T>(string field, params dynamic[] whereIn)
        {
            Type type = typeof(T);
            //属性缓存
            string cachePropertiesKey = "db." + type.Name + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
            PropertyInfo[] props = SqlSugarTool.GetGetPropertiesByCache(type, cachePropertiesKey, cachePropertiesManager);
            bool isSuccess = false;
            if (whereIn != null && whereIn.Length > 0)
            {
                string sql = string.Format("UPDATE  {0} SET {3}=0 WHERE {1} IN ({2})", type.Name, props[0].Name, whereIn.ToJoinSqlInVal(), field);
                int deleteRowCount = ExecuteCommand(sql);
                isSuccess = deleteRowCount > 0;
            }
            return isSuccess;
        }

        /// <summary>
        /// 假删除，根据表达示
        /// 使用说明::
        /// FalseDelete《T》(new int[]{1,2,3})或者Delete《T》(3)
        /// </summary>
        /// <param name="field">更新删除状态字段</param>
        /// <param name="expression">筛选表达示</param>
        public bool FalseDelete<T>(string field, Expression<Func<T, bool>> expression)
        {
            Type type = typeof(T);
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
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);
            string sql = string.Format("UPDATE  {0} SET {1}=0 WHERE  1=1 {2}", type.Name, field, re.SqlWhere);
            int deleteRowCount = ExecuteCommand(sql, re.Paras.ToArray());
            isSuccess = deleteRowCount > 0;
            return isSuccess;
        }

        /// <summary>
        /// 生成实体的对象
        /// </summary>
        public ClassGenerating ClassGenerating = new ClassGenerating();

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void RemoveAllCache()
        {
            CacheManager<int>.GetInstance().RemoveAll(c => c.Contains("SqlSugar."));
        }

    }

}
