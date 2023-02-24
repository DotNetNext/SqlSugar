﻿using System;
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
        internal Type type { get; set; }
    }

    public class TableFilterItem<T>: SqlFilterItem     
    {
        private TableFilterItem()
        {

        }
        private Expression exp { get; set; }
        public TableFilterItem(Expression<Func<T,bool>> expression,bool isJoinOn=false)
        {
            exp = expression;
            type = typeof(T);
            base.IsJoinQuery = isJoinOn;
            this.IsJoinQuery = isJoinOn;
        }

        public TableFilterItem(Type entityType,Expression expression, bool isJoinOn=false)
        {
            exp = expression;
            type = entityType;
            base.IsJoinQuery = isJoinOn;
            this.IsJoinQuery = isJoinOn;
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
