using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：SqlSugar工具类
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public partial class SqlSugarTool
    {
        internal static Type StringType = typeof(string);
        internal static Type IntType = typeof(int);
        internal static Type DecType = typeof(decimal);
        internal static Type GuidType = typeof(Guid);
        internal static Type DateType = typeof(DateTime);
        internal static Type ByteType = typeof(Byte);
        internal static Type BoolType = typeof(bool);
        internal static Type ObjType = typeof(object);
        internal static Type Dob = typeof(double);
        internal static Type DicSS = typeof(KeyValuePair<string, string>);
        internal static Type DicSi = typeof(KeyValuePair<string, int>);
        internal static Type Dicii = typeof(KeyValuePair<int, int>);
        internal static Type DicOO = typeof(KeyValuePair<object, object>);
        internal static Type DicSo = typeof(KeyValuePair<string, object>);
        internal static Type DicIS = typeof(KeyValuePair<int, string>);
        internal static Type DicArraySS = typeof(Dictionary<string, string>);
        internal static Type DicArraySO = typeof(Dictionary<string, object>);

        /// <summary>
        ///  Reader转成T的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="dr"></param>
        /// <param name="fields"></param>
        /// <param name="isClose"></param>
        /// <returns></returns>
        internal static List<T> DataReaderToList<T>(Type type, IDataReader dr, string fields, bool isClose = true)
        {
            if (type.Name.Contains("KeyValuePair"))
            {
                List<T> strReval = new List<T>();
                FillValueTypeToDictionary(type, dr, strReval);
                return strReval;
            }
            //值类型
            else if (type.IsValueType || type == SqlSugarTool.StringType)
            {
                List<T> strReval = new List<T>();
                FillValueTypeToDr<T>(type, dr, strReval);
                return strReval;
            }
            //数组类型
            else if (type.IsArray)
            {
                List<T> strReval = new List<T>();
                FillValueTypeToArray(type, dr, strReval);
                return strReval;
            }


            var cacheManager = CacheManager<IDataReaderEntityBuilder<T>>.GetInstance();
            string key = "DataReaderToList." + fields + type.FullName;
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
            catch (Exception ex)
            {
                if (isClose) { dr.Close(); dr.Dispose(); dr = null; }
                Check.Exception(true, "错误信息：实体映射出错。\r\n错误详情：{0}", ex.Message);
            }
            return list;
        }

        private static void FillValueTypeToDr<T>(Type type, IDataReader dr, List<T> strReval)
        {
            using (IDataReader re = dr)
            {
                while (re.Read())
                {
                    var objValue = re.GetValue(0);
                    if (objValue != DBNull.Value)
                    {
                        strReval.Add((T)Convert.ChangeType(objValue, type));
                    }
                    else
                    {
                        strReval.Add(default(T));
                    }
                }
            }
        }


        /// <summary>
        /// 设置参数Size
        /// </summary>
        /// <param name="par"></param>
        public static void SetParSize(SqlParameter par)
        {
            int size = par.Size;
            if (size < 4000)
            {
                par.Size = 4000;
            }
        }

        /// <summary>
        /// 将实体对象转换成SqlParameter[] 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pis"></param>
        /// <returns></returns>
        public static SqlParameter[] GetParameters(object obj,PropertyInfo [] pis=null)
        {
            List<SqlParameter> listParams = new List<SqlParameter>();
            if (obj != null)
            {
                var type = obj.GetType();
                var isDic = type.IsIn(SqlSugarTool.DicArraySO, SqlSugarTool.DicArraySS);
                if (isDic)
                {
                    if (type == SqlSugarTool.DicArraySO)
                    {
                        var newObj = (Dictionary<string, object>)obj;
                        var pars = newObj.Select(it => new SqlParameter("@" + it.Key, it.Value));
                        foreach (var par in pars)
                        {
                            SetParSize(par);
                        }
                        listParams.AddRange(pars);
                    }
                    else
                    {

                        var newObj = (Dictionary<string, string>)obj;
                        var pars = newObj.Select(it => new SqlParameter("@" + it.Key, it.Value));
                        foreach (var par in pars)
                        {
                            SetParSize(par);
                        }
                        listParams.AddRange(pars); ;
                    }
                }
                else
                {
                    PropertyInfo[] propertiesObj = null;
                    if (pis != null)
                    {
                        propertiesObj = pis;
                    }
                    else {
                        propertiesObj=type.GetProperties();
                    }
                    string replaceGuid = Guid.NewGuid().ToString();
                    foreach (PropertyInfo r in propertiesObj)
                    {
                        var value = r.GetValue(obj, null);
                        if (r.PropertyType.IsEnum)
                        {
                            value =Convert.ToInt64(value);
                        }
                        if (value == null || value.Equals(DateTime.MinValue)) value = DBNull.Value;
                        if (r.Name.ToLower().Contains("hierarchyid"))
                        {
                            var par = new SqlParameter("@" + r.Name, SqlDbType.Udt);
                            par.UdtTypeName = "HIERARCHYID";
                            par.Value = value;
                            listParams.Add(par);
                        }
                        else
                        {
                            var par = new SqlParameter("@" + r.Name, value);
                            SetParSize(par);
                            if (value == DBNull.Value)
                            {//防止文件类型报错
                                SqlSugarTool.SetSqlDbType(r, par);
                            }
                            listParams.Add(par);
                        }
                    }
                }
            }
            return listParams.ToArray();
        }

        /// <summary>
        /// 将实体对象转换成 Dictionary《string, string》
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static Dictionary<string, object> GetObjectToDictionary(object obj)
        {

            Dictionary<string, object> reval = new Dictionary<string, object>();
            if (obj == null) return reval;
            var type = obj.GetType();
            var isDic = type.IsIn(SqlSugarTool.DicArraySO, SqlSugarTool.DicArraySS);
            if (isDic)
            {
                if (type == SqlSugarTool.DicArraySO)
                {
                    return (Dictionary<string, object>)obj;
                }
                else
                {
                    var newObj = (Dictionary<string, string>)obj;
                    foreach (var item in newObj)
                    {
                        reval.Add(item.Key, item.Value);
                    }
                    return reval;
                }
            }
            else
            {
                var propertiesObj = type.GetProperties();
                string replaceGuid = Guid.NewGuid().ToString();
                foreach (PropertyInfo r in propertiesObj)
                {
                    var val = r.GetValue(obj, null);
                    if (r.PropertyType.IsEnum)
                    {
                        val =Convert.ToInt64(val);
                    }
                    reval.Add(r.Name, val == null ? DBNull.Value : val);
                }

                return reval;
            }
        }

        /// <summary>
        /// 获取type属性cache
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cachePropertiesKey"></param>
        /// <param name="cachePropertiesManager"></param>
        /// <returns></returns>
        internal static PropertyInfo[] GetGetPropertiesByCache(Type type, string cachePropertiesKey, CacheManager<PropertyInfo[]> cachePropertiesManager)
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
        internal static bool IsPrimaryKey(SqlSugarClient db, string tableName)
        {
            return GetPrimaryKeyByTableName(db, tableName) != null;
        }


        /// <summary>
        /// 处理like条件的通配符
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string SqlLikeWordEncode(string word)
        {
            if (word == null) return word;
            return Regex.Replace(word, @"(\[|\%)", "[$1]");
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static Guid GetPropertyValue(object obj, string property)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return (Guid)propertyInfo.GetValue(obj, null);
        }
  
        /// <summary>
        /// 使用页面自动填充sqlParameter时 Request.Form出现特殊字符时可以重写Request.Form方法，使用时注意加锁并且用到将该值设为null
        /// </summary>
        public static Func<string, string> SpecialRequestForm = null;


        /// <summary>
        /// 获取最底层类型
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        internal static Type GetUnderType(PropertyInfo propertyInfo, ref bool isNullable)
        {
            Type unType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            isNullable = unType != null;
            unType = unType ?? propertyInfo.PropertyType;
            return unType;
        }

        /// <summary>
        /// 设置Sql类型
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="par"></param>
        internal static void SetSqlDbType(PropertyInfo prop, SqlParameter par)
        {
            var isNullable = false;
            var isByteArray = SqlSugarTool.GetUnderType(prop, ref isNullable) == typeof(byte[]);
            if (isByteArray)
            {
                par.SqlDbType = SqlDbType.VarBinary;
            }
        }
    }
}
