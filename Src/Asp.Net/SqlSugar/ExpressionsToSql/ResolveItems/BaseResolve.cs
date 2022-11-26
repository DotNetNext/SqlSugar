using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public partial class BaseResolve
    {

        #region Append
        protected void AppendMember(ExpressionParameter parameter, bool? isLeft, object appendValue)
        {

            Context.ParameterIndex++;
            if (isLeft == true)
            {
                appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
            }
            if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue.ObjToString());
            }
            else
            {
                this.Context.Result.Append(appendValue);
            }
        }
        protected void AppendValue(ExpressionParameter parameter, bool? isLeft, object value)
        {
            if (parameter.BaseExpression is BinaryExpression || parameter.BaseExpression == null)
            {
                var oppoSiteExpression = isLeft == true ? parameter.BaseParameter.RightExpression : parameter.BaseParameter.LeftExpression;

                if (value is MapperSql)
                {
                    var sql = ((MapperSql)value).Sql;
                    if (isLeft == true)
                    {
                        sql += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
                    }
                    if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
                    {
                        this.Context.Result.Replace(ExpressionConst.FormatSymbol, sql);
                    }
                    else
                    {
                        this.Context.Result.Append(sql);
                    }
                }
                else if (parameter.CurrentExpression is MethodCallExpression || parameter.CurrentExpression is ConditionalExpression || parameter.CurrentExpression.NodeType == ExpressionType.Coalesce)
                {
                    var appendValue = value;
                    if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
                    {
                        this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue.ObjToString());
                    }
                    else
                    {
                        this.Context.Result.Append(appendValue);
                    }
                    this.AppendOpreator(parameter, isLeft);
                }
                else if (oppoSiteExpression is MemberExpression)
                {
                    string appendValue = Context.SqlParameterKeyWord
                        + ((MemberExpression)oppoSiteExpression).Member.Name
                        + Context.ParameterIndex;
                    if (IsNullValue(parameter, value)) 
                    {
                        appendValue = $" NULL ";
                        parameter.BaseParameter.ValueIsNull = true;
                    }
                    else if (value.ObjToString() != "NULL" && !parameter.ValueIsNull)
                    {
                        EntityColumnInfo columnInfo = GetColumnInfo(oppoSiteExpression);
                        if (columnInfo != null && columnInfo.SqlParameterDbType != null && columnInfo.SqlParameterDbType is System.Data.DbType)
                        {
                            var p = new SugarParameter(appendValue, value, (System.Data.DbType)columnInfo.SqlParameterDbType);
                            if (columnInfo.SqlParameterSize != null) 
                            {
                                p.Size = columnInfo.SqlParameterSize.ObjToInt();
                            }
                            this.Context.Parameters.Add(p);
                        }
                        else
                        {
                            this.Context.Parameters.Add(new SugarParameter(appendValue, value));
                        }
                    }
                    else
                    {
                        appendValue = value.ObjToString();
                    }
                    Context.ParameterIndex++;
                    appendValue = string.Format(" {0} ", appendValue);
                    if (isLeft == true)
                    {
                        appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
                    }
                    if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
                    {
                        this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue);
                    }
                    else
                    {
                        this.Context.Result.Append(appendValue);
                    }
                }
                else if ((oppoSiteExpression is UnaryExpression && (oppoSiteExpression as UnaryExpression).Operand is MemberExpression))
                {
                    string appendValue = Context.SqlParameterKeyWord
                      + ((MemberExpression)(oppoSiteExpression as UnaryExpression).Operand).Member.Name
                      + Context.ParameterIndex;
                    if (value.ObjToString() != "NULL" && !parameter.ValueIsNull)
                    {
                        value = this.Context.TableEnumIsString == true ? value.ToString() : value;
                        this.Context.Parameters.Add(new SugarParameter(appendValue, value));
                    }
                    else
                    {
                        appendValue = value.ObjToString();
                    }
                    Context.ParameterIndex++;
                    appendValue = string.Format(" {0} ", appendValue);
                    if (isLeft == true)
                    {
                        appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
                    }
                    if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
                    {
                        this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue);
                    }
                    else
                    {
                        this.Context.Result.Append(appendValue);
                    }
                }
                else
                {
                    var appendValue = this.Context.SqlParameterKeyWord + ExpressionConst.Const + Context.ParameterIndex;
                    Context.ParameterIndex++;
                    if (value != null && value.GetType().IsEnum())
                    {
                        if (this.Context.TableEnumIsString == true)
                        {
                            value = value.ToString();
                        }
                        else
                        {
                            value = Convert.ToInt64(value);
                        }
                    }
                    this.Context.Parameters.Add(new SugarParameter(appendValue, value));
                    appendValue = string.Format(" {0} ", appendValue);
                    if (isLeft == true)
                    {
                        appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
                    }
                    if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
                    {
                        this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue);
                    }
                    else
                    {
                        this.Context.Result.Append(appendValue);
                    }
                }
            }
        }
        protected void AppendOpreator(ExpressionParameter parameter, bool? isLeft)
        {
            if (isLeft == true)
            {
                this.Context.Result.Append(" " + ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index);
            }
        }
        protected string AppendParameter(object paramterValue)
        {
            string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
            this.Context.ParameterIndex++; ;
            this.Context.Parameters.Add(new SugarParameter(parameterName, paramterValue));
            return parameterName;
        }
        protected void AppendNot(object Value)
        {
            var isAppend = !this.Context.Result.Contains(ExpressionConst.FormatSymbol);
            var lastCharIsSpace = this.Context.Result.LastCharIsSpace;
            if (isAppend)
            {
                this.Context.Result.Append(lastCharIsSpace ? "NOT" : " NOT");
            }
            else
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, "NOT");
            }
        }
        protected void AppendNegate(object Value)
        {
            var isAppend = !this.Context.Result.Contains(ExpressionConst.FormatSymbol);
            var lastCharIsSpace = this.Context.Result.LastCharIsSpace;
            if (isAppend)
            {
                this.Context.Result.Append(lastCharIsSpace ? "-" : " -");
            }
            else
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, "-");
            }
        }
        #endregion

        #region New Expression

        public string GetNewExpressionValue(Expression item)
        {
            var newContext = this.Context.GetCopyContextWithMapping();
            newContext.SugarContext = this.Context.SugarContext;
            newContext.Resolve(item, this.Context.IsJoin ? ResolveExpressType.WhereMultiple : ResolveExpressType.WhereSingle);
            this.Context.Index = newContext.Index;
            this.Context.ParameterIndex = newContext.ParameterIndex;
            if (newContext.Parameters.HasValue())
            {
                this.Context.Parameters.AddRange(newContext.Parameters);
            }
            if (this.Context.SingleTableNameSubqueryShortName == "Subqueryable()")
            {
                this.Context.SingleTableNameSubqueryShortName = newContext.SingleTableNameSubqueryShortName;
            }
            else if (newContext.SingleTableNameSubqueryShortName!=null&& newContext.Result !=null && newContext.Result.Contains(this.Context.SqlTranslationLeft+ newContext.SingleTableNameSubqueryShortName+ this.Context.SqlTranslationRight))
            {
                this.Context.SingleTableNameSubqueryShortName = newContext.SingleTableNameSubqueryShortName;
            }
            return newContext.Result.GetResultString();
        }
        public string GetNewExpressionValue(Expression item, ResolveExpressType type)
        {
            var newContext = this.Context.GetCopyContextWithMapping();
            newContext.SugarContext = this.Context.SugarContext;
            newContext.Resolve(item, type);
            this.Context.Index = newContext.Index;
            this.Context.ParameterIndex = newContext.ParameterIndex;
            if (newContext.Parameters.HasValue())
            {
                this.Context.Parameters.AddRange(newContext.Parameters);
            }
            return newContext.Result.GetResultString();
        }
        protected void ResolveNewExpressions(ExpressionParameter parameter, Expression item, string asName)
        {
            if (item is ConstantExpression)
            {
                this.Expression = item;
                this.Start();
                string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
                this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.CommonTempData));
            }
            else if ((item is MemberExpression) && ((MemberExpression)item).Expression == null)
            {
                var paramterValue = ExpressionTool.GetPropertyValue(item as MemberExpression);
                string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
                this.Context.Parameters.Add(new SugarParameter(parameterName, paramterValue));
            }
            else if ((item is MemberExpression) && ((MemberExpression)item).Expression.NodeType == ExpressionType.Constant)
            {
                this.Expression = item;
                this.Start();
                string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
                this.Context.Parameters.Add(new SugarParameter(parameterName, parameter.CommonTempData));
            }
            else if (item is MemberExpression)
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
                    this.Context.Result.Append(this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
                    this.Context.Result.CurrentParameter = null;
                }
            }
            else if (item is UnaryExpression && ((UnaryExpression)item).Operand is MemberExpression)
            {
                if (this.Context.Result.IsLockCurrentParameter == false)
                {
                    var expression = ((UnaryExpression)item).Operand as MemberExpression;
                    var isDateTimeNow = ((UnaryExpression)item).Operand.ToString() == "DateTime.Now";
                    if (expression.Expression == null && !isDateTimeNow)
                    {
                        this.Context.Result.CurrentParameter = parameter;
                        this.Context.Result.IsLockCurrentParameter = true;
                        parameter.IsAppendTempDate();
                        this.Expression = item;
                        this.Start();
                        parameter.IsAppendResult();
                        this.Context.Result.Append(this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
                        this.Context.Result.CurrentParameter = null;
                    }
                    else if (expression.Expression is ConstantExpression || isDateTimeNow)
                    {
                        string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
                        this.Context.ParameterIndex++;
                        parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
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
                        this.Context.Result.Append(this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
                        this.Context.Result.CurrentParameter = null;
                    }
                }
            }
            else if (item is UnaryExpression && ((UnaryExpression)item).Operand is ConstantExpression)
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
            else if (item is BinaryExpression)
            {
                if (this.Context.Result.IsLockCurrentParameter == false)
                {
                    var newContext = this.Context.GetCopyContextWithMapping();
                    var resolveExpressType = this.Context.IsSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple;
                    newContext.Resolve(item, resolveExpressType);
                    this.Context.Index = newContext.Index;
                    this.Context.ParameterIndex = newContext.ParameterIndex;
                    if (newContext.Parameters.HasValue())
                    {
                        this.Context.Parameters.AddRange(newContext.Parameters);
                    }
                    this.Context.Result.Append(this.Context.GetAsString(asName, newContext.Result.GetString()));
                    this.Context.Result.CurrentParameter = null;
                    if (this.Context.SingleTableNameSubqueryShortName.IsNullOrEmpty() && newContext.SingleTableNameSubqueryShortName.HasValue())
                    {
                        this.Context.SingleTableNameSubqueryShortName = newContext.SingleTableNameSubqueryShortName;
                    }
                }
            }
            else if (item.Type.IsClass())
            {
                var mappingKeys = GetMappingColumns(parameter.CurrentExpression);
                var isSameType = mappingKeys.Keys.Count > 0;
                CallContextThread<Dictionary<string, string>>.SetData("Exp_Select_Mapping_Key", mappingKeys);
                CallContextAsync<Dictionary<string, string>>.SetData("Exp_Select_Mapping_Key", mappingKeys);
                this.Expression = item;
                if (this.Context.IsJoin&& (item is MemberInitExpression|| item is NewExpression))
                {
                    List<NewExpressionInfo> newExpressionInfos = new List<NewExpressionInfo>();
                    if (item is MemberInitExpression)
                    {
                        newExpressionInfos = ExpressionTool.GetNewexpressionInfos(item,this.Context,this);
                    }
                    else 
                    {
                        newExpressionInfos = ExpressionTool.GetNewDynamicexpressionInfos(item, this.Context,this);
                    }
                    foreach (NewExpressionInfo newExpressionInfo in newExpressionInfos) 
                    {
                        //var property=item.Type.GetProperties().Where(it => it.Name == newExpressionInfo.l).First();
                        //asName = GetAsName(item, newExpressionInfo.ShortName, property);
                        if (newExpressionInfo.Type == nameof(ConstantExpression))
                        {
                            parameter.Context.Result.Append(
                                 newExpressionInfo.RightDbName +" AS "+
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
                        newExpressionInfos = ExpressionTool.GetNewexpressionInfos(item, this.Context,this);
                    }
                    else
                    {
                        newExpressionInfos = ExpressionTool.GetNewDynamicexpressionInfos(item, this.Context,this);
                    }
                    //mappingKeys = new Dictionary<string, string>(); 
                    foreach (NewExpressionInfo newExpressionInfo in newExpressionInfos)
                    {
                        //var property=item.Type.GetProperties().Where(it => it.Name == newExpressionInfo.l).First();
                        //asName = GetAsName(item, newExpressionInfo.ShortName, property);
                        mappingKeys.Add("Single_" + newExpressionInfo.LeftNameName, asName + "." + newExpressionInfo.LeftNameName);
                        if (newExpressionInfo.Type == nameof(ConstantExpression))
                        {
                            CallContextThread<Dictionary<string, string>>.SetData("Exp_Select_Mapping_Key", mappingKeys);
                            CallContextAsync<Dictionary<string, string>>.SetData("Exp_Select_Mapping_Key", mappingKeys);
                            parameter.Context.Result.Append($" {newExpressionInfo.RightDbName} AS { this.Context.SqlTranslationLeft}{asName}.{newExpressionInfo.LeftNameName}{ this.Context.SqlTranslationRight}  ");
                        }
                        else
                        {
                            CallContextThread<Dictionary<string, string>>.SetData("Exp_Select_Mapping_Key", mappingKeys);
                            CallContextAsync<Dictionary<string, string>>.SetData("Exp_Select_Mapping_Key", mappingKeys);
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
                else
                {
                    asName = GetAsNameResolveAnObject(parameter, item, asName, isSameType);
                }
            }
            else if (item.Type == UtilConstants.BoolType && item is MethodCallExpression && IsNotCaseExpression(item))
            {
                this.Expression = item;
                this.Start();
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
            else if (item.NodeType == ExpressionType.Not
                && (item as UnaryExpression).Operand is MethodCallExpression
                && ((item as UnaryExpression).Operand as MethodCallExpression).Method.Name.IsIn("IsNullOrEmpty", "IsNullOrWhiteSpace"))
            {
                var asValue = GetAsNamePackIfElse(GetNewExpressionValue(item)).ObjToString();
                parameter.Context.Result.Append(this.Context.GetAsString(asName, asValue));
            }
            else if (item is MethodCallExpression && (item as MethodCallExpression).Method.Name.IsIn("Count", "Any") && !item.ToString().StartsWith("Subqueryable"))
            {
                if (this.Context.IsSingle && this.Context.SingleTableNameSubqueryShortName == null)
                {
                    this.Context.SingleTableNameSubqueryShortName = item.ToString().Split('.').First();
                }
                parameter.Context.Result.Append(this.Context.GetAsString(asName, GetNewExpressionValue(item)));
            }
            else if (item is MethodCallExpression || item is UnaryExpression || item is ConditionalExpression || item.NodeType == ExpressionType.Coalesce)
            {
                this.Expression = item;
                this.Start();
                parameter.Context.Result.Append(this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
            }
            else
            {
                Check.ThrowNotSupportedException(item.GetType().Name);
            }
        }

        #endregion

    }
}
