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
            ExpressionParameter baseParameter;
            MemberExpression expression;
            bool? isLeft;
            bool isSetTempData, isValue, isValueBool, isLength, isDateValue, isHasValue, isDateDate, isMemberValue, isSingle, fieldIsBool, isSelectField, isField;
            SettingParameters(parameter, out baseParameter, out expression, out isLeft, out isSetTempData, out isValue, out isValueBool, out isLength, out isDateValue, out isHasValue, out isDateDate, out isMemberValue, out isSingle, out fieldIsBool, out isSelectField, out isField);
            baseParameter.ChildExpression = expression;
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
            else if (isValue && expression.Expression != null && expression.Expression is MethodCallExpression)
            {
                ResolveCallValue(parameter, baseParameter, expression, isLeft, isSetTempData, isSingle);
            }
            else if (isValue)
            {
                ResolveValue(parameter, baseParameter, expression, isLeft, isSetTempData, isSingle);
            }
            else if (expression.Expression != null && expression.Expression.Type == UtilConstants.DateType && expression is MemberExpression && expression.Expression is MethodCallExpression)
            {
                ResolveDateDateByCall(parameter, isLeft, expression);
            }
            else if (isDateDate)
            {
                ResolveDateDate(parameter, isLeft, expression);
            }
            else if (isMemberValue)
            {
                ResolveMemberValue(parameter, baseParameter, isLeft, isSetTempData, expression);
            }
            else if (fieldIsBool && !isField && !isSelectField)
            {
                ResolvefieldIsBool(parameter, baseParameter, isLeft, isSetTempData, expression, isSingle);
            }
            else
            {
                ResolveDefault(parameter, baseParameter, expression, isLeft, isSetTempData, isSingle);
            }
        }


        #region Resolve default
        private void ResolveDefault(ExpressionParameter parameter, ExpressionParameter baseParameter, MemberExpression expression, bool? isLeft, bool isSetTempData, bool isSingle)
        {
            string fieldName = string.Empty;
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.Update:
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

        #endregion

        #region Resolve Where
        private void ResolveBoolLogic(ExpressionParameter parameter, ExpressionParameter baseParameter, MemberExpression expression, bool? isLeft, bool isSetTempData, bool isSingle)
        {
            string fieldName = string.Empty;
            if (isSetTempData)
            {
                if (ExpressionTool.IsConstExpression(expression))
                {
                    var value = ExpressionTool.GetMemberValue(expression.Member, expression);
                    baseParameter.CommonTempData = value+"=1 ";
                }
                else
                {
                    fieldName = GetName(parameter, expression, null, isSingle);
                    baseParameter.CommonTempData = fieldName+"=1 ";
                }
            }
            else
            {
                if (ExpressionTool.IsConstExpression(expression))
                {
                    var value = ExpressionTool.GetMemberValue(expression.Member, expression);
                    base.AppendValue(parameter, isLeft, value+"=1 ");
                }
                else
                {
                    fieldName = GetName(parameter, expression, isLeft, isSingle);
                    AppendMember(parameter, isLeft, fieldName+"=1 ");
                }
            }
        }

        private void ResolveWhereLogic(ExpressionParameter parameter, ExpressionParameter baseParameter, MemberExpression expression, bool? isLeft, bool isSetTempData, bool isSingle)
        {
            string fieldName = string.Empty;
            if (isSetTempData)
            {
                if (ExpressionTool.IsConstExpression(expression))
                {
                    var value = ExpressionTool.GetMemberValue(expression.Member, expression);
                    baseParameter.CommonTempData = value;
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
                }
                else
                {
                    fieldName = GetName(parameter, expression, isLeft, isSingle);
                    AppendMember(parameter, isLeft, fieldName);
                }
            }
        }
        #endregion

        #region Resolve special member
        private void ResolveDateDateByCall(ExpressionParameter parameter, bool? isLeft, MemberExpression expression)
        {
            var value = GetNewExpressionValue(expression.Expression);
            if (expression.Member.Name == "Date")
            {
                AppendMember(parameter, isLeft, GetToDateShort(value));
            }
            else
            {
                foreach (int myCode in Enum.GetValues(typeof(DateType)))
                {
                    string strName = Enum.GetName(typeof(DateType), myCode);//获取名称
                    if (expression.Member.Name == strName)
                    {
                        AppendMember(parameter, isLeft, this.Context.DbMehtods.MergeString(this.GetDateValue(value, (DateType)(myCode))));
                    }
                }
            }
        }
        private void ResolveCallValue(ExpressionParameter parameter, ExpressionParameter baseParameter, MemberExpression expression, bool? isLeft, bool isSetTempData, bool isSingle)
        {
            try
            {
                baseParameter.ChildExpression = expression;
                string fieldName = string.Empty;
                if (isSetTempData)
                {
                    var value = ExpressionTool.DynamicInvoke(expression);
                    baseParameter.CommonTempData = value;
                }
                else
                {
                    var value = ExpressionTool.DynamicInvoke(expression);
                    base.AppendValue(parameter, isLeft, value);
                }
            }
            catch 
            {
                Check.Exception(true, "Not Support {0}",expression.ToString());
            }
        }

        private MemberExpression ResolveValue(ExpressionParameter parameter, ExpressionParameter baseParameter, MemberExpression expression, bool? isLeft, bool isSetTempData, bool isSingle)
        {
            expression = expression.Expression as MemberExpression;
            baseParameter.ChildExpression = expression;
            if (UtilMethods.GetUnderType(expression.Type) == UtilConstants.BoolType&&parameter.BaseExpression!=null&&ExpressionTool.IsLogicOperator(parameter.BaseExpression))
            {
                ResolveBoolLogic(parameter, baseParameter, expression, isLeft, isSetTempData, isSingle);
            }
            else
            {
                ResolveWhereLogic(parameter, baseParameter, expression, isLeft, isSetTempData, isSingle);
            }
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
                if (value is MapperSql)
                {
                    value = (value as MapperSql).Sql;
                }
                baseParameter.CommonTempData = value;
            }
            else
            {
                AppendValue(parameter, isLeft, value);
            }
        }
        private void ResolvefieldIsBool(ExpressionParameter parameter, ExpressionParameter baseParameter, bool? isLeft, bool isSetTempData, MemberExpression expression, bool isSingle)
        {
            var fieldName = GetName(parameter, expression, isLeft, isSingle);
            if (isSetTempData)
            {
                baseParameter.CommonTempData = fieldName;
            }
            else
            {
                fieldName = this.Context.DbMehtods.EqualTrue(fieldName.ObjToString());
                AppendMember(parameter, isLeft, fieldName);
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
                AppendMember(parameter, isLeft, GetToDateShort(parameter.CommonTempData.ObjToString()));
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
            if (this.Expression.Type == UtilConstants.DateType && this.Expression.ToString() == "DateTime.Now")
            {
                this.Expression = expression;
                var parameterName = base.AppendParameter(ExpressionTool.GetMemberValue(expression.Member, expression));
                base.AppendMember(parameter, isLeft, parameterName);
            }
            else
            {
                this.Start();
                var result = this.Context.DbMehtods.DateValue(new MethodCallExpressionModel()
                {
                    Args = new List<MethodCallExpressionArgs>() {
                     new MethodCallExpressionArgs() { IsMember = !isConst, MemberName = parameter.CommonTempData, MemberValue = null },
                     new MethodCallExpressionArgs() { IsMember = true, MemberName = name, MemberValue = name }
                  }
                });
                base.AppendMember(parameter, isLeft, result);
            }
            parameter.CommonTempData = oldCommonTempDate;
        }

        private void ResolveHasValue(ExpressionParameter parameter, MemberExpression expression)
        {
            parameter.CommonTempData = CommonTempDataType.Result;
            this.Expression = expression.Expression;
            this.Start();
            var methodParamter = new MethodCallExpressionArgs() { IsMember = true, MemberName = parameter.CommonTempData, MemberValue = null };
            if (expression.Expression?.Type != null)
            {
                methodParamter.Type =UtilMethods.GetUnderType(expression.Expression?.Type);
            }
            var result = this.Context.DbMehtods.HasValue(new MethodCallExpressionModel()
            {
                Args = new List<MethodCallExpressionArgs>() {
                    methodParamter
                  }
            });
            if (parameter.BaseExpression != null && ExpressionTool.IsLogicOperator(parameter.BaseExpression) && parameter.IsLeft == true)
            {
                if (base.Context.Result.Contains(ExpressionConst.FormatSymbol))
                {
                    base.Context.Result.Replace(ExpressionConst.FormatSymbol, "");
                }
                this.Context.Result.Append(result+" "+ExpressionTool.GetOperator(parameter.BaseExpression.NodeType)+" ");
            }
            else
            {
                this.Context.Result.Append(result);
            }
            parameter.CommonTempData = null;
        }

        private void ResolveLength(ExpressionParameter parameter, bool? isLeft, MemberExpression expression)
        {
            if (parameter.Context.ResolveType == ResolveExpressType.FieldSingle)
            {
                parameter.Context.ResolveType = ResolveExpressType.WhereSingle;
            }
            if (parameter.Context.ResolveType == ResolveExpressType.FieldMultiple)
            {
                parameter.Context.ResolveType = ResolveExpressType.WhereMultiple;
            }
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
            fieldName = Context.GetTranslationColumnName(shortName + UtilConstants.Dot + fieldName);
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
 
        private string GetToDateShort(string value)
        {
            var pars = new MethodCallExpressionModel()
            {
                Args = new List<MethodCallExpressionArgs>() {
                             new MethodCallExpressionArgs() { MemberName=value, MemberValue=value },
                         }
            };
            return this.Context.DbMehtods.ToDateShort(pars);
        }

        private void SettingParameters(ExpressionParameter parameter, out ExpressionParameter baseParameter, out MemberExpression expression, out bool? isLeft, out bool isSetTempData, out bool isValue, out bool isValueBool, out bool isLength, out bool isDateValue, out bool isHasValue, out bool isDateDate, out bool isMemberValue, out bool isSingle, out bool fieldIsBool, out bool isSelectField, out bool isField)
        {
            baseParameter = parameter.BaseParameter;
            expression = base.Expression as MemberExpression;
            var childExpression = expression.Expression as MemberExpression;
            var memberName = expression.Member.Name;
            var childIsMember = childExpression != null;
            var isRoot = parameter.BaseExpression == null;
            isLeft = parameter.IsLeft;
            isSetTempData = parameter.IsSetTempData;
            isValue = memberName == "Value" && expression.Member.DeclaringType.Name == "Nullable`1";
            var isBool = expression.Type == UtilConstants.BoolType;
            isValueBool = isValue && isBool && isRoot;
            isLength = memberName == "Length" && childIsMember && childExpression.Type == UtilConstants.StringType;
            isDateValue = memberName.IsIn(Enum.GetNames(typeof(DateType))) && (childIsMember && childExpression.Type == UtilConstants.DateType);
            var isLogicOperator = ExpressionTool.IsLogicOperator(baseParameter.OperatorValue) || baseParameter.OperatorValue.IsNullOrEmpty();
            isHasValue = isLogicOperator && memberName == "HasValue" && expression.Expression != null && expression.NodeType == ExpressionType.MemberAccess;
            isDateDate = memberName == "Date" && expression.Expression.Type == UtilConstants.DateType;
            isMemberValue = expression.Expression != null && expression.Expression.NodeType != ExpressionType.Parameter && !isValueBool;
            isSingle = parameter.Context.ResolveType.IsIn(ResolveExpressType.WhereSingle, ResolveExpressType.SelectSingle, ResolveExpressType.FieldSingle, ResolveExpressType.ArraySingle);
            fieldIsBool = isBool && isLogicOperator && (parameter.BaseParameter == null || !(parameter.BaseParameter.CurrentExpression is MemberInitExpression || parameter.BaseParameter.CurrentExpression is NewExpression));
            var isSelect = this.Context.ResolveType.IsIn(ResolveExpressType.SelectSingle, ResolveExpressType.SelectMultiple);
            isSelectField = isSelect && isRoot;
            isField = this.Context.ResolveType.IsIn(ResolveExpressType.FieldSingle, ResolveExpressType.FieldMultiple);
        }

        #endregion
    }
}
