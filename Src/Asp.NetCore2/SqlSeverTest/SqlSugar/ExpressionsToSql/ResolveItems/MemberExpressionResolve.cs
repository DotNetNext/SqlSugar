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
            var expression = base.Expression as MemberExpression;
            var childExpression = expression.Expression as MemberExpression;
            var memberName = expression.Member.Name;
            var childIsMember = childExpression != null;
            var isLeft = parameter.IsLeft;
            var isSetTempData = parameter.IsSetTempData;
            var isValue = memberName == "Value" && expression.Member.DeclaringType.Name == "Nullable`1";
            var isBool = expression.Type == UtilConstants.BoolType;
            var isValueBool = isValue && isBool && parameter.BaseExpression == null;
            var isLength = memberName == "Length" && childIsMember && childExpression.Type == UtilConstants.StringType;
            var isDateValue = memberName.IsIn(Enum.GetNames(typeof(DateType))) &&(childIsMember&&childExpression.Type == UtilConstants.DateType);
            var isLogicOperator = ExpressionTool.IsLogicOperator(baseParameter.OperatorValue) || baseParameter.OperatorValue.IsNullOrEmpty();
            var isHasValue = isLogicOperator && memberName == "HasValue" && expression.Expression != null && expression.NodeType == ExpressionType.MemberAccess;
            var isDateDate = memberName == "Date" && expression.Expression.Type == UtilConstants.DateType;
            var isMemberValue = expression.Expression != null && expression.Expression.NodeType != ExpressionType.Parameter && !isValueBool;
            var isSingle = parameter.Context.ResolveType == ResolveExpressType.WhereSingle;

            if (isLength)
            {
                ResolveLength(parameter, isLeft, expression);
            }
            else if (isHasValue)
            {
                ResolveHasValue(parameter, expression);
            }
            else if (isDateValue)
            {
                ResolveDateValue(parameter, isLeft, expression);
            }
            else if (isValueBool)
            {
                ResolveValueBool(parameter, baseParameter, expression, isLeft, isSingle);
            }
            else if (isValue)
            {
                ResolveValue(parameter, baseParameter, expression, isLeft, isSetTempData, isSingle);
            }
            else if (isDateDate)
            {
                ResolveDateDate(parameter, isLeft, expression);
            }
            else if (isMemberValue)
            {
                ResolveMemberValue(parameter, baseParameter, isLeft, isSetTempData, expression);
            }
            else
            {
                baseParameter.ChildExpression = expression;
                string fieldName = string.Empty;
                switch (parameter.Context.ResolveType)
                {
                    case ResolveExpressType.SelectSingle:
                        fieldName = GetSingleName(parameter, expression, isLeft);
                        if (isSetTempData)
                            baseParameter.CommonTempData = fieldName;
                        else
                            base.Context.Result.Append(fieldName);
                        break;
                    case ResolveExpressType.SelectMultiple:
                        fieldName = GetMultipleName(parameter, expression, isLeft);
                        if (isSetTempData)
                            baseParameter.CommonTempData = fieldName;
                        else
                            base.Context.Result.Append(fieldName);
                        break;
                    case ResolveExpressType.WhereSingle:
                    case ResolveExpressType.WhereMultiple:
                        ResolveWhereLogic(parameter, baseParameter, expression, isLeft, isSetTempData, isSingle);
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
        }

        #region Resolve Where
        private void ResolveWhereLogic(ExpressionParameter parameter, ExpressionParameter baseParameter, MemberExpression expression, bool? isLeft, bool isSetTempData, bool isSingle)
        {
            string fieldName = string.Empty;
            if (isSetTempData)
            {
                if (ExpressionTool.IsConstExpression(expression))
                {
                    var value = ExpressionTool.GetMemberValue(expression.Member, expression);
                    base.AppendValue(parameter, isLeft, value);
                }
                else
                {
                    fieldName = GetName(parameter, expression, null, isSingle);
                    baseParameter.CommonTempData = fieldName;
                }
            }
            else
            {
                if (ExpressionTool.IsConstExpression(expression))
                {
                    var value = ExpressionTool.GetMemberValue(expression.Member, expression);
                    base.AppendValue(parameter, isLeft, value);
                    return;
                }
                fieldName = GetName(parameter, expression, isLeft, isSingle);
                if (expression.Type == UtilConstants.BoolType && baseParameter.OperatorValue.IsNullOrEmpty())
                {
                    fieldName = this.Context.DbMehtods.EqualTrue(fieldName);
                }
                AppendMember(parameter, isLeft, fieldName);
            }
        }
        #endregion

        #region Resolve special member
        private MemberExpression ResolveValue(ExpressionParameter parameter, ExpressionParameter baseParameter, MemberExpression expression, bool? isLeft, bool isSetTempData, bool isSingle)
        {
            expression = expression.Expression as MemberExpression;
            baseParameter.ChildExpression = expression;
            ResolveWhereLogic(parameter, baseParameter, expression, isLeft, isSetTempData, isSingle);
            return expression;
        }

        private void ResolveValueBool(ExpressionParameter parameter, ExpressionParameter baseParameter, MemberExpression expression, bool? isLeft, bool isSingle)
        {
            string fieldName = GetName(parameter, expression.Expression as MemberExpression, isLeft, isSingle);
            if (expression.Type == UtilConstants.BoolType && baseParameter.OperatorValue.IsNullOrEmpty())
            {
                fieldName = this.Context.DbMehtods.EqualTrue(fieldName);
            }
            AppendMember(parameter, isLeft, fieldName);
        }

        private void ResolveMemberValue(ExpressionParameter parameter, ExpressionParameter baseParameter, bool? isLeft, bool isSetTempData, MemberExpression expression)
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
        }

        private void ResolveDateDate(ExpressionParameter parameter, bool? isLeft, MemberExpression expression)
        {
            var name = expression.Member.Name;
            var oldCommonTempDate = parameter.CommonTempData;
            parameter.CommonTempData = CommonTempDataType.Result;
            this.Expression = expression.Expression;
            this.Start();
            var isConst = parameter.CommonTempData.GetType() == UtilConstants.DateType;
            if (isConst)
            {
                AppendValue(parameter, isLeft, parameter.CommonTempData.ObjToDate().Date);
            }
            else
            {
                var GetYear = new MethodCallExpressionModel()
                {
                    Args = new List<MethodCallExpressionArgs>() {
                             new MethodCallExpressionArgs() {  IsMember=true, MemberName=parameter.CommonTempData, MemberValue=parameter.CommonTempData },
                             new MethodCallExpressionArgs() {   MemberName=DateType.Year, MemberValue=DateType.Year}
                         }
                };
                AppendMember(parameter, isLeft, GetToDate(this.Context.DbMehtods.MergeString(
                    this.GetDateValue(parameter.CommonTempData, DateType.Year),
                    "+'-'+",
                    this.GetDateValue(parameter.CommonTempData, DateType.Month),
                    "+'-'+",
                    this.GetDateValue(parameter.CommonTempData, DateType.Day))));
            }
            parameter.CommonTempData = oldCommonTempDate;
        }

        private void ResolveDateValue(ExpressionParameter parameter, bool? isLeft, MemberExpression expression)
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
        }

        private void ResolveHasValue(ExpressionParameter parameter, MemberExpression expression)
        {
            parameter.CommonTempData = CommonTempDataType.Result;
            this.Expression = expression.Expression;
            this.Start();
            var methodParamter = new MethodCallExpressionArgs() { IsMember = true, MemberName = parameter.CommonTempData, MemberValue = null };
            var result = this.Context.DbMehtods.HasValue(new MethodCallExpressionModel()
            {
                Args = new List<MethodCallExpressionArgs>() {
                    methodParamter
                  }
            });
            this.Context.Result.Append(result);
            parameter.CommonTempData = null;
        }

        private void ResolveLength(ExpressionParameter parameter, bool? isLeft, MemberExpression expression)
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
        }
        #endregion

        #region Helper
        private string AppendMember(ExpressionParameter parameter, bool? isLeft, string fieldName)
        {
            if (parameter.BaseExpression is BinaryExpression || (parameter.BaseParameter.CommonTempData != null && parameter.BaseParameter.CommonTempData.Equals(CommonTempDataType.Append)))
            {
                fieldName = string.Format(" {0} ", fieldName);
                if (isLeft == true)
                {
                    fieldName += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
                }
                if (base.Context.Result.Contains(ExpressionConst.FormatSymbol))
                {
                    base.Context.Result.Replace(ExpressionConst.FormatSymbol, fieldName);
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
            fieldName = Context.GetTranslationColumnName(shortName +UtilConstants.Dot+ fieldName);
            return fieldName;
        }

        private string GetSingleName(ExpressionParameter parameter, MemberExpression expression, bool? isLeft)
        {
            string fieldName = expression.Member.Name;
            fieldName = this.Context.GetDbColumnName(expression.Expression.Type.Name, fieldName);
            fieldName = Context.GetTranslationColumnName(fieldName);
            return fieldName;
        }

        private string GetDateValue(object value, DateType type)
        {
            var pars = new MethodCallExpressionModel()
            {
                Args = new List<MethodCallExpressionArgs>() {
                             new MethodCallExpressionArgs() {  IsMember=true, MemberName=value, MemberValue=value },
                             new MethodCallExpressionArgs() {   MemberName=type, MemberValue=type}
                         }
            };
            return this.Context.DbMehtods.DateValue(pars);
        }

        private string GetToDate(string value)
        {
            var pars = new MethodCallExpressionModel()
            {
                Args = new List<MethodCallExpressionArgs>() {
                             new MethodCallExpressionArgs() { MemberName=value, MemberValue=value },
                         }
            };
            return this.Context.DbMehtods.ToDate(pars);
        }
        #endregion
    }
}
