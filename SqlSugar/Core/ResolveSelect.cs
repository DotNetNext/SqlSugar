using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{

    /// <summary>
    /// ** 描述：解析SELECT里的表达示
    /// ** 创始时间：2016-9-21
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** qq：610262374 
    /// ** 使用说明：
    /// </summary>
    internal class ResolveSelect
    {
        /// <summary>
        /// 多表情况
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expStr"></param>
        /// <param name="reval"></param>
        internal static void GetResult<TResult>(string expStr, Queryable<TResult> reval)
        {
            reval.Select = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            if (reval.Select.IsNullOrEmpty())
            {
                reval.Select = Regex.Match(expStr, @"c =>.*?\((.+)\)").Groups[1].Value;
            }
            if (reval.Select.IsNullOrEmpty())
            {
                throw new SqlSugarException("Select 解析失败 ", new { selectString = reval.Select });
            }
            reval.Select = reval.Select.Replace("\"", "'");
        }
        /// <summary>
        /// 单表情况
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reval"></param>
        internal static void GetResult<TResult>(Queryable<TResult> reval)
        {
            string expStr = reval.Select;
            expStr = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            expStr = Regex.Replace(expStr, @"(?<=\=).*?\.", "");
            if (reval.Select.IsNullOrEmpty())
            {
                throw new SqlSugarException("Select 解析失败 ", new { selectString = reval.Select });
            }
            expStr = expStr.Replace("\"", "'");
            reval.Select = expStr;
        }
    }
}
