using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class SqlSugarAccessory
    {
        #region Properties
        public SqlSugarClient Context { get; set; }
        public ConnectionConfig CurrentConnectionConfig { get; set; }
        public Dictionary<string, object> TempItems { get; set; }
        public Guid ContextID { get; set; }
        public MappingTableList MappingTables = new MappingTableList();
        public MappingColumnList MappingColumns = new MappingColumnList();
        public IgnoreColumnList IgnoreColumns = new IgnoreColumnList();
        #endregion

        #region Fields
        protected ISqlBuilder _SqlBuilder;
        protected EntityProvider _EntityProvider;
        protected IAdo _Ado;
        protected ILambdaExpressions _LambdaExpressions;
        protected IRewritableMethods _RewritableMethods;
        protected IDbMaintenance _DbMaintenance;
        protected QueryFilterProvider _QueryFilterProvider;
        protected SimpleClient _SimpleClient;
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
        protected void InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8,T9,T10>()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7,T8,T9>();
            InitMppingInfo<T10>();
        }
        protected void InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11>()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9,T10>();
            InitMppingInfo<T11>();
        }
        protected void InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12>()
        {
            InitMppingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11>();
            InitMppingInfo<T12>();
        }
        #endregion

        protected void InitMppingInfo<T>()
        {
            InitMppingInfo(typeof(T));
        }
        public void InitMppingInfo(Type type)
        {
            string cacheKey = "Context.InitAttributeMappingTables" + type.FullName;
            var entityInfo = this.Context.RewritableMethods.GetCacheInstance<EntityInfo>().Func(cacheKey,
              (cm, key) =>
              {
                  var cacheInfo = cm[key];
                  return cacheInfo;
              },
              (cm, key) =>
              {
                  var reval = this.Context.EntityProvider.GetEntityInfo(type);
                  return reval;
              });
            InitMppingInfo(entityInfo);
        }
        private void InitMppingInfo(EntityInfo entityInfo)
        {
            if (this.MappingTables == null)
                this.MappingTables = new MappingTableList();
            if (this.MappingColumns == null)
                this.MappingColumns = new MappingColumnList();
            if (this.IgnoreColumns == null)
                this.IgnoreColumns = new IgnoreColumnList();
            if (!this.MappingTables.Any(it => it.EntityName == entityInfo.EntityName))
            {
                if (entityInfo.DbTableName != entityInfo.EntityName && entityInfo.DbTableName.IsValuable())
                {
                    this.MappingTables.Add(entityInfo.EntityName, entityInfo.DbTableName);
                }
            }
            if (entityInfo.Columns.Any(it => it.EntityName == entityInfo.EntityName))
            {
                var mappingColumnInfos = this.MappingColumns.Where(it => it.EntityName == entityInfo.EntityName);
                foreach (var item in entityInfo.Columns.Where(it => it.IsIgnore == false))
                {
                    if (!mappingColumnInfos.Any(it => it.PropertyName == item.PropertyName))
                        if (item.PropertyName != item.DbColumnName && item.DbColumnName.IsValuable())
                            this.MappingColumns.Add(item.PropertyName, item.DbColumnName, item.EntityName);
                }
                var ignoreInfos = this.IgnoreColumns.Where(it => it.EntityName == entityInfo.EntityName);
                foreach (var item in entityInfo.Columns.Where(it => it.IsIgnore))
                {
                    if (!ignoreInfos.Any(it => it.PropertyName == item.PropertyName))
                        this.IgnoreColumns.Add(item.PropertyName, item.EntityName);
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
            var reval = new InsertableProvider<T>();
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConnectionConfig); ;
            reval.Context = this.Context;
            reval.EntityInfo = this.Context.EntityProvider.GetEntityInfo<T>();
            reval.SqlBuilder = sqlBuilder;
            reval.InsertObjs = insertObjs;
            sqlBuilder.InsertBuilder = reval.InsertBuilder = InstanceFactory.GetInsertBuilder(this.CurrentConnectionConfig);
            sqlBuilder.InsertBuilder.Builder = sqlBuilder;
            sqlBuilder.InsertBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);
            sqlBuilder.Context = reval.SqlBuilder.InsertBuilder.Context = this.Context;
            reval.Init();
            return reval;
        }
        protected DeleteableProvider<T> CreateDeleteable<T>() where T : class, new()
        {
            var reval = new DeleteableProvider<T>();
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConnectionConfig); ;
            reval.Context = this.Context;
            reval.SqlBuilder = sqlBuilder;
            sqlBuilder.DeleteBuilder = reval.DeleteBuilder = InstanceFactory.GetDeleteBuilder(this.CurrentConnectionConfig);
            sqlBuilder.DeleteBuilder.Builder = sqlBuilder;
            sqlBuilder.DeleteBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);
            sqlBuilder.Context = reval.SqlBuilder.DeleteBuilder.Context = this.Context;
            return reval;
        }
        protected UpdateableProvider<T> CreateUpdateable<T>(T[] UpdateObjs) where T : class, new()
        {
            var reval = new UpdateableProvider<T>();
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConnectionConfig); ;
            reval.Context = this.Context;
            reval.EntityInfo = this.Context.EntityProvider.GetEntityInfo<T>();
            reval.SqlBuilder = sqlBuilder;
            reval.UpdateObjs = UpdateObjs;
            sqlBuilder.UpdateBuilder = reval.UpdateBuilder = InstanceFactory.GetUpdateBuilder(this.CurrentConnectionConfig);
            sqlBuilder.UpdateBuilder.Builder = sqlBuilder;
            sqlBuilder.UpdateBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);
            sqlBuilder.Context = reval.SqlBuilder.UpdateBuilder.Context = this.Context;
            reval.Init();
            return reval;
        }

        protected void CreateQueryJoin<T>(Expression joinExpression, Type[] types, ISugarQueryable<T> queryable) where T : class, new()
        {
            this.CreateQueryable<T>(queryable);
            string shortName = string.Empty;
            List<SugarParameter> paramters =new List<SugarParameter>();
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = this.GetJoinInfos(queryable.SqlBuilder,joinExpression, ref  paramters, ref shortName, types);
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            if (paramters != null) {
                queryable.SqlBuilder.QueryBuilder.Parameters.AddRange(paramters);
            }
        }
        protected void CreateEasyQueryJoin<T>(Expression joinExpression, Type[] types, ISugarQueryable<T> queryable) where T : class, new()
        {
            this.CreateQueryable<T>(queryable);
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.EasyJoinInfos = this.GetEasyJoinInfo(joinExpression, ref shortName,queryable.SqlBuilder,types);
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
        }
        #endregion

        #region Private methods
        protected List<JoinQueryInfo> GetJoinInfos(ISqlBuilder sqlBuilder,Expression joinExpression,ref List<SugarParameter> parameters, ref string shortName, params Type[] entityTypeArray)
        {
            List<JoinQueryInfo> result = new List<JoinQueryInfo>();
            var lambdaParameters = ((LambdaExpression)joinExpression).Parameters.ToList();
            ILambdaExpressions expressionContext = sqlBuilder.QueryBuilder.LambdaExpressions;
            expressionContext.MappingColumns = this.Context.MappingColumns;
            expressionContext.MappingTables = this.Context.MappingTables;
            expressionContext.Resolve(joinExpression, ResolveExpressType.Join);
            int i = 0;
            var joinArray = expressionContext.Result.GetResultArray();
            parameters = expressionContext.Parameters;
            foreach (var entityType in entityTypeArray)
            {
                var isFirst = i == 0; ++i;
                JoinQueryInfo joinInfo = new JoinQueryInfo();
                var hasMappingTable = expressionContext.MappingTables.IsValuable();
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
        protected Dictionary<string,string> GetEasyJoinInfo(Expression joinExpression, ref string shortName, ISqlBuilder builder, params Type[] entityTypeArray)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var lambdaParameters = ((LambdaExpression)joinExpression).Parameters.ToList();
            shortName = lambdaParameters.First().Name;
            var index = 1;
            foreach (var item in entityTypeArray)
            {
                result.Add(PubConst.Space +lambdaParameters[index].Name, item.Name);
                ++index;
            }
            return result;
        }
        #endregion
    }
}
