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
        public string Key { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// 自定义键值类 key is string, value is object
    /// </summary>
    public class KeyValueObj
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }

}
