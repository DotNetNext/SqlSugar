﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class SubWhere : ISubOperation
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
            get; set;
        }

        public string GetValue(Expression expression)
        {
            var exp = expression as MethodCallExpression;
            if (Regex.Matches( expression.ToString(), "Subqueryable").Count >= 2)
            {
                new SubSelect() { Context = this.Context }.SetShortName(exp, "+");
            }
            var argExp = exp.Arguments[0];
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
                var value = GetValue(result, regex);
                if (value is Expression)
                {
                    var p = this.Context.Parameters.First(it => it.ParameterName == Regex.Match(result, regex).Groups[1].Value);
                    result = "WHERE " + SubTools.GetMethodValue(Context, value as Expression, ResolveExpressType.WhereMultiple);
                    argExp = value as Expression;
                    this.Context.Parameters.Remove(p);
                }
                else
                {
                    result = "WHERE " + value;
                    return result;
                }
            }

            var selfParameterName = Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            if (this.Context.JoinIndex == 0&&result.Contains(" FROM ")) 
            {
                this.Context.CurrentShortName= selfParameterName.ObjToString().TrimEnd('.');
            }
            else if (this.Context.JoinIndex == 0)
                result = result.Replace(selfParameterName, SubTools.GetSubReplace(this.Context));
            if (!string.IsNullOrEmpty(selfParameterName) && this.Context.IsSingle&& this.Context.JoinIndex == 0) 
            {
                this.Context.CurrentShortName = selfParameterName.TrimEnd('.');
            }
            return result;
        }

        private object GetValue(string result, string regex)
        {
            return this.Context.Parameters.First(it => it.ParameterName == Regex.Match(result, regex).Groups[1].Value).Value;
        }
    }
}
