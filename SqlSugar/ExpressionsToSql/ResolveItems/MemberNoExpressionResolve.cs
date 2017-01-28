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
            object value = null;
            var isField = expression.Member is System.Reflection.FieldInfo;
            var isProperty = expression.Member is System.Reflection.PropertyInfo;
            if (isField)
            {
                value = ExpressionTool.GetFiledValue(expression);
            }
            else if (isProperty)
            {
                value = ExpressionTool.GetPropertyValue(expression);
            }
            switch (base.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                    AppendParameter(parameter, isLeft, value);
                    break;
                case ResolveExpressType.WhereMultiple:
                    AppendParameter(parameter, isLeft, value);
                    break;
                case ResolveExpressType.SelectSingle:
                    break;
                case ResolveExpressType.SelectMultiple:
                    break;
                case ResolveExpressType.FieldSingle:
                    break;
                case ResolveExpressType.FieldMultiple:
                    break;
                default:
                    break;
            }
        }
    }

}
