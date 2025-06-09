﻿using System;
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
            SetSingleTableNameSubqueryShortName(express);
            if (ExpressionTool.GetParameters(express.Test).Count == 0)
            {
                while (express != null)
                {
                    var ps = ExpressionTool.GetParameters(express.Test);
                    if (ps?.Count == 0)
                    {
                        var value = ExpressionTool.DynamicInvoke(express.Test);
                        if (value is bool boolValue)
                        {
                            args[0] = boolValue ? UtilConstants.ExpTrue :UtilConstants.ExpFalse;
                            // 根据结果选择分支
                            var next = boolValue ? express.IfTrue : express.IfFalse;
                            args[1] = next;
                            args[2] = next;

                            // 如果选择的分支还是一个条件表达式，就继续展开
                            if (ExpressionTool.RemoveConvert(next) is ConditionalExpression childConditional)
                            {
                                express = childConditional;
                                args = new List<Expression>() {
                                        express.Test,
                                        express.IfTrue,
                                        express.IfFalse
                                    };
                                continue;
                            }
                            else
                            {
                                break; // 到底了，不是条件表达式，跳出循环
                            }
                        }
                        else
                        {
                            break; // 不是bool，无法判断，退出
                        }
                    }
                    else
                    {
                        break; // 有参数，不能动态执行，退出
                    }
                }
            }
            if (IsBoolMember(express))
            {
                Expression trueValue = Expression.Constant(true);
                args[0]= ExpressionBuilderHelper.CreateExpression(express.Test, trueValue, ExpressionType.Equal);
            }
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

        private void SetSingleTableNameSubqueryShortName(ConditionalExpression express)
        {
            if (this.Context.IsSingle && express.Test is MethodCallExpression callExpression)
            {
                var list = callExpression.Arguments.ToList();
                list.Add(callExpression);
                if (express.IfTrue is MethodCallExpression callExpressionLeft) 
                {
                    list.Add(callExpressionLeft);
                    list.AddRange(callExpressionLeft.Arguments);
                }
                if (express.IfFalse is MethodCallExpression callExpressionRight)
                {
                    list.Add(callExpressionRight);
                    list.AddRange(callExpressionRight.Arguments);
                }
                foreach (var item in list)
                {
                    if (item is MethodCallExpression itemObj)
                    {
                        if (ExpressionTool.IsSubQuery(itemObj))
                        {
                            if (this.Context.SingleTableNameSubqueryShortName == null)
                            {
                                if (this.Context.SugarContext?.QueryBuilder?.SelectValue is LambdaExpression lambda)
                                {
                                    if (lambda?.Parameters?.Count == 1)
                                    {
                                        this.Context.SingleTableNameSubqueryShortName = lambda.Parameters.FirstOrDefault()?.Name;
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private static bool IsBoolMember(ConditionalExpression express)
        {
            return express.Test is MemberExpression && (express.Test as MemberExpression).Expression is ParameterExpression;
        }
    }
}
