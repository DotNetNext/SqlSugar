using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
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
        public static Sqlable Where(this Sqlable sqlable, string whereStr)
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
            if (sqlable.MappingCurrentState.IsIn(MappingCurrentState.Select, MappingCurrentState.Where,MappingCurrentState.MappingTable))
            {
                string sql = "SELECT " + SelectField + sqlable.Sql;
                sqlable = null;
                return sql;
            }
            else {
                throw new Exception(string.Format("{0}无法使用Where,Where必需在Select、SingleTable或Where后面使用", sqlable.MappingCurrentState));
            }
        }
    }
}
