using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class ConditionalExpressionResolve: MethodCallExpressionResolve
    {
        public  ConditionalExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {

            string name = "IIF";
            var express = base.Expression as ConditionalExpression;
            var args = new List<Expression>() {
                express.Test,
                express.IfTrue,
                express.IfFalse
            };
            var isLeft = parameter.IsLeft;
            MethodCallExpressionModel model = new MethodCallExpressionModel();
            model.Args = new List<MethodCallExpressionArgs>();
            switch (this.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                    Check.Exception(name == "GetSelfAndAutoFill", "SqlFunc.GetSelfAndAutoFill can only be used in Select.");
                    base.Where(parameter, isLeft, name, args, model);
                    break;
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.Update:
                    base.Select(parameter, isLeft, name, args, model);
                    break;
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                    base.Field(parameter, isLeft, name, args, model);
                    break;
                default:
                    break;
            }
        }
    }
}
