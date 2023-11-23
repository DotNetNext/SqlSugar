using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Dynamic;
using System.Threading.Tasks; 

namespace SqlSugar
{

    public partial class QueryableProvider<T> : QueryableAccessory, ISugarQueryable<T>
    {
       
        public ISugarQueryable<T> CrossQuery<Type>(string configId)
        {
            return this.CrossQuery(typeof(Type),configId);
        }
        public ISugarQueryable<T> CrossQuery(Type type, string configId)
        {
            if (this.QueryBuilder.CrossQueryItems == null) 
            {
                this.QueryBuilder.CrossQueryItems = new Dictionary<string, string>();
            }
            if(!this.QueryBuilder.CrossQueryItems.ContainsKey(type.FullName))
               this.QueryBuilder.CrossQueryItems.Add(type.FullName, configId);
            return this;
        }
        public ISugarQueryable<T> IncludeLeftJoin(Expression<Func<T, object>> leftObjectExp)
        {
            MemberExpression memberExpression;
            string navObjectName;
            EntityColumnInfo navColumn, navPkColumn;
            EntityInfo navEntityInfo;
            ExpressionTool.GetOneToOneInfo(this.Context, leftObjectExp, out memberExpression, out navObjectName, out navColumn, out navEntityInfo, out navPkColumn);
            var shortName = $"pnv_{navObjectName}";
            var mainShortName = memberExpression.Expression.ToString();
            this.QueryBuilder.TableShortName = mainShortName;
            var onWhere = $"{shortName}.{navPkColumn.DbColumnName}={mainShortName}.{navColumn.DbColumnName}";
            UtilMethods.IsNullReturnNew(this.Context.TempItems);
            this.AddJoinInfo(GetTableName(navEntityInfo, navEntityInfo.DbTableName), shortName, onWhere, JoinType.Left);
            this.QueryBuilder.JoinQueryInfos.Last().EntityType = navEntityInfo.Type;
            return this;
        }
        public ISugarQueryable<T> IncludeInnerJoin(Expression<Func<T, object>> innerObjectExt)
        {
            MemberExpression memberExpression;
            string navObjectName;
            EntityColumnInfo navColumn, navPkColumn;
            EntityInfo navEntityInfo;
            ExpressionTool.GetOneToOneInfo(this.Context, innerObjectExt, out memberExpression, out navObjectName, out navColumn, out navEntityInfo, out navPkColumn);
            var shortName = $"pnv_{navObjectName}";
            var mainShortName = memberExpression.Expression.ToString();
            this.QueryBuilder.TableShortName = mainShortName;
            var onWhere = $"{shortName}.{navPkColumn.DbColumnName}={mainShortName}.{navColumn.DbColumnName}";
            UtilMethods.IsNullReturnNew(this.Context.TempItems);
            this.AddJoinInfo(GetTableName(navEntityInfo, navEntityInfo.DbTableName), shortName, onWhere, JoinType.Inner);
            this.QueryBuilder.JoinQueryInfos.Last().EntityType = navEntityInfo.Type;
            return this;
        }
        public ISugarQueryable<T> IncludeFullJoin(Expression<Func<T, object>> fullObjectExp)
        {
            MemberExpression memberExpression;
            string navObjectName;
            EntityColumnInfo navColumn, navPkColumn;
            EntityInfo navEntityInfo;
            ExpressionTool.GetOneToOneInfo(this.Context, fullObjectExp, out memberExpression, out navObjectName, out navColumn, out navEntityInfo, out navPkColumn);
            var shortName = $"pnv_{navObjectName}";
            var mainShortName = memberExpression.Expression.ToString();
            this.QueryBuilder.TableShortName = mainShortName;
            var onWhere = $"{shortName}.{navPkColumn.DbColumnName}={mainShortName}.{navColumn.DbColumnName}";
            UtilMethods.IsNullReturnNew(this.Context.TempItems);
            this.AddJoinInfo(GetTableName(navEntityInfo, navEntityInfo.DbTableName), shortName, onWhere, JoinType.Full);
            this.QueryBuilder.JoinQueryInfos.Last().EntityType = navEntityInfo.Type;
            return this;
        }
        public ISugarQueryable<T> IncludeRightJoin(Expression<Func<T, object>> rightObjectExp)
        {
            MemberExpression memberExpression;
            string navObjectName;
            EntityColumnInfo navColumn, navPkColumn;
            EntityInfo navEntityInfo;
            ExpressionTool.GetOneToOneInfo(this.Context, rightObjectExp, out memberExpression, out navObjectName, out navColumn, out navEntityInfo, out navPkColumn);
            var shortName = $"pnv_{navObjectName}";
            var mainShortName = memberExpression.Expression.ToString();
            this.QueryBuilder.TableShortName = mainShortName;
            var onWhere = $"{shortName}.{navPkColumn.DbColumnName}={mainShortName}.{navColumn.DbColumnName}";
            UtilMethods.IsNullReturnNew(this.Context.TempItems);
            this.AddJoinInfo(GetTableName(navEntityInfo, navEntityInfo.DbTableName), shortName, onWhere, JoinType.Right);
            this.QueryBuilder.JoinQueryInfos.Last().EntityType = navEntityInfo.Type;
            return this;
        }
        public ISugarQueryable<T, T2> LeftJoinIF<T2>(bool isJoin, ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression) 
        {
            var result = LeftJoin(joinQueryable, joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2> LeftJoin<T2>(ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression)
        {

            if (MasterHasWhereFirstJoin())
            {
                return this.MergeTable().LeftJoin<T2>(joinQueryable, joinExpression);
            }

            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2> InnerJoin<T2>(ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression)
        {
            if (MasterHasWhereFirstJoin())
            {
                return this.MergeTable().InnerJoin<T2>(joinQueryable,joinExpression);
            }

            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2> RightJoin<T2>(ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression)
        {

            if (MasterHasWhereFirstJoin())
            {
                return this.MergeTable().RightJoin<T2>(joinQueryable, joinExpression);
            }

            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2> FullJoin<T2>(ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression) 
        {

            if (MasterHasWhereFirstJoin())
            {
                return this.MergeTable().FullJoin<T2>(joinQueryable,joinExpression);
            }

            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Full);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> joinExpression,string tableName) 
        {
           var result= LeftJoin<T2>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2> LeftJoinIF<T2>(bool isLeftJoin, Expression<Func<T, T2, bool>> joinExpression) 
        {
            var result = LeftJoin(joinExpression);
            if (isLeftJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2> InnerJoinIF<T2>(bool isJoin, Expression<Func<T, T2, bool>> joinExpression)
        {
            var result = InnerJoin(joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> joinExpression)
        {
            if (MasterHasWhereFirstJoin())
            {
                return this.MergeTable().LeftJoin<T2>(joinExpression);
            }

            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            this.QueryBuilder.IsSqlQuery = false;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression,JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2> FullJoin<T2>(Expression<Func<T, T2, bool>> joinExpression)
        {
            if (MasterHasWhereFirstJoin())
            {
                return this.MergeTable().FullJoin<T2>(joinExpression);
            }

            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2> FullJoin<T2>(Expression<Func<T, T2, bool>> joinExpression, string tableName)
        {
            var result = FullJoin<T2>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> joinExpression)
        {

            if (MasterHasWhereFirstJoin())
            {
                return this.MergeTable().RightJoin<T2>(joinExpression);
            }

            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }
        public ISugarQueryable<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> joinExpression, string tableName)
        {
            var result = RightJoin<T2>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> joinExpression)
        {

            if (MasterHasWhereFirstJoin())
            {
                return this.MergeTable().InnerJoin<T2>(joinExpression);
            }

            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        public ISugarQueryable<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> joinExpression, string tableName)
        {
            var result = InnerJoin<T2>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public void Clear()
        {
            QueryBuilder.Clear();
        }
        public ISugarQueryable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = QueryBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it).ToLower()).ToList();
            return IgnoreColumns(ignoreColumns.ToArray());
        }
        public ISugarQueryable<T> IgnoreColumns(params string[] columns)
        {
            if (QueryBuilder.IgnoreColumns.IsNullOrEmpty())
            {
                QueryBuilder.IgnoreColumns = new List<string>();
            }
            QueryBuilder.IgnoreColumns.AddRange(columns);
            return this;
        }
        public void AddQueue()
        {
            var sqlObj = this.ToSql();
            this.Context.Queues.Add(sqlObj.Key, sqlObj.Value);
        }
        public ISugarQueryable<T> Clone()
        {
            var queryable = this.Context.Queryable<object>().Select<T>().WithCacheIF(IsCache, CacheTime);
            CopyQueryBuilder(queryable.QueryBuilder);
            ((QueryableProvider<T>)queryable).CacheKey = this.CacheKey;
            ((QueryableProvider<T>)queryable).MapperAction = this.MapperAction;
            ((QueryableProvider<T>)queryable).MapperActionWithCache = this.MapperActionWithCache;
            ((QueryableProvider<T>)queryable).Mappers = this.Mappers;
            return queryable;
        }
        public ISugarQueryable<T> Hints(string hints) 
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public virtual ISugarQueryable<T> AS<T2>(string tableName)
        {
            var entityName = typeof(T2).Name;
            return _As(tableName, entityName);
        }
        public ISugarQueryable<T> AS(string tableName)
        {
            if (tableName == null) return this;
            var entityName = typeof(T).Name;
            return _As(tableName, entityName);
        }
        public ISugarQueryable<T> AsWithAttr() 
        {
            var asName=GetTableName(this.EntityInfo, this.EntityInfo.DbTableName);
            this.QueryBuilder.IsCrossQueryWithAttr = true;
            return this.AS(asName);
        }
        public ISugarQueryable<T> AsType(Type tableNameType)
        {
            return AS(this.Context.EntityMaintenance.GetEntityInfo(tableNameType).DbTableName);
        }
        public virtual ISugarQueryable<T> With(string withString)
        {
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
            {
                QueryBuilder.TableWithString = withString;
            }
            return this;
        }

        public virtual ISugarQueryable<T> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }

        public ISugarQueryable<T> ClearFilter() 
        {
            this.Filter(null, true);
            return this;
        }
        public ISugarQueryable<T> ClearFilter(params Type[] types)
        {
            if (types == null|| types.Length==0) 
            {
                return this;
            }
            this.QueryBuilder.RemoveFilters = types;
            this.Filter(null, true);
            this.QueryBuilder.IsDisabledGobalFilter = false;
            return this;
        }
        public ISugarQueryable<T> ClearFilter<FilterType1>() 
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public ISugarQueryable<T> ClearFilter<FilterType1, FilterType2>() 
        {
            this.ClearFilter(typeof(FilterType1),typeof(FilterType2));
            return this;
        }
        public ISugarQueryable<T> ClearFilter<FilterType1, FilterType2, FilterType3>() 
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2),typeof(FilterType3));
            return this;
        }
        public ISugarQueryable<T> Filter(Type type)
        {
            if (type == null) 
            {
                return this;
            }
            this.Context.InitMappingInfo(type);
            var whereString= QueryBuilder.GetFilters(type);
            if (whereString.HasValue())
            {
                this.Where(whereString);
            }
            UtilMethods.AddDiscrimator(type,this);
            return this;
        }

        public virtual ISugarQueryable<T> Mapper(Action<T> mapperAction)
        {
            this.MapperAction=UtilMethods.IsNullReturnNew(this.MapperAction);
            this.MapperAction.Add(mapperAction);
            return this;
        }
        public ISugarQueryable<T> Mapper<AType, BType, MappingType>(Expression<Func<MappingType, ManyToMany>> expression)
        {
            var args = ((expression as LambdaExpression).Body as MethodCallExpression).Arguments;

            Type aType = typeof(AType);
            Type bType = typeof(BType);
            Type bListType = typeof(List<BType>);

            this.Context.InitMappingInfo(aType);
            this.Context.InitMappingInfo(bType);
            this.Context.InitMappingInfo(typeof(MappingType));

            //Mapping 
            var mappingEntity = this.Context.EntityMaintenance.GetEntityInfo(typeof(MappingType));
            string m_aPropertyName = (args[0] as MemberExpression).Member.Name;
            string m_bPropertyName= (args[1] as MemberExpression).Member.Name;
            var m_aDbField = mappingEntity.Columns.First(it => it.PropertyName == m_aPropertyName).DbColumnName;
            var m_bDbField = mappingEntity.Columns.First(it => it.PropertyName == m_bPropertyName).DbColumnName;

            //A
            var aEntity = this.Context.EntityMaintenance.GetEntityInfo(aType);
            var aPropertyName = aEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true)?.PropertyName;
            Check.Exception(aPropertyName == null, aEntity.EntityName + " no primary key");

            //B
            var bEntity = this.Context.EntityMaintenance.GetEntityInfo(bType);
            var bProperty = bEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true)?.PropertyName;
            Check.Exception(bProperty == null, bEntity.EntityName + " no primary key");
            var bDbFiled = bEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true).DbColumnName;
            this.Mapper((it,cache) =>
            {
               var list= cache.Get<Dictionary<object, List<BType>>>(oldList=> {


                   //query mapping by a
                   var cons = new List<IConditionalModel>() {
                     new ConditionalModel(){      
                                                   ConditionalType=ConditionalType.In,
                                                   FieldName= m_aDbField,
                                                   FieldValue=string.Join(",",oldList.Select(z=>UtilMethods.GetPropertyValue(z,aPropertyName)).Distinct())
                                                  }
                   };
                   var mappingList = this.Context.Queryable<MappingType>().Where(cons).ToList();
                   var bids = mappingList.Select(z => UtilMethods.GetPropertyValue(z, m_bPropertyName)).Distinct().ToList();

                   //queryable b by mapping
                   cons = new List<IConditionalModel>() {
                     new ConditionalModel(){
                                                   ConditionalType=ConditionalType.In,
                                                   FieldName= bDbFiled,
                                                   FieldValue=string.Join(",",mappingList.Select(z=>UtilMethods.GetPropertyValue(z,m_bPropertyName)).Distinct())
                                                  }
                   };
                   List<BType> bList = new List<BType>();
                   if (mappingList.Any())
                   {
                       bList=this.Context.Queryable<BType>().Where(cons).ToList();
                   }

                   //get result
                   Dictionary<object, List<BType>> result = new Dictionary<object, List<BType>>();
                   var group = mappingList.GroupBy(z => UtilMethods.GetPropertyValue(z, m_aPropertyName));
                   foreach (var item in group)
                   {
                       var currentBids = item.Select(z => UtilMethods.GetPropertyValue(z, m_bPropertyName)).ToList();
                       result.Add(item.Key, bList.Where(z => currentBids.Contains(UtilMethods.GetPropertyValue(z, bProperty))).ToList());
                   }
                   return result;

               }, expression.ToString());
               foreach (var item in aEntity.Columns)
               {
                    var aid = UtilMethods.GetPropertyValue(it, aPropertyName);
                    if (list.ContainsKey(aid))
                    {
                        if (item.PropertyInfo.PropertyType == bType)
                        {
                           var b=UtilMethods.ChangeType<BType>(list[aid].FirstOrDefault());
                            item.PropertyInfo.SetValue(it, b);
                        }
                        else if (item.PropertyInfo.PropertyType == bListType)
                        {
                            var bList = UtilMethods.ChangeType<List<BType>>(list[aid]);
                            item.PropertyInfo.SetValue(it, bList);
                        }
                    }
               }
            });
            return this;
        }

        public virtual ISugarQueryable<T> Mapper(Action<T, MapperCache<T>> mapperAction)
        {
            this.MapperActionWithCache += mapperAction;
            return this;
        }
        public ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, TObject>> mapperObject, Expression<Func<T, object>> mainField, Expression<Func<T, object>> childField)
        {
            Check.Exception(mapperObject.ReturnType.Name == "IList`1", "Mapper no support IList , Use List<T>");
            if (CallContext.MapperExpression.Value == null)
            {
                CallContext.MapperExpression.Value = new List<MapperExpression>();
            }
            CallContext.MapperExpression.Value.Add(new MapperExpression() { SqlBuilder = SqlBuilder, QueryBuilder = this.QueryBuilder, Type = MapperExpressionType.oneToOne, FillExpression = mapperObject, MappingField1Expression = mainField, MappingField2Expression = childField, Context = this.Context });
            return _Mapper<TObject>(mapperObject, mainField, childField);
        }
        public ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, List<TObject>>> mapperObject, Expression<Func<T, object>> mainField, Expression<Func<T, object>> childField)
        {
            return _Mapper<TObject>(mapperObject, mainField, childField);
        }
        public virtual ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, List<TObject>>> mapperObject, Expression<Func<T, object>> mapperField)
        {
            Check.Exception(mapperObject.ReturnType.Name == "IList`1", "Mapper no support IList , Use List<T>");
            if (CallContext.MapperExpression.Value == null)
            {
                CallContext.MapperExpression.Value = new List<MapperExpression>();
            }
            CallContext.MapperExpression.Value.Add(new MapperExpression() { SqlBuilder = SqlBuilder, QueryBuilder = this.QueryBuilder, Type = MapperExpressionType.oneToN, FillExpression = mapperObject, MappingField1Expression = mapperField, Context = this.Context });
            return _Mapper<TObject>(mapperObject, mapperField);
        }
        public virtual ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, TObject>> mapperObject, Expression<Func<T, object>> mapperField)
        {
            if (CallContext.MapperExpression.Value == null)
            {
                CallContext.MapperExpression.Value = new List<MapperExpression>();
            }
            CallContext.MapperExpression.Value.Add(new MapperExpression() { SqlBuilder = SqlBuilder, QueryBuilder = this.QueryBuilder, Type = MapperExpressionType.oneToOne, FillExpression = mapperObject, MappingField1Expression = mapperField, Context = this.Context });
            return _Mapper<TObject>(mapperObject, mapperField);
        }
        public ISugarQueryable<T> Where(string fieldName, string conditionalType, object fieldValue)
        {
            string parameterName = fieldName.Replace(".","_")+ this.QueryBuilder.WhereIndex;
            var whereSql = this.SqlBuilder.GetWhere(fieldName, conditionalType, this.QueryBuilder.WhereIndex);
            this.Where(whereSql);
            this.QueryBuilder.WhereIndex++;
            this.QueryBuilder.Parameters.Add(new SugarParameter(parameterName, fieldValue));
            return this;
        }
        public virtual ISugarQueryable<T> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public virtual ISugarQueryable<T> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public virtual ISugarQueryable<T> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public virtual ISugarQueryable<T> AddParameters(SugarParameter parameter)
        {
            if (parameter != null)
                QueryBuilder.Parameters.Add(parameter);
            return this;
        }
        public ISugarQueryable<T> AddJoinInfo(Type JoinType, Dictionary<string, Type> keyIsShortName_ValueIsType_Dictionary, FormattableString onExpString, JoinType type = JoinType.Left) 
        {
            var whereExp = DynamicCoreHelper.GetWhere(keyIsShortName_ValueIsType_Dictionary,onExpString);
            var name=whereExp.Parameters.Last(it => it.Type == JoinType).Name;
            var sql = this.QueryBuilder.GetExpressionValue(whereExp, ResolveExpressType.WhereMultiple).GetResultString();
            return AddJoinInfo(JoinType, name, sql,type);
        }
        public ISugarQueryable<T> AddJoinInfo(Type JoinType, string shortName, string joinWhere, JoinType type = JoinType.Left) 
        {
            this.Context.InitMappingInfo(JoinType);
            var tableName = this.Context.EntityMaintenance.GetEntityInfo(JoinType).DbTableName; 
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere,
                    EntityType=JoinType
                });
            return this;
        }
        public virtual ISugarQueryable<T> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {

            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }

        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public ISugarQueryable<T> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            return WhereClass(new List<ClassType>() { whereClass }, ignoreDefaultValue);
        }
        public ISugarQueryable<T> WhereClassByPrimaryKey(List<T> list)
        {
            _WhereClassByPrimaryKey(list);
            return this;
        }

        public ISugarQueryable<T> WhereClassByWhereColumns(List<T> list, string[] whereColumns) 
        {
            _WhereClassByWhereColumns(list,whereColumns);
            return this;
        }
        public ISugarQueryable<T> WhereClassByPrimaryKey(T data)
        {
            _WhereClassByPrimaryKey(new List<T>() { data });
            return this;
        }
        public ISugarQueryable<T> TranLock(DbLockType? LockType = DbLockType.Wait) 
        {
            if (LockType == null) return this;
            Check.ExceptionEasy(this.Context.Ado.Transaction == null, "need BeginTran", "需要事务才能使用TranLock");
            Check.ExceptionEasy(this.QueryBuilder.IsSingle()==false, "TranLock, can only be used for single table query", "TranLock只能用在单表查询");
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
            {
                if (LockType == DbLockType.Wait)
                {
                    this.With("WITH(UpdLock,RowLock)");
                }
                else
                {
                    this.With("WITH(UpdLock,RowLock,NoWait)");
                }
            }
            else 
            {
                this.QueryBuilder.TranLock = (LockType == DbLockType.Error? " for update nowait" : " for update");
            }
            return this;
        }
        public ISugarQueryable<T> WhereColumns(Dictionary<string, object>  dictionary) 
        {
            return WhereColumns(new List<Dictionary<string, object>> { dictionary });
        }
        public ISugarQueryable<T> WhereColumns(Dictionary<string, object> columns, bool ignoreDefaultValue) 
        {
            if (ignoreDefaultValue == false || columns == null)
            {
                return WhereColumns(columns);
            }
            else 
            {
                var newColumns = new Dictionary<string, object>();
                foreach (var item in columns)
                {
                    if (!UtilMethods.IsDefaultValue(item.Value)) 
                    {
                        newColumns.Add(item.Key, item.Value);
                    }
                }
                return WhereColumns(newColumns);
            }
        }
        public ISugarQueryable<T> WhereColumns(List<Dictionary<string, object>> list)
        {
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
            foreach (var model in list)
            {
                int i = 0;
                var clist = new List<KeyValuePair<WhereType, ConditionalModel>>();
                foreach (var item in model.Keys)
                {
                    var value = model[item] == null ? "null" : model[item].ObjToString();
                    var csType = model[item] == null ? null : model[item].GetType().Name;
                    if (model[item] is Enum&&this.Context?.CurrentConnectionConfig?.MoreSettings?.TableEnumIsString!=true) 
                    {
                        value = Convert.ToInt64(model[item])+"";
                        csType = "Int64";
                    }
                    clist.Add(new KeyValuePair<WhereType, ConditionalModel>(i == 0 ? WhereType.Or : WhereType.And, new ConditionalModel()
                    {
                        FieldName = item,
                        ConditionalType = ConditionalType.Equal,
                        FieldValue = value,
                        CSharpTypeName = csType
                    }));
                    i++;
                }
                conditionalModels.Add(new ConditionalCollections()
                {
                    ConditionalList = clist
                });
            }
            return this.Where(conditionalModels);
        }

        /// <summary>
        ///  if a property that is primary key is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public ISugarQueryable<T> _WhereClassByPrimaryKey(List<T> whereClassTypes)
        {

            if (whereClassTypes.HasValue())
            {
                var columns = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsIgnore == false && it.IsPrimarykey == true).ToList();
                Check.Exception(columns == null || columns.Count == 0, "{0} no primary key, Can not use WhereClassByPrimaryKey ", typeof(T).Name);
                Check.Exception(this.QueryBuilder.IsSingle() == false, "No support join query");
                List<IConditionalModel> whereModels = new List<IConditionalModel>();
                foreach (var item in whereClassTypes)
                {
                    var cons = new ConditionalCollections();
                    foreach (var column in columns)
                    {
                        WhereType WhereType = WhereType.And;
                        var value = column.PropertyInfo.GetValue(item, null);
                        if (cons.ConditionalList == null)
                        {
                            cons.ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>();
                            if (QueryBuilder.WhereInfos.IsNullOrEmpty() && whereModels.IsNullOrEmpty())
                            {

                            }
                            else
                            {
                                WhereType = WhereType.Or;
                            }
                        }
                        var data = new KeyValuePair<WhereType, ConditionalModel>(WhereType, new ConditionalModel()
                        {
                            ConditionalType = ConditionalType.Equal,
                            FieldName = this.QueryBuilder.Builder.GetTranslationColumnName(column.DbColumnName),
                            FieldValue = value.ObjToStringNew(),
                            CSharpTypeName = column.UnderType.Name
                        });
                        if (value is Enum && this.Context.CurrentConnectionConfig?.MoreSettings?.TableEnumIsString != true)
                        {
                            data.Value.FieldValue = Convert.ToInt64(value).ObjToString();
                            data.Value.CSharpTypeName = "int";
                        }
                        else if (value != null&&column.UnderType==UtilConstants.DateType) 
                        {
                            data.Value.FieldValue = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        }
                        //if (this.Context.CurrentConnectionConfig.DbType == DbType.PostgreSQL) 
                        //{
                        //    data.Value.FieldValueConvertFunc = it =>
                        //    {
                        //        return UtilMethods.ChangeType2(it, value.GetType());
                        //    };
                        //}
                        cons.ConditionalList.Add(data);
                    }
                    if (cons.HasValue())
                    {
                        whereModels.Add(cons);
                    }
                }
                this.Where(whereModels,true);
            }
            else
            {
                this.Where(" 1=2 ");
            }
            return this;
        }
        /// <summary>
        ///  if a property that is whereColumns key is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public ISugarQueryable<T> _WhereClassByWhereColumns(List<T> whereClassTypes,string[] whereColumns)
        {

            if (whereClassTypes.HasValue())
            {
                var columns = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => whereColumns.Any(x=>x==it.PropertyName)|| whereColumns.Any(x => x.EqualCase(it.DbColumnName))).ToList();
                Check.Exception(columns == null || columns.Count == 0, "{0} no primary key, Can not use whereColumns ", typeof(T).Name);
                Check.Exception(this.QueryBuilder.IsSingle() == false, "No support join query");
                List<IConditionalModel> whereModels = new List<IConditionalModel>();
                foreach (var item in whereClassTypes)
                {
                    var cons = new ConditionalCollections();
                    foreach (var column in columns)
                    {
                        WhereType WhereType = WhereType.And;
                        var value = column.PropertyInfo.GetValue(item, null);
                        if (cons.ConditionalList == null)
                        {
                            cons.ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>();
                            if (QueryBuilder.WhereInfos.IsNullOrEmpty() && whereModels.IsNullOrEmpty())
                            {

                            }
                            else
                            {
                                WhereType = WhereType.Or;
                            }
                        }
                        var data = new KeyValuePair<WhereType, ConditionalModel>(WhereType, new ConditionalModel()
                        {
                            ConditionalType = ConditionalType.Equal,
                            FieldName = this.QueryBuilder.Builder.GetTranslationColumnName(column.DbColumnName),
                            FieldValue = value.ObjToStringNew(),
                            CSharpTypeName = column.PropertyInfo.PropertyType.Name
                        });
                        if (value is Enum && this.Context.CurrentConnectionConfig?.MoreSettings?.TableEnumIsString != true)
                        {
                            data.Value.FieldValue = Convert.ToInt64(value).ObjToString();
                            data.Value.CSharpTypeName = "int";
                        }
                        //if (this.Context.CurrentConnectionConfig.DbType == DbType.PostgreSQL) 
                        //{
                        //    data.Value.FieldValueConvertFunc = it =>
                        //    {
                        //        return UtilMethods.ChangeType2(it, value.GetType());
                        //    };
                        //}
                        cons.ConditionalList.Add(data);
                    }
                    if (cons.HasValue())
                    {
                        whereModels.Add(cons);
                    }
                }
                this.Where(whereModels, true);
            }
            else
            {
                this.Where(" 1=2 ");
            }
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public ISugarQueryable<T> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            if (whereClassTypes.HasValue())
            {
                var columns = this.Context.EntityMaintenance.GetEntityInfo<ClassType>().Columns.Where(it => it.IsIgnore == false).ToList();
                List<IConditionalModel> whereModels = new List<IConditionalModel>();
                foreach (var item in whereClassTypes)
                {
                    var cons = new ConditionalCollections();
                    foreach (var column in columns)
                    {

                        var value = column.PropertyInfo.GetValue(item, null);
                        WhereType WhereType = WhereType.And;
                        var isNotNull = ignoreDefaultValue == false && value != null;
                        var isNotNullAndDefault = ignoreDefaultValue && value != null && value.ObjToString() != UtilMethods.DefaultForType(column.PropertyInfo.PropertyType).ObjToString();
                        if (isNotNull || isNotNullAndDefault)
                        {
                            if (cons.ConditionalList == null)
                            {
                                cons.ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>();
                                if (QueryBuilder.WhereInfos.IsNullOrEmpty() && whereModels.IsNullOrEmpty())
                                {

                                }
                                else
                                {
                                    WhereType = WhereType.Or;
                                }

                            }
                            var data = new KeyValuePair<WhereType, ConditionalModel>(WhereType, new ConditionalModel()
                            {
                                ConditionalType = ConditionalType.Equal,
                                FieldName = column.DbColumnName,
                                FieldValue = value.ObjToString(),
                                CSharpTypeName=column.UnderType.Name
                            });
                            if (value != null && value.GetType().IsEnum())
                            {
                                if (this.Context.CurrentConnectionConfig?.MoreSettings?.TableEnumIsString == true)
                                {

                                }
                                else
                                {
                                    data.Value.FieldValue = Convert.ToInt64(value).ObjToString();
                                }

                            }
                            else if (value != null && column.PropertyInfo.PropertyType == UtilConstants.DateType) 
                            {
                                data.Value.FieldValue = value.ObjToDate().ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                            }
                            cons.ConditionalList.Add(data);
                            if (this.Context.CurrentConnectionConfig.DbType == DbType.PostgreSQL)
                            {
                                data.Value.FieldValueConvertFunc = it =>
                                {
                                    return UtilMethods.ChangeType2(it, value.GetType());
                                };
                            }
                        }
                    }
                    if (cons.HasValue())
                    {
                        whereModels.Add(cons);
                    }
                }
                this.Where(whereModels);
            }
            return this;
        }
        public ISugarQueryable<T> Where(Dictionary<string, Type> keyIsShortName_ValueIsType_Dictionary, FormattableString expressionString) 
        {
            var exp = DynamicCoreHelper.GetWhere(keyIsShortName_ValueIsType_Dictionary, expressionString);
            _Where(exp);
            return this;
        }
        public virtual ISugarQueryable<T> Where(string expShortName, FormattableString expressionString) 
        {
            if (expressionString == null&& !Regex.IsMatch(expShortName,@"^\w$")) 
            {
                return this.Where(expShortName, new { });
            }

            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
        public virtual ISugarQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            if (IsSingleWithChildTableQuery())
            {
                expression = ReplaceMasterTableParameters(expression);
            }
            this._Where(expression);
            return this;
        }

        public virtual ISugarQueryable<T> Where(string whereString, object whereObj = null)
        {
            if (whereString.HasValue())
                this.Where<T>(whereString, whereObj);
            return this;
        }
        public virtual ISugarQueryable<T> Where(IFuncModel funcModel) 
        {
            var obj= this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public virtual ISugarQueryable<T> Where(List<IConditionalModel> conditionalModels)
        {
            if (conditionalModels.IsNullOrEmpty()) return this;
            var sqlObj = this.SqlBuilder.ConditionalModelToSql(conditionalModels,0);
            if (sqlObj.Value != null && this.QueryBuilder.Parameters != null) 
            {
                if (sqlObj.Value.Any(it => this.QueryBuilder.Parameters.Any(z => z.ParameterName.EqualCase(it.ParameterName)))) 
                {
                    var sql = sqlObj.Key;
                    this.SqlBuilder.RepairReplicationParameters(ref sql,sqlObj.Value,this.QueryBuilder.Parameters.Count*10);
                    return this.Where(sql, sqlObj.Value);
                }
            }
            return this.Where(sqlObj.Key, sqlObj.Value);
        }
        public ISugarQueryable<T> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            if (conditionalModels.IsNullOrEmpty()) return this;
            var sqlObj = this.SqlBuilder.ConditionalModelToSql(conditionalModels, 0);
            if (isWrap)
            {
                return this.Where("("+sqlObj.Key+")", sqlObj.Value);
            }
            else 
            {
                return this.Where(sqlObj.Key, sqlObj.Value);
            }
        }
        public virtual ISugarQueryable<T> Where<T2>(string whereString, object whereObj = null)
        {
            var whereValue = QueryBuilder.WhereInfos;
            whereValue.Add(SqlBuilder.AppendWhereOrAnd(whereValue.Count == 0, whereString + UtilConstants.Space));
            if (whereObj != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(whereObj));
            return this;
        }

        public virtual ISugarQueryable<T> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T> HavingIF(bool isHaving,Expression<Func<T, bool>> expression)
        {
            if(isHaving)
             this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T> Having(string whereString, object parameters = null)
        {

            QueryBuilder.HavingInfos = SqlBuilder.AppendHaving(whereString);
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }

        public virtual ISugarQueryable<T> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (!isWhere) return this;
            if (IsSingleWithChildTableQuery())
            {
                expression = ReplaceMasterTableParameters(expression);
            }
            _Where(expression);
            return this;
        }
        public virtual ISugarQueryable<T> WhereIF(bool isWhere, string whereString, object whereObj = null)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }

        public virtual T InSingle(object pkValue)
        {
            if (pkValue == null) 
            {
                pkValue = -1;
            }
            Check.Exception(this.QueryBuilder.SelectValue.HasValue(), "'InSingle' and' Select' can't be used together,You can use .Select(it=>...).Single(it.id==1)");
            var list = In(pkValue).ToList();
            if (list == null) return default(T);
            else return list.SingleOrDefault();
        }
        public ISugarQueryable<T> InIF<TParamter>(bool isIn,string fieldName, params TParamter[] pkValues) 
        {
            if (isIn)
            {
                return In(fieldName, pkValues);
            }
            else 
            {
                return this;
            } 
        }
        public ISugarQueryable<T> InIF<TParamter>(bool isIn, params TParamter[] pkValues) 
        {
            if (isIn) 
            {
                In(pkValues);
            }
            return this;
        }
        public virtual ISugarQueryable<T> In<TParamter>(params TParamter[] pkValues)
        {
            if (pkValues == null || pkValues.Length == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            if (pkValues.Length == 1 && pkValues.First().GetType().FullName.IsCollectionsList() || (pkValues.First() is IEnumerable && pkValues.First().GetType() != UtilConstants.StringType))
            {
                var newValues = new List<object>();
                foreach (var item in pkValues.First() as IEnumerable)
                {
                    newValues.Add(item);
                }
                return In(newValues);
            }
            var pks = GetPrimaryKeys().Select(it => SqlBuilder.GetTranslationTableName(it)).ToList();
            Check.Exception(pks == null || pks.Count != 1, "Queryable.In(params object[] pkValues): Only one primary key");
            string filed = pks.FirstOrDefault();
            string shortName = QueryBuilder.TableShortName == null ? null : (QueryBuilder.TableShortName + ".");
            filed = shortName + filed;
            return In(filed, pkValues);
        }
        public virtual ISugarQueryable<T> In<FieldType>(string filed, params FieldType[] inValues)
        {
            if (inValues.Length == 1)
            {
                if (inValues.GetType().IsArray)
                {
                    var whereIndex = QueryBuilder.WhereIndex;
                    string parameterName = this.SqlBuilder.SqlParameterKeyWord + "InPara" + whereIndex;
                    this.AddParameters(new SugarParameter(parameterName, inValues[0]));
                    this.Where(string.Format(QueryBuilder.EqualTemplate,SqlBuilder.GetTranslationColumnName(filed), parameterName));
                    QueryBuilder.WhereIndex++;
                }
                else
                {
                    var values = new List<object>();
                    foreach (var item in ((IEnumerable)inValues[0]))
                    {
                        if (item != null)
                        {
                            values.Add(item.ToString().ToSqlValue());
                        }
                    }
                    this.Where(string.Format(QueryBuilder.InTemplate, SqlBuilder.GetTranslationColumnName(filed), string.Join(",", values)));
                }
            }
            else
            {
                var values = new List<object>();
                foreach (var item in inValues)
                {
                    if (item != null)
                    {
                        if (UtilMethods.IsNumber(item.GetType().Name))
                        {
                            values.Add(item.ToString());
                        }
                        else
                        {
                            values.Add(item.ToString().ToSqlValue());
                        }
                    }
                }
                this.Where(string.Format(QueryBuilder.InTemplate, SqlBuilder.GetTranslationColumnName(filed), string.Join(",", values)));

            }
            return this;
        }
        public virtual ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            var propertyName = ExpressionTool.GetMemberName(expression);
            var propertyColumn = this.EntityInfo.Columns.FirstOrDefault(it => it.PropertyName == propertyName);
            if (inValues?.Length==1&& inValues[0]?.GetType()?.FullName?.IsCollectionsList()==true && propertyColumn != null && propertyColumn?.UnderType?.FullName?.IsCollectionsList()!=true) 
            { 
                return In(fieldName, UtilMethods.ConvertToListOfObjects(inValues[0]));
            }
            else if (inValues?.Length == 1 && inValues[0]?.GetType()?.IsArray == true && propertyColumn != null && propertyColumn?.UnderType?.IsArray != true)
            {
                return In(fieldName, UtilMethods.ConvertToListOfObjects(inValues[0]));
            }
            else
            {
                return In(fieldName, inValues);
            }
        }
        public virtual ISugarQueryable<T> In<TParamter>(List<TParamter> pkValues)
        {
            if (pkValues == null || pkValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(pkValues.ToArray());
        }
        public virtual ISugarQueryable<T> In<FieldType>(string InFieldName, List<FieldType> inValues)
        {
            if (inValues == null || inValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(InFieldName, inValues.ToArray());
        }
        public virtual ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            if (inValues == null || inValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(expression, inValues.ToArray());
        }

        public ISugarQueryable<T> InIF<FieldType>(bool isWhere,Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression) 
        {
            if (isWhere)
            {
                var sqlObj = childQueryExpression.ToSql();
                _InQueryable(expression, sqlObj);
            }
            return this;
        }

        public virtual ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        public ISugarQueryable<T> SampleBy(int timeNumber, SampleByUnit timeType) 
        {
            SampleByUnit sampleBy = timeType;
            string sql = "SAMPLE BY "+timeNumber + sampleBy.ToString().Substring(0, 1).ToLower();
            this.QueryBuilder.SampleBy = sql;
            return this;
        }
        public ISugarQueryable<T> SampleBy(int timeNumber, string timeType)
        {
            string sql = "SAMPLE BY " + timeType;
            this.QueryBuilder.SampleBy = sql;
            return this;
        }
        public ISugarQueryable<T> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null) 
        {
            if (orderPropertyName.HasValue()) 
            {
                if (orderPropertyName.Contains(",")) 
                {
                    foreach (var item in orderPropertyName.Split(','))
                    {
                        this.OrderByPropertyName(item,orderByType);
                    }
                    return this;
                }
                if (this.QueryBuilder.IsSingle() == false && orderPropertyName.Contains(".")) 
                {
                    orderPropertyNameByJoin(orderPropertyName, orderByType);
                    return this;
                }
                if (this.Context.EntityMaintenance.GetEntityInfoWithAttr(typeof(T)).Columns.Any(it =>
                it.DbColumnName?.EqualCase(orderPropertyName)==true
                || it.PropertyName?.EqualCase(orderPropertyName)==true))
                {
                    var name = this.Context.EntityMaintenance.GetEntityInfoWithAttr(typeof(T)).Columns.FirstOrDefault(it =>
                it.DbColumnName?.EqualCase(orderPropertyName) == true
                || it.PropertyName?.EqualCase(orderPropertyName) == true)?.DbColumnName;
                    return this.OrderBy(this.SqlBuilder.GetTranslationColumnName( name) + " "+orderByType);
                }
                else 
                {
                    Check.ExceptionEasy($"OrderByPropertyName error.{orderPropertyName} does not exist in the entity class", $"OrderByPropertyName出错实体类中不存在{orderPropertyName}");
                }
            }
            return this;
        }
        public virtual ISugarQueryable<T> OrderBy(string orderFileds)
        {
            orderFileds = orderFileds.ToCheckField();
            var orderByValue = QueryBuilder.OrderByValue;
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.OrderByTemplate;
            }
            QueryBuilder.OrderByValue += string.IsNullOrEmpty(orderByValue) ? orderFileds : ("," + orderFileds);
            return this;
        }
        public virtual ISugarQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            this._OrderBy(expression, type);
            return this;
        }
        public virtual ISugarQueryable<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public virtual ISugarQueryable<T> GroupByIF(bool isGroupBy,Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }

        public ISugarQueryable<T> GroupByIF(bool isGroupBy, string groupFields)
        {
            if (isGroupBy)
            {
                GroupBy(groupFields);
            }

            return this;
        }


        public virtual ISugarQueryable<T> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                return this.OrderBy(orderFileds);
            else
                return this;
        }
        public virtual ISugarQueryable<T> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                return this.OrderBy(expression, type);
            else
                return this;
        }

        public virtual ISugarQueryable<T> GroupBy(string groupFileds)
        {
            groupFileds = groupFileds.ToCheckField();
            var croupByValue = QueryBuilder.GroupByValue;
            if (QueryBuilder.GroupByValue.IsNullOrEmpty())
            {
                QueryBuilder.GroupByValue = QueryBuilder.GroupByTemplate;
            }
            QueryBuilder.GroupByValue += string.IsNullOrEmpty(croupByValue) ? groupFileds : ("," + groupFileds);
            return this;
        }

        public virtual ISugarQueryable<T> PartitionBy(Expression<Func<T, object>> expression)
        {
            if (QueryBuilder.Take == null)
                QueryBuilder.Take = 1;
            _PartitionBy(expression);
            QueryBuilder.DisableTop = true;
            return this;
        }
        public virtual ISugarQueryable<T> PartitionBy(string groupFileds)
        {
            var partitionByValue = QueryBuilder.PartitionByValue;
            if (QueryBuilder.PartitionByValue.IsNullOrEmpty())
            {
                QueryBuilder.PartitionByValue = QueryBuilder.PartitionByTemplate;
            }
            QueryBuilder.PartitionByValue += string.IsNullOrEmpty(partitionByValue) ? groupFileds : ("," + groupFileds);
            return this;
        }

        public virtual ISugarQueryable<T> Skip(int num)
        {
            QueryBuilder.Skip = num;
            return this;
        }
        public virtual ISugarQueryable<T> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public virtual ISugarQueryable<TResult> Select<TResult>(Expression expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Dictionary<string, Type> keyIsShortName_ValueIsType_Dictionary, FormattableString expSelect, Type resultType) 
        {
            var exp = DynamicCoreHelper.GetMember(keyIsShortName_ValueIsType_Dictionary, resultType, expSelect);
            return _Select<TResult>(exp);
        }
        public ISugarQueryable<TResult> Select<TResult>(string expShortName, FormattableString expSelect, Type resultType) 
        {
            var exp = DynamicCoreHelper.GetMember(typeof(TResult), resultType, expShortName, expSelect);
            return _Select<TResult>(exp); 
        }
        public ISugarQueryable<TResult> Select<TResult>(string expShortName, FormattableString expSelect,Type EntityType, Type resultType)
        {
            var exp = DynamicCoreHelper.GetMember(EntityType, resultType, expShortName, expSelect);
            return _Select<TResult>(exp);
        }
        public ISugarQueryable<T> Select(string expShortName, FormattableString expSelect,Type resultType) 
        {
            return Select<T>(expShortName, expSelect, resultType);
        }
        public virtual ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool isAutoFill)
        {
            if (typeof(TResult).IsAnonymousType()) 
            {
                return Select(expression);
            }
            var clone = this.Select(expression).Clone();
            //clone.QueryBuilder.LambdaExpressions.Index = QueryBuilder.LambdaExpressions.Index+1;
            var ps = clone.QueryBuilder;
            var sql = ps.GetSelectValue;
            if (string.IsNullOrEmpty(sql) || sql.Trim() == "*")
            {
                return this.Select<TResult>(expression);
            }
           if (sql.StartsWith("*,")) 
           {
                var columns = this.Context.EntityMaintenance.GetEntityInfo<T>()
                         .Columns.Where(it => typeof(TResult).GetProperties().Any(s => s.Name.EqualCase(it.PropertyName))).Where(it => it.IsIgnore == false).ToList();
                if (columns.Any())
                {
                    sql = string.Join(",", columns.Select(it => $"{SqlBuilder.GetTranslationColumnName(it.DbColumnName)} AS  {SqlBuilder.GetTranslationColumnName(it.PropertyName)} "))
                         + "," + sql.TrimStart('*').TrimStart(',');
                }
            }
            if (this.QueryBuilder.TableShortName.IsNullOrEmpty()) 
            {
                this.QueryBuilder.TableShortName = clone.QueryBuilder.TableShortName;
            }
            this.QueryBuilder.Parameters = ps.Parameters;
            this.QueryBuilder.SubToListParameters = clone.QueryBuilder.SubToListParameters;
            this.QueryBuilder.LambdaExpressions.ParameterIndex = clone.QueryBuilder.LambdaExpressions.ParameterIndex;
            var parameters = (expression as LambdaExpression).Parameters;
            var columnsResult = this.Context.EntityMaintenance.GetEntityInfo<TResult>().Columns;
            sql = AppendSelect(this.EntityInfo.Columns,sql, parameters, columnsResult, 0);
            return this.Select<TResult>(sql);
        }

        public virtual ISugarQueryable<TResult> Select<TResult>()
        {
            var isJoin = this.QueryBuilder.JoinExpression != null;
            if (isJoin)
            {
                var selectValue = new SugarMapper(this.Context).GetSelectValue<TResult>(this.QueryBuilder);
                return this.Select<TResult>(selectValue);
            }
            else if (this.QueryBuilder.EntityType == UtilConstants.ObjType || (this.QueryBuilder.AsTables != null && this.QueryBuilder.AsTables.Count == 1)||this.QueryBuilder.EntityName!=this.QueryBuilder.EntityType.Name) 
            {
                if (this.QueryBuilder.SelectValue.HasValue()&& this.QueryBuilder.SelectValue.ObjToString().Contains("AS"))
                {
                    return this.Select<TResult>(this.QueryBuilder.SelectValue+"");
                }
                else
                {
                    return this.Select<TResult>(this.SqlBuilder.SqlSelectAll);
                }
            }
            else
            {
                var selects = this.QueryBuilder.GetSelectValueByString();
                if (selects.ObjToString().ToLower().IsContainsIn(".","("," as ")) 
                {
                    return this.Select<TResult>(selects);
                }
                var resultColumns=this.Context.EntityMaintenance.GetEntityInfo<TResult>().Columns;
                var dbColumns = this.EntityInfo.Columns.Where(it=>!it.IsIgnore);
                StringBuilder sb = new StringBuilder();
                foreach (var item in resultColumns)
                {
                    var firstColumn= dbColumns.FirstOrDefault(z =>
                    z.PropertyName.EqualCase(item.PropertyName) ||
                    z.DbColumnName.EqualCase(item.PropertyName));
                    if (firstColumn != null)
                    {
                        var dbColumnName = firstColumn.DbColumnName;
                        var AsName = item.PropertyName;
                        sb.Append($"{this.SqlBuilder.GetTranslationColumnName(dbColumnName)} AS {this.SqlBuilder.GetTranslationColumnName(AsName)} ,");
                    }
                }
                selects = sb.ToString().TrimEnd(',');
                if (selects == "")
                {
                    selects = "*";
                }
                return this.Select<TResult>(selects);
            }
        }

        public virtual ISugarQueryable<TResult> Select<TResult>(string selectValue)
        {
            var result = InstanceFactory.GetQueryable<TResult>(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.SqlBuilder = this.SqlBuilder;
            result.QueryBuilder.ResultType = this.QueryBuilder.ResultType;
            result.IsCache = this.IsCache;
            result.CacheTime = this.CacheTime;
            QueryBuilder.SelectValue = selectValue;
            if (this.IsAs)
            {
                ((QueryableProvider<TResult>)result).IsAs = true;
            }
            return result;
        }
        public virtual ISugarQueryable<T> Select(string selectValue)
        {
            QueryBuilder.SelectValue = selectValue;
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }
        public virtual ISugarQueryable<T> MergeTable()
        {
            if (IsSubToList())
            {
                return MergeTableWithSubToList();
            }
            Check.Exception(this.MapperAction != null || this.MapperActionWithCache != null, ErrorMessage.GetThrowMessage("'Mapper’ needs to be written after ‘MergeTable’ ", "Mapper 只能在 MergeTable 之后使用"));
            //Check.Exception(this.QueryBuilder.SelectValue.IsNullOrEmpty(),ErrorMessage.GetThrowMessage( "MergeTable need to use Queryable.Select Method .", "使用MergeTable之前必须要有Queryable.Select方法"));
            //Check.Exception(this.QueryBuilder.Skip > 0 || this.QueryBuilder.Take > 0 || this.QueryBuilder.OrderByValue.HasValue(),ErrorMessage.GetThrowMessage( "MergeTable  Queryable cannot Take Skip OrderBy PageToList  ", "使用 MergeTable不能有 Take Skip OrderBy PageToList 等操作,你可以在Mergetable之后操作"));
            var sqlobj = this._ToSql();
            if (IsSubToList())
            {
                return MergeTableWithSubToListJoin();
            }
            var index = QueryBuilder.WhereIndex + 1;
            var result = 
                this.EntityInfo.Discrimator.HasValue()?
                this.Context.Queryable<object>().AS(SqlBuilder.GetPackTable(sqlobj.Key, "MergeTable")).AddParameters(sqlobj.Value).Select<T>("*").With(SqlWith.Null) 
                :
                this.Context.Queryable<T>().AS(SqlBuilder.GetPackTable(sqlobj.Key, "MergeTable")).AddParameters(sqlobj.Value).Select("*").With(SqlWith.Null);
            result.QueryBuilder.WhereIndex = index;
            result.QueryBuilder.NoCheckInclude = true;
            result.QueryBuilder.Includes = this.QueryBuilder.Includes;
            result.QueryBuilder.AppendNavInfo = this.QueryBuilder.AppendNavInfo;
            result.QueryBuilder.LambdaExpressions.ParameterIndex = QueryBuilder.LambdaExpressions.ParameterIndex++;
            result.QueryBuilder.LambdaExpressions.Index = QueryBuilder.LambdaExpressions.Index++;
            result.QueryBuilder.IsCrossQueryWithAttr = QueryBuilder.IsCrossQueryWithAttr;
            if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
            {
                result.Select("MergeTable.*");
            }
            return result;
        }
        public ISugarQueryable<T> SplitTable() 
        {
            UtilMethods.StartCustomSplitTable(this.Context, typeof(T));
            //all table
            return this.SplitTable(tag => tag);
        }
        public ISugarQueryable<T> SplitTable(DateTime beginTime, DateTime endTime) 
        {
            UtilMethods.StartCustomSplitTable(this.Context, typeof(T));
            var splitColumn = this.EntityInfo.Columns.FirstOrDefault(it => it.PropertyInfo.GetCustomAttribute<SplitFieldAttribute>() != null);
            Check.ExceptionEasy(splitColumn==null,"[SplitFieldAttribute] need to be added to the table field", "需要在分表字段加上属性[SplitFieldAttribute]");
            var columnName = this.SqlBuilder.GetTranslationColumnName(splitColumn.DbColumnName);
            var sqlParameterKeyWord = this.SqlBuilder.SqlParameterKeyWord;
            var resultQueryable= this.Where($" {columnName}>={sqlParameterKeyWord}spBeginTime AND {columnName}<= {sqlParameterKeyWord}spEndTime",new { spBeginTime = beginTime , spEndTime = endTime}).SplitTable(tas => {
                var result = tas;
                var  type= this.EntityInfo.Type.GetCustomAttribute<SplitTableAttribute>();
                Check.ExceptionEasy(type == null, $"{this.EntityInfo.EntityName}need SplitTableAttribute", $"{this.EntityInfo.EntityName}需要特性 SplitTableAttribute");
                if (SplitType.Month == type.SplitType)
                {
                    result = result.Where(y => y.Date >= beginTime.ToString("yyyy-MM").ObjToDate() && y.Date <= endTime.Date.ToString("yyyy-MM").ObjToDate().AddMonths(1).AddDays(-1)).ToList();
                }
                else if (SplitType.Year == type.SplitType)
                {
                    result = result.Where(y => y.Date.Year >= beginTime.Year && y.Date.Year <= endTime.Year).ToList();
                }
                else if (SplitType.Week == type.SplitType)
                {
                    var begtinWeek = UtilMethods.GetWeekFirstDayMon(beginTime).Date;
                    var endWeek = UtilMethods.GetWeekLastDaySun(endTime).Date;
                    result = result.Where(y => 
                        y.Date >= begtinWeek&& y.Date  <= endWeek).ToList();
                }
                else if (SplitType.Month_6 == type.SplitType)
                {
                    var begtinWeek = beginTime.Month<=6?beginTime.ToString("yyyy-01-01"): beginTime.ToString("yyyy-06-01");
                    var endWeek = endTime.Month <= 6 ? endTime.ToString("yyyy-07-01") : endTime.ToString("yyyy-12-01");
                    result = result.Where(y =>
                        y.Date >= begtinWeek.ObjToDate() && y.Date <= endWeek.ObjToDate().AddMonths(1).AddDays(-1)).ToList();
                }
                else if (SplitType.Season == type.SplitType)
                {

                    var beginSeason= Convert.ToDateTime( beginTime.AddMonths(0 - ((beginTime.Month - 1) % 3)).ToString("yyyy-MM-01")).Date;
                    var endSeason= DateTime.Parse(endTime.AddMonths(3 - ((endTime.Month - 1) % 3)).ToString("yyyy-MM-01")).AddDays(-1).Date;
                    result = result.Where(y =>
                        y.Date >= beginSeason && y.Date <= endSeason).ToList();
                }
                else 
                {
                    result = result.Where(y => y.Date >= beginTime.Date && y.Date <= endTime.Date).ToList();
                }
                return result;
                });
            if (splitColumn.SqlParameterDbType is System.Data.DbType)
            {
                foreach (var item in resultQueryable.QueryBuilder.Parameters)
                {
                    if (item.ParameterName.IsContainsIn("spBeginTime", "spEndTime")) 
                    {
                        item.DbType =(System.Data.DbType)splitColumn.SqlParameterDbType;
                    }
                }
            }
            return resultQueryable;
        }
        public ISugarQueryable<T> SplitTable(Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> getTableNamesFunc) 
        {
            UtilMethods.StartCustomSplitTable(this.Context, typeof(T));
            SplitTableContext helper = new SplitTableContext(Context)
            {
                EntityInfo = this.EntityInfo
            };
            this.Context.MappingTables.Add(this.EntityInfo.EntityName, this.EntityInfo.DbTableName);
            var tables = getTableNamesFunc(helper.GetTables());
            List<ISugarQueryable<T>> tableQueryables = new List<ISugarQueryable<T>>();
            foreach (var item in tables)
            {
                tableQueryables.Add(this.Clone().AS(item.TableName));
            }
            if (tableQueryables.Count == 0)
            {
                var result= this.Context.SqlQueryable<object>("-- No table ").Select<T>();
                result.QueryBuilder.SelectValue = null;
                return result;
            }
            else
            {
                var unionall = this.Context._UnionAll(tableQueryables.ToArray());
                unionall.QueryBuilder.Includes = this.QueryBuilder.Includes;
                if (unionall.QueryBuilder.Includes?.Any()==true) 
                {
                    unionall.QueryBuilder.NoCheckInclude = true;
                }
                return unionall;
            }
            //var values= unionall.QueryBuilder.GetSelectValue;
            //unionall.QueryBuilder.SelectValue = values;
        }
        
        public ISugarQueryable<T> WithCache(string cacheKey, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            Check.ArgumentNullException(this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService, "Use Cache ConnectionConfig.ConfigureExternalServices.DataInfoCacheService is required ");
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            this.CacheKey = cacheKey;
            return this;
        }
        public ISugarQueryable<T> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            Check.ArgumentNullException(this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService, "Use Cache ConnectionConfig.ConfigureExternalServices.DataInfoCacheService is required ");
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public ISugarQueryable<T> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (isCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public ISugarQueryable<T> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
    }
}
