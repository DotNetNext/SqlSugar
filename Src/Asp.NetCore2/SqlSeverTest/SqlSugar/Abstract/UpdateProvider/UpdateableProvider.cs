using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class UpdateableProvider<T> : IUpdateable<T> where T : class, new()
    {
        public SqlSugarClient Context { get; internal set; }
        public EntityInfo EntityInfo { get; internal set; }
        public ISqlBuilder SqlBuilder { get; internal set; }
        public UpdateBuilder UpdateBuilder { get; set; }
        public IAdo Ado { get { return Context.Ado; } }
        public T[] UpdateObjs { get; set; }
        public bool IsMappingTable { get { return this.Context.MappingTables != null && this.Context.MappingTables.Any(); } }
        public bool IsMappingColumns { get { return this.Context.MappingColumns != null && this.Context.MappingColumns.Any(); } }
        public bool IsSingle { get { return this.UpdateObjs.Length == 1; } }
        public List<MappingColumn> MappingColumnList { get; set; }
        private List<string> IgnoreColumnNameList { get; set; }
        private List<string> WhereColumnList { get; set; }
        private bool IsOffIdentity { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
        public bool IsAs { get; set; }
        public virtual int ExecuteCommand()
        {
            PreToSql();
            AutoRemoveDataCache();
            Check.Exception(UpdateBuilder.WhereValues.IsNullOrEmpty() && GetPrimaryKeys().IsNullOrEmpty(), "You cannot have no primary key and no conditions");
            string sql = UpdateBuilder.ToSqlString();
            RestoreMapping();
            return this.Ado.ExecuteCommand(sql, UpdateBuilder.Parameters == null ? null : UpdateBuilder.Parameters.ToArray());
        }
        public bool ExecuteCommandHasChange()
        {
            return this.ExecuteCommand() > 0;
        }
        public Task<int> ExecuteCommandAsync()
        {
            Task<int> result = new Task<int>(() =>
            {
                IUpdateable<T> asyncUpdateable = CopyUpdateable();
                return asyncUpdateable.ExecuteCommand();
            });
            TaskStart(result);
            return result;
        }
        public Task<bool> ExecuteCommandHasChangeAsync()
        {
            Task<bool> result = new Task<bool>(() =>
            {
                IUpdateable<T> asyncUpdateable = CopyUpdateable();
                return asyncUpdateable.ExecuteCommand() > 0;
            });
            TaskStart(result);
            return result;
        }
        public IUpdateable<T> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            IsAs = true;
            OldMappingTableList = this.Context.MappingTables;
            this.Context.MappingTables = this.Context.Utilities.TranslateCopy(this.Context.MappingTables);
            this.Context.MappingTables.Add(entityName, tableName);
            return this; ;
        }
        public IUpdateable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod)
        {
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumMethod(it.PropertyName)).ToList();
            return this;
        }

        public IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.PropertyName)).ToList();
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
            var leftExpression = (expression as BinaryExpression).Left;
            Check.Exception(!(leftExpression is MemberExpression), "Expression  format error");
            var leftResultString = UpdateBuilder.GetExpressionValue(leftExpression, ResolveExpressType.WhereSingle).GetString();
            UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(leftResultString, resultString));
            return this;
        }
        private void AutoRemoveDataCache()
        {
            var moreSetts = this.Context.CurrentConnectionConfig.MoreSettings;
            var extService = this.Context.CurrentConnectionConfig.ConfigureExternalServices;
            if (moreSetts != null && moreSetts.IsAutoRemoveDataCache && extService!=null&& extService.DataInfoCacheService!=null)
            {
                this.RemoveDataCache();
            }
        }
        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            PreToSql();
            string sql = UpdateBuilder.ToSqlString();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, UpdateBuilder.Parameters);
        }

        public IUpdateable<T> WhereColumns(Expression<Func<T, object>> columns)
        {
            var whereColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            if (this.WhereColumnList == null) this.WhereColumnList = new List<string>();
            foreach (var item in whereColumns)
            {
                this.WhereColumnList.Add(item);
            }
            return this;
        }

        public IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns)
        {
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

        public IUpdateable<T> UpdateColumns(Expression<Func<T, bool>> columns) {
            var binaryExp = columns.Body as BinaryExpression;
            Check.Exception(!binaryExp.NodeType.IsIn(ExpressionType.Equal), "No support {0}", columns.ToString());
            Check.Exception(!(binaryExp.Left is MemberExpression)&& !(binaryExp.Left is UnaryExpression), "No support {0}", columns.ToString());
            Check.Exception(ExpressionTool.IsConstExpression(binaryExp.Left as MemberExpression), "No support {0}", columns.ToString());
            var expResult = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.WhereSingle).GetResultString().Replace("))",") )").Replace("((", "( (").Trim().TrimStart('(').TrimEnd(')');
            string key = SqlBuilder.GetNoTranslationColumnName(expResult);
            UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(key), expResult));
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => UpdateBuilder.SetValues.Any(v => SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase) || SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase)) || it.IsPrimarykey == true).ToList();
            return this;
        }

        public IUpdateable<T> UpdateColumns(Func<string, bool> updateColumMethod)
        {
            List<string> primaryKeys = GetPrimaryKeys();
            foreach (var item in this.UpdateBuilder.DbColumnInfoList)
            {
                var mappingInfo = primaryKeys.SingleOrDefault(i => item.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                if (mappingInfo != null && mappingInfo.Any())
                {
                    item.IsPrimarykey = true;
                }
            }
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => updateColumMethod(it.PropertyName) || it.IsPrimarykey || it.IsIdentity).ToList();
            return this;
        }
        public IUpdateable<T> UpdateColumns(Expression<Func<T, T>> columns)
        {
            var expResult = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.Update);
            var resultArray = expResult.GetResultArray();
            Check.ArgumentNullException(resultArray, "UpdateColumns Parameter error, UpdateColumns(it=>new T{ it.id=1}) is valid, UpdateColumns(it=>T) is error");
            if (resultArray.HasValue())
            {
                foreach (var item in resultArray)
                {
                    string key = SqlBuilder.GetNoTranslationColumnName(item);
                    UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(key), item));
                }
            }
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => UpdateBuilder.SetValues.Any(v => SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.DbColumnName,StringComparison.CurrentCultureIgnoreCase)|| SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.PropertyName,StringComparison.CurrentCultureIgnoreCase)) || it.IsPrimarykey == true).ToList();
            return this;
        }

        public IUpdateable<T> Where(bool isUpdateNull, bool IsOffIdentity = false)
        {
            UpdateBuilder.IsOffIdentity = IsOffIdentity;
            if (this.UpdateBuilder.LambdaExpressions == null)
                this.UpdateBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.Context.CurrentConnectionConfig);
            this.UpdateBuilder.IsNoUpdateNull = isUpdateNull;
            return this;
        }
        public IUpdateable<T> Where(Expression<Func<T, bool>> expression)
        {
            var expResult = UpdateBuilder.GetExpressionValue(expression, ResolveExpressType.WhereSingle);
            UpdateBuilder.WhereValues.Add(expResult.GetResultString());
            return this;
        }

        public IUpdateable<T> Where(string whereSql, object parameters = null)
        {
            if (whereSql.HasValue()) {
                UpdateBuilder.WhereValues.Add(whereSql);
            }
            if (parameters != null) {
                UpdateBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            }
            return this;
        }

        public IUpdateable<T> Where(string fieldName,string conditionalType, object fieldValue)
        {
            var whereSql=this.SqlBuilder.GetWhere(fieldName, conditionalType,0);
            this.Where(whereSql);
            string parameterName = this.SqlBuilder.SqlParameterKeyWord + fieldName+ "0";
            this.UpdateBuilder.Parameters.Add(new SugarParameter(parameterName, fieldValue));
            return this;
        }

        public IUpdateable<T> With(string lockString)
        {
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
                this.UpdateBuilder.TableWithString = lockString;
            return this;
        }

        public IUpdateable<T> RemoveDataCache()
        {
            var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
            CacheSchemeMain.RemoveCache(cacheService, this.Context.EntityMaintenance.GetTableName<T>());
            return this;
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
            Check.Exception(UpdateObjs == null || UpdateObjs.Count() == 0, "UpdateObjs is null");
            int i = 0;
            foreach (var item in UpdateObjs)
            {
                List<DbColumnInfo> updateItem = new List<DbColumnInfo>();
                var isDic = item is Dictionary<string,object>;
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
        private void SetUpdateItemByDic(int i, T item, List<DbColumnInfo> updateItem)
        {
            foreach (var column in item as Dictionary<string,object>)
            {
                var columnInfo = new DbColumnInfo()
                {
                    Value = column.Value,
                    DbColumnName =column.Key,
                    PropertyName = column.Key,
                    PropertyType = UtilMethods.GetUnderType(column.Value.GetType()),
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
                var columnInfo = new DbColumnInfo()
                {
                    Value = column.PropertyInfo.GetValue(item, null),
                    DbColumnName = GetDbColumnName(column.PropertyName),
                    PropertyName = column.PropertyName,
                    PropertyType = UtilMethods.GetUnderType(column.PropertyInfo),
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

        private void PreToSql()
        {
            UpdateBuilder.PrimaryKeys = GetPrimaryKeys();
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
                    if (this.UpdateBuilder.SetValues.Any(it =>this.SqlBuilder.GetNoTranslationColumnName(it.Key) == item.PropertyName)) {
                        continue;
                    }
                    this.UpdateBuilder.Parameters.Add(new SugarParameter(this.SqlBuilder.SqlParameterKeyWord + item.DbColumnName, item.Value, item.PropertyType));
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
        private void TaskStart<Type>(Task<Type> result)
        {
            if (this.Context.CurrentConnectionConfig.IsShardSameThread)
            {
                Check.Exception(true, "IsShardSameThread=true can't be used async method");
            }
            result.Start();
        }
        private IUpdateable<T> CopyUpdateable()
        {
            var asyncContext = this.Context.Utilities.CopyContext(true);
            asyncContext.CurrentConnectionConfig.IsAutoCloseConnection = true;

            var asyncUpdateable = asyncContext.Updateable<T>(this.UpdateObjs);
            var asyncUpdateableBuilder = asyncUpdateable.UpdateBuilder;
            asyncUpdateableBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList;
            asyncUpdateableBuilder.IsNoUpdateNull = this.UpdateBuilder.IsNoUpdateNull;
            asyncUpdateableBuilder.Parameters = this.UpdateBuilder.Parameters;
            asyncUpdateableBuilder.sql = this.UpdateBuilder.sql;
            asyncUpdateableBuilder.WhereValues = this.UpdateBuilder.WhereValues;
            asyncUpdateableBuilder.TableWithString = this.UpdateBuilder.TableWithString;
            asyncUpdateableBuilder.TableName = this.UpdateBuilder.TableName;
            asyncUpdateableBuilder.PrimaryKeys = this.UpdateBuilder.PrimaryKeys;
            asyncUpdateableBuilder.IsOffIdentity = this.UpdateBuilder.IsOffIdentity;
            asyncUpdateableBuilder.SetValues = this.UpdateBuilder.SetValues;
            return asyncUpdateable;
        }
    }
}
