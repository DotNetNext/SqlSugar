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
                case ResolveExpressType.Array:
                    foreach (var item in expression.Arguments)
                    {
                        base.Expression = item;
                        base.Start();
                    }
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
                        parameter.Context.Result.Append(base.Context.GetAsString(memberName, parameterName));
                        this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.CommonTempData));
                    }
                    else if (item is MethodCallExpression) {
                        base.Expression = item;
                        base.Start();
                        parameter.Context.Result.Append(base.Context.GetAsString(memberName, parameter.CommonTempData.ObjToString()));
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
                                    it => it.EntityName == item.Type.Name && it.PropertyName == property.Name))
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
}

