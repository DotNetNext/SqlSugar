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
    public partial class OracleExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarClient Context { get; set; }
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
            return base.GetTranslationTableName(entityName, isMapping).ToUpper();
        }
        public override string GetTranslationColumnName(string columnName)
        {
            return base.GetTranslationColumnName(columnName).ToUpper();
        }
        public override string GetDbColumnName(string entityName, string propertyName)
        {
            return base.GetDbColumnName(entityName,propertyName).ToUpper();
        }
    }
    public partial class OracleMethod : DefaultDbMethod, IDbMethods
    {
        public override string DateAddByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" (DATEADD({2},{1},{0})) ", parameter.MemberName, parameter2.MemberName, parameter3.MemberValue);
        }

        public override string DateAddDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" (DATEADD(day,{1},{0})) ", parameter.MemberName, parameter2.MemberName);
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
            var parameter = model.Args[0].MemberName;
            var parameter2 = model.Args[1].MemberName;
            return string.Format(" ROUND(TO_NUMBER({0} - {1})) ", parameter, parameter2);
        }
        public override string DateIsSameByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0].MemberName;
            var parameter2 = model.Args[1].MemberName;
            var parameter3 = model.Args[2].MemberValue;
            var type = (DateType)Enum.Parse(typeof(DateType), parameter3.ObjToString(), false);
            int time = 1;
            switch (type)
            {
                case DateType.Year:
                    time = time * 1 / 365;
                    break;
                case DateType.Month:
                    time = time * 1 / 30;
                    break;
                case DateType.Day:
                    break;
                case DateType.Hour:
                    time = time * 24;
                    break;
                case DateType.Second:
                    time = time * 24 * 60 * 60;
                    break;
                case DateType.Minute:
                    time = time * 24 * 60;
                    break;
                case DateType.Millisecond:
                    time = time * 24 * 60 * 60 * 1000;
                    break;
            }
            return string.Format(" ROUND(TO_NUMBER({0} - {1}) * {2}) ", parameter, parameter2, time);
        }
    }
}
