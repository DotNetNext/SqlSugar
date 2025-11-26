using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.SoftDelete
{
    public class SoftDeleteRestoreService
    {
        private readonly SqlSugarClient _client;
        private readonly SoftDeleteConfig _config;
        private readonly SoftDeleteAuditService _auditService;

        public SoftDeleteRestoreService(SqlSugarClient client, SoftDeleteConfig config)
        {
            _client = client;
            _config = config;
            _auditService = new SoftDeleteAuditService(client, config);
        }

        public bool RestoreWithHistory<T>(object id) where T : class, new()
        {
            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();
            var entity = GetSoftDeletedEntity<T>(id, entityInfo);
            
            if (entity == null) return false;

            ClearSoftDeleteFields(entity, entityInfo);
            var result = _client.Updateable(entity).ExecuteCommand() > 0;

            if (result)
            {
                _auditService.LogRestore<T>(id);
            }

            return result;
        }

        public List<SoftDeleteInfo> GetRestorableEntities<T>(DateTime? deletedAfter = null, DateTime? deletedBefore = null) where T : class, new()
        {
            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();
            var deletedAtField = entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedAtFieldName);
            
            if (deletedAtField == null) return new List<SoftDeleteInfo>();

            var query = _client.Queryable<T>()
                .Where($"{_config.DeletedFieldName} = @deletedValue", new { deletedValue = _config.DeletedValue });

            if (deletedAfter.HasValue)
            {
                query = query.Where($"{deletedAtField.DbColumnName} >= @deletedAfter", new { deletedAfter = deletedAfter.Value });
            }

            if (deletedBefore.HasValue)
            {
                query = query.Where($"{deletedAtField.DbColumnName} <= @deletedBefore", new { deletedBefore = deletedBefore.Value });
            }

            return query.ToList().Select(e => MapToSoftDeleteInfo(e, entityInfo)).ToList();
        }

        private T GetSoftDeletedEntity<T>(object id, EntityInfo entityInfo) where T : class, new()
        {
            return _client.Queryable<T>()
                .Where($"{_config.DeletedFieldName} = @deletedValue", new { deletedValue = _config.DeletedValue })
                .InSingle(id);
        }

        private void ClearSoftDeleteFields(object entity, EntityInfo entityInfo)
        {
            var deletedField = entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedFieldName);
            var deletedAtField = entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedAtFieldName);
            var deletedByField = entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedByFieldName);
            var deletedReasonField = entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedReasonFieldName);

            deletedField?.PropertyInfo.SetValue(entity, _config.NotDeletedValue);
            deletedAtField?.PropertyInfo.SetValue(entity, null);
            deletedByField?.PropertyInfo.SetValue(entity, null);
            deletedReasonField?.PropertyInfo.SetValue(entity, null);
        }

        private SoftDeleteInfo MapToSoftDeleteInfo(object entity, EntityInfo entityInfo)
        {
            var deletedAtField = entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedAtFieldName);
            var deletedByField = entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedByFieldName);
            var deletedReasonField = entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedReasonFieldName);
            var pkField = entityInfo.Columns.First(c => c.IsPrimarykey);

            return new SoftDeleteInfo
            {
                EntityName = entityInfo.EntityName,
                EntityId = pkField.PropertyInfo.GetValue(entity),
                DeletedAt = (DateTime?)deletedAtField?.PropertyInfo.GetValue(entity) ?? DateTime.MinValue,
                DeletedBy = deletedByField?.PropertyInfo.GetValue(entity)?.ToString(),
                DeletedReason = deletedReasonField?.PropertyInfo.GetValue(entity)?.ToString()
            };
        }
    }
}
