﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class DmExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarProvider Context { get; set; }
        public DmExpressionContext()
        {
            base.DbMehtods = new DmMethod();
        }
        public override string SqlParameterKeyWord
        {
            get
            {
                return ":";
            }
        }
        public override string SqlTranslationLeft { get { return "\""; } }
        public override string SqlTranslationRight { get { return "\""; } }
        public override string GetTranslationTableName(string entityName, bool isMapping = true)
        {
            return base.GetTranslationTableName(entityName, isMapping).ToUpper(IsUppper);
        }
        public override string GetTranslationColumnName(string columnName)
        {
            return base.GetTranslationColumnName(columnName).ToUpper(IsUppper);
        }
        public override string GetDbColumnName(string entityName, string propertyName)
        {
            return base.GetDbColumnName(entityName, propertyName).ToUpper(IsUppper);
        }
        public bool IsUppper
        {
            get
            {
                if (this.SugarContext?.Context?.Context?.CurrentConnectionConfig?.MoreSettings==null)
                {
                    return true;
                }
                else
                {
                    return this.SugarContext?.Context?.Context?.CurrentConnectionConfig?.MoreSettings.IsAutoToUpper == true;
                }
            }
        }
    }
    public partial class DmMethod : DefaultDbMethod, IDbMethods
    { 

        public override string WeekOfYear(MethodCallExpressionModel mode)
        {
            var parameterNameA = mode.Args[0].MemberName;
            return $"TO_NUMBER(TO_CHAR({parameterNameA}, 'WW')) ";
        }
        public override string ParameterKeyWord { get; set; } = ":";
        public string ForXmlPathLast;
        public override string GetForXmlPath()
        {
            if (string.IsNullOrEmpty(ForXmlPathLast)) return null;
            return "  GROUP BY  "+ ForXmlPathLast;
        }
        public override string GetStringJoinSelector(string result, string separator)
        {
            if (result.ObjToString().Trim().StartsWith("DISTINCT ", StringComparison.OrdinalIgnoreCase))
            {
                int index = result.IndexOf(result, StringComparison.Ordinal); // 找到去掉前缀空格后的位置
                result = result.Substring(index + 9); // 9 是 "DISTINCT " 的长度
                ForXmlPathLast = result;
                return $"listagg(to_char(max({result})),'{separator}') within group(order by max({result})) ";
            }
            else
            {
                return $"listagg(to_char({result}),'{separator}') within group(order by {result}) ";
            }
        }
        public override string ToInt64(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS Number)", parameter.MemberName);
        }

        public override string ToTime(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" to_timestamp({0},'0000-01-01 hh24:mi:ss') ", parameter.MemberName);
        }
        public override string Substring(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format("SUBSTR({0},1 + {1},{2})", parameter.MemberName, parameter2.MemberName, parameter3.MemberName);
        }
        public override string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var type = (DateType)Enum.Parse(typeof(DateType), parameter2.MemberValue.ObjToString(), false);
            switch (type)
            {
                case DateType.Year:
                    return string.Format("(CAST(TO_CHAR({0},'yyyy') AS NUMBER))", parameter.MemberName);
                case DateType.Month:
                    return string.Format("(CAST(TO_CHAR({0},'mm') AS NUMBER))", parameter.MemberName);
                case DateType.Hour:
                    return string.Format("(CAST(TO_CHAR({0},'hh24') AS NUMBER))", parameter.MemberName);
                case DateType.Second:
                    return string.Format("(CAST(TO_CHAR({0},'ss') AS NUMBER))", parameter.MemberName);
                case DateType.Minute:
                    return string.Format("(CAST(TO_CHAR({0},'mi') AS NUMBER))", parameter.MemberName);
                case DateType.Millisecond:
                    return string.Format("(CAST(TO_CHAR({0},'ff3') AS NUMBER))", parameter.MemberName);
                case DateType.Quarter:
                    return string.Format("(CAST(TO_CHAR({0},'q') AS NUMBER))", parameter.MemberName);
                case DateType.Weekday:
                    return $" (TO_NUMBER(TO_CHAR({parameter.MemberName}, 'D'))-1) ";
                case DateType.Day:
                default:
                    return string.Format("(CAST(TO_CHAR({0},'dd') AS NUMBER))", parameter.MemberName);
            }
        }
        //public override string DateAddByType(MethodCallExpressionModel model)
        //{
        //    var parameter = model.Args[0];
        //    var parameter2 = model.Args[1];
        //    var parameter3 = model.Args[2];
        //    var type = (DateType)Enum.Parse(typeof(DateType), parameter3.MemberValue.ObjToString(), false);
        //    double time = 1;
        //    switch (type)
        //    {
        //        case DateType.Year:
        //            time = 1 * 365;
        //            break;
        //        case DateType.Month:
        //            time = 1 * 30;
        //            break;
        //        case DateType.Day:
        //            break;
        //        case DateType.Hour:
        //            time = 1 / 24.0;
        //            break;
        //        case DateType.Second:
        //            time = 1 / 24.0 / 60.0 / 60.0;
        //            break;
        //        case DateType.Minute:
        //            time = 1 / 24.0 / 60.0;
        //            break;
        //        case DateType.Millisecond:
        //            time = 1 / 24.0 / 60.0 / 60.0 / 1000;
        //            break;
        //    }
        //    return string.Format("({0}+({1}*{2})) ", parameter.MemberName, time, parameter2.MemberName);
        //}

        public override string DateAddDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format("({0}+(1*{1})) ", parameter.MemberName, parameter2.MemberName);
        }

        public override string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS VARCHAR2(4000))", parameter.MemberName);
        }

        public override string ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" to_date({0},'yyyy-mm-dd hh24:mi:ss')", parameter.MemberName);
        }
        public override string Contains(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like '%'||{1}||'%') ", parameter.MemberName, parameter2.MemberName);
        }
        public override string StartsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like {1}||'%') ", parameter.MemberName, parameter2.MemberName);
        }
        public override string EndsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format("  ({0} like '%'||{1}) ", parameter.MemberName, parameter2.MemberName);
        }
        public override string Trim(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" trim({0}) ", parameter.MemberName);
        }
        public override string DateIsSameDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ( cast({0} as date)= cast( {1} as date) ) ", parameter.MemberName, parameter2.MemberName); ;
        }
        public override string DateIsSameByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];

            var dateType = parameter3.MemberValue.ObjToString().ToLower();
            var date1 = parameter.MemberName;
            var date2 = parameter2.MemberName;

            if (dateType == "year")
            {
                return string.Format("(EXTRACT(YEAR FROM {0}) = EXTRACT(YEAR FROM {1}))", date1, date2);
            }
            else if (dateType == "month")
            {
                return string.Format("(EXTRACT(YEAR FROM {0}) = EXTRACT(YEAR FROM {1}) AND EXTRACT(MONTH FROM {0}) = EXTRACT(MONTH FROM {1}))", date1, date2);
            }
            else if (dateType == "day")
            {
                return string.Format("(TRUNC({0}) = TRUNC({1}))", date1, date2);
            }
            else if (dateType == "hour")
            {
                return string.Format("(TRUNC({0}, 'HH24') = TRUNC({1}, 'HH24'))", date1, date2);
            }
            else if (dateType == "minute")
            {
                return string.Format("(TRUNC({0}, 'MI') = TRUNC({1}, 'MI'))", date1, date2);
            }
            else if (dateType == "second")
            {
                return string.Format("(TRUNC({0}, 'SS') = TRUNC({1}, 'SS'))", date1, date2);
            }
            else if (dateType == "week" || dateType == "weekday")
            {
                return string.Format("(TRUNC({0}, 'IW') = TRUNC({1}, 'IW'))", date1, date2);
            }
            else
            {
                // 默认按天比较
                return string.Format("(TRUNC({0}) = TRUNC({1}))", date1, date2);
            }
        }
        public override string Length(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" LENGTH({0}) ", parameter.MemberName);
        }

        public override string IsNull(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            return string.Format("NVL({0},{1})", parameter.MemberName, parameter1.MemberName);
        }

        public override string MergeString(params string[] strings)
        {
            return string.Join("||", strings);
        }

        public override string GetDate()
        {
            return "sysdate";
        }

        public override string GetRandom()
        {
            return "dbms_random.value";
        }

        public override string CharIndex(MethodCallExpressionModel model)
        {
            return string.Format("instr ({0},{1},1,1) ", model.Args[0].MemberName, model.Args[1].MemberName);
        }

        public override string ToDecimal(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS decimal(18,4))", parameter.MemberName);
        }

        public override string TrimEnd(MethodCallExpressionModel mode)
        {
            var parameterNameA = mode.Args[0].MemberName;
            var parameterNameB = mode.Args[1].MemberName;
            return $" RTRIM({parameterNameA}, {parameterNameB}) ";
        }
        public override string TrimStart(MethodCallExpressionModel mode)
        {

            var parameterNameA = mode.Args[0].MemberName;
            var parameterNameB = mode.Args[1].MemberName;
            return $" LTRIM({parameterNameA}, {parameterNameB}) ";
        }

        public override string Left(MethodCallExpressionModel mode)
        {
            var parameterNameA = mode.Args[0].MemberName;
            var parameterNameB = mode.Args[1].MemberName;
            return $" SUBSTR({parameterNameA}, 1, {parameterNameB})  ";
        }
        public override string Right(MethodCallExpressionModel mode)
        {
            var parameterNameA = mode.Args[0].MemberName;
            var parameterNameB = mode.Args[1].MemberName;
            return $" SUBSTR({parameterNameA}, -2, {parameterNameB})  ";
        }

        public override string Ceil(MethodCallExpressionModel mode)
        {
            var parameterNameA = mode.Args[0].MemberName;
            return $" CEIL({parameterNameA}) ";
        }

        public override string NewUid(MethodCallExpressionModel mode)
        {
            return "   SUBSTR(LOWER(RAWTOHEX(SYS_GUID())), 1, 8) ||\r\n  '-' ||\r\n  SUBSTR(LOWER(RAWTOHEX(SYS_GUID())), 9, 4) ||\r\n  '-' ||\r\n  SUBSTR(LOWER(RAWTOHEX(SYS_GUID())), 13, 4) ||\r\n  '-' ||\r\n  SUBSTR(LOWER(RAWTOHEX(SYS_GUID())), 17, 4) ||\r\n  '-' ||\r\n  SUBSTR(LOWER(RAWTOHEX(SYS_GUID())), 21)  ";
        }
        public override string JsonField(MethodCallExpressionModel model)
        {
            return $"JSON_VALUE({model.Args[0].MemberName}, '$.{model.Args[1].MemberValue.ToString().ToSqlFilter()}')";
            //"JSON_VALUE(j.kingorder, '$.Id') = '1'";
        }

        public override string FullTextContains(MethodCallExpressionModel mode)
        {
            var columns = mode.Args[0].MemberName;
            if (mode.Args[0].MemberValue is List<string>)
            {
                columns = "(" + string.Join(",", mode.Args[0].MemberValue as List<string>) + ")";
            }
            var searchWord = mode.Args[1].MemberName;
            return $" CONTAINS({columns}, {searchWord}, 1) ";
        }
    }
}
