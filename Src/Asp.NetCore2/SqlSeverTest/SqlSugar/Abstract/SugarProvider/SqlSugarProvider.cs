using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SqlSugar
{
    ///<summary>
    /// ** description：Create datathis.access object
    /// ** author：sunkaixuan
    /// ** date：2017/1/2
    /// ** email:610262374@qq.com
    /// </summary>
    public partial class SqlSugarProvider : ISqlSugarClient
    {

        #region Constructor
        public SqlSugarProvider(ConnectionConfig config)
        {
            this.Context = this;
            this.CurrentConnectionConfig = config;
            this.ContextID = Guid.NewGuid();
            Check.ArgumentNullException(config, "config is null");
            CheckDbDependency(config);
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
                    result.Context = this;
                    return result;
                }
                return this._Ado;
            }
        }
        #endregion

        #region Aop Log Methods
        public virtual AopProvider Aop { get { return new AopProvider(this); } }
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
                    ContextRewritableMethods.Context = this;
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
        public DateTime GetDate()
        {
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            return this.Ado.GetDateTime(sqlBuilder.FullSqlDateNow);
        }
        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>()
        {

            InitMappingInfo<T>();
            var result = this.CreateQueryable<T>();
            return result;
        }
        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>(string shortName)
        {
            Check.Exception(shortName.HasValue() && shortName.Length > 20, ErrorMessage.GetThrowMessage("shortName参数长度不能超过20，你可能是想用这个方法 db.SqlQueryable(sql)而不是db.Queryable(shortName)", "Queryable.shortName max length 20"));
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
        public virtual ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression)
        {
            InitMappingInfo<T, T2>();
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, JoinQueryInfos>> joinExpression)
        {
            InitMappingInfo<T, T2>();
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression)
        {
            InitMappingInfo<T, T2, T3>();
            var types = new Type[] { typeof(T2), typeof(T3) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, JoinQueryInfos>> joinExpression)
        {
            InitMappingInfo<T, T2, T3>();
            var types = new Type[] { typeof(T2), typeof(T3) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, JoinQueryInfos>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, JoinQueryInfos>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, JoinQueryInfos>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, JoinQueryInfos>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, JoinQueryInfos>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        #region  9-12
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, JoinQueryInfos>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, JoinQueryInfos>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object[]>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, JoinQueryInfos>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object[]>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, JoinQueryInfos>> joinExpression)
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.CurrentConnectionConfig);
            this.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        #endregion
        public virtual ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, bool>> joinExpression) where T : class, new()
        {
            InitMappingInfo<T, T2>();
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, bool>> joinExpression) where T : class, new()
        {
            InitMappingInfo<T, T2, T3>();
            var types = new Type[] { typeof(T2), typeof(T3) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression) where T : class, new()
        {
            InitMappingInfo<T, T2, T3, T4>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression) where T : class, new()
        {
            InitMappingInfo<T, T2, T3, T4, T5>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression) where T : class, new()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression) where T : class, new()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression) where T : class, new()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }

        #region 9-12
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression) where T : class, new()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression) where T : class, new()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression) where T : class, new()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression) where T : class, new()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.CurrentConnectionConfig);
            this.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T>(ISugarQueryable<T> queryable) where T : class, new()
        {
            var sqlobj = queryable.ToSql();
            return this.SqlQueryable<T>(sqlobj.Key).AddParameters(sqlobj.Value);
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

            sqlBuilder.Context = this;
            InitMappingInfo<T, T2>();
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2>(this.CurrentConnectionConfig);
            queryable.Context = this.Context;
            queryable.SqlBuilder = sqlBuilder;
            queryable.QueryBuilder = InstanceFactory.GetQueryBuilder(this.CurrentConnectionConfig);
            queryable.QueryBuilder.JoinQueryInfos = new List<JoinQueryInfo>();
            queryable.QueryBuilder.Builder = sqlBuilder;
            queryable.QueryBuilder.Context = this;
            queryable.QueryBuilder.EntityType = typeof(T);
            queryable.QueryBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);

            //master
            var shortName1 = joinExpression.Parameters[0].Name;
            var sqlObj1 = joinQueryable1.ToSql();
            string sql1 = sqlObj1.Key;
            UtilMethods.RepairReplicationParameters(ref sql1, sqlObj1.Value.ToArray(), 0, "Join");
            queryable.QueryBuilder.EntityName = sqlBuilder.GetPackTable(sql1, shortName1); ;
            queryable.QueryBuilder.Parameters.AddRange(sqlObj1.Value);

            //join table 1
            var shortName2 = joinExpression.Parameters[1].Name;
            var sqlObj2 = joinQueryable2.ToSql();
            string sql2 = sqlObj2.Key;
            UtilMethods.RepairReplicationParameters(ref sql2, sqlObj2.Value.ToArray(), 1, "Join");
            queryable.QueryBuilder.Parameters.AddRange(sqlObj2.Value);
            var exp = queryable.QueryBuilder.GetExpressionValue(joinExpression, ResolveExpressType.WhereMultiple);
            queryable.QueryBuilder.JoinQueryInfos.Add(new JoinQueryInfo() { JoinIndex = 0, JoinType = joinType, JoinWhere = exp.GetResultString(), TableName = sqlBuilder.GetPackTable(sql2, shortName2) });

            return queryable;
        }


        public virtual ISugarQueryable<T, T2 ,T3> Queryable<T, T2 ,T3>(
                ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, ISugarQueryable<T3> joinQueryable3,
                JoinType joinType1, Expression<Func<T, T2, T3, bool>> joinExpression1 ,
                JoinType joinType2 , Expression<Func<T, T2, T3,bool>> joinExpression2
            ) where T : class, new() where T2 : class, new() where T3 : class, new()
        {
            Check.Exception(joinQueryable1.QueryBuilder.Take != null || joinQueryable1.QueryBuilder.Skip != null || joinQueryable1.QueryBuilder.OrderByValue.HasValue(), "joinQueryable1 Cannot have 'Skip' 'ToPageList' 'Take' Or 'OrderBy'");
            Check.Exception(joinQueryable2.QueryBuilder.Take != null || joinQueryable2.QueryBuilder.Skip != null || joinQueryable2.QueryBuilder.OrderByValue.HasValue(), "joinQueryable2 Cannot have 'Skip' 'ToPageList' 'Take' Or 'OrderBy'");
            Check.Exception(joinQueryable3.QueryBuilder.Take != null || joinQueryable3.QueryBuilder.Skip != null || joinQueryable3.QueryBuilder.OrderByValue.HasValue(), "joinQueryable3 Cannot have 'Skip' 'ToPageList' 'Take' Or 'OrderBy'");

            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);

            sqlBuilder.Context = this;
            InitMappingInfo<T, T2,T3>();
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2,T3>(this.CurrentConnectionConfig);
            queryable.Context = this.Context;
            queryable.SqlBuilder = sqlBuilder;
            queryable.QueryBuilder = InstanceFactory.GetQueryBuilder(this.CurrentConnectionConfig);
            queryable.QueryBuilder.JoinQueryInfos = new List<JoinQueryInfo>();
            queryable.QueryBuilder.Builder = sqlBuilder;
            queryable.QueryBuilder.Context = this;
            queryable.QueryBuilder.EntityType = typeof(T);
            queryable.QueryBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);

            //master
            var shortName1 = joinExpression1.Parameters[0].Name;
            var sqlObj1 = joinQueryable1.ToSql();
            string sql1 = sqlObj1.Key;
            UtilMethods.RepairReplicationParameters(ref sql1, sqlObj1.Value.ToArray(), 0, "Join");
            queryable.QueryBuilder.EntityName = sqlBuilder.GetPackTable(sql1, shortName1); ;
            queryable.QueryBuilder.Parameters.AddRange(sqlObj1.Value);

            //join table 1
            var shortName2 = joinExpression1.Parameters[1].Name;
            var sqlObj2 = joinQueryable2.ToSql();
            string sql2 = sqlObj2.Key;
            UtilMethods.RepairReplicationParameters(ref sql2, sqlObj2.Value.ToArray(), 1, "Join");
            queryable.QueryBuilder.Parameters.AddRange(sqlObj2.Value);
            var exp = queryable.QueryBuilder.GetExpressionValue(joinExpression1, ResolveExpressType.WhereMultiple);
            queryable.QueryBuilder.JoinQueryInfos.Add(new JoinQueryInfo() { JoinIndex = 0, JoinType = joinType1, JoinWhere = exp.GetResultString(), TableName = sqlBuilder.GetPackTable(sql2, shortName2) });


            //join table 2
            var shortName3 = joinExpression1.Parameters[2].Name;
            var sqlObj3 = joinQueryable3.ToSql();
            string sql3 = sqlObj3.Key;
            UtilMethods.RepairReplicationParameters(ref sql3, sqlObj3.Value.ToArray(), 2, "Join");
            queryable.QueryBuilder.Parameters.AddRange(sqlObj3.Value);
            var exp2 = queryable.QueryBuilder.GetExpressionValue(joinExpression2, ResolveExpressType.WhereMultiple);
            queryable.QueryBuilder.JoinQueryInfos.Add(new JoinQueryInfo() { JoinIndex = 1, JoinType = joinType2, JoinWhere = exp2.GetResultString(), TableName = sqlBuilder.GetPackTable(sql3, shortName3) });
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
                UtilMethods.RepairReplicationParameters(ref sql, sqlObj.Value.ToArray(), i, "UnionAll");
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
            if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle && sqlBuilder.SqlSelectAll == "*")
            {
                return resulut.Select<T>("unionTable.*");
            }
            else
            {
                return resulut.Select<T>(sqlBuilder.SqlSelectAll);
            }
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
                UtilMethods.RepairReplicationParameters(ref sql, sqlObj.Value.ToArray(), i, "Union");
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
            if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle && sqlBuilder.SqlSelectAll == "*")
            {
                return resulut.Select<T>("unionTable.*");
            }
            else
            {
                return resulut.Select<T>(sqlBuilder.SqlSelectAll);
            }
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
            return this.Context.Queryable<T>().AS(sqlBuilder.GetPackTable(sql, sqlBuilder.GetDefaultShortName())).With(SqlWith.Null).Select(sqlBuilder.GetDefaultShortName() + ".*");
        }
        #endregion

        #region Insertable
        public virtual IInsertable<T> Insertable<T>(T[] insertObjs) where T : class, new()
        {
            InitMappingInfo<T>();
            InsertableProvider<T> result = this.CreateInsertable(insertObjs);
            return result;
        }
        public virtual IInsertable<T> Insertable<T>(List<T> insertObjs) where T : class, new()
        {
            if (insertObjs == null || insertObjs.IsNullOrEmpty())
            {
                insertObjs = new List<T>();
                insertObjs.Add(default(T));
            }
            return this.Context.Insertable(insertObjs.ToArray());
        }
        public virtual IInsertable<T> Insertable<T>(T insertObj) where T : class, new()
        {
            return this.Context.Insertable(new T[] { insertObj });
        }
        public virtual IInsertable<T> Insertable<T>(Dictionary<string, object> columnDictionary) where T : class, new()
        {
            InitMappingInfo<T>();
            Check.Exception(columnDictionary == null || columnDictionary.Count == 0, "Insertable.columnDictionary can't be null");
            var insertObject = this.Context.Utilities.DeserializeObject<T>(this.Context.Utilities.SerializeObject(columnDictionary));
            var columns = columnDictionary.Select(it => it.Key).ToList();
            return this.Context.Insertable(insertObject).InsertColumns(columns.ToArray()); ;
        }
        public virtual IInsertable<T> Insertable<T>(dynamic insertDynamicObject) where T : class, new()
        {
            InitMappingInfo<T>();
            if (insertDynamicObject is T)
            {
                return this.Context.Insertable((T)insertDynamicObject);
            }
            else
            {
                var columns = ((object)insertDynamicObject).GetType().GetProperties().Select(it => it.Name).ToList();
                Check.Exception(columns.IsNullOrEmpty(), "Insertable.updateDynamicObject can't be null");
                T insertObject = this.Context.Utilities.DeserializeObject<T>(this.Context.Utilities.SerializeObject(insertDynamicObject));
                return this.Context.Insertable(insertObject).InsertColumns(columns.ToArray());
            }
        }
        #endregion

        #region Deleteable
        public virtual IDeleteable<T> Deleteable<T>() where T : class, new()
        {
            InitMappingInfo<T>();
            DeleteableProvider<T> result = this.CreateDeleteable<T>();
            return result;
        }
        public virtual IDeleteable<T> Deleteable<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            InitMappingInfo<T>();
            return this.Context.Deleteable<T>().Where(expression);
        }
        public virtual IDeleteable<T> Deleteable<T>(dynamic primaryKeyValue) where T : class, new()
        {
            InitMappingInfo<T>();
            return this.Context.Deleteable<T>().In(primaryKeyValue);
        }
        public virtual IDeleteable<T> Deleteable<T>(dynamic[] primaryKeyValues) where T : class, new()
        {
            InitMappingInfo<T>();
            return this.Context.Deleteable<T>().In(primaryKeyValues);
        }
        public virtual IDeleteable<T> Deleteable<T>(List<dynamic> pkValue) where T : class, new()
        {
            InitMappingInfo<T>();
            return this.Context.Deleteable<T>().In(pkValue);
        }
        public virtual IDeleteable<T> Deleteable<T>(T deleteObj) where T : class, new()
        {
            InitMappingInfo<T>();
            return this.Context.Deleteable<T>().Where(deleteObj);
        }
        public virtual IDeleteable<T> Deleteable<T>(List<T> deleteObjs) where T : class, new()
        {
            InitMappingInfo<T>();
            return this.Context.Deleteable<T>().Where(deleteObjs);
        }
        #endregion

        #region Updateable
        public virtual IUpdateable<T> Updateable<T>(T[] UpdateObjs) where T : class, new()
        {
            InitMappingInfo<T>();
            UpdateableProvider<T> result = this.CreateUpdateable(UpdateObjs);
            return result;
        }
        public virtual IUpdateable<T> Updateable<T>(List<T> UpdateObjs) where T : class, new()
        {
            //Check.ArgumentNullException(UpdateObjs, "Updateable.UpdateObjs can't be null");
            if (UpdateObjs == null)
            {
                UpdateObjs = new List<T>();
            }
            return Updateable(UpdateObjs.ToArray());
        }
        public virtual IUpdateable<T> Updateable<T>(T UpdateObj) where T : class, new()
        {
            return this.Context.Updateable(new T[] { UpdateObj });
        }
        public virtual IUpdateable<T> Updateable<T>() where T : class, new()
        {
            var result = this.Context.Updateable(new T[] { new T() });
            result.UpdateParameterIsNull = true;
            return result;
        }
        public virtual IUpdateable<T> Updateable<T>(Expression<Func<T, T>> columns) where T : class, new()
        {
            var result = this.Context.Updateable<T>().SetColumns(columns);
            result.UpdateParameterIsNull = true;
            return result;
        }
        public virtual IUpdateable<T> Updateable<T>(Expression<Func<T, bool>> columns) where T : class, new()
        {
            var result = this.Context.Updateable<T>().SetColumns(columns);
            result.UpdateParameterIsNull = true;
            return result;
        }


        public virtual IUpdateable<T> Updateable<T>(Dictionary<string, object> columnDictionary) where T : class, new()
        {
            InitMappingInfo<T>();
            Check.Exception(columnDictionary == null || columnDictionary.Count == 0, "Updateable.columnDictionary can't be null");
            var updateObject = this.Context.Utilities.DeserializeObject<T>(this.Context.Utilities.SerializeObject(columnDictionary));
            var columns = columnDictionary.Select(it => it.Key).ToList();
            return this.Context.Updateable(updateObject).UpdateColumns(columns.ToArray()); ;
        }
        public virtual IUpdateable<T> Updateable<T>(dynamic updateDynamicObject) where T : class, new()
        {
            InitMappingInfo<T>();
            if (updateDynamicObject is T)
            {
                return this.Context.Updateable((T)updateDynamicObject);
            }
            else
            {
                var columns = ((object)updateDynamicObject).GetType().GetProperties().Select(it => it.Name).ToList();
                Check.Exception(columns.IsNullOrEmpty(), "Updateable.updateDynamicObject can't be null");
                T updateObject = this.Context.Utilities.DeserializeObject<T>(this.Context.Utilities.SerializeObject(updateDynamicObject));
                return this.Context.Updateable(updateObject).UpdateColumns(columns.ToArray()); ;
            }
        }
        #endregion

        #region Saveable
        public ISaveable<T> Saveable<T>(List<T> saveObjects) where T : class, new()
        {
            return new SaveableProvider<T>(this, saveObjects);
        }
        public ISaveable<T> Saveable<T>(T saveObject) where T : class, new()
        {
            return new SaveableProvider<T>(this, saveObject);
        }
        public IStorageable<T> Storageable<T>(List<T> dataList) where T : class, new()
        {
            this.InitMappingInfo<T>();
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            var result= new Storageable<T>(dataList,this);
            result.Builder = sqlBuilder;
            return result;
        }
        public IStorageable<T> Storageable<T>(T data) where T : class, new()
        {
            return Storageable(new List<T>() { data });
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
                codeFirst.Context = this;
                return codeFirst;
            }
        }
        #endregion

        #region Db Maintenance
        public virtual IDbMaintenance DbMaintenance
        {
            get
            {
                if (this._DbMaintenance == null)
                {
                    IDbMaintenance maintenance = InstanceFactory.GetDbMaintenance(this.Context.CurrentConnectionConfig);
                    this._DbMaintenance = maintenance;
                    maintenance.Context = this;
                }
                return this._DbMaintenance;
            }
        }
        #endregion

        #region Entity Maintenance
        public virtual EntityMaintenance EntityMaintenance
        {
            get
            {
                if (this._EntityProvider == null)
                {
                    this._EntityProvider = new EntityMaintenance();
                    this._EntityProvider.Context = this;
                }
                return this._EntityProvider;
            }
            set { this._EntityProvider = value; }
        }
        #endregion

        #region Gobal Filter
        public virtual QueryFilterProvider QueryFilter
        {
            get
            {
                if (this._QueryFilterProvider == null)
                {
                    this._QueryFilterProvider = new QueryFilterProvider();
                    this._QueryFilterProvider.Context = this;
                }
                return this._QueryFilterProvider;
            }
            set { this._QueryFilterProvider = value; }
        }
        #endregion

        #region SimpleClient
        [Obsolete("Use SqlSugarClient.GetSimpleClient() Or SqlSugarClient.GetSimpleClient<T>() ")]
        public virtual SimpleClient SimpleClient
        {
            get
            {
                if (this._SimpleClient == null)
                    this._SimpleClient = new SimpleClient(this);
                return this._SimpleClient;
            }
        }
        public virtual SimpleClient<T> GetSimpleClient<T>() where T : class, new()
        {
            return new SimpleClient<T>(this);
        }
        //public virtual SimpleClient GetSimpleClient()
        //{
        //    if (this._SimpleClient == null)
        //        this._SimpleClient = new SimpleClient(this);
        //    return this._SimpleClient;
        //}
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

        #region   Queue
        public int SaveQueues(bool isTran = true)
        {
            return SaveQueuesProvider(isTran, (sql, parameters) => { return this.Ado.ExecuteCommand(sql, parameters); });
        }

        public async Task<int> SaveQueuesAsync(bool isTran = true)
        {
            return await SaveQueuesProviderAsync(isTran, (sql, parameters) => { return this.Ado.ExecuteCommandAsync(sql, parameters); });
        }
        public List<T> SaveQueues<T>(bool isTran = true)
        {
            return SaveQueuesProvider(isTran, (sql, parameters) => { return this.Ado.SqlQuery<T>(sql, parameters); });
        }
        public async Task<List<T>> SaveQueuesAsync<T>(bool isTran = true)
        {
            return await SaveQueuesProviderAsync(isTran, (sql, parameters) => { return this.Ado.SqlQueryAsync<T>(sql, parameters); });
        }
        public Tuple<List<T>, List<T2>> SaveQueues<T, T2>(bool isTran = true)
        {
            return SaveQueuesProvider(isTran, (sql, parameters) => { return this.Ado.SqlQuery<T, T2>(sql, parameters); });
        }
        public async Task<Tuple<List<T>, List<T2>>> SaveQueuesAsync<T, T2>(bool isTran = true)
        {
            return await SaveQueuesProviderAsync(isTran, (sql, parameters) => { return this.Ado.SqlQueryAsync<T, T2>(sql, parameters); });
        }
        public Tuple<List<T>, List<T2>, List<T3>> SaveQueues<T, T2, T3>(bool isTran = true)
        {
            return SaveQueuesProvider(isTran, (sql, parameters) => { return this.Ado.SqlQuery<T, T2, T3>(sql, parameters); });
        }
        public async Task<Tuple<List<T>, List<T2>, List<T3>>> SaveQueuesAsync<T, T2, T3>(bool isTran = true)
        {
            return await SaveQueuesProviderAsync(isTran, (sql, parameters) => { return this.Ado.SqlQueryAsync<T, T2, T3>(sql, parameters); });
        }
        public Tuple<List<T>, List<T2>, List<T3>, List<T4>> SaveQueues<T, T2, T3, T4>(bool isTran = true)
        {
            return SaveQueuesProvider(isTran, (sql, parameters) => { return this.Ado.SqlQuery<T, T2, T3, T4>(sql, parameters); });
        }
        public async Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>>> SaveQueuesAsync<T, T2, T3, T4>(bool isTran = true)
        {
            return await SaveQueuesProviderAsync(isTran, (sql, parameters) => { return this.Ado.SqlQueryAsync<T, T2, T3, T4>(sql, parameters); });
        }
        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>> SaveQueues<T, T2, T3, T4, T5>(bool isTran = true)
        {
            return SaveQueuesProvider(isTran, (sql, parameters) => { return this.Ado.SqlQuery<T, T2, T3, T4, T5>(sql, parameters); });
        }
        public async Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>>> SaveQueuesAsync<T, T2, T3, T4, T5>(bool isTran = true)
        {
            return await SaveQueuesProviderAsync(isTran, (sql, parameters) => { return this.Ado.SqlQueryAsync<T, T2, T3, T4, T5>(sql, parameters); });
        }
        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>> SaveQueues<T, T2, T3, T4, T5, T6>(bool isTran = true)
        {
            return SaveQueuesProvider(isTran, (sql, parameters) => { return this.Ado.SqlQuery<T, T2, T3, T4, T5, T6>(sql, parameters); });
        }
        public async Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>>> SaveQueuesAsync<T, T2, T3, T4, T5, T6>(bool isTran = true)
        {
            return await SaveQueuesProviderAsync(isTran, (sql, parameters) => { return this.Ado.SqlQueryAsync<T, T2, T3, T4, T5, T6>(sql, parameters); });
        }
        public Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>> SaveQueues<T, T2, T3, T4, T5, T6, T7>(bool isTran = true)
        {
            return SaveQueuesProvider(isTran, (sql, parameters) => { return this.Ado.SqlQuery<T, T2, T3, T4, T5, T6, T7>(sql, parameters); });
        }
        public async Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>>> SaveQueuesAsync<T, T2, T3, T4, T5, T6, T7>(bool isTran = true)
        {
            return await SaveQueuesProviderAsync(isTran, (sql, parameters) => { return this.Ado.SqlQueryAsync<T, T2, T3, T4, T5, T6, T7>(sql, parameters); });
        }
        public void AddQueue(string sql, object parsmeters = null)
        {
            if (Queues == null)
            {
                Queues = new QueueList();
            }
            this.Queues.Add(sql, this.Context.Ado.GetParameters(parsmeters));
        }
        public void AddQueue(string sql, SugarParameter parsmeter)
        {
            if (Queues == null)
            {
                Queues = new QueueList();
            }
            this.Queues.Add(sql, new List<SugarParameter>() { parsmeter });
        }
        public void AddQueue(string sql, List<SugarParameter> parsmeters)
        {
            if (Queues == null)
            {
                Queues = new QueueList();
            }
            this.Queues.Add(sql, parsmeters);
        }
        public QueueList Queues { get { if (_Queues == null) { _Queues = new QueueList(); } return _Queues; } set { _Queues = value; } }



        private async Task<T> SaveQueuesProviderAsync<T>(bool isTran, Func<string, List<SugarParameter>, Task<T>> func)
        {
            try
            {
                if (this.CurrentConnectionConfig.DbType == DbType.Oracle)
                {
                    throw new Exception("Oracle no support SaveQueues");
                }
                if (this.Queues == null || this.Queues.Count == 0) return default(T);
                isTran = isTran && this.Ado.Transaction == null;
                if (isTran) this.Ado.BeginTran();
                StringBuilder sqlBuilder = new StringBuilder();
                var parsmeters = new List<SugarParameter>();
                var index = 1;
                if (this.Queues.HasValue())
                {
                    foreach (var item in Queues)
                    {
                        if (item.Sql == null)
                            item.Sql = string.Empty;
                        if (item.Parameters == null)
                            item.Parameters = new SugarParameter[] { };
                        var itemParsmeters = item.Parameters.OrderByDescending(it => it.ParameterName.Length).ToList();
                        List<SugarParameter> addParameters = new List<SugarParameter>();
                        var itemSql = item.Sql;
                        foreach (var itemParameter in itemParsmeters)
                        {
                            var newName = itemParameter.ParameterName + "_q_" + index;
                            SugarParameter parameter = new SugarParameter(newName, itemParameter.Value);
                            parameter.DbType = itemParameter.DbType;
                            itemSql = UtilMethods.ReplaceSqlParameter(itemSql, itemParameter, newName);
                            addParameters.Add(parameter);
                        }
                        parsmeters.AddRange(addParameters);
                        itemSql = itemSql.TrimEnd(';') + ";";
                        sqlBuilder.AppendLine(itemSql);
                        index++;
                    }
                }
                this.Queues.Clear();
                var result = await func(sqlBuilder.ToString(), parsmeters);
                if (isTran) this.Ado.CommitTran();
                return result;
            }
            catch (Exception ex)
            {
                if (isTran) this.Ado.RollbackTran();
                throw ex;
            }
        }
        private T SaveQueuesProvider<T>(bool isTran, Func<string, List<SugarParameter>, T> func)
        {
            try
            {
                if (this.CurrentConnectionConfig.DbType == DbType.Oracle)
                {
                    throw new Exception("Oracle no support SaveQueues");
                }
                if (this.Queues == null || this.Queues.Count == 0) return default(T);
                isTran = isTran && this.Ado.Transaction == null;
                if (isTran) this.Ado.BeginTran();
                StringBuilder sqlBuilder = new StringBuilder();
                var parsmeters = new List<SugarParameter>();
                var index = 1;
                if (this.Queues.HasValue())
                {
                    foreach (var item in Queues)
                    {
                        if (item.Sql == null)
                            item.Sql = string.Empty;
                        if (item.Parameters == null)
                            item.Parameters = new SugarParameter[] { };
                        var itemParsmeters = item.Parameters.OrderByDescending(it => it.ParameterName.Length).ToList();
                        List<SugarParameter> addParameters = new List<SugarParameter>();
                        var itemSql = item.Sql;
                        foreach (var itemParameter in itemParsmeters)
                        {
                            var newName = itemParameter.ParameterName + "_q_" + index;
                            SugarParameter parameter = new SugarParameter(newName, itemParameter.Value);
                            parameter.DbType = itemParameter.DbType;
                            itemSql = UtilMethods.ReplaceSqlParameter(itemSql, itemParameter, newName);
                            addParameters.Add(parameter);
                        }
                        parsmeters.AddRange(addParameters);
                        itemSql = itemSql
                            .TrimEnd('\r')
                            .TrimEnd('\n')
                            .TrimEnd('\r')
                            .TrimEnd('\n')
                            .TrimEnd(';') + ";";
                        sqlBuilder.AppendLine(itemSql);
                        index++;
                    }
                }
                this.Queues.Clear();
                var result = func(sqlBuilder.ToString(), parsmeters);
                if (isTran) this.Ado.CommitTran();
                return result;
            }
            catch (Exception ex)
            {
                if (isTran) this.Ado.RollbackTran();
                throw ex;
            }
        }

        #endregion
    }
}
