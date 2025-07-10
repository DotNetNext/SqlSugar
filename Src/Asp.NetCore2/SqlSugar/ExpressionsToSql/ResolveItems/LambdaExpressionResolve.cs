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
            LambdaExpression lambda = base.Expression as LambdaExpression;
            var expression = lambda.Body;
            base.Expression = expression;
            ResolveUnaryExpression(expression);
            if (parameter.Context.ResolveType.IsIn(ResolveExpressType.FieldMultiple, ResolveExpressType.FieldSingle))
            {
                parameter.CommonTempData = CommonTempDataType.Append;
            }
            base.Start();
        }

        private void ResolveUnaryExpression(Expression expression)
        {
            if (this.Context?.ResolveType == ResolveExpressType.SelectSingle && expression is UnaryExpression)
            {
                base.Expression = ExpressionTool.RemoveConvert(base.Expression);
            }
        }
    }
}
