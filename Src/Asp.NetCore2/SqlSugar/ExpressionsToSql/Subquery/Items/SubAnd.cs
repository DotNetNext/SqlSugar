using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class SubAnd:ISubOperation
    {
        public string Name
        {
            get { return "And"; }
        }

        public Expression Expression
        {
            get; set;
        }

        public int Sort
        {
            get
            {
                return 401;
            }
        }

        public ExpressionContext Context
        {
            get;set;
        }

        public bool HasWhere
        {
            get; set;
        }

        public string GetValue(Expression expression)
        {
            var exp = expression as MethodCallExpression;
            var argExp = exp.Arguments[0];
            var copyContext = this.Context;

            var pars = ExpressionTool.GetParameters(expression).Distinct();
            if (this.Context.JoinIndex > 0|| pars.Count()>1) 
            {
                copyContext = this.Context.GetCopyContextWithMapping();
                copyContext.IsSingle = false;
            }
            if (ExpressionTool.GetMethodName(argExp) == "ToExpression")
            {
                argExp = ExpressionTool.DynamicInvoke(argExp) as Expression;
            }
            var result = "AND " + SubTools.GetMethodValue(copyContext, argExp, ResolveExpressType.WhereMultiple);
            if (argExp.Type == typeof(List<IConditionalModel>))
            {
                var p = this.Context.Parameters.Last();
                this.Context.Parameters.Remove(p);
                var cols = p.Value as List<IConditionalModel>;
                var sqlObj = this.Context.SugarContext.QueryBuilder.Builder.ConditionalModelToSql(cols, this.Context.ParameterIndex * 100);
                this.Context.ParameterIndex = this.Context.ParameterIndex + this.Context.ParameterIndex * 100;
                result = "AND " + sqlObj.Key;
                this.Context.Parameters.AddRange(sqlObj.Value);
                return result;
            }
            if (this.Context.JoinIndex > 0 || pars.Count() > 1)
            {
                this.Context.Parameters.AddRange(copyContext.Parameters);
                this.Context.Index = copyContext.Index;
                this.Context.ParameterIndex = copyContext.ParameterIndex;
            }

            var regex = @"^AND  (\@Const\d+) $";
            if (this.Context is OracleExpressionContext)
            {
                regex = @"^AND  (\:Const\d+) $";
            }
            if (this.Context is DmExpressionContext)
            {
                regex = @"^AND  (\:Const\d+) $";
            }
            if (Regex.IsMatch(result, regex))
            {
                var value = GetValue(result, regex);
                if (value is Expression)
                {
                    var p = this.Context.Parameters.First(it => it.ParameterName == Regex.Match(result, regex).Groups[1].Value);
                    result = "AND " + SubTools.GetMethodValue(Context, value as Expression, ResolveExpressType.WhereMultiple);
                    argExp = value as Expression;
                    this.Context.Parameters.Remove(p);
                }
                else
                {
                    result = "AND " + value;
                    return result;
                }
            }

            var selfParameterName = this.Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            if (this.Context.JoinIndex == 0 && result.Contains(" FROM "))
            {
                this.Context.CurrentShortName = selfParameterName.TrimEnd('.');
            }
            else  if (this.Context.JoinIndex == 0 && this.Context.CurrentShortName != selfParameterName.TrimEnd('.'))
                result = result.Replace(selfParameterName, SubTools.GetSubReplace(this.Context));
            return result;
        }

        private object GetValue(string result, string regex)
        {
            return this.Context.Parameters.First(it => it.ParameterName == Regex.Match(result, regex).Groups[1].Value).Value;
        }
    }
}
