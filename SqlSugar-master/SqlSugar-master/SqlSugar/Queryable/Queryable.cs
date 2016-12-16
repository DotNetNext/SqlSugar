using System;
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
        /// <summary>
        /// 联表查询
        /// </summary>
        /// <typeparam name="T2">联接的表对象</typeparam>
        /// <param name="expression">表达式</param>
        /// <param name="type">Join的类型</param>
        /// <returns></returns>
        public Queryable<T> JoinTable<T2>(Expression<Func<T, T2, object>> expression, JoinType type = JoinType.Left)
        {
            return this.JoinTable<T, T2>(expression, type);
        }

        /// <summary>
        /// 联表查询
        /// </summary>
        /// <typeparam name="T2">联接表的对象</typeparam>
        /// <typeparam name="T3">联接表的对象</typeparam>
        /// <param name="expression">表达式</param>
        /// <param name="type">Join的类型</param>
        /// <returns></returns>
        public Queryable<T> JoinTable<T2, T3>(Expression<Func<T, T2, T3, object>> expression, JoinType type = JoinType.Left)
        {
            return this.JoinTable<T, T2, T3>(expression, type);
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <param name="expression">表达式条件</param>
        /// <returns></returns>
        public Queryable<T> Where<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return this.Where<T, T2>(expression);
        }
        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <param name="whereString">Where后面的Sql条件语句 (例如： id=@id )</param>
        /// <param name="whereObj">匿名参数 (例如：new{id=1,name="张三"})</param>
        /// <returns></returns>
        public Queryable<T> Where<T2>(string whereString, object whereObj = null)
        {
            return this.Where(whereString, whereObj);
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <typeparam name="T3">表实体类型</typeparam>
        /// <param name="expression">表达式条件</param>
        /// <returns></returns>
        public Queryable<T> Where<T2, T3>(Expression<Func<T, T2, T3, bool>> expression)
        {
            return this.Where<T, T2, T3>(expression);
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <typeparam name="T3">表实体类型</typeparam>
        /// <typeparam name="T4">表实体类型</typeparam>
        /// <param name="expression">表达式条件</param>
        /// <returns></returns>
        public Queryable<T> Where<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return this.Where<T, T2, T3, T4>(expression);
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <typeparam name="T3">表实体类型</typeparam>
        /// <typeparam name="T4">表实体类型</typeparam>
        /// <typeparam name="T5">表实体类型</typeparam>
        /// <param name="expression">表达式条件</param>
        /// <returns></returns>
        public Queryable<T> Where<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return this.Where<T, T2, T3, T4, T5>(expression);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <param name="expression">例如 (s1,s2)=>s1.id,相当于 order by s1.id</param>
        /// <param name="type">排序类型</param>
        /// <returns></returns>
        public Queryable<T> OrderBy<T2>(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            return this.OrderBy<T, T2>(expression, type);
        }

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="maxField">列名</param>
        /// <returns></returns>
        public TResult Max<TResult>(string maxField)
        {
            return this.Max<T, TResult>(maxField);
        }

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="minField">列名</param>
        /// <returns></returns>
        public TResult Min<TResult>(string minField)
        {
            return this.Min<T, TResult>(minField);
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="T2">原数据实体类型</typeparam>
        /// <typeparam name="TResult">返回值的新实体类型</typeparam>
        /// <param name="expression">给新实体赋值的表达式</param>
        /// <returns></returns>
        public Queryable<TResult> Select<T2, TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return this.Select<T, T2, TResult>(expression);
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="T2">原数据实体类型</typeparam>
        /// <typeparam name="T3">原数据实体类型</typeparam>
        /// <typeparam name="TResult">返回值的新实体类型</typeparam>
        /// <param name="expression">给新实体赋值的表达式</param>
        /// <returns></returns>
        public Queryable<TResult> Select<T2, T3, TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return this.Select<T, T2, T3, TResult>(expression);
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="T2">原数据实体类型</typeparam>
        /// <typeparam name="T3">原数据实体类型</typeparam>
        /// <typeparam name="T4">原数据实体类型</typeparam>
        /// <typeparam name="TResult">返回值的新实体类型</typeparam>
        /// <param name="expression">给新实体赋值的表达式</param>
        /// <returns></returns>
        public Queryable<TResult> Select<T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return this.Select<T, T2, T3, T4, TResult>(expression);
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="T2">原数据实体类型</typeparam>
        /// <typeparam name="T3">原数据实体类型</typeparam>
        /// <typeparam name="T4">原数据实体类型</typeparam>
        /// <typeparam name="T5">原数据实体类型</typeparam>
        /// <typeparam name="TResult">返回值的新实体类型</typeparam>
        /// <param name="expression">给新实体赋值的表达式</param>
        /// <returns></returns>
        public Queryable<TResult> Select<T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return this.Select<T, T2, T3, T4, T5, TResult>(expression);
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="TResult">返回值的新实体类型</typeparam>
        /// <param name="expression">给新实体赋值的表达式</param>
        /// <returns></returns>
        public Queryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            return this.Select<T, TResult>(expression);
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="T2">返回值的新实体类型</typeparam>
        /// <param name="select">查询字符串（例如 id,name）</param>
        /// <returns></returns>
        public Queryable<T2> Select<T2>(string select)
        {
            return this.Select<T,T2>(select);
        }
        #endregion


    }
}
