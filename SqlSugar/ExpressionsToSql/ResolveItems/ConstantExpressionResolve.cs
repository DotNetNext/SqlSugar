using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public class ConstantExpressionResolve : BaseResolve
    {
        public ConstantExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as ConstantExpression;
            var isLeft = parameter.IsLeft;
            object value = expression.Value;
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                    parameter.BaseParameter.CommonTempData = value;
                    break;
                case ResolveExpressType.WhereSingle:
                    if (parameter.BaseExpression is BinaryExpression)
                    {
                        var otherExpression = isLeft == true ? parameter.BaseParameter.RightExpression : parameter.BaseParameter.LeftExpression;
                        if (otherExpression is MemberExpression)
                        {
                            string parameterName = Context.SqlParameterKeyWord
                                + ((MemberExpression)otherExpression).Member.Name
                                + Context.ParameterIndex;
                            base.Context.Parameters.Add(new SugarParameter(parameterName, value));
                            Context.ParameterIndex++;
                            parameterName = string.Format(" {0} ", parameterName);
                            if (isLeft == true)
                            {
                                parameterName += ExpressionConst.Format1 + parameter.BaseParameter.Index;
                            }
                            if (base.Context.Result.Contains(ExpressionConst.Format0))
                            {
                                base.Context.Result.Replace(ExpressionConst.Format0, parameterName);
                            }
                            else {
                                base.Context.Result.Append(parameterName);
                            }
                        }
                        else
                        {

                        }
                    }
                    break;
                case ResolveExpressType.WhereMultiple:
                    if (parameter.BaseExpression is BinaryExpression)
                    {

                    }
                    break;
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                default:
                    break;
            }
        }
    }
}
