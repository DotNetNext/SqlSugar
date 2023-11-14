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
    #region T11
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Hints(string hints)
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10,T11> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null)
        {
            base.OrderByPropertyName(orderPropertyName, orderByType);
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }
       public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoinIF<T12>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression) 
        {
            var result = LeftJoin(joinExpression);
            if (isLeftJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoinIF<T12>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            var result = InnerJoin(joinExpression);
            if (isJoin == false)
            {
                result.QueryBuilder.JoinQueryInfos.Remove(result.QueryBuilder.JoinQueryInfos.Last());
            }
            return result;
        }
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

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression, string tableName) 
        {
            var result = LeftJoin<T12>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FullJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression, string tableName)
        {
            var result = FullJoin<T12>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression, string tableName)
        {
            var result = InnerJoin<T12>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression, string tableName)
        {
            var result = RightJoin<T12>(joinExpression);
            result.QueryBuilder.JoinQueryInfos.Last().TableName = tableName;
            return result;
        }

        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(string expShortName, FormattableString expressionString)
        {
            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            base.Where(conditionalModels, isWrap);
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, TResult>> expression, bool isAutoFill)
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
            sql = AppendSelect<T11>(sql, parameters, columnsResult, 10);
            if (sql.Trim().First() == ',')
            {
                sql = sql.TrimStart(' ').TrimStart(',');
            }
            return this.Select<TResult>(sql);
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8, T9, T10,T11> OrderBy(List<OrderByModel> models)
        {
            base.OrderBy(models);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> ClearFilter()
        {
            this.Filter(null, true);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }


        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ClearFilter(params Type[] types)
        {
            base.ClearFilter(types);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ClearFilter<FilterType1>()
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ClearFilter<FilterType1, FilterType2>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ClearFilter<FilterType1, FilterType2, FilterType3>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2), typeof(FilterType3));
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Hints(string hints)
        {
            this.QueryBuilder.Hints = hints;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null)
        {
            base.OrderByPropertyName(orderPropertyName, orderByType);
            return this;
        }
        public virtual ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, TResult>> expression)
        {
            return this.Select(expression).MergeTable();
        }

        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11,T12> Where(string expShortName, FormattableString expressionString)
        {
            var exp = DynamicCoreHelper.GetWhere<T>(expShortName, expressionString);
            _Where(exp);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            base.Where(conditionalModels, isWrap);
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, TResult>> expression, bool isAutoFill)
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
            sql = AppendSelect <T11>(sql, parameters, columnsResult, 10);
            sql = AppendSelect<T12>(sql, parameters, columnsResult, 11);
            if (sql.Trim().First() == ',')
            {
                sql = sql.TrimStart(' ').TrimStart(',');
            }
            return this.Select<TResult>(sql);
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8, T9, T10, T11,T12> OrderBy(List<OrderByModel> models)
        {
            base.OrderBy(models);
            return this;
        }
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
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> ClearFilter()
        {
            this.Filter(null, true);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ClearFilter(params Type[] types)
        {
            base.ClearFilter(types);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ClearFilter<FilterType1>()
        {
            this.ClearFilter(typeof(FilterType1));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ClearFilter<FilterType1, FilterType2>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ClearFilter<FilterType1, FilterType2, FilterType3>()
        {
            this.ClearFilter(typeof(FilterType1), typeof(FilterType2), typeof(FilterType3));
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
