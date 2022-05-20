using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class OneToManyNavgateExpression
    {
        private SqlSugarProvider context;
        private EntityInfo EntityInfo;
        private EntityInfo ProPertyEntity;
        private Navigate Navigat;
        public string ShorName;
        public string PropertyShortName;
        private string MemberName;
        private string MethodName;
        private string whereSql;
        private MethodCallExpressionResolve methodCallExpressionResolve;
        public OneToManyNavgateExpression(SqlSugarProvider context, MethodCallExpressionResolve methodCallExpressionResolve)
        {
            this.context = context;
            this.methodCallExpressionResolve = methodCallExpressionResolve;
        }

        internal bool IsNavgate(Expression expression)
        {
            var result = false;
            var exp = expression;
            if (exp is UnaryExpression) 
            {
                exp = (exp as UnaryExpression).Operand;
            }
            if (exp is MethodCallExpression) 
            {
                var memberExp=exp as MethodCallExpression;
                MethodName = memberExp.Method.Name;
                if (memberExp.Method.Name.IsIn("Any","Count") &&  memberExp.Arguments.Count>0 && memberExp.Arguments[0] is MemberExpression ) 
                {
                    result = ValidateNav(result, memberExp.Arguments[0] as MemberExpression, memberExp.Arguments[0]);
                    if (memberExp.Arguments.Count > 1)
                    {
                        var pars = ExpressionTool.ExpressionParameters(memberExp.Arguments.Last());
                        if (pars != null&& ProPertyEntity!=null&& pars.Any(z => z.Type == ProPertyEntity.Type))
                        {
                            PropertyShortName = pars.First(z => z.Type == ProPertyEntity.Type).Name;
                        }
                        whereSql = GetWhereSql(memberExp);
                    }           
                }
            }
            return result;
        }

        private string GetWhereSql(MethodCallExpression memberExp)
        {
            var whereExp = memberExp.Arguments[1];
            if (PropertyShortName.HasValue()&& Navigat!=null&& Navigat.NavigatType==NavigateType.OneToMany)
            {
                var result = this.methodCallExpressionResolve.GetNewExpressionValue(whereExp, ResolveExpressType.WhereMultiple);
                return result;
            }
            else
            {
                var result = this.methodCallExpressionResolve.GetNewExpressionValue(whereExp);
                return result;
            }
        }

        private bool ValidateNav(bool result, MemberExpression memberExp, Expression childExpression)
        {
            if (childExpression != null && childExpression is MemberExpression)
            {
                var child2Expression = (childExpression as MemberExpression).Expression;
                if (child2Expression.Type.IsClass() && child2Expression is ParameterExpression)
                {
                    var rootType = child2Expression.Type;
                    var rootEntity = this.context.EntityMaintenance.GetEntityInfo(rootType);
                    var type= childExpression.Type.GetGenericArguments()[0];
                    var entity = this.context.EntityMaintenance.GetEntityInfo(type);
                    if (rootEntity.Columns.Any(x => x.PropertyName == (childExpression as MemberExpression).Member.Name && x.Navigat != null))
                    {
                        EntityInfo = rootEntity;
                        ShorName = child2Expression.ToString();
                        MemberName = memberExp.Member.Name;
                        ProPertyEntity = entity;
                        Navigat = rootEntity.Columns.FirstOrDefault(x => x.PropertyName == (childExpression as MemberExpression).Member.Name).Navigat;
                        result = true;
                    }
                }
            }

            return result;
        }
        internal MapperSql GetSql()
        {
            if (Navigat.NavigatType == NavigateType.OneToMany)
            {
                return GetOneToManySql();
            }
            else 
            {
                return GetManyToManySql();
            }
     
        }
        private MapperSql GetManyToManySql()
        {
         
            var bPk = this.ProPertyEntity.Columns.First(it => it.IsPrimarykey == true).DbColumnName;
            var aPk = this.EntityInfo.Columns.First(it => it.IsPrimarykey == true).DbColumnName;
            MapperSql mapper = new MapperSql();
            var queryable = this.context.Queryable<object>();
            bPk = queryable.QueryBuilder.Builder.GetTranslationColumnName(bPk);
            aPk = queryable.QueryBuilder.Builder.GetTranslationColumnName(aPk);
            var mappingType = Navigat.MappingType;
            var mappingEntity = this.context.EntityMaintenance.GetEntityInfo(mappingType);
            var mappingTableName=queryable.QueryBuilder.Builder.GetTranslationTableName(mappingEntity.DbTableName);
            var mappingA = mappingEntity.Columns.First(it => it.PropertyName == Navigat.MappingAId).DbColumnName;
            var mappingB = mappingEntity.Columns.First(it => it.PropertyName == Navigat.MappingBId).DbColumnName;
            mappingA = queryable.QueryBuilder.Builder.GetTranslationColumnName(mappingA);
            mappingB = queryable.QueryBuilder.Builder.GetTranslationColumnName(mappingB);
            var bTableName = queryable.QueryBuilder.Builder.GetTranslationTableName(this.ProPertyEntity.DbTableName);
            mapper.Sql = $" (select count(1) from {bTableName} {this.ProPertyEntity.DbTableName}_1  where  {this.ProPertyEntity.DbTableName}_1.{bPk} in (select {mappingB} from {mappingTableName} where {mappingA} = {ShorName}.{aPk} )  )";
            if (this.whereSql.HasValue())
            {
                mapper.Sql = mapper.Sql.TrimEnd(')');
                mapper.Sql = mapper.Sql + " AND " + this.whereSql+")";
            }
            mapper.Sql = $" ({mapper.Sql}) ";
            mapper.Sql = GetMethodSql(mapper.Sql);
            return mapper;
        }
        private MapperSql GetOneToManySql()
        {
            var pk = this.EntityInfo.Columns.First(it => it.IsPrimarykey == true).DbColumnName;
            var name = this.ProPertyEntity.Columns.First(it => it.PropertyName == Navigat.Name).DbColumnName;
            //var selectName = this.ProPertyEntity.Columns.First(it => it.PropertyName == MemberName).DbColumnName;
            MapperSql mapper = new MapperSql();
            var queryable = this.context.Queryable<object>();
            pk = queryable.QueryBuilder.Builder.GetTranslationColumnName(pk);
            name = queryable.QueryBuilder.Builder.GetTranslationColumnName(name);
            //selectName = queryable.QueryBuilder.Builder.GetTranslationColumnName(selectName);
            if (PropertyShortName.HasValue())
            {
                queryable.QueryBuilder.TableShortName = PropertyShortName;
            }
            mapper.Sql = queryable
                .AS(this.ProPertyEntity.DbTableName)
                .WhereIF(!string.IsNullOrEmpty(whereSql), whereSql)
                .Where($" {name}={ShorName}.{pk} ").Select(" COUNT(1) ").ToSql().Key;
            mapper.Sql = $" ({mapper.Sql}) ";
            mapper.Sql = GetMethodSql(mapper.Sql);
            return mapper;
        }

        private string GetMethodSql(string sql)
        {
            if (MethodName == "Any") 
            {
                return $" ({sql}>0 ) ";
            }
            return sql;
        }

    }
}
