using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：Queryable扩展函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public static class QueryableExtensions
    {
        public static SqlSugar.Queryable<T> Where<T>(this SqlSugar.Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            string whereStr = string.Empty;
            if (expression.Body is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)expression.Body);
                whereStr = " and " + SqlTool.BinarExpressionProvider(be.Left, be.Right, be.NodeType);
            }
            queryable.Where.Add(whereStr);
            return queryable;
        }
        public static SqlSugar.Queryable<T> Order<T>(this SqlSugar.Queryable<T> queryable, string orderFileds)
        {
            queryable.Order = orderFileds;
            return queryable;
        }

        public static SqlSugar.Queryable<T> Skip<T>(this SqlSugar.Queryable<T> queryable, int num)
        {
            if (queryable.Order.IsNullOrEmpty())
            {
                throw new Exception(".Skip必需使用.Order排序");
            }
            queryable.Skip = num;
            return queryable;
        }

        public static SqlSugar.Queryable<T> Take<T>(this SqlSugar.Queryable<T> queryable, int num)
        {
            if (queryable.Order.IsNullOrEmpty())
            {
                throw new Exception(".Take必需使用.Order排序");
            }
            queryable.Take = num;
            return queryable;
        }
        public static List<T> ToPageList<T>(this SqlSugar.Queryable<T> queryable, int pageIndex, int pageSize)
        {
            if (queryable.Order.IsNullOrEmpty())
            {
                throw new Exception("分页必需使用.Order排序");
            }
            queryable.Skip = (pageIndex - 1) * pageSize + 1;
            queryable.Take = pageSize;
            return queryable.ToList();
        }

        public static T Single<T>(this  SqlSugar.Queryable<T> queryable)
        {
            return queryable.ToList().Single();
        }

        public static T Single<T>(this  SqlSugar.Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            string whereStr = string.Empty;
            if (expression.Body is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)expression.Body);
                whereStr = " and " + SqlTool.BinarExpressionProvider(be.Left, be.Right, be.NodeType);
            }
            queryable.Where.Add(whereStr);
            return queryable.ToList().Single();
        }

        public static List<T> ToList<T>(this SqlSugar.Queryable<T> queryable)
        {
            StringBuilder sbSql = new StringBuilder();
            string withNoLock = queryable.DB.Sqlable.IsNoLock ? "WITH(NOLOCK)" : null;
            var order = queryable.Order.IsValuable() ? (",row_index=ROW_NUMBER() OVER(ORDER BY " + queryable.Order + " )") : null;

            sbSql.AppendFormat("SELECT * {2} FROM {0} {1} ", queryable.Name, withNoLock, order);
            if (queryable.Skip == null && queryable.Take != null)
            {
                sbSql.Insert(0, "SELECT * FROM ( ");
                sbSql.Append(") t WHERE t.row_index<=" + queryable.Take);
            }
            else if (queryable.Skip != null && queryable.Take == null)
            {
                sbSql.Insert(0, "SELECT * FROM ( ");
                sbSql.Append(") t WHERE t.row_index>=" + queryable.Skip);
            }
            else if (queryable.Skip != null && queryable.Take != null)
            {
                sbSql.Insert(0, "SELECT * FROM ( ");
                sbSql.Append(") t WHERE t.row_index BETWEEN " + queryable.Skip + "AND  " + (queryable.Skip + queryable.Take - 1));
            }

            var dt = queryable.DB.GetDataTable(sbSql.ToString());
            var reval = SqlTool.List<T>(dt);
            queryable = null;
            return reval;
        }

    }
}
