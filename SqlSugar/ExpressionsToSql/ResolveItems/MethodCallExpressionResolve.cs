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
            var isLeft = parameter.IsLeft;
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
                    WhereMethod(parameter, isLeft, name, args, model);
                    break;
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                    SelectMethod(parameter, isLeft, name, args, model);
                    break;
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                default:
                    break;
            }
        }
        private void SelectMethod(ExpressionParameter parameter, bool? isLeft, string name, System.Collections.ObjectModel.ReadOnlyCollection<Expression> args, MethodCallExpressionModel model)
        {
            foreach (var item in args)
            {
                parameter.CommonTempData = CommonTempDataType.ChildNodeSet;
                base.Expression = item;
                base.Start();
                var methodCallExpressionArgs = new MethodCallExpressionArgs()
                {
                    IsMember = parameter.ChildExpression is MemberExpression,
                    MemberName = parameter.CommonTempData
                };
                var value = methodCallExpressionArgs.MemberName;
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
                    var parameterName = this.Context.SqlParameterKeyWord + ExpressionConst.METHODCONST + this.Context.ParameterIndex;
                    this.Context.ParameterIndex++;
                    methodCallExpressionArgs.MemberName = parameterName;
                    methodCallExpressionArgs.MemberValue = value;
                    this.Context.Parameters.Add(new SugarParameter(parameterName, value));
                }
                model.Args.Add(methodCallExpressionArgs);
            }
            parameter.BaseParameter.CommonTempData = GetMdthodValue(name, model);
        }
        private void WhereMethod(ExpressionParameter parameter, bool? isLeft, string name, System.Collections.ObjectModel.ReadOnlyCollection<Expression> args, MethodCallExpressionModel model)
        {
            foreach (var item in args)
            {
                parameter.CommonTempData = CommonTempDataType.ChildNodeSet;
                base.Expression = item;
                base.Start();
                var methodCallExpressionArgs = new MethodCallExpressionArgs()
                {
                    IsMember = parameter.ChildExpression is MemberExpression,
                    MemberName = parameter.CommonTempData
                };
                var value = methodCallExpressionArgs.MemberName;
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
                    var parameterName = this.Context.SqlParameterKeyWord + ExpressionConst.METHODCONST + this.Context.ParameterIndex;
                    this.Context.ParameterIndex++;
                    methodCallExpressionArgs.MemberName = parameterName;
                    methodCallExpressionArgs.MemberValue = value;
                    this.Context.Parameters.Add(new SugarParameter(parameterName, value));
                }
                model.Args.Add(methodCallExpressionArgs);
            }
            var methodValue = GetMdthodValue(name, model);
            base.AppendValue(parameter, isLeft, methodValue);
        }

        private object GetMdthodValue(string name, MethodCallExpressionModel model)
        {
            switch (name)
            {
                case "IsNullOrEmpty":
                    return this.Context.DbMehtods.IsNullOrEmpty(model);
                case "ToLower":
                    return this.Context.DbMehtods.ToLower(model);
                case "ToUpper":
                    return this.Context.DbMehtods.ToUpper(model);
                case "Trim":
                    return this.Context.DbMehtods.Trim(model);
                case "Contains":
                        return this.Context.DbMehtods.Contains(model);
                case "ContainsArray":
                        return this.Context.DbMehtods.ContainsArray(model);
                case "Equals":
                    return this.Context.DbMehtods.Equals(model);
                case "DateIsSame":
                    if (model.Args.Count == 2)
                        return this.Context.DbMehtods.DateIsSameDay(model);
                    else
                        return this.Context.DbMehtods.DateIsSameByType(model);
                case "DateAdd":
                    if (model.Args.Count == 2)
                        return this.Context.DbMehtods.DateAddDay(model);
                    else
                        return this.Context.DbMehtods.DateAddByType(model);
                case "DateValue":
                    return this.Context.DbMehtods.DateValue(model);
                case "Between":
                    return this.Context.DbMehtods.Between(model);
                case "StartsWith":
                    return this.Context.DbMehtods.StartsWith(model);
                case "EndsWith":
                    return this.Context.DbMehtods.EndsWith(model);
                case "ToInt32":
                     return this.Context.DbMehtods.ToInt32(model);
                case "ToInt64":
                    return this.Context.DbMehtods.ToInt64(model);
                case "ToDate":
                    return this.Context.DbMehtods.ToDate(model);
                case "ToString":
                    return this.Context.DbMehtods.ToString(model);
                case "ToDecimal":
                    return this.Context.DbMehtods.ToDecimal(model);
                case "ToGuid":
                    return this.Context.DbMehtods.ToGuid(model);
                case "ToDouble":
                    return this.Context.DbMehtods.ToDouble(model);
                case "ToBool":
                    return this.Context.DbMehtods.ToBool(model);
                case "Substring":
                    return this.Context.DbMehtods.Substring(model);
                case "Replace":
                    return this.Context.DbMehtods.Replace(model);
                case "Length":
                    return this.Context.DbMehtods.Length(model);
                case "AggregateSum":
                    return this.Context.DbMehtods.AggregateSum(model);
                case "AggregateAvg":
                    return this.Context.DbMehtods.AggregateAvg(model);
                case "AggregateMin":
                    return this.Context.DbMehtods.AggregateMin(model);
                case "AggregateMax":
                    return this.Context.DbMehtods.AggregateMax(model);
                case "AggregateCount":
                    return this.Context.DbMehtods.AggregateCount(model);
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
