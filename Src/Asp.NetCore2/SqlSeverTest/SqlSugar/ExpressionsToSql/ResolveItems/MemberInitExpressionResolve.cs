using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
                case ResolveExpressType.Update:
                    Update(expression, parameter);
                    break;
                default:
                    break;
            }
        }

        private void Update(MemberInitExpression expression, ExpressionParameter parameter)
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
                var type = memberAssignment.Member.ReflectedType;
                var memberName = this.Context.GetDbColumnName(type.Name, memberAssignment.Member.Name);
                var item = memberAssignment.Expression;
                if ((item is MemberExpression) && ((MemberExpression)item).Expression == null)
                {
                    var paramterValue = ExpressionTool.DynamicInvoke(item);
                    string parameterName = AppendParameter(paramterValue);
                    this.Context.Result.Append(base.Context.GetEqString(memberName, parameterName));
                }
                else if (IsMethod(item))
                {
                    if (item is UnaryExpression)
                        item = (item as UnaryExpression).Operand;
                    MethodCall(parameter, memberName, item);
                }
                else if (IsConst(item))
                {
                    base.Expression = item;
                    base.Start();
                    string parameterName = this.Context.SqlParameterKeyWord + ExpressionConst.Const + this.Context.ParameterIndex;
                    parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameterName));
                    this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.CommonTempData));
                    this.Context.ParameterIndex++;
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
                        parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameter.CommonTempData.ObjToString()));
                        base.Context.Result.CurrentParameter = null;
                    }
                }
                else if (item is BinaryExpression)
                {
                    var result = GetNewExpressionValue(item);
                    this.Context.Result.Append(base.Context.GetEqString(memberName, result));
                }
            }
        }

        private static bool IsConst(Expression item)
        {
            return item is UnaryExpression || item.NodeType == ExpressionType.Constant || (item is MemberExpression) && ((MemberExpression)item).Expression.NodeType == ExpressionType.Constant;
        }

        private static bool IsMethod(Expression item)
        {
            return item is MethodCallExpression || (item is UnaryExpression && (item as UnaryExpression).Operand is MethodCallExpression);
        }

        private void MethodCall(ExpressionParameter parameter, string memberName, Expression item)
        {
            if (IsSubMethod(item as MethodCallExpression))
            {
                UtilMethods.GetOldValue(parameter.CommonTempData, () =>
                {
                    parameter.CommonTempData = CommonTempDataType.Result;
                    base.Expression = item;
                    base.Start();
                    var subSql = base.Context.GetEqString(memberName, parameter.CommonTempData.ObjToString());
                    if (ResolveExpressType.Update == this.Context.ResolveType) {
                        subSql = Regex.Replace(subSql,@" \[\w+?\]\.",this.Context.GetTranslationTableName(parameter.CurrentExpression.Type.Name,true) +".");
                    }
                    parameter.Context.Result.Append(subSql);
                });        
            }
            else
            {
                base.Expression = item;
                base.Start();
                parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameter.CommonTempData.ObjToString()));
            }
        }

        private void Select(MemberInitExpression expression, ExpressionParameter parameter, bool isSingle)
        {
            foreach (MemberBinding binding in expression.Bindings)
            {
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new NotSupportedException();
                }
                MemberAssignment memberAssignment = (MemberAssignment)binding;
                var memberName = memberAssignment.Member.Name;
                var item = memberAssignment.Expression;
                ResolveNewExpressions(parameter, item, memberName);
            }
        }

        private bool IsSubMethod(MethodCallExpression express)
        {
            return SubTools.SubItemsConst.Any(it =>express.Object != null && express.Object.Type.Name == "Subqueryable`1");
        }
    }
}
