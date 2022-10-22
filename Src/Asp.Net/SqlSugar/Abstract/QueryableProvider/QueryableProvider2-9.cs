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
    #region T2
    public partial class QueryableProvider<T, T2> : QueryableProvider<T>, ISugarQueryable<T, T2>
    {
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
            return _Select<TResult>(expression);
        }

        #endregion

        #region Order
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
        public new virtual ISugarQueryable<T, T2> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
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

        #region  Group 
        public new virtual ISugarQueryable<T, T2, T3> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        #endregion

        #region Where
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
        public new ISugarQueryable<T, T2, T3> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
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

        #region Where
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
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
        public new ISugarQueryable<T, T2, T3, T4> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
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

        #region Where
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
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
        public new ISugarQueryable<T, T2, T3, T4, T5> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
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
    #region T6
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(ISugarQueryable<T7> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(ISugarQueryable<T7> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(ISugarQueryable<T7> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> FullJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(IFuncModel funcModel)
        {
            var obj = this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, string whereString, object whereObj)
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new virtual ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, T3, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            this._Having(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }

        public new virtual ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6>((t, t2, t3, t4, t5, T6) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
    #region T7
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(ISugarQueryable<T8> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(ISugarQueryable<T8> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(ISugarQueryable<T8> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> FullJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, string whereString, object whereObj)
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7>((t, t2, t3, t4, t5, T6, t7) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
    #region T8
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(ISugarQueryable<T9> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(ISugarQueryable<T9> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(ISugarQueryable<T9> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> FullJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, string whereString, object whereObj)
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7, T8>((t, t2, t3, t4, t5, T6, t7, t8) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
    #region T9
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(ISugarQueryable<T10> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(ISugarQueryable<T10> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(ISugarQueryable<T10> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> FullJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }


        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>((t, t2, t3, t4, t5, T6, t7, t8, t9) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T10
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(ISugarQueryable<T11> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(ISugarQueryable<T11> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(ISugarQueryable<T11> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FullJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>((t, t2, t3, t4, t5, T6, t7, t8, t9, t10) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T11
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(ISugarQueryable<T12> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(ISugarQueryable<T12> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(ISugarQueryable<T12> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FullJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>((t, t2, t3, t4, t5, T6, t7, t8, t9, t10, t11) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T12
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>((t, t2, t3, t4, t5, T6, t7, t8, t9, t10, t11, t12) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
}
