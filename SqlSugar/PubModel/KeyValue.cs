using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// 自定义键值类 key is string, value is string
    /// </summary>
    public class KeyValue
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
    /// <summary>
    /// 自定义键值类 key is string, value is object
    /// </summary>
    public class KeyValueObj
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }
    }

}
