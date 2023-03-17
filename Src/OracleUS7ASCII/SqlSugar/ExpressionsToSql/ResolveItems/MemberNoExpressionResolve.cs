using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class MemberNoExpressionResolve : BaseResolve
    {
        public MemberNoExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as MemberExpression;
            var isLeft = parameter.IsLeft;
            var isField = expression.Member is System.Reflection.FieldInfo;
            var isProperty = expression.Member is System.Reflection.PropertyInfo;
            var baseParameter = parameter.BaseParameter;
            var isSetTempData = baseParameter.CommonTempData.HasValue() && baseParameter.CommonTempData.Equals(CommonTempDataType.Result);
            object value = GetValue(expression, isField, isProperty);
            switch (base.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.Update:
                    Update(parameter, isLeft, baseParameter, isSetTempData, value);
                    break;
                case ResolveExpressType.FieldSingle:
                    break;
                case ResolveExpressType.FieldMultiple:
                    break;
                default:
                    break;
            }
        }

        private void Update(ExpressionParameter parameter, bool? isLeft, ExpressionParameter baseParameter, bool isSetTempData, object value)
        {
            if (isSetTempData)
            {
                baseParameter.CommonTempData = value;
            }
            else
            {
                AppendValue(parameter, isLeft, value);
            }
        }

        private static object GetValue(MemberExpression expression, bool isField, bool isProperty)
        {
            object value = null;
            if (isField)
            {
                value = ExpressionTool.GetFiledValue(expression);
            }
            else if (isProperty)
            {
                value = ExpressionTool.GetPropertyValue(expression);
            }

            return value;
        }
    }

}
