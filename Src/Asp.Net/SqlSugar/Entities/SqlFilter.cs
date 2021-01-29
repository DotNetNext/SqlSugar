using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    public class TableFilterItem<T>: SqlFilterItem where  T :class,new()
    {
        private TableFilterItem()
        {

        }
        private Expression exp { get; set; }
        private Type type { get; set; }
        public TableFilterItem(Expression<Func<T,bool>> expression)
        {
            exp = expression;
            type = typeof(T);
        }
        private new  string FilterName { get; set; }
        private new Func<ISqlSugarClient, SqlFilterResult> FilterValue { get; set; }
        private new bool IsJoinQuery { get; set; }
    }
 

    public class SqlFilterResult
    {
        public string Sql { get; set; }
        public object Parameters { get; set; }
    }
}
