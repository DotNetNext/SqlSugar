using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    ///MethodCall Helper
    /// </summary>
    public partial class MethodCallExpressionResolve : BaseResolve
    {
        private void CusMethod(ExpressionParameter parameter, MethodCallExpression express, bool? isLeft)
        {
            try
            {
                OneToManyNavgateExpression nav = new OneToManyNavgateExpression(this.Context?.SugarContext?.Context, this);
                nav.ParameterIndex = this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                if (nav.IsNavgate(express))
                {
                    var sql = nav.GetSql();
                    SetNavigateResult();
                    this.Context.SingleTableNameSubqueryShortName = nav.ShorName;
                    base.AppendValue(parameter, isLeft, sql);
                    return;
                }

                OneToManyNavgateExpressionN nav2 = new OneToManyNavgateExpressionN(this.Context?.SugarContext?.Context, this);
                if (nav2.IsNavgate(express))
                {
                    var sql = nav2.GetSql();
                    SetNavigateResult();
                    this.Context.SingleTableNameSubqueryShortName = nav2.shorName;
                    base.AppendValue(parameter, isLeft, sql);
                    return;
                }

                var constValue = ExpressionTool.DynamicInvoke(express);
                if (constValue is MapperSql)
                {
                    constValue = (constValue as MapperSql).Sql;
                    base.AppendValue(parameter, isLeft, constValue);
                    return;
                }
                parameter.BaseParameter.CommonTempData = constValue;
                var parameterName = base.AppendParameter(constValue);
                if (parameter.BaseParameter.CommonTempData != null && parameter.BaseParameter.CommonTempData.Equals(CommonTempDataType.Result))
                {
                    this.Context.Result.Append(parameterName);
                }
                else
                {
                    base.AppendValue(parameter, isLeft, parameterName);
                }
            }
            catch (Exception ex)
            {
                if (ex is SqlSugarException)
                {
                    Check.Exception(true, string.Format(ex.Message, express.Method.Name));
                }
                else
                {
                    Check.Exception(true, string.Format(ErrorMessage.MethodError, express.Method.Name));
                }
            }
        }
        private static bool MethodValueIsTrue(object methodValue)
        {
            return methodValue != null && methodValue.ToString().Contains("THEN true  ELSE false END");
        }
        private object packIfElse(object methodValue)
        { 
            methodValue = this.Context.DbMehtods.CaseWhen(new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("IF",methodValue.ObjToString()),
                    new KeyValuePair<string, string>("Return", this.Context.DbMehtods.TrueValue()),
                    new KeyValuePair<string, string>("End", this.Context.DbMehtods.FalseValue())
                 });
            return methodValue;
        }
        private void SetShortName(Expression exp)
        {
            var lamExp = (exp as LambdaExpression);
            if (lamExp.Parameters != null && lamExp.Parameters.Count == 1)
            {
                if (this.Context.SingleTableNameSubqueryShortName == null)
                {
                    this.Context.SingleTableNameSubqueryShortName = lamExp.Parameters.First().Name;
                }
            }
        }


        private void AppendItem(ExpressionParameter parameter, string name, IEnumerable<Expression> args, MethodCallExpressionModel model, Expression item)
        {
            if (ExpressionTool.IsUnConvertExpress(item))
            {
                item = (item as UnaryExpression).Operand;
            }
            if (this.Context.IsSingle && args.Any(it => ExpressionTool.IsSubQuery(it)) && base.BaseParameter?.BaseParameter?.BaseParameter?.CurrentExpression != null)
            {
                var exp = base.BaseParameter?.BaseParameter?.BaseParameter?.CurrentExpression;
                if (exp is LambdaExpression)
                {
                    SetShortName(exp);
                }
                else if (exp is UnaryExpression)
                {
                    exp = base.BaseParameter?.BaseParameter?.BaseParameter?.BaseParameter?.CurrentExpression;
                    if (exp is LambdaExpression)
                    {
                        SetShortName(exp);
                    }
                }
            }
            else if (this.Context.IsSingle && args.Any(it => ExpressionTool.IsIsNullSubQuery(it)))
            {
                var exp = base.BaseParameter?.BaseParameter?.BaseParameter?.CurrentExpression;
                if (exp is LambdaExpression)
                {
                    SetShortName(exp);
                }
                else if (exp is UnaryExpression)
                {
                    exp = base.BaseParameter?.BaseParameter?.BaseParameter?.BaseParameter?.CurrentExpression;
                    if (exp is LambdaExpression)
                    {
                        SetShortName(exp);
                    }
                }
            }
            var isBinaryExpression = item is BinaryExpression || item is MethodCallExpression;
            var isConst = item is ConstantExpression;
            var isIIF = name == "IIF";
            var isSubIIF = (isIIF && item.ToString().StartsWith("IIF"));
            var isIFFBoolMember = isIIF && (item is MemberExpression) && (item as MemberExpression).Type == UtilConstants.BoolType;
            var isIFFUnary = isIIF && (item is UnaryExpression) && (item as UnaryExpression).Operand.Type == UtilConstants.BoolType;
            var isIFFBoolBinary = isIIF && (item is BinaryExpression) && (item as BinaryExpression).Type == UtilConstants.BoolType;
            var isIFFBoolMethod = isIIF && (item is MethodCallExpression) && (item as MethodCallExpression).Type == UtilConstants.BoolType;
            var isFirst = item == args.First();
            var isBoolValue = item.Type == UtilConstants.BoolType && item.ToString().StartsWith("value(");
            var isLength =ExpressionTool.GetIsLength(item);
            if (isFirst && isIIF && isConst)
            {
                var value = (item as ConstantExpression).Value.ObjToBool() ? this.Context.DbMehtods.True() : this.Context.DbMehtods.False();
                var methodCallExpressionArgs = new MethodCallExpressionArgs()
                {
                    IsMember = true,
                    MemberName = value,
                    MemberValue = value
                };
                model.Args.Add(methodCallExpressionArgs);
            }
            else if (name!=null && name != "MappingColumn" && !name.StartsWith("Row") &&ExpressionTool.GetMethodName(item)== "Format" && ExpressionTool.GetParameters(item).Count==0) 
            {
                var value =  ExpressionTool.DynamicInvoke(item);
                var p = AppendParameter(value);
                var methodCallExpressionArgs = new MethodCallExpressionArgs()
                {
                    IsMember = true,
                    MemberName = p,
                    MemberValue = p
                };
                model.Args.Add(methodCallExpressionArgs);
            }
            else if (isLength)
            {
                var sql = GetNewExpressionValue(item);
                var value = this.Context.DbMehtods.Length(new MethodCallExpressionModel()
                {
                    Name = "Length",
                    Args = new List<MethodCallExpressionArgs>() {
                     new MethodCallExpressionArgs(){
                       IsMember=true,
                       MemberName=sql,
                       MemberValue=sql
                     }
                   }
                });
                var methodCallExpressionArgs = new MethodCallExpressionArgs()
                {
                    IsMember = true,
                    MemberName = value,
                    MemberValue = value
                };
                model.Args.Add(methodCallExpressionArgs);
            }
            else if (isIFFUnary && !isFirst)
            {
                AppendModelByIIFMember(parameter, model, (item as UnaryExpression).Operand);
            }
            else if (isIFFBoolMember && !isFirst)
            {
                AppendModelByIIFMember(parameter, model, item);

            }
            else if (isIFFBoolBinary && !isFirst)
            {
                var binaryExp = item as BinaryExpression;
                var binaryExpEqual = binaryExp != null && ExpressionTool.IsComparisonOperatorBool(binaryExp);
                if (binaryExpEqual)
                {
                    var expValue = GetNewExpressionValue(item);
                    expValue = this.Context.DbMehtods.IIF(new MethodCallExpressionModel()
                    {
                        Name = "IIF",
                        Args = new List<MethodCallExpressionArgs>()
                        {
                             new MethodCallExpressionArgs(){
                               IsMember=true,
                               MemberName=expValue
                             },
                             new MethodCallExpressionArgs(){
                               IsMember=true,
                               MemberName= Context.DbMehtods.TrueValue()
                             },
                             new MethodCallExpressionArgs(){
                               IsMember=true,
                               MemberName= Context.DbMehtods.FalseValue()
                             }
                        }
                    });
                    model.Args.Add(new MethodCallExpressionArgs()
                    {
                        IsMember = false,
                        MemberName = expValue,
                        MemberValue = expValue
                    });
                }
                else
                {
                    AppendModelByIIFBinary(parameter, model, item);
                }

            }
            else if (isIFFBoolMethod && !isFirst)
            {
                AppendModelByIIFMethod(parameter, model, item);
            }
            else if (isBinaryExpression)
            {
                model.Args.Add(GetMethodCallArgs(parameter, item, name));
            }
            else if (isSubIIF)
            {
                model.Args.Add(GetMethodCallArgs(parameter, item));
            }
            else if (isBoolValue && !isIIF && item is MemberExpression)
            {
                model.Args.Add(GetMethodCallArgs(parameter, (item as MemberExpression).Expression));
            }
            else if (isBoolValue && isIIF && item is MemberExpression)
            {
                var argItem = GetMethodCallArgs(parameter, (item as MemberExpression).Expression);
                if (argItem.IsMember)
                {
                    var pName = this.Context.SqlParameterKeyWord + "true_0";
                    if (!this.Context.Parameters.Any(it => it.ParameterName == pName))
                        this.Context.Parameters.Add(new SugarParameter(pName, true));
                    argItem.MemberName = $" {argItem.MemberName}={pName} ";
                }
                model.Args.Add(argItem);
            }
            else if (name.IsIn("ListAny","ListAll") && item is LambdaExpression)
            {
                var sql = GetNewExpressionValue(item, ResolveExpressType.WhereMultiple);
                var lamExp = (item as LambdaExpression);
                var pExp = lamExp.Parameters[0];
                var pname = pExp.Name;
                model.Args.Add(new MethodCallExpressionArgs()
                {
                    MemberValue = new ListAnyParameter()
                    {
                        Sql = sql,
                        Name = pname,
                        Columns = this.Context.SugarContext.Context.EntityMaintenance.GetEntityInfo(pExp.Type).Columns,
                        ConvetColumnFunc = this.Context.GetTranslationColumnName
                    }
                });
                if (this.Context.IsSingle && this.Context.SingleTableNameSubqueryShortName == null)
                {
                    ParameterExpressionVisitor visitor = new ParameterExpressionVisitor();
                    visitor.Visit(lamExp);
                    var tableParamter = visitor.Parameters.FirstOrDefault(it => it.Name != pname);
                    if (tableParamter != null)
                    {
                        this.Context.SingleTableNameSubqueryShortName = tableParamter.Name;
                    }
                }
            }
            else
            {
                AppendModel(parameter, model, item, name, args);
            }
        }


        private void AppendModelByIIFMember(ExpressionParameter parameter, MethodCallExpressionModel model, Expression item)
        {
            parameter.CommonTempData = CommonTempDataType.Result;
            base.Expression = item;
            base.Start();
            var methodCallExpressionArgs = new MethodCallExpressionArgs()
            {
                IsMember = parameter.ChildExpression is MemberExpression,
                MemberName = parameter.CommonTempData
            };
            if (methodCallExpressionArgs.IsMember && parameter.ChildExpression != null && parameter.ChildExpression.ToString() == "DateTime.Now")
            {
                methodCallExpressionArgs.IsMember = false;
            }
            var value = methodCallExpressionArgs.MemberName;
            if (methodCallExpressionArgs.IsMember)
            {
                var childExpression = parameter.ChildExpression as MemberExpression;
                if (childExpression.Expression != null && childExpression.Expression is ConstantExpression)
                {
                    methodCallExpressionArgs.IsMember = false;
                }
            }
            if (methodCallExpressionArgs.IsMember == false)
            {
                var parameterName = this.Context.SqlParameterKeyWord + ExpressionConst.MethodConst + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                methodCallExpressionArgs.MemberName = parameterName;
                methodCallExpressionArgs.MemberValue = value;
                this.Context.Parameters.Add(new SugarParameter(parameterName, value));
            }
            model.Args.Add(methodCallExpressionArgs);
            parameter.ChildExpression = null;
        }
        private void AppendModelByIIFBinary(ExpressionParameter parameter, MethodCallExpressionModel model, Expression item)
        {
            Check.Exception(true, "The SqlFunc.IIF(arg1,arg2,arg3) , {0} argument  do not support ", item.ToString());
        }
        private void AppendModelByIIFMethod(ExpressionParameter parameter, MethodCallExpressionModel model, Expression item)
        {
            var methodExpression = item as MethodCallExpression;
            if (methodExpression.Method.Name.IsIn("ToBool", "ToBoolean", "IIF"))
            {
                model.Args.Add(base.GetMethodCallArgs(parameter, item));
            }
            else if (methodExpression.Method.Name.IsIn("Contains", "EndsWith", "StartsWith"))
            {
                Expression conditionalExpression =ExpressionTool.GetConditionalExpression(item); 
                model.Args.Add(base.GetMethodCallArgs(parameter, conditionalExpression));
            }
            else
            {
                Check.Exception(true, "The SqlFunc.IIF(arg1,arg2,arg3) , {0} argument  do not support ", item.ToString());
            }
        }

        private void AppendModel(ExpressionParameter parameter, MethodCallExpressionModel model, Expression item,string name, IEnumerable<Expression> args)
        {
            parameter.CommonTempData = CommonTempDataType.Result;
            base.Expression = item;
            var isRemoveParamter = false;
            var isNegate = false;
            if (item.Type == UtilConstants.DateType && parameter.CommonTempData.ObjToString() == CommonTempDataType.Result.ToString() && item.ToString() == "DateTime.Now.Date")
            {
                parameter.CommonTempData = DateTime.Now.Date;
            }
            else if (item is ConditionalExpression)
            {
                parameter.CommonTempData = GetNewExpressionValue(item);
            }
            else if (IsNot(item))
            {
                parameter.CommonTempData = GetNewExpressionValue(item);
            }
            else if (IsDateDate(item))
            {
                parameter.CommonTempData = GetNewExpressionValue(item);
            }
            else if (IsDateValue(item))
            {
                parameter.CommonTempData = GetNewExpressionValue(item);
            }
            else if (model.Name == "ToString" && item is ConstantExpression && (item as ConstantExpression).Type.IsEnum())
            {
                parameter.CommonTempData = item.ToString();
            }
            else if (IsDateItemValue(item))
            {
                parameter.CommonTempData = GetNewExpressionValue(item);
            }
            else if (name == "Format" && item is NewArrayExpression)
            {
                var exps = (item as NewArrayExpression).Expressions;
                parameter.CommonTempData = exps.Select(it =>
                {
                    var res = GetNewExpressionValue(ExpressionTool.RemoveConvert(it));
                    return res;
                }).ToArray();
            }
            else if (name == "Format" && item is ConstantExpression)
            {
                parameter.CommonTempData = ExpressionTool.GetExpressionValue(item);
            }
            else if (name == "FullTextContains" && item is NewArrayExpression) 
            {
                var array = ExpressionTool.GetNewArrayMembers(item as NewArrayExpression);
                parameter.CommonTempData = array.Select(it=>this.Context.GetTranslationColumnName(it)).ToList();
                isRemoveParamter = true;
            }
            else if (ExpressionTool.IsNegate(item) && (item as UnaryExpression)?.Operand is MemberExpression)
            {
                var exp = (item as UnaryExpression)?.Operand;
                parameter.CommonTempData = GetNewExpressionValue(exp) + " * -1 ";
                isRemoveParamter = true;
                isNegate = true;
            }
            else
            {
                base.Start();
            }
            var methodCallExpressionArgs = new MethodCallExpressionArgs()
            {
                IsMember = parameter.ChildExpression is MemberExpression && !ExpressionTool.IsConstExpression(parameter.ChildExpression as MemberExpression),
                MemberName = parameter.CommonTempData
            };
            if (methodCallExpressionArgs.MemberName is MapperSql)
            {
                methodCallExpressionArgs.MemberName = (methodCallExpressionArgs.MemberName as MapperSql).Sql;
            }
            if (methodCallExpressionArgs.IsMember && parameter.ChildExpression != null && parameter.ChildExpression.ToString() == "DateTime.Now")
            {
                methodCallExpressionArgs.IsMember = false;
            }
            var value = methodCallExpressionArgs.MemberName;
            if (methodCallExpressionArgs.IsMember)
            {
                var childExpression = parameter.ChildExpression as MemberExpression;
                if (childExpression.Expression != null && childExpression.Expression is ConstantExpression)
                {
                    methodCallExpressionArgs.IsMember = false;
                }
            }
            if (IsDateDate(item) || IsDateValue(item) || IsDateItemValue(item) || item is ConditionalExpression || IsNot(item))
            {
                methodCallExpressionArgs.IsMember = true;
            }
            if (methodCallExpressionArgs.IsMember == false && (item is MethodCallExpression && item.ToString() == "GetDate()") || (item is UnaryExpression && ((UnaryExpression)item).Operand.ToString() == "GetDate()"))
            {
                var parameterName = this.Context.SqlParameterKeyWord + ExpressionConst.MethodConst + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                methodCallExpressionArgs.MemberName = value;
                methodCallExpressionArgs.MemberValue = null;
            }
            else if (methodCallExpressionArgs.IsMember == false&&isNegate==false)
            {
                var parameterName = this.Context.SqlParameterKeyWord + ExpressionConst.MethodConst + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                methodCallExpressionArgs.MemberName = parameterName;
                if (name == "ToString"&&UtilMethods.GetUnderType(base.Expression.Type).IsEnum())
                {
                    value = value?.ToString();
                }
                else if (name == "ContainsArray"&&args.Count()==2&& value!= null && value is IList) 
                {
                    List<object> result = new List<object>();
                    foreach (var memItem in (value as IList))
                    {
                        result.Add(GetMemberValue(memItem, args.Last()));
                    }
                    value = result;
                }
                else if (name.IsIn("Contains", "StartsWith", "EndsWith") &&item==args.Last()&& ExpressionTool.IsSqlParameterDbType(this.Context, args.First()))
                {
                    var myvalue = ExpressionTool.DynamicInvoke(args.Last());
                    var parametre = ExpressionTool.GetParameterBySqlParameterDbType(this.Context.ParameterIndex,myvalue, this.Context, args.First());
                    this.Context.Parameters.Add(parametre);
                    methodCallExpressionArgs.MemberName = parametre.ParameterName;
                    methodCallExpressionArgs.MemberValue = parametre.Value;
                    methodCallExpressionArgs.IsMember = true;
                    isRemoveParamter = true;
                    this.Context.ParameterIndex++;
                }
                methodCallExpressionArgs.MemberValue = value;
                if (isRemoveParamter != true)
                {
                    if (value == null && item != null)
                    {
                        this.Context.Parameters.Add(new SugarParameter(parameterName, value, UtilMethods.GetUnderType(item.Type)));
                    }
                    else
                    {
                        this.Context.Parameters.Add(new SugarParameter(parameterName, value));
                    }
                }
            } 
            model.Args.Add(methodCallExpressionArgs);
            parameter.ChildExpression = null;
        }


        private void GetConfigValue(MethodCallExpression express, ExpressionParameter parameter)
        {
            var exp = express.Arguments[0];
            var name = Regex.Match(express.Method.ToString(), @"GetConfigValue\[(.+)\]").Groups[1].Value;
            string code = null;
            if (express.Arguments.Count > 1)
            {
                code = ExpressionTool.GetExpressionValue(express.Arguments[1]) + "";
            }
            var entityDb = SqlFuncExtendsion.TableInfos.FirstOrDefault(y => y.Type.Name == name && y.Code == code);
            Check.Exception(entityDb == null, string.Format("GetConfigValue no configuration  Entity={0} UniqueCode={1}", name, code));
            var entity = new ConfigTableInfo()
            {
                Code = entityDb.Code,
                TableName = entityDb.TableName,
                Key = entityDb.Key,
                Parameter = new List<SugarParameter>(),
                Type = entityDb.Type,
                Value = entityDb.Value,
                Where = entityDb.Where
            };
            if (entityDb.Parameter != null && entityDb.Parameter.Any())
            {
                foreach (var item in entityDb.Parameter)
                {
                    entity.Parameter.Add(new SugarParameter("", null)
                    {
                        DbType = item.DbType,
                        Direction = item.Direction,
                        IsArray = item.IsArray,
                        IsJson = item.IsJson,
                        IsNullable = item.IsNullable,
                        IsRefCursor = item.IsRefCursor,
                        ParameterName = item.ParameterName,
                        Size = item.Size,
                        SourceColumn = item.SourceColumn,
                        SourceColumnNullMapping = item.SourceColumnNullMapping,
                        SourceVersion = item.SourceVersion,
                        TempDate = item.TempDate,
                        TypeName = item.TypeName,
                        Value = item.Value,
                        _Size = item._Size

                    });
                }
            }
            string sql = " (SELECT {0} FROM {1} WHERE {2}={3}";
            if (ExpressionTool.IsUnConvertExpress(exp))
            {
                exp = (exp as UnaryExpression).Operand;
            }
            var member = exp as MemberExpression;
            var it = member.Expression;
            var type = it.Type;
            var properyName = member.Member.Name;
            var eqName = string.Format("{0}.{1}", this.Context.GetTranslationColumnName(it.ToString()), this.Context.GetDbColumnName(type.Name, properyName));
            if (this.Context.IsSingle)
            {
                this.Context.SingleTableNameSubqueryShortName = it.ToString();
            }
            sql = string.Format(sql, entity.Value, this.Context.GetTranslationColumnName(entity.TableName), entity.Key, eqName);
            if (entity.Parameter != null)
            {
                foreach (var item in entity.Parameter)
                {
                    var oldName = item.ParameterName;
                    item.ParameterName = Regex.Split(oldName, "_con_").First() + "_con_" + this.Context.ParameterIndex;
                    entity.Where = entity.Where.Replace(oldName, item.ParameterName);
                }
                this.Context.ParameterIndex++;
                this.Context.Parameters.AddRange(entity.Parameter);
            }
            if (entity.Where.HasValue())
            {
                sql += " AND " + entity.Where;
            }
            sql += " )";
            if (this.Context.ResolveType.IsIn(ResolveExpressType.SelectMultiple, ResolveExpressType.SelectSingle, ResolveExpressType.Update))
            {
                parameter.BaseParameter.CommonTempData = sql;
            }
            else
            {
                AppendMember(parameter, parameter.IsLeft, sql);
            }
        }
        private object GetMethodValue(string name, MethodCallExpressionModel model)
        {
            model.Parameters = this.Context.Parameters;
            if (IsExtMethod(name))
            {
                model.Expression = this.Expression;
                model.BaseExpression = this.BaseParameter.CurrentExpression;
                DbType type = DbType.SqlServer;
                if (this.Context is SqlServerExpressionContext)
                    type = DbType.SqlServer;
                else if (this.Context is MySqlExpressionContext)
                    type = DbType.MySql;
                else if (this.Context is SqliteExpressionContext)
                    type = DbType.Sqlite;
                else if (this.Context is OracleExpressionContext)
                    type = DbType.Oracle;
                else if (this.Context is PostgreSQLExpressionContext)
                    type = DbType.PostgreSQL;
                else if (this.Context.GetType().Name.StartsWith("MySql"))
                {
                    type = DbType.MySql;
                }
                else
                {
                    type = GetType(this.Context.GetType().Name);
                }
                return this.Context.SqlFuncServices.First(it => it.UniqueMethodName == name).MethodValue(model, type, this.Context);
            }
            else
            {
                if (name == "Parse" && TempParseType.IsIn(UtilConstants.GuidType) && model.Args != null && model.Args.Count() > 1)
                {
                    name = "Equals";
                }
                else if (name == "Parse")
                {
                    name = "To" + TempParseType.Name;
                }
                else if (name == "IsNullOrWhiteSpace")
                {
                    name = "IsNullOrEmpty";
                }
                switch (name)
                {
                    case "IIF":
                        return this.Context.DbMehtods.IIF(model);
                    case "HasNumber":
                        return this.Context.DbMehtods.HasNumber(model);
                    case "HasValue":
                        return this.Context.DbMehtods.HasValue(model);
                    case "IsNullOrEmpty":
                        return this.Context.DbMehtods.IsNullOrEmpty(model);
                    case "ToLower":
                        return this.Context.DbMehtods.ToLower(model);
                    case "ToUpper":
                        return this.Context.DbMehtods.ToUpper(model);
                    case "Trim":
                        return this.Context.DbMehtods.Trim(model);
                    case "Contains":
                        return this.Context.DbMehtods.Contains(model);
                    case "ContainsArray":
                        if (model.Args[0].MemberValue == null)
                        {
                            var first = this.Context.Parameters.FirstOrDefault(it => it.ParameterName == model.Args[0].MemberName.ObjToString());
                            if (first.HasValue())
                            {
                                model.Args[0].MemberValue = first.Value;
                            }
                        }
                        if (this.Context.TableEnumIsString == true)
                        {
                            List<string> enumStringList = new List<string>();
                            foreach (var inItem in (model.Args[0].MemberValue as IEnumerable))
                            {
                                if (inItem != null)
                                {
                                    if (UtilMethods.GetUnderType(inItem.GetType()).IsEnum())
                                    {
                                        enumStringList.Add(inItem.ToString());
                                    }
                                }
                            }
                            if (enumStringList.Any())
                            {
                                model.Args[0].MemberValue = enumStringList;
                            }
                        }
                        var caResult = this.Context.DbMehtods.ContainsArray(model);
                        this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[0].MemberName.ObjToString());
                        return caResult;
                    case "ContainsArrayUseSqlParameters":
                        if (model.Args[0].MemberValue == null)
                        {
                            var first = this.Context.Parameters.FirstOrDefault(it => it.ParameterName == model.Args[0].MemberName.ObjToString());
                            if (first.HasValue())
                            {
                                model.Args[0].MemberValue = first.Value;
                            }
                        }
                        model.Data = this.Context.SqlParameterKeyWord + "INP_" + this.Context.ParameterIndex;
                        this.Context.ParameterIndex++;
                        if (model.Args[0].MemberValue.HasValue())
                        {
                            var inValueIEnumerable = (IEnumerable)model.Args[0].MemberValue;
                            int i = 0;
                            foreach (var item in inValueIEnumerable)
                            {
                                this.Context.Parameters.Add(new SugarParameter(model.Data + "_" + i, item));
                                i++;
                            }
                        }
                        var caResult2 = this.Context.DbMehtods.ContainsArrayUseSqlParameters(model);
                        this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[0].MemberName.ObjToString());
                        return caResult2;
                    case "Equals":
                        return this.Context.DbMehtods.Equals(model);
                    case "EqualsNull":
                        return this.Context.DbMehtods.EqualsNull(model);
                    case "DateIsSame":
                        if (model.Args.Count == 2)
                            return this.Context.DbMehtods.DateIsSameDay(model);
                        else
                        {
                            var dsResult = this.Context.DbMehtods.DateIsSameByType(model);
                            this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[2].MemberName.ObjToString());
                            return dsResult;
                        }
                    case "DateAdd":
                        if (model.Args.Count == 2)
                            return this.Context.DbMehtods.DateAddDay(model);
                        else
                        {
                            var daResult = this.Context.DbMehtods.DateAddByType(model);
                            this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[2].MemberName.ObjToString());
                            return daResult;
                        }
                    case "DateValue":
                        var dvResult = this.Context.DbMehtods.DateValue(model);
                        this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[1].MemberName.ObjToString());
                        return dvResult;
                    case "Between":
                        return this.Context.DbMehtods.Between(model);
                    case "StartsWith":
                        return this.Context.DbMehtods.StartsWith(model);
                    case "EndsWith":
                        return this.Context.DbMehtods.EndsWith(model);
                    case "ToInt32":
                        return this.Context.DbMehtods.ToInt32(model);
                    case "ToInt64":
                        return this.Context.DbMehtods.ToInt64(model);
                    case "ToDate":
                        return this.Context.DbMehtods.ToDate(model);
                    case "ToDateTime":
                        return this.Context.DbMehtods.ToDate(model);
                    case "ToTime":
                        return this.Context.DbMehtods.ToTime(model);
                    case "ToString":
                        if (model.Args.Count > 1)
                        {
                            var dateString2 = this.Context.DbMehtods.GetDateString(model.Args.First().MemberName.ObjToString(), model.Args.Last().MemberValue.ObjToString());
                            if (dateString2 != null) return dateString2;
                            return GeDateFormat(model.Args.Last().MemberValue.ObjToString(), model.Args.First().MemberName.ObjToString());
                        }
                        //Check.Exception(model.Args.Count > 1, "ToString (Format) is not supported, Use ToString().If time formatting can be used it.Date.Year+\"-\"+it.Data.Month+\"-\"+it.Date.Day ");
                        return this.Context.DbMehtods.ToString(model);
                    case "ToVarchar":
                        return this.Context.DbMehtods.ToVarchar(model);
                    case "ToDecimal":
                        return this.Context.DbMehtods.ToDecimal(model);
                    case "ToGuid":
                        return this.Context.DbMehtods.ToGuid(model);
                    case "ToDouble":
                        return this.Context.DbMehtods.ToDouble(model);
                    case "ToBool":
                        return this.Context.DbMehtods.ToBool(model);
                    case "ToBoolean":
                        return this.Context.DbMehtods.ToBool(model);
                    case "Substring":
                        if (model.Args.Count == 2) 
                        {
                            model.Args.Add(new MethodCallExpressionArgs()
                            {
                                 MemberName="100000",
                                 IsMember=true,
                                  MemberValue= "100000",
                            });
                        }
                        return this.Context.DbMehtods.Substring(model);
                    case "Replace":
                        return this.Context.DbMehtods.Replace(model);
                    case "Length":
                        return this.Context.DbMehtods.Length(model);
                    case "AggregateSum":
                        return this.Context.DbMehtods.AggregateSum(model);
                    case "AggregateAvg":
                        return this.Context.DbMehtods.AggregateAvg(model);
                    case "AggregateMin":
                        return this.Context.DbMehtods.AggregateMin(model);
                    case "AggregateMax":
                        return this.Context.DbMehtods.AggregateMax(model);
                    case "AggregateCount":
                        return this.Context.DbMehtods.AggregateCount(model);
                    case "AggregateDistinctCount":
                        return this.Context.DbMehtods.AggregateDistinctCount(model);
                    case "MappingColumn":
                        var mappingColumnResult = this.Context.DbMehtods.MappingColumn(model);
                        if (model.Args.Count == 1&& mappingColumnResult.IsNullOrEmpty()) 
                        {
                            return model.Args[0].MemberName.ObjToString().TrimStart('\'').TrimEnd('\'');
                        }
                        var isValid = model.Args[0].IsMember && model.Args[1].IsMember == false;
                        //Check.Exception(!isValid, "SqlFunc.MappingColumn parameters error, The property name on the left, string value on the right");
                        if (model.Args.Count > 1)
                        {
                            this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[1].MemberName.ObjToString());
                        }
                        else 
                        {
                            this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[0].MemberName.ObjToString());
                        }
                        if (mappingColumnResult == "")
                        {
                            return model.Args[1].MemberName.ObjToString().TrimStart('\'').TrimEnd('\'');
                        }
                        return mappingColumnResult;
                    case "IsNull":
                        return this.Context.DbMehtods.IsNull(model);
                    case "MergeString":
                        return this.Context.DbMehtods.MergeString(model.Args.Select(it => it.MemberName.ObjToString()).ToArray());
                    case "SelectAll":
                    case "GetSelfAndAutoFill":
                        this.Context.Parameters.RemoveAll(it => it.ParameterName == model.Args[0].MemberName.ObjToString());
                        var result1 = this.Context.DbMehtods.GetSelfAndAutoFill(this.Context.GetTranslationColumnName(model.Args[0].MemberValue.ObjToString()), this.Context.IsSingle);
                        if ((model.Args[0].MemberValue + "") == "." && this.Context.IsSingle)
                        {
                            result1 = this.Context.GetTranslationTableName(model.Args[0].MemberName + "", false) + ".*/**/" + result1;
                        }
                        return result1;
                    case "GetDate":
                        return this.Context.DbMehtods.GetDate();
                    case "GetRandom":
                        return this.Context.DbMehtods.GetRandom();
                    case "CharIndex":
                        return this.Context.DbMehtods.CharIndex(model);
                    case "BitwiseAnd":
                        return this.Context.DbMehtods.BitwiseAnd(model);
                    case "BitwiseInclusiveOR":
                        return this.Context.DbMehtods.BitwiseInclusiveOR(model);
                    case "ToDateShort":
                        return this.Context.DbMehtods.ToDateShort(model);
                    case "Oracle_ToChar":
                        return this.Context.DbMehtods.Oracle_ToChar(model);
                    case "Oracle_ToDate":
                        return this.Context.DbMehtods.Oracle_ToDate(model);
                    case "SqlServer_DateDiff":
                        return this.Context.DbMehtods.SqlServer_DateDiff(model);
                    case "Format":
                        var xx = base.BaseParameter;
                        var result = this.Context.DbMehtods.Format(model);
                        if (this.Context.MethodName== "MappingColumn" || this.Context.MethodName?.StartsWith("Row")==true)
                        {
                            result = this.Context.DbMehtods.FormatRowNumber(model);
                        }
                        this.Context.Parameters.RemoveAll(it => model.Args.Select(x => x.MemberName.ObjToString()).Contains(it.ParameterName));
                        return result;
                    case "Abs":
                        return this.Context.DbMehtods.Abs(model);
                    case "Round":
                        return this.Context.DbMehtods.Round(model);
                    case "DateDiff":
                        return this.Context.DbMehtods.DateDiff(model);
                    case "GreaterThan":
                        return this.Context.DbMehtods.GreaterThan(model);
                    case "GreaterThanOrEqual":
                        return this.Context.DbMehtods.GreaterThanOrEqual(model);
                    case "LessThan":
                        return this.Context.DbMehtods.LessThan(model);
                    case "LessThanOrEqual":
                        return this.Context.DbMehtods.LessThanOrEqual(model);
                    case "Asc":
                        return this.Context.DbMehtods.Asc(model);
                    case "Desc":
                        return this.Context.DbMehtods.Desc(model);
                    case "Stuff":
                        return this.Context.DbMehtods.Stuff(model);
                    case "RowNumber":
                        return this.Context.DbMehtods.RowNumber(model);
                    case "RowCount":
                        return this.Context.DbMehtods.RowCount(model);
                    case "RowSum":
                        return this.Context.DbMehtods.RowSum(model);
                    case "RowMax":
                        return this.Context.DbMehtods.RowMax(model);
                    case "RowMin":
                        return this.Context.DbMehtods.RowMin(model);
                    case "RowAvg":
                        return this.Context.DbMehtods.RowAvg(model);
                    case "Exists":
                        if (model.Args.Count > 1)
                        {
                            this.Context.Parameters.RemoveAll(it => model.Args[1].MemberName.ObjToString().Contains(it.ParameterName));
                            List<IConditionalModel> conditionalModels = (List<IConditionalModel>)model.Args[1].MemberValue;
                            var sqlObj = this.Context.SugarContext.Context.Queryable<object>().SqlBuilder.ConditionalModelToSql(conditionalModels, 0);
                            var sql = sqlObj.Key;
                            UtilMethods.RepairReplicationParameters(ref sql, sqlObj.Value, 0, "_" + this.Context.ParameterIndex + "_B");
                            model.Args[1].MemberName = sql;
                            if (sqlObj.Value != null)
                            {
                                this.Context.Parameters.AddRange(sqlObj.Value);
                            }
                            else
                            {
                                return " 1=1 ";
                            }
                        }
                        return this.Context.DbMehtods.Exists(model);

                    case "JsonField":
                        return this.Context.DbMehtods.JsonField(model);
                    case "JsonArrayLength":
                        return this.Context.DbMehtods.JsonArrayLength(model);
                    case "JsonContainsFieldName":
                        return this.Context.DbMehtods.JsonContainsFieldName(model);
                    case "JsonParse":
                        return this.Context.DbMehtods.JsonParse(model);
                    case "JsonLike":
                        return this.Context.DbMehtods.JsonLike(model);
                    case "Collate":
                        return this.Context.DbMehtods.Collate(model);
                    case "AggregateSumNoNull":
                        return this.Context.DbMehtods.AggregateSumNoNull(model);
                    case "JsonListObjectAny":
                        return this.Context.DbMehtods.JsonListObjectAny(model);
                    case "JsonArrayAny":
                        return this.Context.DbMehtods.JsonArrayAny(model);
                    case "CompareTo":
                        return this.Context.DbMehtods.CompareTo(model);
                    case "SplitIn":
                        return this.Context.DbMehtods.SplitIn(model);
                    case "ListAny":
                        this.Context.Result.IsNavicate = true;
                        this.Context.Parameters.RemoveAll(it => model.Args[0].MemberName.ObjToString().Contains(it.ParameterName));
                        return this.Context.DbMehtods.ListAny(model);
                    case "ListAll":
                        this.Context.Result.IsNavicate = true;
                        this.Context.Parameters.RemoveAll(it => model.Args[0].MemberName.ObjToString().Contains(it.ParameterName));
                        return this.Context.DbMehtods.ListAll(model);
                    case "Modulo":
                        return this.Context.DbMehtods.Modulo(model);
                    case "Like":
                        return this.Context.DbMehtods.Like(model);
                    case "ToSingle":
                        return this.Context.DbMehtods.ToSingle(model);
                    default:
                        if (typeof(IDbMethods).GetMethods().Any(it => it.Name == name))
                        {
                            return this.Context.DbMehtods.GetType().GetMethod(name).Invoke(this.Context.DbMehtods,new object[] { model});
                        }
                        break;
                }
            }
            return null;
        }
        private DbType GetType(string name)
        {
            DbType result = DbType.SqlServer;
            foreach (var item in UtilMethods.EnumToDictionary<DbType>())
            {
                if (name.StartsWith(item.Value.ToString()))
                {
                    result = item.Value;
                    break;
                }
            }
            return result;
        }

        private bool IsContainsArray(MethodCallExpression express, string methodName, bool isValidNativeMethod)
        {
            return !isValidNativeMethod && express.Method.DeclaringType.Namespace.IsIn("System.Collections", "System.Linq", "System.Collections.Generic") && methodName == "Contains";
        }
        private bool IsSubMethod(MethodCallExpression express, string methodName)
        {
            return SubTools.SubItemsConst.Any(it => it.Name == methodName) && express.Object != null && (express.Object.Type.Name.StartsWith("Subqueryable`"));
        }
        private bool CheckMethod(MethodCallExpression expression)
        {
            if (expression?.Object?.Type?.Name?.StartsWith("ISugarQueryable`") == true) 
            {
                Check.ExceptionEasy("Sublookup is implemented using SqlFunc.Subquery<Order>(); Queryable objects cannot be used", "子查请使用SqlFunc.Subquery<Order>()来实现，不能用Queryable对象");
            }
            if (expression.Method.Name == "SelectAll")
            {
                return true;
            }
            if (expression.Method.Name == "CompareTo") 
            {
                return true;
            }
            if (expression.Method.Name == "Any"&& expression.Arguments.Count()>0&& ExpressionTool.IsVariable(expression.Arguments[0]) )
            {
                return true;
            }
            if (expression.Method.Name == "All" && expression.Arguments.Count() > 0 && ExpressionTool.IsVariable(expression.Arguments[0]))
            {
                return true;
            }
            if (expression.Method.Name == "Format" && expression.Method.DeclaringType == UtilConstants.StringType)
            {
                return true;
            }
            if (IsExtMethod(expression.Method.Name))
                return true;
            if (IsParseMethod(expression))
                return true;
            if (expression.Method.Name == "IsNullOrEmpty" && expression.Method.DeclaringType == UtilConstants.StringType)
            {
                return true;
            }
            if (expression.Method.Name == "IsNullOrWhiteSpace" && expression.Method.DeclaringType == UtilConstants.StringType)
            {
                return true;
            }
            if (expression.Method.ReflectedType().FullName != ExpressionConst.SqlFuncFullName)
                return false;
            else
                return true;
        }
        private Type TempParseType;
        public bool IsParseMethod(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Parse" && expression.Method.DeclaringType.IsIn(
                                                                                          UtilConstants.DecType,
                                                                                          UtilConstants.DateType,
                                                                                          UtilConstants.DobType,
                                                                                          UtilConstants.GuidType,
                                                                                          UtilConstants.FloatType,
                                                                                          UtilConstants.ShortType,
                                                                                          UtilConstants.LongType,
                                                                                          UtilConstants.IntType,
                                                                                          UtilConstants.BoolType))
            {
                TempParseType = expression.Method.DeclaringType;
                return true;
            }
            return false;
        }
        private static bool IsNot(Expression item)
        {
            return item is UnaryExpression && (item as UnaryExpression).NodeType == ExpressionType.Not;
        }
        private bool IsDateItemValue(Expression item)
        {
            var result = false;
            if (item is MemberExpression)
            {
                var memberExp = item as MemberExpression;
                if (memberExp != null && memberExp.Expression != null && memberExp.Expression.Type == UtilConstants.DateType)
                {
                    foreach (var dateType in UtilMethods.EnumToDictionary<DateType>())
                    {
                        if (memberExp.Member.Name.EqualCase(dateType.Key))
                        {
                            result = true;
                            break;
                        }
                        else if (memberExp.Member.Name == "DayOfWeek")
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }
        private static bool IsDateDate(Expression item)
        {
            return item.Type == UtilConstants.DateType && item is MemberExpression && (item as MemberExpression).Member.Name == "Date" && item.ToString() != "DateTime.Now.Date";
        }
        private static bool IsDateValue(Expression item)
        {
            return item.Type == UtilConstants.IntType &&
                                    item is MemberExpression &&
                                    (item as MemberExpression).Expression != null &&
                                    (item as MemberExpression).Expression.Type == UtilConstants.DateType &&
                                    (item as MemberExpression).Expression is MemberExpression &&
                                    ((item as MemberExpression).Expression as MemberExpression).Member.Name == "Value";
        }
        private bool IsValidNativeMethod(MethodCallExpression express, string methodName)
        {
            return MethodMapping.ContainsKey(methodName) && express.Method.DeclaringType.Namespace == ("System");
        }
        private bool IsExtMethod(string methodName)
        {
            if (this.Context.SqlFuncServices == null) return false;
            return this.Context.SqlFuncServices.Select(it => it.UniqueMethodName).Contains(methodName);
        }
        private bool IsIfElse(MethodCallExpression express, string methodName)
        {
            if (methodName == "End" && express.Object.Type == typeof(CaseWhen))
                return true;
            else
                return false;
        }
    }
}
