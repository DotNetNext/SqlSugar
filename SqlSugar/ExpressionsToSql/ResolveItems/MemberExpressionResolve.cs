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
            var expression = base.Expression as MemberExpression;
            var isLeft = parameter.IsLeft;
            string fieldName = string.Empty;
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                    fieldName = getSingleName(parameter, expression, isLeft);
                    base.Context.Result.Append(fieldName);
                    break;
                case ResolveExpressType.WhereMultiple:
                    fieldName = getMultipleName(parameter, expression, isLeft);
                    base.Context.Result.Append(fieldName);
                    break;
                case ResolveExpressType.SelectSingle:
                    fieldName = getSingleName(parameter, expression, isLeft);
                    base.Context.Result.Append(fieldName);
                    break;
                case ResolveExpressType.SelectMultiple:
                    fieldName = getMultipleName(parameter, expression, isLeft);
                    base.Context.Result.Append(fieldName);
                    break;
                case ResolveExpressType.FieldSingle:
                    fieldName = getSingleName(parameter, expression, isLeft);
                    base.Context.Result.Append(fieldName);
                    break;
                case ResolveExpressType.FieldMultiple:
                    fieldName = getMultipleName(parameter, expression, isLeft);
                    base.Context.Result.Append(fieldName);
                    break;
                default:
                    break;
            }
        }

        private string  getMultipleName(ExpressionParameter parameter, MemberExpression expression, bool? isLeft)
        {
            string shortName = expression.Expression.ToString();
            string fieldName = expression.Member.Name;
            fieldName = shortName + "." + fieldName;
            if (parameter.BaseParameter.BinaryTempData != null)
                parameter.BaseParameter.BinaryTempData.Add(new KeyValuePair<string, BinaryExpressionInfo>(ExpressionConst.BinaryExpressionInfoListKey, new BinaryExpressionInfo()
                {
                    IsLeft = Convert.ToBoolean(isLeft),
                    Value = fieldName,
                    ExpressionType = expression.GetType()
                }));
            return fieldName;
        }

        private string getSingleName(ExpressionParameter parameter, MemberExpression expression, bool? isLeft)
        {
            string fieldName = expression.Member.Name;
            if (parameter.BaseParameter.BinaryTempData != null)
                parameter.BaseParameter.BinaryTempData.Add(new KeyValuePair<string, BinaryExpressionInfo>(ExpressionConst.BinaryExpressionInfoListKey, new BinaryExpressionInfo()
                {
                    IsLeft = Convert.ToBoolean(isLeft),
                    Value = fieldName,
                    ExpressionType = expression.GetType()
                }));
            return fieldName;
        }
    }
}
