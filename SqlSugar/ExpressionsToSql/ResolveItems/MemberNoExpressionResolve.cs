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
                            else
                            {
                                base.Context.Result.Append(parameterName);
                            }
                        }
                        else
                        {

                        }
                    }
                    break;
                case ResolveExpressType.WhereMultiple:
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
