using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class KeyValueObj
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
