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
        public static string SelectToSql(this Sqlable sqlable, string SelectField = "*")
        {
            string sql = "SELECT " + SelectField + sqlable.Sql;
            sqlable = null;
            return sql;
        }
        public static string SelectToPageSql(this Sqlable sqlable, int pageIndex, int pageSize,string orderFields, string SelectField = "*")
        {
            string sql = "SELECT " + SelectField + ",row_index=ROW_NUMBER() OVER(ORDER BY " + orderFields + " )" + sqlable.Sql;
            sqlable = null;
            int skip = (pageIndex - 1) * pageSize + 1;
            int take = pageSize;
            return string.Format("SELECT * FROM ({0}) t WHERE  t.row_index BETWEEN {1} AND {2}", sql,skip,skip+take-1);
        }
    }
}
