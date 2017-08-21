using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class UpdateableProvider<T> : IUpdateable<T>
    {
        public SqlSugarClient Context { get; internal set; }
        public EntityInfo EntityInfo { get; internal set; }
        public ISqlBuilder SqlBuilder { get; internal set; }
        public UpdateBuilder UpdateBuilder { get; internal set; }
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
        public int ExecuteCommand()
        {
            PreToSql();
            string sql = UpdateBuilder.ToSqlString();
            RestoreMapping();
            return this.Ado.ExecuteCommand(sql, UpdateBuilder.Parameters == null ? null : UpdateBuilder.Parameters.ToArray());
        }
        public IUpdateable<T> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            IsAs = true;
            OldMappingTableList = this.Context.MappingTables;
            this.Context.MappingTables = this.Context.RewritableMethods.TranslateCopy(this.Context.MappingTables);
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
              this.WhereColumnList.Add(this.Context.EntityProvider.GetDbColumnName<T>(item));
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
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => updateColumns.Contains(it.PropertyName) || it.IsPrimarykey || it.IsIdentity).ToList();
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
            if (resultArray.IsValuable())
            {
                foreach (var item in resultArray)
                {
                    string key = SqlBuilder.GetNoTranslationColumnName(item);
                    UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(key), item));
                }
            }
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => UpdateBuilder.SetValues.Any(v => SqlBuilder.GetNoTranslationColumnName(v.Key) == it.DbColumnName) || it.IsPrimarykey == true).ToList();
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
        public IUpdateable<T> With(string lockString)
        {
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
                this.UpdateBuilder.TableWithString = lockString;
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
                foreach (var column in EntityInfo.Columns)
                {
                    var columnInfo = new DbColumnInfo()
                    {
                        Value = column.PropertyInfo.GetValue(item, null),
                        DbColumnName = GetDbColumnName(column.PropertyName),
                        PropertyName = column.PropertyName,
                        PropertyType = PubMethod.GetUnderType(column.PropertyInfo),
                        TableId = i
                    };
                    if (columnInfo.PropertyType.IsEnum())
                    {
                        columnInfo.Value = Convert.ToInt64(columnInfo.Value);
                    }
                    updateItem.Add(columnInfo);
                }
                this.UpdateBuilder.DbColumnInfoList.AddRange(updateItem);
                ++i;
            }
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
            if (this.UpdateBuilder.Parameters.IsValuable() && this.UpdateBuilder.SetValues.IsValuable())
            {
                this.UpdateBuilder.Parameters.RemoveAll(it => this.UpdateBuilder.SetValues.Any(v => (SqlBuilder.SqlParameterKeyWord + SqlBuilder.GetNoTranslationColumnName(v.Key)) == it.ParameterName));
            }
        }
        private string GetDbColumnName(string entityName)
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
        private List<string> GetPrimaryKeys()
        {
            if (this.WhereColumnList.IsValuable()) {
                return this.WhereColumnList;
            }
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetPrimaries(this.Context.EntityProvider.GetTableName(this.EntityInfo.EntityName));
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            }
        }
        private List<string> GetIdentityKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetIsIdentities(this.Context.EntityProvider.GetTableName(this.EntityInfo.EntityName));
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
    }
}
