using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class NewExpressionResolve : BaseResolve
    {
        public NewExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as NewExpression;
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

        private void Select(NewExpression expression, ExpressionParameter parameter, bool isSingle)
        {
            if (expression.Arguments != null)
            {
                int i = 0;
                foreach (var item in expression.Arguments)
                {
                    ++i;
                    if (item.NodeType == ExpressionType.Constant)
                    {
                        base.Expression = item;
                        base.Start();
                        string parameterName = this.Context.SqlParameterKeyWord + "constant" + i;
                        parameter.Context.Result.Append(parameterName);
                        this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.TempDate));
                    }
                    else
                    {
                        var memberExpression = (MemberExpression)item;
                        if (memberExpression.Expression.NodeType.IsIn(ExpressionType.Constant))
                        {
                            base.Expression = memberExpression;
                            base.Start();
                            string parameterName = this.Context.SqlParameterKeyWord + "constant" + i;
                            parameter.Context.Result.Append(parameterName);
                            this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.TempDate));
                        }
                        else
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
                    }
                }
            }
        }
    }
}
