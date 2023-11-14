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
using System.Collections.ObjectModel;

namespace SqlSugar 
{
    #region T2
    public partial class QueryableProvider<T, T2> : QueryableProvider<T>, ISugarQueryable<T, T2>
    {
        public new ISugarQueryable<T,T2> Hints(string hints)
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public new ISugarQueryable<T, T2> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null)
        {
            base.OrderByPropertyName(orderPropertyName, orderByType);
            return this;
        }
        public new ISugarQueryable<T, T2> SampleBy(int timeNumber, SampleByUnit timeType) 
        {
             base.SampleBy(timeNumber, timeType);
             return this;
        }
        public new ISugarQueryable<T, T2> SampleBy(int timeNumber, string timeType) 
        {
            base.SampleBy(timeNumber, timeType);
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T,T2, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }
        public ISugarQueryable<T, T2, T3> LeftJoinIF<T3>(bool isLeftJoin, Expression<Func<T, T2, T3, bool>> joinExpression) 
        {
            var result = LeftJoin(joinExpression);
            if (isLeftJoin==false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3> InnerJoinIF<T3>(bool isJoin, Expression<Func<T, T2, T3, bool>> joinExpression)
        {
            var result = InnerJoin(joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3> LeftJoin<T3>(ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2, T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3> LeftJoinIF<T3>(bool isJoin, ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2, T3, bool>> joinExpression)
        {
            var result = LeftJoin(joinQueryable, joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3> InnerJoin<T3>(ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2, T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3> RightJoin<T3>(ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2, T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
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

        public ISugarQueryable<T, T2, T3> FullJoin<T3>(ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2, T3, bool>> joinExpression) 
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3> LeftJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3> FullJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3> RightJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }

        public ISugarQueryable<T, T2, T3> LeftJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression, string tableName) 
        {
            var result= LeftJoin<T3>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3> FullJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression, string tableName) 
        {
            var result = FullJoin<T3>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3> InnerJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression, string tableName) 
        {
            var result = InnerJoin<T3>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3> RightJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression, string tableName) 
        {
            var result = RightJoin<T3>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3> InnerJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2> Where(List<IConditionalModel> conditionalModels, bool isWrap) 
        {
            base.Where(conditionalModels, isWrap);
            return this;
        }
        public new ISugarQueryable<T, T2> Where(string expShortName, FormattableString expressionString) 
        {
            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
        public new ISugarQueryable<T, T2> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }
        public new ISugarQueryable<T, T2> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }
        public new ISugarQueryable<T, T2> Where(IFuncModel funcModel)
        {
            var obj = this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public new ISugarQueryable<T, T2> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T,T2, TResult>> expression, bool isAutoFill)
        {
            var clone = this.Select(expression).Clone();
            var sql = clone.QueryBuilder.GetSelectValue;
            if (this.QueryBuilder.IsSingle() || string.IsNullOrEmpty(sql) || sql.Trim() == "*")
            {
                sql = " ";
            }
            this.QueryBuilder.SubToListParameters = clone.QueryBuilder.SubToListParameters;
            this.QueryBuilder.Parameters = clone.QueryBuilder.Parameters;
            this.QueryBuilder.LambdaExpressions.ParameterIndex = clone.QueryBuilder.LambdaExpressions.ParameterIndex;
            var parameters = (expression as LambdaExpression).Parameters;
            var columnsResult = this.Context.EntityMaintenance.GetEntityInfo<TResult>().Columns;
            sql = AppendSelect<T>(sql, parameters, columnsResult, 0);
            sql = AppendSelect<T2>(sql, parameters, columnsResult, 1);
            if (sql.Trim().First()==',')
            {
                sql = sql.TrimStart(' ').TrimStart(',');
            }
            return this.Select<TResult>(sql);
        }
        #endregion

        #region Order
        public new ISugarQueryable<T,T2> OrderBy(List<OrderByModel> models) 
        {
            base.OrderBy(models);
            return this;
        }
        public new ISugarQueryable<T, T2> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public ISugarQueryable<T, T2> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new virtual ISugarQueryable<T, T2> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2> OrderByDescending(Expression<Func<T, T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public new ISugarQueryable<T, T2> OrderBy(Expression<Func<T, object>> expression, OrderByType type)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2> PartitionBy(Expression<Func<T, object>> expression) 
        {
            if (QueryBuilder.Take == null)
                QueryBuilder.Take = 1;
            _PartitionBy(expression);
            QueryBuilder.DisableTop = true;
            return this;
        }
        public ISugarQueryable<T, T2> PartitionBy(Expression<Func<T, T2, object>> expression) 
        {
            if (QueryBuilder.Take == null)
                QueryBuilder.Take = 1;
            _PartitionBy(expression);
            QueryBuilder.DisableTop = true;
            return this;
        }
        public new ISugarQueryable<T, T2> PartitionBy(string groupFileds) 
        {
            base.PartitionBy(groupFileds);
            return this;
        }
        public new virtual ISugarQueryable<T, T2> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public new virtual ISugarQueryable<T, T2> GroupByIF(bool isGroupBy, string groupFields)
        {
            if (isGroupBy)
            {
                GroupBy(groupFields);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public new virtual ISugarQueryable<T, T2> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public new ISugarQueryable<T, T2> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public ISugarQueryable<T, T2> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public new ISugarQueryable<T, T2> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _MaxAsync<TResult>(expression);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _MinAsync<TResult>(expression);
        }
        public Task<TResult> SumAsync<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _SumAsync<TResult>(expression);
        }
        public Task<TResult> AvgAsync<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _AvgAsync<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T,T2> InIF<TParamter>(bool isIn, string fieldName, params TParamter[] pkValues)
        {
            if (isIn)
            {
               In(fieldName, pkValues);
            }
            return this;
        }
        public new ISugarQueryable<T, T2> InIF<TParamter>(bool isIn, params TParamter[] pkValues)
        {
            if (isIn)
            {
                In(pkValues);
            }
            return this;
        }
        public new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, T2, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, T2, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, T2, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2> Clone()
        {
            var queryable = this.Context.Queryable<T, T2>((t, t2) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T,T2> ClearFilter()
        {
            this.Filter(null, true);
            return this;
        }
        public new ISugarQueryable<T,T2> ClearFilter(params Type[] types)
        {
            base.ClearFilter(types);
            return this;
        }
        public new ISugarQueryable<T, T2> ClearFilter<FilterType1>()
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public new ISugarQueryable<T, T2> ClearFilter<FilterType1, FilterType2>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2));
            return this;
        }
        public new ISugarQueryable<T, T2> ClearFilter<FilterType1, FilterType2, FilterType3>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2), typeof(FilterType3));
            return this;
        }
        public new ISugarQueryable<T, T2> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public new ISugarQueryable<T, T2> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (isCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }

        public new ISugarQueryable<T, T2> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        #endregion
    }
    #endregion
    #region T3
    public partial class QueryableProvider<T, T2, T3> : QueryableProvider<T>, ISugarQueryable<T, T2, T3>
    {
        public new ISugarQueryable<T, T2,T3> Hints(string hints)
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public new ISugarQueryable<T, T2, T3> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null)
        {
            base.OrderByPropertyName(orderPropertyName, orderByType);
            return this;
        }
        public new ISugarQueryable<T, T2,T3> SampleBy(int timeNumber, SampleByUnit timeType)
        {
            base.SampleBy(timeNumber, timeType);
            return this;
        }
        public new ISugarQueryable<T, T2,T3> SampleBy(int timeNumber, string timeType)
        {
            base.SampleBy(timeNumber, timeType);
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2,T3, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }
        public ISugarQueryable<T, T2, T3, T4> LeftJoinIF<T4>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, bool>> joinExpression) 
        {
            var result = LeftJoin(joinExpression);
            if (isLeftJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4> InnerJoinIF<T4>(bool isJoin, Expression<Func<T, T2, T3, T4, bool>> joinExpression)
        {
            var result = InnerJoin(joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4> LeftJoin<T4>(ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3, T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4> LeftJoinIF<T4>(bool isJoin, ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3, T4, bool>> joinExpression) 
        {
            var result = LeftJoin(joinQueryable, joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4> InnerJoin<T4>(ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3, T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4> RightJoin<T4>(ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3, T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4> FullJoin<T4>(ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3, T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4> LeftJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4> FullJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4> RightJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }


        public ISugarQueryable<T, T2, T3, T4> InnerJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4> LeftJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression, string tableName) 
        {
            var result= LeftJoin<T4>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4> FullJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression, string tableName) 
        {
            var result = FullJoin<T4>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4> InnerJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression, string tableName) 
        {
            var result = InnerJoin<T4>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4> RightJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression, string tableName)
        {
            var result = RightJoin<T4>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;

        }


        #region  Group 
        public new ISugarQueryable<T, T2,T3> PartitionBy(Expression<Func<T, object>> expression)
        {
            if (QueryBuilder.Take == null)
                QueryBuilder.Take = 1;
            _PartitionBy(expression);
            QueryBuilder.DisableTop = true;
            return this;
        }
        public ISugarQueryable<T, T2,T3> PartitionBy(Expression<Func<T, T2, object>> expression)
        {
            if (QueryBuilder.Take == null)
                QueryBuilder.Take = 1;
            _PartitionBy(expression);
            QueryBuilder.DisableTop = true;
            return this;
        }
        public ISugarQueryable<T, T2, T3> PartitionBy(Expression<Func<T, T2,T3, object>> expression)
        {
            if (QueryBuilder.Take == null)
                QueryBuilder.Take = 1;
            _PartitionBy(expression);
            QueryBuilder.DisableTop = true;
            return this;
        }
        public new ISugarQueryable<T, T2,T3> PartitionBy(string groupFileds)
        {
            base.PartitionBy(groupFileds);
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public new virtual ISugarQueryable<T, T2,T3> GroupByIF(bool isGroupBy, string groupFields)
        {
            if (isGroupBy)
            {
                GroupBy(groupFields);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        #endregion

        #region Order
        public new ISugarQueryable<T, T2, T3> OrderBy(List<OrderByModel> models)
        {
            base.OrderBy(models);
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3> OrderByDescending(Expression<Func<T, T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3> OrderByDescending(Expression<Func<T, T2, T3, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, object>> expression, OrderByType type)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2,T3, TResult>> expression, bool isAutoFill)
        {
            var clone = this.Select(expression).Clone();
            var sql = clone.QueryBuilder.GetSelectValue;
            if (this.QueryBuilder.IsSingle() || string.IsNullOrEmpty(sql) || sql.Trim() == "*")
            {
                sql = " ";
            }
            this.QueryBuilder.Parameters = clone.QueryBuilder.Parameters;
            this.QueryBuilder.SubToListParameters = clone.QueryBuilder.SubToListParameters;
            this.QueryBuilder.LambdaExpressions.ParameterIndex = clone.QueryBuilder.LambdaExpressions.ParameterIndex;
            var parameters = (expression as LambdaExpression).Parameters;
            var columnsResult = this.Context.EntityMaintenance.GetEntityInfo<TResult>().Columns;
            sql = AppendSelect<T>(sql, parameters, columnsResult, 0);
            sql = AppendSelect<T2>(sql, parameters, columnsResult, 1);
            sql = AppendSelect<T3>(sql, parameters, columnsResult, 2);
            if (sql.Trim().First() == ',')
            {
                sql = sql.TrimStart(' ').TrimStart(',');
            }
            return this.Select<TResult>(sql);
        }

        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        #endregion

        #region Where
        public new ISugarQueryable<T, T2,T3> Where(string expShortName, FormattableString expressionString)
        {
            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
        public ISugarQueryable<T, T2, T3> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }
        public new ISugarQueryable<T, T2,T3> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            base.Where(conditionalModels, isWrap);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> Where(IFuncModel funcModel)
        {
            var obj = this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _MaxAsync<TResult>(expression);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _MinAsync<TResult>(expression);
        }
        public Task<TResult> SumAsync<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _SumAsync<TResult>(expression);
        }
        public Task<TResult> AvgAsync<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _AvgAsync<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2,T3> InIF<TParamter>(bool isIn, string fieldName, params TParamter[] pkValues)
        {
            if (isIn)
            {
                In(fieldName, pkValues);
            }
            return this;
        }
        public new ISugarQueryable<T, T2, T3> InIF<TParamter>(bool isIn, params TParamter[] pkValues)
        {
            if (isIn)
            {
                In(pkValues);
            }
            return this;
        }
        public new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3>((t, t2, t3) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> ClearFilter()
        {
            this.Filter(null, true);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2,T3> ClearFilter(params Type[] types)
        {
            base.ClearFilter(types);
            return this;
        }
        public new ISugarQueryable<T, T2,T3> ClearFilter<FilterType1>()
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public new ISugarQueryable<T, T2,T3> ClearFilter<FilterType1, FilterType2>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2));
            return this;
        }
        public new ISugarQueryable<T, T2,T3> ClearFilter<FilterType1, FilterType2, FilterType3>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2), typeof(FilterType3));
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public new ISugarQueryable<T, T2, T3> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
    #region T4
    public partial class QueryableProvider<T, T2, T3, T4> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4>
    {
        public new ISugarQueryable<T, T2, T3,T4> Hints(string hints)
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null)
        {
            base.OrderByPropertyName(orderPropertyName, orderByType);
            return this;
        }
        public new ISugarQueryable<T, T2,T3,T4> SampleBy(int timeNumber, SampleByUnit timeType)
        {
            base.SampleBy(timeNumber, timeType);
            return this;
        }
        public new ISugarQueryable<T, T2,T3, T4> SampleBy(int timeNumber, string timeType)
        {
            base.SampleBy(timeNumber, timeType);
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3,T4, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }
        public ISugarQueryable<T, T2, T3, T4, T5> LeftJoinIF<T5>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression) 
        {
            var result = LeftJoin(joinExpression);
            if (isLeftJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> InnerJoinIF<T5>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            var result = InnerJoin(joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> LeftJoin<T5>(ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5> LeftJoinIF<T5>(bool isJoin, ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression) 
        {
            var result = LeftJoin(joinQueryable, joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> InnerJoin<T5>(ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5> RightJoin<T5>(ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5> LeftJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> FullJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> InnerJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> LeftJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression, string tableName) 
        {
            var result = LeftJoin<T5>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> FullJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression, string tableName) 
        {
            var result = FullJoin<T5>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> InnerJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression, string tableName) 
        {
            var result = InnerJoin<T5>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression, string tableName) 
        {
            var result = RightJoin<T5>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }


        #region Where
        public new ISugarQueryable<T, T2, T3,T4> Where(string expShortName, FormattableString expressionString)
        {
            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }
        public new ISugarQueryable<T, T2,T3,T4> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            base.Where(conditionalModels, isWrap);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Where(IFuncModel funcModel)
        {
            var obj = this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public new ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3,T4, TResult>> expression, bool isAutoFill)
        {
            var clone = this.Select(expression).Clone();
            var sql = clone.QueryBuilder.GetSelectValue;
            if (this.QueryBuilder.IsSingle() || string.IsNullOrEmpty(sql) || sql.Trim() == "*")
            {
                sql = " ";
            }
            this.QueryBuilder.Parameters = clone.QueryBuilder.Parameters;
            this.QueryBuilder.SubToListParameters = clone.QueryBuilder.SubToListParameters;
            this.QueryBuilder.LambdaExpressions.ParameterIndex=clone.QueryBuilder.LambdaExpressions.ParameterIndex; 
            var parameters = (expression as LambdaExpression).Parameters;
            var columnsResult = this.Context.EntityMaintenance.GetEntityInfo<TResult>().Columns;
            sql = AppendSelect<T>(sql, parameters, columnsResult, 0);
            sql = AppendSelect<T2>(sql, parameters, columnsResult, 1);
            sql = AppendSelect<T3>(sql, parameters, columnsResult, 2);
            sql = AppendSelect<T4>(sql, parameters, columnsResult, 3);
            if (sql.Trim().First() == ',')
            {
                sql = sql.TrimStart(' ').TrimStart(',');
            }
            return this.Select<TResult>(sql);
        }

        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2,T3,T4> OrderBy(List<OrderByModel> models)
        {
            base.OrderBy(models);
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3, T4> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> OrderByDescending(Expression<Func<T, T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> OrderByDescending(Expression<Func<T, T2, T3, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new virtual ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public new virtual ISugarQueryable<T, T2,T3,T4> GroupByIF(bool isGroupBy, string groupFields)
        {
            if (isGroupBy)
            {
                GroupBy(groupFields);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }

        public new virtual ISugarQueryable<T, T2, T3, T4> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3,T4> InIF<TParamter>(bool isIn, string fieldName, params TParamter[] pkValues)
        {
            if (isIn)
            {
                In(fieldName, pkValues);
            }
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> InIF<TParamter>(bool isIn, params TParamter[] pkValues)
        {
            if (isIn)
            {
                In(pkValues);
            }
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, T4, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, T4, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, T4, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion


        #region Other
        public new ISugarQueryable<T, T2, T3, T4> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4>((t, t2, t3, t4) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3,T4> ClearFilter()
        {
            this.Filter(null, true);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> ClearFilter(params Type[] types)
        {
            base.ClearFilter(types);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> ClearFilter<FilterType1>()
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> ClearFilter<FilterType1, FilterType2>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> ClearFilter<FilterType1, FilterType2, FilterType3>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2), typeof(FilterType3));
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public new ISugarQueryable<T, T2, T3, T4> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3, T4> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
    #region T5   
    public partial class QueryableProvider<T, T2, T3, T4, T5> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5>
    {
        public new ISugarQueryable<T, T2, T3, T4,T5> Hints(string hints)
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null)
        {
            base.OrderByPropertyName(orderPropertyName, orderByType);
            return this;
        }
        public new ISugarQueryable<T, T2,T3, T4,T5> SampleBy(int timeNumber, SampleByUnit timeType)
        {
            base.SampleBy(timeNumber, timeType);
            return this;
        }
        public new ISugarQueryable<T, T2,T3, T4,T5> SampleBy(int timeNumber, string timeType)
        {
            base.SampleBy(timeNumber, timeType);
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }
        
        public ISugarQueryable<T, T2, T3, T4, T5, T6> LeftJoinIF<T6>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression) 
        {
            var result = LeftJoin(joinExpression);
            if (isLeftJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> InnerJoinIF<T6>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            var result = InnerJoin(joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> LeftJoin<T6>(ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
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

        public ISugarQueryable<T, T2, T3, T4, T5, T6> LeftJoinIF<T6>(bool isJoin, ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression) 
        {
            var result = LeftJoin(joinQueryable, joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> InnerJoin<T6>(ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6> RightJoin<T6>(ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6> LeftJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> FullJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> RightJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> InnerJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> LeftJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression, string tableName)
        {
            var result = LeftJoin<T6>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> FullJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression, string tableName)
        {
            var result = FullJoin<T6>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> InnerJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression, string tableName)
        {
            var result = InnerJoin<T6>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> RightJoin<T6>(Expression<Func<T, T2, T3, T4, T5,T6, bool>> joinExpression, string tableName)
        {
            var result = RightJoin<T6>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }


        #region Where
        public new ISugarQueryable<T, T2, T3, T4,T5> Where(string expShortName, FormattableString expressionString)
        {
            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            base.Where(conditionalModels, isWrap);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Where(IFuncModel funcModel)
        {
            var obj = this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }

        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }

        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4,T5, TResult>> expression, bool isAutoFill)
        {
            var clone = this.Select(expression).Clone();
            var sql = clone.QueryBuilder.GetSelectValue;
            if (this.QueryBuilder.IsSingle() || string.IsNullOrEmpty(sql) || sql.Trim() == "*")
            {
                sql = " ";
            }
            this.QueryBuilder.Parameters = clone.QueryBuilder.Parameters;
            this.QueryBuilder.SubToListParameters = clone.QueryBuilder.SubToListParameters;
            this.QueryBuilder.LambdaExpressions.ParameterIndex = clone.QueryBuilder.LambdaExpressions.ParameterIndex;
            var parameters = (expression as LambdaExpression).Parameters;
            var columnsResult = this.Context.EntityMaintenance.GetEntityInfo<TResult>().Columns;
            sql = AppendSelect<T>(sql, parameters, columnsResult, 0);
            sql = AppendSelect<T2>(sql, parameters, columnsResult, 1);
            sql = AppendSelect<T3>(sql, parameters, columnsResult, 2);
            sql = AppendSelect<T4>(sql, parameters, columnsResult, 3);
            sql = AppendSelect<T5>(sql, parameters, columnsResult, 4);
            if (sql.Trim().First() == ',')
            {
                sql = sql.TrimStart(' ').TrimStart(',');
            }
            return this.Select<TResult>(sql);
        }

        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3,T4,T5> OrderBy(List<OrderByModel> models)
        {
            base.OrderBy(models);
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, T2, T3, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new virtual ISugarQueryable<T, T2, T3, T4,T5> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3, T4, T5> GroupByIF(bool isGroupBy, string groupFields)
        {
            if (isGroupBy)
            {
                GroupBy(groupFields);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4,T5, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Having(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Having(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }

        public new virtual ISugarQueryable<T, T2, T3, T4, T5> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5>((t, t2, t3, t4, t5) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4,T5> ClearFilter()
        {
            this.Filter(null, true);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }


        public new ISugarQueryable<T, T2, T3, T4, T5> ClearFilter(params Type[] types)
        {
            base.ClearFilter(types);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> ClearFilter<FilterType1>()
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> ClearFilter<FilterType1, FilterType2>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> ClearFilter<FilterType1, FilterType2, FilterType3>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2), typeof(FilterType3));
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public new ISugarQueryable<T, T2, T3, T4, T5> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
}
