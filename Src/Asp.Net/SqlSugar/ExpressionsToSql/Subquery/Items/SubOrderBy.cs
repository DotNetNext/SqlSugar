using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubOrderBy : ISubOperation
    {
        public int OrderIndex { get; set; } = 0;
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
                return 480+OrderIndex;
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
            var result = "";
            if (this.Context.JoinIndex == 0)
            {
                result = (OrderIndex == 0 ? "ORDER BY " : ",") + SubTools.GetMethodValue(this.Context, argExp, ResolveExpressType.FieldSingle);
            }
            else 
            {
                result = (OrderIndex == 0 ? "ORDER BY " : ",") + SubTools.GetMethodValueSubJoin(this.Context, argExp, ResolveExpressType.FieldMultiple);
            }
            var selfParameterName = this.Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            if (this.Context.JoinIndex == 0)
            {
                result = result.Replace(selfParameterName, SubTools.GetSubReplace(this.Context));
            }
            return result;
        }
    }
    public class SubOrderByDesc : ISubOperation
    {
        public int OrderIndex { get; set; } = 0;
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
                return 480+OrderIndex;
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
            var result = "";
            if (this.Context.JoinIndex == 0)
            {
                result = (OrderIndex == 0 ? "ORDER BY " : ",") + SubTools.GetMethodValue(this.Context, argExp, ResolveExpressType.FieldSingle) + " DESC";
            }
            else
            {
                result = (OrderIndex == 0 ? "ORDER BY " : ",") + SubTools.GetMethodValueSubJoin(this.Context, argExp, ResolveExpressType.FieldMultiple) + " DESC";
            }
            var selfParameterName = this.Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            if (this.Context.JoinIndex == 0)
            {
                result = result.Replace(selfParameterName, string.Empty);
            }
            return result;
        }
    }
}
