using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class QueryableFormat
    {
        public Type Type { get; set; }
        public string TypeString { get; set; }
        public string Format { get; set; }
        public string PropertyName { get; set; }
        public string MethodName { get; set; }
        public MethodInfo MethodInfo { get; set; }
    }
}
