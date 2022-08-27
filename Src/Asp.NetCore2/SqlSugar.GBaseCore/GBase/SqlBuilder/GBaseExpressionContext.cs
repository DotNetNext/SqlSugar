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

    }
    public partial class GBaseMethod : DefaultDbMethod, IDbMethods
    {
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
    }
}
