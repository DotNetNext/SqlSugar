using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class EntityColumnInfo
    {
        public PropertyInfo PropertyInfo { get; set; }
        public string PropertyName { get; set; }
        public string DbColumnName { get; set; }
        public string OldDbColumnName { get; set; }
        public int Length { get; set; }
        public string ColumnDescription { get; set; }
        public string DefaultValue { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsPrimarykey { get; set; }
        public string EntityName { get;  set; }
        public string DbTableName { get; set; }
        public bool IsIgnore { get;  set; }
        public string DataType { get; set; }
        public int DecimalDigits { get; set; }
        public string OracleSequenceName { get; set; }
        public bool IsOnlyIgnoreInsert { get; set; }
    }
}
