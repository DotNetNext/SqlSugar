using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{

    /// ** 描述：SqlSugar扩展工具类
    /// ** 创始时间：2015-7-19
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    internal static class SqlSugarToolExtensions
    {

        /// <summary>
        /// 数组字串转换成SQL参数格式，例如: 参数 new int{1,2,3} 反回 "'1','2','3'"
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
        /// 将字符串转换成SQL参数格式，例如: 参数value返回'value'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlValue(this string value)
        {
            return string.Format("'{0}'", value.ToSqlFilter());
        }
        /// <summary>
        /// SQL关键字过滤,过滤拉姆达式中的特殊字符，出现特殊字符则引发异常
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
        ///  指定数据类型，如果不在指定类当中则引发异常(只允许输入指定字母、数字、下划线、时间、GUID)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSuperSqlFilter(this string value)
        {
            if (value.IsNullOrEmpty()) return value;
            if (Regex.IsMatch(value, @"^(\w|\.|\:|\-| |\,)+$"))
            {
                return value;
            }
            throw new SqlSecurityException("指定类型(只允许输入指定字母、数字、下划线、时间、guid)。");
        }

        /// <summary>
        /// 获取锁字符串
        /// </summary>
        /// <param name="isNoLock"></param>
        /// <returns></returns>
        public static string GetLockString(this bool isNoLock)
        {
            return isNoLock ? "WITH(NOLOCK)" : null; ;
        }
        /// <summary>
        /// 获取Select需要的字段
        /// </summary>
        /// <param name="selectFileds"></param>
        /// <returns></returns>
        public static string GetSelectFiles(this string selectFileds)
        {
            return selectFileds.IsNullOrEmpty() ? "*" : selectFileds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupByFileds"></param>
        /// <returns></returns>
        public static string GetGroupBy(this string groupByFileds)
        {
            return groupByFileds.IsNullOrEmpty() ? "" :" GROUP BY "+groupByFileds;
        }
    }
}
