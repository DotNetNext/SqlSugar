using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class UnaryExpressionResolve : BaseResolve
    {
        public UnaryExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as UnaryExpression;
            var baseParameter = parameter.BaseParameter;
            switch (this.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                    base.Expression = expression.Operand;
                    if (base.Expression is BinaryExpression||parameter.BaseExpression is BinaryExpression)
                    {
                        BaseParameter.ChildExpression = base.Expression;
                        parameter.CommonTempData = CommonTempDataType.Default;
                        base.Start();
                        parameter.BaseParameter.CommonTempData = parameter.CommonTempData;
                        parameter.BaseParameter.ChildExpression = base.Expression;
                        parameter.CommonTempData = null;
                    }
                    else if (base.Expression is MemberExpression || base.Expression is ConstantExpression)
                    {
                        BaseParameter.ChildExpression = base.Expression;
                        parameter.CommonTempData = CommonTempDataType.ChildNodeSet;
                        base.Start();
                        parameter.BaseParameter.CommonTempData = parameter.CommonTempData;
                        parameter.BaseParameter.ChildExpression = base.Expression;
                        parameter.CommonTempData = null;
                    }
                    break;
                case ResolveExpressType.SelectSingle:
                    break;
                case ResolveExpressType.SelectMultiple:
                    break;
                default:
                    break;
            }
        }
    }
}
