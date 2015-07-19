using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    /// <summary>
    /// 扩展工具类
    /// </summary>
    internal static class SqlSugarToolExtensions
    {

        /// <summary>
        /// 将数组转为 '1','2' 这种格式的字符串 用于 where id in(  )
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToJoinSqlInVal<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
            {
                return ToSqlValue(string.Empty);
            }
            else
            {
                return string.Join(",", array.Where(c => c != null).Select(it => (it + "").ToSuperSqlFilter().ToSqlValue()));
            }
        }
        /// <summary>
        /// 转换为sql
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlValue(this string value)
        {
            return string.Format("'{0}'", value.ToSqlFilter());
        }
        /// <summary>
        /// SQL关键字过滤,用于过滤拉姆达式特殊字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlFilter(this string value)
        {
            if (!value.IsNullOrEmpty())
            {
                if (Regex.IsMatch(value, @"'|%|0x|(\@.*\=)", RegexOptions.IgnoreCase))
                {
                    throw new SqlSecurityException("查询参数不允许存在特殊字符。");
                }
            }
            return value;
        }
        /// <summary>
        ///  指定类型(只允许输入指定字母、数字、下划线、时间、guid)、用于 where in过滤
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSuperSqlFilter(this string value)
        {
            if (value.IsNullOrEmpty()) return value;
            if (Regex.IsMatch(value, @"^\w|\.|\:|\-$"))
            {
                return value;
            }
            throw new SqlSecurityException("查询参数不允许存在特殊字符。");
        }
    }
}
