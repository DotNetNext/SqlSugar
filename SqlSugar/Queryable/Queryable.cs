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

        /// <summary>
        /// 联表查询
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Queryable<T> JoinTable<T2>(Expression<Func<T, T2, object>> expression, JoinType type = JoinType.LEFT)
        {
            return this.JoinTable<T, T2>(expression, type);
        }

        /// <summary>
        /// 联表查询
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Queryable<T> JoinTable<T2, T3>(Expression<Func<T, T2, T3, object>> expression, JoinType type = JoinType.LEFT)
        {
            return this.JoinTable<T, T2, T3>(expression, type);
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达式条件</param>
        /// <returns>Queryable</returns>
        public Queryable<T> Where<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return this.Where<T, T2>(expression);
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="whereString">Where后面的Sql条件语句 (例如： id=@id )</param>
        /// <param name="whereObj"> 匿名参数 (例如：new{id=1,name="张三"})</param>
        /// <returns>Queryable</returns>
        public Queryable<T> Where<T2>(string whereString, object whereObj = null)
        {
            return this.Where(whereString, whereObj);
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达式条件</param>
        /// <returns>Queryable</returns>
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



        public Queryable<T> In<FieldType>(string InFieldName, params FieldType[] inValues)
        {
            return this.In<T, FieldType>(InFieldName, inValues);
        }


        public Queryable<T> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            return this.In<T, FieldType>(expression, inValues);
        }


        public Queryable<T> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            return this.In<T, FieldType>(expression, inValues);
        }

        public Queryable<T> In<FieldType>(string InFieldName, List<FieldType> inValues)
        {
            return this.In<T, FieldType>(InFieldName, inValues);
        }


    }
}
