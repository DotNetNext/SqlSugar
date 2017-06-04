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
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.Update:
                    var nodeType = expression.NodeType;
                    base.Expression = expression.Operand;
                    var isMember = expression.Operand is MemberExpression;
                    if (base.Expression is BinaryExpression || parameter.BaseExpression is BinaryExpression)
                    {
                        Default(parameter, nodeType);
                    }
                    else if (base.Expression is MemberExpression || base.Expression is ConstantExpression||isMember)
                    {
                        ChildNodeSet(parameter, nodeType);
                    }
                    else
                    {
                        Default(parameter, nodeType);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ChildNodeSet(ExpressionParameter parameter, ExpressionType nodeType)
        {
            BaseParameter.ChildExpression = base.Expression;
            parameter.CommonTempData = CommonTempDataType.ChildNodeSet;
            if (nodeType == ExpressionType.Not)
                AppendNot(parameter.CommonTempData);
            base.Start();
            parameter.BaseParameter.CommonTempData = parameter.CommonTempData;
            parameter.BaseParameter.ChildExpression = base.Expression;
            parameter.CommonTempData = null;
        }

        private void Default(ExpressionParameter parameter, ExpressionType nodeType)
        {
            BaseParameter.ChildExpression = base.Expression;
            parameter.CommonTempData = CommonTempDataType.Default;
            if (nodeType == ExpressionType.Not)
                AppendNot(parameter.CommonTempData);
            base.Start();
            parameter.BaseParameter.CommonTempData = parameter.CommonTempData;
            parameter.BaseParameter.ChildExpression = base.Expression;
            parameter.CommonTempData = null;
        }
    }
}
