using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class MemberExpressionResolve : BaseResolve
    {
        public MemberExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as MemberExpression;
            var isLeft = parameter.IsLeft;
            var isWhereSingle = parameter.Context.IsWhereSingle;
            string fieldName = string.Empty;
            switch (parameter.Context.Type)
            {
                case ResolveExpressType.WhereSingle:
                    fieldName = GetFieldNameByWhereSingle(parameter, expression, isLeft);
                    break;
                case ResolveExpressType.WhereMultiple:
                    fieldName = GetFiledNameByWhereMultiple(parameter, expression, isLeft);
                    break;
                case ResolveExpressType.SelectSingle:
                    base.Context.ResultString = new StringBuilder();
                    base.Context.ResultString.Append(fieldName);
                    break;
                case ResolveExpressType.SelectMultiple:
                    base.Context.ResultString = new StringBuilder();
                    base.Context.ResultString.Append(fieldName);
                    break;
                case ResolveExpressType.FieldSingle:
                    break;
                case ResolveExpressType.FieldMultiple:
                    break;
                default:
                    break;
            }
        }

        private string GetFiledNameByWhereMultiple(ExpressionParameter parameter, MemberExpression expression, bool? isLeft)
        {
            string fieldName = expression.Member.ToString();
            if (parameter.BaseParameter.BinaryExpressionInfoList != null)
                parameter.BaseParameter.BinaryExpressionInfoList.Add(new KeyValuePair<string, BinaryExpressionInfo>(ExpressionConst.BinaryExpressionInfoListKey, new BinaryExpressionInfo()
                {
                    IsLeft = Convert.ToBoolean(isLeft),
                    Value = fieldName,
                    ExpressionType = expression.GetType()
                }));
            return fieldName;
        }

        private string GetFieldNameByWhereSingle(ExpressionParameter parameter, MemberExpression expression, bool? isLeft)
        {
            string fieldName = expression.Member.Name;
            if (parameter.BaseParameter.BinaryExpressionInfoList != null)
                parameter.BaseParameter.BinaryExpressionInfoList.Add(new KeyValuePair<string, BinaryExpressionInfo>(ExpressionConst.BinaryExpressionInfoListKey, new BinaryExpressionInfo()
                {
                    IsLeft = Convert.ToBoolean(isLeft),
                    Value = fieldName,
                    ExpressionType = expression.GetType()
                }));
            return fieldName;
        }
    }
}
