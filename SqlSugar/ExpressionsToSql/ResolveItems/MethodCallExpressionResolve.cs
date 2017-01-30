using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class MethodCallExpressionResolve : BaseResolve
    {
        public MethodCallExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var express = base.Expression as MethodCallExpression;
            CheckMethod(express);
            var method = express.Method;
            string name = method.Name;
            var args = express.Arguments;
            MethodCallExpressionModel model = new MethodCallExpressionModel();
            model.Args = new List<MethodCallExpressionArgs>();
            switch (this.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                    foreach (var item in args)
                    {
                        base.Expression = item;
                        base.Start();
                        var methodCallExpressionArgs = new MethodCallExpressionArgs()
                        {
                            IsMember = parameter.ChildExpression is MemberExpression,
                            Value = parameter.CommonTempData
                        };
                        var value = methodCallExpressionArgs.Value;
                        if (methodCallExpressionArgs.IsMember)
                        {
                            var childExpression = parameter.ChildExpression as MemberExpression;
                            if (childExpression.Expression != null && childExpression.Expression is ConstantExpression)
                            {
                                methodCallExpressionArgs.IsMember = false;
                            }
                        }
                        if (methodCallExpressionArgs.IsMember == false)
                        {
                            var parameterName = this.Context.SqlParameterKeyWord + ExpressionConst.METHODCOST + this.Context.ParameterIndex;
                            this.Context.ParameterIndex++;
                            methodCallExpressionArgs.Value = parameterName;
                            this.Context.Parameters.Add(new SugarParameter(parameterName, value));
                        }
                        model.Args.Add(methodCallExpressionArgs);
                    }
                    var methodValue = GetMdthodValue(name, model);
                    base.Context.Result.Append(methodValue);
                    break;
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                default:
                    break;
            }
        }

        private object GetMdthodValue(string name, MethodCallExpressionModel model)
        {
            switch (name)
            {
                case "IsNullOrEmpty":
                    return this.Context.DbMehtods.IsNullOrEmpty(model);
                default:
                    break;
            }
            return null;
        }

        public void CheckMethod(MethodCallExpression expression)
        {
            Check.Exception(expression.Method.ReflectedType.FullName != ExpressionConst.NBORMFULLNAME, ExpressionErrorMessage.MethodError);
        }
    }
}
