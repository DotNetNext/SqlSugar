using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar 
{
    /// <summary>
    /// MethodCall base DateFomat
    /// </summary>
    public partial class MethodCallExpressionResolve : BaseResolve
    {
        public string GeDateFormat(string formatString, string value)
        {
            if (IsOracle() && formatString == "yyyy-MM-dd HH:mm:ss")
            {
                return $"to_char({value},'yyyy-MM-dd HH24:mi:ss') ";
            }
            else if (IsOracle() || IsPg())
            {
                if (!(formatString?.Contains("24") == true))
                {
                    formatString = formatString.Replace("HH", "hh24");
                    if (!(formatString?.Contains("24") == true))
                    {
                        formatString = formatString.Replace("hh", "hh24");
                    }
                }
                formatString = formatString.Replace("mm", "mi");
                //if (formatString.HasValue() && formatString.Contains("hh:mm"))
                //{
                //    formatString = formatString.Replace("hh:mm", "hh:mi");
                //}
                //else if (formatString.HasValue() && formatString.Contains("hhmm"))
                //{
                //    formatString = formatString.Replace("hhmm", "hhmi");
                //}
                //else if (formatString.HasValue() && formatString.Contains("HH:mm"))
                //{
                //    formatString = formatString.Replace("HH:mm", "HH:mi");
                //}
                //else if (formatString.HasValue() && formatString.Contains("HHmm"))
                //{
                //    formatString = formatString.Replace("HHmm", "HHmi");
                //}
                return $"to_char({value},'{formatString}') ";
            }
            else if (IsSqlite() && formatString == "yyyy-MM-dd")
            {
                return $"strftime('%Y-%m-%d', {value})";
            }
            else if (IsSqlite() && formatString == "yyyy-MM-dd HH:mm:ss")
            {
                return $"strftime('%Y-%m-%d %H:%M:%S', {value})";
            }
            else if (IsSqlite() && formatString == "yyyy-MM-dd hh:mm:ss")
            {
                return $"strftime('%Y-%m-%d %H:%M:%S', {value})";
            }
            else if (IsSqlite() && formatString == "yyyy-MM")
            {
                return $"strftime('%Y-%m', {value})";
            }
            else if (IsSqlite() && formatString == "yyyyMM")
            {
                return $"strftime('%Y%m', {value})";
            }
            else if (IsSqlite() && formatString == "yyyyMMdd")
            {
                return $"strftime('%Y%m%d', {value})";
            }
            else if (IsSqlite() && formatString.Contains("%"))
            {
                return $"strftime('{formatString}', {value})";
            }
            else if (IsMySql() && formatString == "yyyy-MM-dd")
            {
                return $"DATE_FORMAT({value}, '%Y-%m-%d')";
            }
            else if (IsMySql() && formatString == "yyyy-MM")
            {
                return $"DATE_FORMAT({value}, '%Y-%m')";
            }
            else if (IsMySql() && formatString == "yyyyMM")
            {
                return $"DATE_FORMAT({value}, '%Y%m')";
            }
            else if (IsMySql() && formatString == "yyyyMMdd")
            {
                return $"DATE_FORMAT({value}, '%Y%m%d')";
            }
            else if (IsMySql() && formatString == "yyyy-MM-dd HH:mm:ss")
            {
                return $"DATE_FORMAT({value}, '%Y-%m-%d %H:%i:%S')";
            }
            else if (IsMySql() && formatString == "yyyy-MM-dd hh:mm:ss")
            {
                return $"DATE_FORMAT({value}, '%Y-%m-%d %H:%i:%S')";
            }
            else if (IsMySql() && formatString.Contains("%"))
            {
                return $"DATE_FORMAT({value}, '{formatString}')";
            }
            else if (formatString == "yyyy-MM-dd" && IsSqlServer())
            {
                return $"CONVERT(varchar(100),convert(datetime,{value}), 23)";
            }
            else if (formatString == "yyyy-MM" && IsSqlServer())
            {
                return $"CONVERT(varchar(7),convert(datetime,{value}), 23)";
            }
            else if (formatString == "yyyy-MM-dd HH:mm:ss" && IsSqlServer())
            {
                return $"CONVERT(varchar(100),convert(datetime,{value}), 120)";
            }
            else if (formatString == "yyyy-MM-dd hh:mm:ss" && IsSqlServer())
            {
                return $"CONVERT(varchar(100),convert(datetime,{value}), 120)";
            }
            else if (formatString == "yyyy-MM-dd HH:mm" && IsSqlServer())
            {
                return $"CONVERT(varchar(16),convert(datetime,{value}), 120)";
            }
            else if (formatString == "yyyy-MM-dd hh:mm" && IsSqlServer())
            {
                return $"CONVERT(varchar(16),convert(datetime,{value}), 120)";
            }
            else if (formatString == "yyyy-MM-dd hh:mm:ss.ms" && IsSqlServer())
            {
                return $"CONVERT(varchar(100),convert(datetime,{value}), 121)";
            }
            else if (IsSqlServer() && formatString != null && formatString.IsInt())
            {
                return string.Format("CONVERT(varchar(100),convert(datetime,{0}), {1})", value, formatString);
            }
            else if (IsSqlServer())
            {
                return string.Format("FORMAT({0},'{1}','en-US')", value, formatString);
            }
            else if (IsMySql()&& !formatString.Contains("%"))
            {
                var newFormt = formatString
                    .Replace("yyyy", "%Y")
                    .Replace("yy", "%Y")
                    .Replace("MM", "%m")
                    .Replace("M", "%m")
                    .Replace("dd", "%d")
                    .Replace("HH", "%H")
                    .Replace("hh", "%h")
                    .Replace("mm", "%i") 
                    .Replace("ss", "%s") 
                    .Replace("fff", "%f");
                return $"DATE_FORMAT({value}, '{newFormt}')";
            }
            else if (IsSqlite() && !formatString.Contains("%"))
            {
                var newFormt = formatString
                    .Replace("yyyy", "%Y")
                    .Replace("yy", "%Y")
                    .Replace("MM", "%m")
                    .Replace("M", "%m")
                    .Replace("dd", "%d")
                    .Replace("HH", "%H")
                    .Replace("hh", "%h")
                    .Replace("mm", "%M")
                    .Replace("ss", "%S")
                    .Replace("fff", "%f");
                return $"strftime('{newFormt}',{value})";
            }
            var parameter = new MethodCallExpressionArgs() { IsMember = true, MemberValue = DateType.Year };
            var parameter2 = new MethodCallExpressionArgs() { IsMember = true, MemberName = value };
            var parameters = new MethodCallExpressionModel() { Args = new List<MethodCallExpressionArgs>() { parameter2, parameter } };
            var begin = @"^";
            var end = @"$";
            formatString = formatString.Replace("yyyy", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            formatString = formatString.Replace("yy", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);

            parameters.Args.Last().MemberValue = DateType.Month;
            if (IsMySql())
            {
                formatString = formatString.Replace("MM", begin + UtilMethods.ConvertStringToNumbers("LPAD(" + this.GetMethodValue("DateValue", parameters).ObjToString() + ",2,0)") + end);
            }
            else if (IsSqlite())
            {
                formatString = formatString.Replace("MM", begin + UtilMethods.ConvertStringToNumbers("SUBSTR('00' ||" + this.GetMethodValue("DateValue", parameters).ObjToString() + ", -2, 2)") + end);
            }
            else if (IsPg())
            {
                formatString = formatString.Replace("MM", begin + UtilMethods.ConvertStringToNumbers("lpad(cast(" + this.GetMethodValue("DateValue", parameters).ObjToString() + " as varchar(20)),2,'0')") + end);
            }
            else if (IsOracle())
            {
                formatString = formatString.Replace("MM", begin + UtilMethods.ConvertStringToNumbers("lpad(cast(" + this.GetMethodValue("DateValue", parameters).ObjToString() + " as varchar(20)),2,'0')") + end);
            }
            else
            {
                formatString = formatString.Replace("MM", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            }
            formatString = formatString.Replace("M", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);

            parameters.Args.Last().MemberValue = DateType.Day;
            if (IsSqlServer())
            {
                formatString = formatString.Replace("dd", begin + UtilMethods.ConvertStringToNumbers(string.Format("CASE  WHEN  LEN({0})=1  THEN '0'+ {0}   else  {0}  end", this.GetMethodValue("DateValue", parameters))) + end);
            }
            formatString = formatString.Replace("dd", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            formatString = formatString.Replace("d", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);

            parameters.Args.Last().MemberValue = DateType.Hour;
            formatString = Regex.Replace(formatString, "hh", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end, RegexOptions.IgnoreCase);
            formatString = Regex.Replace(formatString, "h", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end, RegexOptions.IgnoreCase);

            parameters.Args.Last().MemberValue = DateType.Minute;
            formatString = formatString.Replace("mm", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            formatString = formatString.Replace("m", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);

            parameters.Args.Last().MemberValue = DateType.Second;
            formatString = formatString.Replace("ss", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            formatString = formatString.Replace("s", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);

            if (!IsSqlite())
            {
                parameters.Args.Last().MemberValue = DateType.Millisecond;
                formatString = formatString.Replace("ms", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            }

            var items = Regex.Matches(formatString, @"\^\d+\$").Cast<Match>().ToList();
            foreach (var item in items)
            {
                formatString = formatString.Replace(item.Value, "$@" + UtilMethods.ConvertNumbersToString(item.Value.TrimStart('^').TrimEnd('$')) + "$");
            }
            var strings = formatString.TrimStart('$').TrimEnd('$').Split('$');
            var joinStringParameter = new MethodCallExpressionModel()
            {
                Args = new List<MethodCallExpressionArgs>()
            };
            foreach (var r in strings)
            {
                if (r != "" && r.Substring(0, 1) == "@")
                {
                    joinStringParameter.Args.Add(new MethodCallExpressionArgs()
                    {
                        MemberName = r.TrimStart('@')
                    });
                }
                else
                {

                    var name = base.AppendParameter(r);
                    joinStringParameter.Args.Add(new MethodCallExpressionArgs()
                    {
                        MemberName = name
                    });
                }
            }
            return this.GetMethodValue("MergeString", joinStringParameter).ObjToString();
        }
        private bool IsSqlServer()
        {
            return this.Context is SqlServerExpressionContext;
        }
        private bool IsMySql()
        {
            var name = this.Context.GetType().Name;
            var result = (name == "MySqlExpressionContext");
            return result;
        }
        private bool IsSqlite()
        {
            return this.Context is SqliteExpressionContext;
        }
        private bool IsPg()
        {
            return this.Context is PostgreSQLExpressionContext
                   ||this.Context is KdbndpExpressionContext;
        }
        private bool IsOracle()
        {
            return this.Context is OracleExpressionContext;
        }
    }
}
