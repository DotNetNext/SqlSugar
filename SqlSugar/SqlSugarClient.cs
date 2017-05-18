using System;
using System.Collections.Generic;
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
    /// ** qq:610262374
    /// </summary>
    public partial class SqlSugarClient : SqlSugarAccessory, IDisposable
    {
        #region Properties
        public bool IsSystemTablesConfig
        {
            get
            {
                return this.CurrentConnectionConfig is SystemTablesConfig;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// If you have system table permissions, use this
        /// </summary>
        /// <param name="config"></param>
        public SqlSugarClient(SystemTablesConfig config)
        {
            base.Context = this;
            base.CurrentConnectionConfig = config;
            base.InitConstructor();
        }
        /// <summary>
        /// If you do not have system table permissions, use this
        /// </summary>
        /// <param name="config"></param>
        public SqlSugarClient(AttrbuitesCofnig config)
        {
            base.Context = this;
            base.CurrentConnectionConfig = config;
            Check.ArgumentNullException(config.EntityNamespace, ErrorMessage.EntityNamespaceError);
            base.EntityNamespace = config.EntityNamespace;
            base.InitConstructor();
        }
        /// <summary>
        /// Read / write mode. If you have system table permissions, use this
        /// </summary>
        /// <param name="masterConnectionConfig"></param>
        /// <param name="slaveConnectionConfigs"></param>
        public SqlSugarClient(SystemTablesConfig masterConnectionConfig, IConnectionConfig[] slaveConnectionConfigs)
        {
            base.Context = this;
            base.CurrentConnectionConfig = masterConnectionConfig;
            base.InitConstructor();
            if (slaveConnectionConfigs.IsNullOrEmpty()) return;

            var db = this.Database;
            db.MasterConnectionConfig = masterConnectionConfig;
            db.SlaveConnectionConfigs = slaveConnectionConfigs.ToList();
        }
        /// <summary>
        /// Read / write mode. If you do not have system table permissions, use this
        /// </summary>
        /// <param name="masterConnectionConfig"></param>
        /// <param name="slaveConnectionConfigs"></param>
        public SqlSugarClient(AttrbuitesCofnig masterConnectionConfig, IConnectionConfig[] slaveConnectionConfigs)
        {
            base.Context = this;
            base.CurrentConnectionConfig = masterConnectionConfig;
            base.InitConstructor();
            if (slaveConnectionConfigs.IsNullOrEmpty()) return;

            var db = this.Database;
            Check.ArgumentNullException(masterConnectionConfig.EntityNamespace, ErrorMessage.EntityNamespaceError);
            base.EntityNamespace = masterConnectionConfig.EntityNamespace;
            db.MasterConnectionConfig = masterConnectionConfig;
            db.SlaveConnectionConfigs = slaveConnectionConfigs.ToList();
        }

        #endregion

        #region  ADO Method
        /// <summary>
        ///Database operation
        /// </summary>
        public virtual IDb Database
        {
            get
            {
                if (_Ado == null)
                {
                    var reval = InstanceFactory.GetDb(base.CurrentConnectionConfig);
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
            var reval = InstanceFactory.GetQueryable<T>(base.CurrentConnectionConfig);
            reval.Context = this;
            var sqlBuilder = InstanceFactory.GetSqlbuilder(base.CurrentConnectionConfig); ;
            reval.SqlBuilder = sqlBuilder;
            reval.SqlBuilder.QueryBuilder = InstanceFactory.GetQueryBuilder(base.CurrentConnectionConfig);
            reval.SqlBuilder.QueryBuilder.Builder = sqlBuilder;
            reval.SqlBuilder.Context = reval.SqlBuilder.QueryBuilder.Context = this;
            reval.SqlBuilder.QueryBuilder.EntityName = typeof(T).Name;
            reval.SqlBuilder.QueryBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(base.CurrentConnectionConfig);
            return reval;
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
        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>(string shortName) where T : class, new()
        {
            var queryable = Queryable<T>();
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, ref shortName, typeof(T2));
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, ref shortName, typeof(T2), typeof(T3));
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, ref shortName, typeof(T2), typeof(T3), typeof(T4));
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, ref shortName, typeof(T2), typeof(T3), typeof(T4), typeof(T5));
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, ref shortName, typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, ref shortName, typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, ref shortName, typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, ref shortName, typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9));
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T10, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, ref shortName, typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10));
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            return queryable;
        }

        #endregion

        #region Insertable
        public virtual IInsertable<T> Insertable<T>(T[] insertObjs) where T : class, new()
        {
            var reval = new InsertableProvider<T>();
            var sqlBuilder = InstanceFactory.GetSqlbuilder(base.CurrentConnectionConfig); ;
            reval.Context = this;
            reval.EntityInfo = this.EntityProvider.GetEntityInfo<T>();
            reval.SqlBuilder = sqlBuilder;
            reval.InsertObjs = insertObjs;
            sqlBuilder.InsertBuilder = reval.InsertBuilder = InstanceFactory.GetInsertBuilder(base.CurrentConnectionConfig);
            sqlBuilder.InsertBuilder.Builder = sqlBuilder;
            sqlBuilder.InsertBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(base.CurrentConnectionConfig);
            sqlBuilder.Context = reval.SqlBuilder.InsertBuilder.Context = this;
            reval.Init();
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
            var reval = new DeleteableProvider<T>();
            var sqlBuilder = InstanceFactory.GetSqlbuilder(base.CurrentConnectionConfig); ;
            reval.Context = this;
            reval.SqlBuilder = sqlBuilder;
            sqlBuilder.DeleteBuilder = reval.DeleteBuilder = InstanceFactory.GetDeleteBuilder(base.CurrentConnectionConfig);
            sqlBuilder.DeleteBuilder.Builder = sqlBuilder;
            sqlBuilder.DeleteBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(base.CurrentConnectionConfig);
            sqlBuilder.Context = reval.SqlBuilder.DeleteBuilder.Context = this;
            return reval;
        }
        #endregion

        #region Updateable
        public virtual IUpdateable<T> Updateable<T>(T[] UpdateObjs) where T : class, new()
        {
            var reval = new UpdateableProvider<T>();
            var sqlBuilder = InstanceFactory.GetSqlbuilder(base.CurrentConnectionConfig); ;
            reval.Context = this;
            reval.EntityInfo = this.EntityProvider.GetEntityInfo<T>();
            reval.SqlBuilder = sqlBuilder;
            reval.UpdateObjs = UpdateObjs;
            sqlBuilder.UpdateBuilder = reval.UpdateBuilder = InstanceFactory.GetUpdateBuilder(base.CurrentConnectionConfig);
            sqlBuilder.UpdateBuilder.Builder = sqlBuilder;
            sqlBuilder.UpdateBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(base.CurrentConnectionConfig);
            sqlBuilder.Context = reval.SqlBuilder.UpdateBuilder.Context = this;
            reval.Init();
            return reval;
        }
        public virtual IUpdateable<T> Updateable<T>(T UpdateObj) where T : class, new()
        {
            return this.Updateable(new T[] { UpdateObj });
        }
        #endregion

        #region SqlQuery
        public virtual List<T> SqlQuery<T>(string sql, object whereObj = null)
        {
            var parameters = this.Database.GetParameters(whereObj);
            var builder = InstanceFactory.GetSqlbuilder(base.CurrentConnectionConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parameters != null && parameters.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parameters);
            using (var dataReader = this.Database.GetDataReader(builder.SqlQueryBuilder.ToSqlString(), builder.SqlQueryBuilder.Parameters.ToArray()))
            {
                var reval = this.Database.DbBind.DataReaderToList<T>(typeof(T), dataReader, builder.SqlQueryBuilder.Fields);
                if (this.CurrentConnectionConfig.IsAutoCloseConnection) this.Close();
                builder.SqlQueryBuilder.Clear();
                return reval;
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

        #region Dispose OR Close
        public virtual void Close()
        {
            if (this.Database != null)
            {
                this.Database.Close();
            }
        }
        public virtual void Dispose()
        {
            if (this.Database != null)
            {
                this.Database.Dispose();
            }
        }
        #endregion
    }
}
