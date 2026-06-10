using System;
using System.Reflection;

namespace SqlSugar.TDSQLForOracleODBC
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
