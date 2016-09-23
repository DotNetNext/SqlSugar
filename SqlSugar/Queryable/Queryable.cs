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
    }
}
