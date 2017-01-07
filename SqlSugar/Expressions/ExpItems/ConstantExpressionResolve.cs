using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class ConstantExpressionResolve:BaseResolve
    {
        public ConstantExpressionResolve(Expression exp) : base(exp)
        {

        }
    }
}
