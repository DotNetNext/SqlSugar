using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.ExpressionsToSql.Subquery
{
    public class SubWhere: ISubOperation
    {
        public string Name
        {
            get { return "Where"; }
        }

        public Expression Expression
        {
            get; set;
        }

        public int Sort
        {
            get
            {
                return 400;
            }
        }

        public string GetValue(ExpressionContext context, Expression expression)
        {
            var exp = expression as MethodCallExpression;
            return "WHERE "+SubTool.GetMethodValue(context, exp.Arguments[0], ResolveExpressType.WhereMultiple);
        }
    }
}
