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
    #region T6
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6>
    {
        public new ISugarQueryable<T, T2, T3, T4, T5,T6> Hints(string hints)
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5,T6> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null)
        {
            base.OrderByPropertyName(orderPropertyName, orderByType);
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5,T6, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> LeftJoinIF<T7>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression) 
        {
            var result = LeftJoin(joinExpression);
            if (isLeftJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> InnerJoinIF<T7>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            var result = InnerJoin(joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
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

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression, string tableName) 
        {
            var result = LeftJoin<T7>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> FullJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression, string tableName) 
        {
            var result = FullJoin<T7>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression, string tableName) 
        {
            var result = InnerJoin<T7>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression, string tableName) 
        {
            var result = RightJoin<T7>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }

        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5,T6> Where(string expShortName, FormattableString expressionString)
        {
            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4,T5,T6> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            base.Where(conditionalModels, isWrap);
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5,T6, TResult>> expression, bool isAutoFill)
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
            sql = AppendSelect<T6>(sql, parameters, columnsResult, 5);
            if (sql.Trim().First() == ',')
            {
                sql = sql.TrimStart(' ').TrimStart(',');
            }
            return this.Select<TResult>(sql);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5,T6> OrderBy(List<OrderByModel> models)
        {
            base.OrderBy(models);
            return this;
        }
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
        public new virtual ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, string groupFields)
        {
            if (isGroupBy)
            {
                GroupBy(groupFields);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5,T6> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, T5,T6, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5,T6> ClearFilter()
        {
            this.Filter(null, true);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }


        public new ISugarQueryable<T, T2, T3, T4, T5, T6> ClearFilter(params Type[] types)
        {
            base.ClearFilter(types);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> ClearFilter<FilterType1>()
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> ClearFilter<FilterType1, FilterType2>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> ClearFilter<FilterType1, FilterType2, FilterType3>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2), typeof(FilterType3));
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> Hints(string hints)
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null)
        {
            base.OrderByPropertyName(orderPropertyName, orderByType);
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }
         public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> LeftJoinIF<T8>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression) 
         {
            var result = LeftJoin(joinExpression);
            if (isLeftJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> InnerJoinIF<T8>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            var result = InnerJoin(joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
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

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression, string tableName) 
        {
            var result = LeftJoin<T8>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> FullJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression, string tableName) 
        {
            var result = FullJoin<T8>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression, string tableName)
        {
            var result = InnerJoin<T8>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression, string tableName)
        {
            var result = RightJoin<T8>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }

        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> Where(string expShortName, FormattableString expressionString)
        {
            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            base.Where(conditionalModels, isWrap);
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7, TResult>> expression, bool isAutoFill)
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
            sql = AppendSelect<T6>(sql, parameters, columnsResult, 5);
            sql = AppendSelect<T7>(sql, parameters, columnsResult, 6);
            if (sql.Trim().First() == ',')
            {
                sql = sql.TrimStart(' ').TrimStart(',');
            }
            return this.Select<TResult>(sql);
        }

        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5,T6,T7> OrderBy(List<OrderByModel> models)
        {
            base.OrderBy(models);
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, T2, T3, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6,T7, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }

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
        public new virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, string groupFields)
        {
            if (isGroupBy)
            {
                GroupBy(groupFields);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6,T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, T5, T6,T7, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> ClearFilter()
        {
            this.Filter(null, true);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> ClearFilter(params Type[] types)
        {
            base.ClearFilter(types);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> ClearFilter<FilterType1>()
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> ClearFilter<FilterType1, FilterType2>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> ClearFilter<FilterType1, FilterType2, FilterType3>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2), typeof(FilterType3));
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> Hints(string hints)
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null)
        {
            base.OrderByPropertyName(orderPropertyName, orderByType);
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoinIF<T9>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression) 
        {
            var result = LeftJoin(joinExpression);
            if (isLeftJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoinIF<T9>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            var result = InnerJoin(joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
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

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression, string tableName) 
        {
            var result = LeftJoin<T9>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> FullJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression, string tableName) 
        {
            var result = FullJoin<T9>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression, string tableName) 
        {
            var result = InnerJoin<T9>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression, string tableName) 
        {
            var result = RightJoin<T9>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }

        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> Where(string expShortName, FormattableString expressionString)
        {
            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            base.Where(conditionalModels, isWrap);
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, TResult>> expression, bool isAutoFill)
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
            sql = AppendSelect<T6>(sql, parameters, columnsResult, 5);
            sql = AppendSelect<T7>(sql, parameters, columnsResult, 6);
            sql = AppendSelect<T8>(sql, parameters, columnsResult, 7);
            if (sql.Trim().First() == ',')
            {
                sql = sql.TrimStart(' ').TrimStart(',');
            }
            return this.Select<TResult>(sql);
        }

        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderBy(List<OrderByModel> models)
        {
            base.OrderBy(models);
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, T3, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6,T7,T8> ClearFilter()
        {
            this.Filter(null, true);
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

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> ClearFilter(params Type[] types)
        {
            base.ClearFilter(types);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> ClearFilter<FilterType1>()
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> ClearFilter<FilterType1, FilterType2>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> ClearFilter<FilterType1, FilterType2, FilterType3>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2), typeof(FilterType3));
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Hints(string hints)
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null)
        {
            base.OrderByPropertyName(orderPropertyName, orderByType);
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoinIF<T10>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            var result = LeftJoin(joinExpression);
            if (isLeftJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoinIF<T10>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            var result = InnerJoin(joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
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

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression, string tableName) 
        {
            var result = LeftJoin<T10>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> FullJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression, string tableName)
        {
            var result = FullJoin<T10>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression, string tableName)
        {
            var result = InnerJoin<T10>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression, string tableName)
        {
            var result = RightJoin<T10>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }

        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(string expShortName, FormattableString expressionString)
        {
            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            base.Where(conditionalModels, isWrap);
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, TResult>> expression, bool isAutoFill)
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
            sql = AppendSelect<T6>(sql, parameters, columnsResult, 5);
            sql = AppendSelect<T7>(sql, parameters, columnsResult, 6);
            sql = AppendSelect<T8>(sql, parameters, columnsResult, 7);
            sql = AppendSelect<T9>(sql, parameters, columnsResult, 8);
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8,T9> OrderBy(List<OrderByModel> models)
        {
            base.OrderBy(models);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> ClearFilter()
        {
            this.Filter(null, true);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> ClearFilter(params Type[] types)
        {
            base.ClearFilter(types);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> ClearFilter<FilterType1>()
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> ClearFilter<FilterType1, FilterType2>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> ClearFilter<FilterType1, FilterType2, FilterType3>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2), typeof(FilterType3));
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Hints(string hints)
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null)
        {
            base.OrderByPropertyName(orderPropertyName, orderByType);
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoinIF<T11>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression) 
        {
            var result = LeftJoin(joinExpression);
            if (isLeftJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoinIF<T11>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            var result = InnerJoin(joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
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

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression, string tableName) 
        {
            var result = LeftJoin<T11>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FullJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression, string tableName)
        {
            var result = FullJoin<T11>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression, string tableName)
        {
            var result = InnerJoin<T11>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression, string tableName)
        {
            var result = RightJoin<T11>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }

        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(string expShortName, FormattableString expressionString)
        {
            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            base.Where(conditionalModels, isWrap);
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, TResult>> expression, bool isAutoFill)
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
            sql = AppendSelect<T6>(sql, parameters, columnsResult, 5);
            sql = AppendSelect<T7>(sql, parameters, columnsResult, 6);
            sql = AppendSelect<T8>(sql, parameters, columnsResult, 7);
            sql = AppendSelect<T9>(sql, parameters, columnsResult, 8);
            sql = AppendSelect<T10>(sql, parameters, columnsResult, 9);
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            if (IsAppendNavColumns())
            {
                SetAppendNavColumns(expression);
            }
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8,T9,T10> OrderBy(List<OrderByModel> models)
        {
            base.OrderBy(models);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> ClearFilter()
        {
            this.Filter(null, true);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ClearFilter(params Type[] types)
        {
            base.ClearFilter(types);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ClearFilter<FilterType1>()
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ClearFilter<FilterType1, FilterType2>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ClearFilter<FilterType1, FilterType2, FilterType3>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2), typeof(FilterType3));
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
}
