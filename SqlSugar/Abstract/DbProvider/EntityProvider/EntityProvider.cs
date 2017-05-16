using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class EntityProvider
    {
        public SqlSugarClient Context { get; set; }
        public EntityInfo GetEntityInfo<T>()where T:class,new()
        {
            string cacheKey = "GetEntityInfo"+typeof(T).FullName;
            return CacheFactory.Func<EntityInfo>(cacheKey,
            (cm, key) =>
            {
                return cm[cacheKey];

            }, (cm, key) =>
            {
                EntityInfo result = new EntityInfo();
                var type = typeof(T);
                result.Type = type;
                result.Type.GetProperties();
                result.Name =result.Type.Name;
                result.Columns = new List<EntityColumnInfo>();
                foreach (var item in result.Type.GetProperties())
                {
                    EntityColumnInfo column = new EntityColumnInfo();
                    column.Name = item.Name;
                    column.PropertyInfo = item;
                    result.Columns.Add(column);
                }
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
    }
}
