using System;
using System.Linq;

namespace SqlSugar.SoftDelete
{
    public class SoftDeleteEntityAnalyzer
    {
        private readonly SqlSugarClient _client;
        private readonly SoftDeleteConfig _config;

        public SoftDeleteEntityAnalyzer(SqlSugarClient client, SoftDeleteConfig config)
        {
            _client = client;
            _config = config;
        }

        public bool IsSoftDeleteEnabled<T>() where T : class, new()
        {
            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();
            return HasSoftDeleteField(entityInfo);
        }

        public bool HasSoftDeleteField(EntityInfo entityInfo)
        {
            return entityInfo.Columns.Any(c => c.PropertyName == _config.DeletedFieldName);
        }

        public bool HasDeletedAtField(EntityInfo entityInfo)
        {
            return entityInfo.Columns.Any(c => c.PropertyName == _config.DeletedAtFieldName);
        }

        public bool HasDeletedByField(EntityInfo entityInfo)
        {
            return entityInfo.Columns.Any(c => c.PropertyName == _config.DeletedByFieldName);
        }

        public SoftDeleteAttribute GetSoftDeleteAttribute<T>() where T : class
        {
            var type = typeof(T);
            return (SoftDeleteAttribute)Attribute.GetCustomAttribute(type, typeof(SoftDeleteAttribute));
        }

        public bool ValidateEntityForSoftDelete<T>() where T : class, new()
        {
            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();
            
            if (!HasSoftDeleteField(entityInfo))
            {
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have {_config.DeletedFieldName} field for soft delete.");
            }

            var deletedField = entityInfo.Columns.First(c => c.PropertyName == _config.DeletedFieldName);
            if (deletedField.PropertyInfo.PropertyType != typeof(bool) && deletedField.PropertyInfo.PropertyType != typeof(bool?))
            {
                throw new InvalidOperationException($"{_config.DeletedFieldName} field must be of type bool or bool?");
            }

            return true;
        }
    }
}
