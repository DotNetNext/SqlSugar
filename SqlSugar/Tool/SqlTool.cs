using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;

namespace SqlSugar
{
    internal class SqlTool
    {
        /// <summary>
        /// 生成MappingTable的内部函数
        /// </summary>
        /// <param name="mappCount"></param>
        /// <returns></returns>
        public static string CreateMappingTable(int mappCount)
        {
            Func<int, string> GetMethodString = count =>
            {
                string methodString = (@"  
public static Sqlable MappingTable<{0}>(this Sqlable sqlable{5})
           {1}
        {{
            {2};
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(""{3}"",{4});
            sqlable.Sql = sb;
            return sqlable;
        }}
            ");
                string tStr = string.Empty;
                string tStr2 = string.Empty;
                string tStr3 = string.Empty;
                string tStr4 = string.Empty;
                string tStr5 = string.Empty;
                string tStr6 = string.Empty;
                int j = 2;
                for (int i = 1; i <= count; i++)
                {

                    tStr += string.Format("T{0},", i);
                    tStr2 += string.Format("\n where T{0} : new() \n", i);
                    tStr3 += string.Format("\n    T{0} t{0} = new T{0}(); \n", i);
                    if (i != count)
                        tStr6 += string.Format(", string mappingFeild" + i);
                    if (i == 1)
                    {
                        tStr4 += @" FROM {1} t1 {0} \n   ";
                        tStr5 += "sqlable.IsNoLock.IsNoLock(),t" + i + ".GetType().Name,";

                    }
                    else
                    {

                        tStr4 += @"\n {" + (j) + "} JOIN {" + (j + 1) + "} t" + i + " {0} ON {" + (j + 2) + "} ";

                        tStr5 += "mappingFeild" + (i - 1) + ".IsLeft(),t" + (i) + ".GetType().Name, mappingFeild" + (i - 1) + ".Remove(),";
                        j = j + 3;
                    }

                }
                methodString = string.Format(methodString, tStr.TrimEnd(','), tStr2.TrimEnd(','), tStr3.TrimEnd(','), tStr4, tStr5.TrimEnd(','), tStr6);
                return methodString;
            };
            string reval = string.Empty;
            for (int i = 2; i < mappCount; i++)
            {
                reval += GetMethodString(i);
            }
            return reval;
        }


        /// <summary>
        /// 将dataTable转成List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> List<T>(DataTable dt)
        {
            if (dt == null) return new List<T>();
            var list = new List<T>();
            Type type = typeof(T);

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

            var plist = new List<PropertyInfo>(props);

            foreach (DataRow item in dt.Rows)
            {
                T s = System.Activator.CreateInstance<T>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
                    if (info != null)
                    {
                        if (!Convert.IsDBNull(item[i]))
                        {
                            info.SetValue(s, item[i], null);
                        }
                    }
                }
                list.Add(s);
            }
            return list;
        }


        /// <summary>
        /// 将数组转为 '1','2' 这种格式的字符串 用于 where id in(  )
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToJoinSqlInVal(object[] array)
        {
            if (array == null || array.Length == 0)
            {
                return "''";
            }
            else
            {
                return string.Join(",", array.Where(c => c != null).Select(c => "'" + (c + "").Replace("'", "''") + "'"));//除止SQL注入
            }
        }

        public static SqlParameter[] GetObjectToParameters(object obj)
        {
            List<SqlParameter> listParams = new List<SqlParameter>();
            var propertiesObj = obj.GetType().GetProperties();
            string replaceGuid = Guid.NewGuid().ToString();
            foreach (PropertyInfo r in propertiesObj)
            {
                listParams.Add(new SqlParameter("@" + r.Name, r.GetValue(obj, null).ToString()));
            }

            return listParams.ToArray();
        }

        public static Dictionary<string, string> GetObjectToDictionary(object obj)
        {
            Dictionary<string, string> reval = new Dictionary<string, string>();
            var propertiesObj = obj.GetType().GetProperties();
            string replaceGuid = Guid.NewGuid().ToString();
            foreach (PropertyInfo r in propertiesObj)
            {
                var val = r.GetValue(obj, null);
                reval.Add(r.Name, val == null ? "" : val.ToString());
            }

            return reval;
        }
    }
}
