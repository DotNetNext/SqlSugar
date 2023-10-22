using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
            if (StaticConfig.CompleteDbFunc != null) 
            {
                StaticConfig.CompleteDbFunc(this);
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
        public QueryMethodInfo QueryableByObject(Type entityType) 
        {
            QueryMethodInfo result = new QueryMethodInfo();
            var method=this.GetType().GetMyMethod("Queryable", 0);
            var methodT=method.MakeGenericMethod(entityType);
            var queryableObj=methodT.Invoke(this,new object[] {});
            result.QueryableObj = queryableObj;
            result.Context = this.Context;
            result.EntityType = entityType;
            return result;
        }
        public QueryMethodInfo QueryableByObject(Type entityType, string shortName)
        {
            return this.QueryableByObject(entityType).AS(this.Context.EntityMaintenance.GetTableName(entityType),shortName);
        }
        /// <summary>
        /// Get datebase time
        /// </summary>
        /// <returns></returns>
        public DateTime GetDate()
        {
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            return this.Ado.GetDateTime(sqlBuilder.FullSqlDateNow);
        }
        public ISugarQueryable<T> MasterQueryable<T>()
        {
            var result = this.Queryable<T>();
            result.QueryBuilder.IsDisableMasterSlaveSeparation = true;
            return result;
        }

        public ISugarQueryable<T> SlaveQueryable<T>()
        {
            var result = this.Queryable<T>();
            result.QueryBuilder.IsEnableMasterSlaveSeparation = true;
            return result;
        }
        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>()
        {

            InitMappingInfo<T>();
            var result = this.CreateQueryable<T>();
            UtilMethods.AddDiscrimator(typeof(T), result);
            return result;
        }
        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>(string shortName)
        {
            Check.Exception(shortName.HasValue() && shortName.Length > 40, ErrorMessage.GetThrowMessage("shortName参数长度不能超过40，你可能是想用这个方法 db.SqlQueryable(sql)而不是db.Queryable(shortName)", "Queryable.shortName max length 20"));
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
        public virtual ISugarQueryable<T> Queryable<T>(ISugarQueryable<T> queryable)  
        {
            var sqlobj = queryable.ToSql();
            var newQueryable = this.SqlQueryable<object>(sqlobj.Key).AddParameters(sqlobj.Value);
            var result = newQueryable.Select<T>(newQueryable.QueryBuilder.SelectValue+"");
            result.QueryBuilder.IsSqlQuery = false;
            result.QueryBuilder.NoCheckInclude = true;
            result.QueryBuilder.Includes = queryable.QueryBuilder.Includes?.ToList();
            return result;
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

        public virtual ISugarQueryable<T, T2, T3,T4> Queryable<T, T2, T3,T4>(
              ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, ISugarQueryable<T3> joinQueryable3, ISugarQueryable<T4> joinQueryable4,
              JoinType joinType1, Expression<Func<T, T2, T3, T4, bool>> joinExpression1,
              JoinType joinType2, Expression<Func<T, T2, T3, T4, bool>> joinExpression2,
               JoinType joinType3, Expression<Func<T, T2, T3,T4, bool>> joinExpression3
          ) where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new()
        {
            Check.Exception(joinQueryable1.QueryBuilder.Take != null || joinQueryable1.QueryBuilder.Skip != null || joinQueryable1.QueryBuilder.OrderByValue.HasValue(), "joinQueryable1 Cannot have 'Skip' 'ToPageList' 'Take' Or 'OrderBy'");
            Check.Exception(joinQueryable2.QueryBuilder.Take != null || joinQueryable2.QueryBuilder.Skip != null || joinQueryable2.QueryBuilder.OrderByValue.HasValue(), "joinQueryable2 Cannot have 'Skip' 'ToPageList' 'Take' Or 'OrderBy'");
            Check.Exception(joinQueryable3.QueryBuilder.Take != null || joinQueryable3.QueryBuilder.Skip != null || joinQueryable3.QueryBuilder.OrderByValue.HasValue(), "joinQueryable3 Cannot have 'Skip' 'ToPageList' 'Take' Or 'OrderBy'");
            Check.Exception(joinQueryable4.QueryBuilder.Take != null || joinQueryable4.QueryBuilder.Skip != null || joinQueryable4.QueryBuilder.OrderByValue.HasValue(), "joinQueryable4 Cannot have 'Skip' 'ToPageList' 'Take' Or 'OrderBy'");
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);

            sqlBuilder.Context = this;
            InitMappingInfo<T, T2, T3,T4>();
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3,T4>(this.CurrentConnectionConfig);
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

            //join table 3
            var shortName4 = joinExpression1.Parameters[3].Name;
            var sqlObj4 = joinQueryable4.ToSql();
            string sql4 = sqlObj4.Key;
            UtilMethods.RepairReplicationParameters(ref sql4, sqlObj4.Value.ToArray(), 3, "Join");
            queryable.QueryBuilder.Parameters.AddRange(sqlObj4.Value);
            var exp3 = queryable.QueryBuilder.GetExpressionValue(joinExpression3, ResolveExpressType.WhereMultiple);
            queryable.QueryBuilder.JoinQueryInfos.Add(new JoinQueryInfo() { JoinIndex = 1, JoinType = joinType3, JoinWhere = exp3.GetResultString(), TableName = sqlBuilder.GetPackTable(sql4, shortName4) });

            return queryable;
        }
        #endregion

        public virtual ISugarQueryable<T> UnionAll<T>(params ISugarQueryable<T>[] queryables) where T : class, new()
        {
            return _UnionAll(queryables);
        }

        internal ISugarQueryable<T> _UnionAll<T>(ISugarQueryable<T>[] queryables) 
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
                    allItems.Add(new KeyValuePair<string, List<SugarParameter>>(sqlBuilder.GetUnionFomatSql(sql), sqlObj.Value));
                else
                    allItems.Add(new KeyValuePair<string, List<SugarParameter>>(sqlBuilder.GetUnionFomatSql(sql), new List<SugarParameter>()));
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
                item.QueryBuilder.DisableTop = true;
                var sqlObj = item.ToSql();
                string sql = sqlObj.Key;
                UtilMethods.RepairReplicationParameters(ref sql, sqlObj.Value.ToArray(), i, "Union");
                if (sqlObj.Value.HasValue())
                    allItems.Add(new KeyValuePair<string, List<SugarParameter>>(sqlBuilder.GetUnionFomatSql(sql), sqlObj.Value));
                else
                    allItems.Add(new KeyValuePair<string, List<SugarParameter>>(sqlBuilder.GetUnionFomatSql(sql), new List<SugarParameter>()));
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
            var result= this.Context.Queryable<T>().AS(sqlBuilder.GetPackTable(sql, sqlBuilder.GetDefaultShortName())).With(SqlWith.Null).Select(sqlBuilder.GetDefaultShortName() + ".*");
            result.QueryBuilder.IsSqlQuery = true;
            result.QueryBuilder.OldSql = sql;
            result.QueryBuilder.NoCheckInclude = true;
            return result;
        }
        #endregion

        #region Insertable
        public IInsertable<Dictionary<string, object>> InsertableByDynamic(object insertDynamicObject)
        {
            return this.Insertable<Dictionary<string, object>>(insertDynamicObject);
        }
        public InsertMethodInfo InsertableByObject(object singleEntityObjectOrListObject)
        {
            if (singleEntityObjectOrListObject == null)
                return new InsertMethodInfo();
            if (singleEntityObjectOrListObject.GetType().FullName.IsCollectionsList())
            {
                var list = ((IList)singleEntityObjectOrListObject);
                if (list == null || list.Count == 0)
                    return new InsertMethodInfo();
                var type = list[0].GetType();
                var newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
                foreach (var item in list)
                {
                    newList.Add(item);
                }
                var methods = this.Context.GetType().GetMethods()
               .Where(it => it.Name == "Insertable")
               .Where(it => it.GetGenericArguments().Any())
               .Where(it => it.GetParameters().Any(z => z.ParameterType.Name.StartsWith("List")))
               .Where(it => it.Name == "Insertable").ToList();
                var method = methods.Single().MakeGenericMethod(newList.GetType().GetGenericArguments().First());
                InsertMethodInfo result = new InsertMethodInfo()
                {
                    Context = this.Context,
                    MethodInfo = method,
                    objectValue = newList
                };
                return result;
            }
            else
            {
                var methods = this.Context.GetType().GetMethods()
                    .Where(it => it.Name == "Insertable")
                    .Where(it => it.GetGenericArguments().Any())
                    .Where(it => it.GetParameters().Any(z => z.ParameterType.Name == "T"))
                    .Where(it => it.Name == "Insertable").ToList();
                var method = methods.Single().MakeGenericMethod(singleEntityObjectOrListObject.GetType());
                InsertMethodInfo result = new InsertMethodInfo()
                {
                    Context = this.Context,
                    MethodInfo = method,
                    objectValue = singleEntityObjectOrListObject
                };
                return result;
            }
        }
        public virtual IInsertable<T> Insertable<T>(T[] insertObjs) where T : class, new()
        {
            UtilMethods.CheckArray(insertObjs);
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
        public DeleteMethodInfo DeleteableByObject(object singleEntityObjectOrListObject)
        {
            if (singleEntityObjectOrListObject == null)
                return new DeleteMethodInfo();
            if (singleEntityObjectOrListObject.GetType().FullName.IsCollectionsList())
            {
                var list = ((IList)singleEntityObjectOrListObject);
                if (list == null || list.Count == 0)
                    return new DeleteMethodInfo();
                var type = list[0].GetType();
                var newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
                foreach (var item in list)
                {
                    newList.Add(item);
                }
                var methods = this.Context.GetType().GetMethods()
               .Where(it => it.Name == "Deleteable")
               .Where(it => it.GetGenericArguments().Any())
               .Where(it => it.GetParameters().Any(z =>z.Name!= "pkValue" && z.ParameterType.Name.StartsWith("List")))
               .Where(it => it.Name == "Deleteable").ToList();
                var method = methods.FirstOrDefault().MakeGenericMethod(newList.GetType().GetGenericArguments().FirstOrDefault());
                DeleteMethodInfo result = new DeleteMethodInfo()
                {
                    Context = this.Context,
                    MethodInfo = method,
                    objectValue = newList
                };
                return result;
            }
            else
            {
                var methods = this.Context.GetType().GetMethods()
                    .Where(it => it.Name == "Deleteable")
                    .Where(it => it.GetGenericArguments().Any())
                    .Where(it => it.GetParameters().Any(z => z.ParameterType.Name == "T"))
                    .Where(it => it.Name == "Deleteable").ToList();
                var method = methods.Single().MakeGenericMethod(singleEntityObjectOrListObject.GetType());
                DeleteMethodInfo result = new DeleteMethodInfo()
                {
                    Context = this.Context,
                    MethodInfo = method,
                    objectValue = singleEntityObjectOrListObject
                };
                return result;
            }
        }
        public virtual IDeleteable<T> Deleteable<T>() where T : class, new()
        {
            InitMappingInfo<T>();
            DeleteableProvider<T> result = this.CreateDeleteable<T>();
            if (this.Context.CurrentConnectionConfig?.MoreSettings?.IsAutoDeleteQueryFilter == true)
            {
                return result.EnableQueryFilter();
            }
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
        public UpdateMethodInfo UpdateableByObject(object singleEntityObjectOrListObject)
        {
            if (singleEntityObjectOrListObject == null)
                return new UpdateMethodInfo();
            if (singleEntityObjectOrListObject.GetType().FullName.IsCollectionsList())
            {
                var list = ((IList)singleEntityObjectOrListObject);
                if (list == null || list.Count == 0)
                    return new UpdateMethodInfo();
                var type = list[0].GetType();
                var newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
                foreach (var item in list)
                {
                    newList.Add(item);
                }
                var methods = this.Context.GetType().GetMethods()
               .Where(it => it.Name == "Updateable")
               .Where(it => it.GetGenericArguments().Any())
               .Where(it => it.GetParameters().Any(z => z.ParameterType.Name.StartsWith("List")))
               .Where(it => it.Name == "Updateable").ToList();
                var method = methods.Single().MakeGenericMethod(newList.GetType().GetGenericArguments().First());
                UpdateMethodInfo result = new UpdateMethodInfo()
                {
                    Context = this.Context,
                    MethodInfo = method,
                    objectValue = newList
                };
                return result;
            }
            else
            {
                var methods = this.Context.GetType().GetMethods()
                    .Where(it => it.Name == "Updateable")
                    .Where(it => it.GetGenericArguments().Any())
                    .Where(it => it.GetParameters().Any(z => z.ParameterType.Name == "T"))
                    .Where(it => it.Name == "Updateable").ToList();
                var method = methods.Single().MakeGenericMethod(singleEntityObjectOrListObject.GetType());
                UpdateMethodInfo result = new UpdateMethodInfo()
                {
                    Context = this.Context,
                    MethodInfo = method,
                    objectValue = singleEntityObjectOrListObject
                };
                return result;
            }
        }
        public virtual IUpdateable<T> Updateable<T>(T[] UpdateObjs) where T : class, new()
        {
            InitMappingInfo<T>();
            Check.ExceptionEasy(UpdateObjs is IList&&typeof(T).FullName.IsCollectionsList(), "The methods you encapsulate are loaded incorrectly, so List<T> should be Updateable<T>(List<T> UpdateObjs)where T: class, new()", "你封装的方法进错重载，List<T>应该进Updateable<T>(List<T> UpdateObjs)where T : class, new()重载");
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
            var result= (UpdateableProvider<T>)Updateable(UpdateObjs.ToArray());
            result.UpdateBuilder.IsListUpdate = true;
            return result;
        }
        public virtual IUpdateable<T> Updateable<T>(T UpdateObj) where T : class, new()
        {

            return this.Context.Updateable(new T[] { UpdateObj });
        }
        public virtual IUpdateable<T> Updateable<T>() where T : class, new()
        {
            var result = this.Context.Updateable(new T[] { new T() });
            result.UpdateParameterIsNull = true;
            if (this.Context.CurrentConnectionConfig?.MoreSettings?.IsAutoUpdateQueryFilter == true)
            {
                return result.EnableQueryFilter();
            }
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

        public IUpdateable<Dictionary<string, object>> UpdateableByDynamic(object updateDynamicObject) 
        {
            return this.Updateable<Dictionary<string, object>>(updateDynamicObject);
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
        public GridSaveProvider<T> GridSave<T>(List<T> saveList) where T : class, new()
        {
            Check.ExceptionEasy(saveList == null, "saveList is null", "saveList 不能是 null");
            var isTran = this.Context.TempItems != null
                                          && this.Context.TempItems.Any(it => it.Key == "OldData_" + saveList.GetHashCode());
            Check.ExceptionEasy(isTran == false, "saveList no tracking", "saveList 没有使用跟踪");
            var oldList = (List<T>)this.Context.TempItems.FirstOrDefault(it => it.Key == "OldData_" + saveList.GetHashCode()).Value;
            return GridSave(oldList, saveList);
        }
        public GridSaveProvider<T> GridSave<T>(List<T> oldList, List<T> saveList) where T : class, new() 
        {
            GridSaveProvider<T> result = new GridSaveProvider<T>();
            result.Context = this;
            result.OldList = oldList;
            result.SaveList = saveList;
            return result;
        }
        public IStorageable<T> Storageable<T>(T[] dataList) where T : class, new()
        {
            return Storageable(dataList?.ToList());
        }
        public ISaveable<T> Saveable<T>(List<T> saveObjects) where T : class, new()
        {
            return new SaveableProvider<T>(this, saveObjects);
        }
        public ISaveable<T> Saveable<T>(T saveObject) where T : class, new()
        {
            return new SaveableProvider<T>(this, saveObject);
        }
        public StorageableDataTable Storageable(List<Dictionary<string, object>> dictionaryList, string tableName)
        {
            DataTable dt = this.Context.Utilities.DictionaryListToDataTable(dictionaryList);
            dt.TableName = tableName;
            return this.Context.Storageable(dt);
        }
        public StorageableDataTable Storageable(Dictionary<string, object> dictionary, string tableName)
        {
            DataTable dt = this.Context.Utilities.DictionaryListToDataTable(new List<Dictionary<string, object>>() { dictionary });
            dt.TableName = tableName;
            return this.Context.Storageable(dt);
        }
        public IStorageable<T> Storageable<T>(List<T> dataList) where T : class, new()
        {
            dataList = dataList.Where(it => it != null).ToList();
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
        public StorageableDataTable Storageable(DataTable data) 
        {
            var result = new StorageableDataTable();
            Check.Exception(data.TableName.IsNullOrEmpty() || data.TableName == "Table",ErrorMessage.GetThrowMessage( "DataTable data.TableName is null", "参数DataTable没有设置TableName ，参数.TableName=表名"));
            result.DataTable = data;
            result.Context = this;
            data.Columns.Add(new DataColumn("SugarGroupId", typeof(StorageType)));
            data.Columns.Add(new DataColumn("SugarUpdateRows", typeof(List<DataRow>)));
            data.Columns.Add(new DataColumn("SugarErrorMessage", typeof(string)));
            data.Columns.Add(new DataColumn("SugarColumns", typeof(string[])));
            return result;
        }
        public StorageableMethodInfo StorageableByObject(object singleEntityObjectOrList)
        {
            if (singleEntityObjectOrList == null)
                return new StorageableMethodInfo();
            if (singleEntityObjectOrList.GetType().FullName.IsCollectionsList())
            {
                var list = ((IList)singleEntityObjectOrList);
                if(list==null|| list.Count==0)
                    return new StorageableMethodInfo();
                var type=list[0].GetType();
                var newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) ;
                foreach (var item in list)
                {
                    newList.Add(item);
                }
               var methods = this.Context.GetType().GetMethods()
              .Where(it => it.Name == "Storageable")
              .Where(it => it.GetGenericArguments().Any())
              .Where(it => it.GetParameters().Any(z => z.ParameterType.Name.StartsWith("List")))
              .Where(it => it.Name == "Storageable").ToList();
                var method = methods.Single().MakeGenericMethod(newList.GetType().GetGenericArguments().First());
                StorageableMethodInfo result = new StorageableMethodInfo()
                {
                    Context = this.Context,
                    MethodInfo = method,
                    objectValue = newList
                };
                return result;
            }
            else
            {
                var methods = this.Context.GetType().GetMethods()
                    .Where(it => it.Name == "Storageable")
                    .Where(it => it.GetGenericArguments().Any())
                    .Where(it => it.GetParameters().Any(z => z.ParameterType.Name == "T"))
                    .Where(it => it.Name == "Storageable").ToList();
                var method = methods.Single().MakeGenericMethod(singleEntityObjectOrList.GetType());
                StorageableMethodInfo result = new StorageableMethodInfo()
                {
                    Context = this.Context,
                    MethodInfo = method,
                    objectValue = singleEntityObjectOrList
                };
                return result;
            }
        }
        #endregion

        #region Reportable
        public IReportable<T> Reportable<T>(T data)  
        {
            var result = new ReportableProvider<T>(data);
            result.formatBuilder = InstanceFactory.GetInsertBuilder(this.Context.CurrentConnectionConfig);
            result.Context = this;
            result.formatBuilder.Context = this;
            result.queryBuilder = this.Queryable<object>().QueryBuilder;
            return result;
        }
        public IReportable<T> Reportable<T>(List<T> list)  
        {
            var result = new ReportableProvider<T>(list);
            result.formatBuilder = InstanceFactory.GetInsertBuilder(this.Context.CurrentConnectionConfig);
            result.Context = this;
            result.formatBuilder.Context = this;
            result.queryBuilder = this.Queryable<object>().QueryBuilder;
            return result;
        }
        public IReportable<T> Reportable<T>(T [] list)
        {
            if (list == null)
                list = new T[] { };
            var result = new ReportableProvider<T>(list.ToList());
            result.formatBuilder = InstanceFactory.GetInsertBuilder(this.Context.CurrentConnectionConfig);
            result.Context = this;
            result.formatBuilder.Context = this;
            result.queryBuilder = this.Queryable<object>().QueryBuilder;
            return result;
        }
        #endregion

        #region  Nav CUD
        public InsertNavTaskInit<T, T> InsertNav<T>(T data) where T : class, new()
        {
            return InsertNav(new List<T>() { data });
        }
        public InsertNavTaskInit<T, T> InsertNav<T>(List<T> datas) where T : class, new()
        {
            var result = new InsertNavTaskInit<T, T>();
            var provider = new InsertNavProvider<T, T>();
            provider._Roots = datas;
            provider._Context = this;
            result.insertNavProvider = provider;
            result.NavContext = new NavContext() { Items = new List<NavContextItem>() };
            return result;
        }
        public InsertNavTaskInit<T, T> InsertNav<T>(T data, InsertNavRootOptions rootOptions) where T : class, new()
        {
            return InsertNav(new List<T>() { data },rootOptions); ;
        }
        public InsertNavTaskInit<T, T> InsertNav<T>(List<T> datas, InsertNavRootOptions rootOptions) where T : class, new()
        {
            var result = new InsertNavTaskInit<T, T>();
            var provider = new InsertNavProvider<T, T>();
            provider._Roots = datas;
            provider._Context = this;
            provider._RootOptions = rootOptions;
            result.insertNavProvider = provider;
            result.NavContext = new NavContext() { Items = new List<NavContextItem>() };
            return result;
        }
        public DeleteNavTaskInit<T, T> DeleteNav<T>(T data) where T : class, new()
        {
            return DeleteNav(new List<T>() { data });
        }
        public DeleteNavTaskInit<T, T> DeleteNav<T>(List<T> datas) where T : class, new()
        {
            var result = new DeleteNavTaskInit<T, T>();
            result.deleteNavProvider = new DeleteNavProvider<T, T>();
            result.deleteNavProvider._Roots = datas;
            result.deleteNavProvider._Context = this;
            return result;
        }
        public DeleteNavTaskInit<T, T> DeleteNav<T>(Expression<Func<T, bool>> whereExpression) where T : class, new() 
        {
            return DeleteNav(this.Queryable<T>().Where(whereExpression).ToList());
        }

        public DeleteNavTaskInit<T, T> DeleteNav<T>(T data, DeleteNavRootOptions options) where T : class, new()
        {
            return DeleteNav(new List<T>() { data }, options);
        }
        public DeleteNavTaskInit<T, T> DeleteNav<T>(List<T> datas, DeleteNavRootOptions options) where T : class, new()
        {
            var result = new DeleteNavTaskInit<T, T>();
            result.deleteNavProvider = new DeleteNavProvider<T, T>();
            result.deleteNavProvider._Roots = datas;
            result.deleteNavProvider._Context = this;
            result.deleteNavProvider._RootOptions = options;
            return result;
        }
        public DeleteNavTaskInit<T, T> DeleteNav<T>(Expression<Func<T, bool>> whereExpression, DeleteNavRootOptions options) where T : class, new()
        {
            return DeleteNav(this.Queryable<T>().Where(whereExpression).ToList(),options);
        }

        public UpdateNavTaskInit<T, T> UpdateNav<T>(T data) where T : class, new()
        {
            return UpdateNav(new List<T>() { data });
        }
        public UpdateNavTaskInit<T, T> UpdateNav<T>(List<T> datas) where T : class, new()
        {
            var result = new UpdateNavTaskInit<T, T>();
            var provider = new UpdateNavProvider<T, T>();
            provider._Roots = datas;
            provider._Context = this;
            result.UpdateNavProvider = provider;
            result.NavContext = new NavContext() { Items = new List<NavContextItem>() { } };
            return result;
        }
        public UpdateNavTaskInit<T, T> UpdateNav<T>(T data, UpdateNavRootOptions rootOptions) where T : class, new()
        {
            return UpdateNav(new List<T>() { data},rootOptions);
        }
        public UpdateNavTaskInit<T, T> UpdateNav<T>(List<T> datas, UpdateNavRootOptions rootOptions) where T : class, new()
        {
            var result = new UpdateNavTaskInit<T, T>();
            var provider = new UpdateNavProvider<T, T>();
            provider._Roots = datas;
            provider._RootOptions = rootOptions;
            provider._Context = this;
            result.UpdateNavProvider = provider;
            result.NavContext = new NavContext() { Items = new List<NavContextItem>() { } };
            return result; ;
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
        public T CreateContext<T>(bool isTran) where T : SugarUnitOfWork, new() 
        {
            Check.ExceptionEasy(" var childDb=Db.GetConnection(configId);  use Db.CreateContext ", " 例如 var childDb=Db.GetConnection(configId);其中Db才能使用CreateContext，childDb不能使用");
            return null;
        }
        public SugarUnitOfWork CreateContext(bool isTran = true)
        {
            Check.ExceptionEasy(" var childDb=Db.GetConnection(configId);  use Db.CreateContext ", " 例如 var childDb=Db.GetConnection(configId);其中Db才能使用CreateContext，childDb不能使用");
            return null;
        }
        //[Obsolete("Use SqlSugarClient.GetSimpleClient() Or SqlSugarClient.GetSimpleClient<T>() ")]
        //public virtual SimpleClient SimpleClient
        //{
        //    get
        //    {
        //        if (this._SimpleClient == null)
        //            this._SimpleClient = new SimpleClient(this);
        //        return this._SimpleClient;
        //    }
        //}
        public virtual SimpleClient<T> GetSimpleClient<T>() where T : class, new()
        {
            return new SimpleClient<T>(this);
        }

        public RepositoryType GetRepository<RepositoryType>() where RepositoryType : ISugarRepository, new()
        {
            Type type = typeof(RepositoryType);
            var isAnyParamter = type.GetConstructors().Any(z => z.GetParameters().Any());
            object o = null;
            if (isAnyParamter)
            {
                o = Activator.CreateInstance(type, new string[] { null });
            }
            else
            {
                o = Activator.CreateInstance(type);
            }
            var result = (RepositoryType)o;
            if (result.Context == null)
            {
                result.Context = this.Context;
            }
            return result;
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
            var pars = this.Context.Ado.GetParameters(parsmeters);
            if (pars != null)
            {
                foreach (var par in pars)
                {
                    if (par.ParameterName.StartsWith(":"))
                    {
                        par.ParameterName = ("@" + par.ParameterName.Trim(':'));
                    }
                }
            }
            this.Queues.Add(sql, pars);
            
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
                //if (this.CurrentConnectionConfig.DbType == DbType.Oracle)
                //{
                //    throw new Exception("Oracle no support SaveQueues");
                //}
                if (this.Queues == null || this.Queues.Count == 0) return default(T);
                isTran = isTran && this.Ado.Transaction == null;
                if (isTran) this.Ado.BeginTran();
                StringBuilder sqlBuilder = new StringBuilder();
                var parsmeters = new List<SugarParameter>();
                var index = 1;
                if (this.Queues.HasValue())
                {
                    var repeatList =
                        Queues.SelectMany(it => it.Parameters ?? new SugarParameter[] { }).Select(it => it.ParameterName)
                       .GroupBy(it => it?.ToLower())
                       .Where(it => it.Count() > 1);
                    var repeatCount = repeatList.Count();
                    var isParameterNameRepeat = repeatList
                        .Count() > 0;
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
                            if (repeatCount>500||(isParameterNameRepeat&& repeatList.Any(it=>it.Key.EqualCase(itemParameter.ParameterName))))
                            {
                                itemSql = UtilMethods.ReplaceSqlParameter(itemSql, itemParameter, newName);
                                addParameters.Add(parameter);
                            }
                            else
                            {
                                parameter.ParameterName = itemParameter.ParameterName;
                                addParameters.Add(parameter);
                            }
                        }
                        parsmeters.AddRange(addParameters);
                        itemSql = itemSql
                           .TrimEnd('\r')
                           .TrimEnd('\n')
                           .TrimEnd('\r')
                           .TrimEnd('\n')
                           .TrimEnd(';') + ";";
                        if (itemSql == "begin;")
                        {
                            itemSql = itemSql.TrimEnd(';') + "\n";
                        }
                        sqlBuilder.AppendLine(itemSql);
                        index++;
                    }
                }
                this.Queues.Clear();
                var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
                builder.FormatSaveQueueSql(sqlBuilder);
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
                //if (this.CurrentConnectionConfig.DbType == DbType.Oracle)
                //{
                //    throw new Exception("Oracle no support SaveQueues");
                //}
                if (this.Queues == null || this.Queues.Count == 0) return default(T);
                isTran = isTran && this.Ado.Transaction == null;
                if (isTran) this.Ado.BeginTran();
                StringBuilder sqlBuilder = new StringBuilder();
                var parsmeters = new List<SugarParameter>();
                var index = 1;
                if (this.Queues.HasValue())
                {
                    var repeatList =
                         Queues.SelectMany(it => it.Parameters ?? new SugarParameter[] { }).Select(it => it.ParameterName)
                        .GroupBy(it => it?.ToLower())
                        .Where(it => it.Count() > 1);
                    var repeatCount = repeatList.Count();
                    var isParameterNameRepeat = repeatList
                        .Count() > 0;
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
                            if (repeatCount>500||(isParameterNameRepeat&& repeatList.Any(it=>it.Key.EqualCase(itemParameter.ParameterName))))
                            {
                                itemSql = UtilMethods.ReplaceSqlParameter(itemSql, itemParameter, newName);
                            }
                            else 
                            {
                                parameter.ParameterName = itemParameter.ParameterName;
                            }
                            addParameters.Add(parameter);
                        }
                        parsmeters.AddRange(addParameters);
                        itemSql = itemSql
                            .TrimEnd('\r')
                            .TrimEnd('\n')
                            .TrimEnd('\r')
                            .TrimEnd('\n')
                            .TrimEnd(';') + ";";
                        if (itemSql == "begin;"   ) 
                        {
                            itemSql = itemSql.TrimEnd(';')+"\n";
                        }
                        sqlBuilder.AppendLine(itemSql);
                        index++;
                    }
                }
                this.Queues.Clear();
                var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
                builder.FormatSaveQueueSql(sqlBuilder);
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

        #region Cache
        public SugarCacheProvider DataCache 
        { 
            get {
                var services=this.CurrentConnectionConfig.ConfigureExternalServices;
                if (services == null)
                    return new SugarCacheProvider();
                if (services.DataInfoCacheService == null)
                    return new SugarCacheProvider();
                SugarCacheProvider cache = new SugarCacheProvider();
                cache.Servie=services.DataInfoCacheService;
                return cache;
            }
        }
        #endregion

        #region Split table
        public SplitTableContext SplitHelper<T>() where T : class, new()
        {
            UtilMethods.StartCustomSplitTable(this, typeof(T));
            var result = new SplitTableContext(this.Context)
            {
                EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>()
            };
            return result;
        }
        public SplitTableContext SplitHelper(Type entityType)  
        {
            UtilMethods.StartCustomSplitTable(this,entityType);
            var result = new SplitTableContext(this.Context)
            {
                EntityInfo = this.Context.EntityMaintenance.GetEntityInfo(entityType)
            };
            return result;
        }
        public SplitTableContextResult<T> SplitHelper<T>(T data) where T : class, new()
        {
            UtilMethods.StartCustomSplitTable(this, typeof(T));
            var result = new SplitTableContext(this.Context)
            {
                EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>()
            };
            return new SplitTableContextResult<T>()
            {
                Items = new List<T> { data },
                Helper = result
            };
        }
        public SplitTableContextResult<T> SplitHelper<T>(List<T> data) where T : class, new()
        {
            UtilMethods.StartCustomSplitTable(this, typeof(T));
            var result = new SplitTableContext(this.Context)
            {
                EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>()
            };
            return new SplitTableContextResult<T>()
            {
                Items = data,
                Helper = result
            };
        }
        #endregion

        #region AsTenant
        public ITenant AsTenant()
        {
            if (this.Root != null)
            {
                return this.Root;
            }
            else
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage("Child objects do not support tenant methods, var childDb= Db.GetConnection(confid)  ,Db is master  ", "Db子对象不支持租户方法，请使用主对象,例如：var childDb= Db.GetConnection(confid)  Db是主对象，childDb是子对象 "));
                return null;
            }
        }

        #endregion

        #region Fastest
        public IFastest<T> Fastest<T>() where T:class,new()
        {
            return new FastestProvider<T>(this);
        }
        #endregion

        #region Other
        public Task<SugarAsyncLock> AsyncLock(int timeOutSeconds = 30)
        {
            var result = new SugarAsyncLock(this);
            return result.AsyncLock(timeOutSeconds);
        }
        public DynamicBuilder DynamicBuilder() 
        {
            return new DynamicBuilder(this.Context);
        }
        public void Tracking<T>(T data) where T : class, new()
        {
            if (data != null)
            {
                UtilMethods.IsNullReturnNew(this.TempItems);
                var key = "Tracking_" + data.GetHashCode() + "";
                if (!this.TempItems.ContainsKey(key))
                {
                    var newT = new T();
                    FastCopy.Copy(data, newT);
                    this.TempItems.Add(key, newT);
                }
            }
        }
        public void ClearTracking() 
        {
            if (this.Context.TempItems != null)
            {
                var removeKeys = this.Context.TempItems.Where(it => it.Key.StartsWith("Tracking_") || it.Key.StartsWith("OldData_")).Select(it => it.Key).ToList();
                foreach (string key in removeKeys)
                {
                    this.Context.TempItems.Remove(key);
                } 
            }
        }
        public void Tracking<T>(List<T> datas) where T : class, new()
        {
            foreach (var data in datas) 
            {
                this.Tracking(data);
            } 
            if (datas != null)
            {
                Check.ExceptionEasy(this.Context.TempItems.ContainsKey("OldData_" + datas.GetHashCode()), "The object already has a trace", "对象已存在跟踪,如果要在跟踪可以先清除 db.ClearTracking() ");
                this.Context.TempItems.Add("OldData_" + datas.GetHashCode(), datas.Cast<T>().ToList());
            }
        }
        public SqlSugarClient CopyNew()
        {
            var result= new SqlSugarClient(UtilMethods.CopyConfig(this.Ado.Context.CurrentConnectionConfig));
            result.QueryFilter = this.QueryFilter;
            return result;
        }
        public void ThenMapper<T>(IEnumerable<T> list, Action<T> action)
        {
            this.Context.Utilities.PageEach(list, 200, pageList =>
            {
                _ThenMapper(pageList, action);
            });
        }

        public async Task ThenMapperAsync<T>(IEnumerable<T> list, Func<T, Task> action)
        {
            await this.Context.Utilities.PageEachAsync(list, 200,async pageList =>
            {
                await _ThenMapperAsync(pageList, action);
            });
        }
        #endregion
    }
}
