using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class TypeParameterExpressionReolve : BaseResolve
    {
        public TypeParameterExpressionReolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = (ParameterExpression)base.Expression;
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                    break;
                case ResolveExpressType.WhereMultiple:
                    break;
                case ResolveExpressType.Update:
                    parameter.BaseParameter.CommonTempData = expression.Name;
                    break;
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                    if (parameter.BaseParameter!=null&&parameter.BaseParameter.CurrentExpression.NodeType == ExpressionType.Lambda)
                    {
                        this.Context.Result.Append(expression.Name + ".*");
                    }
                    else
                    {
                        parameter.BaseParameter.CommonTempData = expression.Name;
                    }
                    break;
                case ResolveExpressType.FieldSingle:
                    break;
                case ResolveExpressType.FieldMultiple:
                    break;
                case ResolveExpressType.Join:
                    break;
                default:
                    break;
            }
        }
    }
}
