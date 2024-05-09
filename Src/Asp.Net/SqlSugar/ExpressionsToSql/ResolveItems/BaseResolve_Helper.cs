using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    /// <summary>
    /// BaseResolve-Helper
    /// </summary>
    public partial class BaseResolve
    {
        #region Set Method
        protected void SetNavigateResult()
        {
            if (this.Context != null)
            {
                if (this.Context.Result != null)
                {
                    this.Context.Result.IsNavicate = true;
                }
            }
        }
        private void SetParameter(out Expression expression, out ExpressionParameter parameter)
        {
            Context.Index++;
            expression = this.Expression;
            parameter = new ExpressionParameter()
            {
                Context = this.Context,
                CurrentExpression = expression,
                IsLeft = this.IsLeft,
                BaseExpression = this.ExactExpression,
                BaseParameter = this.BaseParameter,
                Index = Context.Index
            };
        }

        #endregion

        #region Get Mehtod
        protected object GetMemberValue(object value, Expression exp)
        {
            if (exp is MemberExpression)
            {
                var member = (exp as MemberExpression);
                var memberParent = member.Expression;
                if (memberParent != null && this.Context?.SugarContext?.Context != null)
                {
                    var entity = this.Context.SugarContext.Context.EntityMaintenance.GetEntityInfo(memberParent.Type);
                    var columnInfo = entity.Columns.FirstOrDefault(it => it.PropertyName == member.Member.Name);
                    if (columnInfo?.SqlParameterDbType is Type)
                    {
                        var type = columnInfo.SqlParameterDbType as Type;
                        var ParameterConverter = type.GetMethod("ParameterConverter").MakeGenericMethod(columnInfo.PropertyInfo.PropertyType);
                        var obj = Activator.CreateInstance(type);
                        var p = ParameterConverter.Invoke(obj, new object[] { value, 100 + this.Context.ParameterIndex }) as SugarParameter;
                        value = p.Value;
                    }
                }
            }

            return value;
        }
        private string GetAsName(Expression item, object shortName, PropertyInfo property)
        {
            string asName;
            var propertyName = property.Name;
            var dbColumnName = propertyName;
            var mappingInfo = this.Context.MappingColumns.FirstOrDefault(it => it.EntityName == item.Type.Name && it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            if (mappingInfo.HasValue())
            {
                dbColumnName = mappingInfo.DbColumnName;
            }
            asName = this.Context.GetTranslationText(item.Type.Name + "." + propertyName);
            if (Context.IsJoin)
            {
                this.Context.Result.Append(Context.GetAsString(asName, dbColumnName, shortName.ObjToString()));
            }
            else
            {
                this.Context.Result.Append(Context.GetAsString(asName, dbColumnName));
            }

            return asName;
        }
        private string GetAsNameAndShortName(Expression item, object shortName, PropertyInfo property)
        {
            string asName;
            var propertyName = property.Name;
            var dbColumnName = propertyName;
            var mappingInfo = this.Context.MappingColumns.FirstOrDefault(it => it.EntityName == item.Type.Name && it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            if (mappingInfo.HasValue())
            {
                dbColumnName = mappingInfo.DbColumnName;
            }
            if (shortName != null && shortName.ObjToString().Contains(this.Context.SqlTranslationLeft) && this.Context.IsSingle)
            {
                asName = this.Context.GetTranslationText(item.Type.Name + "." + propertyName);
            }
            else
            {
                asName = this.Context.GetTranslationText(shortName + "." + item.Type.Name + "." + propertyName);
            }
            if (Context.IsJoin)
            {
                this.Context.Result.Append(Context.GetAsString(asName, dbColumnName, shortName.ObjToString()));
            }
            else
            {
                this.Context.Result.Append(Context.GetAsString(asName, dbColumnName));
            }

            return asName;
        }
        private EntityColumnInfo GetColumnInfo(Expression oppoSiteExpression)
        {
            var oppsite = (oppoSiteExpression as MemberExpression);
            if (oppsite == null) return null;
            if (this.Context.SugarContext == null) return null;
            if (this.Context.SugarContext.Context == null) return null;
            if (oppsite.Expression == null) return null;
            var columnInfo = this.Context.SugarContext.Context.EntityMaintenance
                .GetEntityInfo(oppsite.Expression.Type).Columns.FirstOrDefault(it => it.PropertyName == oppsite.Member.Name);
            return columnInfo;
        }
        protected MethodCallExpressionArgs GetMethodCallArgs(ExpressionParameter parameter, Expression item, string name = null)
        {
            var newContext = this.Context.GetCopyContext();
            newContext.MappingColumns = this.Context.MappingColumns;
            newContext.MappingTables = this.Context.MappingTables;
            newContext.IgnoreComumnList = this.Context.IgnoreComumnList;
            newContext.IsSingle = this.Context.IsSingle;
            newContext.SqlFuncServices = this.Context.SqlFuncServices;
            newContext.MethodName = name;
            newContext.Resolve(item, this.Context.IsJoin ? ResolveExpressType.WhereMultiple : ResolveExpressType.WhereSingle);
            this.Context.Index = newContext.Index;
            this.Context.ParameterIndex = newContext.ParameterIndex;
            if (newContext.Parameters.HasValue())
            {
                this.Context.Parameters.AddRange(newContext.Parameters);
            }
            if (newContext.SingleTableNameSubqueryShortName.HasValue())
            {
                this.Context.SingleTableNameSubqueryShortName = newContext.SingleTableNameSubqueryShortName;
            }
            var methodCallExpressionArgs = new MethodCallExpressionArgs()
            {
                IsMember = true,
                MemberName = newContext.Result.GetResultString()
            };
            return methodCallExpressionArgs;
        }
        private string GetAsNameResolveAnObject(ExpressionParameter parameter, Expression item, string asName, bool isSameType)
        {
            this.Start();
            var shortName = parameter.CommonTempData;
            var listProperties = item.Type.GetProperties().Cast<PropertyInfo>().ToList();
            foreach (var property in listProperties)
            {
                var hasIgnore = this.Context.IgnoreComumnList != null && this.Context.IgnoreComumnList.Any(it => it.EntityName.Equals(item.Type.Name, StringComparison.CurrentCultureIgnoreCase) && it.PropertyName.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase));
                if (hasIgnore)
                {
                    continue;
                }
                if (property.PropertyType.IsClass())
                {
                    var comumnInfo = property.GetCustomAttribute<SugarColumn>();
                    if (comumnInfo != null && comumnInfo.IsJson && isSameType)
                    {
                        asName = GetAsNameAndShortName(item, shortName, property);
                    }
                    else if (comumnInfo != null && comumnInfo.IsJson)
                    {
                        asName = GetAsName(item, shortName, property);
                    }
                    else if (comumnInfo != null && this.Context.SugarContext != null && this.Context.SugarContext.Context != null)
                    {
                        var entityInfo = this.Context.SugarContext.Context.EntityMaintenance.GetEntityInfo(item.Type);
                        var entityColumn = entityInfo.Columns.FirstOrDefault(it => it.PropertyName == property.Name);
                        if (entityColumn != null && entityColumn.IsJson)
                        {
                            asName = GetAsName(item, shortName, property);
                        }
                    }
                }
                else if (isSameType)
                {
                    asName = GetAsNameAndShortName(item, shortName, property);
                }
                else
                {
                    asName = GetAsName(item, shortName, property);
                }
            }

            return asName;
        }
        public object GetAsNamePackIfElse(object methodValue)
        {
            methodValue = this.Context.DbMehtods.CaseWhen(new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("IF",methodValue.ObjToString()),
                    new KeyValuePair<string, string>("Return","1"),
                    new KeyValuePair<string, string>("End","0")
                 });
            return methodValue;
        } 
        #endregion
    }
}
