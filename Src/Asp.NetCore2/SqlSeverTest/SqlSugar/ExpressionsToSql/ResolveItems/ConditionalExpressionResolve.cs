using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class ConditionalExpressionResolve:BaseResolve
    {
        public ConditionalExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var express = base.Expression as ConditionalExpression;
            var isLeft = parameter.IsLeft;
            switch (base.Context.ResolveType)
            {
                case ResolveExpressType.None:
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                case ResolveExpressType.Join:
                case ResolveExpressType.ArraySingle:
                case ResolveExpressType.ArrayMultiple:
                case ResolveExpressType.Update:
                default:
                    Check.Exception(true, "Does not support it.xx==value ? true:false , Use SqlFunc.IIF (it.xx==value,true,false)");
                    break;
            }
        }
    }
}
