using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：Sqlable扩展函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public static partial class SqlableExtensions
    {
        /// <summary>
        /// Form
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="modelObj">表名</param>
        /// <param name="shortName">表名简写</param>
        /// <returns></returns>
        public static Sqlable Form(this Sqlable sqlable, object tableName, string shortName)
        {
            sqlable.Sql = new StringBuilder();
            sqlable.Sql.AppendFormat(" FROM {0} {1} {2} ", tableName, GetIsNoLock(sqlable.IsNoLock), shortName);
            return sqlable;
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="leftFiled"></param>
        /// <param name="RightFiled"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Sqlable Join(this Sqlable sqlable, object tableName, string shortName, string leftFiled, string RightFiled, JoinType type)
        {
            Check.ArgumentNullException(sqlable.Sql, "语法错误，正确用法：sqlable.Form(“table”).Join");
            sqlable.Sql.AppendFormat(" {0} JOIN {1} {5} {2} ON  {3} = {4} ", type.ToString(), tableName, GetIsNoLock(sqlable.IsNoLock), leftFiled, RightFiled, shortName);
            return sqlable;
        }

        /// <summary>
        /// 查询条件比如  t1.id=@id
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static Sqlable Where(this Sqlable sqlable, string where)
        {
            sqlable.Where.Add(where);
            return sqlable;
        }
        public static Sqlable OrderBy(this Sqlable sqlable, string orderBy)
        {
            sqlable.OrderBy = orderBy;
            return sqlable;
        }

        public static Sqlable Apply(this Sqlable sqlable, string applySql, string shotName, ApplyType type)
        {
            Check.ArgumentNullException(sqlable.Sql, "语法错误，正确用法：sqlable.Form(“table”).Join");
            sqlable.Sql.AppendFormat(" {0} APPLY ({1}) {2}} ", type.ToString(), applySql, shotName);
            return sqlable;
        }

        public static Sqlable GroupBy(this Sqlable sqlable, string groupBy)
        {
            sqlable.GroupBy = groupBy;
            return sqlable;
        }

        public static List<T> SelectToList<T>(this Sqlable sqlable, string fileds, object whereObj = null) where T : class
        {
            string sql = null;
            try
            {
                Check.ArgumentNullException(sqlable.Sql, "语法错误，SelectToSql必需要在.Form后面使用");
                sqlable.Sql.Insert(0, string.Format("SELECT {0} ", fileds));
                sqlable.Sql.Append(" Where 1=1").Append(string.Join(" ", sqlable.Where));
                sqlable.Sql.Append(sqlable.OrderBy);
                sqlable.Sql.Append(sqlable.GroupBy);
                sql = sqlable.Sql.ToString();
                var sqlParams = SqlTool.GetParameters(whereObj);
                var reval = SqlTool.DataReaderToList<T>(typeof(T), sqlable.DB.GetReader(sql, sqlParams));
                return reval;
            }
            catch (Exception ex)
            {
                Check.Exception(true, "sql:{0} \r\n message:{1}", sql, ex.Message);
                throw;
            }
            finally
            {
                sqlable = null;
            }
        }

        public static List<T> SelectToPageList<T>(this Sqlable sqlable, string fileds, string orderByFiled, int pageIndex, int pageSize, object whereObj = null) where T : class
        {
            string sql = null;
            try
            {
                if (pageIndex == 0) pageIndex = 1;
                Check.ArgumentNullException(sqlable.Sql, "语法错误，SelectToSql必需要在.Form后面使用");
                sqlable.Sql.Insert(0, string.Format("SELECT {0},row_index=ROW_NUMBER() OVER(ORDER BY {1} )", fileds, orderByFiled));
                sqlable.Sql.Append(" Where 1=1 ").Append(string.Join(" ", sqlable.Where));
                sqlable.Sql.Append(sqlable.OrderBy);
                sqlable.Sql.Append(sqlable.GroupBy);
                sql = sqlable.Sql.ToString();
                int skip = (pageIndex - 1) * pageSize + 1;
                int take = pageSize;
                sql = string.Format("SELECT * FROM ({0}) t WHERE  t.row_index BETWEEN {1} AND {2}", sql, skip, skip + take - 1);
                var sqlParams = SqlTool.GetParameters(whereObj);
                var reval = SqlTool.DataReaderToList<T>(typeof(T), sqlable.DB.GetReader(sql, sqlParams));
                return reval;
            }
            catch (Exception ex)
            {
                Check.Exception(true, "sql:{0} \r\n message:{1}", sql, ex.Message);
                throw;
            }
            finally
            {
                sql = null;
                sqlable = null;
            }
        }

        private static string GetIsNoLock(bool isNoLock)
        {
            return isNoLock ? "WITH(NOLOCK)" : null; ;
        }

    }
}
