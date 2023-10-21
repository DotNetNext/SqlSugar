using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public partial class MethodCallExpressionResolve : BaseResolve
    {
        int contextIndex = 0;

        public MethodCallExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            contextIndex = this.Context.Index;
            var express = base.Expression as MethodCallExpression;
            if (express == null) return;
            var isLeft = parameter.IsLeft;
            string methodName = express.Method.Name;
            var isValidNativeMethod = IsValidNativeMethod(express, methodName);
            List<MethodCallExpressionArgs> appendArgs = null;
            if (MethodTimeMapping.ContainsKey(methodName))
            {
                appendArgs = new List<MethodCallExpressionArgs>();
                var dateType = MethodTimeMapping[methodName];
                string paramterName = this.Context.SqlParameterKeyWord + ExpressionConst.Const + this.Context.ParameterIndex;
                appendArgs.Add(new MethodCallExpressionArgs() { IsMember = false, MemberName = paramterName, MemberValue = dateType });
                this.Context.Parameters.Add(new SugarParameter(paramterName, dateType.ToString()));
                this.Context.ParameterIndex++;
                methodName = "DateAdd";
                isValidNativeMethod = true;
            }
            else if (methodName == "get_Item")
            {
                string paramterName = this.Context.SqlParameterKeyWord + ExpressionConst.Const + this.Context.ParameterIndex;
                this.Context.Parameters.Add(new SugarParameter(paramterName, ExpressionTool.DynamicInvoke(express)));
                this.Context.Result.Append(string.Format(" {0} ", paramterName));
                this.Context.ParameterIndex++;
                return;
            }
            else if (methodName == "NewGuid")
            {
                this.Context.Result.Append(this.Context.DbMehtods.NewUid(null));
                return;
            }
            else if (methodName == "GetConfigValue") 
            {
                GetConfigValue(express,parameter);
                return;
            }
            else if (IsSubMethod(express, methodName))
            {
                //Check.Exception(!(parameter.BaseExpression is BinaryExpression), "Current expressions are not supported");
                SubResolve subResolve = new SubResolve(express, this.Context, parameter.OppsiteExpression);
                var appendSql = subResolve.GetSql();
                if (this.Context.ResolveType.IsIn(ResolveExpressType.SelectMultiple, ResolveExpressType.SelectSingle) || (parameter.BaseParameter != null && parameter.BaseParameter.CommonTempData != null && parameter.BaseParameter.CommonTempData.Equals(CommonTempDataType.Result)))
                {
                    parameter.BaseParameter.CommonTempData = appendSql;
                }
                else
                {
                    base.AppendValue(parameter, isLeft, appendSql);
                }
                return;
            }
            else if (IsIfElse(express, methodName))
            {
                CaseWhenResolve caseResole = new CaseWhenResolve(express, this.Context, parameter.OppsiteExpression);
                var appendSql = caseResole.GetSql();
                var isRoot = contextIndex == 2 && parameter.BaseExpression == null;
                if (isRoot || (parameter.BaseExpression != null && ExpressionTool.IsLogicOperator(parameter.BaseExpression)))
                {
                    appendSql = appendSql + "=1 ";
                }
                if (this.Context.ResolveType.IsIn(ResolveExpressType.SelectMultiple, ResolveExpressType.SelectSingle, ResolveExpressType.Update))
                {
                    parameter.BaseParameter.CommonTempData = appendSql;
                }
                else
                {
                    base.AppendValue(parameter, isLeft, appendSql);
                }
                return;
            }
            if (IsContainsArray(express, methodName, isValidNativeMethod))
            {
                methodName = "ContainsArray";
                isValidNativeMethod = true;
            }
            if (isValidNativeMethod)
            {
                NativeExtensionMethod(parameter, express, isLeft, MethodMapping[methodName], appendArgs);
            }
            else
            {
                SqlFuncMethod(parameter, express, isLeft);
            }
        }

        private void NativeExtensionMethod(ExpressionParameter parameter, MethodCallExpression express, bool? isLeft, string name, List<MethodCallExpressionArgs> appendArgs = null)
        {
            var method = express.Method;
            var args = express.Arguments.Cast<Expression>().ToList();
            MethodCallExpressionModel model = new MethodCallExpressionModel();
            model.Name = name;
            model.Args = new List<MethodCallExpressionArgs>();
            switch (this.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                    if (express.Object != null)
                        args.Insert(0, express.Object);
                    Where(parameter, isLeft, name, args, model, appendArgs);
                    break;
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.Update:
                    if (express.Object != null)
                        args.Insert(0, express.Object);
                    Select(parameter, isLeft, name, args, model, appendArgs);
                    break;
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                    if (express.Method.Name == "ToString" && express.Object != null && express.Object?.Type == UtilConstants.DateType)
                    {
                        var format = (args[0] as ConstantExpression)?.Value + "";
                        if (format == ""&& args[0] is MemberExpression) 
                        {
                            format = ExpressionTool.GetExpressionValue(args[0]) + "";
                        }
                        var value = GetNewExpressionValue(express.Object);
                        var dateString2 = this.Context.DbMehtods.GetDateString(value, format);
                        if (dateString2 == null)
                        {
                            var dateString = GeDateFormat(format, value);
                            base.AppendValue(parameter, isLeft, dateString);
                        }
                        else
                        {
                            base.AppendValue(parameter, isLeft, dateString2);
                        }
                    }
                    else
                    {
                        var value = GetNewExpressionValue(express, this.Context.IsJoin ? ResolveExpressType.WhereMultiple : ResolveExpressType.WhereSingle);
                        base.AppendValue(parameter, isLeft, value);
                    }
                    break;
                default:
                    break;
            }
        }
        protected void SqlFuncMethod(ExpressionParameter parameter, MethodCallExpression express, bool? isLeft)
        {
            if (!CheckMethod(express))
            {
                CusMethod(parameter, express, isLeft);
            }
            else
            {
                var method = express.Method;
                string name = method.Name;
                if (name == "Any" &&ExpressionTool.IsVariable(express.Arguments[0])) 
                {
                    name = "ListAny";
                }
                else if (name == "All" && ExpressionTool.IsVariable(express.Arguments[0]))
                {
                    name = "ListAll";
                }
                var args = express.Arguments.Cast<Expression>().ToList();
                MethodCallExpressionModel model = new MethodCallExpressionModel();
                model.Args = new List<MethodCallExpressionArgs>();
                switch (this.Context.ResolveType)
                {
                    case ResolveExpressType.WhereSingle:
                    case ResolveExpressType.WhereMultiple:
                        Check.Exception(name == "GetSelfAndAutoFill", "SqlFunc.GetSelfAndAutoFill can only be used in Select.");
                        Where(parameter, isLeft, name, args, model);
                        break;
                    case ResolveExpressType.SelectSingle:
                    case ResolveExpressType.SelectMultiple:
                    case ResolveExpressType.Update:
                        Select(parameter, isLeft, name, args, model);
                        break;
                    case ResolveExpressType.FieldSingle:
                    case ResolveExpressType.FieldMultiple:
                        Field(parameter, isLeft, name, args, model);
                        break;
                    default:
                        break;
                }
            }
        }


        protected void Field(ExpressionParameter parameter, bool? isLeft, string name, IEnumerable<Expression> args, MethodCallExpressionModel model, List<MethodCallExpressionArgs> appendArgs = null)
        {
            if (this.Context.ResolveType == ResolveExpressType.FieldSingle)
            {
                this.Context.ResolveType = ResolveExpressType.WhereSingle;
            }
            else
            {
                this.Context.ResolveType = ResolveExpressType.WhereMultiple;
            }
            Where(parameter, isLeft, name, args, model);
        }
        protected void Select(ExpressionParameter parameter, bool? isLeft, string name, IEnumerable<Expression> args, MethodCallExpressionModel model, List<MethodCallExpressionArgs> appendArgs = null)
        {
            if (name.IsIn("GetSelfAndAutoFill","SelectAll"))
            {
                var memberValue = (args.First() as MemberExpression)?.Expression?.ToString();
                if (memberValue == null&& args.First() is ParameterExpression) 
                {
                    memberValue = (args.First() as ParameterExpression).Type.GetProperties().First().Name;
                }
                var data = new MethodCallExpressionArgs() { MemberValue = memberValue, IsMember = true, MemberName = memberValue };
                model.Args.Add(data);
                if (args.Count() == 2) 
                {
                    data.MemberName = (args.Last()).ToString().Replace("\"","");
                    data.MemberValue = "." ;
                }
            }
            else
            {
                foreach (var item in args)
                {
                    if (name == "IIF" && item == args.First() && item is MemberExpression)
                    {
                        Expression trueValue = Expression.Constant(true);
                        var newItem = ExpressionBuilderHelper.CreateExpression(item, trueValue, ExpressionType.Equal);
                        var member = (item as MemberExpression);
                        if (member.Member.Name == "HasValue") 
                        {
                            newItem = ExpressionBuilderHelper.CreateExpression(member.Expression, Expression.Constant(null), ExpressionType.NotEqual);
                        }
                        AppendItem(parameter, name, new List<Expression>() { newItem}, model, newItem);
                    }
                    else
                    {
                        AppendItem(parameter, name, args, model, item);
                    }
                }
                if (appendArgs != null)
                {
                    model.Args.AddRange(appendArgs);
                }
            }
            if (parameter.BaseParameter.BaseParameter.BaseParameter == null)
            {
                this.Context.Result.Append(GetMethodValue(name, model));
            }
            else
            {
                parameter.BaseParameter.CommonTempData = GetMethodValue(name, model);
            }
        }
        protected void Where(ExpressionParameter parameter, bool? isLeft, string name, IEnumerable<Expression> args, MethodCallExpressionModel model, List<MethodCallExpressionArgs> appendArgs = null)
        {
            foreach (var item in args)
            {
                var expItem = item;
                if (item is UnaryExpression)
                {
                    expItem = (item as UnaryExpression).Operand;
                }
                AppendItem(parameter, name, args, model, expItem);
            }
            if (appendArgs != null)
            {
                model.Args.AddRange(appendArgs);
            }
            var methodValue = GetMethodValue(name, model);
            if (parameter.BaseExpression is BinaryExpression && parameter.OppsiteExpression.Type == UtilConstants.BoolType && name == "HasValue" && !(parameter.OppsiteExpression is BinaryExpression) && !(parameter.OppsiteExpression is MethodCallExpression && parameter.OppsiteExpression.Type == UtilConstants.BoolType))
            {
                methodValue = packIfElse(methodValue);
            }
            if (parameter.OppsiteExpression != null && name == "IsNullOrEmpty" && parameter.OppsiteExpression.Type == UtilConstants.BoolType && parameter.OppsiteExpression is ConstantExpression)
            {
                methodValue = packIfElse(methodValue);
            }
            var isRoot = contextIndex == 2;
            if (isRoot && parameter.BaseExpression == null && this.Context.ResolveType.IsIn(ResolveExpressType.WhereMultiple, ResolveExpressType.WhereSingle) && (parameter.CurrentExpression is MethodCallExpression) && ((parameter.CurrentExpression as MethodCallExpression).Method.Name.IsIn("ToBool", "ToBoolean")))
            {
                methodValue = methodValue + "=1 ";
                ;
            }
            if (isRoot && parameter.BaseExpression == null && this.Context.ResolveType.IsIn(ResolveExpressType.WhereMultiple, ResolveExpressType.WhereSingle) && (parameter.CurrentExpression is ConditionalExpression) && ((parameter.CurrentExpression as ConditionalExpression).Type == UtilConstants.BoolType))
            {
                var isContainsTrue = MethodValueIsTrue(methodValue);
                if (isContainsTrue)
                {
                    methodValue = methodValue + "=true ";
                }
                else
                {
                    methodValue = methodValue + "=1 ";
                }
            }
            if (isRoot && parameter.BaseExpression == null && this.Context.ResolveType.IsIn(ResolveExpressType.WhereMultiple, ResolveExpressType.WhereSingle) && (parameter.CurrentExpression is MethodCallExpression) && ((parameter.CurrentExpression as MethodCallExpression).Method.Name.IsIn("IIF")) && (parameter.CurrentExpression as MethodCallExpression).Method.ReturnType == UtilConstants.BoolType)
            {
                methodValue = methodValue + "=1 ";
            }
            if (parameter.BaseExpression != null && ExpressionTool.IsLogicOperator(parameter.BaseExpression) && this.Context.ResolveType.IsIn(ResolveExpressType.WhereMultiple, ResolveExpressType.WhereSingle) && (parameter.CurrentExpression is ConditionalExpression) && ((parameter.CurrentExpression as ConditionalExpression).Type == UtilConstants.BoolType))
            {
                methodValue = methodValue + "=1 ";
            }
            if (parameter.BaseExpression != null && ExpressionTool.IsLogicOperator(parameter.BaseExpression) && this.Context.ResolveType.IsIn(ResolveExpressType.WhereMultiple, ResolveExpressType.WhereSingle) && (parameter.CurrentExpression is MethodCallExpression) && ((parameter.CurrentExpression as MethodCallExpression).Method.Name.IsIn("IIF")) && (parameter.CurrentExpression as MethodCallExpression).Method.ReturnType == UtilConstants.BoolType)
            {
                methodValue = methodValue + "=1 ";
            }
            if (parameter.BaseExpression != null && ExpressionTool.IsLogicOperator(parameter.BaseExpression) && this.Context.ResolveType.IsIn(ResolveExpressType.WhereMultiple, ResolveExpressType.WhereSingle) && (parameter.CurrentExpression is MethodCallExpression) && ((parameter.CurrentExpression as MethodCallExpression).Method.Name.IsIn("ToBool", "ToBoolean")))
            {
                methodValue = methodValue + "=1 ";
            }
            base.AppendValue(parameter, isLeft, methodValue);
        }

    }
}
