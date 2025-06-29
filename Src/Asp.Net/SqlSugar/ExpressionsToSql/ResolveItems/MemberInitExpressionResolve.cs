﻿using System;
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
                case ResolveExpressType.ArraySingle:
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
            var entityMaintenance = this.Context?.SugarContext?.Context?.EntityMaintenance;
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
                item = ExpressionTool.RemoveConvert(item);
                //Column IsJson Handler
                if (entityMaintenance!=null)
                { 
                    EntityColumnInfo columnInfo = entityMaintenance.GetEntityInfo(type).Columns.FirstOrDefault(it => it.PropertyName == memberAssignment.Member.Name);
                    if (columnInfo?.IsJson ?? false)
                    {
                        var paramterValue = ExpressionTool.DynamicInvoke(item);
                        var parameterName = AppendParameter(new SerializeService().SerializeObject(paramterValue));
                        var parameterObj = this.Context.Parameters.FirstOrDefault(it => it.ParameterName == parameterName);
                        if (parameterObj != null) 
                        {
                            parameterObj.IsJson = true;
                        }
                        this.Context.Result.Append(base.Context.GetEqString(memberName, parameterName));

                        continue;
                    }
                    else if (UtilMethods.IsParameterConverter(columnInfo))
                    { 
                        var value = ExpressionTool.DynamicInvoke(item);
                        var p=UtilMethods.GetParameterConverter(this.Context.ParameterIndex,this.Context.SugarContext.Context, value, memberAssignment.Expression, columnInfo);
                        this.Context.Result.Append(base.Context.GetEqString(memberName, p.ParameterName));
                        this.Context.ParameterIndex++;
                        this.Context.Parameters.Add(p);
                        continue;
                    }
                }

                if ((item is MemberExpression) && ((MemberExpression)item).Expression == null)
                {
                    var paramterValue = ExpressionTool.DynamicInvoke(item);
                    string parameterName = AppendParameter(paramterValue);
                    this.Context.Result.Append(base.Context.GetEqString(memberName, parameterName));
                }
                else if (entityMaintenance != null 
                    && entityMaintenance.GetEntityInfo(type).Columns.Any(it =>it.SqlParameterDbType is Type 
                    &&it.PropertyInfo.Name == memberName)
                    &&IsConstNew(ExpressionTool.RemoveConvertThanOne(item))) 
                {
                    var columnInfo= entityMaintenance.GetEntityInfo(expression.Type).Columns.First(it => it.SqlParameterDbType is Type && it.PropertyInfo.Name == memberName);
                    var columnDbType = columnInfo.SqlParameterDbType as Type;
                    var ParameterConverter = columnDbType.GetMethod("ParameterConverter").MakeGenericMethod(columnInfo.PropertyInfo.PropertyType);
                    var obj = Activator.CreateInstance(columnDbType);
                    var value = ExpressionTool.DynamicInvoke(item);
                    var p = ParameterConverter.Invoke(obj, new object[] { value, 100+i }) as SugarParameter;
                    parameter.Context.Result.Append(base.Context.GetEqString(memberName, p.ParameterName));
                    this.Context.Parameters.Add(p);
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
                                  new MethodCallExpressionArgs(){ IsMember=true, MemberName=parameter.CommonTempData.ObjToString()+"=1",Type=UtilConstants.BoolType  },
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
                    base.Expression = ExpressionTool.RemoveConvertThanOne(item);
                    base.Start();
                    string parameterName = this.Context.SqlParameterKeyWord + ExpressionConst.Const + this.Context.ParameterIndex;
                    parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameterName));
                    var addItem = new SugarParameter(parameterName, parameter.CommonTempData);
                    if (addItem.Value == null&&item.Type?.Name== "Nullable`1") 
                    {
                        var genericType = item.Type?.GenericTypeArguments?.FirstOrDefault();
                        if (genericType != null) 
                        {
                            addItem.DbType = new SugarParameter(parameterName, UtilMethods.GetDefaultValue(genericType)).DbType;
                        }
                    }
                    ConvertParameterTypeByType(item, addItem);

                    this.Context.Parameters.Add(addItem);
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
                        parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameter.CommonTempData.ObjToString().Replace(",",UtilConstants.ReplaceCommaKey)));

                        if (this.Context.Parameters != null)
                        {
                            var memberParameter = this.Context.Parameters?.FirstOrDefault(it => it.Value == null && it.ParameterName == parameter.CommonTempData.ObjToString());
                            if (memberParameter != null)
                            {
                                ConvertParameterTypeByType(item, memberParameter);
                            }
                        }

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

        private static void ConvertParameterTypeByType(Expression item, SugarParameter addItem)
        {
            var dataType = UtilMethods.GetUnderType(item.Type);
            if (addItem.Value == null && dataType == UtilConstants.DateType)
            {
                addItem.DbType = System.Data.DbType.Date;
            }
            if (addItem.Value == null && dataType.IsIn(UtilConstants.ULongType,UtilConstants.UIntType,UtilConstants.FloatType, UtilConstants.IntType, UtilConstants.LongType, UtilConstants.DecType, UtilConstants.DobType))
            {
                addItem.DbType = System.Data.DbType.Int32;
            }
            if (addItem.Value == null && dataType == UtilConstants.BoolType)
            {
                addItem.DbType = System.Data.DbType.Boolean;
            }
        }

        private static bool IsConst(Expression item)
        {
            return item is UnaryExpression || item.NodeType == ExpressionType.Constant || (item is MemberExpression) && ((MemberExpression)item).Expression.NodeType == ExpressionType.Constant;
        }
        private static bool IsConstNew(Expression item)
        {
            if (item != null)
            {
                if (!ExpressionTool.GetParameters(item).Any())
                {
                    return true;
                }
            }
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
                    var isSubJoin = subSql.Contains(" JOIN ")&& subSql.Contains(" ON ");
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
                        else if(isSubJoin)
                        {
                            var shortName=(base.BaseParameter.BaseParameter.CurrentExpression as LambdaExpression).Parameters[0].Name;
                            subSql = subSql.Replace(this.Context.GetTranslationColumnName(shortName), name);
                        }
                        else
                        {
                            var p=(base.BaseParameter?.BaseParameter?.CurrentExpression as LambdaExpression)?.Parameters[0].Name;
                            subSql = subSql.Replace(this.Context.SqlTranslationLeft+p+this.Context.SqlTranslationRight+".",name + ".") ;
                            subSql = subSql.Replace(this.Context.SqlTranslationLeft + p.ToUpper() + this.Context.SqlTranslationRight + ".", name + ".");
                            subSql = subSql.Replace(this.Context.SqlTranslationLeft + p.ToLower() + this.Context.SqlTranslationRight + ".", name + ".");
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
            var isAnyParameterExpression = false;
            foreach (MemberBinding binding in expression.Bindings)
            {
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new NotSupportedException();
                }
                MemberAssignment memberAssignment = (MemberAssignment)binding;
                var memberName = memberAssignment.Member.Name;
                if (this.Context?.SugarContext?.QueryBuilder?.AppendNavInfo?.MappingNavProperties?.ContainsKey(memberName) == true) 
                {
                    continue;
                }
                var item = memberAssignment.Expression;
                if (item.Type.IsClass()&& item is MemberExpression &&(item as MemberExpression).Expression is ParameterExpression) 
                {
                    var rootType = ((item as MemberExpression).Expression as ParameterExpression).Type;
                    if (this.Context.SugarContext != null) 
                    {
                        var navColumn = this.Context.SugarContext.Context.EntityMaintenance.GetEntityInfo(rootType)
                            .Columns.FirstOrDefault(x=>x.PropertyName==memberName);
                        if (navColumn != null&& navColumn.Navigat!=null) 
                        {
                            break;
                        }
                    }
                }
                if (IsNullable(item) && item is UnaryExpression)
                {
                    var memtype = ExpressionTool.GetMemberInfoType(memberAssignment.Member);
                    if (IsNullable(memtype) && UtilMethods.GetUnderType(memtype) == UtilMethods.GetUnderType(item.Type))
                    {
                        item = (item as UnaryExpression).Operand;
                    }
                }
                if(item is ParameterExpression) 
                {
                    isAnyParameterExpression = true;
                }
                ResolveNewExpressions(parameter, item, memberName);
            }
            if (isAnyParameterExpression && this.Context?.SugarContext?.QueryBuilder is QueryBuilder builder) 
            {
                builder.IsAnyParameterExpression = true;
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
