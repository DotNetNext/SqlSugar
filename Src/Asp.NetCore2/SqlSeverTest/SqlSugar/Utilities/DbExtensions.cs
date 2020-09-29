using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace SqlSugar
{
    public static class DbExtensions
    {
        public static string ToJoinSqlInVals<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
            {
                return ToSqlValue(string.Empty);
            }
            else
            {
                return string.Join(",", array.Where(c => c != null).Select(it => (it + "").ToSqlValue()));
            }
        }

        public static string ToSqlValue(this string value)
        {
            return string.Format("'{0}'", value.ToSqlFilter());
        }

        /// <summary>
        ///Sql Filter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlFilter(this string value)
        {
            if (!value.IsNullOrEmpty())
            {
                value = value.Replace("'", "''");
            }
            return value;
        }

        internal static string ToLower(this string value ,bool isAutoToLower)
        {
            if (value == null) return null;
            if (isAutoToLower == false) return value;
            return value.ToLower();
        }
    }
}
