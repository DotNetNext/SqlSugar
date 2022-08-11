using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    internal class JsonQueryableProvider_TableInfo
    {
        public string Table { get; set; }
        public string ShortName { get; set; }
        public bool IsMaster { get; set; }
        public bool IsJoin { get; set; }
        public int Index { get; set; }
    }
}
