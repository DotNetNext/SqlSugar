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
    /// ** description：Create database access object
    /// ** author：sunkaixuan
    /// ** date：2017/1/2
    /// ** email:610262374@qq.com
    /// </summary>
    public partial class SqlSugarClient : SqlSugarAccessory, IDisposable
    {

        #region Constructor
        public SqlSugarClient(ConnectionConfig config)
        {
            base.Context = this;
            base.CurrentConnectionConfig = config;
            base.ContextID = Guid.NewGuid();
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
                default:
                    throw new Exception("ConnectionConfig.DbType is null");
            }
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
                if (base.Context._Ado == null)
                {
                    var reval = InstanceFactory.GetAdo(base.Context.CurrentConnectionConfig);
                    base.Context._Ado = reval;
                    reval.Context = base.Context;
                    return reval;
                }
                return base.Context._Ado;
            }
        }
        #endregion

        #region Aop Log Methods
        public virtual AopProvider Aop { get { return new AopProvider(base.Context); } }
        #endregion

        #region Util Methods
        [Obsolete("Use SqlSugarClient.Utilities")]
        public virtual IContextMethods RewritableMethods
        {
            get { return base.Context.Utilities; }
            set { base.Context.Utilities = value; }
        }
        public virtual IContextMethods Utilities
        {
            get
            {
                if (base.Context._RewritableMethods == null)
                {
                    base.Context._RewritableMethods = new ContextMethods();
                    base.Context._RewritableMethods.Context = base.Context;
                }
                return base.Context._RewritableMethods;
            }
            set { base.Context._RewritableMethods = value; }
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
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        #region  9-12
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object[]>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(base.CurrentConnectionConfig);
            base.CreateQueryJoin(joinExpression, types, queryable);
            return queryable;
        }
        #endregion
        public virtual ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2>();
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2>(base.CurrentConnectionConfig);
            base.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3>();
            var types = new Type[] { typeof(T2), typeof(T3) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3>(base.CurrentConnectionConfig);
            base.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4>(base.CurrentConnectionConfig);
            base.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(base.CurrentConnectionConfig);
            base.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(base.CurrentConnectionConfig);
            base.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(base.CurrentConnectionConfig);
            base.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T8>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(base.CurrentConnectionConfig);
            base.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }

        #region 9-12
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T8, T9>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(base.CurrentConnectionConfig);
            base.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T8, T9, T10>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(base.CurrentConnectionConfig);
            base.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T8, T9, T10, T11>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(base.CurrentConnectionConfig);
            base.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression) where T : class, new()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T8, T9, T10, T11, T12>();
            var types = new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) };
            var queryable = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(base.CurrentConnectionConfig);
            base.CreateEasyQueryJoin(joinExpression, types, queryable);
            queryable.Where(joinExpression);
            return queryable;
        }
        #endregion

        public virtual ISugarQueryable<T> UnionAll<T>(params ISugarQueryable<T>[] queryables) where T : class, new()
        {
            var sqlBuilder = InstanceFactory.GetSqlbuilder(base.Context.CurrentConnectionConfig);
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
            var resulut = base.Context.Queryable<ExpandoObject>().AS(UtilMethods.GetPackTable(allSql, "unionTable"));
            resulut.AddParameters(allParameters);
            return resulut.Select<T>("*");
        }
        public virtual ISugarQueryable<T> UnionAll<T>(List<ISugarQueryable<T>> queryables) where T : class, new()
        {
            Check.Exception(queryables.IsNullOrEmpty(), "UnionAll.queryables is null ");
            return UnionAll(queryables.ToArray());
        }
        #endregion

        #region SqlQueryable
        public ISugarQueryable<T> SqlQueryable<T>(string sql) where T : class, new()
        {
            var sqlBuilder = InstanceFactory.GetSqlbuilder(base.Context.CurrentConnectionConfig);
            return base.Context.Queryable<T>().AS(sqlBuilder.GetPackTable(sql, sqlBuilder.GetDefaultShortName())).Select("*");
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
            return base.Context.Insertable(insertObjs.ToArray());
        }
        public virtual IInsertable<T> Insertable<T>(T insertObj) where T : class, new()
        {
            return base.Context.Insertable(new T[] { insertObj });
        }
        public virtual IInsertable<T> Insertable<T>(Dictionary<string, object> columnDictionary) where T : class, new()
        {
            InitMppingInfo<T>();
            Check.Exception(columnDictionary == null || columnDictionary.Count == 0, "Insertable.columnDictionary can't be null");
            var insertObject = base.Context.Utilities.DeserializeObject<T>(base.Context.Utilities.SerializeObject(columnDictionary));
            var columns = columnDictionary.Select(it => it.Key).ToList();
            return base.Context.Insertable(insertObject).InsertColumns(it => columns.Any(c => it.Equals(c, StringComparison.CurrentCultureIgnoreCase))); ;
        }
        public virtual IInsertable<T> Insertable<T>(dynamic insertDynamicObject) where T : class, new()
        {
            InitMppingInfo<T>();
            if (insertDynamicObject is T)
            {
                return base.Context.Insertable((T)insertDynamicObject);
            }
            else
            {
                var columns = ((object)insertDynamicObject).GetType().GetProperties().Select(it => it.Name).ToList();
                Check.Exception(columns.IsNullOrEmpty(), "Insertable.updateDynamicObject can't be null");
                T insertObject = base.Context.Utilities.DeserializeObject<T>(base.Context.Utilities.SerializeObject(insertDynamicObject));
                return base.Context.Insertable(insertObject).InsertColumns(it => columns.Any(c => it.Equals(c, StringComparison.CurrentCultureIgnoreCase)));
            }
        }
        #endregion

        #region Deleteable
        public virtual IDeleteable<T> Deleteable<T>() where T : class, new()
        {
            InitMppingInfo<T>();
            DeleteableProvider<T> reval = base.CreateDeleteable<T>();
            return reval;
        }
        public virtual IDeleteable<T> Deleteable<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            InitMppingInfo<T>();
            return base.Context.Deleteable<T>().Where(expression);
        }
        public virtual IDeleteable<T> Deleteable<T>(dynamic primaryKeyValue) where T : class, new()
        {
            InitMppingInfo<T>();
            return base.Context.Deleteable<T>().In(primaryKeyValue);
        }
        public virtual IDeleteable<T> Deleteable<T>(dynamic[] primaryKeyValues) where T : class, new()
        {
            InitMppingInfo<T>();
            return base.Context.Deleteable<T>().In(primaryKeyValues);
        }
        public virtual IDeleteable<T> Deleteable<T>(List<dynamic> pkValue) where T : class, new()
        {
            InitMppingInfo<T>();
            return base.Context.Deleteable<T>().In(pkValue);
        }
        public virtual IDeleteable<T> Deleteable<T>(T deleteObj) where T : class, new()
        {
            InitMppingInfo<T>();
            return base.Context.Deleteable<T>().Where(deleteObj);
        }
        public virtual IDeleteable<T> Deleteable<T>(List<T> deleteObjs) where T : class, new()
        {
            InitMppingInfo<T>();
            return base.Context.Deleteable<T>().Where(deleteObjs);
        }
        #endregion

        #region Updateable
        public virtual IUpdateable<T> Updateable<T>(T[] UpdateObjs) where T : class, new()
        {
            InitMppingInfo<T>();
            UpdateableProvider<T> reval = base.CreateUpdateable(UpdateObjs);
            return reval;
        }
        public virtual IUpdateable<T> Updateable<T>(List<T> UpdateObjs) where T : class, new()
        {
            Check.ArgumentNullException(UpdateObjs, "Updateable.UpdateObjs can't be null");
            return Updateable(UpdateObjs.ToArray());
        }
        public virtual IUpdateable<T> Updateable<T>(T UpdateObj) where T : class, new()
        {
            return base.Context.Updateable(new T[] { UpdateObj });
        }
        public virtual IUpdateable<T> Updateable<T>() where T : class, new()
        {
            return base.Context.Updateable(new T[] { new T() });
        }
        public virtual IUpdateable<T> Updateable<T>(Dictionary<string, object> columnDictionary) where T : class, new()
        {
            InitMppingInfo<T>();
            Check.Exception(columnDictionary == null || columnDictionary.Count == 0, "Updateable.columnDictionary can't be null");
            var updateObject = base.Context.Utilities.DeserializeObject<T>(base.Context.Utilities.SerializeObject(columnDictionary));
            var columns = columnDictionary.Select(it => it.Key).ToList();
            return base.Context.Updateable(updateObject).UpdateColumns(it => columns.Any(c => it.Equals(c, StringComparison.CurrentCultureIgnoreCase))); ;
        }
        public virtual IUpdateable<T> Updateable<T>(dynamic updateDynamicObject) where T : class, new()
        {
            InitMppingInfo<T>();
            if (updateDynamicObject is T)
            {
                return base.Context.Updateable((T)updateDynamicObject);
            }
            else
            {
                var columns = ((object)updateDynamicObject).GetType().GetProperties().Select(it => it.Name).ToList();
                Check.Exception(columns.IsNullOrEmpty(), "Updateable.updateDynamicObject can't be null");
                T updateObject = base.Context.Utilities.DeserializeObject<T>(base.Context.Utilities.SerializeObject(updateDynamicObject));
                return base.Context.Updateable(updateObject).UpdateColumns(it => columns.Any(c => it.Equals(c, StringComparison.CurrentCultureIgnoreCase))); ;
            }
        }
        #endregion

        #region DbFirst
        public virtual IDbFirst DbFirst
        {
            get
            {
                IDbFirst dbFirst = InstanceFactory.GetDbFirst(base.Context.CurrentConnectionConfig);
                dbFirst.Context = base.Context;
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
                ICodeFirst codeFirst = InstanceFactory.GetCodeFirst(base.Context.CurrentConnectionConfig);
                codeFirst.Context = base.Context;
                return codeFirst;
            }
        }
        #endregion

        #region Db Maintenance
        public virtual IDbMaintenance DbMaintenance
        {
            get
            {
                if (base.Context._DbMaintenance == null)
                {
                    IDbMaintenance maintenance = InstanceFactory.GetDbMaintenance(base.Context.CurrentConnectionConfig);
                    base.Context._DbMaintenance = maintenance;
                    maintenance.Context = base.Context;
                }
                return base.Context._DbMaintenance;
            }
        }
        #endregion

        #region Entity Maintenance
        [Obsolete("Use SqlSugarClient.EntityMaintenance")]
        public virtual EntityMaintenance EntityProvider
        {
            get { return base.Context.EntityMaintenance; }
            set {  base.Context.EntityMaintenance = value; }
        }
        public virtual EntityMaintenance EntityMaintenance
        {
            get
            {
                if (base.Context._EntityProvider == null)
                {
                    base.Context._EntityProvider = new EntityMaintenance();
                    base.Context._EntityProvider.Context = base.Context;
                }
                return base.Context._EntityProvider;
            }
            set { base.Context._EntityProvider = value; }
        }
        #endregion

        #region Gobal Filter
        public virtual QueryFilterProvider QueryFilter
        {
            get
            {
                if (base.Context._QueryFilterProvider == null)
                {
                    base.Context._QueryFilterProvider = new QueryFilterProvider();
                    base.Context._QueryFilterProvider.Context = base.Context;
                }
                return base.Context._QueryFilterProvider;
            }
            set { base.Context._QueryFilterProvider = value; }
        }
        #endregion

        #region SimpleClient
        public virtual SimpleClient SimpleClient
        {
            get
            {
                if (base.Context._SimpleClient == null)
                    base.Context._SimpleClient = new SimpleClient(base.Context);
                return base.Context._SimpleClient;
            }
        }
        #endregion

        #region Dispose OR Close
        public virtual void Close()
        {
            if (base.Context.Ado != null)
                base.Context.Ado.Close();
        }
        public virtual void Dispose()
        {
            if (base.Context.Ado != null)
                base.Context.Ado.Dispose();
        }
        #endregion
    }
}
