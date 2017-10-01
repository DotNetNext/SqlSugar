using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class InsertableProvider<T> : IInsertable<T> where T : class, new()
    {
        public SqlSugarClient Context { get; set; }
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

        #region Core
        public virtual int ExecuteCommand()
        {
            InsertBuilder.IsReturnIdentity = false;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            return Ado.ExecuteCommand(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
        }

        public virtual KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, InsertBuilder.Parameters);
        }
        public virtual int ExecuteReturnIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            return Ado.GetInt(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
        }
        public virtual long ExecuteReturnBigIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            return Convert.ToInt64( Ado.GetScalar(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()));
        }
        public T ExecuteReturnEntity()
        {
            ExecuteCommandIdentityIntoEntity();
            return InsertObjs.First();
        }
        public bool ExecuteCommandIdentityIntoEntity()
        {
            var result = InsertObjs.First();
            var identityKeys = GetIdentityKeys();
            if (identityKeys.Count == 0) { return this.ExecuteCommand() > 0; }
            var idValue = ExecuteReturnBigIdentity();
            Check.Exception(identityKeys.Count > 1, "ExecuteCommandIdentityIntoEntity does not support multiple identity keys");
            var identityKey = identityKeys.First();
            object setValue= 0;
            if (idValue > int.MaxValue)
                setValue = idValue;
            else
                setValue = Convert.ToInt32(idValue);
            this.Context.EntityMaintenance.GetProperty<T>(identityKey).SetValue(result,setValue, null);
            return idValue>0;
        }
        public Task<int> ExecuteCommandAsync()
        {
            Task<int> result = new Task<int>(() =>
            {
                IInsertable<T> asyncInsertable = CopyInsertable();
                return asyncInsertable.ExecuteCommand();
            });
            result.Start();
            return result;
        }
        public Task<int> ExecuteReturnIdentityAsync()
        {
            Task<int> result = new Task<int>(() =>
            {
                IInsertable<T> asyncInsertable = CopyInsertable();
                return asyncInsertable.ExecuteReturnIdentity();
            });
            result.Start();
            return result;
        }
        public Task<T> ExecuteReturnEntityAsync()
        {
            Task<T> result = new Task<T>(() =>
            {
                IInsertable<T> asyncInsertable = CopyInsertable();
                return asyncInsertable.ExecuteReturnEntity();
            });
            result.Start();
            return result;
        }
        public Task<bool> ExecuteCommandIdentityIntoEntityAsync()
        {
            Task<bool> result = new Task<bool>(() =>
            {
                IInsertable<T> asyncInsertable = CopyInsertable();
                return asyncInsertable.ExecuteCommandIdentityIntoEntity();
            });
            result.Start();
            return result;
        }
        public Task<long> ExecuteReturnBigIdentityAsync()
        {
            Task<long> result = new Task<long>(() =>
            {
                IInsertable<T> asyncInsertable = CopyInsertable();
                return asyncInsertable.ExecuteReturnBigIdentity();
            });
            result.Start();
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
            this.Context.MappingTables.Add(entityName, tableName);
            return this; ;
        }
        public IInsertable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = InsertBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Any(ig => ig.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            return this;
        }
        public IInsertable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod)
        {
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => !ignoreColumMethod(it.PropertyName)).ToList();
            return this;
        }

        public IInsertable<T> InsertColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = InsertBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => ignoreColumns.Any(ig=>ig.Equals(it.PropertyName,StringComparison.CurrentCultureIgnoreCase))).ToList();
            return this;
        }

        public IInsertable<T> InsertColumns(Func<string, bool> insertColumMethod)
        {
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => insertColumMethod(it.PropertyName)).ToList();
            return this;
        }

        public IInsertable<T> With(string lockString)
        {
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
                this.InsertBuilder.TableWithString = lockString;
            return this;
        }

        public IInsertable<T> Where(bool isNoInsertNull, bool isOffIdentity = false)
        {
            this.IsOffIdentity = isOffIdentity;
            if (this.InsertBuilder.LambdaExpressions == null)
                this.InsertBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.Context.CurrentConnectionConfig);
            this.InsertBuilder.IsNoInsertNull = isNoInsertNull;
            return this;
        }
        #endregion

        #region Protected Methods
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
                    insertItem.Add(columnInfo);
                }
                this.InsertBuilder.DbColumnInfoList.AddRange(insertItem);
                ++i;
            }
        }
        protected string GetDbColumnName(string entityName)
        {
            if (!IsMappingColumns)
            {
                return entityName;
            }
            if (this.Context.MappingColumns.Any(it => it.EntityName.Equals(EntityInfo.EntityName, StringComparison.CurrentCultureIgnoreCase)))
            {
                this.MappingColumnList = this.Context.MappingColumns.Where(it => it.EntityName.Equals(EntityInfo.EntityName, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            if (MappingColumnList == null || !MappingColumnList.Any())
            {
                return entityName;
            }
            else
            {
                var mappInfo = this.Context.MappingColumns.FirstOrDefault(it => it.PropertyName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
                return mappInfo == null ? entityName : mappInfo.DbColumnName;
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
        protected void RestoreMapping()
        {
            if (IsAs)
            {
                this.Context.MappingTables = OldMappingTableList;
            }
        }
        protected IInsertable<T> CopyInsertable()
        {
            var asyncContext = this.Context.Utilities.CopyContext(true);
            asyncContext.CurrentConnectionConfig.IsAutoCloseConnection = true;

            var asyncInsertable = asyncContext.Insertable<T>(this.InsertObjs);
            var asyncInsertableBuilder = asyncInsertable.InsertBuilder;
            asyncInsertableBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList;
            asyncInsertableBuilder.EntityInfo = this.InsertBuilder.EntityInfo;
            asyncInsertableBuilder.Parameters = this.InsertBuilder.Parameters;
            asyncInsertableBuilder.sql = this.InsertBuilder.sql;
            asyncInsertableBuilder.IsNoInsertNull = this.InsertBuilder.IsNoInsertNull;
            asyncInsertableBuilder.IsReturnIdentity = this.InsertBuilder.IsReturnIdentity;
            asyncInsertableBuilder.EntityInfo = this.InsertBuilder.EntityInfo;
            asyncInsertableBuilder.TableWithString = this.InsertBuilder.TableWithString;
            return asyncInsertable;
        }
        #endregion
    }
}
