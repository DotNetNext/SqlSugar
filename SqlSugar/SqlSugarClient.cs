using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Sql string processing
        /// </summary>
        public virtual ISqlBuilder SqlBuilder
        {
            get
            {
                if (_SqlBuilder == null)
                {
                    var reval = InstanceFactory.GetSqlbuilder(base.CurrentConnectionConfig);
                    _SqlBuilder = reval;
                    _SqlBuilder.Context = this;
                    return reval;
                }
                return _SqlBuilder;
            }
        }
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
        /// Lambda Expressions operation
        /// </summary>
        public virtual ILambdaExpressions LambdaExpressions
        {
            get
            {
                if (_LambdaExpressions == null)
                {
                    var reval = InstanceFactory.GetLambdaExpressions(base.CurrentConnectionConfig);
                    reval.Context = this;
                    _LambdaExpressions = reval;
                    return reval;
                }
                return _LambdaExpressions;
            }
        }
        /// <summary>
        /// Lambda Query operation
        /// </summary>
        public virtual ISugarQueryable<T> Queryable<T>() where T : class, new()
        {
            if (_Queryable == null)
            {
                var reval = InstanceFactory.GetQueryable<T>(base.CurrentConnectionConfig);
                reval.Context = this;
                var sqlBuilder = reval.Context.SqlBuilder;
                sqlBuilder.LambadaQueryBuilder = InstanceFactory.GetLambadaQueryBuilder(base.CurrentConnectionConfig);
                sqlBuilder.LambadaQueryBuilder.Conext = this;
                sqlBuilder.LambadaQueryBuilder.EntityType = typeof(T);
                _Queryable = reval;
                return reval;
            }
            return (ISugarQueryable<T>)_Queryable;
        }
        /// <summary>
        /// Sqlable Query operation
        /// </summary>
        public virtual ISugarSqlable Sqlable
        {
            get
            {
                if (_Sqlable == null)
                {
                    var reval = InstanceFactory.GetSqlable(base.CurrentConnectionConfig);
                    reval.Context = this;
                    _Sqlable = reval;
                    return reval;
                }
                return (ISugarSqlable)_Sqlable;
            }
        }
        #endregion

        #region functions
        public virtual List<T> SqlQuery<T>(string sql, object pars = null)
        {
            var dbPars = this.Database.GetParameters(pars);
            this.SqlBuilder.SqlQueryBuilder.Sql.Append(sql);
            using (var dataReader = this.Database.GetDataReader(sql, dbPars))
            {
                var reval = this.Database.DbBind.DataReaderToList<T>(typeof(T), dataReader, this.SqlBuilder.SqlQueryBuilder.Fields);
                this.SqlBuilder.SqlQueryBuilder.Clear();
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
