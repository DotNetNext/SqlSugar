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
                var memberName = memberAssignment.Member.Name;
                var item = memberAssignment.Expression;
                if (item.NodeType == ExpressionType.Constant || (item is MemberExpression) && ((MemberExpression)item).Expression.NodeType == ExpressionType.Constant)
                {
                    base.Expression = item;
                    base.Start();
                    string parameterName = this.Context.SqlParameterKeyWord + "constant" + i;
                    parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameterName));
                    this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.CommonTempData));
                }
                else if (item is MethodCallExpression)
                {
                    base.Expression = item;
                    base.Start();
                    parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameter.CommonTempData.ObjToString()));
                }
                else if (item is MemberExpression || item is UnaryExpression)
                {
                    if (base.Context.Result.IsLockCurrentParameter == false)
                    {
                        base.Context.Result.CurrentParameter = parameter;
                        base.Context.Result.IsLockCurrentParameter = true;
                        parameter.IsAppendTempDate();
                        base.Expression = item;
                        base.Start();
                        parameter.IsAppendResult();
                        base.Context.Result.Append(base.Context.GetEqString(memberName, parameter.CommonTempData.ObjToString()));
                        base.Context.Result.CurrentParameter = null;
                    }
                }
                else if (item is BinaryExpression)
                {
                    Check.ThrowNotSupportedException(item.GetType().Name);
                }
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
                var memberName = memberAssignment.Member.Name;
                var item = memberAssignment.Expression;
                if (item.NodeType == ExpressionType.Constant || (item is MemberExpression) && ((MemberExpression)item).Expression.NodeType == ExpressionType.Constant)
                {
                    base.Expression = item;
                    base.Start();
                    string parameterName = this.Context.SqlParameterKeyWord + "constant" + i;
                    parameter.Context.Result.Append(base.Context.GetAsString(memberName, parameterName));
                    this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.CommonTempData));
                }
                else if (item is MethodCallExpression)
                {
                    base.Expression = item;
                    base.Start();
                    parameter.Context.Result.Append(base.Context.GetAsString(memberName, parameter.CommonTempData.ObjToString()));
                }
                else if (item is MemberExpression|| item is UnaryExpression)
                {
                    if (base.Context.Result.IsLockCurrentParameter == false)
                    {
                        base.Context.Result.CurrentParameter = parameter;
                        base.Context.Result.IsLockCurrentParameter = true;
                        parameter.IsAppendTempDate();
                        base.Expression = item;
                        base.Start();
                        parameter.IsAppendResult();
                        base.Context.Result.Append(base.Context.GetAsString(memberName, parameter.CommonTempData.ObjToString()));
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
                        this.Context.Result.TrimEnd();
                        base.Context.Result.Append(base.Context.GetAsString(memberName, parameter.CommonTempData.ObjToString()));
                        base.Context.Result.CurrentParameter = null;
                    }
                }
                else if (item.Type.IsClass())
                {
                    base.Expression = item;
                    base.Start();
                    var shortName = parameter.CommonTempData;
                    var listProperties = item.Type.GetProperties().Cast<PropertyInfo>().ToList();
                    foreach (var property in listProperties)
                    {
                        if (this.Context.IgnoreComumnList != null
                            && this.Context.IgnoreComumnList.Any(
                                it => it.EntityName == item.Type.Name && it.EntityPropertyName == property.Name))
                        {
                            continue;
                        }
                        if (property.PropertyType.IsClass())
                        {

                        }
                        else
                        {
                            var asName = "[" + item.Type.Name + "." + property.Name + "]";
                            var columnName = property.Name;
                            if (Context.IsJoin)
                            {
                                base.Context.Result.Append(Context.GetAsString(asName, columnName, shortName.ObjToString()));
                            }
                            else
                            {
                                base.Context.Result.Append(Context.GetAsString(asName, columnName));
                            }
                        }
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
