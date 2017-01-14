using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class MemberInitExpressionResolve : BaseResolve
    {
        public MemberInitExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as MemberInitExpression;
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                    break;
                case ResolveExpressType.WhereMultiple:
                    break;
                case ResolveExpressType.SelectSingle:
                    Select(expression, parameter, true);
                    break;
                case ResolveExpressType.SelectMultiple:
                    Select(expression, parameter, false);
                    break;
                case ResolveExpressType.FieldSingle:
                    break;
                case ResolveExpressType.FieldMultiple:
                    break;
                default:
                    break;
            }
        }

        public void Select(MemberInitExpression expression, ExpressionParameter parameter, bool isSingle)
        {
            int i = 0;
            foreach (MemberBinding binding in expression.Bindings)
            {
                ++i;
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new NotSupportedException();
                }
                MemberAssignment memberAssignment = (MemberAssignment)binding;
                var item = memberAssignment.Expression;
                if (item.NodeType == ExpressionType.Constant || (item is MemberExpression) && ((MemberExpression)item).Expression.NodeType == ExpressionType.Constant)
                {
                    base.Expression = item;
                    base.Start();
                    string parameterName = this.Context.SqlParameterKeyWord + "constant" + i;
                    parameter.Context.Result.Append(parameterName);
                    this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.TempDate));
                }
                else if(item is MemberExpression)
                {
                    base.Expression= memberAssignment.Expression;
                    base.Start();
                }
            }
        }
    }
}
