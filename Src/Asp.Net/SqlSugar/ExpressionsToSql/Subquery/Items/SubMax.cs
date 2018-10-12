using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubMax:ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "Max";
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

        public ExpressionContext Context
        {
            get; set;
        }

        public string GetValue(Expression expression = null)
        {
            var exp = expression as MethodCallExpression;
            var argExp = exp.Arguments[0];
            var result = "MAX(" + SubTools.GetMethodValue(Context, argExp, ResolveExpressType.WhereMultiple) + ")";
            var selfParameterName = Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            result = result.Replace(selfParameterName, string.Empty);
            return result;
        }
    }
}
