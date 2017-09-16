using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public interface ISubOperation
    {
        string Name { get; }
        string GetValue(ExpressionContext context, Expression expression);
        int Sort { get; }
        Expression Expression { get; set; }
    }
}
