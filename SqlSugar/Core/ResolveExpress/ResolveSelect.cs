using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

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
    internal partial class ResolveSelect
    {
        /// <summary>
        /// 多表情况
        /// </summary>
        /// <typeparam name="TResult">实体类型</typeparam>
        /// <param name="expStr">拉姆达字符串</param>
        /// <param name="reval">查旬对象</param>
        /// <param name="exp"></param>
        internal static void GetResult<TResult>(string expStr, Queryable<TResult> reval, Expression exp)
        {
            var isComplexAnalysis = IsComplexAnalysis(expStr);
            reval.SelectValue = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            if (reval.SelectValue.IsNullOrEmpty())
            {
                reval.SelectValue = Regex.Match(expStr, @"[a-z,A-Z]\W* =>.*?\((.+)\)").Groups[1].Value;
            }
            var hasOutPar = expStr.Contains(SqlSugarTool.ParSymbol);
            if (hasOutPar)//有
            {
                var ms = Regex.Matches(expStr, @"""(\" + SqlSugarTool.ParSymbol + @"[a-z,A-Z]+)?""\.ObjTo[a-z,A-Z]+?\(\)").Cast<Match>().ToList();
                foreach (var m in ms)
                {
                    reval.SelectValue = reval.SelectValue.Replace(m.Groups[0].Value, m.Groups[1].Value);
                }
            }
            if (reval.SelectValue.IsNullOrEmpty())
            {
                throw new SqlSugarException(ExpSelectValueIsNull, new { selectString = reval.SelectValue });
            }
            reval.SelectValue = reval.SelectValue.Replace("\"", "'");
            reval.SelectValue = reval.SelectValue.Replace("DateTime.Now", "GETDATE()");
            reval.SelectValue = ConvertFuns(reval.SelectValue, false);
            if (reval.DB != null && reval.DB.IsEnableAttributeMapping && reval.DB._mappingColumns.IsValuable())
            {
                foreach (var item in reval.DB._mappingColumns)
                {
                    reval.SelectValue = Regex.Replace(reval.SelectValue,@"\."+item.Key,"."+item.Value);
                }
            }
        }

        private static bool IsComplexAnalysis(string expStr)
        {
            string errorFunName = null;
            if (expStr.IsValuable() && (expStr.Contains("+<>") || Regex.IsMatch(expStr, @"\.[a-z,A-Z,_]\w*\.[a-z,A-Z,_]\w*?(\,|\})")))
            {
                throw new SqlSugarException(ExpNoSupportOutPars);
            }
            if(expStr.IsValuable()&&Regex.IsMatch(expStr, @"\+|\-|\*|\/")){
                throw new SqlSugarException(ExpNoSupportOperation);
            }
            string reg= @"(\.[a-z,A-Z,_]\w*?\(.*?\))|\=\s*[a-z,A-Z,_]\w*?\(.*?\)|\=\s*[a-z,A-Z,_]\w*?\(.*?\)|\=[a-z,A-Z,_]\w*.[a-z,A-Z,_]\w*.[a-z,A-Z,_]\w*";
            if (expStr.IsValuable() & Regex.IsMatch(expStr,reg))
            {
                var ms = Regex.Matches(expStr, reg);
                var errorNum = 0;
                foreach (Match item in ms.Cast<Match>().OrderBy(it=>it.Value.Split('.').Length))
                {
                    if (item.Value == null) {
                        errorNum++;
                        break;
                    }
                    if (!item.Value.IsMatch(@"\.ObjTo|Convert|ToString"))
                    {
                        errorNum++;
                        errorFunName = item.Value;
                        break;
                    }
                }
                if (errorNum > 0)
                {
                    throw new SqlSugarException(string.Format(ExpNoSupportMethod,errorFunName));
                }
            }
            return false;
        }
        /// <summary>
        /// 单表情况
        /// </summary>
        /// <typeparam name="TResult">实体类型</typeparam>
        /// <param name="reval">查旬对象</param>
        /// <param name="exp"></param>
        internal static void GetResult<TResult>(Queryable<TResult> reval, Expression exp)
        {
            string expStr = reval.SelectValue;
            var isComplexAnalysis = IsComplexAnalysis(expStr);
            expStr = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            if (expStr.IsNullOrEmpty())
            {
                expStr = Regex.Match(reval.SelectValue, @"[a-z,A-Z]\W* =>.*?\((.+)\)").Groups[1].Value;
            }
            var hasOutPar = expStr.Contains(SqlSugarTool.ParSymbol);
            if (hasOutPar)//有
            {
                var ms = Regex.Matches(expStr, @"""(\" + SqlSugarTool.ParSymbol + @"[a-z,A-Z]+)?""\.ObjTo[a-z,A-Z]+?\(\)").Cast<Match>().ToList();
                foreach (var m in ms)
                {
                    expStr = expStr.Replace(m.Groups[0].Value, m.Groups[1].Value);
                }
            }
            expStr = Regex.Replace(expStr, @"(?<=\=)[^\,]*?\.", "");
            if (reval.SelectValue.IsNullOrEmpty())
            {
                throw new SqlSugarException(ExpSelectValueIsNull, new { selectString = reval.SelectValue });
            }
            expStr = expStr.Replace("\"", "'");
            expStr = expStr.Replace("DateTime.Now", "GETDATE()");
            expStr = ConvertFuns(expStr);
            reval.SelectValue = expStr;
            if (reval.DB != null && reval.DB.IsEnableAttributeMapping && reval.DB._mappingColumns.IsValuable())
            {
                foreach (var item in reval.DB._mappingColumns)
                {
                    reval.SelectValue = Regex.Replace(reval.SelectValue, @"\=" + item.Key, "=" + item.Value);
                }
            }
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
            if (selectStr.Contains(".ToString"))
            {
                throw new SqlSugarException(ExpNoSupportToString);
            }
            if (selectStr.Contains("+<>"))
            {
                throw new SqlSugarException(ExpNoSupportOutPars);
            }
            return selectStr;
        }
    }
}
