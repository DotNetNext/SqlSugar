using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class NavgateExpression
    {
        internal bool IsNavgate(Expression expression)
        {
            var exp = expression;
            if (exp is UnaryExpression) 
            {
                exp = (exp as UnaryExpression).Operand;
            }
            if (exp is MemberExpression) 
            {
                var memberExp=exp as MemberExpression;
            }
            return false;
        }

        internal void Execute(MapperExpressionResolve mapperExpressionResolve)
        {
          
        }
    }
}
