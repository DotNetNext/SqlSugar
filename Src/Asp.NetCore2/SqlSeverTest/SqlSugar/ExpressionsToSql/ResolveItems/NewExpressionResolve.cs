using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class NewExpressionResolve : BaseResolve
    {
        public NewExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as NewExpression;
            if (expression.Type.IsIn(UtilConstants.DateType,UtilConstants.GuidType))
            {
                NewValueType(parameter, expression);
                return;
            }
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                    Check.ThrowNotSupportedException(expression.ToString());
                    break;
                case ResolveExpressType.WhereMultiple:
                    Check.ThrowNotSupportedException(expression.ToString());
                    break;
                case ResolveExpressType.SelectSingle:
                    Check.Exception(expression.Type == UtilConstants.DateType, "ThrowNotSupportedException {0} ", expression.ToString());
                    Select(expression, parameter, true);
                    break;
                case ResolveExpressType.SelectMultiple:
                    Check.Exception(expression.Type == UtilConstants.DateType, "ThrowNotSupportedException {0} ", expression.ToString());
                    Select(expression, parameter, false);
                    break;
                case ResolveExpressType.FieldSingle:
                    Check.ThrowNotSupportedException(expression.ToString());
                    break;
                case ResolveExpressType.FieldMultiple:
                case ResolveExpressType.ArrayMultiple:
                case ResolveExpressType.ArraySingle:
                    foreach (var item in expression.Arguments)
                    {
                        base.Expression = item;
                        base.Start();
                    }
                    break;
                case ResolveExpressType.Join:
                    base.Context.ResolveType = ResolveExpressType.WhereMultiple;
                    int i = 0;
                    foreach (var item in expression.Arguments)
                    {
                        if (item.Type!=typeof(JoinType))
                        {
                            base.Expression = item;
                            base.Start();
                        }
                        if (item.Type == typeof(JoinType))
                        {
                            if (i > 0)
                            {
                                base.Context.Result.Append("," + item.ToString()+ ",");
                            }
                            else
                            {
                                base.Context.Result.Append(item.ToString() + ",");
                            }
                            ++i;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void NewValueType(ExpressionParameter parameter, NewExpression expression)
        {
            try
            {
                var value = ExpressionTool.DynamicInvoke(expression);
                var isSetTempData = parameter.CommonTempData.HasValue() && parameter.CommonTempData.Equals(CommonTempDataType.Result);
                if (isSetTempData)
                {
                    parameter.CommonTempData = value;
                }
                else
                {
                    AppendValue(parameter, parameter.IsLeft, value);
                }
            }
            catch (Exception ex)
            {
                Check.Exception(expression.Type == UtilConstants.DateType, "ThrowNotSupportedException {0} ", ex.ToString());
            }
        }

        private void Select(NewExpression expression, ExpressionParameter parameter, bool isSingle)
        {
            if (expression.Arguments != null)
            {
                int i = 0;
                foreach (var item in expression.Arguments)
                {
                    string memberName = expression.Members[i].Name;
                    ++i;
                    ResolveNewExpressions(parameter, item, memberName);
                }
            }
        }
    }
}

