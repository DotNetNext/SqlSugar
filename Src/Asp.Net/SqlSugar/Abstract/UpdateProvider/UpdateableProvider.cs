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
    public class UpdateableProvider<T> : IUpdateable<T> where T : class, new()
    {
        #region Property
        public SqlSugarProvider Context { get; internal set; }
        public EntityInfo EntityInfo { get; internal set; }
        public ISqlBuilder SqlBuilder { get; internal set; }
        public UpdateBuilder UpdateBuilder { get; set; }
        public IAdo Ado { get { return Context.Ado; } }
        public T[] UpdateObjs { get; set; }
        public bool UpdateParameterIsNull { get; set; }
        public bool IsMappingTable { get { return this.Context.MappingTables != null && this.Context.MappingTables.Any(); } }
        public bool IsMappingColumns { get { return this.Context.MappingColumns != null && this.Context.MappingColumns.Any(); } }
        public bool IsSingle { get { return this.UpdateObjs.Length == 1; } }
        public List<MappingColumn> MappingColumnList { get; set; }
        private List<string> IgnoreColumnNameList { get; set; }
        private List<string> WhereColumnList { get; set; }
        private bool IsWhereColumns { get; set; }
        private bool IsOffIdentity { get; set; }
        private bool IsVersionValidation { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
        public bool IsAs { get; set; }
        public bool IsEnableDiffLogEvent { get; set; }
        public DiffLogModel diffModel { get; set; }
        private Action RemoveCacheFunc { get; set; }
        private int SetColumnsIndex { get; set; }
        #endregion

        #region Core

        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            PreToSql();
            string sql = UpdateBuilder.ToSqlString();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, UpdateBuilder.Parameters);
        }
        public void AddQueue()
        {
            var sqlObj = this.ToSql();
            this.Context.Queues.Add(sqlObj.Key, sqlObj.Value);
        }
        public virtual int ExecuteCommand()
        {
            string sql = _ExecuteCommand();
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            var result = this.Ado.ExecuteCommand(sql, UpdateBuilder.Parameters == null ? null : UpdateBuilder.Parameters.ToArray());
            After(sql);
            return result;
        }
        public bool ExecuteCommandHasChange()
        {
            return this.ExecuteCommand() > 0;
        }
        public async Task<int> ExecuteCommandAsync()
        {
            string sql = _ExecuteCommand();
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            var result = await this.Ado.ExecuteCommandAsync(sql, UpdateBuilder.Parameters == null ? null : UpdateBuilder.Parameters.ToArray());
            After(sql);
            return result;
        }
        public async Task<bool> ExecuteCommandHasChangeAsync()
        {
            return await this.ExecuteCommandAsync() > 0;
        }
        #endregion

        public IUpdateable<T> With(string lockString)
        {
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
                this.UpdateBuilder.TableWithString = lockString;
            return this;
        }
        public IUpdateable<T> RemoveDataCache()
        {
            this.RemoveCacheFunc = () =>
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                CacheSchemeMain.RemoveCache(cacheService, this.Context.EntityMaintenance.GetTableName<T>());
            };
            return this;
        }
        public IUpdateable<T> IsEnableUpdateVersionValidation()
        {
            this.IsVersionValidation = true;
            return this;
        }
        public IUpdateable<T> AS(string tableName)
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
        public IUpdateable<T> EnableDiffLogEvent(object businessData = null)
        {
            Check.Exception(this.UpdateObjs.HasValue() && this.UpdateObjs.Count() > 1, "DiffLog does not support batch operations");
            diffModel = new DiffLogModel();
            this.IsEnableDiffLogEvent = true;
            diffModel.BusinessData = businessData;
            diffModel.DiffType = DiffType.update;
            return this;
        }



        public IUpdateable<T> IgnoreColumns(bool ignoreAllNullColumns, bool isOffIdentity = false, bool ignoreAllDefaultValue = false)
        {
            Check.Exception(this.UpdateObjs.Count() > 1 && ignoreAllNullColumns, ErrorMessage.GetThrowMessage("ignoreNullColumn NoSupport batch insert", "ignoreNullColumn 不支持批量操作"));
            UpdateBuilder.IsOffIdentity = isOffIdentity;
            if (this.UpdateBuilder.LambdaExpressions == null)
                this.UpdateBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.Context.CurrentConnectionConfig);
            this.UpdateBuilder.IsNoUpdateNull = ignoreAllNullColumns;
            this.UpdateBuilder.IsNoUpdateDefaultValue = ignoreAllDefaultValue;
            return this;
        }
        public IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it).ToLower()).ToList();
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.PropertyName.ToLower())).ToList();
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.DbColumnName.ToLower())).ToList();
            return this;
        }
        public IUpdateable<T> IgnoreColumns(string[] columns)
        {
            if (columns.HasValue())
            {
                var ignoreColumns = columns.Select(it => it.ToLower()).ToList();
                this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.PropertyName.ToLower())).ToList();
                this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.DbColumnName.ToLower())).ToList();
            }
            return this;
        }


        public IUpdateable<T> ReSetValue(Expression<Func<T, bool>> setValueExpression)
        {
            Check.Exception(!IsSingle, "Batch operation not supported ReSetValue");
            var expResult = UpdateBuilder.GetExpressionValue(setValueExpression, ResolveExpressType.WhereSingle);
            var resultString = Regex.Match(expResult.GetResultString(), @"\((.+)\)").Groups[1].Value;
            LambdaExpression lambda = setValueExpression as LambdaExpression;
            var expression = lambda.Body;
            Check.Exception(!(expression is BinaryExpression), "Expression  format error");
            Check.Exception((expression as BinaryExpression).NodeType != ExpressionType.Equal, "Expression  format error");
            var leftExpression = (expression as BinaryExpression).Left;
            Check.Exception(!(leftExpression is MemberExpression), "Expression  format error");
            var leftResultString = UpdateBuilder.GetExpressionValue(leftExpression, ResolveExpressType.FieldSingle).GetString();
            UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(leftResultString, resultString));
            return this;
        }

        #region Update by object

        public IUpdateable<T> WhereColumns(Expression<Func<T, object>> columns)
        {
            ThrowUpdateByExpression();
            this.IsWhereColumns = true;
            UpdateBuilder.IsWhereColumns = true;
            Check.Exception(UpdateParameterIsNull == true, "Updateable<T>().Updateable is error,Use Updateable(obj).WhereColumns");
            var whereColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            if (this.WhereColumnList == null) this.WhereColumnList = new List<string>();
            foreach (var item in whereColumns)
            {
                this.WhereColumnList.Add(item);
            }
            return this;
        }
        public IUpdateable<T> WhereColumns(string columnName)
        {

            ThrowUpdateByExpression();
            if (this.WhereColumnList == null) this.WhereColumnList = new List<string>();
            this.WhereColumnList.Add(columnName);
            return this;
        }
        public IUpdateable<T> WhereColumns(string[] columnNames)
        {
            ThrowUpdateByExpression();
            if (this.WhereColumnList == null) this.WhereColumnList = new List<string>();
            foreach (var columnName in columnNames)
            {
                this.WhereColumnList.Add(columnName);
            }
            return this;
        }

        public IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns)
        {
            ThrowUpdateByExpression();
            var updateColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            List<string> primaryKeys = GetPrimaryKeys();
            foreach (var item in this.UpdateBuilder.DbColumnInfoList)
            {
                var mappingInfo = primaryKeys.SingleOrDefault(i => item.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                if (mappingInfo != null && mappingInfo.Any())
                {
                    item.IsPrimarykey = true;
                }
            }
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => updateColumns.Any(uc => uc.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase) || uc.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase)) || it.IsPrimarykey || it.IsIdentity).ToList();
            return this;
        }
        public IUpdateable<T> UpdateColumns(string[] columns)
        {
            ThrowUpdateByExpression();
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => GetPrimaryKeys().Select(iit => iit.ToLower()).Contains(it.DbColumnName.ToLower()) || columns.Contains(it.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();
            return this;
        }
        public IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns, Expression<Func<T, object>> columns)
        {
            ThrowUpdateByExpression();
            if (isUpdateColumns)
                UpdateColumns(columns);
            return this;
        }
        public IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns, string[] columns)
        {
            ThrowUpdateByExpression();
            if (isUpdateColumns)
                UpdateColumns(columns);
            return this;
        }
        #endregion

        #region Update by expression
        public IUpdateable<T> SetColumns(Expression<Func<T, T>> columns)
        {
            ThrowUpdateByObject();
            var expResult = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.Update);
            var resultArray = expResult.GetResultArray();
            Check.ArgumentNullException(resultArray, "UpdateColumns Parameter error, UpdateColumns(it=>new T{ it.id=1}) is valid, UpdateColumns(it=>T) is error");
            if (resultArray.HasValue())
            {
                foreach (var item in resultArray)
                {
                    string key = SqlBuilder.GetNoTranslationColumnName(item);
                    var value = item;
                    if (value.Contains("= \"SYSDATE\""))
                    {
                        value = value.Replace("= \"SYSDATE\"", "= SYSDATE");
                    }
                    UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(key), value));
                }
            }
            this.UpdateBuilder.DbColumnInfoList = UpdateBuilder.DbColumnInfoList.Where(it => (UpdateParameterIsNull == false && IsPrimaryKey(it)) || UpdateBuilder.SetValues.Any(v => SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase) || SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase)) || it.IsPrimarykey == true).ToList();
            CheckTranscodeing(false);
            AppendSets();
            return this;
        }
        public IUpdateable<T> SetColumns(Expression<Func<T, bool>> columns)
        {
            ThrowUpdateByObject();
            CheckTranscodeing();
            var binaryExp = columns.Body as BinaryExpression;
            Check.Exception(!binaryExp.NodeType.IsIn(ExpressionType.Equal), "No support {0}", columns.ToString());
            Check.Exception(!(binaryExp.Left is MemberExpression) && !(binaryExp.Left is UnaryExpression), "No support {0}", columns.ToString());
            Check.Exception(ExpressionTool.IsConstExpression(binaryExp.Left as MemberExpression), "No support {0}", columns.ToString());
            var expResult = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.WhereSingle).GetResultString().Replace(")", " )").Replace("(", "( ").Trim().TrimStart('(').TrimEnd(')');
            string key = SqlBuilder.GetNoTranslationColumnName(expResult);
            UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(key), expResult));
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => (UpdateParameterIsNull == false && IsPrimaryKey(it)) || UpdateBuilder.SetValues.Any(v => SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase) || SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase)) || it.IsPrimarykey == true).ToList();
            AppendSets();
            return this;
        }
        public IUpdateable<T> SetColumnsIF(bool isUpdateColumns, Expression<Func<T, bool>> columns)
        {
            ThrowUpdateByObject();
            if (isUpdateColumns)
                SetColumns(columns);
            return this;
        }
        public IUpdateable<T> SetColumnsIF(bool isUpdateColumns, Expression<Func<T, T>> columns)
        {
            ThrowUpdateByObject();
            if (isUpdateColumns)
                SetColumns(columns);
            return this;
        }

        public IUpdateable<T> Where(Expression<Func<T, bool>> expression)
        {
            Check.Exception(UpdateObjs.Length > 1, ErrorMessage.GetThrowMessage("update List no support where","集合更新不支持Where请使用WhereColumns"));
            var expResult = UpdateBuilder.GetExpressionValue(expression, ResolveExpressType.WhereSingle);
            var whereString = expResult.GetResultString();
            if (expression.ToString().Contains("Subqueryable()"))
            {
                whereString = whereString.Replace(this.SqlBuilder.GetTranslationColumnName(expression.Parameters.First().Name) + ".", this.SqlBuilder.GetTranslationTableName(this.EntityInfo.DbTableName) + ".");
            }
            UpdateBuilder.WhereValues.Add(whereString);
            return this;
        }
        public IUpdateable<T> Where(string whereSql, object parameters = null)
        {
            Check.Exception(UpdateObjs.Length > 1, ErrorMessage.GetThrowMessage("update List no support where", "集合更新不支持Where请使用WhereColumns"));
            if (whereSql.HasValue())
            {
                UpdateBuilder.WhereValues.Add(whereSql);
            }
            if (parameters != null)
            {
                UpdateBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            }
            return this;
        }
        public IUpdateable<T> Where(string fieldName, string conditionalType, object fieldValue)
        {
            Check.Exception(UpdateObjs.Length > 1, ErrorMessage.GetThrowMessage("update List no support where", "集合更新不支持Where请使用WhereColumns"));
            var whereSql = this.SqlBuilder.GetWhere(fieldName, conditionalType, 0);
            this.Where(whereSql);
            string parameterName = this.SqlBuilder.SqlParameterKeyWord + fieldName + "0";
            this.UpdateBuilder.Parameters.Add(new SugarParameter(parameterName, fieldValue));
            return this;
        } 
        #endregion

        #region Helper
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
            PreToSql();
            AutoRemoveDataCache();
            Check.Exception(UpdateBuilder.WhereValues.IsNullOrEmpty() && GetPrimaryKeys().IsNullOrEmpty(), "You cannot have no primary key and no conditions");
            string sql = UpdateBuilder.ToSqlString();
            ValidateVersion();
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
            foreach (var item in UpdateObjs)
            {
                List<DbColumnInfo> updateItem = new List<DbColumnInfo>();
                var isDic = item is Dictionary<string, object>;
                if (isDic)
                {
                    SetUpdateItemByDic(i, item, updateItem);
                }
                else
                {
                    SetUpdateItemByEntity(i, item, updateItem);
                }
                ++i;
            }
        }
        private void CheckTranscodeing(bool checkIsJson = true)
        {
            if (this.EntityInfo.Columns.Any(it => it.IsTranscoding))
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage("UpdateColumns no support IsTranscoding", "SetColumns方式更新不支持IsTranscoding，你可以使用db.Updateable(实体)的方式更新"));
            }
            if (checkIsJson && this.EntityInfo.Columns.Any(it => it.IsJson))
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage("UpdateColumns no support IsJson", "SetColumns方式更新不支持IsJson，你可以使用db.Updateable(实体)的方式更新"));
            }
            if (this.EntityInfo.Columns.Any(it => it.IsArray))
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage("UpdateColumns no support IsArray", "SetColumns方式更新不支持IsArray，你可以使用db.Updateable(实体)的方式更新"));
            }
        }
        private void SetUpdateItemByDic(int i, T item, List<DbColumnInfo> updateItem)
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
                    TableId = i
                };
                if (columnInfo.PropertyType.IsEnum()&& columnInfo.Value!=null)
                {
                    columnInfo.Value = Convert.ToInt64(columnInfo.Value);
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

            UpdateBuilder.PrimaryKeys = GetPrimaryKeys();
            if (this.IsWhereColumns)
            {
                foreach (var pkName in UpdateBuilder.PrimaryKeys)
                {
                    if (WhereColumnList != null&& WhereColumnList.Count()>0)
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
                foreach (var item in this.UpdateBuilder.DbColumnInfoList)
                {
                    if (this.UpdateBuilder.Parameters == null) this.UpdateBuilder.Parameters = new List<SugarParameter>();
                    if (this.UpdateBuilder.SetValues.Any(it => this.SqlBuilder.GetNoTranslationColumnName(it.Key) == item.PropertyName))
                    {
                        continue;
                    }
                    var parameter = new SugarParameter(this.SqlBuilder.SqlParameterKeyWord + item.DbColumnName, item.Value, item.PropertyType);
                    if (item.IsJson)
                    {
                        parameter.IsJson = true;
                    }
                    if (item.IsArray)
                    {
                        parameter.IsArray = true;
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
                    conModels.Add(new ConditionalModel() { FieldName = item.DbColumnName, ConditionalType = ConditionalType.Equal, FieldValue = item.Value.ObjToString() });
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
            if (this.IsEnableDiffLogEvent)
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

        private void Before(string sql)
        {
            if (this.IsEnableDiffLogEvent)
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
            var whereSql = Regex.Replace(sql, ".* WHERE ", "", RegexOptions.Singleline);
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
                        addItem.ColumnDescription = this.EntityInfo.Columns.Where(it => it.DbColumnName != null).First(it => it.DbColumnName.Equals(col.ColumnName, StringComparison.CurrentCultureIgnoreCase)).ColumnDescription;
                        item.Columns.Add(addItem);
                    }
                    result.Add(item);
                }
            }
            return result;
        }

        private void ThrowUpdateByExpression()
        {
            Check.Exception(UpdateParameterIsNull == true, ErrorMessage.GetThrowMessage("no support SetColumns and Where ", "根据对象进行更新 db.Updateable(现有集合对象) 禁止使用 SetColumns和Where,你可以使用 WhereColumns UpdateColumns 等。更新分为2种方式 1.根据表达式更新 2.根据实体或者集合更新， 具体用法请查看文档 "));
        }
        private void ThrowUpdateByObject()
        {
            Check.Exception(UpdateParameterIsNull == false, ErrorMessage.GetThrowMessage("no support UpdateColumns and WhereColumns ", "根据表达式进行更新 禁止使用 UpdateColumns和WhereColumns ,你可以使用SetColumns 和 Where。 更新分为2种方式 1.根据表达式更新 2.根据实体或者集合更新 ， 具体用法请查看文档 "));
        }
        #endregion
    }
}
