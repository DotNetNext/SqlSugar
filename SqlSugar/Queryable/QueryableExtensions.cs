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
        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SqlSugar.Queryable<T> Where<T>(this SqlSugar.Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            string whereStr = SqlTool.GetWhereByExpression<T>(expression);
            queryable.Where.Add(whereStr);
            return queryable;
        }
        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="whereString"></param>
        /// <returns></returns>
        public static SqlSugar.Queryable<T> Where<T>(this SqlSugar.Queryable<T> queryable, string whereString)
        {
            var type = queryable.Type;
            string whereStr = whereString;
            queryable.Where.Add(whereStr);
            return queryable;
        }


        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="orderFileds">如：id asc,name desc </param>
        /// <returns></returns>
        public static SqlSugar.Queryable<T> OrderBy<T>(this SqlSugar.Queryable<T> queryable, string orderFileds)
        {
            queryable.Order = orderFileds;
            return queryable;
        }

        /// <summary>
        /// 返回指定索引数据以及索引后面所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static SqlSugar.Queryable<T> Skip<T>(this SqlSugar.Queryable<T> queryable, int index)
        {
            if (queryable.Order.IsNullOrEmpty())
            {
                throw new Exception(".Skip必需使用.Order排序");
            }
            queryable.Skip = index;
            return queryable;
        }

        /// <summary>
        /// 从起始点向后取指定条数的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static SqlSugar.Queryable<T> Take<T>(this SqlSugar.Queryable<T> queryable, int num)
        {
            if (queryable.Order.IsNullOrEmpty())
            {
                throw new Exception(".Take必需使用.Order排序");
            }
            queryable.Take = num;
            return queryable;
        }

        /// <summary>
        /// 返回分页List《T》集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <returns></returns>
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

        /// <summary>
        ///  返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static T Single<T>(this  SqlSugar.Queryable<T> queryable)
        {
            return queryable.ToList().Single();
        }

        /// <summary>
        ///  返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取总条数
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static int Count<T>(this SqlSugar.Queryable<T> queryable)
        {
            StringBuilder sbSql = new StringBuilder();
            string withNoLock = queryable.DB.Sqlable.IsNoLock ? "WITH(NOLOCK)" : null;
            sbSql.AppendFormat("SELECT COUNT(*)  FROM {0} {1} WHERE 1=1 {2}  ", queryable.Name, withNoLock, string.Join("", queryable.Where));
            var count = queryable.DB.GetInt(sbSql.ToString());
            return count;
        }

        /// <summary>
        /// 转换为List<T>集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this SqlSugar.Queryable<T> queryable)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                string withNoLock = queryable.DB.Sqlable.IsNoLock ? "WITH(NOLOCK)" : null;
                var order = queryable.Order.IsValuable() ? (",row_index=ROW_NUMBER() OVER(ORDER BY " + queryable.Order + " )") : null;

                sbSql.AppendFormat("SELECT * {1} FROM {0} {2} WHERE 1=1 {3}  ", queryable.Name, order, withNoLock, string.Join("", queryable.Where));
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
                    sbSql.Append(") t WHERE t.row_index BETWEEN " + queryable.Skip + " AND " + (queryable.Skip + queryable.Take - 1));
                }

                var reader = queryable.DB.GetReader(sbSql.ToString());
                var reval = SqlTool.DataReaderToList<T>(typeof(T), reader);
                queryable = null;
                return reval;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("sql:{0}\r\n message:{1}", ex.Message));
            }
            finally
            {
                sbSql = null;
                queryable = null;
            }

        }

    }
}
