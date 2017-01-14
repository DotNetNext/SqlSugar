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
                    SelectSingle(expression, parameter);
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

        private void SelectSingle(MemberInitExpression expression, ExpressionParameter parameter)
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
                if (memberAssignment.Expression.NodeType == ExpressionType.Constant)
                {
                    var value = ((ConstantExpression)memberAssignment.Expression).Value;
                    string parameterName = this.Context.SqlParameterKeyWord + "constant" + i;
                    parameter.Context.Result.Append(parameterName);
                    this.Context.Parameters.Add(new SugarParameter(parameterName, value));
                }
                else
                {
                    var memberExpression = (MemberExpression)memberAssignment.Expression;
                    if (memberExpression.Expression.NodeType.IsIn(ExpressionType.Constant))
                    {
                        var value = ExpressionTool.GetMemberValue(memberExpression.Member, memberExpression);
                        string parameterName = this.Context.SqlParameterKeyWord + "constant" + i;
                        parameter.Context.Result.Append(parameterName);
                        this.Context.Parameters.Add(new SugarParameter(parameterName, value));
                    }
                    else
                    {
                        var fieldNme = (memberExpression).Member.Name;
                        parameter.Context.Result.Append(fieldNme);
                    }
                }
            }
        }
    }
}
