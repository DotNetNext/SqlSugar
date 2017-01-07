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

        }
    }
}
