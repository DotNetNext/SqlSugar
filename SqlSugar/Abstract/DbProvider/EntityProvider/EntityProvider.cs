using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class EntityProvider
    {
        public SqlSugarClient Context { get; set; }

        public List<EntityInfo> GetAllEntities()
        {
            string cacheKey = "GetAllEntities";
            return CacheFactory.Func<List<EntityInfo>>(cacheKey,
            (cm, key) =>
            {
                return cm[key];

            }, (cm, key) =>
            {
                List<EntityInfo> reval = new List<EntityInfo>();
                var classes = Assembly.Load(this.Context.EntityNamespace.Split('.').First()).GetTypes();
                foreach (var item in classes)
                {
                    if (item.FullName.Contains(this.Context.EntityNamespace))
                        reval.Add(GetEntityInfo(item));
                }
                return reval;
            });
        }
        public EntityInfo GetEntityInfo<T>()
        {
            return GetEntityInfo(typeof(T));
        }
        public EntityInfo GetEntityInfo(Type type)
        {
            string cacheKey = "GetEntityInfo" + type.FullName;
            return CacheFactory.Func<EntityInfo>(cacheKey,
            (cm, key) =>
            {
                return cm[cacheKey];

            }, (cm, key) =>
            {
                EntityInfo result = new EntityInfo();
                var sugarAttributeInfo = type.GetCustomAttributes(typeof(SugarTable), true).Where(it => it is SugarTable).SingleOrDefault();
                if (sugarAttributeInfo.IsValuable())
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
        public string GetEntityName(string tableName)
        {
            if (this.Context.MappingTables == null || this.Context.MappingTables.Count == 0) return tableName;
            else
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.DbTableName == tableName);
                return mappingInfo == null ? tableName : mappingInfo.EntityName;
            }
        }
        public string GetDbColumnName<T>(string entityPropertyName)
        {
            var typeName = typeof(T).Name;
            if (this.Context.MappingColumns == null || this.Context.MappingColumns.Count == 0) return entityPropertyName;
            else
            {
                var mappingInfo = this.Context.MappingColumns.SingleOrDefault(it => it.EntityName == typeName && it.EntityPropertyName == entityPropertyName);
                return mappingInfo == null ? entityPropertyName : mappingInfo.DbColumnName;
            }
        }
        public string GetEntityPropertyName<T>(string dbColumnName)
        {
            var typeName = typeof(T).Name;
            if (this.Context.MappingColumns == null || this.Context.MappingColumns.Count == 0) return dbColumnName;
            else
            {
                var mappingInfo = this.Context.MappingColumns.SingleOrDefault(it => it.EntityName == typeName && it.DbColumnName == dbColumnName);
                return mappingInfo == null ? dbColumnName : mappingInfo.DbColumnName;
            }
        }

        #region Primary key
        private static void SetColumns(EntityInfo result)
        {
            foreach (var property in result.Type.GetProperties())
            {
                EntityColumnInfo column = new EntityColumnInfo();
                var isVirtual = property.GetGetMethod().IsVirtual;
                if (isVirtual) continue;
                var sugarColumn = property.GetCustomAttributes(typeof(SugarColumn), true)
                .Where(it => it is SugarColumn)
                .Select(it => (SugarColumn)it)
                .FirstOrDefault();
                column.DbTableName = result.DbTableName;
                column.EnitytName = result.EntityName;
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
                    }
                    else
                    {
                        column.IsIgnore = true;
                    }
                }
                result.Columns.Add(column);
            }
        }
        #endregion

    }
}
