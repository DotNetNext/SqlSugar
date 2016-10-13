using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SqlSugar
{
    /// <summary>
    /// 表名属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class SugarMappingAttribute : Attribute
    {
        private string tableName;
        /// <summary>
        /// 据库对应的表名
        /// </summary>
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        private string columnName;
        /// <summary>
        /// 数据库对应的列名
        /// </summary>
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }
    }

    internal class ReflectionSugarMapping
    {
        /// <summary>
        /// 通过反射取自定义属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static SugarMappingModel GetMappingInfo<T>()
        {
            Type objType = typeof(T);
            string cacheKey = "ReflectionSugarMapping.DisplaySelfAttribute" + objType.FullName;
            var cm = CacheManager<SugarMappingModel>.GetInstance();
            if (cm.ContainsKey(cacheKey))
            {
                return cm[cacheKey];
            }
            else
            {
                SugarMappingModel reval = new SugarMappingModel();
                string tableName = string.Empty;
                List<KeyValue> columnInfoList = new List<KeyValue>();
                var oldName = objType.Name;
                //取属性上的自定义特性
                foreach (PropertyInfo propInfo in objType.GetProperties())
                {
                    object[] objAttrs = propInfo.GetCustomAttributes(typeof(SugarMappingAttribute), true);
                    if (objAttrs.Length > 0)
                    {
                        if (objAttrs[0] is SugarMappingAttribute)
                        {
                            SugarMappingAttribute attr = objAttrs[0] as SugarMappingAttribute;
                            if (attr != null)
                            {
                                columnInfoList.Add(new KeyValue() { Key = propInfo.Name, Value = attr.ColumnName }); //列名
                            }
                        }
                    }
                }

                //取类上的自定义特性
                object[] objs = objType.GetCustomAttributes(typeof(SugarMappingAttribute), true);
                foreach (object obj in objs)
                {
                    if (obj is SugarMappingAttribute)
                    {
                        SugarMappingAttribute attr = obj as SugarMappingAttribute;
                        if (attr != null)
                        {

                            tableName = attr.TableName;//表名只有获取一次
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = objType.Name;
                }
                reval.TableMaping = new KeyValue() { Key = oldName, Value = tableName };
                reval.ColumnsMapping = columnInfoList;
                cm.Add(cacheKey,reval,cm.Day);
                return reval;
            }
        }
    }

    internal class SugarMappingModel
    {

        public KeyValue TableMaping { get; set; }
        public List<KeyValue> ColumnsMapping { get; set; }
    }

}
