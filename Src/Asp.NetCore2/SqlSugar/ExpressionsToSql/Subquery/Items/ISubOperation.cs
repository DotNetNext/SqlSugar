using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public interface ISubOperation
    {
        ExpressionContext Context { get; set; }
        string Name { get; }
        string GetValue(Expression expression);
        int Sort { get; }
        Expression Expression { get; set; }
        bool HasWhere { get; set; }
    }
}
