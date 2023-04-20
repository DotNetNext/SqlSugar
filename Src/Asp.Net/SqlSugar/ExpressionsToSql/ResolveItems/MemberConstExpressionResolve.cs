using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class MemberConstExpressionResolve : BaseResolve
    {
        public MemberConstExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as MemberExpression;
            var isLeft = parameter.IsLeft;
            object value = ExpressionTool.GetMemberValue(expression.Member, expression);
            var baseParameter = parameter.BaseParameter;
            var isSetTempData = baseParameter.CommonTempData.HasValue() && baseParameter.CommonTempData.Equals(CommonTempDataType.Result);
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.Update:
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                    value = Select(parameter, value);
                    break;
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                    Where(parameter, isLeft, value, baseParameter, isSetTempData);
                    break;
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                    break;
            }
        }

        private void Where(ExpressionParameter parameter, bool? isLeft, object value, ExpressionParameter baseParameter, bool isSetTempData)
        {
            if (parameter.OppsiteExpression != null)
            {
                var exp = ExpressionTool.RemoveConvert(parameter.OppsiteExpression);
                value = GetMemberValue(value, exp);
            }
            if (isSetTempData)
            {
                baseParameter.CommonTempData = value;
            }
            else
            {
                AppendValue(parameter, isLeft, value);
            }
        }
  
        private  object Select(ExpressionParameter parameter, object value)
        {
            if (value != null && value.GetType().IsEnum())
            {
                if (this.Context?.SugarContext?.Context?.CurrentConnectionConfig?.MoreSettings?.TableEnumIsString == true)
                {
                    value = Convert.ToString(value);
                }
                else
                {
                    value = Convert.ToInt64(value);
                }
            }
            parameter.BaseParameter.CommonTempData = value;
            return value;
        }
    }
}
