using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{

    /// <summary>
    /// ** 描述：解析Queryable.Select函数的参数
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
            reval.Select = reval.Select.Replace("DateTime.Now", "GETDATE()");
            reval.Select = ConvertFuns(reval.Select);
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
            expStr = expStr.Replace("DateTime.Now", "GETDATE()");
            expStr = ConvertFuns(expStr);
            reval.Select = expStr;
        }
        /// <summary>
        /// 替换函数
        /// </summary>
        /// <param name="selectStr"></param>
        /// <returns></returns>
        internal static string ConvertFuns(string selectStr)
        {
            var hasFun = selectStr.Contains("(");
            if (hasFun)
            {
                var ms = Regex.Matches(selectStr, @"(?<=\=\s+)([a-z,A-Z]+)?\((.+?)\)");
                if (ms != null && ms.Count > 0)
                {
                    foreach (Match m in ms)
                    {
                        string itemStr = m.Groups[0].Value;
                        string itemValue = m.Groups[2].Value;
                        string funName = m.Groups[1].Value;
                        selectStr = selectStr.Replace(itemStr, itemValue);
                    }
                }
                var hasObjToFun = Regex.IsMatch(selectStr, @"\.ObjTo[a-z,A-Z]+?\(\s*\)");
                if (hasObjToFun)
                {
                    selectStr = Regex.Replace(selectStr,@"\.ObjTo[a-z,A-Z]+?\(\s*\)","");
                }
            }
            return selectStr;
        }
    }
}
