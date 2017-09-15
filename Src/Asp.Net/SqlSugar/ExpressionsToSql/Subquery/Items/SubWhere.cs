using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.ExpressionsToSql.Subquery
{
    public class SubWhere : ISubOperation
    {
        public string Name
        {
            get { return "Where"; }
        }

        public int Sort
        {
            get
            {
                return 400;
            }
        }

        public string GetValue(ExpressionContext context, Expression expression = null)
        {
            var newContext = context.GetCopyContext();
            newContext.ParameterIndex = context.ParameterIndex;
            newContext.Resolve(expression, ResolveExpressType.WhereMultiple);
            context.Parameters.AddRange(newContext.Parameters);
            context.ParameterIndex = newContext.ParameterIndex;
            return newContext.Result.GetResultString();
        }
    }
}
