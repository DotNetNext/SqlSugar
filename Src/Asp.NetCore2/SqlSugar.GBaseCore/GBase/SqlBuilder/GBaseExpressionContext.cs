using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    }
    public partial class GBaseMethod : DefaultDbMethod, IDbMethods
    {
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
            if (parameter.MemberName != null && parameter.MemberName is DateTime)
            {
                return string.Format(" datepart({0},'{1}') ", parameter2.MemberValue, parameter.MemberName);
            }
            else
            {
                return string.Format(" datepart({0},{1}) ", parameter2.MemberValue, parameter.MemberName);
            }
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
    }
}
