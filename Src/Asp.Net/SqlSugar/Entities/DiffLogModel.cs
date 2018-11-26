using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DiffLogModel
    {
        public List<DiffLogTableInfo> AfterData { get; set; }
        public List<DiffLogTableInfo> BeforeData { get; set; }
        public SugarParameter[] Parameters { get; set; }
        public string Sql { get; set; }
        public TimeSpan? Time { get; set; }
        public object BusinessData { get; set; }
        public DiffType DiffType { get; set; }
    }
    public class DiffLogTableInfo
    {
        public string TableName { get; set; }
        public string TableDescription { get; set; }
        public List<DiffLogColumnInfo> Columns { get; set; }
    }
    public class DiffLogColumnInfo {

        public string ColumnName { get; set; }
        public string ColumnDescription { get; set; }
        public object Value { get; set; }
    }
}
