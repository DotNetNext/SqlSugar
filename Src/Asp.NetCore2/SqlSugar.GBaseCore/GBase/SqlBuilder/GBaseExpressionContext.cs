using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.GBase
{
    public partial class GBaseExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarProvider Context { get; set; }
        public GBaseExpressionContext()
        {
            base.DbMehtods = new GBaseMethod();
        }
        public override string SqlTranslationLeft { get { return ""; } }
        public override string SqlTranslationRight { get { return ""; } }
        public override bool IsTranslationText(string name)
        {
            var result = name.IsContainsIn( UtilConstants.Space,"(",")");
            return result;
        }
        public override string GetLimit() { return ""; }
        public override string GetTranslationText(string name)
        {
            if (name.Contains('.'))
            {
                // add "\"" to support alias name format : table.column
                // delimident=y should be added to connection string.
                return base.GetTranslationText("\"" + name + "\"");
            }

            return base.GetTranslationText(name);
        }
    }
    public partial class GBaseMethod : DefaultDbMethod, IDbMethods
    {
        private string _dateTimeType = "datetime year to fraction(5)";
        public override string Length(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" LENGTH({0}) ", parameter.MemberName);
        }

        public override string IsNull(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            string str = string.Format("NVL({0},{1})", parameter.MemberName, parameter1.MemberName);

            if (parameter1 != null && parameter1.MemberValue != null)
            {
                if (parameter1.MemberValue.GetType() == UtilConstants.DateType)
                {
                    str += string.Format("::{0}", _dateTimeType);
                }
            }
            return str;
        }

        public override string MergeString(params string[] strings)
        {
            return string.Join("||", strings);
        }

        public override string GetRandom()
        {
            return " SYS_GUID() ";
        }
        public override string GetForXmlPath()
        {
            return "  FOR XML PATH('')),1,len(N','),'')  ";
        }
        public override string GetStringJoinSelector(string result, string separator)
        {
            return $"stuff((SELECT cast(N'{separator}' as nvarchar(max)) + cast({result} as nvarchar(max))";
        }
        public override string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            string str = string.Empty;
            if (parameter.MemberName != null && parameter.MemberName is DateTime)
            {
                switch (parameter2.MemberValue.ToString().ToLower())
                {
                    case "year":
                        str = string.Format(" year('{0}'::{1}) ", parameter.MemberName, _dateTimeType);
                        break;
                    case "month":
                        str = string.Format(" month('{0}'::{1}) ", parameter.MemberName, _dateTimeType);
                        break;
                    case "day":
                        str = string.Format(" day('{0}'::{1}) ", parameter.MemberName, _dateTimeType);
                        break;
                    case "hour":
                        str = string.Format(" extend('{0}'::{1}, hour to hour) ", parameter.MemberName, _dateTimeType);
                        break;
                    case "minute":
                        str = string.Format(" extend('{0}'::{1}, minute to minute) ", parameter.MemberName, _dateTimeType);
                        break;
                    case "second":
                        str = string.Format(" extend('{0}'::{1}, second to second) ", parameter.MemberName, _dateTimeType);
                        break;
                }
            }
            else
            {
                switch (parameter2.MemberValue.ToString().ToLower())
                {
                    case "year":
                        str = string.Format(" year({0}::{1}) ", parameter.MemberName, _dateTimeType);
                        break;
                    case "month":
                        str = string.Format(" month({0}::{1}) ", parameter.MemberName, _dateTimeType);
                        break;
                    case "day":
                        str = string.Format(" day({0}::{1}) ", parameter.MemberName, _dateTimeType);
                        break;
                    case "hour":
                        str = string.Format(" extend({0}::{1}, hour to hour)::varchar(2) ", parameter.MemberName, _dateTimeType);
                        break;
                    case "minute":
                        str = string.Format(" extend({0}::{1}, minute to minute)::varchar(2) ", parameter.MemberName, _dateTimeType);
                        break;
                    case "second":
                        str = string.Format(" extend({0}::{1}, second to second)::varchar(2) ", parameter.MemberName, _dateTimeType);
                        break;
                }
            }
            return str;
        }
        public override string GetDate()
        {
            return " sysdate  ";
        }

        public override string HasValue(MethodCallExpressionModel model)
        {
            if (model.Args[0].Type == UtilConstants.GuidType)
            {
                var parameter = model.Args[0];
                return string.Format("( {0} IS NOT NULL )", parameter.MemberName);
            }
            else
            {
                var parameter = model.Args[0];
                return string.Format("( {0}<>'' AND {0} IS NOT NULL )", parameter.MemberName);
            }
        }
        public override string CharIndex(MethodCallExpressionModel model)
        {
            return string.Format("instr ({0},{1},1,1) ", model.Args[0].MemberName, model.Args[1].MemberName);
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
        public override string DateDiff(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" DATEDIFF('{0}',{1},{2}) ", parameter.MemberValue?.ToString().ToSqlFilter(), parameter2.MemberName, parameter3.MemberName);
        }
        public override string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS NVARCHAR(4000))", parameter.MemberName);
        }

        public override string EqualTrue(string fieldName)
        {
            return "( " + fieldName + "='t' )";
        }

        public override string ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS DATETIME year to fraction(5))", parameter.MemberName);
        }

        public override string DateAddByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            string str = string.Empty;
            switch (parameter3.MemberValue.ToString().ToLower())
            {
                case "year":
                    str = string.Format(" ({0}::{2}+ {1} units year)", parameter.MemberName, parameter2.MemberName, _dateTimeType);
                    break;
                case "month":
                    str = string.Format(" ({0}::{2}+ {1} units month)", parameter.MemberName, parameter2.MemberName, _dateTimeType);
                    break;
                case "day":
                    str = string.Format(" ({0}::{2}+ {1} units day)", parameter.MemberName, parameter2.MemberName, _dateTimeType);
                    break;
                case "hour":
                    str = string.Format(" ({0}::{2}+ {1} units hour)", parameter.MemberName, parameter2.MemberName, _dateTimeType);
                    break;
                case "minute":
                    str = string.Format(" ({0}::{2}+ {1} units minute)", parameter.MemberName, parameter2.MemberName, _dateTimeType);
                    break;
                case "second":
                    str = string.Format(" ({0}::{2}+ {1} units second)", parameter.MemberName, parameter2.MemberName, _dateTimeType);
                    break;
            }

            return str;
        }
    }
}
