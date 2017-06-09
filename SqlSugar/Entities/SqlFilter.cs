using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SqlFilterItem
    {
        public string FilterName { get; set; }
        public Func<SqlSugarClient,SqlFilterResult> GetFilterSql { get; set; }
        public bool IsJoinQuery { get; set; }
    }

    public class SqlFilterResult
    {
        public string Sql { get; set; }
        public List<SugarParameter> Parameters { get; set; }
    }
}
