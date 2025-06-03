﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    /// <summary>
    ///BaseResolve New Expression
    /// </summary>
    public partial class BaseResolve
    {
        private void ResloveOtherMUC(ExpressionParameter parameter, Expression item, string asName)
        {
            if (ExpressionTool.GetMethodName(item) == "NewGuid")
            {
                parameter.Context.Result.Append(this.Context.GetAsString2(asName, this.Context.DbMehtods.NewUid(null)));
                return;
            }
            else if (ExpressionTool.GetMethodName(item) == "OnlyInSelectConvertToString")
            {
                AppendOnlyInSelectConvertToString(parameter, item, asName);
                return;
            }
            else if (ExpressionTool.GetMethodName(item) == "ToString"
                                      && (item as MethodCallExpression)?.Arguments?.Count() == 1
                                      && (item as MethodCallExpression)?.Object?.Type != UtilConstants.DateType
                                      && this.Context?.SugarContext?.QueryBuilder != null
                                      && (item as MethodCallExpression)?.Method?.ReflectedType?.Name != "SqlFunc"
                                      && (item as MethodCallExpression)?.Method?.ReflectedType?.Name != "Convert"
                                       )
            {
                var format = ExpressionTool.GetExpressionValue((item as MethodCallExpression)?.Arguments[0]);
                var childExpression = (item as MethodCallExpression)?.Object;
                var type = childExpression.Type;
                if (this.Context.SugarContext.QueryBuilder.QueryableFormats == null)
                {
                    this.Context.SugarContext.QueryBuilder.QueryableFormats = new List<QueryableFormat>();
                }
                this.Context.SugarContext.QueryBuilder.QueryableFormats.Add(new QueryableFormat()
                {
                    Format = format + "",
                    PropertyName = asName,
                    Type = type,
                    TypeString = type.FullName,
                    MethodName = "ToString"
                });
                parameter.Context.Result.Append(this.Context.GetAsString2(asName, GetNewExpressionValue(childExpression)));
                return;
            }
            else if (ExpressionTool.GetMethodName(item) == "ToString"
                              && (item as MethodCallExpression)?.Arguments?.Count() == 0
                              && (item as MethodCallExpression)?.Object?.Type?.IsEnum == true
                              && this.Context?.SugarContext?.QueryBuilder != null)
            {
                var childExpression = (item as MethodCallExpression)?.Object;
                var type = childExpression.Type;
                if (this.Context.SugarContext.QueryBuilder.QueryableFormats == null)
                {
                    this.Context.SugarContext.QueryBuilder.QueryableFormats = new List<QueryableFormat>();
                }
                this.Context.SugarContext.QueryBuilder.QueryableFormats.Add(new QueryableFormat()
                {
                    PropertyName = asName,
                    Type = type,
                    TypeString = "Enum",
                    MethodName = "ToString"
                });
                parameter.Context.Result.Append(this.Context.GetAsString2(asName, GetNewExpressionValue(childExpression)));
                return;
            }
            else if (ExpressionTool.GetMethodName(item) == "IsNull"
                              && this.Context.SingleTableNameSubqueryShortName == null
                              && this.BaseParameter?.CurrentExpression is NewExpression
                              && (item as MethodCallExpression)?.Arguments?.FirstOrDefault() is MethodCallExpression
                              && item?.ToString()?.Contains("Join") == true
                              && ExpressionTool.GetParameters(this.BaseParameter?.CurrentExpression).Count() > 1)
            {
                var ps = ExpressionTool.GetParameters(this.BaseParameter?.CurrentExpression);
                this.Expression = item;
                this.Start();
                parameter.Context.Result.Append(this.Context.GetAsString2(asName, parameter.CommonTempData.ObjToString()));
                this.Context.SingleTableNameSubqueryShortName = ps.FirstOrDefault().Name;
                return;
            }
            else if (item is MethodCallExpression && ExpressionTool.IsVariable(item))
            {
                var p = GetNewExpressionValue(item);
                parameter.Context.Result.Append(this.Context.GetAsString2(asName, p));
                return;
            }
            else if (item is ConditionalExpression&& ExpressionTool.GetParameters(item).Count > 0) 
            {
                var p = GetNewExpressionValue(item);
                parameter.Context.Result.Append(this.Context.GetAsString2(asName, p));
                return;
            }
            this.Expression = item;
            var negateString = string.Empty;
            if (item.NodeType == ExpressionType.Negate)
            {
                negateString = " -1*";
                this.Expression = (this.Expression as UnaryExpression).Operand;
            }
            this.Start();
            if (ExpressionTool.GetMethodName(item) == "MappingColumn")
            {
                parameter.Context.Result.Append(negateString+this.Context.GetAsString2(asName, parameter.CommonTempData.ObjToString()));
            }
            else if (parameter.CommonTempData?.Equals(CommonTempDataType.Append) == true) 
            {
                if (item.NodeType == ExpressionType.Negate)
                {
                    negateString = "*-1 ";
                }
                else 
                {
                    negateString = null;
                }
                parameter.Context.Result.TrimEnd();
                parameter.Context.Result.Append(negateString+" AS " + this.Context.GetTranslationColumnName(asName));
            }
            else
            {
                parameter.Context.Result.Append(negateString+this.Context.GetAsString2(asName, parameter.CommonTempData.ObjToString()));
            }
        }

        private void ResloveCountAny(ExpressionParameter parameter, Expression item, string asName)
        {
            if (this.Context.IsSingle && this.Context.SingleTableNameSubqueryShortName == null)
            {
                this.Context.SingleTableNameSubqueryShortName = item.ToString().Split('.').First();
            }
            parameter.Context.Result.Append(this.Context.GetAsString(asName, GetNewExpressionValue(item)));
        }

        private void ResloveNot(ExpressionParameter parameter, Expression item, string asName)
        {
            var asValue = GetAsNamePackIfElse(GetNewExpressionValue(item)).ObjToString();
            parameter.Context.Result.Append(this.Context.GetAsString(asName, asValue));
        }

        private void ResloveBoolMethod(ExpressionParameter parameter, Expression item, string asName)
        {
            this.Expression = item;
            if (ExpressionTool.GetMethodName(item) == "Any"&&!ExpressionTool.GetTopLevelMethodCalls(item).Contains("Subqueryable"))
            {
                parameter.CommonTempData = GetNewExpressionValue(item);
            }
            else if (ExpressionTool.GetMethodName(item) == "All" && !ExpressionTool.GetTopLevelMethodCalls(item).Contains("Subqueryable"))
            {
                parameter.CommonTempData = GetNewExpressionValue(item);
            }
            else
            {
                this.Start();
            }
            var sql = this.Context.DbMehtods.IIF(new MethodCallExpressionModel()
            {
                Args = new List<MethodCallExpressionArgs>() {
                          new MethodCallExpressionArgs() {
                               IsMember=true,
                               MemberName=parameter.CommonTempData.ObjToString()
                          },
                             new MethodCallExpressionArgs() {
                                IsMember=true,
                                MemberName=1
                          },
                          new MethodCallExpressionArgs() {
                               IsMember=true,
                               MemberName=0
                          }
                     }
            });
            parameter.Context.Result.Append(this.Context.GetAsString(asName, sql));
        }

        private string ResolveClass(ExpressionParameter parameter, Expression item, string asName)
        {
            var mappingKeys = GetMappingColumns(parameter.CurrentExpression);
            var isSameType = mappingKeys.Keys.Count > 0;
            this.Context.SugarContext.QueryBuilder.MappingKeys = mappingKeys;
            this.Expression = item;
            if (this.Context.IsJoin && (item is MemberInitExpression || item is NewExpression))
            {
                List<NewExpressionInfo> newExpressionInfos = new List<NewExpressionInfo>();
                if (item is MemberInitExpression)
                {
                    newExpressionInfos = ExpressionTool.GetNewexpressionInfos(item, this.Context, this);
                    var ignorePropertyNames = item.Type.GetProperties().Where(it => it.PropertyType.IsClass()&&!it.PropertyType.Name.StartsWith("System."))
                        .Select(it=>it.PropertyType.Name).ToList();
                    if (ignorePropertyNames.Count > 0)
                    {
                        var names = new List<string>() { };
                        foreach (MemberBinding binding in ((MemberInitExpression)item).Bindings)
                        {
                            names.Add(binding.Member.Name);
                        }
                        ignorePropertyNames = ignorePropertyNames.Where(it => !names.Contains(it)).ToList();
                        var q=this.Context?.SugarContext?.QueryBuilder;
                        if (q != null) 
                        {
                            foreach (var ignorePropertyName in ignorePropertyNames)
                            {
                                if (q.SelectNewIgnoreColumns == null) 
                                {
                                    q.SelectNewIgnoreColumns = new List<KeyValuePair<string, string>>();
                                }
                                var addItem = new KeyValuePair<string,string>(ignorePropertyName,item.Type.Name);
                                q.SelectNewIgnoreColumns.Add(addItem);
                            }
                        }
                    }
                }
                else
                {
                    newExpressionInfos = ExpressionTool.GetNewDynamicexpressionInfos(item, this.Context, this);
                } 
                foreach (NewExpressionInfo newExpressionInfo in newExpressionInfos)
                {
                    //var property=item.Type.GetProperties().Where(it => it.Name == newExpressionInfo.l).First();
                    //asName = GetAsName(item, newExpressionInfo.ShortName, property);
                    if (newExpressionInfo.Type == nameof(ConstantExpression))
                    {
                        parameter.Context.Result.Append(
                             newExpressionInfo.RightDbName + " AS " +
                              this.Context.SqlTranslationLeft + asName + "." + newExpressionInfo.LeftNameName + this.Context.SqlTranslationRight

                          );
                    }
                    else
                    {
                        parameter.Context.Result.Append(this.Context.GetAsString(
                           this.Context.SqlTranslationLeft + asName + "." + newExpressionInfo.LeftNameName + this.Context.SqlTranslationRight,
                        newExpressionInfo.ShortName + "." + newExpressionInfo.RightDbName
                      ));
                    }
                }
            }
            else if (!this.Context.IsJoin && (item is MemberInitExpression || item is NewExpression))
            {
                List<NewExpressionInfo> newExpressionInfos = new List<NewExpressionInfo>();
                if (item is MemberInitExpression)
                {
                    newExpressionInfos = ExpressionTool.GetNewexpressionInfos(item, this.Context, this);
                }
                else
                {
                    newExpressionInfos = ExpressionTool.GetNewDynamicexpressionInfos(item, this.Context, this);
                }
                //mappingKeys = new Dictionary<string, string>(); 
                foreach (NewExpressionInfo newExpressionInfo in newExpressionInfos)
                {
                    //var property=item.Type.GetProperties().Where(it => it.Name == newExpressionInfo.l).First();
                    //asName = GetAsName(item, newExpressionInfo.ShortName, property);
                    mappingKeys.Add("Single_" + newExpressionInfo.LeftNameName, asName + "." + newExpressionInfo.LeftNameName);
                    if (newExpressionInfo.Type == nameof(ConstantExpression))
                    {
                        this.Context.SugarContext.QueryBuilder.MappingKeys = mappingKeys;
                        parameter.Context.Result.Append($" {newExpressionInfo.RightDbName} AS {this.Context.SqlTranslationLeft}{asName}.{newExpressionInfo.LeftNameName}{this.Context.SqlTranslationRight}  ");
                    }
                    else
                    {
                        this.Context.SugarContext.QueryBuilder.MappingKeys = mappingKeys;
                        parameter.Context.Result.Append(this.Context.GetAsString(
                               this.Context.SqlTranslationLeft + asName + "." + newExpressionInfo.LeftNameName + this.Context.SqlTranslationRight,
                                newExpressionInfo.RightDbName
                          ));
                    }
                }
            }
            else if (IsExtSqlFuncObj(item))
            {
                var value = GetNewExpressionValue(item);
                parameter.Context.Result.Append($" {value} AS {asName} ");
            }
            else if (IsSubToList(item))
            {
                var value = GetNewExpressionValue(item);
                if (this.Context.SugarContext.QueryBuilder.SubToListParameters == null)
                    this.Context.SugarContext.QueryBuilder.SubToListParameters = new Dictionary<string, object>();
                if (!this.Context.SugarContext.QueryBuilder.SubToListParameters.ContainsKey(asName))
                {
                    this.Context.SugarContext.QueryBuilder.SubToListParameters.Add(asName, value);
                }
                //throw new Exception("子查询ToList开发中..");
            }
            else if (ExpressionTool.GetMethodName(item) == nameof(SqlFunc.MappingColumn)) 
            {
                var value = GetNewExpressionValue(item);
                parameter.Context.Result.Append($" {value} AS {asName} ");
            }
            else if (item is ConditionalExpression)
            {
                var value = GetNewExpressionValue(item);
                parameter.Context.Result.Append($" {value} AS {asName} ");
            }
            else if (ExpressionTool.GetMethodName(item)==nameof(SqlFunc.IIF))
            {
                var value = GetNewExpressionValue(item);
                parameter.Context.Result.Append($" {value} AS {asName} ");
            }
            else
            {
                asName = GetAsNameResolveAnObject(parameter, item, asName, isSameType);
            }

            return asName;
        }

        private void ResolveBinary(Expression item, string asName)
        {
            if (this.Context.Result.IsLockCurrentParameter == false)
            {
                var newContext = this.Context.GetCopyContextWithMapping();
                var resolveExpressType = this.Context.IsSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple;
                if (resolveExpressType == ResolveExpressType.WhereSingle&& item is BinaryExpression) 
                {
                    var binaryExp = (item as BinaryExpression);
                    if (ExpressionTool.ContainsMethodName(binaryExp, "Subquery")) 
                    {
                        resolveExpressType = ResolveExpressType.WhereMultiple;
                    }
                    if (this.Context.Expression!=null&&this.Context.SingleTableNameSubqueryShortName.IsNullOrEmpty())
                    {
                        this.Context.SingleTableNameSubqueryShortName = (this.Context.Expression as LambdaExpression)?.Parameters?.FirstOrDefault()?.Name;
                    }
                }
                newContext.Resolve(item, resolveExpressType);
                this.Context.Index = newContext.Index;
                this.Context.ParameterIndex = newContext.ParameterIndex;
                if (newContext.Parameters.HasValue())
                {
                    this.Context.Parameters.AddRange(newContext.Parameters);
                }
                if (ExpressionTool.IsEqualOrLtOrGt(item))
                {
                    var sql = newContext.Result.GetString();
                    var pTrue = AppendParameter(true);
                    var pFalse = AppendParameter(false);
                    sql =this.Context.DbMehtods.IIF(new MethodCallExpressionModel() {
                     Args=new List<MethodCallExpressionArgs>() 
                     { 
                         new MethodCallExpressionArgs(){ MemberName=sql,MemberValue=sql,IsMember=true } ,
                          new MethodCallExpressionArgs(){ MemberName=pTrue,MemberValue=pTrue,IsMember=true },
                           new MethodCallExpressionArgs(){ MemberName=pFalse,MemberValue=pFalse,IsMember=true }
                     }
                    });
                    this.Context.Result.Append(this.Context.GetAsString(asName, sql));
                }
                else
                {
                    this.Context.Result.Append(this.Context.GetAsString(asName, newContext.Result.GetString()));
                }
                this.Context.Result.CurrentParameter = null;
                if (this.Context.SingleTableNameSubqueryShortName.IsNullOrEmpty() && newContext.SingleTableNameSubqueryShortName.HasValue())
                {
                    this.Context.SingleTableNameSubqueryShortName = newContext.SingleTableNameSubqueryShortName;
                }
            }
        }

        private void ResolveUnaryExpConst(ExpressionParameter parameter, Expression item, string asName)
        {
            if (this.Context.Result.IsLockCurrentParameter == false)
            {
                this.Expression = ((UnaryExpression)item).Operand;
                this.Start();
                string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
                this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.CommonTempData));
            }
        }

        private void ResolveUnaryExpMem(ExpressionParameter parameter, Expression item, string asName)
        {
            if (this.Context.Result.IsLockCurrentParameter == false)
            {
                var expression = ((UnaryExpression)item).Operand as MemberExpression;
                var negateString = string.Empty;
                if (item.NodeType == ExpressionType.Negate)
                {
                    negateString = " -1*"; 
                }
                var isDateTimeNow = ((UnaryExpression)item).Operand.ToString() == "DateTime.Now";
                if (expression.Expression == null && !isDateTimeNow)
                {
                    this.Context.Result.CurrentParameter = parameter;
                    this.Context.Result.IsLockCurrentParameter = true;
                    parameter.IsAppendTempDate();
                    this.Expression = item;
                    this.Start();
                    parameter.IsAppendResult();
                    this.Context.Result.Append(negateString + this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
                    this.Context.Result.CurrentParameter = null;
                }
                else if (expression.Expression is ConstantExpression || isDateTimeNow)
                {
                    string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
                    this.Context.ParameterIndex++;
                    parameter.Context.Result.Append(negateString + this.Context.GetAsString(asName, parameterName));
                    this.Context.Parameters.Add(new SugarParameter(parameterName, ExpressionTool.GetMemberValue(expression.Member, expression)));
                }
                else
                {
                    this.Context.Result.CurrentParameter = parameter;
                    this.Context.Result.IsLockCurrentParameter = true;
                    parameter.IsAppendTempDate();
                    this.Expression = expression;
                    this.Start();
                    parameter.IsAppendResult();
                    this.Context.Result.Append(negateString + this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
                    this.Context.Result.CurrentParameter = null;
                }
            }
        }

        private void ResolveMemberOther(ExpressionParameter parameter, Expression item, string asName)
        {
            if (this.Context.Result.IsLockCurrentParameter == false)
            {
                this.Context.Result.CurrentParameter = parameter;
                this.Context.Result.IsLockCurrentParameter = true;
                parameter.IsAppendTempDate();
                this.Expression = item;
                if (IsBoolValue(item))
                {
                    this.Expression = (item as MemberExpression).Expression;
                }
                this.Start();
                parameter.IsAppendResult();
                var value = parameter.CommonTempData.ObjToString();
                if (this.Context?.SugarContext?.Context?.CurrentConnectionConfig?.MoreSettings?.IsCorrectErrorSqlParameterName == true)
                {
                    value = ExpressionTool.ResolveMemberValue(this.Context, item, value);
                }
                this.Context.Result.Append(this.Context.GetAsString2(asName, value));
                this.Context.Result.CurrentParameter = null;
            }
        }
         
        private void ResolveMemberConst(ExpressionParameter parameter, Expression item, string asName)
        {
            this.Expression = item;
            this.Start();
            string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
            this.Context.ParameterIndex++;
            parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
            this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.CommonTempData));
        }

        private void ResolveMember(ExpressionParameter parameter, Expression item, string asName)
        {
            var paramterValue = ExpressionTool.GetPropertyValue(item as MemberExpression);
            string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
            this.Context.ParameterIndex++;
            parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
            this.Context.Parameters.Add(new SugarParameter(parameterName, paramterValue));
        }

        private void ResolveConst(ExpressionParameter parameter, Expression item, string asName)
        {
            this.Expression = item;
            this.Start();
            string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
            this.Context.ParameterIndex++;
            parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
            this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.CommonTempData));
        }

    }
}
