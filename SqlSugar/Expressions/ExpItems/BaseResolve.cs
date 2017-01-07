using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class BaseResolve
    {
        protected Expression Expression { get; set; }
        public ExpressionContext Context { get; set; }
        public string SqlWhere { get; set; }
        public bool IsFinished { get; set; }

        private BaseResolve()
        {

        }
        public BaseResolve(Expression expression)
        {
            this.Expression = expression;
        }

        public BaseResolve Start()
        {
            this.IsFinished = false;
            Expression exp = this.Expression;
            if (exp is LambdaExpression)
            {
                return new LambdaExpressionResolve(exp);
            }
            else if (exp is BinaryExpression)
            {
                return new BinaryExpressionResolve(exp);
            }
            else if (exp is BlockExpression)
            {
                Check.ThrowNotSupportedException("BlockExpression");
            }
            else if (exp is ConditionalExpression)
            {
                Check.ThrowNotSupportedException("ConditionalExpression");
            }
            else if (exp is MethodCallExpression)
            {
                return new MethodCallExpressionResolve(exp);
            }
            else if (exp is ConstantExpression)
            {
                return new ConstantExpressionResolve(exp);
            }
            else if (exp is MemberExpression)
            {
                return new MemberExpressionResolve(exp);
            }
            else if (exp is UnaryExpression)
            {
                return new UnaryExpressionResolve(exp);
            }
            else if (exp != null && exp.NodeType.IsIn(ExpressionType.New, ExpressionType.NewArrayBounds, ExpressionType.NewArrayInit))
            {
                Check.ThrowNotSupportedException("ExpressionType.New、ExpressionType.NewArrayBounds and ExpressionType.NewArrayInit");
            }
            return null;
        }
        public void Continue()
        {
            if (!IsFinished)
            {
                this.Start();
            }
        }
    }
}
