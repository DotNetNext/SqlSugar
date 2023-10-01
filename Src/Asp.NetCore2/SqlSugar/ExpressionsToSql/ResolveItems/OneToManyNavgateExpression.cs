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
        public int ParameterIndex = 0;
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
            if (PropertyShortName.HasValue() && Navigat != null && Navigat.NavigatType == NavigateType.OneToMany)
            {
                InitType(whereExp);
                var result = this.methodCallExpressionResolve.GetNewExpressionValue(whereExp, ResolveExpressType.WhereMultiple);
                return result;
            }
            else if (whereExp!=null&&whereExp.Type == typeof(List<IConditionalModel>)) 
            {
                var value = ExpressionTool.GetExpressionValue(whereExp) as List<IConditionalModel>;
                //this.context.Utilities.Context.Queryable<object>().Where(value).ToList();
                if (value.HasValue())
                {
                    var sqlbuilder = this.context.Queryable<object>().SqlBuilder;
                    var sqlObj = sqlbuilder.ConditionalModelToSql(value, 0);
                    var sql = sqlObj.Key;
                    var count = methodCallExpressionResolve?.Context?.SugarContext?.QueryBuilder?.Parameters?.Count??0;
                    UtilMethods.RepairReplicationParameters(ref sql, sqlObj.Value,0,"_"+ count+"_");
                    methodCallExpressionResolve.Context.Parameters.AddRange(sqlObj.Value);
                    return sql;
                }
                else
                {
                    return " 1=1 ";
                }
            }
            else
            {
                InitType(whereExp);
                var result = this.methodCallExpressionResolve.GetNewExpressionValue(whereExp);
                return result;
            }
        }

        private void InitType(Expression whereExp)
        {
            if (whereExp is LambdaExpression)
            {
                var parameters = (whereExp as LambdaExpression).Parameters;
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var item in parameters)
                    {
                        this.context.InitMappingInfo(item.Type);
                    }
                }
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
            else if (Navigat.NavigatType == NavigateType.Dynamic) 
            {
                Check.ExceptionEasy(
                   true,
                   " NavigateType.Dynamic no support expression .  " + this.ProPertyEntity.Type.Name,
                   " NavigateType.Dynamic 自定义导航对象不支持在Where(x=>x.自定义.Id==1)等方法中使用" + this.ProPertyEntity.Type.Name);
                return null;
            }
            else
            {
                return GetManyToManySql();
            }
     
        }
        private MapperSql GetManyToManySql()
        {
         
            var bPk = this.ProPertyEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true)?.DbColumnName;
            var aPk = this.EntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey == true)?.DbColumnName;
            Check.ExceptionEasy(aPk.IsNullOrEmpty(), $"{this.EntityInfo.EntityName}need primary key", $"{this.EntityInfo.EntityName}需要主键");
            Check.ExceptionEasy(bPk.IsNullOrEmpty(), $"{this.ProPertyEntity.EntityName}need primary key", $"{this.ProPertyEntity.EntityName}需要主键");
            MapperSql mapper = new MapperSql();
            var queryable = this.context.Queryable<object>();
            bPk = queryable.QueryBuilder.Builder.GetTranslationColumnName(bPk);
            aPk = queryable.QueryBuilder.Builder.GetTranslationColumnName(aPk);
            var mappingType = Navigat.MappingType;
            Check.ExceptionEasy(mappingType == null, "ManyToMany misconfiguration", "多对多配置错误");
            var mappingEntity = this.context.EntityMaintenance.GetEntityInfo(mappingType);
            var mappingTableName=queryable.QueryBuilder.Builder.GetTranslationTableName(mappingEntity.DbTableName);
            var mappingA = mappingEntity.Columns.First(it => it.PropertyName == Navigat.MappingAId).DbColumnName;
            var mappingB = mappingEntity.Columns.First(it => it.PropertyName == Navigat.MappingBId).DbColumnName;
            mappingA = queryable.QueryBuilder.Builder.GetTranslationColumnName(mappingA);
            mappingB = queryable.QueryBuilder.Builder.GetTranslationColumnName(mappingB);
            var bTableName = queryable.QueryBuilder.Builder.GetTranslationTableName(this.ProPertyEntity.DbTableName);
            this.context.InitMappingInfo(mappingType);
            var queryBuilerAB=this.context.Queryable<object>().QueryBuilder;
            queryBuilerAB.LambdaExpressions.ParameterIndex = 100+this.ParameterIndex;
            var filters= queryBuilerAB.GetFilters(mappingType);
            if (filters.HasValue()&& this.methodCallExpressionResolve?.Context?.SugarContext?.QueryBuilder?.IsDisabledGobalFilter!=true) 
            {
                aPk += " AND " + filters;
                if (queryBuilerAB.Parameters != null) 
                {
                    this.methodCallExpressionResolve.Context.Parameters.AddRange(queryBuilerAB.Parameters);
                }
            }

            mapper.Sql = $" (select {(MethodName == "Any" ? "1":" COUNT(1) ")} from {bTableName} {this.ProPertyEntity.DbTableName}_1  where  {this.ProPertyEntity.DbTableName}_1.{bPk} in (select {mappingB} from {mappingTableName} where {mappingA} = {ShorName}.{aPk} )  )";
            if (this.whereSql.HasValue())
            {
                mapper.Sql = mapper.Sql.TrimEnd(')');
                if (this.whereSql.Contains($" {PropertyShortName}.")) 
                {
                    this.whereSql = this.whereSql.Replace($" {PropertyShortName}.", $" {this.ProPertyEntity.DbTableName}_1.");
                }
                else if (this.whereSql.Contains($" {queryable.QueryBuilder.Builder.GetTranslationColumnName(PropertyShortName)}."))
                {
                    this.whereSql = this.whereSql.Replace($" {queryable.QueryBuilder.Builder.GetTranslationColumnName(PropertyShortName)}.", $" {this.ProPertyEntity.DbTableName}_1.");
                }
                else if (this.whereSql.Contains($"({queryable.QueryBuilder.Builder.GetTranslationColumnName(PropertyShortName)}."))
                {
                    this.whereSql = this.whereSql.Replace($"({queryable.QueryBuilder.Builder.GetTranslationColumnName(PropertyShortName)}.", $"({this.ProPertyEntity.DbTableName}_1.");
                }
                mapper.Sql = mapper.Sql + " AND " + this.whereSql+")";
            }
            if (MethodName == "Any")
            {
                mapper.Sql = $" {mapper.Sql} ";
            }
            else 
            {
                mapper.Sql = $" ({mapper.Sql}) ";
            }
            mapper.Sql = GetMethodSql(mapper.Sql);
            return mapper;
        } 
        private MapperSql GetOneToManySql()
        {
            var pkColumn = this.EntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
            if (Navigat.Name2.HasValue()) 
            {
                pkColumn = this.EntityInfo.Columns.FirstOrDefault(it => it.PropertyName== Navigat.Name2);
            }
            Check.ExceptionEasy(pkColumn == null, $"{this.EntityInfo.EntityName} need primary key ",
                $"导航属性 {this.EntityInfo.EntityName}需要主键");
            var pk = pkColumn.DbColumnName;
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
            queryable.QueryBuilder.LambdaExpressions.ParameterIndex = 500;
            var isClearFilter = false;
            Type[] clearTypes = null;
            if (this.methodCallExpressionResolve?.Context?.SugarContext?.QueryBuilder != null) 
            {
                queryable.QueryBuilder.LambdaExpressions.ParameterIndex=500+ this.methodCallExpressionResolve.Context.SugarContext.QueryBuilder.LambdaExpressions.ParameterIndex;
                this.methodCallExpressionResolve.Context.SugarContext.QueryBuilder.LambdaExpressions.ParameterIndex++;
                isClearFilter=this.methodCallExpressionResolve.Context.SugarContext.QueryBuilder.IsDisabledGobalFilter;
                clearTypes = this.methodCallExpressionResolve.Context.SugarContext.QueryBuilder.RemoveFilters;
            }
            var sqlObj = queryable
                .AS(this.ProPertyEntity.DbTableName)
                .ClearFilter(clearTypes)
                .Filter(isClearFilter?null:this.ProPertyEntity.Type) 
                .WhereIF(!string.IsNullOrEmpty(whereSql), whereSql)
                .Where($" {name}={queryable.QueryBuilder.Builder.GetTranslationColumnName( ShorName)}.{pk} ").Select(MethodName == "Any" ? "1" : " COUNT(1) ").ToSql();
            if (sqlObj.Value?.Any() == true)
            {
                foreach (var item in sqlObj.Value)
                {
                    if (!this.methodCallExpressionResolve.Context.Parameters.Any(it => it.ParameterName == item.ParameterName)) 
                    {
                        this.methodCallExpressionResolve.Context.Parameters.Add(item);
                    } 
                }
            }
            mapper.Sql = sqlObj.Key;
            mapper.Sql = $" ({mapper.Sql}) ";
            mapper.Sql = GetMethodSql(mapper.Sql);
            return mapper;
        }

        private string GetMethodSql(string sql)
        {
            if (MethodName == "Any") 
            {
                return $" ( EXISTS {sql} ) ";
            }
            return sql;
        }

    }
}
