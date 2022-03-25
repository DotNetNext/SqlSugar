using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubOrderBy : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get { return "OrderBy"; }
        }

        public Expression Expression
        {
            get; set;
        }

        public int Sort
        {
            get
            {
                return 480;
            }
        }

        public ExpressionContext Context
        {
            get; set;
        }

        public string GetValue(Expression expression)
        {
            if (this.Context is OracleExpressionContext)
            {
                throw new Exception("Oracle Subquery can't OrderBy");
            }
            var exp = expression as MethodCallExpression;
            var argExp = exp.Arguments[0];
            var result = "ORDER BY " + SubTools.GetMethodValue(this.Context, argExp, ResolveExpressType.FieldSingle);
            var selfParameterName = this.Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            result = result.Replace(selfParameterName, SubTools.GetSubReplace(this.Context));
            return result;
        }
    }
    public class SubOrderByDesc : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get { return "OrderByDesc"; }
        }

        public Expression Expression
        {
            get; set;
        }

        public int Sort
        {
            get
            {
                return 480;
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
            var result = "ORDER BY " + SubTools.GetMethodValue(this.Context, argExp, ResolveExpressType.FieldSingle)+" DESC";
            var selfParameterName = this.Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            result = result.Replace(selfParameterName, string.Empty);
            return result;
        }
    }
}
