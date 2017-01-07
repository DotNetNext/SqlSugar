using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class LambdaExpressionResolve : BaseResolve
    {
        public LambdaExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            LambdaExpression lambda =base.Expression as LambdaExpression;
            var expression = lambda.Body;
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                base.SqlWhere = "(" + ((MemberExpression)expression).Member.Name + "=1)";
            }
            else
            {
                base.Expression = expression;
                base.Start();
            }
        }
    }
}
