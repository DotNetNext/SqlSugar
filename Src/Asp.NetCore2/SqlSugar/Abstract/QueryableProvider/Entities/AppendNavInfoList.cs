using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar 
{
    internal class AppendNavInfo 
    {
        public Dictionary<string, MappingNavColumnInfo> MappingNavProperties { get; set; }= new Dictionary<string, MappingNavColumnInfo>();
        public Dictionary<string, string> AppendProperties { get; set; }= new Dictionary<string, string>();
        public List<AppendNavResult> Result { get; set; } =new List<AppendNavResult>();
    }
    internal class AppendNavResult 
    {
        public Dictionary<string, object> result = new Dictionary<string, object>();
    }
    internal class MappingNavColumnInfo
    {
        public List<Expression> ExpressionList { get;  set; }
        public string Name { get;  set; }
    }
}
