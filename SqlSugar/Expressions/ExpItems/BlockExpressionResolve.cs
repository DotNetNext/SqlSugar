using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class BlockExpressionResolve:BaseResolve
    {
        public BlockExpressionResolve(Expression exp):base(exp)
        {

        }
    }
}
