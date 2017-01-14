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
                if (memberAssignment.Expression.NodeType == ExpressionType.Constant)
                {
                    base.Expression = memberAssignment.Expression;
                    base.Start();
                    string parameterName = this.Context.SqlParameterKeyWord + "constant" + i;
                    parameter.Context.Result.Append(parameterName);
                    this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.TempDate));
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
                    else if (memberExpression.Expression.NodeType.IsIn(ExpressionType.Parameter))
                    {
                        var fieldNme = (memberExpression).Member.Name;
                        if (isSingle)
                        {
                            parameter.Context.Result.Append(fieldNme);
                        }
                        else
                        {
                            var shortName = memberExpression.Expression.ToString();
                            parameter.Context.Result.Append(shortName + "." + fieldNme);
                        }
                    }
                    else
                    {
                        Check.ThrowNotSupportedException(memberExpression.Expression.NodeType.ToString());
                    }
                }
            }
        }
    }
}
