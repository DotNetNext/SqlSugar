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
                string appendString = "(" +((MemberExpression)expression).Member.Name + "=1)";
                base.Context.SqlWhere.Append(appendString);
            }
            else
            {
                base.Expression = expression;
                base.Start();
            }
        }
    }
}
