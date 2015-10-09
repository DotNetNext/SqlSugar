using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq.Expressions;


namespace SqlSugar
{
    /// <summary>
    /// ** 描述：SqlSugar工具类
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    internal class SqlSugarTool
    {
        public static Type StringType = typeof(string);
        public static Type IntType = typeof(int);

        /// <summary>
        /// Reader转成List《T》
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dr"></param>
        /// <param name="isClose"></param>
        /// <returns></returns>
        public static List<T> DataReaderToList<T>(Type type, IDataReader dr, bool isClose = true)
        {
            var cacheManager = CacheManager<IDataReaderEntityBuilder<T>>.GetInstance();
            string key = "DataReaderToList." + type.FullName;
            IDataReaderEntityBuilder<T> eblist = null;
            if (cacheManager.ContainsKey(key))
            {
                eblist = cacheManager[key];
            }
            else
            {
                eblist = IDataReaderEntityBuilder<T>.CreateBuilder(type, dr);
                cacheManager.Add(key, eblist, cacheManager.Day);
            }
            List<T> list = new List<T>();
            try
            {
                if (dr == null) return list;
                while (dr.Read())
                {
                    list.Add(eblist.Build(dr));
                }
                if (isClose) { dr.Close(); dr.Dispose(); dr = null; }
            }
            catch (Exception)
            {
                if (isClose) { dr.Close(); dr.Dispose(); dr = null; }
            }
            return list;
        }

        /// <summary>
        /// 将实体对象转换成SqlParameter[] 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SqlParameter[] GetParameters(object obj)
        {
            List<SqlParameter> listParams = new List<SqlParameter>();
            if (obj != null)
            {
                var type = obj.GetType();
                var propertiesObj = type.GetProperties();
                string replaceGuid = Guid.NewGuid().ToString();
                foreach (PropertyInfo r in propertiesObj)
                {
                    var value = r.GetValue(obj, null);
                    if (value == null) value = DBNull.Value;
                    listParams.Add(new SqlParameter("@" + r.Name, value.ToString()));
                }
            }
            return listParams.ToArray();
        }

        /// <summary>
        /// 将实体对象转换成 Dictionary《string, string》
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetObjectToDictionary(object obj)
        {

            Dictionary<string, string> reval = new Dictionary<string, string>();
            if (obj == null) return reval;
            var type = obj.GetType();
            var propertiesObj = type.GetProperties();
            string replaceGuid = Guid.NewGuid().ToString();
            foreach (PropertyInfo r in propertiesObj)
            {
                var val = r.GetValue(obj, null);
                reval.Add(r.Name, val == null ? "" : val.ToString());
            }

            return reval;
        }

        /// <summary>
        /// 获取type属性cache
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cachePropertiesKey"></param>
        /// <param name="cachePropertiesManager"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetGetPropertiesByCache(Type type, string cachePropertiesKey, CacheManager<PropertyInfo[]> cachePropertiesManager)
        {
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
            return props;
        }


        /// <summary>
        /// 判段是否包含主键
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool IsPrimaryKey(SqlSugarClient db, string tableName)
        {
            return GetPrimaryKeyByTableName(db, tableName) != null;
        }

        /// <summary>
        /// 根据表获取主键
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetPrimaryKeyByTableName(SqlSugarClient db, string tableName)
        {
            string key = "GetPrimaryKeyByTableName";
            tableName = tableName.ToLower();
            var cm = CacheManager<List<KeyValue>>.GetInstance();
            List<KeyValue> primaryInfo = null;

            //获取主键信息
            if (cm.ContainsKey(key))
                primaryInfo = cm[key];
            else
            {
                string sql = @"  with tb as(
                                        select * from sysobjects where xtype='U'  
                                        ),
                                        pk as(
                                         SELECT TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
                                        )
                                        select pk.COLUMN_NAME as keyName,tb.name as tableName from tb inner join pk on tb.name=pk.TABLE_NAME";
                var dt = db.GetDataTable(sql);
                primaryInfo = new List<KeyValue>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        primaryInfo.Add(new KeyValue() { Key = dr["tableName"].ToString().ToLower(), Value = dr["keyName"].ToString() });
                    }
                }
                cm.Add(key, primaryInfo, cm.Day);
            }

            //反回主键
            if (!primaryInfo.Any(it=>it.Key==tableName))
            {
                return null;
            }
            return primaryInfo.First(it=>it.Key==tableName).Value;

        }
    }
}
