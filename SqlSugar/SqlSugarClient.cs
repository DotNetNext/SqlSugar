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
        #region constructor
        /// <summary>
        /// If you have system table permissions, use this
        /// </summary>
        /// <param name="config"></param>
        public SqlSugarClient(SystemTablesConfig config)
        {
            base.CurrentConnectionConfig = config;
            base.InitConstructor();
        }
        /// <summary>
        /// If you do not have system table permissions, use this
        /// </summary>
        /// <param name="config"></param>
        public SqlSugarClient(AttrbuitesCofnig config)
        {
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

        #region properties
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

        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>() where T : class, new()
        {
            var reval = InstanceFactory.GetQueryable<T>(base.CurrentConnectionConfig);
            reval.Context = this;
            var sqlBuilder = InstanceFactory.GetSqlbuilder(base.CurrentConnectionConfig); ;
            reval.SqlBuilder = sqlBuilder;
            reval.SqlBuilder.LambadaQueryBuilder = InstanceFactory.GetLambadaQueryBuilder(base.CurrentConnectionConfig);
            reval.SqlBuilder.LambadaQueryBuilder.Builder = sqlBuilder;
            reval.SqlBuilder.Context = reval.SqlBuilder.LambadaQueryBuilder.Context = this;
            reval.SqlBuilder.LambadaQueryBuilder.EntityName = typeof(T).Name;
            reval.SqlBuilder.LambadaQueryBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(base.CurrentConnectionConfig);
            return reval;
        }

        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<SugarDynamic> Queryable(string tableName,string shortName,string widthString=null)
        {
            var queryable = Queryable<SugarDynamic>();
            queryable.SqlBuilder.LambadaQueryBuilder.EntityName = tableName;
            queryable.SqlBuilder.LambadaQueryBuilder.TableShortName = shortName;
            queryable.SqlBuilder.LambadaQueryBuilder.TableWithString = widthString;
            return queryable;
        }

        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>(string shortName, string widthString = null) where T : class, new()
        {
            var queryable = Queryable<T>();
            queryable.SqlBuilder.LambadaQueryBuilder.TableShortName = shortName;
            queryable.SqlBuilder.LambadaQueryBuilder.TableWithString = widthString;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.LambadaQueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, this,ref shortName, typeof(T2));
            queryable.SqlBuilder.LambadaQueryBuilder.TableShortName=shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.LambadaQueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, this, ref shortName, typeof(T2),typeof(T3));
            queryable.SqlBuilder.LambadaQueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.LambadaQueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, this, ref shortName, typeof(T2), typeof(T3),typeof(T4));
            queryable.SqlBuilder.LambadaQueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.LambadaQueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, this, ref shortName, typeof(T2), typeof(T3), typeof(T4),typeof(T5));
            queryable.SqlBuilder.LambadaQueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.LambadaQueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, this, ref shortName, typeof(T2), typeof(T3), typeof(T4), typeof(T5),typeof(T6));
            queryable.SqlBuilder.LambadaQueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.LambadaQueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, this, ref shortName, typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),typeof(T7));
            queryable.SqlBuilder.LambadaQueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.LambadaQueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, this, ref shortName, typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7),typeof(T8));
            queryable.SqlBuilder.LambadaQueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.LambadaQueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, this, ref shortName, typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),typeof(T9));
            queryable.SqlBuilder.LambadaQueryBuilder.TableShortName = shortName;
            return queryable;
        }
        public virtual ISugarQueryable<T> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T10, object[]>> joinExpression) where T : class, new()
        {
            var queryable = Queryable<T>();
            string shortName = string.Empty;
            queryable.SqlBuilder.LambadaQueryBuilder.JoinQueryInfos = base.GetJoinInfos(joinExpression, this, ref shortName, typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9),typeof(T10));
            queryable.SqlBuilder.LambadaQueryBuilder.TableShortName = shortName;
            return queryable;
        }
        #endregion

        #region functions
        public virtual List<T> SqlQuery<T>(string sql, object pars = null)
        {
            var dbPars = this.Database.GetParameters(pars);
            var builder = InstanceFactory.GetSqlbuilder(base.CurrentConnectionConfig);
            builder.SqlQueryBuilder.Sql.Append(sql);
            using (var dataReader = this.Database.GetDataReader(sql, dbPars))
            {
                var reval = this.Database.DbBind.DataReaderToList<T>(typeof(T), dataReader, builder.SqlQueryBuilder.Fields);
                builder.SqlQueryBuilder.Clear();
                return reval;
            }
        }
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
