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
    public class MethodCallExpressionResolve : BaseResolve
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
                this.Context.Result.Append(this.Context.DbMehtods.GuidNew());
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

        private bool IsValidNativeMethod(MethodCallExpression express, string methodName)
        {
            return MethodMapping.ContainsKey(methodName) && express.Method.DeclaringType.Namespace == ("System");
        }

        private bool IsExtMethod(string methodName)
        {
            if (this.Context.SqlFuncServices == null) return false;
            return this.Context.SqlFuncServices.Select(it => it.UniqueMethodName).Contains(methodName);
        }

        private bool IsIfElse(MethodCallExpression express, string methodName)
        {
            if (methodName == "End" && express.Object.Type == typeof(CaseWhen))
                return true;
            else
                return false;
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

        private void CusMethod(ExpressionParameter parameter, MethodCallExpression express, bool? isLeft)
        {
            try
            {
                var constValue = ExpressionTool.DynamicInvoke(express);
                if (constValue is MapperSql)
                {
                    constValue = (constValue as MapperSql).Sql;
                    base.AppendValue(parameter, isLeft, constValue);
                    return;
                }
                parameter.BaseParameter.CommonTempData = constValue;
                var parameterName = base.AppendParameter(constValue);
                if (parameter.BaseParameter.CommonTempData != null && parameter.BaseParameter.CommonTempData.Equals(CommonTempDataType.Result))
                {
                    this.Context.Result.Append(parameterName);
                }
                else
                {
                    base.AppendValue(parameter, isLeft, parameterName);
                }
            }
            catch
            {
                Check.Exception(true, string.Format(ErrorMessage.MethodError, express.Method.Name));
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
                default:
                    break;
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
            if (name == "GetSelfAndAutoFill")
            {
                var memberValue = (args.First() as MemberExpression).Expression.ToString();
                model.Args.Add(new MethodCallExpressionArgs() { MemberValue = memberValue, IsMember = true, MemberName = memberValue });
            }
            else
            {
                foreach (var item in args)
                {
                    AppendItem(parameter, name, args, model, item);
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
                methodValue = methodValue + "=1 ";
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

        private object packIfElse(object methodValue)
        {
            methodValue = this.Context.DbMehtods.CaseWhen(new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("IF",methodValue.ObjToString()),
                    new KeyValuePair<string, string>("Return","1"),
                    new KeyValuePair<string, string>("End","0")
                 });
            return methodValue;
        }

        private void AppendItem(ExpressionParameter parameter, string name, IEnumerable<Expression> args, MethodCallExpressionModel model, Expression item)
        {
            if (ExpressionTool.IsUnConvertExpress(item))
            {
                item = (item as UnaryExpression).Operand;
            }
            var isBinaryExpression = item is BinaryExpression || item is MethodCallExpression;
            var isConst = item is ConstantExpression;
            var isIIF = name == "IIF";
            var isIFFBoolMember = isIIF && (item is MemberExpression) && (item as MemberExpression).Type == UtilConstants.BoolType;
            var isIFFUnary = isIIF && (item is UnaryExpression) && (item as UnaryExpression).Operand.Type == UtilConstants.BoolType;
            var isIFFBoolBinary = isIIF && (item is BinaryExpression) && (item as BinaryExpression).Type == UtilConstants.BoolType;
            var isIFFBoolMethod = isIIF && (item is MethodCallExpression) && (item as MethodCallExpression).Type == UtilConstants.BoolType;
            var isFirst = item == args.First();
            if (isFirst && isIIF && isConst)
            {
                var value = (item as ConstantExpression).Value.ObjToBool() ? this.Context.DbMehtods.True() : this.Context.DbMehtods.False();
                var methodCallExpressionArgs = new MethodCallExpressionArgs()
                {
                    IsMember = true,
                    MemberName = value,
                    MemberValue = value
                };
                model.Args.Add(methodCallExpressionArgs);
            }
            else if (isIFFUnary && !isFirst)
            {
                AppendModelByIIFMember(parameter, model, (item as UnaryExpression).Operand);
            }
            else if (isIFFBoolMember && !isFirst)
            {
                AppendModelByIIFMember(parameter, model, item);

            }
            else if (isIFFBoolBinary && !isFirst)
            {
                AppendModelByIIFBinary(parameter, model, item);

            }
            else if (isIFFBoolMethod && !isFirst)
            {
                AppendModelByIIFMethod(parameter, model, item);
            }
            else if (isBinaryExpression)
            {
                model.Args.Add(GetMethodCallArgs(parameter, item));
            }
            else
            {
                AppendModel(parameter, model, item);
            }
        }


        private void AppendModelByIIFMember(ExpressionParameter parameter, MethodCallExpressionModel model, Expression item)
        {
            parameter.CommonTempData = CommonTempDataType.Result;
            base.Expression = item;
            base.Start();
            var methodCallExpressionArgs = new MethodCallExpressionArgs()
            {
                IsMember = parameter.ChildExpression is MemberExpression,
                MemberName = parameter.CommonTempData
            };
            if (methodCallExpressionArgs.IsMember && parameter.ChildExpression != null && parameter.ChildExpression.ToString() == "DateTime.Now")
            {
                methodCallExpressionArgs.IsMember = false;
            }
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
                var parameterName = this.Context.SqlParameterKeyWord + ExpressionConst.MethodConst + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                methodCallExpressionArgs.MemberName = parameterName;
                methodCallExpressionArgs.MemberValue = value;
                this.Context.Parameters.Add(new SugarParameter(parameterName, value));
            }
            model.Args.Add(methodCallExpressionArgs);
            parameter.ChildExpression = null;
        }
        private void AppendModelByIIFBinary(ExpressionParameter parameter, MethodCallExpressionModel model, Expression item)
        {
            Check.Exception(true, "The SqlFunc.IIF(arg1,arg2,arg3) , {0} argument  do not support ", item.ToString());
        }
        private void AppendModelByIIFMethod(ExpressionParameter parameter, MethodCallExpressionModel model, Expression item)
        {
            var methodExpression = item as MethodCallExpression;
            if (methodExpression.Method.Name.IsIn("ToBool", "ToBoolean", "IIF"))
            {
                model.Args.Add(base.GetMethodCallArgs(parameter, item));
            }
            else
            {
                Check.Exception(true, "The SqlFunc.IIF(arg1,arg2,arg3) , {0} argument  do not support ", item.ToString());
            }
        }
        private void AppendModel(ExpressionParameter parameter, MethodCallExpressionModel model, Expression item)
        {
            parameter.CommonTempData = CommonTempDataType.Result;
            base.Expression = item;
            if (item.Type == UtilConstants.DateType && parameter.CommonTempData.ObjToString() == CommonTempDataType.Result.ToString() && item.ToString() == "DateTime.Now.Date")
            {
                parameter.CommonTempData = DateTime.Now.Date;
            }
            else if (IsDateDate(item))
            {
                parameter.CommonTempData = GetNewExpressionValue(item);
            }
            else if (IsDateValue(item))
            {
                parameter.CommonTempData = GetNewExpressionValue(item);
            }
            else if (model.Name == "ToString" && item is ConstantExpression && (item as ConstantExpression).Type.IsEnum())
            {
                parameter.CommonTempData = item.ToString();
            }
            else
            {
                base.Start();
            }
            var methodCallExpressionArgs = new MethodCallExpressionArgs()
            {
                IsMember = parameter.ChildExpression is MemberExpression && !ExpressionTool.IsConstExpression(parameter.ChildExpression as MemberExpression),
                MemberName = parameter.CommonTempData
            };
            if (methodCallExpressionArgs.IsMember && parameter.ChildExpression != null && parameter.ChildExpression.ToString() == "DateTime.Now")
            {
                methodCallExpressionArgs.IsMember = false;
            }
            var value = methodCallExpressionArgs.MemberName;
            if (methodCallExpressionArgs.IsMember)
            {
                var childExpression = parameter.ChildExpression as MemberExpression;
                if (childExpression.Expression != null && childExpression.Expression is ConstantExpression)
                {
                    methodCallExpressionArgs.IsMember = false;
                }
            }
            if (IsDateDate(item) || IsDateValue(item))
            {
                methodCallExpressionArgs.IsMember = true;
            }
            if (methodCallExpressionArgs.IsMember == false && (item is MethodCallExpression && item.ToString() == "GetDate()") || (item is UnaryExpression && ((UnaryExpression)item).Operand.ToString() == "GetDate()"))
            {
                var parameterName = this.Context.SqlParameterKeyWord + ExpressionConst.MethodConst + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                methodCallExpressionArgs.MemberName = value;
                methodCallExpressionArgs.MemberValue = null;
            }
            else if (methodCallExpressionArgs.IsMember == false)
            {
                var parameterName = this.Context.SqlParameterKeyWord + ExpressionConst.MethodConst + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                methodCallExpressionArgs.MemberName = parameterName;
                methodCallExpressionArgs.MemberValue = value;
                this.Context.Parameters.Add(new SugarParameter(parameterName, value));
            }
            model.Args.Add(methodCallExpressionArgs);
            parameter.ChildExpression = null;
        }

        private static bool IsDateDate(Expression item)
        {
            return item.Type == UtilConstants.DateType && item is MemberExpression && (item as MemberExpression).Member.Name == "Date" && item.ToString() != "DateTime.Now.Date";
        }
        private static bool IsDateValue(Expression item)
        {
            return item.Type == UtilConstants.IntType &&
                                    item is MemberExpression &&
                                    (item as MemberExpression).Expression != null &&
                                    (item as MemberExpression).Expression.Type == UtilConstants.DateType &&
                                    (item as MemberExpression).Expression is MemberExpression &&
                                    ((item as MemberExpression).Expression as MemberExpression).Member.Name == "Value";
        }

        private object GetMethodValue(string name, MethodCallExpressionModel model)
        {
            if (IsExtMethod(name))
            {
                model.Expression = this.Expression;
                model.BaseExpression = this.BaseParameter.CurrentExpression;
                DbType type = DbType.SqlServer;
                if (this.Context is SqlServerExpressionContext)
                    type = DbType.SqlServer;
                else if (this.Context is MySqlExpressionContext)
                    type = DbType.MySql;
                else if (this.Context is SqliteExpressionContext)
                    type = DbType.Sqlite;
                else if (this.Context is OracleExpressionContext)
                    type = DbType.Oracle;
                return this.Context.SqlFuncServices.First(it => it.UniqueMethodName == name).MethodValue(model, type, this.Context);
            }
            else
            {
                if (name == "Parse" && TempParseType.IsIn(UtilConstants.GuidType) && model.Args != null && model.Args.Count() > 1)
                {
                    name = "Equals";
                }
                else if (name == "Parse")
                {
                    name = "To" + TempParseType.Name;
                }
                else if (name == "IsNullOrWhiteSpace")
                {
                    name = "IsNullOrEmpty";
                }
                switch (name)
                {
                    case "IIF":
                        return this.Context.DbMehtods.IIF(model);
                    case "HasNumber":
                        return this.Context.DbMehtods.HasNumber(model);
                    case "HasValue":
                        return this.Context.DbMehtods.HasValue(model);
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
                        if (model.Args[0].MemberValue == null)
                        {
                            var first = this.Context.Parameters.FirstOrDefault(it => it.ParameterName == model.Args[0].MemberName.ObjToString());
                            if (first.HasValue())
                            {
                                model.Args[0].MemberValue = first.Value;
                            }
                        }
                        var caResult = this.Context.DbMehtods.ContainsArray(model);
                        this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[0].MemberName.ObjToString());
                        return caResult;
                    case "ContainsArrayUseSqlParameters":
                        if (model.Args[0].MemberValue == null)
                        {
                            var first = this.Context.Parameters.FirstOrDefault(it => it.ParameterName == model.Args[0].MemberName.ObjToString());
                            if (first.HasValue())
                            {
                                model.Args[0].MemberValue = first.Value;
                            }
                        }
                        model.Data = this.Context.SqlParameterKeyWord + "INP_" + this.Context.ParameterIndex;
                        this.Context.ParameterIndex++;
                        if (model.Args[0].MemberValue.HasValue())
                        {
                            var inValueIEnumerable = (IEnumerable)model.Args[0].MemberValue;
                            int i = 0;
                            foreach (var item in inValueIEnumerable)
                            {
                                this.Context.Parameters.Add(new SugarParameter(model.Data + "_" + i, item));
                                i++;
                            }
                        }
                        var caResult2 = this.Context.DbMehtods.ContainsArrayUseSqlParameters(model);
                        this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[0].MemberName.ObjToString());
                        return caResult2;
                    case "Equals":
                        return this.Context.DbMehtods.Equals(model);
                    case "EqualsNull":
                        return this.Context.DbMehtods.EqualsNull(model);
                    case "DateIsSame":
                        if (model.Args.Count == 2)
                            return this.Context.DbMehtods.DateIsSameDay(model);
                        else
                        {
                            var dsResult = this.Context.DbMehtods.DateIsSameByType(model);
                            this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[2].MemberName.ObjToString());
                            return dsResult;
                        }
                    case "DateAdd":
                        if (model.Args.Count == 2)
                            return this.Context.DbMehtods.DateAddDay(model);
                        else
                        {
                            var daResult = this.Context.DbMehtods.DateAddByType(model);
                            this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[2].MemberName.ObjToString());
                            return daResult;
                        }
                    case "DateValue":
                        var dvResult = this.Context.DbMehtods.DateValue(model);
                        this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[1].MemberName.ObjToString());
                        return dvResult;
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
                    case "ToDateTime":
                        return this.Context.DbMehtods.ToDate(model);
                    case "ToTime":
                        return this.Context.DbMehtods.ToTime(model);
                    case "ToString":
                        if (model.Args.Count > 1 && model.Args.Last().MemberValue.ObjToString().IsContainsIn("-", "/", ":", "yy", "ms", "hh"))
                        {
                            return GeDateFormat(model.Args.Last().MemberValue.ObjToString(), model.Args.First().MemberName.ObjToString());
                        }
                        Check.Exception(model.Args.Count > 1, "ToString (Format) is not supported, Use ToString().If time formatting can be used it.Date.Year+\"-\"+it.Data.Month+\"-\"+it.Date.Day ");
                        return this.Context.DbMehtods.ToString(model);
                    case "ToVarchar":
                        return this.Context.DbMehtods.ToVarchar(model);
                    case "ToDecimal":
                        return this.Context.DbMehtods.ToDecimal(model);
                    case "ToGuid":
                        return this.Context.DbMehtods.ToGuid(model);
                    case "ToDouble":
                        return this.Context.DbMehtods.ToDouble(model);
                    case "ToBool":
                        return this.Context.DbMehtods.ToBool(model);
                    case "ToBoolean":
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
                    case "AggregateDistinctCount":
                        return this.Context.DbMehtods.AggregateDistinctCount(model);
                    case "MappingColumn":
                        var mappingColumnResult = this.Context.DbMehtods.MappingColumn(model);
                        var isValid = model.Args[0].IsMember && model.Args[1].IsMember == false;
                        //Check.Exception(!isValid, "SqlFunc.MappingColumn parameters error, The property name on the left, string value on the right");
                        this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[1].MemberName.ObjToString());
                        return mappingColumnResult;
                    case "IsNull":
                        return this.Context.DbMehtods.IsNull(model);
                    case "MergeString":
                        return this.Context.DbMehtods.MergeString(model.Args.Select(it => it.MemberName.ObjToString()).ToArray());
                    case "GetSelfAndAutoFill":
                        this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[0].MemberName.ObjToString());
                        return this.Context.DbMehtods.GetSelfAndAutoFill(model.Args[0].MemberValue.ObjToString(), this.Context.IsSingle);
                    case "GetDate":
                        return this.Context.DbMehtods.GetDate();
                    case "GetRandom":
                        return this.Context.DbMehtods.GetRandom();
                    case "CharIndex":
                        return this.Context.DbMehtods.CharIndex(model);
                    case "BitwiseAnd":
                        return this.Context.DbMehtods.BitwiseAnd(model);
                    case "BitwiseInclusiveOR":
                        return this.Context.DbMehtods.BitwiseInclusiveOR(model);
                    case "ToDateShort":
                        return this.Context.DbMehtods.ToDateShort(model);
                    case "Oracle_ToChar":
                        return this.Context.DbMehtods.Oracle_ToChar(model);
                    case "Oracle_ToDate":
                        return this.Context.DbMehtods.Oracle_ToDate(model);
                    case "SqlServer_DateDiff":
                        return this.Context.DbMehtods.SqlServer_DateDiff(model);
                    default:
                        break;
                }
            }
            return null;
        }

        private bool IsContainsArray(MethodCallExpression express, string methodName, bool isValidNativeMethod)
        {
            return !isValidNativeMethod && express.Method.DeclaringType.Namespace.IsIn("System.Linq", "System.Collections.Generic") && methodName == "Contains";
        }

        private bool IsSubMethod(MethodCallExpression express, string methodName)
        {
            return SubTools.SubItemsConst.Any(it => it.Name == methodName) && express.Object != null && express.Object.Type.Name == "Subqueryable`1";
        }
        private bool CheckMethod(MethodCallExpression expression)
        {
            if (IsExtMethod(expression.Method.Name))
                return true;
            if (IsParseMethod(expression))
                return true;
            if (expression.Method.Name == "IsNullOrEmpty" && expression.Method.DeclaringType == UtilConstants.StringType)
            {
                return true;
            }
            if (expression.Method.Name == "IsNullOrWhiteSpace" && expression.Method.DeclaringType == UtilConstants.StringType)
            {
                return true;
            }
            if (expression.Method.ReflectedType().FullName != ExpressionConst.SqlFuncFullName)
                return false;
            else
                return true;
        }
        private Type TempParseType;
        public bool IsParseMethod(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Parse" && expression.Method.DeclaringType.IsIn(
                                                                                          UtilConstants.DecType,
                                                                                          UtilConstants.DateType,
                                                                                          UtilConstants.DobType,
                                                                                          UtilConstants.GuidType,
                                                                                          UtilConstants.FloatType,
                                                                                          UtilConstants.ShortType,
                                                                                          UtilConstants.LongType,
                                                                                          UtilConstants.IntType,
                                                                                          UtilConstants.BoolType))
            {
                TempParseType = expression.Method.DeclaringType;
                return true;
            }
            return false;
        }


        public string GeDateFormat(string formatString, string value)
        {
            var parameter = new MethodCallExpressionArgs() { IsMember = true, MemberValue = DateType.Year };
            var parameter2 = new MethodCallExpressionArgs() { IsMember = true, MemberName = value };
            var parameters = new MethodCallExpressionModel() { Args = new List<MethodCallExpressionArgs>() { parameter2, parameter } };
            var begin = @"^";
            var end = @"$";
            formatString = formatString.Replace("yyyy", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            formatString = formatString.Replace("yy", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);

            parameters.Args.Last().MemberValue = DateType.Month;
            formatString = formatString.Replace("MM", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            formatString = formatString.Replace("M", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);

            parameters.Args.Last().MemberValue = DateType.Day;
            formatString = formatString.Replace("dd", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            formatString = formatString.Replace("d", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);

            parameters.Args.Last().MemberValue = DateType.Hour;
            formatString = Regex.Replace(formatString, "hh", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end, RegexOptions.IgnoreCase);
            formatString = Regex.Replace(formatString, "h", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end, RegexOptions.IgnoreCase);

            parameters.Args.Last().MemberValue = DateType.Minute;
            formatString = formatString.Replace("mm", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            formatString = formatString.Replace("m", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);

            parameters.Args.Last().MemberValue = DateType.Second;
            formatString = formatString.Replace("ss", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            formatString = formatString.Replace("s", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);

            parameters.Args.Last().MemberValue = DateType.Millisecond;
            formatString = formatString.Replace("ms", begin + UtilMethods.ConvertStringToNumbers(this.GetMethodValue("DateValue", parameters).ObjToString()) + end);
            var items = Regex.Matches(formatString, @"\^\d+\$").Cast<Match>().ToList();
            foreach (var item in items)
            {
                formatString = formatString.Replace(item.Value, "$@" + UtilMethods.ConvertNumbersToString(item.Value.TrimStart('^').TrimEnd('$')) + "$");
            }
            var strings = formatString.TrimStart('$').TrimEnd('$').Split('$');
            var joinStringParameter = new MethodCallExpressionModel()
            {
                Args = new List<MethodCallExpressionArgs>()
            };
            foreach (var r in strings)
            {
                if (r.Substring(0, 1) == "@")
                {
                    joinStringParameter.Args.Add(new MethodCallExpressionArgs()
                    {
                        MemberName = r.TrimStart('@')
                    });
                }
                else
                {

                    var name = base.AppendParameter(r);
                    joinStringParameter.Args.Add(new MethodCallExpressionArgs()
                    {
                        MemberName = name
                    });
                }
            }
            return this.GetMethodValue("MergeString", joinStringParameter).ObjToString();
        }
    }
}
