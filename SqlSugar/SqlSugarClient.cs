using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    ///<summary>
    /// ** description：Create database access object
    /// ** author：sunkaixuan
    /// ** date：2017/1/2
    /// ** email:610262374@qq.com
    /// </summary>
    public partial class SqlSugarClient : SqlSugarAccessory, IDisposable
    {
        #region Properties
        public bool IsSystemTablesConfig
        {
            get
            {
                return this.CurrentConnectionConfig.InitKeyType == InitKeyType.SystemTable;
            }
        }
        #endregion

        #region Constructor
        public SqlSugarClient(ConnectionConfig config)
        {
            base.Context = this;
            base.CurrentConnectionConfig = config;
        }

        /// <summary>
        /// Read / write mode
        /// </summary>
        /// <param name="masterConnectionConfig"></param>
        /// <param name="slaveConnectionConfigs"></param>
        public SqlSugarClient(ConnectionConfig masterConnectionConfig, ConnectionConfig[] slaveConnectionConfigs)
        {
            base.Context = this;
            base.CurrentConnectionConfig = masterConnectionConfig;
            if (slaveConnectionConfigs.IsNullOrEmpty()) return;

            var db = this.Ado;
            db.MasterConnectionConfig = masterConnectionConfig;
            db.SlaveConnectionConfigs = slaveConnectionConfigs.ToList();
        }
        #endregion

        #region  ADO Methods
        /// <summary>
        ///Database operation
        /// </summary>
        public virtual IAdo Ado
        {
            get
            {
                if (_Ado == null)
                {
                    var reval = InstanceFactory.GetAdo(base.CurrentConnectionConfig);
                    Check.ConnectionConfig(base.CurrentConnectionConfig);
                    _Ado = reval;
                    reval.Context = this;
                    return reval;
                }
                return _Ado;
            }
        }
        #endregion

        #region Rewritable Methods
        /// <summary>
        /// Rewritable Methods
        /// </summary>
        public virtual IRewritableMethods RewritableMethods
        {
            get
            {
                if (base._RewritableMethods == null)
                {
                    base._RewritableMethods = new RewritableMethods();
                }
                return _RewritableMethods;
            }
            set
            {
                base._RewritableMethods = value;
            }
        }
        #endregion

        #region Queryable
        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>() where T : class, new()
        {
            InitMppingInfo<T>();
            var result = base.CreateQueryable<T>();
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
        public virtual ISugarQueryable<SugarDynamic> Queryable(string tableName, string shortName)
        {
            var queryable = Queryable<SugarDynamic>();
            queryable.SqlBuilder.QueryBuilder.EntityName = tableName;
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2>();
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3>();
            var types = new Type[] { typeof(T2), typeof(T3) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T8>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        #endregion

        #region Insertable
        public virtual IInsertable<T> Insertable<T>(T[] insertObjs) where T : class, new()
        {
            InitMppingInfo<T>();
            InsertableProvider<T> reval = base.CreateInsertable(insertObjs);
            return reval;
        }
        public virtual IInsertable<T> Insertable<T>(List<T> insertObjs) where T : class, new()
        {
            Check.ArgumentNullException(insertObjs, "Insertable.insertObjs can't be null");
            return this.Insertable(insertObjs.ToArray());
        }
        public virtual IInsertable<T> Insertable<T>(T insertObj) where T : class, new()
        {
            return this.Insertable(new T[] { insertObj });
        }
        #endregion

        #region Deleteable
        public virtual IDeleteable<T> Deleteable<T>() where T : class, new()
        {
            InitMppingInfo<T>();
            DeleteableProvider<T> reval = base.CreateDeleteable<T>();
            return reval;
        }
        #endregion

        #region Updateable
        public virtual IUpdateable<T> Updateable<T>(T[] UpdateObjs) where T : class, new()
        {
            InitMppingInfo<T>();
            UpdateableProvider<T> reval = base.CreateUpdateable(UpdateObjs);
            return reval;
        }
        public virtual IUpdateable<T> Updateable<T>(T UpdateObj) where T : class, new()
        {
            return this.Updateable(new T[] { UpdateObj });
        }
        public virtual IUpdateable<T> Updateable<T>() where T : class, new()
        {
            return this.Updateable(new T[] { new T() });
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

        #region DbMaintenance
        public virtual IDbMaintenance DbMaintenance
        {
            get
            {
                if (base._DbMaintenance == null)
                {
                    IDbMaintenance maintenance = InstanceFactory.GetDbMaintenance(this.Context.CurrentConnectionConfig);
                    base._DbMaintenance = maintenance;
                    maintenance.Context = this.Context;
                }
                return base._DbMaintenance;
            }
        }
        #endregion

        #region Entity Methods
        public virtual EntityProvider EntityProvider
        {
            get
            {
                if (base._EntityProvider == null)
                {
                    base._EntityProvider = new EntityProvider();
                    base._EntityProvider.Context = this;
                }
                return _EntityProvider;
            }
            set
            {
                base._EntityProvider = value;
            }
        }
        #endregion

        #region Gobal Filter
        public QueryFilterProvider QueryFilter
        {
            get
            {
                if (base._QueryFilterProvider == null)
                {
                    base._QueryFilterProvider = new QueryFilterProvider();
                    base._QueryFilterProvider.Context = this;
                }
                return _QueryFilterProvider;
            }
            set
            {
                base._QueryFilterProvider = value;
            }
        }
        #endregion

        #region Dispose OR Close
        public virtual void Close()
        {
            if (this.Ado != null)
            {
                this.Ado.Close();
            }
        }
        public virtual void Dispose()
        {
            if (this.Ado != null)
            {
                this.Ado.Dispose();
            }
        }
        #endregion
    }
}
