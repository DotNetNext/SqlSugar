using System;
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
            return base.GetTranslationTableName(entityName, isMapping).ToUpper();
        }
        public override string GetTranslationColumnName(string columnName)
        {
            return base.GetTranslationColumnName(columnName).ToUpper();
        }
        public override string GetDbColumnName(string entityName, string propertyName)
        {
            return base.GetDbColumnName(entityName, propertyName).ToUpper();
        }
    }
    public partial class DmMethod : DefaultDbMethod, IDbMethods
    {
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
                    time = 1 * 30;
                    break;
                case DateType.Day:
                    break;
                case DateType.Hour:
                    time = 1 / 24.0;
                    break;
                case DateType.Second:
                    time = 1 / 24.0 / 60.0 / 60.0;
                    break;
                case DateType.Minute:
                    time = 1 / 24.0 / 60.0;
                    break;
                case DateType.Millisecond:
                    time = 1 / 24.0 / 60.0 / 60.0 / 1000;
                    break;
            }
            return string.Format("({0}+({1}*{2})) ", parameter.MemberName, time, parameter2.MemberName);
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
            throw new NotSupportedException("Oracle NotSupportedException DateIsSameDay");
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
    }
}
