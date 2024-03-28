using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SqlSugar 
{
    internal class OneToOneNavgateExpression
    {
        public ExpressionContext ExpContext;
        private SqlSugarProvider context;
        internal EntityInfo EntityInfo;
        internal EntityInfo ProPertyEntity;
        private Navigate Navigat;
        public string ShorName;
        internal string MemberName;
        private MemberExpressionResolve _memberExpressionResolve;
        public OneToOneNavgateExpression(SqlSugarProvider context, MemberExpressionResolve memberExpressionResolve)
        {
            this.context = context;
            _memberExpressionResolve= memberExpressionResolve;
        }

        internal bool IsNavgate(Expression expression)
        {
            var result = false;
            var exp = expression;
            if (exp is UnaryExpression) 
            {
                exp = (exp as UnaryExpression).Operand;
            }
            if (exp is MemberExpression)
            {
                var memberExp = exp as MemberExpression;
                var childExpression = memberExp.Expression;
                result = ValidateNav(result, memberExp, childExpression);
            }
            return result;
        }

        private bool ValidateNav(bool result, MemberExpression memberExp, Expression childExpression)
        {
            if (childExpression != null && childExpression is MemberExpression)
            {
                var child2Expression = (childExpression as MemberExpression).Expression;
                if (child2Expression == null) 
                {
                    return false;
                }
                if (child2Expression.Type.IsClass() && child2Expression is ParameterExpression)
                {
                    var entity = this.context.EntityMaintenance.GetEntityInfo(child2Expression.Type);
                    if (entity.Columns.Any(x => x.PropertyName == (childExpression as MemberExpression).Member.Name && x.Navigat != null))
                    {
                        EntityInfo = entity;
                        ShorName = child2Expression.ToString();
                        MemberName = memberExp.Member.Name;
                        ProPertyEntity = this.context.EntityMaintenance.GetEntityInfo(childExpression.Type);
                        Navigat = entity.Columns.FirstOrDefault(x => x.PropertyName == (childExpression as MemberExpression).Member.Name).Navigat;
                        result = true;
                    }
                }
            }

            return result;
        }

        internal MapperSql GetSql()
        {
            if (this.ProPertyEntity.Type.Name.StartsWith("List`")) 
            {
                Check.ExceptionEasy(true, " expression error ", "导航查询出错，比如.Count要改成 .Count()");
            }
            else if (Navigat.NavigatType == NavigateType.Dynamic)
            {
                return NavigatDynamicSql();
            }
            var pk = this.ProPertyEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true)?.DbColumnName;
            if (Navigat.Name2.HasValue())
            {
                pk = this.ProPertyEntity.Columns.FirstOrDefault(it => it.PropertyName == Navigat.Name2)?.DbColumnName;
            }
            if(pk==null) 
            {
                Check.ExceptionEasy(
                  true,
                  $"{this.ProPertyEntity.EntityName} naviate config error",
                  $"{this.ProPertyEntity.EntityName} 导航配置错误");
            }
            var name = this.EntityInfo.Columns.First(it => it.PropertyName == Navigat.Name).DbColumnName;
            var selectName = this.ProPertyEntity.Columns.First(it => it.PropertyName ==MemberName).DbColumnName;
            MapperSql mapper = new MapperSql();
            var queryable = this.context.Queryable<object>();
            pk = queryable.QueryBuilder.Builder.GetTranslationColumnName(pk);
            name = queryable.QueryBuilder.Builder.GetTranslationColumnName(name);
            selectName = queryable.QueryBuilder.Builder.GetTranslationColumnName(selectName);
            var tableName = this.ProPertyEntity.DbTableName;
            if (ExpContext?.SugarContext?.QueryBuilder?.IsCrossQueryWithAttr==true) 
            {
                var attr= this.ProPertyEntity.Type.GetCustomAttribute<TenantAttribute>();
                var configId = ((object)this.context.CurrentConnectionConfig.ConfigId).ObjToString();
                if (attr != null && configId != attr.configId.ObjToString())
                {
                    var context = this.context.Root.GetConnection(attr.configId);
                    var dbName = context.Ado.Connection.Database;
                    if (context.CurrentConnectionConfig.DbLinkName.HasValue())
                    {
                        tableName = UtilMethods.GetTableByDbLink(context, tableName, tableName, attr);
                    }
                    else
                    {
                        tableName = queryable.QueryBuilder.LambdaExpressions.DbMehtods.GetTableWithDataBase
                            (queryable.QueryBuilder.Builder.GetTranslationColumnName(dbName), queryable.QueryBuilder.Builder.GetTranslationColumnName(tableName));
                    }
                }
            }
            Type[] clearTypes = null;
            var isClearFilter = false;
            if (this._memberExpressionResolve?.Context?.SugarContext?.QueryBuilder != null)
            {
                queryable.QueryBuilder.LambdaExpressions.ParameterIndex = 500 + this._memberExpressionResolve.Context.SugarContext.QueryBuilder.LambdaExpressions.ParameterIndex;
                this._memberExpressionResolve.Context.SugarContext.QueryBuilder.LambdaExpressions.ParameterIndex++;
                isClearFilter = this._memberExpressionResolve.Context.SugarContext.QueryBuilder.IsDisabledGobalFilter;
                clearTypes = this._memberExpressionResolve.Context.SugarContext.QueryBuilder.RemoveFilters;
            }
            var type = this.ProPertyEntity.Columns.Count(it => it.IsPrimarykey) > 1 ? this.ProPertyEntity.Type : null;
            if (isClearFilter) 
            {
                type = null;
            }
            var sqlObj = queryable
                .AS(tableName)
                .ClearFilter(clearTypes)
                .Filter(type)
                .WhereIF(Navigat.WhereSql.HasValue(), Navigat.WhereSql)
                .Where($" {queryable.SqlBuilder.GetTranslationColumnName(ShorName)}.{name}={pk} ").Select(selectName).ToSql();
            mapper.Sql = sqlObj.Key;
            mapper.Sql = $" ({mapper.Sql}) ";

            if (type!=null&sqlObj.Value?.Any() == true)
            {
                foreach (var item in sqlObj.Value)
                {
                    if (!this._memberExpressionResolve.Context.Parameters.Any(it => it.ParameterName == item.ParameterName))
                    {
                        this._memberExpressionResolve.Context.Parameters.Add(item);
                    }
                }
            }

            return mapper;
        }

        private MapperSql NavigatDynamicSql()
        {
            if (Navigat.Name == null) 
            {
                Check.ExceptionEasy(
                   true,
                   " NavigateType.Dynamic User-defined navigation objects need to be configured with json to be used in expressions .  " + this.ProPertyEntity.Type.Name,
                   " NavigateType.Dynamic 自定义导航对象需要配置json才能在表达式中使用。 " + this.ProPertyEntity.Type.Name);
            }
            MapperSql mapperSql = new MapperSql();
            //var name = this.EntityInfo.Columns.First(it => it.PropertyName == Navigat.Name).DbColumnName;
            var selectName = this.ProPertyEntity.Columns.First(it => it.PropertyName == MemberName).DbColumnName;
            MapperSql mapper = new MapperSql();
            var queryable = this.context.Queryable<object>();
            var tableName = this.ProPertyEntity.DbTableName;
            Type[] clearTypes = null;
            var isClearFilter = false;
            if (this._memberExpressionResolve?.Context?.SugarContext?.QueryBuilder != null)
            {
                queryable.QueryBuilder.LambdaExpressions.ParameterIndex = 500 + this._memberExpressionResolve.Context.SugarContext.QueryBuilder.LambdaExpressions.ParameterIndex;
                this._memberExpressionResolve.Context.SugarContext.QueryBuilder.LambdaExpressions.ParameterIndex++;
                isClearFilter = this._memberExpressionResolve.Context.SugarContext.QueryBuilder.IsDisabledGobalFilter;
                clearTypes = this._memberExpressionResolve.Context.SugarContext.QueryBuilder.RemoveFilters;
            }
            var type = this.ProPertyEntity.Columns.Count(it => it.IsPrimarykey) > 1 ? this.ProPertyEntity.Type : null;
            if (isClearFilter)
            {
                type = null;
            }
            queryable
                .AS(tableName)
                .Take(1)
                .ClearFilter(clearTypes)
                .Filter(type)
                .WhereIF(Navigat.WhereSql.HasValue(), Navigat.WhereSql) 
                .Select(selectName);
            var json = Newtonsoft.Json.Linq.JArray.Parse(Navigat.Name);
            foreach (var item in json)
            {
                string m = item["m"] + "";
                string c = item["c"] + "";
                var leftName= this.EntityInfo.Columns.First(it => it.PropertyName == m).DbColumnName;
                var rightName= this.ProPertyEntity.Columns.First(it => it.PropertyName == c).DbColumnName;
                queryable.Where($" {queryable.SqlBuilder.GetTranslationColumnName(ShorName)}.{queryable.SqlBuilder.GetTranslationColumnName(leftName)}={queryable.SqlBuilder.GetTranslationColumnName(rightName)} ");
            }
            var sqlObj= queryable.ToSql();
            mapper.Sql = sqlObj.Key;
            mapper.Sql = $" ({mapper.Sql}) ";
            return mapper;
        }
    }
}
