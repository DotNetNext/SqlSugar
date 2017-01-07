using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class UnaryExpressionResolve:BaseResolve
    {
        public UnaryExpressionResolve(Expression exp) : base(exp)
        {

        }
    }
}
