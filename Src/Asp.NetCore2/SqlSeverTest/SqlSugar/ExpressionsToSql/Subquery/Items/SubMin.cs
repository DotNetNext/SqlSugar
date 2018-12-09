using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubMin: ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "Min";
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
            var parametres = (argExp as LambdaExpression).Parameters;
            if ((argExp as LambdaExpression).Body is UnaryExpression)
            {
                argExp = ((argExp as LambdaExpression).Body as UnaryExpression).Operand;
            }
            var result = "MIN(" + SubTools.GetMethodValue(Context, argExp, ResolveExpressType.WhereMultiple) + ")";
            var selfParameterName = Context.GetTranslationColumnName(parametres.First().Name) + UtilConstants.Dot;
            result = result.Replace(selfParameterName, SubTools.GetSubReplace(this.Context));
            return result;
        }
    }
}
