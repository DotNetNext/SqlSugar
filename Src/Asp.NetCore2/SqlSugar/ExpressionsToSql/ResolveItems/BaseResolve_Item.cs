using System;
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
            this.Expression = item;
            this.Start();
            parameter.Context.Result.Append(this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
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

        private string ResolveClass(ExpressionParameter parameter, Expression item, string asName)
        {
            var mappingKeys = GetMappingColumns(parameter.CurrentExpression);
            var isSameType = mappingKeys.Keys.Count > 0;
            CallContextThread<Dictionary<string, string>>.SetData("Exp_Select_Mapping_Key", mappingKeys);
            CallContextAsync<Dictionary<string, string>>.SetData("Exp_Select_Mapping_Key", mappingKeys);
            this.Expression = item;
            if (this.Context.IsJoin && (item is MemberInitExpression || item is NewExpression))
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
                        CallContextThread<Dictionary<string, string>>.SetData("Exp_Select_Mapping_Key", mappingKeys);
                        CallContextAsync<Dictionary<string, string>>.SetData("Exp_Select_Mapping_Key", mappingKeys);
                        parameter.Context.Result.Append($" {newExpressionInfo.RightDbName} AS {this.Context.SqlTranslationLeft}{asName}.{newExpressionInfo.LeftNameName}{this.Context.SqlTranslationRight}  ");
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
                this.Context.Result.Append(this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
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
