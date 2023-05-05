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

    internal class ExpressionTreeVisitor : ExpressionVisitor
    {
        private readonly List<Expression> _nodes = new List<Expression>();
        //protected override Expression VisitBinary(BinaryExpression node)
        //{
        //    // 解析二元操作符表达式
        //    _nodes.Add(node);
        //    Visit(node.Left);
        //    Visit(node.Right);
        //    return node;
        //}
        //protected override Expression VisitConstant(ConstantExpression node)
        //{
        //    // 解析常量表达式
        //    _nodes.Add(node);
        //    return node;
        //}
        protected override Expression VisitMember(MemberExpression node)
        {
            // 解析成员访问表达式
            _nodes.Add(node);
            return node;
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // 解析方法调用表达式
            _nodes.Add(node);
            if (node.Arguments.Any())
            {
                Visit(node.Arguments.First());
            }
            return node;
        }
        public List<Expression> GetExpressions(Expression expression)
        {
            Visit(expression);
            _nodes.Reverse();
            return _nodes;
        }
    } 

}
