using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SqlSugar 
{
    internal class ParameterExpressionVisitor : ExpressionVisitor
    {
        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Parameters.Add(node);
            return base.VisitParameter(node);
        }
    }
    internal class MethodCallExpressionVisitor : ExpressionVisitor
    {
        private readonly string _methodName;
        private bool _hasMethodCallWithName;

        public MethodCallExpressionVisitor(string methodName)
        {
            _methodName = methodName;
        }

        public bool HasMethodCallWithName(Expression expression)
        {
            Visit(expression);
            return _hasMethodCallWithName;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name.Contains(_methodName))
            {
                _hasMethodCallWithName = true;
                return node;
            }

            return base.VisitMethodCall(node);
        }
    } 
}
