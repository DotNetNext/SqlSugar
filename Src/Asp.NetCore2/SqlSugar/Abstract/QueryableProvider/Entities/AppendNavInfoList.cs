using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar 
{
    internal class AppendNavInfo 
    {
        public Dictionary<string, MappingNavColumnInfo> MappingNavProperties = new Dictionary<string, MappingNavColumnInfo>();
        public Dictionary<string, string> AppendProperties = new Dictionary<string, string>();
    }
    internal class MappingNavColumnInfo
    {
        public List<Expression> ExpressionList { get;  set; }
        public string Name { get;  set; }
    }
}
