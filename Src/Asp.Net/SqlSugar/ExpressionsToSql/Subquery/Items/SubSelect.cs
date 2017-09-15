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

        public int Sort
        {
            get
            {
                return 200;
            }
        }

        public string GetValue(ExpressionContext context, Expression expression = null)
        {
           var newContext=context.GetCopyContext();
            newContext.ParameterIndex = context.ParameterIndex;
            newContext.Resolve(expression, ResolveExpressType.SelectMultiple);
            context.Parameters.AddRange(newContext.Parameters);
            context.ParameterIndex = newContext.ParameterIndex;
            return newContext.Result.GetResultString();
        }
    }
}
