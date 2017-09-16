using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubSelect : ISubOperation
    {
        public string Name
        {
            get
            {
                return "Select";
            }
        }

        public Expression Expression
        {
            get; set;
        }


        public int Sort
        {
            get
            {
                return 200;
            }
        }

        public string GetValue(ExpressionContext context, Expression expression = null)
        {
            var exp = expression as MethodCallExpression;
            return SubTools.GetMethodValue(context, exp.Arguments[0],ResolveExpressType.FieldSingle);
        }
    }
}
