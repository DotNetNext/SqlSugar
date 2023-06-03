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
            var result = "GROUP BY ";
            if (this.Context.JoinIndex == 0)
            {
                result = result + SubTools.GetMethodValue(this.Context, argExp, type);
            }
            else 
            {
                if (type == ResolveExpressType.ArraySingle) 
                {
                    type= ResolveExpressType.ArrayMultiple;
                }
                else if (type == ResolveExpressType.FieldSingle)
                {
                    type = ResolveExpressType.FieldMultiple;
                }
                else if (type == ResolveExpressType.WhereSingle)
                {
                    type = ResolveExpressType.WhereMultiple;
                }
                result = result + SubTools.GetMethodValueSubJoin(this.Context, argExp, type);
            }
            result = result.TrimEnd(',');
            var selfParameterName = this.Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            if (this.Context.JoinIndex == 0)
                result = result.Replace(selfParameterName, SubTools.GetSubReplace(this.Context));
            if (this.Context.CurrentShortName == null) 
            {
                this.Context.CurrentShortName =this.Context.GetTranslationColumnName(ExpressionTool.GetParameters(exp).FirstOrDefault().Name);
            }
            return result;
        }
    }
}
