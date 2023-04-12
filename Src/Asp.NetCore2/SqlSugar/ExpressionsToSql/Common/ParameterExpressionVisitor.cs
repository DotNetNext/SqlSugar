using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
}
