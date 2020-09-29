using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class DeleteableProvider<T> : IDeleteable<T> where T : class, new()
    {
        public ISqlSugarClient Context { get; set; }
        public IAdo Db { get { return Context.Ado; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
        public bool IsAs { get; set; }
        public bool IsEnableDiffLogEvent { get; set; }
        public DiffLogModel diffModel { get; set; }
        public List<string> tempPrimaryKeys { get; set; }
        private Action RemoveCacheFunc { get; set; }
        public EntityInfo EntityInfo
        {
            get
            {
                return this.Context.EntityMaintenance.GetEntityInfo<T>();
            }
        }
        public void AddQueue()
        {
            var sqlObj = this.ToSql();
            this.Context.Queues.Add(sqlObj.Key, sqlObj.Value);
        }
        public int ExecuteCommand()
        {
            string sql;
            SugarParameter[] paramters;
            _ExecuteCommand(out sql, out paramters);
            var result = Db.ExecuteCommand(sql, paramters);
            After(sql);
            return result;
        }
        public bool ExecuteCommandHasChange()
        {
            return ExecuteCommand() > 0;
        }
        public async Task<int> ExecuteCommandAsync()
        {
            string sql;
            SugarParameter[] paramters;
            _ExecuteCommand(out sql, out paramters);
            var result =await Db.ExecuteCommandAsync(sql, paramters);
            After(sql);
            return result;
        }
        public async Task<bool> ExecuteCommandHasChangeAsync()
        {
            return await ExecuteCommandAsync() > 0;
        }
        public IDeleteable<T> AS(string tableName)
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

        public IDeleteable<T> EnableDiffLogEvent(object businessData = null)
        {

            diffModel = new DiffLogModel();
            this.IsEnableDiffLogEvent = true;
            diffModel.BusinessData = businessData;
            diffModel.DiffType = DiffType.delete;
            return this;
        }

        public IDeleteable<T> Where(List<T> deleteObjs)
        {
            if (deleteObjs == null || deleteObjs.Count() == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            string tableName = this.Context.EntityMaintenance.GetTableName<T>();
            var primaryFields = this.GetPrimaryKeys();
            var isSinglePrimaryKey = primaryFields.Count == 1;
            Check.Exception(primaryFields.IsNullOrEmpty(), string.Format("Table {0} with no primarykey", tableName));
            if (isSinglePrimaryKey)
            {
                List<object> primaryKeyValues = new List<object>();
                var primaryField = primaryFields.Single();
                foreach (var deleteObj in deleteObjs)
                {
                    var entityPropertyName = this.Context.EntityMaintenance.GetPropertyName<T>(primaryField);
                    var columnInfo = EntityInfo.Columns.Single(it => it.PropertyName.Equals(entityPropertyName, StringComparison.CurrentCultureIgnoreCase));
                    var value = columnInfo.PropertyInfo.GetValue(deleteObj, null);
                    primaryKeyValues.Add(value);
                }
                if (primaryKeyValues.Count < 10000)
                {
                    var inValueString = primaryKeyValues.ToArray().ToJoinSqlInVals();
                    Where(string.Format(DeleteBuilder.WhereInTemplate, SqlBuilder.GetTranslationColumnName(primaryFields.Single()), inValueString));
                }
                else
                {
                    if (DeleteBuilder.BigDataInValues == null)
                        DeleteBuilder.BigDataInValues = new List<object>();
                    DeleteBuilder.BigDataInValues.AddRange(primaryKeyValues);
                    DeleteBuilder.BigDataFiled = primaryField;
                }
            }
            else
            {
                StringBuilder whereInSql = new StringBuilder();
                foreach (var deleteObj in deleteObjs)
                {
                    StringBuilder orString = new StringBuilder();
                    var isFirst = deleteObjs.IndexOf(deleteObj) == 0;
                    if (!isFirst)
                    {
                        orString.Append(DeleteBuilder.WhereInOrTemplate + UtilConstants.Space);
                    }
                    int i = 0;
                    StringBuilder andString = new StringBuilder();
                    foreach (var primaryField in primaryFields)
                    {
                        if (i != 0)
                            andString.Append(DeleteBuilder.WhereInAndTemplate + UtilConstants.Space);
                        var entityPropertyName = this.Context.EntityMaintenance.GetPropertyName<T>(primaryField);
                        var columnInfo = EntityInfo.Columns.Single(it => it.PropertyName == entityPropertyName);
                        var entityValue = columnInfo.PropertyInfo.GetValue(deleteObj, null);
                        if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                        {
                            andString.AppendFormat(DeleteBuilder.WhereInEqualTemplate, primaryField.ToUpper(), entityValue);
                        }
                        else
                        {
                            andString.AppendFormat(DeleteBuilder.WhereInEqualTemplate, primaryField, entityValue);
                        }
                        ++i;
                    }
                    orString.AppendFormat(DeleteBuilder.WhereInAreaTemplate, andString);
                    whereInSql.Append(orString);
                }
                Where(string.Format(DeleteBuilder.WhereInAreaTemplate, whereInSql.ToString()));
            }
            return this;
        }

        public IDeleteable<T> Where(Expression<Func<T, bool>> expression)
        {
            var expResult = DeleteBuilder.GetExpressionValue(expression, ResolveExpressType.WhereSingle);
            var whereString = expResult.GetResultString();
            if (expression.ToString().Contains("Subqueryable()")){
                whereString = whereString.Replace(this.SqlBuilder.GetTranslationColumnName(expression.Parameters.First().Name) + ".",this.SqlBuilder.GetTranslationTableName(this.EntityInfo.DbTableName) + ".");
            }
            DeleteBuilder.WhereInfos.Add(whereString);
            return this;
        }

        public IDeleteable<T> Where(T deleteObj)
        {
            Check.Exception(GetPrimaryKeys().IsNullOrEmpty(), "Where(entity) Primary key required");
            Where(new List<T>() { deleteObj });
            return this;
        }

        public IDeleteable<T> Where(string whereString, object parameters = null)
        {
            DeleteBuilder.WhereInfos.Add(whereString);
            if (parameters != null)
            {
                if (DeleteBuilder.Parameters == null)
                {
                    DeleteBuilder.Parameters = new List<SugarParameter>();
                }
                DeleteBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            }
            return this;
        }

        public IDeleteable<T> Where(string whereString, SugarParameter parameter)
        {
            DeleteBuilder.WhereInfos.Add(whereString);
            if (DeleteBuilder.Parameters == null)
            {
                DeleteBuilder.Parameters = new List<SugarParameter>();
            }
            DeleteBuilder.Parameters.Add(parameter);
            return this;
        }
        public IDeleteable<T> Where(string whereString, SugarParameter[] parameters)
        {
            DeleteBuilder.WhereInfos.Add(whereString);
            if (DeleteBuilder.Parameters == null)
            {
                DeleteBuilder.Parameters = new List<SugarParameter>();
            }
            DeleteBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public IDeleteable<T> Where(string whereString, List<SugarParameter> parameters)
        {
            DeleteBuilder.WhereInfos.Add(whereString);
            if (DeleteBuilder.Parameters == null)
            {
                DeleteBuilder.Parameters = new List<SugarParameter>();
            }
            DeleteBuilder.Parameters.AddRange(parameters);
            return this;
        }

        public IDeleteable<T> RemoveDataCache()
        {
            this.RemoveCacheFunc = () =>
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                CacheSchemeMain.RemoveCache(cacheService, this.Context.EntityMaintenance.GetTableName<T>());
            };
            return this;
        }

        public IDeleteable<T> In<PkType>(List<PkType> primaryKeyValues)
        {
            if (primaryKeyValues == null || primaryKeyValues.Count() == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In<PkType>(primaryKeyValues.ToArray());
        }

        public IDeleteable<T> In<PkType>(PkType[] primaryKeyValues)
        {
            if (primaryKeyValues == null || primaryKeyValues.Count() == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            string tableName = this.Context.EntityMaintenance.GetTableName<T>();
            string primaryField = null;
            primaryField = GetPrimaryKeys().FirstOrDefault();
            Check.ArgumentNullException(primaryField, "Table " + tableName + " with no primarykey");
            if (primaryKeyValues.Length < 10000)
            {
                Where(string.Format(DeleteBuilder.WhereInTemplate, SqlBuilder.GetTranslationColumnName(primaryField), primaryKeyValues.ToJoinSqlInVals()));
            }
            else
            {
                if (DeleteBuilder.BigDataInValues == null)
                    DeleteBuilder.BigDataInValues = new List<object>();
                DeleteBuilder.BigDataInValues.AddRange(primaryKeyValues.Select(it => (object)it));
                DeleteBuilder.BigDataFiled = primaryField;
            }
            return this;
        }

        public IDeleteable<T> In<PkType>(PkType primaryKeyValue)
        {
            if (typeof(PkType).FullName.IsCollectionsList())
            {
                var newValues = new List<object>();
                foreach (var item in primaryKeyValue as IEnumerable)
                {
                    newValues.Add(item);
                }
                return In(newValues);
            }


            In(new PkType[] { primaryKeyValue });
            return this;
        }

        public IDeleteable<T> In<PkType>(Expression<Func<T, object>> inField, PkType primaryKeyValue)
        {
            var lamResult = DeleteBuilder.GetExpressionValue(inField, ResolveExpressType.FieldSingle);
            var fieldName = lamResult.GetResultString();
            tempPrimaryKeys = new List<string>() { fieldName };
            var result = In(primaryKeyValue);;
            tempPrimaryKeys = null;
            return this;
        }
        public IDeleteable<T> In<PkType>(Expression<Func<T, object>> inField, PkType[] primaryKeyValues)
        {
            var lamResult = DeleteBuilder.GetExpressionValue(inField, ResolveExpressType.FieldSingle);
            var fieldName = lamResult.GetResultString();
            tempPrimaryKeys = new List<string>() { fieldName };
            var result = In(primaryKeyValues);
            tempPrimaryKeys = null;
            return this;
        }
        public IDeleteable<T> In<PkType>(Expression<Func<T, object>> inField, List<PkType> primaryKeyValues)
        {
            var lamResult = DeleteBuilder.GetExpressionValue(inField, ResolveExpressType.FieldSingle);
            var fieldName = lamResult.GetResultString();
            tempPrimaryKeys = new List<string>() { fieldName };
            var result = In(primaryKeyValues);
            tempPrimaryKeys = null;
            return this;
        }

        public IDeleteable<T> With(string lockString)
        {
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
                DeleteBuilder.TableWithString = lockString;
            return this;
        }

        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            DeleteBuilder.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            string sql = DeleteBuilder.ToSqlString();
            var paramters = DeleteBuilder.Parameters == null ? null : DeleteBuilder.Parameters.ToList();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, paramters);
        }

        private List<string> GetPrimaryKeys()
        {
            if (tempPrimaryKeys.HasValue())
            {
                return tempPrimaryKeys;
            }
            else if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetPrimaries(this.Context.EntityMaintenance.GetTableName(this.EntityInfo.EntityName));
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            }
        }
        private void _ExecuteCommand(out string sql, out SugarParameter[] paramters)
        {
            DeleteBuilder.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            sql = DeleteBuilder.ToSqlString();
            paramters = DeleteBuilder.Parameters == null ? null : DeleteBuilder.Parameters.ToArray();
            RestoreMapping();
            AutoRemoveDataCache();
            Before(sql);
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

        //private void TaskStart<Type>(Task<Type> result)
        //{
        //    if (this.Context.CurrentConnectionConfig.IsShardSameThread)
        //    {
        //        Check.Exception(true, "IsShardSameThread=true can't be used async method");
        //    }
        //    result.Start();
        //}

        private void AutoRemoveDataCache()
        {
            var moreSetts = this.Context.CurrentConnectionConfig.MoreSettings;
            var extService = this.Context.CurrentConnectionConfig.ConfigureExternalServices;
            if (moreSetts != null && moreSetts.IsAutoRemoveDataCache && extService != null && extService.DataInfoCacheService != null)
            {
                this.RemoveDataCache();
            }
        }


        private void After(string sql)
        {
            if (this.IsEnableDiffLogEvent)
            {
                var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
                this.Context.Ado.IsDisableMasterSlaveSeparation = true;
                var parameters = DeleteBuilder.Parameters;
                if (parameters == null)
                    parameters = new List<SugarParameter>();
                diffModel.AfterData = null;
                diffModel.Time = this.Context.Ado.SqlExecutionTime;
                if (this.Context.CurrentConnectionConfig.AopEvents.OnDiffLogEvent != null)
                    this.Context.CurrentConnectionConfig.AopEvents.OnDiffLogEvent(diffModel);
                this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
            }
            if (this.RemoveCacheFunc != null) {
                this.RemoveCacheFunc();
            }
        }

        private void Before(string sql)
        {
            if (this.IsEnableDiffLogEvent)
            {
                var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
                this.Context.Ado.IsDisableMasterSlaveSeparation = true;
                var parameters = DeleteBuilder.Parameters;
                if (parameters == null)
                    parameters = new List<SugarParameter>();
                diffModel.BeforeData = GetDiffTable(sql, parameters);
                diffModel.Sql = sql;
                diffModel.Parameters = parameters.ToArray();
                this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
            }
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
                        addItem.ColumnDescription = this.EntityInfo.Columns.First(it => it.DbColumnName.Equals(col.ColumnName, StringComparison.CurrentCultureIgnoreCase)).ColumnDescription;
                        item.Columns.Add(addItem);
                    }
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
