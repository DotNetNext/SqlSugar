using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class InsertableProvider<T> : IInsertable<T> where T : class, new()
    {
        public SqlSugarProvider Context { get; set; }
        public IAdo Ado { get { return Context.Ado; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public InsertBuilder InsertBuilder { get; set; }

        public bool IsMappingTable { get { return this.Context.MappingTables != null && this.Context.MappingTables.Any(); } }
        public bool IsMappingColumns { get { return this.Context.MappingColumns != null && this.Context.MappingColumns.Any(); } }
        public bool IsSingle { get { return this.InsertObjs.Length == 1; } }

        public EntityInfo EntityInfo { get; set; }
        public List<MappingColumn> MappingColumnList { get; set; }
        private List<string> IgnoreColumnNameList { get; set; }
        private bool IsOffIdentity { get; set; }
        public T[] InsertObjs { get; set; }

        public MappingTableList OldMappingTableList { get; set; }
        public bool IsAs { get; set; }
        public bool IsEnableDiffLogEvent { get; set; }
        public DiffLogModel diffModel { get; set; }
        private Action RemoveCacheFunc { get; set; }


        #region Core
        public void AddQueue()
        {
            var sqlObj = this.ToSql();
            this.Context.Queues.Add(sqlObj.Key, sqlObj.Value);
        }
        public virtual int ExecuteCommand()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteCommand();
            var result = Ado.ExecuteCommand(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            After(sql, null);
            return result;
        }
        public virtual KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            AutoRemoveDataCache();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, InsertBuilder.Parameters);
        }
        public virtual int ExecuteReturnIdentity()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteReturnIdentity();
            var result = Ado.GetInt(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            After(sql, result);
            return result;
        }

        public virtual long ExecuteReturnBigIdentity()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteReturnBigIdentity();
            var result = Convert.ToInt64(Ado.GetScalar(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()));
            After(sql, result);
            return result;
        }



        public virtual T ExecuteReturnEntity()
        {
            ExecuteCommandIdentityIntoEntity();
            return InsertObjs.First();
        }
        public virtual bool ExecuteCommandIdentityIntoEntity()
        {
            var result = InsertObjs.First();
            var identityKeys = GetIdentityKeys();
            if (identityKeys.Count == 0) { return this.ExecuteCommand() > 0; }
            var idValue = ExecuteReturnBigIdentity();
            Check.Exception(identityKeys.Count > 1, "ExecuteCommandIdentityIntoEntity does not support multiple identity keys");
            var identityKey = identityKeys.First();
            object setValue = 0;
            if (idValue > int.MaxValue)
                setValue = idValue;
            else
                setValue = Convert.ToInt32(idValue);
            this.Context.EntityMaintenance.GetProperty<T>(identityKey).SetValue(result, setValue, null);
            return idValue > 0;
        }

        public async Task<int> ExecuteCommandAsync()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteCommand();
            var result =await Ado.ExecuteCommandAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            After(sql, null);
            return result;
        }
        public virtual async Task<int> ExecuteReturnIdentityAsync()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteReturnIdentity();
            var result =await Ado.GetIntAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            After(sql, result);
            return result;
        }
        public async Task<T> ExecuteReturnEntityAsync()
        {
            await ExecuteCommandIdentityIntoEntityAsync();
            return InsertObjs.First();
        }
        public async Task<bool> ExecuteCommandIdentityIntoEntityAsync()
        {
            var result = InsertObjs.First();
            var identityKeys = GetIdentityKeys();
            if (identityKeys.Count == 0) { return await this.ExecuteCommandAsync() > 0; }
            var idValue =await ExecuteReturnBigIdentityAsync();
            Check.Exception(identityKeys.Count > 1, "ExecuteCommandIdentityIntoEntity does not support multiple identity keys");
            var identityKey = identityKeys.First();
            object setValue = 0;
            if (idValue > int.MaxValue)
                setValue = idValue;
            else
                setValue = Convert.ToInt32(idValue);
            this.Context.EntityMaintenance.GetProperty<T>(identityKey).SetValue(result, setValue, null);
            return idValue > 0;
        }
        public virtual async Task<long> ExecuteReturnBigIdentityAsync()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteReturnBigIdentity();
            var result = Convert.ToInt64(await Ado.GetScalarAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()));
            After(sql, result);
            return result;
        }
        #endregion

        #region Setting
        public IInsertable<T> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            IsAs = true;
            OldMappingTableList = this.Context.MappingTables;
            this.Context.MappingTables = this.Context.Utilities.TranslateCopy(this.Context.MappingTables);
            if (this.Context.MappingTables.Any(it => it.EntityName == entityName))
            {
                this.Context.MappingTables.Add(this.Context.MappingTables.First(it => it.EntityName == entityName).DbTableName, tableName);
            }
            this.Context.MappingTables.Add(entityName, tableName);
            return this; ;
        }
        public IInsertable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = InsertBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Any(ig => ig.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Any(ig => ig.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            return this;
        }
        public IInsertable<T> IgnoreColumns(params string[] columns)
        {
            if (columns == null)
                columns = new string[] { };
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => !columns.Any(ig => ig.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            return this;
        }

        public IInsertable<T> InsertColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = InsertBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => ignoreColumns.Any(ig => ig.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            return this;
        }

        public IInsertable<T> InsertColumns(string[] columns)
        {
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => columns.Any(ig => ig.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            return this;
        }

        public IInsertable<T> With(string lockString)
        {
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
                this.InsertBuilder.TableWithString = lockString;
            return this;
        }
        public IInsertable<T> IgnoreColumns(bool ignoreNullColumn, bool isOffIdentity = false) {
            Check.Exception(this.InsertObjs.Count() > 1&& ignoreNullColumn, ErrorMessage.GetThrowMessage("ignoreNullColumn NoSupport batch insert", "ignoreNullColumn 不支持批量操作"));
            this.IsOffIdentity = isOffIdentity;
            if (this.InsertBuilder.LambdaExpressions == null)
                this.InsertBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.Context.CurrentConnectionConfig);
            this.InsertBuilder.IsNoInsertNull = ignoreNullColumn;
            return this;
        }

        public IInsertable<T> RemoveDataCache()
        {
            this.RemoveCacheFunc = () =>
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                CacheSchemeMain.RemoveCache(cacheService, this.Context.EntityMaintenance.GetTableName<T>());
            };
            return this;
        }
        public MySqlBlueCopy<T> UseMySql()
        {
            return new MySqlBlueCopy<T>(this.Context, this.SqlBuilder, InsertObjs);
        }
        public SqlServerBlueCopy UseSqlServer()
        {
            PreToSql();
            var currentType = this.Context.CurrentConnectionConfig.DbType;
            Check.Exception(currentType != DbType.SqlServer, "UseSqlServer no support " + currentType);
            SqlServerBlueCopy result = new SqlServerBlueCopy();
            result.DbColumnInfoList =this.InsertBuilder.DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            result.InsertBuilder = this.InsertBuilder;
            result.Builder = this.SqlBuilder;
            result.Context = this.Context;
            result.Inserts=this.InsertObjs;
            return result;
        }

        public IInsertable<T> EnableDiffLogEvent(object businessData = null)
        {
            Check.Exception(this.InsertObjs.HasValue() && this.InsertObjs.Count() > 1, "DiffLog does not support batch operations");
            diffModel = new DiffLogModel();
            this.IsEnableDiffLogEvent = true;
            diffModel.BusinessData = businessData;
            diffModel.DiffType = DiffType.insert;
            return this;
        }

        public ISubInsertable<T> AddSubList(Expression<Func<T, object>> items)
        {
            Check.Exception(GetPrimaryKeys().Count == 0, typeof(T).Name + " need Primary key");
            Check.Exception(GetPrimaryKeys().Count > 1, typeof(T).Name + "Multiple primary keys are not supported");
            //Check.Exception(this.InsertObjs.Count() > 1, "SubInserable No Support Insertable(List<T>)");
            //Check.Exception(items.ToString().Contains(".First().")==false, items.ToString()+ " not supported ");
            if (this.InsertObjs == null || this.InsertObjs.Count() == 0)
            {
                return new SubInsertable<T>();
            }
            SubInsertable<T> result = new SubInsertable<T>();
            result.InsertObjects = this.InsertObjs;
            result.Context = this.Context;
            result.SubList = new List<SubInsertTreeExpression>();
            result.SubList.Add(new SubInsertTreeExpression() { Expression= items });
            result.InsertBuilder = this.InsertBuilder;
            result.Pk = GetPrimaryKeys().First();
            result.Entity = this.EntityInfo;
            return result;
        }
        public ISubInsertable<T> AddSubList(Expression<Func<T, SubInsertTree>> tree)
        {
            Check.Exception(GetPrimaryKeys().Count == 0, typeof(T).Name + " need Primary key");
            Check.Exception(GetPrimaryKeys().Count > 1, typeof(T).Name + "Multiple primary keys are not supported");
            //Check.Exception(this.InsertObjs.Count() > 1, "SubInserable No Support Insertable(List<T>)");
            //Check.Exception(items.ToString().Contains(".First().")==false, items.ToString()+ " not supported ");
            if (this.InsertObjs == null || this.InsertObjs.Count() == 0)
            {
                return new SubInsertable<T>();
            }
            SubInsertable<T> result = new SubInsertable<T>();
            result.InsertObjects = this.InsertObjs;
            result.Context = this.Context;
            result.SubList = new List<SubInsertTreeExpression>();
            result.InsertBuilder = this.InsertBuilder;
            result.Pk = GetPrimaryKeys().First();
            result.Entity = this.EntityInfo;
            result.AddSubList(tree);
            return result;
        }

        #endregion

        #region Protected Methods
        private string _ExecuteReturnBigIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            AutoRemoveDataCache();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            Before(sql);
            return sql;
        }
        private string _ExecuteReturnIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            AutoRemoveDataCache();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            Before(sql);
            return sql;
        }

        private string _ExecuteCommand()
        {
            if (InsertBuilder.DbColumnInfoList.HasValue())
            {
                var pks = GetPrimaryKeys();
                foreach (var item in InsertBuilder.DbColumnInfoList)
                {
                    var isPk = pks.Any(y => y.Equals(item.DbColumnName, StringComparison.CurrentCultureIgnoreCase)) || item.IsPrimarykey;
                    if (isPk && item.PropertyType == UtilConstants.GuidType && item.Value.ObjToString() == Guid.Empty.ToString())
                    {
                        item.Value = Guid.NewGuid();
                        if (InsertObjs.First().GetType().GetProperties().Any(it => it.Name == item.PropertyName))
                            InsertObjs.First().GetType().GetProperties().First(it => it.Name == item.PropertyName).SetValue(InsertObjs.First(), item.Value, null);
                    }
                }
            }
            InsertBuilder.IsReturnIdentity = false;
            PreToSql();
            AutoRemoveDataCache();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            Before(sql);
            return sql;
        }
        private void AutoRemoveDataCache()
        {
            var moreSetts = this.Context.CurrentConnectionConfig.MoreSettings;
            var extService = this.Context.CurrentConnectionConfig.ConfigureExternalServices;
            if (moreSetts != null && moreSetts.IsAutoRemoveDataCache && extService != null && extService.DataInfoCacheService != null)
            {
                this.RemoveDataCache();
            }
        }
        protected virtual void PreToSql()
        {
            #region Identities
            if (!IsOffIdentity)
            {
                List<string> identities = GetIdentityKeys();
                if (identities != null && identities.Any())
                {
                    this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it =>
                    {
                        return !identities.Any(i => it.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                    }).ToList();
                }
            }
            #endregion

            #region IgnoreColumns
            if (this.Context.IgnoreColumns != null && this.Context.IgnoreColumns.Any())
            {
                var currentIgnoreColumns = this.Context.IgnoreColumns.Where(it => it.EntityName == this.EntityInfo.EntityName).ToList();
                this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it =>
                {
                    return !currentIgnoreColumns.Any(i => it.PropertyName.Equals(i.PropertyName, StringComparison.CurrentCulture));
                }).ToList();
            }

            if (this.Context.IgnoreInsertColumns != null && this.Context.IgnoreInsertColumns.Any())
            {
                var currentIgnoreColumns = this.Context.IgnoreInsertColumns.Where(it => it.EntityName == this.EntityInfo.EntityName).ToList();
                this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it =>
                {
                    return !currentIgnoreColumns.Any(i => it.PropertyName.Equals(i.PropertyName, StringComparison.CurrentCulture));
                }).ToList();
            }
            #endregion
            if (this.IsSingle)
            {
                foreach (var item in this.InsertBuilder.DbColumnInfoList)
                {
                    if (this.InsertBuilder.Parameters == null) this.InsertBuilder.Parameters = new List<SugarParameter>();
                    var paramters = new SugarParameter(this.SqlBuilder.SqlParameterKeyWord + item.DbColumnName, item.Value, item.PropertyType);
                    if (InsertBuilder.IsNoInsertNull && paramters.Value == null)
                    {
                        continue;
                    }
                    if (item.IsJson)
                    {
                        paramters.IsJson = true;
                    }
                    if (item.IsArray)
                    {
                        paramters.IsArray = true;
                    }
                    this.InsertBuilder.Parameters.Add(paramters);
                }
            }
        }
        internal void Init()
        {
            InsertBuilder.EntityInfo = this.EntityInfo;
            Check.Exception(InsertObjs == null || InsertObjs.Count() == 0, "InsertObjs is null");
            int i = 0;
            foreach (var item in InsertObjs)
            {
                List<DbColumnInfo> insertItem = new List<DbColumnInfo>();
                if (item is Dictionary<string, object>)
                {
                    SetInsertItemByDic(i, item, insertItem);
                }
                else
                {
                    SetInsertItemByEntity(i, item, insertItem);
                }
                this.InsertBuilder.DbColumnInfoList.AddRange(insertItem);
                ++i;
            }
        }
        private void SetInsertItemByDic(int i, T item, List<DbColumnInfo> insertItem)
        {
            foreach (var column in item as Dictionary<string, object>)
            {
                var columnInfo = new DbColumnInfo()
                {
                    Value = column.Value,
                    DbColumnName = column.Key,
                    PropertyName = column.Key,
                    PropertyType = column.Value == null ? DBNull.Value.GetType() : UtilMethods.GetUnderType(column.Value.GetType()),
                    TableId = i
                };
                if (columnInfo.PropertyType.IsEnum())
                {
                    columnInfo.Value = Convert.ToInt64(columnInfo.Value);
                }
                insertItem.Add(columnInfo);
            }
        }
        private void SetInsertItemByEntity(int i, T item, List<DbColumnInfo> insertItem)
        {
            if (item == null)
            {
                return;
            }
            foreach (var column in EntityInfo.Columns)
            {
                if (column.IsIgnore || column.IsOnlyIgnoreInsert) continue;
                var columnInfo = new DbColumnInfo()
                {
                    Value = column.PropertyInfo.GetValue(item, null),
                    DbColumnName = GetDbColumnName(column.PropertyName),
                    PropertyName = column.PropertyName,
                    PropertyType = UtilMethods.GetUnderType(column.PropertyInfo),
                    TableId = i
                };
                if (column.IsJson)
                {
                    columnInfo.IsJson = true;
                }
                if (column.IsArray)
                {
                    columnInfo.IsArray = true;
                }
                if (columnInfo.PropertyType.IsEnum()&& columnInfo.Value!=null)
                {
                    columnInfo.Value = Convert.ToInt64(columnInfo.Value);
                }
                if (column.IsJson&& columnInfo.Value!=null)
                {
                    if(columnInfo.Value!=null)
                       columnInfo.Value = this.Context.Utilities.SerializeObject(columnInfo.Value);
                }
                var tranColumn=EntityInfo.Columns.FirstOrDefault(it => it.IsTranscoding && it.DbColumnName.Equals(column.DbColumnName, StringComparison.CurrentCultureIgnoreCase));
                if (tranColumn!=null&&columnInfo.Value.HasValue()) {
                    columnInfo.Value = UtilMethods.EncodeBase64(columnInfo.Value.ToString());
                }
                insertItem.Add(columnInfo);
            }
        }

        private string GetDbColumnName(string propertyName)
        {
            if (!IsMappingColumns)
            {
                return propertyName;
            }
            if (this.Context.MappingColumns.Any(it => it.EntityName.Equals(EntityInfo.EntityName, StringComparison.CurrentCultureIgnoreCase)))
            {
                this.MappingColumnList = this.Context.MappingColumns.Where(it => it.EntityName.Equals(EntityInfo.EntityName, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            if (MappingColumnList == null || !MappingColumnList.Any())
            {
                return propertyName;
            }
            else
            {
                var mappInfo = this.MappingColumnList.FirstOrDefault(it => it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
                return mappInfo == null ? propertyName : mappInfo.DbColumnName;
            }
        }

        protected virtual List<string> GetPrimaryKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetPrimaries(this.Context.EntityMaintenance.GetTableName(this.EntityInfo.EntityName));
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            }
        }
        protected virtual List<string> GetIdentityKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetIsIdentities(this.Context.EntityMaintenance.GetTableName(this.EntityInfo.EntityName));
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsIdentity).Select(it => it.DbColumnName).ToList();
            }
        }
        //private void TaskStart<Type>(Task<Type> result)
        //{
        //    if (this.Context.CurrentConnectionConfig.IsShardSameThread)
        //    {
        //        Check.Exception(true, "IsShardSameThread=true can't be used async method");
        //    }
        //    result.Start();
        //}
        protected void RestoreMapping()
        {
            if (IsAs)
            {
                this.Context.MappingTables = OldMappingTableList;
            }
        }
        //protected IInsertable<T> CopyInsertable()
        //{
        //    var asyncContext = this.Context.Utilities.CopyContext(true);
        //    asyncContext.CurrentConnectionConfig.IsAutoCloseConnection = true;
        //    asyncContext.IsAsyncMethod = true;
        //    var asyncInsertable = asyncContext.Insertable<T>(this.InsertObjs);
        //    var asyncInsertableBuilder = asyncInsertable.InsertBuilder;
        //    asyncInsertableBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList;
        //    asyncInsertableBuilder.EntityInfo = this.InsertBuilder.EntityInfo;
        //    asyncInsertableBuilder.Parameters = this.InsertBuilder.Parameters;
        //    asyncInsertableBuilder.sql = this.InsertBuilder.sql;
        //    asyncInsertableBuilder.IsNoInsertNull = this.InsertBuilder.IsNoInsertNull;
        //    asyncInsertableBuilder.IsReturnIdentity = this.InsertBuilder.IsReturnIdentity;
        //    asyncInsertableBuilder.EntityInfo = this.InsertBuilder.EntityInfo;
        //    asyncInsertableBuilder.TableWithString = this.InsertBuilder.TableWithString;
        //    if (this.RemoveCacheFunc != null)
        //    {
        //        asyncInsertable.RemoveDataCache();
        //    }
        //    return asyncInsertable;
        //}

        private void After(string sql, long? result)
        {
            if (this.IsEnableDiffLogEvent)
            {
                var isDisableMasterSlaveSeparation = this.Ado.IsDisableMasterSlaveSeparation;
                this.Ado.IsDisableMasterSlaveSeparation = true;
                var parameters = InsertBuilder.Parameters;
                if (parameters == null)
                    parameters = new List<SugarParameter>();
                diffModel.AfterData = GetDiffTable(sql, result);
                diffModel.Time = this.Context.Ado.SqlExecutionTime;
                if (this.Context.CurrentConnectionConfig.AopEvents.OnDiffLogEvent != null)
                    this.Context.CurrentConnectionConfig.AopEvents.OnDiffLogEvent(diffModel);
                this.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
            }
            if (this.RemoveCacheFunc != null)
            {
                this.RemoveCacheFunc();
            }
        }
        private void Before(string sql)
        {
            if (this.IsEnableDiffLogEvent)
            {
                var isDisableMasterSlaveSeparation = this.Ado.IsDisableMasterSlaveSeparation;
                this.Ado.IsDisableMasterSlaveSeparation = true;
                var parameters = InsertBuilder.Parameters;
                if (parameters == null)
                    parameters = new List<SugarParameter>();
                diffModel.BeforeData = null;
                diffModel.Sql = sql;
                diffModel.Parameters = parameters.ToArray();
                this.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
            }
        }
        private List<DiffLogTableInfo> GetDiffTable(string sql, long? identity)
        {
            List<SugarParameter> parameters = new List<SugarParameter>();
            List<DiffLogTableInfo> result = new List<DiffLogTableInfo>();
            var whereSql = string.Empty;
            List<IConditionalModel> cons = new List<IConditionalModel>();
            if (identity != null && identity > 0 && GetIdentityKeys().HasValue())
            {
                var fieldName = GetIdentityKeys().Last();
                cons.Add(new ConditionalModel() { ConditionalType = ConditionalType.Equal, FieldName = fieldName, FieldValue = identity.ToString() });
            }
            else
            {
                foreach (var item in this.EntityInfo.Columns.Where(it => it.IsIgnore == false && GetPrimaryKeys().Any(pk => pk.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase))))
                {
                    var fielddName = item.DbColumnName;
                    var fieldValue = this.EntityInfo.Columns.FirstOrDefault(it => it.PropertyName == item.PropertyName).PropertyInfo.GetValue(this.InsertObjs.Last(), null).ObjToString();
                    cons.Add(new ConditionalModel() { ConditionalType = ConditionalType.Equal, FieldName = fielddName, FieldValue = fieldValue });
                }
            }
            Check.Exception(cons.IsNullOrEmpty(), "Insertable.EnableDiffLogEvent need primary key");
            var sqlable = this.SqlBuilder.ConditionalModelToSql(cons);
            whereSql = sqlable.Key;
            parameters.AddRange(sqlable.Value);
            var dt = this.Context.Queryable<T>().Where(whereSql).AddParameters(parameters).ToDataTable();
            if (dt.Rows != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    DiffLogTableInfo item = new DiffLogTableInfo();
                    item.TableDescription = this.EntityInfo.TableDescription;
                    item.TableName = this.EntityInfo.DbTableName;
                    item.Columns = new List<DiffLogColumnInfo>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        DiffLogColumnInfo addItem = new DiffLogColumnInfo();
                        addItem.Value = row[col.ColumnName];
                        addItem.ColumnName = col.ColumnName;
                        addItem.ColumnDescription = this.EntityInfo.Columns.Where(it=>it.DbColumnName!=null).First(it => it.DbColumnName.Equals(col.ColumnName, StringComparison.CurrentCultureIgnoreCase)).ColumnDescription;
                        item.Columns.Add(addItem);
                    }
                    result.Add(item);
                }
                return result;
            }
            else
            {
                DiffLogTableInfo diffTable = new DiffLogTableInfo();
                diffTable.TableName = this.EntityInfo.DbTableName;
                diffTable.TableDescription = this.EntityInfo.TableDescription;
                diffTable.Columns = this.EntityInfo.Columns.Where(it => it.IsIgnore == false).Select(it => new DiffLogColumnInfo()
                {
                    ColumnDescription = it.ColumnDescription,
                    ColumnName = it.DbColumnName,
                    Value = it.PropertyInfo.GetValue(this.InsertObjs.Last(), null)
                }).ToList();
                return new List<DiffLogTableInfo>() { diffTable };
            }

        }
        #endregion

    }
}
