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
        internal static void GetResult<TResult>(string expStr, Queryable<TResult> reval)
        {
            reval.Select = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            if (reval.Select.IsNullOrEmpty())
            {
                reval.Select = Regex.Match(expStr, @"c =>.*?\((.+)\)").Groups[1].Value;
            }
        }

        internal static void GetResult<TResult>(Queryable<TResult> reval)
        {
            reval.Select = Regex.Replace(reval.Select, @"(?<=\=).*?\.", "");
        }
    }
}
