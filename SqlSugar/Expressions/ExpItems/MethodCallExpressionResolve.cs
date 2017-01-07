using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class MethodCallExpressionResolve:BaseResolve
    {
        public MethodCallExpressionResolve(Expression exp) : base(exp)
        {

        }
    }
}
