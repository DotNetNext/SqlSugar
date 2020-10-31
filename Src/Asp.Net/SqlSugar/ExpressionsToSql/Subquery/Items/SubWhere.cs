using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class SubWhere: ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

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

        public ExpressionContext Context
        {
            get;set;
        }

        public string GetValue(Expression expression)
        {
            var exp = expression as MethodCallExpression;
            var argExp= exp.Arguments[0];
            var result= "WHERE "+SubTools.GetMethodValue(Context, argExp, ResolveExpressType.WhereMultiple);


            var regex = @"^WHERE  (\@Const\d+) $";
            if (this.Context is OracleExpressionContext)
            {
                regex = @"^WHERE  (\:Const\d+) $";
            }
            if (this.Context is DmExpressionContext)
            {
                regex = @"^WHERE  (\:Const\d+) $";
            }
            if (Regex.IsMatch(result, regex))
            {
                result = "WHERE " + this.Context.Parameters.First(it => it.ParameterName == Regex.Match(result, regex).Groups[1].Value).Value;
                return result;
            }

            var selfParameterName = Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name)+UtilConstants.Dot;
            result = result.Replace(selfParameterName,SubTools.GetSubReplace(this.Context));
            return result;
        }
    }
}
