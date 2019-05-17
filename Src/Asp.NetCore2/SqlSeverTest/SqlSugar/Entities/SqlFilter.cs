using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SqlFilterItem
    {
        /// <summary>
        /// Equal to NULL representing global
        /// </summary>
        public string FilterName { get; set; }
        public Func<ISqlSugarClient,SqlFilterResult> FilterValue { get; set; }
        /// <summary>
        /// Is it a multiple table query?
        /// </summary>
        public bool IsJoinQuery { get; set; }
    }

    public class SqlFilterResult
    {
        public string Sql { get; set; }
        public object Parameters { get; set; }
    }
}
