﻿using System;
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
                return string.Join(",", array.Where(c => c != null).Select(it => it.ToSqlValue()));
            }
        }
        public static string ToJoinSqlInValsN<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
            {
                return ToSqlValue(string.Empty);
            }
            else
            {
                return string.Join(",", array.Where(c => c != null).Select(it => "N"+it.ToSqlValue()));
            }
        }
        public static string ToSqlValue(this object value)
        {
            if (value!=null&& UtilConstants.NumericalTypes.Contains(value.GetType()))
                return value.ToString();

            var str = value + "";
            return str.ToSqlValue();
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
                var oldLength=value.Length;
                value = value.Replace("'", "''");
                if (oldLength!=value.Length&& value.IndexOf(")")>0&&value.IndexOf(@"\''")>0) value=value.Replace("\\","\\\\");
            }
            return value;
        }

        /// <summary>
        /// Check field format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToCheckField(this string value)
        {
            if (value != null)
            {
                if (value.IsContainsIn(";", "--")) 
                {
                    throw new Exception($"{value} format error ");
                }
                else if (value.IsContainsIn("'")&&(value.Length- value.Replace("'","").Length)%2!=0)
                {
                    throw new Exception($"{value} format error ");
                }
            }
            return value;
        }
        public static string ToCheckRegexW(this string value) 
        {
            if (Regex.IsMatch(value,@"^\w+$"))
            {
                return value;
            }
            else 
            {
                throw new Exception($"ToCheckRegexW {value} format error ");
            }
        }
        internal static string ToLower(this string value ,bool isAutoToLower)
        {
            if (value == null) return null;
            if (isAutoToLower == false) return value;
            return value.ToLower();
        }
        internal static string ToUpper(this string value, bool isAutoToUpper)
        {
            if (value == null) return null;
            if (isAutoToUpper == false) return value;
            return value.ToUpper();
        }
    }
}
