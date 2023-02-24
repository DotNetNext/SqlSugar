using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        public OneToOneNavgateExpression(SqlSugarProvider context)
        {
            this.context = context;
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
                Check.ExceptionEasy(
                    true,
                    " NavigateType.Dynamic no support expression .  "+ this.ProPertyEntity.Type.Name,
                    " NavigateType.Dynamic 自定义导航对象不支持在Where(x=>x.自定义.Id==1)等方法中使用"+ this.ProPertyEntity.Type.Name);
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
                if (attr != null&& configId != attr.configId.ObjToString()) 
                {
                    var dbName = this.context.Root.GetConnection(attr.configId).Ado.Connection.Database;
                    tableName = queryable.QueryBuilder.LambdaExpressions.DbMehtods.GetTableWithDataBase
                        (queryable.QueryBuilder.Builder.GetTranslationColumnName(dbName), queryable.QueryBuilder.Builder.GetTranslationColumnName(tableName));
                }
            }
            mapper.Sql = queryable
                .AS(tableName)
                .WhereIF(Navigat.WhereSql.HasValue(),Navigat.WhereSql)
                .Where($" {queryable.SqlBuilder.GetTranslationColumnName(ShorName)}.{name}={pk} ").Select(selectName).ToSql().Key;
            mapper.Sql = $" ({mapper.Sql}) ";
            return mapper;
        }
    }
}
