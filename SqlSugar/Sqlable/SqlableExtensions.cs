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
            if (sqlable.MappingCurrentState == MappingCurrentState.MappingTable)
            {
                sqlable.MappingCurrentState = MappingCurrentState.Where;
                sqlable.Sql.Append("WHERE  " + whereStr.Replace("where", "").Replace("WHERE", ""));
                return sqlable;
            }
            else
            {
                throw new Exception(string.Format("{0}无法使用Where,Where必需在MappingTable后面使用", sqlable.MappingCurrentState));
            }
        }
        /// <summary>
        /// 查询列并且返回完整SQL符串
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="SelectField"></param>
        /// <returns></returns>
        public static string SelectToSql(this Sqlable sqlable, string SelectField = "*")
        {
            if (sqlable.MappingCurrentState == null) {
                throw new Exception("SelectToSql必需在MappingTable后面使用");
            }
            string sql = "SELECT " + SelectField + sqlable.Sql;
            sqlable = null;
            return sql;
        }
        /// <summary>
        /// 查询列并且返回完整SQL分页符串
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderFields"></param>
        /// <param name="SelectField"></param>
        /// <returns></returns>
        public static string SelectToPageSql(this Sqlable sqlable, int pageIndex, int pageSize, string orderFields, string SelectField = "*")
        {
            if (sqlable.MappingCurrentState == null)
            {
                throw new Exception("SelectToSql必需在MappingTable后面使用");
            }
            string sql = "SELECT " + SelectField + ",row_index=ROW_NUMBER() OVER(ORDER BY " + orderFields + " )" + sqlable.Sql;
            sqlable = null;
            int skip = (pageIndex - 1) * pageSize + 1;
            int take = pageSize;
            return string.Format("SELECT * FROM ({0}) t WHERE  t.row_index BETWEEN {1} AND {2}", sql, skip, skip + take - 1);
        }
        /// <summary>
        /// 根据T生成查询字符串
        /// </summary>
        /// <typeparam name="T">实表类</typeparam>
        /// <param name="sqlable"></param>
        /// <returns> select * from T</returns>
        public static string TableToSql<T>(this Sqlable sqlable)
        {
            if (sqlable.MappingCurrentState != null)
                throw new Exception(".Table只能在Sqlable后面使用,正确用法：Sqlable.Table<T>() ");
            return string.Format(" FROM {0} {1}", typeof(T).Name, sqlable.IsNoLock.IsNoLock());
        }
        /// <summary>
        /// 根据tableName生成查询字符串
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="tableName"></param>
        /// <returns>select * from tableName</returns>
        public static string TableToSql(this Sqlable sqlable, string tableName)
        {
            if (sqlable.MappingCurrentState != null)
                throw new Exception(".Table只能在Sqlable后面使用,正确用法：Sqlable.Table(”tableName“) ");
            return string.Format(" FROM {0} {1}", tableName, sqlable.IsNoLock.IsNoLock());
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
