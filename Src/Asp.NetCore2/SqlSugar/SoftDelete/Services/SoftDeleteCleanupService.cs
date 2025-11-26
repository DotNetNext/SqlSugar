using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.SoftDelete
{
    public class SoftDeleteCleanupService
    {
        private readonly SqlSugarClient _client;
        private readonly SoftDeleteConfig _config;

        public SoftDeleteCleanupService(SqlSugarClient client, SoftDeleteConfig config)
        {
            _client = client;
            _config = config;
        }

        public int CleanupExpiredDeletes<T>() where T : class, new()
        {
            if (_config.PermanentDeleteAfterDays <= 0) return 0;

            var entityInfo = _client.EntityMaintenance.GetEntityInfo<T>();
            var deletedAtField = entityInfo.Columns.FirstOrDefault(c => c.PropertyName == _config.DeletedAtFieldName);
            
            if (deletedAtField == null) return 0;

            var expiryDate = DateTime.UtcNow.AddDays(-_config.PermanentDeleteAfterDays);
            var pkField = entityInfo.Columns.First(c => c.IsPrimarykey);

            var expiredEntities = _client.Queryable<T>()
                .ClearFilter<T>()
                .Where($"{deletedAtField.DbColumnName} < @expiryDate AND {_config.DeletedFieldName} = @deletedValue",
                    new { expiryDate, deletedValue = _config.DeletedValue })
                .ToList();

            if (!expiredEntities.Any()) return 0;

            var ids = expiredEntities.Select(e => pkField.PropertyInfo.GetValue(e)).ToArray();
            
            return _client.Deleteable<T>().In(ids).ExecuteCommand();
        }
    }
}
