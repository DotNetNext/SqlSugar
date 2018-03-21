using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class DeleteableProvider<T> : IDeleteable<T> where T : class, new()
    {
        public SqlSugarClient Context { get; set; }
        public IAdo Db { get { return Context.Ado; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
        public bool IsAs { get; set; }
        public EntityInfo EntityInfo
        {
            get
            {
                return this.Context.EntityMaintenance.GetEntityInfo<T>();
            }
        }
        public int ExecuteCommand()
        {
            DeleteBuilder.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            string sql = DeleteBuilder.ToSqlString();
            var paramters = DeleteBuilder.Parameters == null ? null : DeleteBuilder.Parameters.ToArray();
            RestoreMapping();
            AutoRemoveDataCache();
            return Db.ExecuteCommand(sql, paramters);
        }
        public bool ExecuteCommandHasChange()
        {
            return ExecuteCommand() > 0;
        }
        public Task<int> ExecuteCommandAsync()
        {
            Task<int> result = new Task<int>(() =>
            {
                IDeleteable<T> asyncDeleteable = CopyDeleteable();
                return asyncDeleteable.ExecuteCommand();
            });
            TaskStart(result);
            return result;
        }

        public Task<bool> ExecuteCommandHasChangeAsync()
        {
            Task<bool> result = new Task<bool>(() =>
            {
                IDeleteable<T> asyncDeleteable = CopyDeleteable();
                return asyncDeleteable.ExecuteCommand() > 0;
            });
            TaskStart(result);
            return result;
        }
        public IDeleteable<T> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            IsAs = true;
            OldMappingTableList = this.Context.MappingTables;
            this.Context.MappingTables = this.Context.Utilities.TranslateCopy(this.Context.MappingTables);
            this.Context.MappingTables.Add(entityName, tableName);
            return this; ;
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
            DeleteBuilder.WhereInfos.Add(expResult.GetResultString());
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
            DeleteBuilder.Parameters.Add(parameter);
            return this;
        }
        public IDeleteable<T> Where(string whereString, SugarParameter[] parameters)
        {
            DeleteBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public IDeleteable<T> Where(string whereString, List<SugarParameter> parameters)
        {
            DeleteBuilder.Parameters.AddRange(parameters);
            return this;
        }

        public IDeleteable<T> RemoveDataCache()
        {
            var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
            CacheSchemeMain.RemoveCache(cacheService, this.Context.EntityMaintenance.GetTableName<T>());
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
            In(new PkType[] { primaryKeyValue });
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
            if (this.Context.CurrentConnectionConfig.IsShardSameThread) {
                Check.Exception(true, "IsShardSameThread=true can't be used async method");
            }
            result.Start();
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


        private IDeleteable<T> CopyDeleteable()
        {
            var asyncContext = this.Context.Utilities.CopyContext(true);
            asyncContext.CurrentConnectionConfig.IsAutoCloseConnection = true;

            var asyncDeleteable = asyncContext.Deleteable<T>();
            var asyncDeleteBuilder = asyncDeleteable.DeleteBuilder;
            asyncDeleteBuilder.BigDataFiled = this.DeleteBuilder.BigDataFiled;
            asyncDeleteBuilder.BigDataInValues = this.DeleteBuilder.BigDataInValues;
            asyncDeleteBuilder.Parameters = this.DeleteBuilder.Parameters;
            asyncDeleteBuilder.sql = this.DeleteBuilder.sql;
            asyncDeleteBuilder.WhereInfos = this.DeleteBuilder.WhereInfos;
            asyncDeleteBuilder.TableWithString = this.DeleteBuilder.TableWithString;
            return asyncDeleteable;
        }
    }
}
