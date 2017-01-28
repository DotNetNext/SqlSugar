using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class BinaryExpressionResolve : BaseResolve
    {
        public BinaryExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            if (parameter.BaseParameter.CommonTempData != null && parameter.BaseParameter.CommonTempData.Equals("simple"))
            {
                parameter.BaseParameter = parameter;
                new SimpleBinaryExpressionResolve(parameter);
                this.Context.Result.CurrentParameter = null;
            }
            else
            {
                var expression = this.Expression as BinaryExpression;
                var operatorValue = ExpressionTool.GetOperator(expression.NodeType);
                var isComparisonOperator =
                                            expression.NodeType != ExpressionType.And &&
                                            expression.NodeType != ExpressionType.AndAlso &&
                                            expression.NodeType != ExpressionType.Or &&
                                            expression.NodeType != ExpressionType.OrElse;
                base.BaseExpression = expression;
                var leftExpression = expression.Left;
                var rightExpression = expression.Right;
                var leftIsBinary = leftExpression is BinaryExpression;
                var rightBinary = rightExpression is BinaryExpression;
                var lbrs = leftIsBinary && !rightBinary;
                var lsrb = !leftIsBinary && rightBinary;
                var lbrb = rightBinary && leftIsBinary;
                var lsbs = !leftIsBinary && !rightBinary;
                if (!base.Context.Result.Contains(ExpressionConst.Format0))
                {
                    base.Context.Result.Append(ExpressionConst.Format3);
                    base.Context.Result.Append(ExpressionConst.Format0);
                }
                else {
                    base.Context.Result.Replace(ExpressionConst.Format0,ExpressionConst.Format3+ ExpressionConst.Format0);
                }
                parameter.LeftExpression = leftExpression;
                parameter.RightExpression = rightExpression;
                base.Expression = leftExpression;
                base.IsLeft = true;
                base.Start();
                base.Context.Result.Replace(ExpressionConst.Format1+parameter.Index,operatorValue);
                base.IsLeft = false;
                base.Expression = rightExpression;
                base.Start();
                base.IsLeft = null;
                base.Context.Result.Append(ExpressionConst.Format4);
                if (parameter.BaseExpression is BinaryExpression && parameter.IsLeft == true)
                {
                    base.Context.Result.Append(" "+ExpressionConst.Format1 + parameter.BaseParameter.Index+" ");
                }
            }
        }
    }
}
