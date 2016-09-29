using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// 表名属性
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SugarTableName : Attribute
    {
        /// <summary>
        /// 表值
        /// </summary>
        public string value { get; private set; }
        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="value"></param>
        public SugarTableName(string tableName)
        {
            value = tableName;
        }
    }
    /// <summary>
    /// 表字段属性
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SugarFieldName : Attribute
    {
        /// <summary>
        /// 字段值
        /// </summary>
        public string value { get; private set; }
        /// <summary>
        /// 字段名
        /// </summary>
        /// <param name="value"></param>
        public SugarFieldName(string fieldName)
        {
            value = fieldName;
        }
    }
}
