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
        /// <typeparam name="TResult">实体类型</typeparam>
        /// <param name="expStr">拉姆达字符串</param>
        /// <param name="reval">查旬对象</param>
        internal static void GetResult<TResult>(string expStr, Queryable<TResult> reval)
        {
            reval.Select = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            if (reval.Select.IsNullOrEmpty())
            {
                reval.Select = Regex.Match(expStr, @"c =>.*?\((.+)\)").Groups[1].Value;
            }
            var hasOutPar = expStr.Contains("@");
            if (hasOutPar)//有
            {
                var ms = Regex.Matches(expStr, @"""(\@[a-z,A-Z]+)?""\.ObjTo[a-z,A-Z]+?\(\)").Cast<Match>().ToList();
                foreach (var m in ms)
                {
                    reval.Select = reval.Select.Replace(m.Groups[0].Value, m.Groups[1].Value);
                }
            }
            if (reval.Select.IsNullOrEmpty())
            {
                throw new SqlSugarException("Select 解析失败 ", new { selectString = reval.Select });
            }
            reval.Select = reval.Select.Replace("\"", "'");
            reval.Select = reval.Select.Replace("DateTime.Now", "GETDATE()");
            reval.Select = ConvertFuns(reval.Select, false);
        }
        /// <summary>
        /// 单表情况
        /// </summary>
        /// <typeparam name="TResult">实体类型</typeparam>
        /// <param name="reval">查旬对象</param>
        internal static void GetResult<TResult>(Queryable<TResult> reval)
        {
            string expStr = reval.Select;
            expStr = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            var hasOutPar = expStr.Contains("@");
            if (hasOutPar)//有
            {
                var ms = Regex.Matches(expStr, @"""(\@[a-z,A-Z]+)?""\.ObjTo[a-z,A-Z]+?\(\)").Cast<Match>().ToList();
                foreach (var m in ms)
                {
                    expStr = expStr.Replace(m.Groups[0].Value, m.Groups[1].Value);
                }
            }
            expStr = Regex.Replace(expStr, @"(?<=\=)[^\,]*?\.", "");
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
        /// <param name="selectStr">拉姆达字符串</param>
        /// <param name="isOneT">true为单表，false为多表</param>
        /// <returns></returns>
        internal static string ConvertFuns(string selectStr, bool isOneT = true)
        {
            if (isOneT == false)//多表
            {
                var hasFun = selectStr.Contains(")");
                if (hasFun)
                {
                    var m1 = Regex.Matches(selectStr, @"To[A-Z][a-z]+?\(Convert\((.+?)\)\)").Cast<Match>().ToList();
                    if (m1.IsValuable())
                    {
                        foreach (var m in m1)
                        {
                            selectStr = selectStr.Replace(m.Groups[0].Value, m.Groups[1].Value);
                        }
                    }
                    var m2 = Regex.Matches(selectStr, @"Convert\((.+?)\)\.ObjTo[a-z,A-Z]+?\(\)").Cast<Match>().ToList();
                    if (m2.IsValuable())
                    {
                        foreach (var m in m2)
                        {
                            selectStr = selectStr.Replace(m.Groups[0].Value, m.Groups[1].Value);
                        }
                    }
                    var m3 = Regex.Matches(selectStr, @"Convert\((.+?)\)").Cast<Match>().ToList();
                    if (m3.IsValuable())
                    {
                        foreach (var m in m3)
                        {
                            selectStr = selectStr.Replace(m.Groups[0].Value, m.Groups[1].Value);
                        }
                    }
                }

            }
            else//单表
            {
                var hasObjToFun = Regex.IsMatch(selectStr, @"\)\s*\.ObjTo[a-z,A-Z]+?\(\s*\)");
                if (hasObjToFun)
                {
                    selectStr = Regex.Replace(selectStr, @"\)\s*\.ObjTo[a-z,A-Z]+?\(\s*\)", "");
                }
                var hasFun = selectStr.Contains(")");
                if (hasFun)
                {
                    selectStr = selectStr.Replace(")", "");
                    selectStr = selectStr.Replace("(", "");
                }
            }
            if (selectStr.Contains("+<>")) {
                throw new SqlSugarException("Select中的拉姆达表达式,不支持外部传参数,目前支持的写法 Where(\"1=1\",new {id=1}).Select(it=>{ id=\"@id\".ObjToInt()}");
            }
            return selectStr;
        }
    }
}
