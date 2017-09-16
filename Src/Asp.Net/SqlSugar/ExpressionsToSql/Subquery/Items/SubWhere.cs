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
            var argExp= exp.Arguments[0];
            var result= "WHERE "+SubTools.GetMethodValue(context, argExp, ResolveExpressType.WhereMultiple);
            var selfParameterName =context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name)+UtilConstants.Dot;
            result = result.Replace(selfParameterName,string.Empty);
            return result;
        }
    }
}
