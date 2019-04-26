using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubGroupBy : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get { return "GroupBy"; }
        }

        public Expression Expression
        {
            get; set;
        }

        public int Sort
        {
            get
            {
                return 479;
            }
        }

        public ExpressionContext Context
        {
            get; set;
        }

        public string GetValue(Expression expression)
        {
            var exp = expression as MethodCallExpression;
            var argExp = exp.Arguments[0];
            var type = ResolveExpressType.FieldSingle;
            if ((argExp as LambdaExpression).Body is NewExpression) {
                type = ResolveExpressType.ArraySingle;
            }
            var result = "GROUP BY " + SubTools.GetMethodValue(this.Context, argExp,type);
            result = result.TrimEnd(',');
            var selfParameterName = this.Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            result = result.Replace(selfParameterName, SubTools.GetSubReplace(this.Context));
            return result;
        }
    }
}
