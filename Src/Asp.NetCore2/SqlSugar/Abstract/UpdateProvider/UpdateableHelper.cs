using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class UpdateableProvider<T> : IUpdateable<T> where T : class, new()
    {
        private bool IsUpdateNullByList()
        {
            return this.UpdateObjs.Count() > 1 && (this.UpdateBuilder.IsNoUpdateNull || this.UpdateBuilder.IsNoUpdateDefaultValue);
        }

        private int DatasTrackingExecommand()
        {
            var trakRows = 0;
            var isNoTran = this.Context.Ado.IsNoTran();
            try
            {
                if (isNoTran) 
                {
                    this.Context.Ado.BeginTran();
                }
                int i = 0;
                foreach (var item in this.UpdateObjs)
                {
                    var newUpdateable = this.Clone();
                    (newUpdateable as UpdateableProvider<T>).UpdateObjs = new[] { item };
                    newUpdateable.UpdateBuilder.IsListUpdate = null;
                    newUpdateable.UpdateBuilder.DbColumnInfoList =
                        newUpdateable.UpdateBuilder.DbColumnInfoList.Where(it => it.TableId == i).ToList();
                    AppendTracking(item, newUpdateable);
                    if (newUpdateable.UpdateBuilder.DbColumnInfoList?.Any() == true)
                    {
                        trakRows += newUpdateable.ExecuteCommand();
                    }
                    ++i;
                }
                if (isNoTran)
                {
                    this.Context.Ado.CommitTran();
                }
            }
            catch (Exception)
            {
                if (isNoTran)
                {
                    this.Context.Ado.RollbackTran();
                }
                throw;
            }
            return trakRows;
        }
        private async Task<int> DatasTrackingExecommandAsync()
        {
            var isNoTran = this.Context.Ado.IsNoTran();
            var trakRows = 0;
            try
            {
                if (isNoTran)
                {
                    await this.Context.Ado.BeginTranAsync();
                }
                int i = 0;
                foreach (var item in this.UpdateObjs)
                {
                    var newUpdateable = this.Clone();
                    (newUpdateable as UpdateableProvider<T>).UpdateObjs = new[] { item };
                    newUpdateable.UpdateBuilder.IsListUpdate = null;
                    newUpdateable.UpdateBuilder.DbColumnInfoList =
                        newUpdateable.UpdateBuilder.DbColumnInfoList.Where(it => it.TableId == i).ToList();
                    AppendTracking(item, newUpdateable);
                    if (newUpdateable.UpdateBuilder.DbColumnInfoList?.Any() == true)
                    {
                        trakRows +=await newUpdateable.ExecuteCommandAsync();
                    }
                    ++i;
                }
                if (isNoTran)
                {
                    await this.Context.Ado.CommitTranAsync();
                }
            }
            catch (Exception)
            {
                if (isNoTran)
                {
                   await  this.Context.Ado.RollbackTranAsync();
                }
                throw;
            }
            return trakRows;
        }
        private bool UpdateObjectNotWhere()
        {
            return this.Context.CurrentConnectionConfig.DbType != DbType.MySql
                && this.Context.CurrentConnectionConfig.DbType != DbType.MySqlConnector
                && this.Context.CurrentConnectionConfig.DbType != DbType.SqlServer;
        }
        private void AppendTracking(T item, IUpdateable<T> newUpdateable)
        {
            if (IsTrakingData() || IsTrakingDatas())
            {
                var trackingData = this.Context.TempItems.FirstOrDefault(it => it.Key.StartsWith("Tracking_" + item.GetHashCode()));
                var diffColumns = FastCopy.GetDiff(item, (T)trackingData.Value);
                if (diffColumns.Count > 0)
                {
                    var pks = EntityInfo.Columns
                        .Where(it => it.IsPrimarykey).Select(it => it.PropertyName).ToList();
                    diffColumns = diffColumns.Where(it => !pks.Contains(it)).ToList();
                    if (diffColumns.Count > 0)
                    {
                        newUpdateable.UpdateColumns(diffColumns.ToArray());
                    }
                }
                else
                {
                    (newUpdateable as UpdateableProvider<T>).UpdateObjs = new T[] { null };
                    newUpdateable.UpdateBuilder.DbColumnInfoList = new List<DbColumnInfo>();
                }
            }
        }
        private void AppendSets()
        {
            if (SetColumnsIndex > 0)
            {
                var keys = UpdateBuilder.SetValues.Select(it => SqlBuilder.GetNoTranslationColumnName(it.Key.ToLower())).ToList();
                var addKeys = keys.Where(k => !this.UpdateBuilder.DbColumnInfoList.Any(it => it.PropertyName.ToLower() == k || it.DbColumnName.ToLower() == k)).ToList();
                var addItems = this.EntityInfo.Columns.Where(it => !GetPrimaryKeys().Any(p => p.ToLower() == it.PropertyName?.ToLower() || p.ToLower() == it.DbColumnName?.ToLower()) && addKeys.Any(k => it.PropertyName?.ToLower() == k || it.DbColumnName?.ToLower() == k)).ToList();
                this.UpdateBuilder.DbColumnInfoList.AddRange(addItems.Select(it => new DbColumnInfo() { PropertyName = it.PropertyName, DbColumnName = it.DbColumnName }));
            }
            SetColumnsIndex++;
        }
        private string _ExecuteCommand()
        {
            CheckWhere();
            PreToSql();
            AutoRemoveDataCache();
            Check.ExceptionEasy(this.UpdateParameterIsNull&&this.UpdateBuilder.DbColumnInfoList.Count() == this.UpdateObjs.Length && this.UpdateObjs.Length==this.UpdateBuilder.DbColumnInfoList.Count(it => it.IsPrimarykey), "The primary key cannot be updated", "主键不能更新，更新主键会对代码逻辑存在未知隐患,如果非要更新：建议你删除在插入或者新建一个没主键的类。");
            Check.Exception(UpdateBuilder.WhereValues.IsNullOrEmpty() && GetPrimaryKeys().IsNullOrEmpty(), "You cannot have no primary key and no conditions");
            string sql = UpdateBuilder.ToSqlString();
            ValidateVersion();
            RestoreMapping();
            Before(sql);
            return sql;
        }

        private void CheckWhere()
        {
            if (UpdateParameterIsNull && UpdateBuilder.WhereValues.IsNullOrEmpty()) 
            {
                Check.ExceptionEasy("Update requires conditions", "更新需要条件 Where");
            }
        }

        private void _WhereColumn(string columnName)
        {
            var columnInfos = columns.Where(it => it.DbColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase) || it.PropertyName.Equals(columnName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!this.UpdateBuilder.DbColumnInfoList.Any(y => y.DbColumnName == columnInfos.First().DbColumnName))
            {
                this.UpdateBuilder.DbColumnInfoList.AddRange(columnInfos);
            }
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
        internal void Init()
        {
            this.UpdateBuilder.TableName = EntityInfo.EntityName;
            if (IsMappingTable)
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.EntityName == EntityInfo.EntityName);
                if (mappingInfo != null)
                {
                    this.UpdateBuilder.TableName = mappingInfo.DbTableName;
                }
            }
            //Check.Exception(UpdateObjs == null || UpdateObjs.Count() == 0, "UpdateObjs is null");
            int i = 0;
            if (this.EntityInfo.Columns.Any(it => it.IsPrimarykey)) 
            {
                this.UpdateBuilder.OldPrimaryKeys = this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it=>it.DbColumnName).ToList();
            }
            foreach (var item in UpdateObjs)
            {
                List<DbColumnInfo> updateItem = new List<DbColumnInfo>();
                var isDic = item is Dictionary<string, object>;
                if (item is Dictionary<string, string>) 
                {
                    Check.ExceptionEasy("To use Updateable dictionary, use string or object", "Updateable字典请使用string,object类型");
                }
                if (isDic)
                {
                    SetUpdateItemByDic(i, item, updateItem);
                }
                else
                {
                    DataAop(item);
                    SetUpdateItemByEntity(i, item, updateItem);
                    Tracking(item);
                }
                ++i;
            }
            this.columns = this.UpdateBuilder.DbColumnInfoList;

            var ignoreColumns = EntityInfo.Columns.Where(it => it.IsOnlyIgnoreUpdate).ToList();
            if (ignoreColumns != null && ignoreColumns.Any())
            {
                this.IgnoreColumns(ignoreColumns.Select(it => it.PropertyName).ToArray());
            }
        }

        private void Tracking(T item)
        {
            if (IsTrakingData())
            {
                var trackingData = this.Context.TempItems.FirstOrDefault(it => it.Key.StartsWith("Tracking_" + item.GetHashCode()));
                if (trackingData.Key == null && trackingData.Value == null) 
                {
                    return;
                }
                var diffColumns = FastCopy.GetDiff(item, (T)trackingData.Value);
                if (diffColumns.Count > 0)
                {
                    var pks = EntityInfo.Columns
                        .Where(it => it.IsPrimarykey).Select(it => it.PropertyName).ToList();
                    diffColumns = diffColumns.Where(it => !pks.Contains(it)).ToList();
                    if (diffColumns.Count > 0)
                    {
                        this.UpdateColumns(diffColumns.ToArray());
                    }
                }
                else 
                {
                    this.UpdateObjs = new T [] { null };
                    this.UpdateBuilder.DbColumnInfoList = new List<DbColumnInfo>();
                }
            }
        }

        private bool IsTrakingData()
        {
            return this.UpdateParameterIsNull == false
                                    && this.Context.TempItems != null
                                    && this.Context.TempItems.Any(it => it.Key.StartsWith("Tracking_"))
                                    && this.UpdateObjs.Length == 1;
        }

        private bool IsTrakingDatas()
        {
            return this.UpdateParameterIsNull == false
                                    && this.Context.TempItems != null
                                    && this.Context.TempItems.Any(it => it.Key.StartsWith("Tracking_"))
                                    && this.UpdateObjs.Length > 1;
        }
        private void DataAop(T item)
        {
            var dataEvent = this.Context.CurrentConnectionConfig.AopEvents?.DataExecuting;
            if (dataEvent != null && item != null)
            {
                foreach (var columnInfo in this.EntityInfo.Columns)
                {
                    dataEvent(columnInfo.PropertyInfo.GetValue(item, null), new DataFilterModel() { OperationType = DataFilterType.UpdateByObject, EntityValue = item, EntityColumnInfo = columnInfo });
                }
            }
        }

        private void CheckTranscodeing(bool checkIsJson = true)
        {
            if (this.EntityInfo.Columns.Any(it => it.IsTranscoding))
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage("UpdateColumns no support IsTranscoding", "SetColumns方式更新不支持IsTranscoding，你可以使用db.Updateable(实体)的方式更新"));
            }
            //if (checkIsJson && this.EntityInfo.Columns.Any(it => it.IsJson))
            //{
            //    Check.Exception(true, ErrorMessage.GetThrowMessage("UpdateColumns no support IsJson", "SetColumns方式更新不支持IsJson，你可以使用db.Updateable(实体)的方式更新"));
            //}
            //if (this.EntityInfo.Columns.Any(it => it.IsArray))
            //{
            //    Check.Exception(true, ErrorMessage.GetThrowMessage("UpdateColumns no support IsArray", "SetColumns方式更新不支持IsArray，你可以使用db.Updateable(实体)的方式更新"));
            //}
        }
        private void SetUpdateItemByDic(int i, T item, List<DbColumnInfo> updateItem)
        {
            foreach (var column in (item as Dictionary<string, object>).OrderBy(it=>it.Key))
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
                    if (this.Context.CurrentConnectionConfig.MoreSettings?.TableEnumIsString == true)
                    {
                        columnInfo.PropertyType = UtilConstants.StringType;
                        columnInfo.Value = columnInfo.Value.ToString();
                    }
                    else
                    {
                        columnInfo.Value = Convert.ToInt64(columnInfo.Value);
                    }
                }
                updateItem.Add(columnInfo);
            }
            this.UpdateBuilder.DbColumnInfoList.AddRange(updateItem);
        }
        private void SetUpdateItemByEntity(int i, T item, List<DbColumnInfo> updateItem)
        {
            foreach (var column in EntityInfo.Columns)
            {
                if (column.IsIgnore) continue;
                var columnInfo = new DbColumnInfo()
                {
                    Value = column.PropertyInfo.GetValue(item, null),
                    DbColumnName = GetDbColumnName(column.PropertyName),
                    PropertyName = column.PropertyName,
                    PropertyType = UtilMethods.GetUnderType(column.PropertyInfo),
                    SqlParameterDbType = column.SqlParameterDbType,
                    TableId = i,
                    UpdateSql=column.UpdateSql,
                    UpdateServerTime= column.UpdateServerTime
                };
                if (columnInfo.PropertyType.IsEnum() && columnInfo.Value != null)
                {
                    if (this.Context.CurrentConnectionConfig.MoreSettings?.TableEnumIsString == true)
                    {
                        columnInfo.PropertyType = UtilConstants.StringType;
                        columnInfo.Value = columnInfo.Value.ToString();
                    }
                    else
                    {
                        columnInfo.Value = Convert.ToInt64(columnInfo.Value);
                    }
                }
                if (column.IsJson)
                {
                    columnInfo.IsJson = true;
                    if (columnInfo.Value != null)
                        columnInfo.Value = this.Context.Utilities.SerializeObject(columnInfo.Value);
                }
                if (column.IsArray)
                {
                    columnInfo.IsArray = true;
                }
                var tranColumn = EntityInfo.Columns.FirstOrDefault(it => it.IsTranscoding && it.DbColumnName.Equals(column.DbColumnName, StringComparison.CurrentCultureIgnoreCase));
                if (tranColumn != null && columnInfo.Value.HasValue())
                {
                    columnInfo.Value = UtilMethods.EncodeBase64(columnInfo.Value.ToString());
                }
                updateItem.Add(columnInfo);
            }
            this.UpdateBuilder.DbColumnInfoList.AddRange(updateItem);
        }

        private void PreToSql()
        {
            if (this.UpdateBuilder.UpdateColumns.HasValue())
            {
                var columns = this.UpdateBuilder.UpdateColumns;
                this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => GetPrimaryKeys().Select(
                iit => iit.ToLower()).Contains(it.DbColumnName.ToLower()) 
                || columns.Contains(it.PropertyName, StringComparer.OrdinalIgnoreCase)
                || columns.Contains(it.DbColumnName, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            UpdateBuilder.PrimaryKeys = GetPrimaryKeys();
            if (this.IsWhereColumns)
            {
                foreach (var pkName in UpdateBuilder.PrimaryKeys)
                {
                    if (WhereColumnList != null && WhereColumnList.Count() > 0)
                    {
                        continue;
                    }
                    var isContains = this.UpdateBuilder.DbColumnInfoList.Select(it => it.DbColumnName.ToLower()).Contains(pkName.ToLower());
                    Check.Exception(isContains == false, "Use UpdateColumns().WhereColumn() ,UpdateColumns need {0}", pkName);
                }
            }
            #region IgnoreColumns
            if (this.Context.IgnoreColumns != null && this.Context.IgnoreColumns.Any())
            {
                var currentIgnoreColumns = this.Context.IgnoreColumns.Where(it => it.EntityName == this.EntityInfo.EntityName).ToList();
                this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it =>
                {
                    return !currentIgnoreColumns.Any(i => it.PropertyName.Equals(i.PropertyName, StringComparison.CurrentCulture));
                }).ToList();
            }
            #endregion
            if (this.IsSingle)
            {
                var isDic = this.EntityInfo.DbTableName.StartsWith("Dictionary`");
                foreach (var item in this.UpdateBuilder.DbColumnInfoList)
                {
                    if (this.UpdateBuilder.Parameters == null) this.UpdateBuilder.Parameters = new List<SugarParameter>();
                    if (this.UpdateBuilder.SetValues.Any(it => this.SqlBuilder.GetNoTranslationColumnName(it.Key) == item.PropertyName))
                    {
                        continue;
                    }
                    if (item.SqlParameterDbType is Type)
                    {
                        continue;
                    }
                    var parameter = new SugarParameter(this.SqlBuilder.SqlParameterKeyWord + item.DbColumnName, item.Value, item.PropertyType);
                    if (item.IsJson)
                    {
                        parameter.IsJson = true;
                        SqlBuilder.ChangeJsonType(parameter);
                    }
                    if (item.IsArray)
                    {
                        parameter.IsArray = true;
                        if (parameter.Value == null || parameter.Value == DBNull.Value)
                        {
                            ArrayNull(item, parameter);
                        }
                    }
                    if (item.Value == null && isDic)
                    {
                        var type = this.SqlBuilder.GetNullType(this.UpdateBuilder.GetTableNameString, item.DbColumnName);
                        if (type != null)
                        {
                            parameter = new SugarParameter(this.SqlBuilder.SqlParameterKeyWord + item.DbColumnName, item.Value, type);
                        }
                    }
                    this.UpdateBuilder.Parameters.Add(parameter);
                }
            }

            #region Identities
            List<string> identities = GetIdentityKeys();
            if (identities != null && identities.Any())
            {
                this.UpdateBuilder.DbColumnInfoList.ForEach(it =>
                {
                    var mappingInfo = identities.SingleOrDefault(i => it.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                    if (mappingInfo != null && mappingInfo.Any())
                    {
                        it.IsIdentity = true;
                    }
                });
            }
            #endregion
            List<string> primaryKey = GetPrimaryKeys();
            if (primaryKey != null && primaryKey.Count > 0)
            {
                this.UpdateBuilder.DbColumnInfoList.ForEach(it =>
                {
                    var mappingInfo = primaryKey.SingleOrDefault(i => it.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                    if (mappingInfo != null && mappingInfo.Any())
                    {
                        it.IsPrimarykey = true;
                    }
                });
            }
            if (this.UpdateBuilder.Parameters.HasValue() && this.UpdateBuilder.SetValues.IsValuable())
            {
                this.UpdateBuilder.Parameters.RemoveAll(it => this.UpdateBuilder.SetValues.Any(v => (SqlBuilder.SqlParameterKeyWord + SqlBuilder.GetNoTranslationColumnName(v.Key)) == it.ParameterName));
            }
        }

        private static void ArrayNull(DbColumnInfo item, SugarParameter parameter)
        {
            if (item.PropertyType.IsIn(typeof(Guid[]), typeof(Guid?[])))
            {
                parameter.DbType = System.Data.DbType.Guid;
            }
            else if (item.PropertyType.IsIn(typeof(int[]), typeof(int?[])))
            {
                parameter.DbType = System.Data.DbType.Int32;
            }
            else if (item.PropertyType.IsIn(typeof(long[]), typeof(long?[])))
            {
                parameter.DbType = System.Data.DbType.Int64;
            }
            else if (item.PropertyType.IsIn(typeof(short[]), typeof(short?[])))
            {
                parameter.DbType = System.Data.DbType.Int16;
            }
        }
        private void OptRollBack(int updateRows,T updateData, object oldValue, string name)
        {
            if (updateRows == 0)
            {
                var verInfo = this.EntityInfo.Columns.FirstOrDefault(it => it.PropertyName == name);
                if (verInfo != null)
                {
                    verInfo.PropertyInfo.SetValue(updateData, oldValue);
                }
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
        private List<string> GetPrimaryKeys()
        {
            if (this.WhereColumnList.HasValue())
            {
                return this.WhereColumnList;
            }
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
        private void RestoreMapping()
        {
            if (IsAs)
            {
                this.Context.MappingTables = OldMappingTableList;
            }
        }


        private void ValidateVersion()
        {
            var versionColumn = this.EntityInfo.Columns.FirstOrDefault(it => it.IsEnableUpdateVersionValidation);
            var pks = this.UpdateBuilder.DbColumnInfoList.Where(it => it.IsPrimarykey).ToList();
            if (versionColumn != null && this.IsVersionValidation)
            {
                Check.Exception(pks.IsNullOrEmpty(), "UpdateVersionValidation the primary key is required.");
                List<IConditionalModel> conModels = new List<IConditionalModel>();
                foreach (var item in pks)
                {
                    conModels.Add(new ConditionalModel() {CSharpTypeName=item.PropertyType.Name, FieldName = item.DbColumnName, ConditionalType = ConditionalType.Equal, FieldValue = item.Value.ObjToString() });
                }
                var dbInfo = this.Context.Queryable<T>().Where(conModels).First();
                if (dbInfo != null)
                {
                    var currentVersion = this.EntityInfo.Type.GetProperty(versionColumn.PropertyName).GetValue(UpdateObjs.Last(), null);
                    var dbVersion = this.EntityInfo.Type.GetProperty(versionColumn.PropertyName).GetValue(dbInfo, null);
                    Check.Exception(currentVersion == null, "UpdateVersionValidation entity property {0} is not null", versionColumn.PropertyName);
                    Check.Exception(dbVersion == null, "UpdateVersionValidation database column {0} is not null", versionColumn.DbColumnName);
                    if (versionColumn.PropertyInfo.PropertyType.IsIn(UtilConstants.IntType, UtilConstants.LongType))
                    {
                        if (Convert.ToInt64(dbVersion) != Convert.ToInt64(currentVersion))
                        {
                            throw new VersionExceptions(string.Format("UpdateVersionValidation {0} Not the latest version ", versionColumn.PropertyName));
                        }
                    }
                    else if (versionColumn.PropertyInfo.PropertyType.IsIn(UtilConstants.DateType))
                    {
                        if (dbVersion.ObjToDate() != currentVersion.ObjToDate())
                        {
                            throw new VersionExceptions(string.Format("UpdateVersionValidation {0} Not the latest version ", versionColumn.PropertyName));
                        }
                    }
                    else if (versionColumn.PropertyInfo.PropertyType.IsIn(UtilConstants.ByteArrayType))
                    {
                        if (UtilMethods.GetLong((byte[])dbVersion) != UtilMethods.GetLong((byte[])currentVersion))
                        {
                            throw new VersionExceptions(string.Format("UpdateVersionValidation {0} Not the latest version ", versionColumn.PropertyName));
                        }
                    }
                    else
                    {
                        Check.ThrowNotSupportedException(string.Format("UpdateVersionValidation Not Supported Type [ {0} ] , {1}", versionColumn.PropertyInfo.PropertyType, versionColumn.PropertyName));
                    }
                }
            }
        }
        private void After(string sql)
        {
            if (this.IsEnableDiffLogEvent && !string.IsNullOrEmpty(sql))
            {
                var isDisableMasterSlaveSeparation = this.Ado.IsDisableMasterSlaveSeparation;
                this.Ado.IsDisableMasterSlaveSeparation = true;
                var parameters = UpdateBuilder.Parameters;
                if (parameters == null)
                    parameters = new List<SugarParameter>();
                diffModel.AfterData = GetDiffTable(sql, parameters);
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
        private string _ExecuteCommandWithOptLock(T updateData,ref object oldVerValue)
        {
            Check.ExceptionEasy(UpdateParameterIsNull == true, "Optimistic lock can only be an entity update method", "乐观锁只能是实体更新方式");
            var verColumn = this.EntityInfo.Columns.FirstOrDefault(it => it.IsEnableUpdateVersionValidation);
            Check.ExceptionEasy(verColumn == null, $" {this.EntityInfo.EntityName } need  IsEnableUpdateVersionValidation=true ", $"实体{this.EntityInfo.EntityName}没有找到版本标识特性 IsEnableUpdateVersionValidation");
            Check.ExceptionEasy(UpdateObjs.Length > 1, $"Optimistic lock can only handle a single update ", $"乐观锁只能处理单条更新");
            Check.ExceptionEasy(!verColumn.UnderType.IsIn(UtilConstants.StringType, UtilConstants.LongType, UtilConstants.GuidType, UtilConstants.DateType), $"Optimistic locks can only be guid, long, and string types", $"乐观锁只能是Guid、Long和字符串类型");
            var oldValue = verColumn.PropertyInfo.GetValue(updateData);
            oldVerValue = oldValue;
            var newValue = UtilMethods.GetRandomByType(verColumn.UnderType);
            verColumn.PropertyInfo.SetValue(updateData, newValue);
            var data = this.UpdateBuilder.DbColumnInfoList.FirstOrDefault(it =>
            it.PropertyName.EqualCase(verColumn.PropertyName));
            if (data == null) 
            {
                data = new DbColumnInfo() { DbColumnName= verColumn.DbColumnName,PropertyName=verColumn.PropertyName, Value=newValue };
                this.UpdateBuilder.DbColumnInfoList.Add(data);
            }
            data.Value = newValue;
            var pks = GetPrimaryKeys();
            Check.ExceptionEasy(pks.Count == 0, "need primary key or WhereColumn", "需要主键或者WhereColumn");
            this.Where(verColumn.DbColumnName, "=", oldValue);
            foreach (var p in pks)
            {
                var pkColumn = this.EntityInfo.Columns.FirstOrDefault(
                    it => it.DbColumnName.EqualCase(p) || it.PropertyName.EqualCase(p));
                this.Where(pkColumn.DbColumnName, "=", pkColumn.PropertyInfo.GetValue(updateData));
            }
            return verColumn.PropertyName;
        }
        private void Before(string sql)
        {
            if (this.IsEnableDiffLogEvent&&!string.IsNullOrEmpty(sql))
            {
                var isDisableMasterSlaveSeparation = this.Ado.IsDisableMasterSlaveSeparation;
                this.Ado.IsDisableMasterSlaveSeparation = true;
                var parameters = UpdateBuilder.Parameters;
                if (parameters == null)
                    parameters = new List<SugarParameter>();
                diffModel.BeforeData = GetDiffTable(sql, parameters);
                diffModel.Sql = sql;
                diffModel.Parameters = parameters.ToArray();
                this.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
            }
        }
        private bool IsPrimaryKey(DbColumnInfo it)
        {
            var result = GetPrimaryKeys().Any(p => p.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase) || p.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase));
            return result;
        }

        private List<DiffLogTableInfo> GetDiffTable(string sql, List<SugarParameter> parameters)
        {
            List<DiffLogTableInfo> result = new List<DiffLogTableInfo>();
            DataTable dt = null;
            if (this.UpdateParameterIsNull)
            {
                var whereSql = Regex.Replace(sql, ".* WHERE ", "", RegexOptions.Singleline);
                if (IsExists(sql))
                {
                    if (this.UpdateBuilder.SetValues != null)
                    {
                        foreach (var item in this.UpdateBuilder.SetValues)
                        {
                            if (item.Value?.Contains("SELECT") == true)
                            {
                                sql = sql.Replace(item.Value, null);
                            }
                        }
                    }
                    whereSql = UtilMethods.RemoveBeforeFirstWhere(sql);
                } 
                dt = this.Context.Queryable<T>().AS(this.UpdateBuilder.TableName).Filter(null, true).Where(whereSql).AddParameters(parameters).ToDataTable();
            }
            else
            {
                if (this.UpdateObjs.ToList().Count == 0)
                {
                    dt = new DataTable();
                }
                else if (this.WhereColumnList?.Any() == true) 
                { 
                    dt = this.Context.Queryable<T>().Filter(null, true).WhereClassByWhereColumns(this.UpdateObjs.ToList(), this.WhereColumnList.ToArray()).ToDataTable();
                }
                else
                {
                    dt = this.Context.Queryable<T>().Filter(null, true).WhereClassByPrimaryKey(this.UpdateObjs.ToList()).ToDataTable();
                }
            }
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
                        var sugarColumn = this.EntityInfo.Columns.Where(it => it.DbColumnName != null).First(it =>
                            it.DbColumnName.Equals(col.ColumnName, StringComparison.CurrentCultureIgnoreCase));
                        DiffLogColumnInfo addItem = new DiffLogColumnInfo();
                        addItem.Value = row[col.ColumnName];
                        addItem.ColumnName = col.ColumnName;
                        addItem.IsPrimaryKey = sugarColumn.IsPrimarykey;
                        addItem.ColumnDescription = sugarColumn.ColumnDescription;
                        item.Columns.Add(addItem);
                    }
                    result.Add(item);
                }
            }
            return result;
        }

        private static bool IsExists(string sql)
        {
            return UtilMethods.CountSubstringOccurrences(sql,"WHERE")>1;
        }

        private void ThrowUpdateByExpression()
        {
            Check.Exception(UpdateParameterIsNull == true, ErrorMessage.GetThrowMessage(" no support UpdateColumns and WhereColumns", "根据表达式更新 db.Updateable<T>() 禁止使用 UpdateColumns和WhereColumns,你可以使用 SetColumns Where 等。更新分为2种方式 1.根据表达式更新 2.根据实体或者集合更新， 具体用法请查看文档 "));
        }
        private void ThrowUpdateByObject()
        {
            Check.Exception(UpdateParameterIsNull == false, ErrorMessage.GetThrowMessage(" no support SetColumns and Where", "根据对像更新 db.Updateabe(对象) 禁止使用 SetColumns和Where ,你可以使用WhereColumns 和  UpdateColumns。 更新分为2种方式 1.根据表达式更新 2.根据实体或者集合更新 ， 具体用法请查看文档 "));
        }
        private void ThrowUpdateByExpressionByMesage(string message)
        {
            Check.Exception(UpdateParameterIsNull == true, ErrorMessage.GetThrowMessage(" no support "+ message, "根据表达式更新 db.Updateable<T>()禁止使用 " + message+"。 更新分为2种方式 1.根据表达式更新 2.根据实体或者集合更新 ， 具体用法请查看文档 "));
        }
        private void ThrowUpdateByObjectByMesage(string message)
        {
            Check.Exception(UpdateParameterIsNull == false, ErrorMessage.GetThrowMessage(" no support " + message, "根据对象更新 db.Updateable(对象)禁止使用 " + message + "。 更新分为2种方式 1.根据表达式更新 2.根据实体或者集合更新 ， 具体用法请查看文档 "));
        }
    }
}
