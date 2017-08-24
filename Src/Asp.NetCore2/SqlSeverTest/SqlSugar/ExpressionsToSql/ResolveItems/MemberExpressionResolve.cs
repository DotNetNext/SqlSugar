using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class MemberExpressionResolve : BaseResolve
    {
        public ExpressionParameter Parameter { get; set; }
        public MemberExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var baseParameter = parameter.BaseParameter;
            var isLeft = parameter.IsLeft;
            var isSetTempData = baseParameter.CommonTempData.IsValuable() && baseParameter.CommonTempData.Equals(CommonTempDataType.Result);
            var expression = base.Expression as MemberExpression;
            var isValue = expression.Member.Name == "Value" && expression.Member.DeclaringType.Name == "Nullable`1";
            var isBool = expression.Type == PubConst.BoolType;
            var isValueBool = isValue && isBool && parameter.BaseExpression == null;
            var isLength = expression.Member.Name == "Length" && (expression.Expression as MemberExpression).Type == PubConst.StringType;
            var isDateValue = expression.Member.Name.IsIn(Enum.GetNames(typeof(DateType))) && (expression.Expression as MemberExpression).Type == PubConst.DateType;
            var isLogicOperator = ExpressionTool.IsLogicOperator(baseParameter.OperatorValue) || baseParameter.OperatorValue.IsNullOrEmpty();
            var isHasValue = isLogicOperator && expression.Member.Name == "HasValue" && expression.Expression != null && expression.NodeType == ExpressionType.MemberAccess;
            if (isLength)
            {
                var oldCommonTempDate = parameter.CommonTempData;
                parameter.CommonTempData = CommonTempDataType.Result;
                this.Expression = expression.Expression;
                var isConst = this.Expression is ConstantExpression;
                this.Start();
                var methodParamter = new MethodCallExpressionArgs() { IsMember = !isConst, MemberName = parameter.CommonTempData, MemberValue = null };
                var result = this.Context.DbMehtods.Length(new MethodCallExpressionModel()
                {
                    Args = new List<MethodCallExpressionArgs>() {
                      methodParamter
                  }
                });
                base.AppendMember(parameter, isLeft, result);
                parameter.CommonTempData = oldCommonTempDate;
                return;
            }
            else if (isHasValue) {
                parameter.CommonTempData = CommonTempDataType.Result;
                this.Expression = expression.Expression;
                this.Start();
                var methodParamter =new MethodCallExpressionArgs() { IsMember = true, MemberName = parameter.CommonTempData, MemberValue = null };
                var result = this.Context.DbMehtods.HasValue(new MethodCallExpressionModel()
                {
                    Args = new List<MethodCallExpressionArgs>() {
                    methodParamter
                  }
                });
                this.Context.Result.Append(result);
                parameter.CommonTempData = null;
                return;
            }
            else if (isDateValue)
            {
                var name = expression.Member.Name;
                var oldCommonTempDate = parameter.CommonTempData;
                parameter.CommonTempData = CommonTempDataType.Result;
                this.Expression = expression.Expression;
                var isConst = this.Expression is ConstantExpression;
                this.Start();
                var result = this.Context.DbMehtods.DateValue(new MethodCallExpressionModel()
                {
                    Args = new List<MethodCallExpressionArgs>() {
                     new MethodCallExpressionArgs() { IsMember = !isConst, MemberName = parameter.CommonTempData, MemberValue = null },
                     new MethodCallExpressionArgs() { IsMember = true, MemberName = name, MemberValue = name }
                  }
                });
                base.AppendMember(parameter, isLeft, result);
                parameter.CommonTempData = oldCommonTempDate;
                return;
            }
            else if (isValueBool)
            {
                isValue = false;
            }
            else if (isValue)
            {
                expression = expression.Expression as MemberExpression;
            }
            else if (expression.Expression != null && expression.Expression.NodeType != ExpressionType.Parameter && !isValueBool)
            {
                var value = ExpressionTool.GetMemberValue(expression.Member, expression);
                if (isSetTempData)
                {
                    baseParameter.CommonTempData = value;
                }
                else
                {
                    AppendValue(parameter, isLeft, value);
                }
                return;
            }
            string fieldName = string.Empty;
            baseParameter.ChildExpression = expression;
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.SelectSingle:
                    fieldName = GetSingleName(parameter, expression, isLeft);
                    if (isSetTempData)
                    {
                        baseParameter.CommonTempData = fieldName;
                    }
                    else
                    {
                        base.Context.Result.Append(fieldName);
                    }
                    break;
                case ResolveExpressType.SelectMultiple:
                    fieldName = GetMultipleName(parameter, expression, isLeft);
                    if (isSetTempData)
                    {
                        baseParameter.CommonTempData = fieldName;
                    }
                    else
                    {
                        base.Context.Result.Append(fieldName);
                    }
                    break;
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                    var isSingle = parameter.Context.ResolveType == ResolveExpressType.WhereSingle;
                    if (isSetTempData)
                    {
                        fieldName = GetName(parameter, expression, null, isSingle);
                        baseParameter.CommonTempData = fieldName;
                    }
                    else
                    {
                        if (isValueBool)
                        {
                            fieldName = GetName(parameter, expression.Expression as MemberExpression, isLeft, isSingle);
                        }
                        else if (ExpressionTool.IsConstExpression(expression))
                        {
                            var value = ExpressionTool.GetMemberValue(expression.Member, expression);
                            base.AppendValue(parameter, isLeft, value);
                            return;
                        }
                        else
                        {
                            fieldName = GetName(parameter, expression, isLeft, isSingle);
                        }
                        if (expression.Type == PubConst.BoolType && baseParameter.OperatorValue.IsNullOrEmpty())
                        {
                            fieldName = "( " + fieldName + "=1 )";
                        }
                        fieldName = AppendMember(parameter, isLeft, fieldName);
                    }
                    break;
                case ResolveExpressType.FieldSingle:
                    fieldName = GetSingleName(parameter, expression, isLeft);
                    base.Context.Result.Append(fieldName);
                    break;
                case ResolveExpressType.FieldMultiple:
                    fieldName = GetMultipleName(parameter, expression, isLeft);
                    base.Context.Result.Append(fieldName);
                    break;
                case ResolveExpressType.ArrayMultiple:
                case ResolveExpressType.ArraySingle:
                    fieldName = GetName(parameter, expression, isLeft, parameter.Context.ResolveType == ResolveExpressType.ArraySingle);
                    base.Context.Result.Append(fieldName);
                    break;
                default:
                    break;
            }
        }

        private string AppendMember(ExpressionParameter parameter, bool? isLeft, string fieldName)
        {
            if (parameter.BaseExpression is BinaryExpression || (parameter.BaseParameter.CommonTempData != null && parameter.BaseParameter.CommonTempData.Equals(CommonTempDataType.Append)))
            {
                fieldName = string.Format(" {0} ", fieldName);
                if (isLeft == true)
                {
                    fieldName += ExpressionConst.Format1 + parameter.BaseParameter.Index;
                }
                if (base.Context.Result.Contains(ExpressionConst.Format0))
                {
                    base.Context.Result.Replace(ExpressionConst.Format0, fieldName);
                }
                else
                {
                    base.Context.Result.Append(fieldName);
                }
            }
            else
            {
                base.Context.Result.Append(fieldName);
            }

            return fieldName;
        }

        private string GetName(ExpressionParameter parameter, MemberExpression expression, bool? isLeft, bool isSingle)
        {
            if (isSingle)
            {
                return GetSingleName(parameter, expression, IsLeft);
            }
            else
            {
                return GetMultipleName(parameter, expression, IsLeft);
            }
        }

        private string GetMultipleName(ExpressionParameter parameter, MemberExpression expression, bool? isLeft)
        {
            string shortName = expression.Expression.ToString();
            string fieldName = expression.Member.Name;
            fieldName = this.Context.GetDbColumnName(expression.Expression.Type.Name, fieldName);
            fieldName = Context.GetTranslationColumnName(shortName + "." + fieldName);
            return fieldName;
        }

        private string GetSingleName(ExpressionParameter parameter, MemberExpression expression, bool? isLeft)
        {
            string fieldName = expression.Member.Name;
            fieldName = this.Context.GetDbColumnName(expression.Expression.Type.Name, fieldName);
            fieldName = Context.GetTranslationColumnName(fieldName);
            return fieldName;
        }
    }
}
