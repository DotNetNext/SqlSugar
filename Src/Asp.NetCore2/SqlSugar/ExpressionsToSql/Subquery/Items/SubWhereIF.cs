using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class SubWhereIF : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get { return "WhereIF"; }
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
            get; set;
        }

        public string GetValue(Expression expression)
        {
            var exp = expression as MethodCallExpression;
            object value = null;
            try
            {
                value = ExpressionTool.DynamicInvoke(exp.Arguments[0]);
            }
            catch 
            {
                Check.Exception(true, ErrorMessage.WhereIFCheck,exp.Arguments[0].ToString());
            }
            if (Regex.Matches(expression.ToString(), "Subqueryable").Count >= 2)
            {
                new SubSelect() { Context = this.Context }.SetShortNameNext(exp, "+");
            }
            var isWhere= Convert.ToBoolean(value);
            if (!Convert.ToBoolean(isWhere)) {
                return "WHERE 1=1 ";
            }
            var argExp = exp.Arguments[1];
            var copyContext = this.Context;
            if (this.Context.JoinIndex > 0)
            {
                copyContext = this.Context.GetCopyContextWithMapping();
                copyContext.IsSingle = false;
            }
            var result = "WHERE " + SubTools.GetMethodValue(copyContext, argExp, ResolveExpressType.WhereMultiple);
            if (this.Context.JoinIndex > 0)
            {
                this.Context.Parameters.AddRange(copyContext.Parameters);
                this.Context.Index = copyContext.Index;
                this.Context.ParameterIndex = copyContext.ParameterIndex;
            }
            var selfParameterName = Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            if (this.Context.JoinIndex == 0)
                result = result.Replace(selfParameterName, SubTools.GetSubReplace(this.Context));
            if (!string.IsNullOrEmpty(selfParameterName) && this.Context.IsSingle && this.Context.JoinIndex == 0)
            {
                this.Context.CurrentShortName = selfParameterName.TrimEnd('.');
            }
            return result;
        }
    }
}
