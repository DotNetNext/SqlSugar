using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SqlSugar.SoftDelete
{
    public class SoftDeleteProvider : ISoftDeleteProvider
    {
        private readonly SqlSugarClient _client;
        private readonly SoftDeleteConfig _config;
        private readonly SoftDeleteFilterManager _filterManager;
        private readonly SoftDeleteAuditService _auditService;

        public SoftDeleteConfig Config => _config;

        public SoftDeleteProvider(SqlSugarClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _config = new SoftDeleteConfig();
            _filterManager = new SoftDeleteFilterManager(_client, _config);
            _auditService = new SoftDeleteAuditService(_client, _config);
        }

        public void Enable()
        {
            _config.IsEnabled = true;
            if (_config.AutoFilter)
            {
                _filterManager.ApplyFilters();
            }
        }

        public void Disable()
        {
            _config.IsEnabled = false;
            _filterManager.RemoveFilters();
        }

        public bool SoftDelete<T>(object id, string reason = null) where T : class, new()
        {
            if (!_config.IsEnabled) return false;

            var entity = _client.Queryable<T>().InSingle(id);
            if (entity == null) return false;

            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();
            SetSoftDeleteFields(entity, entityInfo, reason);

            var result = _client.Updateable(entity).ExecuteCommand() > 0;

            if (result && _config.EnableAuditTrail)
            {
                _auditService.LogDelete<T>(id, reason, false);
            }

            if (result && _config.EnableCascadeDelete)
            {
                CascadeDelete<T>(entity);
            }

            return result;
        }

        public int SoftDeleteBatch<T>(Expression<Func<T, bool>> whereExpression, string reason = null) where T : class, new()
        {
            if (!_config.IsEnabled) return 0;

            var entities = _client.Queryable<T>().Where(whereExpression).ToList();
            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();

            foreach (var entity in entities)
            {
                SetSoftDeleteFields(entity, entityInfo, reason);
            }

            var result = _client.Updateable(entities).ExecuteCommand();

            if (result > 0 && _config.EnableAuditTrail)
            {
                foreach (var entity in entities)
                {
                    var id = entityInfo.Columns.First(c => c.IsPrimarykey).PropertyInfo.GetValue(entity);
                    _auditService.LogDelete<T>(id, reason, false);
                }
            }

            return result;
        }

        public bool Restore<T>(object id) where T : class, new()
        {
            if (!_config.IsEnabled) return false;

            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();
            var entity = GetSoftDeletedEntity<T>(id);
            if (entity == null) return false;

            ClearSoftDeleteFields(entity, entityInfo);

            var result = _client.Updateable(entity).ExecuteCommand() > 0;

            if (result && _config.EnableAuditTrail)
            {
                _auditService.LogRestore<T>(id);
            }

            return result;
        }

        public int RestoreBatch<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            if (!_config.IsEnabled) return 0;

            var entities = GetSoftDeletedEntitiesList<T>(whereExpression);
            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();

            foreach (var entity in entities)
            {
                ClearSoftDeleteFields(entity, entityInfo);
            }

            return _client.Updateable(entities).ExecuteCommand();
        }

        public bool PermanentDelete<T>(object id) where T : class, new()
        {
            var result = _client.Deleteable<T>().In(id).ExecuteCommand() > 0;

            if (result && _config.EnableAuditTrail)
            {
                _auditService.LogPermanentDelete<T>(id);
            }

            return result;
        }

        public List<SoftDeleteInfo> GetSoftDeletedEntities<T>(int pageIndex = 1, int pageSize = 20) where T : class, new()
        {
            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();
            var deletedField = GetDeletedField(entityInfo);
            if (deletedField == null) return new List<SoftDeleteInfo>();

            var entities = _client.Queryable<T>()
                .Where($"{deletedField.DbColumnName} = @deletedValue", new { deletedValue = _config.DeletedValue })
                .ToPageList(pageIndex, pageSize);

            return entities.Select(e => MapToSoftDeleteInfo(e, entityInfo)).ToList();
        }

        public SoftDeleteStatistics GetStatistics()
        {
            return new SoftDeleteStatistics
            {
                TotalSoftDeleted = GetTotalSoftDeleted(),
                TotalRestored = _auditService.GetTotalRestored(),
                TotalPermanentDeleted = _auditService.GetTotalPermanentDeleted()
            };
        }

        public int CleanupExpiredDeletes()
        {
            if (_config.PermanentDeleteAfterDays <= 0) return 0;

            var expiryDate = DateTime.UtcNow.AddDays(-_config.PermanentDeleteAfterDays);
            // Implementation for cleanup based on DeletedAt field
            return 0; // Placeholder
        }

        private void SetSoftDeleteFields(object entity, EntityInfo entityInfo, string reason)
        {
            var deletedField = GetDeletedField(entityInfo);
            var deletedAtField = GetDeletedAtField(entityInfo);
            var deletedByField = GetDeletedByField(entityInfo);
            var deletedReasonField = GetDeletedReasonField(entityInfo);

            if (deletedField != null)
                deletedField.PropertyInfo.SetValue(entity, _config.DeletedValue);

            if (deletedAtField != null)
                deletedAtField.PropertyInfo.SetValue(entity, DateTime.UtcNow);

            if (deletedByField != null && _config.CurrentUserProvider != null)
                deletedByField.PropertyInfo.SetValue(entity, _config.CurrentUserProvider());

            if (deletedReasonField != null && !string.IsNullOrEmpty(reason))
                deletedReasonField.PropertyInfo.SetValue(entity, reason);
        }

        private void ClearSoftDeleteFields(object entity, EntityInfo entityInfo)
        {
            var deletedField = GetDeletedField(entityInfo);
            var deletedAtField = GetDeletedAtField(entityInfo);
            var deletedByField = GetDeletedByField(entityInfo);
            var deletedReasonField = GetDeletedReasonField(entityInfo);

            if (deletedField != null)
                deletedField.PropertyInfo.SetValue(entity, _config.NotDeletedValue);

            if (deletedAtField != null)
                deletedAtField.PropertyInfo.SetValue(entity, null);

            if (deletedByField != null)
                deletedByField.PropertyInfo.SetValue(entity, null);

            if (deletedReasonField != null)
                deletedReasonField.PropertyInfo.SetValue(entity, null);
        }

        private EntityColumnInfo GetDeletedField(EntityInfo entityInfo) =>
            entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedFieldName);

        private EntityColumnInfo GetDeletedAtField(EntityInfo entityInfo) =>
            entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedAtFieldName);

        private EntityColumnInfo GetDeletedByField(EntityInfo entityInfo) =>
            entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedByFieldName);

        private EntityColumnInfo GetDeletedReasonField(EntityInfo entityInfo) =>
            entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedReasonFieldName);

        private T GetSoftDeletedEntity<T>(object id) where T : class, new()
        {
            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();
            var deletedField = GetDeletedField(entityInfo);
            if (deletedField == null) return null;

            return _client.Queryable<T>()
                .Where($"{deletedField.DbColumnName} = @deletedValue", new { deletedValue = _config.DeletedValue })
                .InSingle(id);
        }

        private List<T> GetSoftDeletedEntitiesList<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();
            var deletedField = GetDeletedField(entityInfo);
            if (deletedField == null) return new List<T>();

            return _client.Queryable<T>()
                .Where($"{deletedField.DbColumnName} = @deletedValue", new { deletedValue = _config.DeletedValue })
                .Where(whereExpression)
                .ToList();
        }

        private SoftDeleteInfo MapToSoftDeleteInfo(object entity, EntityInfo entityInfo)
        {
            var deletedAtField = GetDeletedAtField(entityInfo);
            var deletedByField = GetDeletedByField(entityInfo);
            var deletedReasonField = GetDeletedReasonField(entityInfo);
            var pkField = entityInfo.Columns.First(c => c.IsPrimarykey);

            return new SoftDeleteInfo
            {
                EntityName = entityInfo.EntityName,
                EntityId = pkField.PropertyInfo.GetValue(entity),
                DeletedAt = deletedAtField != null ? (DateTime?)deletedAtField.PropertyInfo.GetValue(entity) ?? DateTime.MinValue : DateTime.MinValue,
                DeletedBy = deletedByField?.PropertyInfo.GetValue(entity)?.ToString(),
                DeletedReason = deletedReasonField?.PropertyInfo.GetValue(entity)?.ToString()
            };
        }

        private void CascadeDelete<T>(T entity) where T : class, new()
        {
            // Placeholder for cascade delete implementation
        }

        private int GetTotalSoftDeleted()
        {
            // Placeholder - would query audit trail
            return 0;
        }
    }
}
