using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class OracleExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarProvider Context { get; set; }
        public OracleExpressionContext()
        {
            base.DbMehtods = new OracleMethod(); 
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
            if (columnName == "systimestamp") 
            {
                return columnName;
            }
            if (columnName.Contains(":"))
                return base.GetTranslationColumnName(columnName);
            else if (columnName.Contains("\".\""))
            {
                return columnName;
            }
            else
            return base.GetTranslationColumnName(columnName).ToUpper(IsUppper);
        }
        public override string GetDbColumnName(string entityName, string propertyName)
        {
            return base.GetDbColumnName(entityName,propertyName).ToUpper(IsUppper);
        }
        public override bool IsTranslationText(string name)
        {
            if (!string.IsNullOrEmpty(name) && name.ToLower() == "sysdate") 
            {
                return true;
            }
            var result = name.IsContainsIn(SqlTranslationLeft, SqlTranslationRight, UtilConstants.Space, ExpressionConst.LeftParenthesis, ExpressionConst.RightParenthesis);
            return result;
        }
        public bool IsUppper
        {
            get
            {
                if (this.SugarContext?.Context?.Context?.CurrentConnectionConfig?.MoreSettings == null)
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
    public partial class OracleMethod : DefaultDbMethod, IDbMethods
    {
        public override string IsNullOrEmpty(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("( {0} IS NULL )", parameter.MemberName);
        }
        public override string WeekOfYear(MethodCallExpressionModel mode)
        {
            var parameterNameA = mode.Args[0].MemberName;
            return $"TO_NUMBER(TO_CHAR({parameterNameA}, 'WW')) ";
        }
        public override string BitwiseAnd(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" BITAND({0},{1}) ", parameter.MemberName, parameter2.MemberName);
        }
        public override string BitwiseInclusiveOR(MethodCallExpressionModel model)
        { 
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" BITOR({0},{1}) ", parameter.MemberName, parameter2.MemberName);
        }
        public override string ParameterKeyWord { get; set; } = ":";
        public override string Modulo(MethodCallExpressionModel model)
        {
            return " MOD(" + model.Args[0].MemberName+ " , " + model.Args[1].MemberName+")";
        }
        public override string GetStringJoinSelector(string result, string separator)
        {
            return $"listagg(to_char({result}),'{separator}') within group(order by {result}) ";
        }
        public override string HasValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("( {0} IS NOT NULL ) ", parameter.MemberName);
        }

        public override string DateDiff(MethodCallExpressionModel model)
        {
            var parameter = (DateType)(Enum.Parse(typeof(DateType), model.Args[0].MemberValue.ObjToString()));
            var begin = model.Args[1].MemberName;
            var end = model.Args[2].MemberName;
            switch (parameter)
            {
                case DateType.Year:
                    return $" ( cast((months_between( {end} ,  {begin}))/12 as number(9,0) ) )";
                case DateType.Month:
                    return $" ( cast((months_between( {end} ,  {begin})) as number(9,0) ) )";
                case DateType.Day:
                    return $" ( ROUND(TO_NUMBER(cast({end} as date) - cast({begin} as date)))  )";
                case DateType.Hour:
                    return $" ( ROUND(TO_NUMBER(cast({end} as date) - cast({begin} as date)) * 24)  )";
                case DateType.Minute:
                    return $" ( ROUND(TO_NUMBER(cast({end} as date) - cast({begin} as date)) * 24 * 60) )";
                case DateType.Second:
                    return $" ( ROUND(TO_NUMBER(cast({end} as date) - cast({begin} as date)) * 24 * 60 * 60) )";
                case DateType.Millisecond:
                    return $" ( ROUND(TO_NUMBER(cast({end} as date) - cast({begin} as date)) * 24 * 60 * 60 * 60) )";
                default:
                    break;
            }
            throw new Exception(parameter + " datediff no support");
        }
        private void PageEach<T>(IEnumerable<T> pageItems, int pageSize, Action<List<T>> action)
        {
            if (pageItems != null && pageItems.Any())
            {
                int totalRecord = pageItems.Count();
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                for (int i = 1; i <= pageCount; i++)
                {
                    var list = pageItems.Skip((i - 1) * pageSize).Take(pageSize).ToList();
                    action(list);
                }
            }
        }
        public override string ContainsArray(MethodCallExpressionModel model)
        {
            if (model.Args[0].MemberValue == null)
            {
                return base.ContainsArray(model);
            }
            var inValueIEnumerable = ((IEnumerable)model.Args[0].MemberValue).Cast<object>().ToArray();
            if (inValueIEnumerable.Count() < 1000)
            {
                return base.ContainsArray(model);
            }
            else
            {
                string result = "";
                PageEach(inValueIEnumerable, 999, it =>
                {
                    model.Args.First().MemberValue = it;
                    result+= (base.ContainsArray(model) + " OR ");

                });
                return " ( "+result.TrimEnd(' ').TrimEnd('R').TrimEnd('O')+" ) ";
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
                case DateType.Weekday:
                    return $" to_char({parameter.MemberName},'day') ";
                case DateType.Day:
                default:
                    return string.Format("(CAST(TO_CHAR({0},'dd') AS NUMBER))", parameter.MemberName);
            }
        }
        public override string DateAddByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            var type = (DateType)Enum.Parse(typeof(DateType), parameter3.MemberValue.ObjToString(), false);
            double time = 1;
            switch (type)
            {
                case DateType.Year:
                    time = 1 * 365;
                    break;
                case DateType.Month:
                    time = 1 *30;
                    break;
                case DateType.Day:
                    break;
                case DateType.Hour:
                    time = 1 / 24.0;
                    break;
                case DateType.Second:
                    time = 1 / 24.0/60.0/60.0;
                    break;
                case DateType.Minute:
                    time = 1 / 24.0/60.0;
                    break;
                case DateType.Millisecond:
                    time = 1 / 24.0 / 60.0 / 60.0/1000;
                    break;
            }
            return string.Format("({0}+({1}*{2})) ", parameter.MemberName, time,parameter2.MemberName);
        }

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

        public override string ToDecimal(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS Number)", parameter.MemberName);
        }

        public override string ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" TO_TIMESTAMP({0}, 'YYYY-MM-DD HH24:MI:SS.FF') ", parameter.MemberName);
        }

        public override string ToDateShort(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("  TRUNC({0},'dd') ", parameter.MemberName);
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
            throw new NotSupportedException("Oracle NotSupportedException DateIsSameDay");
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
            return "systimestamp";
        }

        public override string GetRandom()
        {
            return "dbms_random.value";
        }

        public override string Collate(MethodCallExpressionModel model)
        {
            var name = model.Args[0].MemberName;
            return $"  NLSSORT({0}, 'NLS_SORT = Latin_CI')   ";
        }

        public override string JsonField(MethodCallExpressionModel model)
        {
            return $"JSON_VALUE({model.Args[0].MemberName}, '$.{model.Args[1].MemberValue.ToString().ToSqlFilter()}')";
            //"JSON_VALUE(j.kingorder, '$.Id') = '1'";
        }

        public override string CharIndex(MethodCallExpressionModel model)
        {
            return string.Format("instr ({0},{1},1,1) ", model.Args[0].MemberName, model.Args[1].MemberName);
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
