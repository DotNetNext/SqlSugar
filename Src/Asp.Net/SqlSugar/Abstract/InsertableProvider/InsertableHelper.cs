using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class InsertableProvider<T> : IInsertable<T> where T : class, new()
    {
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
                        if (StaticConfig.CustomGuidFunc != null)
                        {
                            item.Value = StaticConfig.CustomGuidFunc();
                        }
                        else
                        {
                            item.Value = Guid.NewGuid();
                        }
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
                var isDic = this.EntityInfo.DbTableName.StartsWith("Dictionary`");
                foreach (var item in this.InsertBuilder.DbColumnInfoList)
                {
                    if (this.InsertBuilder.Parameters == null) this.InsertBuilder.Parameters = new List<SugarParameter>();
                    var paramters = new SugarParameter(this.SqlBuilder.SqlParameterKeyWord + item.DbColumnName, item.Value, item.PropertyType);
                    if (InsertBuilder.IsNoInsertNull && paramters.Value == null)
                    {
                        continue;
                    }
                    if (item.SqlParameterDbType is Type) 
                    {
                        continue;
                    }
                    if (item.IsJson)
                    {
                        paramters.IsJson = true;
                        SqlBuilder.ChangeJsonType(paramters);
                    }
                    if (item.IsArray)
                    {
                        paramters.IsArray = true;
                        if (item.Value == null || item.Value == DBNull.Value) 
                        {
                            ArrayNull(item,paramters);
                        }

                    }
                    if (item.Value == null && isDic)
                    {
                        var type = this.SqlBuilder.GetNullType(this.InsertBuilder.GetTableNameString, item.DbColumnName);
                        if (type != null)
                        {
                            paramters = new SugarParameter(this.SqlBuilder.SqlParameterKeyWord + item.DbColumnName, item.Value, type);

                        }
                    }
                    this.InsertBuilder.Parameters.Add(paramters);
                }
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
        internal void Init()
        {
            InsertBuilder.EntityInfo = this.EntityInfo;
            Check.Exception(InsertObjs == null || InsertObjs.Count() == 0, "InsertObjs is null");
            int i = 0;
            foreach (var item in InsertObjs)
            {
                List<DbColumnInfo> insertItem = new List<DbColumnInfo>();
                if (item is Dictionary<string, string>)
                {
                    Check.ExceptionEasy("To use Insertable dictionary, use string or object", "Insertable字典请使用string,object类型");
                }
                if (item is Dictionary<string, object>)
                {
                    SetInsertItemByDic(i, item, insertItem);
                }
                else
                {
                    DataAop(item);
                    SetInsertItemByEntity(i, item, insertItem);
                }
                this.InsertBuilder.DbColumnInfoList.AddRange(insertItem);
                ++i;
            }
        }

        private void DataAop(T item)
        {
            var dataEvent = this.Context.CurrentConnectionConfig.AopEvents?.DataExecuting;
            if (dataEvent != null && item != null)
            {
                foreach (var columnInfo in this.EntityInfo.Columns)
                {
                    dataEvent(columnInfo.PropertyInfo.GetValue(item, null), new DataFilterModel() { OperationType = DataFilterType.InsertByObject, EntityValue = item, EntityColumnInfo = columnInfo });
                }
            }
        }

        private void SetInsertItemByDic(int i, T item, List<DbColumnInfo> insertItem)
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
                if (columnInfo.PropertyType?.FullName == "System.Text.Json.JsonElement") 
                {
                    columnInfo.Value = column.Value.ObjToString(); 
                }
                if (columnInfo.PropertyType.IsEnum())
                {
                    if (this.Context.CurrentConnectionConfig.MoreSettings?.TableEnumIsString == true)
                    {
                        columnInfo.Value = columnInfo.Value.ToString();
                        columnInfo.PropertyType = UtilConstants.StringType;
                    }
                    else
                    {
                        columnInfo.Value = Convert.ToInt64(columnInfo.Value);
                    }
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
                var isMapping = IsMappingColumns;
                var columnInfo = new DbColumnInfo()
                {
                    Value = GetValue(item,column),
                    DbColumnName = column.DbColumnName,
                    PropertyName = column.PropertyName,
                    PropertyType = UtilMethods.GetUnderType(column.PropertyInfo),
                    TableId = i,
                    InsertSql = column.InsertSql,
                    InsertServerTime = column.InsertServerTime,
                    DataType=column.DataType,
                    SqlParameterDbType= column.SqlParameterDbType ,
                    IsIdentity= column.IsIdentity
                     
                };
                if (column.DbColumnName == null)
                {
                    column.DbColumnName = column.PropertyName;
                }
                if (isMapping)
                {
                    columnInfo.DbColumnName = GetDbColumnName(column.PropertyName);
                }
                if (column.IsJson)
                {
                    columnInfo.IsJson = true;
                }
                if (column.IsArray)
                {
                    columnInfo.IsArray = true;
                }
                if (columnInfo.PropertyType.IsEnum() && columnInfo.Value != null)
                {
                    if (this.Context.CurrentConnectionConfig.MoreSettings?.TableEnumIsString == true)
                    {
                        columnInfo.Value = columnInfo.Value.ToString();
                        columnInfo.PropertyType = UtilConstants.StringType;
                    }
                    else
                    {
                        columnInfo.Value = Convert.ToInt64(columnInfo.Value);
                    }
                }
                if (column.IsJson && columnInfo.Value != null)
                {
                    if (columnInfo.Value != null)
                        columnInfo.Value = this.Context.Utilities.SerializeObject(columnInfo.Value);
                }
                //var tranColumn=EntityInfo.Columns.FirstOrDefault(it => it.IsTranscoding && it.DbColumnName.Equals(column.DbColumnName, StringComparison.CurrentCultureIgnoreCase));
                if (column.IsTranscoding && columnInfo.Value.HasValue())
                {
                    columnInfo.Value = UtilMethods.EncodeBase64(columnInfo.Value.ToString());
                }
                insertItem.Add(columnInfo);
            }
            if (EntityInfo.Discrimator.HasValue()) 
            {
                Check.ExceptionEasy(!Regex.IsMatch(EntityInfo.Discrimator, @"^(?:\w+:\w+)(?:,\w+:\w+)*$"), "The format should be type:cat for this type, and if there are multiple, it can be FieldName:cat,FieldName2:dog ", "格式错误应该是type:cat这种格式，如果是多个可以FieldName:cat,FieldName2:dog，不要有空格");
                var array = EntityInfo.Discrimator.Split(',');
                foreach (var disItem in array)
                {
                    var name = disItem.Split(':').First();
                    var value = disItem.Split(':').Last();
                    insertItem.Add(new DbColumnInfo() { DbColumnName = name, PropertyName = name, PropertyType = typeof(string), Value = value });
                }
            }
        }
        private static object GetValue(T item, EntityColumnInfo column)
        {
            if (StaticConfig.EnableAot)
            {
                return column.PropertyInfo.GetValue(item, null);
            }
            else
            {
                return PropertyCallAdapterProvider<T>.GetInstance(column.PropertyName).InvokeGet(item);
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
                return this.EntityInfo.Columns.Where(it => {
                    
                    if (StaticConfig.Check_StringIdentity)
                    {
                        Check.ExceptionEasy(it.IsIdentity && it.UnderType == typeof(string), "Auto-incremented is not a string, how can I use a executable startup configuration: StaticConfig.Check_StringIdentity=false ", "自增不是能string,如何非要用可以程序启动配置：StaticConfig.Check_StringIdentity=false");
                    }
                    return it.IsIdentity;

                }).Select(it => it.DbColumnName).ToList();
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

        protected void After(string sql, long? result)
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
        protected void Before(string sql)
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

            if (GetIdentityKeys().HasValue() && this.InsertObjs.Count() > 1)
            {
                return GetDiffTableByEntity();
            }
            else if (GetIdentityKeys().IsNullOrEmpty())
            {
                return GetDiffTableByEntity();
            }
            else
            {
                return GetDiffTableBySql(identity);
            }

        }

        private List<DiffLogTableInfo> GetDiffTableByEntity()
        {
            List<SugarParameter> parameters = new List<SugarParameter>();
            List<DiffLogTableInfo> result = new List<DiffLogTableInfo>();
            var dt2 = this.Context.Utilities.ListToDataTable<T>(this.InsertObjs.ToList());
            foreach (DataRow row in dt2.Rows)
            {
                DiffLogTableInfo item = new DiffLogTableInfo();
                item.TableDescription = this.EntityInfo.TableDescription;
                item.TableName = this.EntityInfo.DbTableName;
                item.Columns = new List<DiffLogColumnInfo>();
                foreach (DataColumn col in dt2.Columns)
                {
                    var sugarColumn = this.EntityInfo.Columns.Where(it => it.DbColumnName != null).FirstOrDefault(it =>
                        it.PropertyName.Equals(col.ColumnName, StringComparison.CurrentCultureIgnoreCase));
                    DiffLogColumnInfo addItem = new DiffLogColumnInfo();
                    addItem.Value = row[col.ColumnName];
                    addItem.ColumnName = sugarColumn?.DbColumnName??col.ColumnName;
                    addItem.IsPrimaryKey = sugarColumn?.IsPrimarykey ?? false;
                    addItem.ColumnDescription = sugarColumn?.ColumnDescription;
                    item.Columns.Add(addItem);
                }
                result.Add(item);
            }
            return result;
        }

        private List<DiffLogTableInfo> GetDiffTableBySql(long? identity)
        {
            List<SugarParameter> parameters = new List<SugarParameter>();
            List<DiffLogTableInfo> result = new List<DiffLogTableInfo>();
            var whereSql = string.Empty;
            List<IConditionalModel> cons = new List<IConditionalModel>();
            if (identity != null && identity > 0 && GetIdentityKeys().HasValue())
            {
                var fieldName = GetIdentityKeys().Last();

                if (this.Context.CurrentConnectionConfig.DbType == DbType.PostgreSQL)
                {
                    var fieldObjectType = this.EntityInfo.Columns.FirstOrDefault(x => x.DbColumnName == fieldName)
                        .PropertyInfo.PropertyType;
                    cons.Add(new ConditionalModel()
                    {
                        ConditionalType = ConditionalType.Equal,
                        FieldName = fieldName,
                        FieldValue = identity.ToString(),
                        FieldValueConvertFunc = it => UtilMethods.ChangeType2(it, fieldObjectType)
                    });
                }
                else
                    cons.Add(new ConditionalModel() { ConditionalType = ConditionalType.Equal, FieldName = fieldName, FieldValue = identity.ToString() });
            }
            else
            {
                foreach (var item in this.EntityInfo.Columns.Where(it => it.IsIgnore == false && GetPrimaryKeys().Any(pk => pk.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase))))
                {
                    var fielddName = item.DbColumnName;
                    var filedObject = this.EntityInfo.Columns.FirstOrDefault(it => it.PropertyName == item.PropertyName).PropertyInfo.GetValue(this.InsertObjs.Last(), null);
                    var fieldValue = filedObject.ObjToString();
                    if (filedObject != null && filedObject.GetType() != typeof(string) && this.Context.CurrentConnectionConfig.DbType == DbType.PostgreSQL)
                    {
                        cons.Add(new ConditionalModel() { ConditionalType = ConditionalType.Equal, FieldName = fielddName, FieldValue = fieldValue, FieldValueConvertFunc = it => UtilMethods.ChangeType2(it, filedObject.GetType()) });
                    }
                    else
                    {
                        cons.Add(new ConditionalModel() { ConditionalType = ConditionalType.Equal, FieldName = fielddName, FieldValue = fieldValue });
                    }
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
                    Value = it.PropertyInfo.GetValue(this.InsertObjs.Last(), null),
                    IsPrimaryKey = it.IsPrimarykey
                }).ToList();
                return new List<DiffLogTableInfo>() { diffTable };
            }
        }

        public IInsertable<T> CallEntityMethod(Expression<Action<T>> method)
        {
            if (this.InsertObjs.HasValue())
            {
                var oldColumns = this.InsertBuilder.DbColumnInfoList.Select(it => it.PropertyName).ToList();
                var expression = (LambdaExpression.Lambda(method).Body as LambdaExpression).Body;
                Check.Exception(!(expression is MethodCallExpression), method.ToString() + " is not method");
                var callExpresion = expression as MethodCallExpression;
                UtilMethods.DataInoveByExpresson(this.InsertObjs, callExpresion);
                this.InsertBuilder.DbColumnInfoList = new List<DbColumnInfo>();
                Init();
                this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => oldColumns.Contains(it.PropertyName)).ToList();
            }
            return this;
        }

        #endregion

        #region Insert PkList

        private List<Type> InsertPkListWithFunc<Type>(EntityColumnInfo pkInfo)
        {
            InsertBuilder.IsReturnPkList = true;
            InsertBuilder.IsNoPage = true;
            string sql = _ExecuteCommand();
            sql = this.InsertBuilder.ConvertInsertReturnIdFunc(SqlBuilder.GetTranslationColumnName(pkInfo.DbColumnName), sql);
            var result = Ado.SqlQuery<Type>(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            After(sql, null);
            return result;
        }

        private List<Type> InsertPkListNoFunc<Type>(EntityColumnInfo pkInfo)
        {
            if (this.Ado.Transaction != null)
            {
                return ReturnDefaultIdentity<Type>(pkInfo);
            }
            else
            {
                try
                {
                    this.Context.Ado.BeginTran();
                    var result = ReturnDefaultIdentity<Type>(pkInfo);
                    this.Context.Ado.CommitTran();
                    return result;
                }
                catch (Exception ex)
                {
                    this.Context.Ado.RollbackTran();
                    throw ex;
                }
            }
        }

        private List<Type> InsertPkListIdentityCount1<Type>(EntityColumnInfo pkInfo)
        {
            if (pkInfo.UnderType == UtilConstants.IntType)
            {
                return new List<Type> { (Type)(object)this.ExecuteReturnIdentity() };
            }
            else
            {
                return new List<Type> { (Type)(object)this.ExecuteReturnBigIdentity() };
            }
        }

        private List<Type> InsertPkListLong<Type>()
        {
            var list = this.ExecuteReturnSnowflakeIdList();
            try
            {
                return list.Cast<Type>().ToList();
            }
            catch
            {
                Check.ExceptionEasy($"long to ExecuteReturnPkList<{typeof(Type).Name}> error ", $" long 转换成ExecuteReturnPkList<{typeof(Type).Name}>失败");
                return null;
            }
        }

        private List<Type> InsertPkListGuid<Type>(EntityColumnInfo pkInfo)
        {
            Check.ExceptionEasy(pkInfo.UnderType.Name != typeof(Type).Name, $"{pkInfo.UnderType.Name} to ExecuteReturnPkList<{typeof(Type).Name}> error ", $" {pkInfo.UnderType.Name} 转换成ExecuteReturnPkList<{typeof(Type).Name}>失败");
            this.ExecuteCommand();
            List<Type> result = new List<Type>();
            if (InsertBuilder.DbColumnInfoList.HasValue())
            {
                foreach (var item in InsertBuilder.DbColumnInfoList)
                {
                    var isPk = item.DbColumnName.EqualCase(pkInfo.DbColumnName);
                    if (isPk)
                    {
                        result.Add((Type)item.Value);
                    }
                }
            }
            return result;
        }
        private List<Type> ReturnDefaultIdentity<Type>(EntityColumnInfo pkInfo)
        {
            List<Type> result = new List<Type>();
            foreach (var item in this.InsertObjs)
            {
                var insertable = this.Context.Insertable(item)
                    .InsertColumns(this.InsertBuilder.DbColumnInfoList.Select(it => it.DbColumnName).Distinct().ToArray());
                if (pkInfo.UnderType == UtilConstants.IntType)
                {
                    result.Add((Type)(object)insertable.ExecuteReturnIdentity());
                }
                else
                {
                    result.Add((Type)(object)insertable.ExecuteReturnBigIdentity());
                }
            }
            return result;
        }
        #endregion
    }
}
