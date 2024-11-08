﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class SqlSugarProvider
    {
        #region Properties
        public SqlSugarProvider Context
        {
            get
            {
                _Context = this;
                return _Context;
            }
            set
            {
                _Context = value;
            }
        }
        public SqlSugarClient Root { get; set; }
        public ConnectionConfig CurrentConnectionConfig { get; set; }
        public Dictionary<string, object> TempItems { get { if (_TempItems == null) { _TempItems = new Dictionary<string, object>(); }  return _TempItems; } set { _TempItems = value; } }
        public bool IsSystemTablesConfig { get { return this.CurrentConnectionConfig.InitKeyType == InitKeyType.SystemTable; } }
        public Guid ContextID { get; set; }
        public MappingTableList MappingTables { get; set; }
        public MappingColumnList MappingColumns { get; set; }
        public IgnoreColumnList IgnoreColumns { get; set; }
        public IgnoreColumnList IgnoreInsertColumns { get; set; }
        public SugarActionType SugarActionType { get; set; } = SugarActionType.UnKnown;
        public ConfigQuery ConfigQuery { 
            get 
            {
                if (_SqlConfigTable==null) 
                {
                    _SqlConfigTable = new ConfigQuery() { Context = this.Context }; 
                }
                return _SqlConfigTable; 
             }
            set 
            {
                _SqlConfigTable = value;
            }
       }


        #endregion

        #region Fields       
        public Dictionary<string, object> _TempItems;
        public QueueList _Queues;
        protected ISqlBuilder _SqlBuilder;
        protected SqlSugarProvider _Context { get; set; }
        protected EntityMaintenance _EntityProvider;
        protected IAdo _Ado;
        protected ILambdaExpressions _LambdaExpressions;
        protected IContextMethods _RewritableMethods;
        protected IDbMaintenance _DbMaintenance;
        protected QueryFilterProvider _QueryFilterProvider;
        protected ConfigQuery _SqlConfigTable;
        //protected SimpleClient _SimpleClient;
        protected IAdo ContextAdo
        {
            get
            {
                return this._Ado;
            }
            set
            {
                this._Ado = value;
            }
        }
        protected IContextMethods ContextRewritableMethods
        {
            get
            {
                return this._RewritableMethods;
            }
            set
            {
                this._RewritableMethods = value;
            }
        }
        #endregion

        #region Init mappingInfo
        protected void InitMappingInfo<T, T2>()
        {
            InitMappingInfo<T>();
            InitMappingInfo<T2>();
        }
        protected void InitMappingInfo<T, T2, T3>()
        {
            InitMappingInfo<T, T2>();
            InitMappingInfo<T3>();
        }
        protected void InitMappingInfo<T, T2, T3, T4>()
        {
            InitMappingInfo<T, T2, T3>();
            InitMappingInfo<T4>();
        }
        protected void InitMappingInfo<T, T2, T3, T4, T5>()
        {
            InitMappingInfo<T, T2, T3, T4>();
            InitMappingInfo<T5>();
        }
        protected void InitMappingInfo<T, T2, T3, T4, T5, T6>()
        {
            InitMappingInfo<T, T2, T3, T4, T5>();
            InitMappingInfo<T6>();
        }
        protected void InitMappingInfo<T, T2, T3, T4, T5, T6, T7>()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6>();
            InitMappingInfo<T7>();
        }
        protected void InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8>()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7>();
            InitMappingInfo<T8>();
        }

        #region 9-12
        protected void InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9>()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8>();
            InitMappingInfo<T9>();
        }
        protected void InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9>();
            InitMappingInfo<T10>();
        }
        protected void InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
            InitMappingInfo<T11>();
        }
        protected void InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
        {
            InitMappingInfo<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
            InitMappingInfo<T12>();
        }
        #endregion

        public void InitMappingInfo<T>()
        {
            InitMappingInfo(typeof(T));
        }
        public void InitMappingInfo(Type type)
        {
            string cacheKey = "Context.InitAttributeMappingTables" + type.FullName+this.Context?.CurrentConnectionConfig?.ConfigId;
            var entityInfo = this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate<EntityInfo>(cacheKey,
              () =>
              {
                  var result = this.Context.EntityMaintenance.GetEntityInfoWithAttr(type);
                  return result;
              });
            //var copyObj = CopyEntityInfo(entityInfo);
            InitMappingInfo(entityInfo);
        }
        public void InitMappingInfoNoCache(Type type)
        {
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfoNoCache(type);
            InitMappingInfo(entityInfo);
        }
        public EntityInfo GetEntityNoCacheInitMappingInfo(Type type)
        {
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfoNoCache(type);
            InitMappingInfo(entityInfo);
            return entityInfo;
        }
        //private EntityInfo CopyEntityInfo(EntityInfo entityInfo)
        //{
        //    EntityInfo result = new EntityInfo()
        //    {
        //        DbTableName = entityInfo.DbTableName,
        //        EntityName = entityInfo.EntityName,
        //        Type = entityInfo.Type
        //    };
        //    List<EntityColumnInfo> columns = new List<EntityColumnInfo>();
        //    if (entityInfo.Columns.HasValue())
        //    {
        //        foreach (var item in entityInfo.Columns)
        //        {
        //            EntityColumnInfo column = new EntityColumnInfo()
        //            {
        //                ColumnDescription = item.ColumnDescription,
        //                DataType = item.DataType,
        //                DbColumnName = item.DbColumnName,
        //                DbTableName = item.DbTableName,
        //                DecimalDigits = item.DecimalDigits,
        //                DefaultValue = item.DefaultValue,
        //                EntityName = item.EntityName,
        //                IsIdentity = item.IsIdentity,
        //                IsIgnore = item.IsIgnore,
        //                IsNullable = item.IsNullable,
        //                IsOnlyIgnoreInsert = item.IsOnlyIgnoreInsert,
        //                IsPrimarykey = item.IsPrimarykey,
        //                Length = item.Length,
        //                OldDbColumnName = item.OldDbColumnName,
        //                OracleSequenceName = item.OracleSequenceName,
        //                PropertyInfo = item.PropertyInfo,
        //                PropertyName = item.PropertyName,
        //                IsArray=item.IsArray,
        //                IsJson=item.IsJson
        //            };
        //            columns.Add(item);
        //        }
        //    }
        //    result.Columns = columns;
        //    return result;
        //}

        private void InitMappingInfo(EntityInfo entityInfo)
        {
            if (this.MappingTables == null)
                this.MappingTables = new MappingTableList();
            if (this.MappingColumns == null)
                this.MappingColumns = new MappingColumnList();
            if (this.IgnoreColumns == null)
                this.IgnoreColumns = new IgnoreColumnList();
            if (this.IgnoreInsertColumns == null)
                this.IgnoreInsertColumns = new IgnoreColumnList();
            if (!this.MappingTables.Any(it => it.EntityName == entityInfo.EntityName))
            {
                if (entityInfo.DbTableName != entityInfo.EntityName && entityInfo.DbTableName.HasValue())
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
                        if (item.PropertyName != item.DbColumnName && item.DbColumnName.HasValue())
                            this.MappingColumns.Add(item.PropertyName, item.DbColumnName, item.EntityName);
                }
                var ignoreInfos = this.IgnoreColumns.Where(it => it.EntityName == entityInfo.EntityName);
                foreach (var item in entityInfo.Columns.Where(it => it.IsIgnore))
                {
                    if (!ignoreInfos.Any(it => it.PropertyName == item.PropertyName))
                        this.IgnoreColumns.Add(item.PropertyName, item.EntityName);
                }

                var ignoreInsertInfos = this.IgnoreInsertColumns.Where(it => it.EntityName == entityInfo.EntityName);
                foreach (var item in entityInfo.Columns.Where(it => it.IsOnlyIgnoreInsert))
                {
                    if (!ignoreInsertInfos.Any(it => it.PropertyName == item.PropertyName))
                        this.IgnoreInsertColumns.Add(item.PropertyName, item.EntityName);
                }
            }
        }
        #endregion

        #region Create Instance
        protected ISugarQueryable<T> CreateQueryable<T>()
        {
            ISugarQueryable<T> result = InstanceFactory.GetQueryable<T>(this.CurrentConnectionConfig);
            return CreateQueryable(result);
        }
        protected ISugarQueryable<T> CreateQueryable<T>(ISugarQueryable<T> result)
        {
            this.SugarActionType = SugarActionType.Query;
            Check.Exception(typeof(T).IsClass() == false || typeof(T).GetConstructors().Length == 0, "Queryable<{0}> Error ,{0} is invalid , need is a class,and can new().", typeof(T).Name);
            var sqlBuilder = InstanceFactory.GetSqlbuilder(CurrentConnectionConfig);
            result.Context = this.Context;
            result.SqlBuilder = sqlBuilder;
            result.SqlBuilder.QueryBuilder = InstanceFactory.GetQueryBuilder(CurrentConnectionConfig);
            result.SqlBuilder.QueryBuilder.Builder = sqlBuilder;
            result.SqlBuilder.Context = result.SqlBuilder.QueryBuilder.Context = this;
            result.SqlBuilder.QueryBuilder.EntityType = typeof(T);
            result.SqlBuilder.QueryBuilder.EntityName = typeof(T).Name;
            result.SqlBuilder.QueryBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(CurrentConnectionConfig);
            if (StaticConfig.CompleteQueryableFunc != null)
            {
                StaticConfig.CompleteQueryableFunc(result);
            }
            return result;
        }
        protected InsertableProvider<T> CreateInsertable<T>(T[] insertObjs) where T : class, new()
        {
            this.SugarActionType = SugarActionType.Insert;
            var result = InstanceFactory.GetInsertableProvider<T>(this.CurrentConnectionConfig);
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConnectionConfig); ;
            result.Context = this;
            result.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            result.SqlBuilder = sqlBuilder;
            result.InsertObjs = insertObjs;
            sqlBuilder.InsertBuilder = result.InsertBuilder = InstanceFactory.GetInsertBuilder(this.CurrentConnectionConfig);
            sqlBuilder.InsertBuilder.Builder = sqlBuilder;
            sqlBuilder.InsertBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);
            sqlBuilder.Context = result.SqlBuilder.InsertBuilder.Context = this;
            result.Init();
            if (StaticConfig.CompleteInsertableFunc != null) 
            {
                StaticConfig.CompleteInsertableFunc(result);
            }
            return result;
        }
        protected DeleteableProvider<T> CreateDeleteable<T>() where T : class, new()
        {
            this.SugarActionType = SugarActionType.Delete;
            var result = InstanceFactory.GetDeleteableProvider<T>(this.CurrentConnectionConfig);
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConnectionConfig); ;
            result.Context = this;
            result.SqlBuilder = sqlBuilder;
            sqlBuilder.DeleteBuilder = result.DeleteBuilder = InstanceFactory.GetDeleteBuilder(this.CurrentConnectionConfig);
            sqlBuilder.DeleteBuilder.Builder = sqlBuilder;
            sqlBuilder.DeleteBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);
            sqlBuilder.Context = result.SqlBuilder.DeleteBuilder.Context = this;
            if (StaticConfig.CompleteDeleteableFunc != null)
            {
                StaticConfig.CompleteDeleteableFunc(result);
            }
            return result;
        }
        protected UpdateableProvider<T> CreateUpdateable<T>(T[] UpdateObjs) where T : class, new()
        {
            this.SugarActionType = SugarActionType.Update;
            var result = InstanceFactory.GetUpdateableProvider<T>(this.CurrentConnectionConfig);
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConnectionConfig); ;
            result.Context = this;
            result.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            result.SqlBuilder = sqlBuilder;
            result.UpdateObjs = UpdateObjs;
            sqlBuilder.UpdateBuilder = result.UpdateBuilder = InstanceFactory.GetUpdateBuilder(this.CurrentConnectionConfig);
            sqlBuilder.UpdateBuilder.Builder = sqlBuilder;
            sqlBuilder.UpdateBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConnectionConfig);
            sqlBuilder.Context = result.SqlBuilder.UpdateBuilder.Context = this;
            result.Init();
            if (StaticConfig.CompleteUpdateableFunc != null)
            {
                StaticConfig.CompleteUpdateableFunc(result);
            }
            return result;
        }

        protected void CreateQueryJoin<T>(Expression joinExpression, Type[] types, ISugarQueryable<T> queryable)
        {
            this.SugarActionType = SugarActionType.Query;
            this.CreateQueryable<T>(queryable);
            string shortName = string.Empty;
            List<SugarParameter> paramters = new List<SugarParameter>();
            queryable.SqlBuilder.QueryBuilder.JoinQueryInfos = this.GetJoinInfos(queryable.SqlBuilder, joinExpression, ref paramters, ref shortName, types);
            if (queryable.SqlBuilder.QueryBuilder.JoinQueryInfos.Any())
            {
                queryable.SqlBuilder.QueryBuilder.JoinQueryInfos.Last().EntityType = types.Last();
            }
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            queryable.SqlBuilder.QueryBuilder.JoinExpression = joinExpression;
            if (paramters != null)
            {
                queryable.SqlBuilder.QueryBuilder.Parameters.AddRange(paramters);
            }
            UtilMethods.AddDiscrimator(typeof(T), queryable, queryable.QueryBuilder.TableShortName + ".");
        }
        protected void CreateEasyQueryJoin<T>(Expression joinExpression, Type[] types, ISugarQueryable<T> queryable)
        {
            this.SugarActionType = SugarActionType.Query;
            this.CreateQueryable<T>(queryable);
            string shortName = string.Empty;
            queryable.SqlBuilder.QueryBuilder.EasyJoinInfos = this.GetEasyJoinInfo(joinExpression, ref shortName, queryable.SqlBuilder, types);
            queryable.SqlBuilder.QueryBuilder.TableShortName = shortName;
            queryable.SqlBuilder.QueryBuilder.JoinExpression = joinExpression;
            var isNoPgAuto = this.Context.CurrentConnectionConfig.MoreSettings?.PgSqlIsAutoToLower == false;
            if (isNoPgAuto) 
                queryable.SqlBuilder.QueryBuilder.TableShortName = queryable.SqlBuilder.GetTranslationColumnName(shortName);
        }
        #endregion

        #region Private methods
        private void _ThenMapper<T>(IEnumerable<T> list, Action<T> action)
        {
            MapperContext<T> result = new MapperContext<T>();
            result.context = this.Context;
            if (result.context.TempItems == null)
            {
                result.context.TempItems = new Dictionary<string, object>();
            }
            var key = "Queryable_To_Context";
            result.context.TempItems.Add(key, result);
            result.list = list.ToList();
            foreach (var item in list)
            {
                action.Invoke(item);
            }
            result.context.TempItems.Remove(key);
        }
        private async Task _ThenMapperAsync<T>(IEnumerable<T> list, Func<T, Task> action)
        {
            MapperContext<T> result = new MapperContext<T>();
            result.context = this.Context;
            if (result.context.TempItems == null)
            {
                result.context.TempItems = new Dictionary<string, object>();
            }
            var key = "Queryable_To_Context";
            result.context.TempItems.Add(key, result);
            result.list = list.ToList();
            foreach (var item in list)
            {
                await action.Invoke(item);
            }
            result.context.TempItems.Remove(key);
        }
        internal string GetN()
        {
            var N = "N";
            if (_Context.CurrentConnectionConfig.MoreSettings != null && _Context.CurrentConnectionConfig.MoreSettings.DisableNvarchar)
            {
                N = "";
            }
            return N;
        }
        public bool IsVarchar()
        {
            if (_Context.CurrentConnectionConfig.MoreSettings != null && _Context.CurrentConnectionConfig.MoreSettings.DisableNvarchar)
            {
                return true;
            }
            return false;
        }
        private  void CheckDbDependency(ConnectionConfig config)
        {
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
                    DependencyManagement.TryPostgreSQL();
                    break;
                case DbType.OpenGauss:
                    config.DbType = DbType.PostgreSQL;
                    if (this.CurrentConnectionConfig.MoreSettings == null)
                        this.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings();
                    this.CurrentConnectionConfig.MoreSettings.DatabaseModel = DbType.OpenGauss;
                    break;
                case DbType.HG:
                    InstanceFactory.CustomDllName = SugarCompatible.IsFramework ? throw new Exception("Only.NET CORE is supported") : "SqlSugar.HGCore";
                    break;
                case DbType.Kdbndp:
                    DependencyManagement.TryKdbndb();
                    break;
                case DbType.Dm:
                    DependencyManagement.TryDm();
                   break;
                case DbType.Oscar:
                    DependencyManagement.TryOscar();
                    break;
                case DbType.MySqlConnector:
                    InstanceFactory.CustomDllName = SugarCompatible.IsFramework?"SqlSugar.MySqlConnector": "SqlSugar.MySqlConnectorCore";
                    if (SugarCompatible.IsFramework.ObjToBool() == false) 
                    {
                        config.DbType= DbType.MySql;
                        InstanceFactory.CustomDllName = null;
                    }
                    break;
                case DbType.Access:
                    InstanceFactory.CustomDllName = SugarCompatible.IsFramework?"SqlSugar.Access": "SqlSugar.AccessCore";
                    break;
                case DbType.Custom:
                    Check.Exception(InstanceFactory.CustomDbName.IsNullOrEmpty() , "DbType.Custom: InstanceFactory.CustomDbName is not null  ");
                    Check.Exception(InstanceFactory.CustomNamespace.IsNullOrEmpty(), "DbType.Custom: InstanceFactory.CustomNamespace is not null  ");
                    Check.Exception(InstanceFactory.CustomDllName.IsNullOrEmpty(), "DbType.Custom: InstanceFactory.CustomDllName is not null  ");
                    break;
                case DbType.QuestDB:
                    DependencyManagement.TryPostgreSQL();
                    break;
                case DbType.ClickHouse:
                    Check.Exception(SugarCompatible.IsFramework, "ClickHouse only support .net core");
                    InstanceFactory.CustomDllName = SugarCompatible.IsFramework ? "SqlSugar.ClickHouse" : "SqlSugar.ClickHouseCore";
                    break;
                case DbType.GBase:
                    Check.Exception(SugarCompatible.IsFramework, "GBase only support .net core");
                    InstanceFactory.CustomDllName = SugarCompatible.IsFramework ? "SqlSugar.GBase" : "SqlSugar.GBaseCore";
                    break;
                case DbType.Odbc:
                    InstanceFactory.CustomDllName = SugarCompatible.IsFramework ? "SqlSugar.Odbc" : "SqlSugar.OdbcCore";
                    break;
                case DbType.OceanBaseForOracle:
                    Check.Exception(SugarCompatible.IsFramework, "OceanBaseForOracle only support .net core");
                    InstanceFactory.CustomDllName = SugarCompatible.IsFramework ? "SqlSugar.OceanBaseForOracle" : "SqlSugar.OceanBaseForOracleCore";
                    break;
                case DbType.TDSQLForPGODBC:
                    Check.Exception(SugarCompatible.IsFramework, "TDSQLForPGODBC only support .net core");
                    InstanceFactory.CustomDllName = SugarCompatible.IsFramework ? "SqlSugar.TDSQLForPGODBC" : "SqlSugar.TDSQLForPGODBC";
                    break;
                case DbType.GaussDB:
                    config.DbType = DbType.PostgreSQL;
                    if (this.CurrentConnectionConfig.MoreSettings == null)
                        this.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings();
                    this.CurrentConnectionConfig.MoreSettings.DatabaseModel = DbType.GaussDB;
                    break;
                case DbType.Vastbase:
                    config.DbType = DbType.PostgreSQL;
                    if (this.CurrentConnectionConfig.MoreSettings==null) 
                        this.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings();
                    this.CurrentConnectionConfig.MoreSettings.DatabaseModel = DbType.Vastbase;
                    break;
                case DbType.OceanBase:
                    config.DbType = DbType.MySql; 
                    break;
                case DbType.Tidb:
                    config.DbType = DbType.MySql;
                    break;
                case DbType.PolarDB:
                    config.DbType = DbType.MySql;
                    break;
                case DbType.TDSQL:
                    config.DbType = DbType.MySql;
                    break;
                case DbType.Doris:
                    config.DbType = DbType.MySql;
                    if (this.CurrentConnectionConfig.MoreSettings == null)
                        this.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings();
                    this.CurrentConnectionConfig.MoreSettings.DatabaseModel = DbType.Doris;
                    this.CurrentConnectionConfig.MoreSettings.DisableNvarchar = true;
                    break;
                case DbType.TDengine:
                    Check.Exception(SugarCompatible.IsFramework, "TDengine only support .net core");
                    InstanceFactory.CustomDllName = SugarCompatible.IsFramework ? "SqlSugar.TDengine" : "SqlSugar.TDengineCore";
                    break;
                case DbType.Xugu:
                    Check.Exception(SugarCompatible.IsFramework, "Xugu only support .net core");
                    //InstanceFactory.CustomDbName = "Xugu"; 
                    InstanceFactory.CustomDllName = "SqlSugar.XuguCore"; 
                    //InstanceFactory.CustomNamespace = "SqlSugar.Xugu"; 
                    break;
                case DbType.GoldenDB:
                    config.DbType = DbType.MySql;
                    break;
                default:
                    throw new Exception("ConnectionConfig.DbType is null");
            }
        }
        protected List<JoinQueryInfo> GetJoinInfos(ISqlBuilder sqlBuilder, Expression joinExpression, ref List<SugarParameter> parameters, ref string shortName, params Type[] entityTypeArray)
        {
            List<JoinQueryInfo> result = new List<JoinQueryInfo>();
            var lambdaParameters = ((LambdaExpression)joinExpression).Parameters.ToList();
            ILambdaExpressions expressionContext = sqlBuilder.QueryBuilder.LambdaExpressions;
            expressionContext.MappingColumns = this.MappingColumns;
            expressionContext.MappingTables = this.MappingTables;
            expressionContext.IsSingle = false;
            expressionContext.SugarContext = new ExpressionOutParameter() { Context=this.Context };
            if (this.Context.CurrentConnectionConfig.MoreSettings != null)
            {
                expressionContext.PgSqlIsAutoToLower = this.Context.CurrentConnectionConfig.MoreSettings.PgSqlIsAutoToLower;
            }
            else
            {
                expressionContext.PgSqlIsAutoToLower = true;
            }
            if (this.Context.CurrentConnectionConfig.ConfigureExternalServices != null)
                expressionContext.SqlFuncServices = this.Context.CurrentConnectionConfig.ConfigureExternalServices.SqlFuncServices;
            expressionContext.Resolve(joinExpression, ResolveExpressType.Join);
            int i = 0;
            var joinArray = MergeJoinArray(expressionContext.Result.GetResultArray());
            if (joinArray == null) return null;
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
                var isNoPgAuto = this.Context.CurrentConnectionConfig.MoreSettings?.PgSqlIsAutoToLower == false;
                if (isFirst)
                {
                    var firstItem = lambdaParameters.First();
                    lambdaParameters.Remove(firstItem);
                    shortName = firstItem.Name;
                    if (isNoPgAuto) 
                        shortName = sqlBuilder.GetTranslationColumnName(shortName);
                }
                var joinString = joinArray[i * 2 - 2];
                joinInfo.ShortName = lambdaParameters[i - 1].Name;
                joinInfo.JoinType = (JoinType)Enum.Parse(typeof(JoinType), joinString);
                joinInfo.JoinWhere = joinArray[i * 2 - 1];
                joinInfo.JoinIndex = i;
                joinInfo.EntityType = entityType;
                if (isNoPgAuto)
                    joinInfo.ShortName = sqlBuilder.GetTranslationColumnName(joinInfo.ShortName);
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
            if (joinArray == null) return null;
            foreach (var item in joinArray)
            {
                ++i;
                var isLast = joinArray.Length == i;
                var isJoinType = item.IsIn(JoinType.Full.ToString(),JoinType.Inner.ToString(), JoinType.Left.ToString(), JoinType.Right.ToString(),JoinType.Cross.ToString());
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
            var isNoPgAuto = this.Context.CurrentConnectionConfig.MoreSettings?.PgSqlIsAutoToLower == false;
            var index = 1;
            foreach (var item in entityTypeArray)
            {
                if (isNoPgAuto)
                {
                    result.Add(UtilConstants.Space +builder.GetTranslationColumnName(lambdaParameters[index].Name), item.Name);
                }
                else
                {
                    result.Add(UtilConstants.Space + lambdaParameters[index].Name, item.Name);
                }
                ++index;
            }
            return result;
        }
        #endregion
    }
}
