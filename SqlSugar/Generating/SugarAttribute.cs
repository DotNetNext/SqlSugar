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

        private string ignore;
        /// <summary>
        /// 数据库对应的列名
        /// </summary>
        public string Ignore
        {
            get { return columnName; }
            set { columnName = value; }
        }
    }

    public class ReflectionSugarMapping
    {
        /// <summary>
        /// 通过反射取自定义属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static void DisplaySelfAttribute<T>() where T : class ,new()
        {
            string tableName = string.Empty;
            List<string> listColumnName = new List<string>();
            Type objType = typeof(T);
            //取属性上的自定义特性
            foreach (PropertyInfo propInfo in objType.GetProperties())
            {
                object[] objAttrs = propInfo.GetCustomAttributes(typeof(SugarMapping), true);
                if (objAttrs.Length > 0)
                {
                    SugarMapping attr = objAttrs[0] as SugarMapping;
                    if (attr != null)
                    {
                        listColumnName.Add(attr.ColumnName); //列名
                    }
                }
            }

            //取类上的自定义特性
            object[] objs = objType.GetCustomAttributes(typeof(SugarMapping), true);
            foreach (object obj in objs)
            {
                SugarMapping attr = obj as SugarMapping;
                if (attr != null)
                {

                    tableName = attr.TableName;//表名只有获取一次
                    break;
                }
            }
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = objType.Name;
            }
            Console.WriteLine(string.Format("The tablename of the entity is:{0} ", tableName));
            if (listColumnName.Count > 0)
            {
                Console.WriteLine("The columns of the table are as follows:");
                foreach (string item in listColumnName)
                {
                    Console.WriteLine(item);
                }
            }
        }
    }
   
}
