using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar 
{
    internal class ParameterReplacer : ExpressionVisitor
    {
        private readonly string _oldParameterName;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(string oldParameterName, ParameterExpression newParameter)
        {
            _oldParameterName = oldParameterName;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node.Name == _oldParameterName ? _newParameter : base.VisitParameter(node);
        }
    }
}
