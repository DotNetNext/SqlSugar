using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    ///<summary>
    /// ** description：Create datathis.access object
    /// ** author：sunkaixuan
    /// ** date：2017/1/2
    /// ** email:610262374@qq.com
    /// </summary>
    public partial class SqlSugarClient : IDisposable
    {

        #region Constructor
        public SqlSugarClient(ConnectionConfig config)
        {
            this.Context = this;
            this.CurrentConnectionConfig = config;
            this.ContextID = Guid.NewGuid();
            Check.ArgumentNullException(config, "config is null");
            switch (config.DbType)
            {
                case DbType.MySql:
                    DependencyManagement.TryMySqlData();
                    break;
                case DbType.SqlServer:
                    break;
                case DbType.Sqlite:
                    DependencyManagement.TrySqlite();
                    break;
                case DbType.Oracle:
                    DependencyManagement.TryOracle();
                    break;
                case DbType.PostgreSQL:
                    throw new Exception("开发中");
                default:
                    throw new Exception("ConnectionConfig.DbType is null");
            }
        }
        #endregion

        #region  ADO Methods
        /// <summary>
        ///Datathis.operation
        /// </summary>
        public virtual IAdo Ado
        {
            get
            {
                if (this.ContextAdo == null)
                {
                    var result = InstanceFactory.GetAdo(this.Context.CurrentConnectionConfig);
                    this.ContextAdo = result;
                    result.Context = this.Context;
                    return result;
                }
                return this.Context._Ado;
            }
        }
        #endregion

        #region Aop Log Methods
        public virtual AopProvider Aop { get { return new AopProvider(this.Context); } }
        #endregion

        #region Util Methods
        [Obsolete("Use SqlSugarClient.Utilities")]
        public virtual IContextMethods RewritableMethods
        {
            get { return this.Context.Utilities; }
            set { this.Context.Utilities = value; }
        }
        public virtual IContextMethods Utilities
        {
            get
            {
                if (ContextRewritableMethods == null)
                {
                    ContextRewritableMethods = new ContextMethods();
                    ContextRewritableMethods.Context = this.Context;
                }
                return ContextRewritableMethods;
            }
            set { ContextRewritableMethods = value; }
        }
        #endregion

        #region Queryable
        /// <summary>
        /// Get datebase time
        /// </summary>
        /// <returns></returns>
        public DateTime GetDate() {
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            return this.Ado.GetDateTime(sqlBuilder.FullSqlDateNow);
        }
        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>() where T : class, new()
        {

            InitMppingInfo<T>();
            var result = this.CreateQueryable<T>();
            return result;
        }
        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>(string shortName) where T : class, new()
        {
            var queryable = Queryable<T>();
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<ExpandoObject> Queryable(string tableName, string shortName)
        {
            var queryable = Queryable<ExpandoObject>();
            queryable.SqlBuilder.QueryBuilder.EntityName = tableName;
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2>();
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3>();
            var types = new Type[] { typeof(T2), typeof(T3) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        #region  9-12
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        #endregion
        public virtual ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2>();
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3>();
            var types = new Type[] { typeof(T2), typeof(T3) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T8>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }

        #region 9-12
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T8, T9>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T8, T9, T10>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T8, T9, T10, T11>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T8, T9, T10, T11, T12>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2> Queryable<T, T2>(
     ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, Expression<Func<T, T2, bool>> joinExpression) where T : class, new() where T2 : class, new()
        {
            return Queryable(joinQueryable1, joinQueryable2, JoinType.Inner, joinExpression);
        }
        public virtual ISugarQueryable<T, T2> Queryable<T, T2>(
             ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, JoinType joinType, Expression<Func<T, T2, bool>> joinExpression) where T : class, new() where T2 : class, new()
        {
            Check.Exception(joinQueryable1.QueryBuilder.Take != null || joinQueryable1.QueryBuilder.Skip != null || joinQueryable1.QueryBuilder.OrderByValue.HasValue(), "joinQueryable1 Cannot have 'Skip' 'ToPageList' 'Take' Or 'OrderBy'");
            Check.Exception(joinQueryable2.QueryBuilder.Take != null || joinQueryable2.QueryBuilder.Skip != null || joinQueryable2.QueryBuilder.OrderByValue.HasValue(), "joinQueryable2 Cannot have 'Skip' 'ToPageList' 'Take' Or 'OrderBy'");

            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);

            sqlBuilder.Context = this.Context;
            InitMppingInfo<T, T2>();
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2>(this.CurrentConnectionConfig);
            queryable.Context = this.Context;
            queryable.SqlBuilder = sqlBuilder;
            queryable.QueryBuilder = InstanceFactory.GetQueryBuilder(this.CurrentConnectionConfig);
            queryable.QueryBuilder.JoinQueryInfos = new List<JoinQueryInfo>();
            queryable.QueryBuilder.Builder = sqlBuilder;
            queryable.QueryBuilder.Context = this.Context;
            queryable.QueryBuilder.EntityType = typeof(T);
            queryable.QueryBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);

            //master
            var shortName1 = joinExpression.Parameters[0].Name;
            var sqlObj1 = joinQueryable1.ToSql();
            string sql1 = sqlObj1.Key;
            UtilMethods.RepairReplicationParameters(ref sql1, sqlObj1.Value.ToArray(), 0);
            queryable.QueryBuilder.EntityName = sqlBuilder.GetPackTable(sql1, shortName1); ;
            queryable.QueryBuilder.Parameters.AddRange(sqlObj1.Value);

            //join table 1
            var shortName2 = joinExpression.Parameters[1].Name;
            var sqlObj2 = joinQueryable2.ToSql();
            string sql2 = sqlObj2.Key;
            UtilMethods.RepairReplicationParameters(ref sql2, sqlObj2.Value.ToArray(), 1);
            queryable.QueryBuilder.Parameters.AddRange(sqlObj2.Value);
            var exp = queryable.QueryBuilder.GetExpressionValue(joinExpression, ResolveExpressType.WhereMultiple);
            queryable.QueryBuilder.JoinQueryInfos.Add(new JoinQueryInfo() { JoinIndex = 0, JoinType = joinType, JoinWhere = exp.GetResultString(), TableName = sqlBuilder.GetPackTable(sql2, shortName2) });

            return queryable;
        }
        #endregion

        public virtual ISugarQueryable<T> UnionAll<T>(params ISugarQueryable<T>[] queryables) where T : class, new()
        {
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            Check.Exception(queryables.IsNullOrEmpty(), "UnionAll.queryables is null ");
            int i = 1;
            List<KeyValuePair<string, List<SugarParameter>>> allItems = new List<KeyValuePair<string, List<SugarParameter>>>();
            foreach (var item in queryables)
            {
                var sqlObj = item.ToSql();
                string sql = sqlObj.Key;
                UtilMethods.RepairReplicationParameters(ref sql, sqlObj.Value.ToArray(), i);
                if (sqlObj.Value.HasValue())
                    allItems.Add(new KeyValuePair<string, List<SugarParameter>>(sql, sqlObj.Value));
                else
                    allItems.Add(new KeyValuePair<string, List<SugarParameter>>(sql, new List<SugarParameter>()));
                i++;
            }
            var allSql = sqlBuilder.GetUnionAllSql(allItems.Select(it => it.Key).ToList());
            var allParameters = allItems.SelectMany(it => it.Value).ToArray();
            var resulut = this.Context.Queryable<ExpandoObject>().AS(UtilMethods.GetPackTable(allSql, "unionTable")).With(SqlWith.Null);
            resulut.AddParameters(allParameters);
            return resulut.Select<T>(sqlBuilder.SqlSelectAll);
        }
        public virtual ISugarQueryable<T> UnionAll<T>(List<ISugarQueryable<T>> queryables) where T : class, new()
        {
            Check.Exception(queryables.IsNullOrEmpty(), "UnionAll.queryables is null ");
            return UnionAll(queryables.ToArray());
        }
        public virtual ISugarQueryable<T> Union<T>(params ISugarQueryable<T>[] queryables) where T : class, new()
        {
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            Check.Exception(queryables.IsNullOrEmpty(), "UnionAll.queryables is null ");
            int i = 1;
            List<KeyValuePair<string, List<SugarParameter>>> allItems = new List<KeyValuePair<string, List<SugarParameter>>>();
            foreach (var item in queryables)
            {
                var sqlObj = item.ToSql();
                string sql = sqlObj.Key;
                UtilMethods.RepairReplicationParameters(ref sql, sqlObj.Value.ToArray(), i);
                if (sqlObj.Value.HasValue())
                    allItems.Add(new KeyValuePair<string, List<SugarParameter>>(sql, sqlObj.Value));
                else
                    allItems.Add(new KeyValuePair<string, List<SugarParameter>>(sql, new List<SugarParameter>()));
                i++;
            }
            var allSql = sqlBuilder.GetUnionSql(allItems.Select(it => it.Key).ToList());
            var allParameters = allItems.SelectMany(it => it.Value).ToArray();
            var resulut = this.Context.Queryable<ExpandoObject>().AS(UtilMethods.GetPackTable(allSql, "unionTable")).With(SqlWith.Null);
            resulut.AddParameters(allParameters);
            return resulut.Select<T>(sqlBuilder.SqlSelectAll);
        }
        public virtual ISugarQueryable<T> Union<T>(List<ISugarQueryable<T>> queryables) where T : class, new()
        {
            Check.Exception(queryables.IsNullOrEmpty(), "Union.queryables is null ");
            return Union(queryables.ToArray());
        }
        #endregion

        #region SqlQueryable
        public ISugarQueryable<T> SqlQueryable<T>(string sql) where T : class, new()
        {
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            return this.Context.Queryable<T>().AS(sqlBuilder.GetPackTable(sql, sqlBuilder.GetDefaultShortName())).With(SqlWith.Null).Select(sqlBuilder.GetDefaultShortName()+".*");
        }
        #endregion

        #region Insertable
        public virtual IInsertable<T> Insertable<T>(T[] insertObjs) where T : class, new()
        {
            InitMppingInfo<T>();
            InsertableProvider<T> result = this.CreateInsertable(insertObjs);
            return result;
        }
        public virtual IInsertable<T> Insertable<T>(List<T> insertObjs) where T : class, new()
        {
            Check.ArgumentNullException(insertObjs, "Insertable.insertObjs can't be null");
            return this.Context.Insertable(insertObjs.ToArray());
        }
        public virtual IInsertable<T> Insertable<T>(T insertObj) where T : class, new()
        {
            return this.Context.Insertable(new T[] { insertObj });
        }
        public virtual IInsertable<T> Insertable<T>(Dictionary<string, object> columnDictionary) where T : class, new()
        {
            InitMppingInfo<T>();
            Check.Exception(columnDictionary == null || columnDictionary.Count == 0, "Insertable.columnDictionary can't be null");
            var insertObject = this.Context.Utilities.DeserializeObject<T>(this.Context.Utilities.SerializeObject(columnDictionary));
            var columns = columnDictionary.Select(it => it.Key).ToList();
            return this.Context.Insertable(insertObject).InsertColumns(it => columns.Any(c => it.Equals(c, StringComparison.CurrentCultureIgnoreCase))); ;
        }
        public virtual IInsertable<T> Insertable<T>(dynamic insertDynamicObject) where T : class, new()
        {
            InitMppingInfo<T>();
            if (insertDynamicObject is T)
            {
                return this.Context.Insertable((T)insertDynamicObject);
            }
            else
            {
                var columns = ((object)insertDynamicObject).GetType().GetProperties().Select(it => it.Name).ToList();
                Check.Exception(columns.IsNullOrEmpty(), "Insertable.updateDynamicObject can't be null");
                T insertObject = this.Context.Utilities.DeserializeObject<T>(this.Context.Utilities.SerializeObject(insertDynamicObject));
                return this.Context.Insertable(insertObject).InsertColumns(it => columns.Any(c => it.Equals(c, StringComparison.CurrentCultureIgnoreCase)));
            }
        }
        #endregion

        #region Deleteable
        public virtual IDeleteable<T> Deleteable<T>() where T : class, new()
        {
            InitMppingInfo<T>();
            DeleteableProvider<T> result = this.CreateDeleteable<T>();
            return result;
        }
        public virtual IDeleteable<T> Deleteable<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            InitMppingInfo<T>();
            return this.Context.Deleteable<T>().Where(expression);
        }
        public virtual IDeleteable<T> Deleteable<T>(dynamic primaryKeyValue) where T : class, new()
        {
            InitMppingInfo<T>();
            return this.Context.Deleteable<T>().In(primaryKeyValue);
        }
        public virtual IDeleteable<T> Deleteable<T>(dynamic[] primaryKeyValues) where T : class, new()
        {
            InitMppingInfo<T>();
            return this.Context.Deleteable<T>().In(primaryKeyValues);
        }
        public virtual IDeleteable<T> Deleteable<T>(List<dynamic> pkValue) where T : class, new()
        {
            InitMppingInfo<T>();
            return this.Context.Deleteable<T>().In(pkValue);
        }
        public virtual IDeleteable<T> Deleteable<T>(T deleteObj) where T : class, new()
        {
            InitMppingInfo<T>();
            return this.Context.Deleteable<T>().Where(deleteObj);
        }
        public virtual IDeleteable<T> Deleteable<T>(List<T> deleteObjs) where T : class, new()
        {
            InitMppingInfo<T>();
            return this.Context.Deleteable<T>().Where(deleteObjs);
        }
        #endregion

        #region Updateable
        public virtual IUpdateable<T> Updateable<T>(T[] UpdateObjs) where T : class, new()
        {
            InitMppingInfo<T>();
            UpdateableProvider<T> result = this.CreateUpdateable(UpdateObjs);
            return result;
        }
        public virtual IUpdateable<T> Updateable<T>(List<T> UpdateObjs) where T : class, new()
        {
            Check.ArgumentNullException(UpdateObjs, "Updateable.UpdateObjs can't be null");
            return Updateable(UpdateObjs.ToArray());
        }
        public virtual IUpdateable<T> Updateable<T>(T UpdateObj) where T : class, new()
        {
            return this.Context.Updateable(new T[] { UpdateObj });
        }
        public virtual IUpdateable<T> Updateable<T>() where T : class, new()
        {
            return this.Context.Updateable(new T[] { new T() });
        }
        public virtual IUpdateable<T> Updateable<T>(Dictionary<string, object> columnDictionary) where T : class, new()
        {
            InitMppingInfo<T>();
            Check.Exception(columnDictionary == null || columnDictionary.Count == 0, "Updateable.columnDictionary can't be null");
            var updateObject = this.Context.Utilities.DeserializeObject<T>(this.Context.Utilities.SerializeObject(columnDictionary));
            var columns = columnDictionary.Select(it => it.Key).ToList();
            return this.Context.Updateable(updateObject).UpdateColumns(it => columns.Any(c => it.Equals(c, StringComparison.CurrentCultureIgnoreCase))); ;
        }
        public virtual IUpdateable<T> Updateable<T>(dynamic updateDynamicObject) where T : class, new()
        {
            InitMppingInfo<T>();
            if (updateDynamicObject is T)
            {
                return this.Context.Updateable((T)updateDynamicObject);
            }
            else
            {
                var columns = ((object)updateDynamicObject).GetType().GetProperties().Select(it => it.Name).ToList();
                Check.Exception(columns.IsNullOrEmpty(), "Updateable.updateDynamicObject can't be null");
                T updateObject = this.Context.Utilities.DeserializeObject<T>(this.Context.Utilities.SerializeObject(updateDynamicObject));
                return this.Context.Updateable(updateObject).UpdateColumns(it => columns.Any(c => it.Equals(c, StringComparison.CurrentCultureIgnoreCase))); ;
            }
        }
        #endregion

        #region DbFirst
        public virtual IDbFirst DbFirst
        {
            get
            {
                IDbFirst dbFirst = InstanceFactory.GetDbFirst(this.Context.CurrentConnectionConfig);
                dbFirst.Context = this.Context;
                dbFirst.Init();
                return dbFirst;
            }
        }
        #endregion

        #region CodeFirst
        public virtual ICodeFirst CodeFirst
        {
            get
            {
                ICodeFirst codeFirst = InstanceFactory.GetCodeFirst(this.Context.CurrentConnectionConfig);
                codeFirst.Context = this.Context;
                return codeFirst;
            }
        }
        #endregion

        #region Db Maintenance
        public virtual IDbMaintenance DbMaintenance
        {
            get
            {
                if (this.Context._DbMaintenance == null)
                {
                    IDbMaintenance maintenance = InstanceFactory.GetDbMaintenance(this.Context.CurrentConnectionConfig);
                    this.Context._DbMaintenance = maintenance;
                    maintenance.Context = this.Context;
                }
                return this.Context._DbMaintenance;
            }
        }
        #endregion

        #region Entity Maintenance
        [Obsolete("Use SqlSugarClient.EntityMaintenance")]
        public virtual EntityMaintenance EntityProvider
        {
            get { return this.Context.EntityMaintenance; }
            set { this.Context.EntityMaintenance = value; }
        }
        public virtual EntityMaintenance EntityMaintenance
        {
            get
            {
                if (this.Context._EntityProvider == null)
                {
                    this.Context._EntityProvider = new EntityMaintenance();
                    this.Context._EntityProvider.Context = this.Context;
                }
                return this.Context._EntityProvider;
            }
            set { this.Context._EntityProvider = value; }
        }
        #endregion

        #region Gobal Filter
        public virtual QueryFilterProvider QueryFilter
        {
            get
            {
                if (this.Context._QueryFilterProvider == null)
                {
                    this.Context._QueryFilterProvider = new QueryFilterProvider();
                    this.Context._QueryFilterProvider.Context = this.Context;
                }
                return this.Context._QueryFilterProvider;
            }
            set { this.Context._QueryFilterProvider = value; }
        }
        #endregion

        #region SimpleClient
        [Obsolete("Use SqlSugarClient.GetSimpleClient() Or SqlSugarClient.GetSimpleClient<T>() ")]
        public virtual SimpleClient SimpleClient
        {
            get
            {
                if (this.Context._SimpleClient == null)
                    this.Context._SimpleClient = new SimpleClient(this.Context);
                return this.Context._SimpleClient;
            }
        }
        public virtual SimpleClient<T> GetSimpleClient<T>() where T : class, new()
        {
            return new SimpleClient<T>(this.Context);
        }
        public virtual SimpleClient GetSimpleClient()
        {
            if (this.Context._SimpleClient == null)
                this.Context._SimpleClient = new SimpleClient(this.Context);
            return this.Context._SimpleClient;
        }
        #endregion

        #region Dispose OR Close
        public virtual void Close()
        {
            if (this.Context.Ado != null)
                this.Context.Ado.Close();
        }
        public virtual void Open()
        {
            if (this.Context.Ado != null)
                this.Context.Ado.Open();
        }
        public virtual void Dispose()
        {
            if (this.Context.Ado != null)
                this.Context.Ado.Dispose();
        }
        #endregion
    }
}
