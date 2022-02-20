using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.Access
{
    public partial class AccessExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarProvider Context { get; set; }
        public AccessExpressionContext()
        {
            base.DbMehtods = new AccessMethod();
        }

    }
    public partial class AccessMethod : DefaultDbMethod, IDbMethods
    {
        public override string GetRandom()
        {
            return " rnd() ";
        }
        public override string GetDate()
        {
            return " NOW()";
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
