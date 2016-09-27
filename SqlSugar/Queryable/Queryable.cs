﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace SqlSugar
{

    /// <summary>
    /// ** 描述：Queryable拉姆达查询对象
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class Queryable<T>
    {
        #region 临时变量
        /// <summary>
        /// T的名称
        /// </summary>
        internal string TName { get { return typeof(T).Name; } }
        /// <summary>
        /// 实体类型
        /// </summary>
        internal Type Type { get { return typeof(T); } }
        /// <summary>
        /// 数据接口
        /// </summary>
        public SqlSugarClient DB = null;
        /// <summary>
        /// Where临时数据
        /// </summary>
        internal List<string> WhereValue = new List<string>();
        /// <summary>
        /// Skip临时数据
        /// </summary>
        internal int? Skip { get; set; }
        /// <summary>
        /// Take临时数据
        /// </summary>
        internal int? Take { get; set; }
        /// <summary>
        /// Order临时数据
        /// </summary>
        internal string OrderByValue { get; set; }
        /// <summary>
        /// Select临时数据
        /// </summary>
        internal string SelectValue { get; set; }
        /// <summary>
        /// SqlParameter临时数据
        /// </summary>
        internal List<SqlParameter> Params = new List<SqlParameter>();
        /// <summary>
        /// 表名临时数据
        /// </summary>
        internal string TableName { get; set; }
        /// <summary>
        /// 分组查询临时数据
        /// </summary>
        internal string GroupByValue { get; set; }
        /// <summary>
        /// 条件索引临时数据
        /// </summary>
        internal int WhereIndex = 1;
        /// <summary>
        /// 联表查询临时数据
        /// </summary>
        internal List<string> JoinTableValue = new List<string>();
        
        #endregion


        #region 公开函数
        public Queryable<T> JoinTable<T2>(Expression<Func<T, T2, object>> expression, JoinType type = JoinType.LEFT)
        {
            return this.JoinTable<T, T2>(expression, type);
        }

        public Queryable<T> JoinTable<T2, T3>(Expression<Func<T, T2, T3, object>> expression, JoinType type = JoinType.LEFT)
        {
            return this.JoinTable<T, T2, T3>(expression, type);
        }


        public Queryable<T> Where<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return this.Where<T, T2>(expression);
        }

        public Queryable<T> Where<T2>(string whereString, object whereObj = null)
        {
            return this.Where(whereString, whereObj);
        }

        public Queryable<T> Where<T2, T3>(Expression<Func<T, T2, T3, bool>> expression)
        {
            return this.Where<T, T2, T3>(expression);
        }

        public Queryable<T> Where<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return this.Where<T, T2, T3, T4>(expression);
        }

        public Queryable<T> Where<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return this.Where<T, T2, T3, T4, T5>(expression);
        }

        public Queryable<T> OrderBy<T2>(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.asc)
        {
            return this.OrderBy<T, T2>(expression, type);
        }


        public TResult Max<TResult>(string maxField)
        {
            return this.Max<T, TResult>(maxField);
        }

        public TResult Min<TResult>(string minField)
        {
            return this.Min<T, TResult>(minField);
        }


        public Queryable<TResult> Select<T2, TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return this.Select<T, T2, TResult>(expression);
        }

        public Queryable<TResult> Select<T2, T3, TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return this.Select<T, T2, T3, TResult>(expression);
        }

        public Queryable<TResult> Select<T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return this.Select<T, T2, T3, T4, TResult>(expression);
        }

        public Queryable<TResult> Select<T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return this.Select<T, T2, T3, T4, T5, TResult>(expression);
        }

        public Queryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            return this.Select<T, TResult>(expression);
        }
        public Queryable<T2> Select<T2>(string select)
        {
            return this.Select<T,T2>(select);
        }
        #endregion


    }
}
