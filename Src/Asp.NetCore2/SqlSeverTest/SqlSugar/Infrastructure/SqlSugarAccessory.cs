using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class SqlSugarClient
    {
        #region Properties
        public SqlSugarClient Context
        {
            get
            {
                var result = _Context; ;
                if (CurrentConnectionConfig.IsShardSameThread)
                {
                    if (CallContext.ContextList.Value.IsNullOrEmpty())
                    {
                        CallContext.ContextList.Value = new List<SqlSugarClient>();
                        CallContext.ContextList.Value.Add(_Context);
                    }
                    else
                    {
                        var cacheContext = CallContext.ContextList.Value.FirstOrDefault(it =>
                         it.CurrentConnectionConfig.ConnectionString == _Context.CurrentConnectionConfig.ConnectionString &&
                         it.CurrentConnectionConfig.DbType == _Context.CurrentConnectionConfig.DbType &&
                         it.CurrentConnectionConfig.IsAutoCloseConnection == _Context.CurrentConnectionConfig.IsAutoCloseConnection &&
                         it.CurrentConnectionConfig.IsShardSameThread == _Context.CurrentConnectionConfig.IsShardSameThread);
                        if (cacheContext != null)
                        {
                            return cacheContext;
                        }
                    }
                }
                return result;
            }
            set
            {
                _Context = value;
            }
        }
        public ConnectionConfig CurrentConnectionConfig { get; set; }
        public Dictionary<string, object> TempItems { get; set; }
        public bool IsSystemTablesConfig { get { return this.CurrentConnectionConfig.InitKeyType == InitKeyType.SystemTable; } }
        public Guid ContextID { get; set; }
        public MappingTableList MappingTables = new MappingTableList();
        public MappingColumnList MappingColumns = new MappingColumnList();
        public IgnoreColumnList IgnoreColumns = new IgnoreColumnList();
        public IgnoreColumnList IgnoreInsertColumns = new IgnoreColumnList();
        #endregion

        #region Fields
        protected ISqlBuilder _SqlBuilder;
        protected SqlSugarClient _Context { get; set; }
        protected EntityMaintenance _EntityProvider;
        protected IAdo _Ado;
        protected ILambdaExpressions _LambdaExpressions;
        protected IContextMethods _RewritableMethods;
        protected IDbMaintenance _DbMaintenance;
        protected QueryFilterProvider _QueryFilterProvider;
        protected SimpleClient _SimpleClient;
        protected IAdo ContextAdo
        {
            get
            {
                return this.Context._Ado;
            }
            set
            {
                this.Context._Ado = value;
            }
        }
        protected IContextMethods ContextRewritableMethods
        {
            get
            {
                return this.Context._RewritableMethods;
            }
            set
            {
                this.Context._RewritableMethods = value;
            }
        }
        #endregion

        #region Init mppingInfo
        protected void InitMppingInfo<T, T2>()
        {
            InitMppingInfo<T>();
            InitMppingInfo<T2>();
        }
        protected void InitMppingInfo<T, T2, T3>()
        {
            InitMppingInfo<T, T2>();
            InitMppingInfo<T3>();
        }
        protected void InitMppingInfo<T, T2, T3, T4>()
        {
            InitMppingInfo<T, T2, T3>();
            InitMppingInfo<T4>();
        }
        protected void InitMppingInfo<T, T2, T3, T4, T5>()
        {
            InitMppingInfo<T, T2, T3, T4>();
            InitMppingInfo<T5>();
        }
        protected void InitMppingInfo<T, T2, T3, T4, T5, T6>()
        {
            InitMppingInfo<T, T2, T3, T4, T5>();
            InitMppingInfo<T6>();
        }
        protected void InitMppingInfo<T, T2, T3, T4, T5, T6, T7>()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6>();
            InitMppingInfo<T7>();
        }
        protected void InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8>()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7>();
            InitMppingInfo<T8>();
        }

        #region 9-12
        protected void InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9>()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8>();
            InitMppingInfo<T9>();
        }
        protected void InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9>();
            InitMppingInfo<T10>();
        }
        protected void InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
            InitMppingInfo<T11>();
        }
        protected void InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
            InitMppingInfo<T12>();
        }
        #endregion

        internal void InitMppingInfo<T>()
        {
            InitMppingInfo(typeof(T));
        }
        public void InitMppingInfo(Type type)
        {
            string cacheKey = "Context.InitAttributeMappingTables" + type.FullName;
            var entityInfo = this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate<EntityInfo>(cacheKey,
              () =>
              {
                  var result = this.Context.EntityMaintenance.GetEntityInfo(type);
                  return result;
              });
            var copyObj = CopyEntityInfo(entityInfo);
            InitMppingInfo(copyObj);
        }

        private EntityInfo CopyEntityInfo(EntityInfo entityInfo)
        {
            EntityInfo result = new EntityInfo()
            {
                DbTableName = entityInfo.DbTableName,
                EntityName = entityInfo.EntityName,
                Type = entityInfo.Type
            };
            List<EntityColumnInfo> columns = new List<EntityColumnInfo>();
            if (entityInfo.Columns.HasValue())
            {
                foreach (var item in entityInfo.Columns)
                {
                    EntityColumnInfo column = new EntityColumnInfo()
                    {
                        ColumnDescription = item.ColumnDescription,
                        DataType = item.DataType,
                        DbColumnName = item.DbColumnName,
                        DbTableName = item.DbTableName,
                        DecimalDigits = item.DecimalDigits,
                        DefaultValue = item.DefaultValue,
                        EntityName = item.EntityName,
                        IsIdentity = item.IsIdentity,
                        IsIgnore = item.IsIgnore,
                        IsNullable = item.IsNullable,
                        IsOnlyIgnoreInsert = item.IsOnlyIgnoreInsert,
                        IsPrimarykey = item.IsPrimarykey,
                        Length = item.Length,
                        OldDbColumnName = item.OldDbColumnName,
                        OracleSequenceName = item.OracleSequenceName,
                        PropertyInfo = item.PropertyInfo,
                        PropertyName = item.PropertyName
                    };
                    columns.Add(item);
                }
            }
            result.Columns = columns;
            return result;
        }

        private void InitMppingInfo(EntityInfo entityInfo)
        {
            if (this.Context.MappingTables == null)
                this.Context.MappingTables = new MappingTableList();
            if (this.Context.MappingColumns == null)
                this.Context.MappingColumns = new MappingColumnList();
            if (this.Context.IgnoreColumns == null)
                this.Context.IgnoreColumns = new IgnoreColumnList();
            if (this.Context.IgnoreInsertColumns == null)
                this.Context.IgnoreInsertColumns = new IgnoreColumnList();
            if (!this.Context.MappingTables.Any(it => it.EntityName == entityInfo.EntityName))
            {
                if (entityInfo.DbTableName != entityInfo.EntityName && entityInfo.DbTableName.HasValue())
                {
                    this.Context.MappingTables.Add(entityInfo.EntityName, entityInfo.DbTableName);
                }
            }
            if (entityInfo.Columns.Any(it => it.EntityName == entityInfo.EntityName))
            {
                var mappingColumnInfos = this.Context.MappingColumns.Where(it => it.EntityName == entityInfo.EntityName);
                foreach (var item in entityInfo.Columns.Where(it => it.IsIgnore == false))
                {
                    if (!mappingColumnInfos.Any(it => it.PropertyName == item.PropertyName))
                        if (item.PropertyName != item.DbColumnName && item.DbColumnName.HasValue())
                            this.Context.MappingColumns.Add(item.PropertyName, item.DbColumnName, item.EntityName);
                }
                var ignoreInfos = this.Context.IgnoreColumns.Where(it => it.EntityName == entityInfo.EntityName);
                foreach (var item in entityInfo.Columns.Where(it => it.IsIgnore))
                {
                    if (!ignoreInfos.Any(it => it.PropertyName == item.PropertyName))
                        this.Context.IgnoreColumns.Add(item.PropertyName, item.EntityName);
                }

                var ignoreInsertInfos = this.Context.IgnoreInsertColumns.Where(it => it.EntityName == entityInfo.EntityName);
                foreach (var item in entityInfo.Columns.Where(it => it.IsOnlyIgnoreInsert))
                {
                    if (!ignoreInsertInfos.Any(it => it.PropertyName == item.PropertyName))
                        this.Context.IgnoreInsertColumns.Add(item.PropertyName, item.EntityName);
                }
            }
        }
        #endregion

        #region Create Instance
        protected ISugarQueryable<T> CreateQueryable<T>() where T : class, new()
        {
            ISugarQueryable<T> result = InstanceFactory.GetQueryable<T>(this.CurrentConnectionConfig);
            return CreateQueryable(result);
        }
        protected ISugarQueryable<T> CreateQueryable<T>(ISugarQueryable<T> result) where T : class, new()
        {
            var sqlBuilder = InstanceFactory.GetSqlbuilder(CurrentConnectionConfig);
            result.Context = this.Context;
            result.SqlBuilder = sqlBuilder;
            result.SqlBuilder.QueryBuilder = InstanceFactory.GetQueryBuilder(CurrentConnectionConfig);
            result.SqlBuilder.QueryBuilder.Builder = sqlBuilder;
            result.SqlBuilder.Context = result.SqlBuilder.QueryBuilder.Context = this.Context;
            result.SqlBuilder.QueryBuilder.EntityType = typeof(T);
            result.SqlBuilder.QueryBuilder.EntityName = typeof(T).Name;
            result.SqlBuilder.QueryBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(CurrentConnectionConfig);
            return result;
        }
        protected InsertableProvider<T> CreateInsertable<T>(T[] insertObjs) where T : class, new()
        {
            var result = InstanceFactory.GetInsertableProvider<T>(this.CurrentConnectionConfig);
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConnectionConfig); ;
            result.Context = this.Context;
            result.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            result.SqlBuilder = sqlBuilder;
            result.InsertObjs = insertObjs;
            sqlBuilder.InsertBuilder = result.InsertBuilder = InstanceFactory.GetInsertBuilder(this.CurrentConnectionConfig);
            sqlBuilder.InsertBuilder.Builder = sqlBuilder;
            sqlBuilder.InsertBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);
            sqlBuilder.Context = result.SqlBuilder.InsertBuilder.Context = this.Context;
            result.Init();
            return result;
        }
        protected DeleteableProvider<T> CreateDeleteable<T>() where T : class, new()
        {
            var result = InstanceFactory.GetDeleteableProvider<T>(this.CurrentConnectionConfig);
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConnectionConfig); ;
            result.Context = this.Context;
            result.SqlBuilder = sqlBuilder;
            sqlBuilder.DeleteBuilder = result.DeleteBuilder = InstanceFactory.GetDeleteBuilder(this.CurrentConnectionConfig);
            sqlBuilder.DeleteBuilder.Builder = sqlBuilder;
            sqlBuilder.DeleteBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);
            sqlBuilder.Context = result.SqlBuilder.DeleteBuilder.Context = this.Context;
            return result;
        }
        protected UpdateableProvider<T> CreateUpdateable<T>(T[] UpdateObjs) where T : class, new()
        {
            var result = InstanceFactory.GetUpdateableProvider<T>(this.CurrentConnectionConfig);
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConnectionConfig); ;
            result.Context = this.Context;
            result.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            result.SqlBuilder = sqlBuilder;
            result.UpdateObjs = UpdateObjs;
            sqlBuilder.UpdateBuilder = result.UpdateBuilder = InstanceFactory.GetUpdateBuilder(this.CurrentConnectionConfig);
            sqlBuilder.UpdateBuilder.Builder = sqlBuilder;
            sqlBuilder.UpdateBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);
            sqlBuilder.Context = result.SqlBuilder.UpdateBuilder.Context = this.Context;
            result.Init();
            return result;
        }

        protected void CreateQueryJoin<T>(Expression joinExpression, Type[] types, ISugarQueryable<T> queryable) where T : class, new()
        {
            this.CreateQueryable<T>(queryable);
            string shortName = string.Empty;
            List<SugarParameter> paramters = new List<SugarParameter>();
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = this.GetJoinInfos(queryable.SqlBuilder, joinExpression, ref paramters, ref shortName, types);
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            if (paramters != null)
            {
                queryable.SqlBuilder.QueryBuilder.Parameters.AddRange(paramters);
            }
        }
        protected void CreateEasyQueryJoin<T>(Expression joinExpression, Type[] types, ISugarQueryable<T> queryable) where T : class, new()
        {
            this.CreateQueryable<T>(queryable);
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.EasyJoinInfos = this.GetEasyJoinInfo(joinExpression, ref shortName, queryable.SqlBuilder, types);
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
        }
        #endregion

        #region Private methods
        protected List<JoinQueryInfo> GetJoinInfos(ISqlBuilder sqlBuilder, Expression joinExpression, ref List<SugarParameter> parameters, ref string shortName, params Type[] entityTypeArray)
        {
            List<JoinQueryInfo> result = new List<JoinQueryInfo>();
            var lambdaParameters = ((LambdaExpression)joinExpression).Parameters.ToList();
            ILambdaExpressions expressionContext = sqlBuilder.QueryBuilder.LambdaExpressions;
            expressionContext.MappingColumns = this.Context.MappingColumns;
            expressionContext.MappingTables = this.Context.MappingTables;
            expressionContext.Resolve(joinExpression, ResolveExpressType.Join);
            int i = 0;
            var joinArray = MergeJoinArray(expressionContext.Result.GetResultArray());
            parameters = expressionContext.Parameters;
            foreach (var entityType in entityTypeArray)
            {
                var isFirst = i == 0; ++i;
                JoinQueryInfo joinInfo = new JoinQueryInfo();
                var hasMappingTable = expressionContext.MappingTables.HasValue();
                MappingTable mappingInfo = null;
                if (hasMappingTable)
                {
                    mappingInfo = expressionContext.MappingTables.FirstOrDefault(it => it.EntityName.Equals(entityType.Name, StringComparison.CurrentCultureIgnoreCase));
                    joinInfo.TableName = mappingInfo != null ? mappingInfo.DbTableName : entityType.Name;
                }
                else
                {
                    joinInfo.TableName = entityType.Name;
                }
                if (isFirst)
                {
                    var firstItem = lambdaParameters.First();
                    lambdaParameters.Remove(firstItem);
                    shortName = firstItem.Name;
                }
                var joinString = joinArray[i * 2 - 2];
                joinInfo.ShortName = lambdaParameters[i - 1].Name;
                joinInfo.JoinType = (JoinType)Enum.Parse(typeof(JoinType), joinString);
                joinInfo.JoinWhere = joinArray[i * 2 - 1];
                joinInfo.JoinIndex = i;
                result.Add((joinInfo));
            }
            expressionContext.Clear();
            return result;
        }

        private string[] MergeJoinArray(string[] joinArray)
        {
            List<string> result = new List<string>();
            string joinValue = null;
            int i = 0;
            foreach (var item in joinArray)
            {
                ++i;
                var isLast = joinArray.Length == i;
                var isJoinType = item.IsIn(JoinType.Inner.ToString(), JoinType.Left.ToString(), JoinType.Right.ToString());
                if (isJoinType)
                {
                    if (joinValue != null)
                        result.Add(joinValue);
                    joinValue = null;
                    result.Add(item);
                }
                else
                {
                    isJoinType = false;
                    joinValue += joinValue == null ? item : ("," + item);
                }
                if (isLast)
                {
                    result.Add(joinValue);
                }
            }
            return result.ToArray(); ;
        }

        protected Dictionary<string, string> GetEasyJoinInfo(Expression joinExpression, ref string shortName, ISqlBuilder builder, params Type[] entityTypeArray)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var lambdaParameters = ((LambdaExpression)joinExpression).Parameters.ToList();
            shortName = lambdaParameters.First().Name;
            var index = 1;
            foreach (var item in entityTypeArray)
            {
                result.Add(UtilConstants.Space + lambdaParameters[index].Name, item.Name);
                ++index;
            }
            return result;
        }
        #endregion
    }
}
