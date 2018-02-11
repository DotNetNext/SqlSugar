using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class EntityMaintenance
    {
        public SqlSugarClient Context { get; set; }

        public EntityInfo GetEntityInfo<T>()
        {
            return GetEntityInfo(typeof(T));
        }
        public EntityInfo GetEntityInfo(Type type)
        {
            string cacheKey = "GetEntityInfo" + type.FullName;
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
            () =>
            {
                EntityInfo result = new EntityInfo();
                var sugarAttributeInfo = type.GetTypeInfo().GetCustomAttributes(typeof(SugarTable), true).Where(it => it is SugarTable).SingleOrDefault();
                if (sugarAttributeInfo.HasValue())
                {
                    var sugarTable = (SugarTable)sugarAttributeInfo;
                    result.DbTableName = sugarTable.TableName;
                }
                result.Type = type;
                result.EntityName = result.Type.Name;
                result.Columns = new List<EntityColumnInfo>();
                SetColumns(result);
                return result;
            });
        }
        public string GetTableName<T>()
        {
            var typeName = typeof(T).Name;
            if (this.Context.MappingTables == null || this.Context.MappingTables.Count == 0) return typeName;
            else
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.EntityName == typeName);
                return mappingInfo == null ? typeName : mappingInfo.DbTableName;
            }
        }
        public string GetTableName(Type entityType)
        {
            var typeName = entityType.Name;
            if (this.Context.MappingTables == null || this.Context.MappingTables.Count == 0) return typeName;
            else
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.EntityName == typeName);
                return mappingInfo == null ? typeName : mappingInfo.DbTableName;
            }
        }
        public string GetTableName(string entityName)
        {
            var typeName = entityName;
            if (this.Context.MappingTables == null || this.Context.MappingTables.Count == 0) return typeName;
            else
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.EntityName == typeName);
                return mappingInfo == null ? typeName : mappingInfo.DbTableName;
            }
        }
        public string GetEntityName(string tableName)
        {
            if (this.Context.MappingTables == null || this.Context.MappingTables.Count == 0) return tableName;
            else
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.DbTableName == tableName);
                return mappingInfo == null ? tableName : mappingInfo.EntityName;
            }
        }
        public string GetDbColumnName<T>(string propertyName)
        {
            var isAny = this.GetEntityInfo<T>().Columns.Any(it => it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            Check.Exception(!isAny, "Property " + propertyName + " is Invalid");
            var typeName = typeof(T).Name;
            if (this.Context.MappingColumns == null || this.Context.MappingColumns.Count == 0) return propertyName;
            else
            {
                var mappingInfo = this.Context.MappingColumns.SingleOrDefault(it => it.EntityName == typeName && it.PropertyName == propertyName);
                return mappingInfo == null ? propertyName : mappingInfo.DbColumnName;
            }
        }
        public string GetDbColumnName(string propertyName,Type entityType)
        {
            var isAny = this.GetEntityInfo(entityType).Columns.Any(it => it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            Check.Exception(!isAny, "Property " + propertyName + " is Invalid");
            var typeName = entityType.Name;
            if (this.Context.MappingColumns == null || this.Context.MappingColumns.Count == 0) return propertyName;
            else
            {
                var mappingInfo = this.Context.MappingColumns.SingleOrDefault(it => it.EntityName == typeName && it.PropertyName == propertyName);
                return mappingInfo == null ? propertyName : mappingInfo.DbColumnName;
            }
        }
        public string GetPropertyName<T>(string dbColumnName)
        {
            var typeName = typeof(T).Name;
            if (this.Context.MappingColumns == null || this.Context.MappingColumns.Count == 0) return dbColumnName;
            else
            {
                var mappingInfo = this.Context.MappingColumns.SingleOrDefault(it => it.EntityName == typeName && it.DbColumnName.Equals(dbColumnName,StringComparison.CurrentCultureIgnoreCase));
                return mappingInfo == null ? dbColumnName : mappingInfo.PropertyName;
            }
        }
        public string GetPropertyName(string dbColumnName,Type entityType)
        {
            var typeName = entityType.Name;
            if (this.Context.MappingColumns == null || this.Context.MappingColumns.Count == 0) return dbColumnName;
            else
            {
                var mappingInfo = this.Context.MappingColumns.SingleOrDefault(it => it.EntityName == typeName && it.DbColumnName.Equals(dbColumnName,StringComparison.CurrentCultureIgnoreCase));
                return mappingInfo == null ? dbColumnName : mappingInfo.PropertyName;
            }
        }
        public PropertyInfo GetProperty<T>(string dbColumnName)
        {
            var propertyName = GetPropertyName<T>(dbColumnName);
            return typeof(T).GetProperties().First(it => it.Name == propertyName);
        }
        #region Primary key
        private void SetColumns(EntityInfo result)
        {
            foreach (var property in result.Type.GetProperties())
            {
                EntityColumnInfo column = new EntityColumnInfo();
                //var isVirtual = property.GetGetMethod().IsVirtual;
                //if (isVirtual) continue;
                var sugarColumn = property.GetCustomAttributes(typeof(SugarColumn), true)
                .Where(it => it is SugarColumn)
                .Select(it => (SugarColumn)it)
                .FirstOrDefault();
                column.DbTableName = result.DbTableName;
                column.EntityName = result.EntityName;
                column.PropertyName = property.Name;
                column.PropertyInfo = property;
                if (sugarColumn.IsNullOrEmpty())
                {
                    column.DbColumnName = property.Name;
                }
                else
                {
                    if (sugarColumn.IsIgnore == false)
                    {
                        column.DbColumnName = sugarColumn.ColumnName.IsNullOrEmpty() ? property.Name : sugarColumn.ColumnName;
                        column.IsPrimarykey = sugarColumn.IsPrimaryKey;
                        column.IsIdentity = sugarColumn.IsIdentity;
                        column.ColumnDescription = sugarColumn.ColumnDescription;
                        column.IsNullable = sugarColumn.IsNullable;
                        column.Length = sugarColumn.Length;
                        column.OldDbColumnName = sugarColumn.OldColumnName;
                        column.DataType = sugarColumn.ColumnDataType;
                        column.DecimalDigits = sugarColumn.DecimalDigits;
                        column.OracleSequenceName = sugarColumn.OracleSequenceName;
                        column.IsOnlyIgnoreInsert = sugarColumn.IsOnlyIgnoreInsert;
                    }
                    else
                    {
                        column.IsIgnore = true;
                    }
                }
                if (this.Context.MappingColumns.HasValue())
                {
                    var golbalMappingInfo = this.Context.MappingColumns.FirstOrDefault(it => it.EntityName.Equals(result.EntityName, StringComparison.CurrentCultureIgnoreCase) && it.PropertyName == column.PropertyName);
                    if (golbalMappingInfo != null)
                        column.DbColumnName = golbalMappingInfo.DbColumnName;
                }
                if (this.Context.IgnoreColumns.HasValue())
                {
                    var golbalMappingInfo = this.Context.IgnoreColumns.FirstOrDefault(it => it.EntityName.Equals(result.EntityName, StringComparison.CurrentCultureIgnoreCase) && it.PropertyName == column.PropertyName);
                    if (golbalMappingInfo != null)
                        column.IsIgnore = true;
                }
                result.Columns.Add(column);
            }
        }
        #endregion

    }
}
