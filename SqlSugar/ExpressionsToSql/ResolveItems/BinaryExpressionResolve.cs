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
            parameter.TempData = new Dictionary<string, object>();
            var expression = this.Expression as BinaryExpression;
            parameter.TempData.Add(ExpConst.Operator, ExpTool.GetOperator(expression.NodeType));
            base.BaseExpression = expression;
            base.IsLeft = true;
            base.Expression = expression.Left;
            base.Start();
            base.IsLeft = false;
            base.Expression = expression.Right;
            base.Start();
            base.IsLeft = null;
        }
    }
}
