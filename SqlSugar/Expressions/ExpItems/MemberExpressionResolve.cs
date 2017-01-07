using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class MemberExpressionResolve:BaseResolve
    {
        public MemberExpressionResolve(Expression exp) : base(exp)
        {
            var isLeft = this.IsLeft;
            this.IsLeft = null;
            var isSingle=base.Context.IsSingle;
            var expression = exp as MemberExpression;
        }
    }
}
