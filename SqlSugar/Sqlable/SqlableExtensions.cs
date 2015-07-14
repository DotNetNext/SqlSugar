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
        /// sql语句Where以后的所有字符串
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="whereStr">Where以后的所有字符串</param>
        /// <returns></returns>
        public static Sqlable WhereAfter(this Sqlable sqlable, string whereStr)
        {
            if (sqlable.SqlableCurrentState.IsIn(SqlableCurrentState.MappingTable,SqlableCurrentState.Table))
            {
                sqlable.SqlableCurrentState = SqlableCurrentState.Where;
                sqlable.Sql.Append("WHERE  " + whereStr.Replace("where", "").Replace("WHERE", ""));
                return sqlable;
            }
            else
            {
                throw new Exception(string.Format("{0}无法使用Where,Where必需在MappingTable或者Table后面使用", sqlable.SqlableCurrentState));
            }
        }
        /// <summary>
        /// 查询列并且返回完整SQL符串
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="SelectFields"></param>
        /// <returns></returns>
        public static string ToSql(this Sqlable sqlable, string SelectFields = "*")
        {
            if (sqlable.SqlableCurrentState == null)
            {
                throw new Exception("ToSql必需在MappingTable或者Table后面使用");
            }
            string sql = "SELECT " + SelectFields + sqlable.Sql;
            sqlable.SqlableCurrentState = null;
            sqlable =null;
            return sql;
        }
        /// <summary>
        /// 查询列并且返回完整SQL分页符串
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderFields"></param>
        /// <param name="SelectFields"></param>
        /// <returns></returns>
        public static string ToPageSql(this Sqlable sqlable, int pageIndex, int pageSize, string orderFields, string SelectFields = "*")
        {
            if (sqlable.SqlableCurrentState == null)
            {
                throw new Exception("ToPageSql必需在MappingTable或者Table后面使用");
            }
            string sql = "SELECT " + SelectFields + ",row_index=ROW_NUMBER() OVER(ORDER BY " + orderFields + " )" + sqlable.Sql;
            int skip = (pageIndex - 1) * pageSize + 1;
            int take = pageSize;
            sql = string.Format("SELECT * FROM ({0}) t WHERE  t.row_index BETWEEN {1} AND {2}", sql, skip, skip + take - 1);
            sqlable.SqlableCurrentState = null;
            sqlable = null;
            return sql;
        }
        /// <summary>
        /// 根据T生成查询字符串
        /// </summary>
        /// <typeparam name="T">实表类</typeparam>
        /// <param name="sqlable"></param>
        /// <returns> select * from T</returns>
        public static Sqlable Table<T>(this Sqlable sqlable)
        {
            if (sqlable.SqlableCurrentState != null)
                throw new Exception(".Table只能在Sqlable后面使用,正确用法：Sqlable.Table<T>() ");
            sqlable.Sql = new StringBuilder();
            sqlable.Sql.Append(string.Format(" FROM {0} {1}", typeof(T).Name, sqlable.IsNoLock.IsNoLock()));
            sqlable.SqlableCurrentState = SqlableCurrentState.Table;
            return sqlable;
        }
        /// <summary>
        /// 根据tableName生成查询字符串
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="tableName"></param>
        /// <returns>select * from tableName</returns>
        public static Sqlable Table(this Sqlable sqlable, string tableName)
        {
            if (sqlable.SqlableCurrentState != null)
                throw new Exception(".Table只能在Sqlable后面使用,正确用法：Sqlable.Table(”tableName“) ");
            sqlable.Sql = new StringBuilder();
            sqlable.Sql.Append(string.Format(" FROM {0} {1}", tableName, sqlable.IsNoLock.IsNoLock()));
            sqlable.SqlableCurrentState = SqlableCurrentState.Table;
            return sqlable;
        }

        #region helper methods
        private static string Remove(this string thisValue)
        {
            if (thisValue == null)
                throw new ArgumentNullException("SqlSugarExtensions.Remove.thisValue");
            return thisValue.TrimEnd('?');
        }
        private static string IsLeft(this string thisValue)
        {
            if (thisValue == null)
                throw new ArgumentNullException("SqlSugarExtensions.IsLeft.thisValue");
            return (thisValue.Last() == '?') ? "LEFT" : "INNER";

        }
        private static string IsNoLock(this bool thisValue)
        {
            if (thisValue) return " WITH(NOLOCK) ";
            return "";

        }
        #endregion

    }
}
