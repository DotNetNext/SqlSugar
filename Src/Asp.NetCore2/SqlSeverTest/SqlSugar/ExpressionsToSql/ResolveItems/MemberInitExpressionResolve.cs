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
                var type = expression.Type;
                var memberName = this.Context.GetDbColumnName(type.Name, memberAssignment.Member.Name);
                var item = memberAssignment.Expression;

                //Column IsJson Handler
                if (memberAssignment.Member.CustomAttributes != null)
                {
                    var customAttribute = memberAssignment.Member.GetCustomAttribute<SugarColumn>();

                    if (customAttribute?.IsJson ?? false)
                    {
                        var paramterValue = ExpressionTool.DynamicInvoke(item);
                        var parameterName = AppendParameter(new SerializeService().SerializeObject(paramterValue));
                        this.Context.Result.Append(base.Context.GetEqString(memberName, parameterName));

                        continue;
                    }
                }

                if ((item is MemberExpression) && ((MemberExpression)item).Expression == null)
                {
                    var paramterValue = ExpressionTool.DynamicInvoke(item);
                    string parameterName = AppendParameter(paramterValue);
                    this.Context.Result.Append(base.Context.GetEqString(memberName, parameterName));
                }
                else if (IsNotMember(item))
                {
                    if (base.Context.Result.IsLockCurrentParameter == false)
                    {
                        base.Context.Result.CurrentParameter = parameter;
                        base.Context.Result.IsLockCurrentParameter = true;
                        parameter.IsAppendTempDate();
                        base.Expression = item;
                        base.Expression = (item as UnaryExpression).Operand;
                        base.Start();
                        parameter.IsAppendResult();
                        var result = this.Context.DbMehtods.IIF(new MethodCallExpressionModel()
                        {
                            Args = new List<MethodCallExpressionArgs>() {
                                  new MethodCallExpressionArgs(){ IsMember=true, MemberName=parameter.CommonTempData.ObjToString()+"=1" },
                                  new MethodCallExpressionArgs(){ IsMember=true,MemberName=AppendParameter(0)  },
                                  new MethodCallExpressionArgs(){ IsMember=true, MemberName=AppendParameter(1)  }
                           }
                        });
                        parameter.Context.Result.Append(base.Context.GetEqString(memberName, result));
                        base.Context.Result.CurrentParameter = null;
                    }
                }
                else if (IsNotParameter(item))
                {
                    try
                    {
                        parameter.Context.Result.Append(base.Context.GetEqString(memberName, AppendParameter(ExpressionTool.DynamicInvoke(item).ObjToBool())));
                    }
                    catch
                    {
                        throw new NotSupportedException(item.ToString());
                    }
                }
                else if (IsMethod(item))
                {
                    if (item is UnaryExpression)
                        item = (item as UnaryExpression).Operand;
                    var callMethod = item as MethodCallExpression;
                    if (MethodTimeMapping.Any(it => it.Key == callMethod.Method.Name) || MethodMapping.Any(it => it.Key == callMethod.Method.Name) || IsExtMethod(callMethod.Method.Name) || IsSubMethod(callMethod) || callMethod.Method.DeclaringType.FullName.StartsWith(UtilConstants.AssemblyName + UtilConstants.Dot))
                    {
                        MethodCall(parameter, memberName, item);
                    }
                    else
                    {
                        var paramterValue = ExpressionTool.DynamicInvoke(item);
                        string parameterName = AppendParameter(paramterValue);
                        this.Context.Result.Append(base.Context.GetEqString(memberName, parameterName));
                    }
                }
                else if (IsConst(item) && IsConvert(item) && UtilMethods.IsNullable(item.Type) && UtilMethods.GetUnderType(item.Type) == UtilConstants.BoolType)
                {
                    item = (item as UnaryExpression).Operand;
                    parameter.Context.Result.Append(base.Context.GetEqString(memberName, GetNewExpressionValue(item)));
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
                    if (result.HasValue())
                    {
                        result = result.Replace(",", UtilConstants.ReplaceCommaKey);
                    }
                    this.Context.Result.Append(base.Context.GetEqString(memberName, result));
                }
                else if (item is MemberInitExpression)
                {
                    try
                    {
                        var value = ExpressionTool.DynamicInvoke(item);
                        var parameterName = AppendParameter(value);
                        parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameterName));
                    }
                    catch (Exception ex)
                    {
                        throw new NotSupportedException("Not Supported " + item.ToString() + " " + ex.Message);
                    }
                }
                else if (item is NewExpression)
                {
                    try
                    {
                        var value = ExpressionTool.DynamicInvoke(item);
                        var parameterName = AppendParameter(value);
                        parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameterName));
                    }
                    catch (Exception ex)
                    {
                        throw new NotSupportedException("Not Supported " + item.ToString() + " " + ex.Message);
                    }
                }
                else if (item is ConditionalExpression)
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
                    if (subSql.Contains(","))
                    {
                        subSql = subSql.Replace(",", UtilConstants.ReplaceCommaKey);
                    }
                    if (ResolveExpressType.Update == this.Context.ResolveType)
                    {
                        string name = this.Context.GetTranslationTableName(parameter.CurrentExpression.Type.Name, true);
                        if (name.Contains("."))
                        {

                        }
                        else
                        {
                            subSql = Regex.Replace(subSql, @" \[\w+?\]\.| ""\w+?""\.| \`\w+?\`\.", name + ".");
                        }
                    }
                    parameter.Context.Result.Append(subSql);
                });
            }
            else
            {
                base.Expression = item;
                base.Start();
                parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameter.CommonTempData.ObjToString().Replace(",", UtilConstants.ReplaceCommaKey)));
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
                if (IsNullable(item) && item is UnaryExpression)
                {
                    var memtype = ExpressionTool.GetMemberInfoType(memberAssignment.Member);
                    if (IsNullable(memtype) && UtilMethods.GetUnderType(memtype) == UtilMethods.GetUnderType(item.Type))
                    {
                        item = (item as UnaryExpression).Operand;
                    }
                }
                ResolveNewExpressions(parameter, item, memberName);
            }
        }

        private static bool IsNullable(Type memtype)
        {
            return memtype.Name == "Nullable`1";
        }

        private static bool IsNullable(Expression item)
        {
            return item.Type.Name == "Nullable`1";
        }

        //private bool IsSubMethod(MethodCallExpression express)
        //{
        //    return SubTools.SubItemsConst.Any(it =>express.Object != null && express.Object.Type.Name == "Subqueryable`1");
        //}
        private bool IsExtMethod(string methodName)
        {
            if (this.Context.SqlFuncServices == null) return false;
            return this.Context.SqlFuncServices.Select(it => it.UniqueMethodName).Contains(methodName);
        }
        private bool CheckMethod(MethodCallExpression expression)
        {
            if (IsExtMethod(expression.Method.Name))
                return true;
            if (expression.Method.ReflectedType().FullName != ExpressionConst.SqlFuncFullName)
                return false;
            else
                return true;
        }
    }
}
