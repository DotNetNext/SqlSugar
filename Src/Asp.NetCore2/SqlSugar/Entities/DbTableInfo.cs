using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DbTableInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DbObjectType DbObjectType { get; set; }
    }

    public class RazorTableInfo
    {
        public string DbTableName { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public DbObjectType DbObjectType { get; set; }
        public List<RazorColumnInfo> Columns { get; set; }
    }

    public class RazorColumnInfo {
        public string DbColumnName { get; set; }
        public string DataType { get; set; }
        public int Length { get; set; }
        public string ColumnDescription { get; set; }
        public string DefaultValue { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsPrimarykey { get; set; }
    }
}
