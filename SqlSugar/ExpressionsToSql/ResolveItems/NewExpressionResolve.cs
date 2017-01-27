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
                    string memberName = expression.Members[i].Name;
                    ++i;
                    if (item.NodeType == ExpressionType.Constant || (item is MemberExpression) && ((MemberExpression)item).Expression.NodeType == ExpressionType.Constant)
                    {
                        base.Expression = item;
                        base.Start();
                        string parameterName = this.Context.SqlParameterKeyWord + "constant" + i;
                        parameter.Context.Result.Append(base.Context.GetAsString(memberName,parameterName));
                        this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.CommonTempData));
                    }
                    else if (item is MemberExpression)
                    {
                        if (base.Context.Result.IsLockCurrentParameter == false)
                        {
                            base.Context.Result.CurrentParameter = parameter;
                            base.Context.Result.IsLockCurrentParameter = true;
                            parameter.IsAppendTempDate();
                            base.Expression = item;
                            base.Start();
                            parameter.IsAppendResult();
                            base.Context.Result.Append(base.Context.GetAsString(memberName,parameter.CommonTempData.ObjToString()));
                            base.Context.Result.CurrentParameter = null;
                        }
                    }
                    else if (item is BinaryExpression)
                    {
                        if (base.Context.Result.IsLockCurrentParameter == false)
                        {
                            base.Context.Result.CurrentParameter = parameter;
                            base.Context.Result.IsLockCurrentParameter = true;
                            parameter.IsAppendTempDate();
                            base.Expression = item;
                            parameter.CommonTempData = "simple";
                            base.Start();
                            parameter.CommonTempData = null;
                            parameter.IsAppendResult();
                            base.Context.Result.Append(base.Context.GetAsString(memberName, parameter.CommonTempData.ObjToString()));
                            base.Context.Result.CurrentParameter = null;
                        }
                    }
                    else
                    {
                        Check.ThrowNotSupportedException(item.GetType().Name);
                    }
                }
            }
        }
    }
}

