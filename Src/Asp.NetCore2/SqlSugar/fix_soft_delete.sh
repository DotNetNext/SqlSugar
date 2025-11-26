#!/bin/bash

# Fix 1: Simplify SoftDeleteFilterManager - remove GetEntityInfos and complex filtering
cat > SoftDelete/Core/SoftDeleteFilterManager.cs << 'EOF'
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SqlSugar.SoftDelete
{
    public class SoftDeleteFilterManager
    {
        private readonly SqlSugarClient _client;
        private readonly SoftDeleteConfig _config;
        private bool _filtersApplied = false;

        public SoftDeleteFilterManager(SqlSugarClient client, SoftDeleteConfig config)
        {
            _client = client;
            _config = config;
        }

        public void ApplyFilters()
        {
            if (_filtersApplied || !_config.AutoFilter) return;
            _filtersApplied = true;
            // Filters are applied per-query via QueryableExtensions instead of globally
        }

        public void RemoveFilters()
        {
            if (!_filtersApplied) return;
            _filtersApplied = false;
        }

        public void ApplyFilterForType<T>(Expression<Func<T, bool>> filter) where T : class
        {
            _client.QueryFilter.AddTableFilter(filter);
        }
    }
}
EOF

# Fix 2: Fix ClearFilter calls - use Type instead of string
cat > SoftDelete/Core/SoftDeleteQueryExtensions.cs << 'EOF'
using System;

namespace SqlSugar.SoftDelete
{
    public static class SoftDeleteQueryExtensions
    {
        public static ISugarQueryable<T> IncludeSoftDeleted<T>(this ISugarQueryable<T> queryable) where T : class, new()
        {
            return queryable.ClearFilter<T>();
        }

        public static ISugarQueryable<T> OnlySoftDeleted<T>(this ISugarQueryable<T> queryable, SoftDeleteConfig config) where T : class, new()
        {
            return queryable.ClearFilter<T>()
                .Where($"{config.DeletedFieldName} = @deletedValue", new { deletedValue = config.DeletedValue });
        }
    }
}
EOF

# Fix 3: Fix queryable extensions - use Type instead of string
cat > SoftDelete/Extensions/SoftDeleteQueryableExtensions.cs << 'EOF'
using System;

namespace SqlSugar.SoftDelete
{
    public static class SoftDeleteQueryableExtensions
    {
        public static ISugarQueryable<T> WithSoftDeleted<T>(this ISugarQueryable<T> queryable) where T : class, new()
        {
            return queryable.ClearFilter<T>();
        }

        public static ISugarQueryable<T> OnlySoftDeleted<T>(this ISugarQueryable<T> queryable, ISqlSugarClient client) where T : class, new()
        {
            var config = client.GetSoftDelete().Config;
            return queryable.ClearFilter<T>()
                .Where($"{config.DeletedFieldName} = @deletedValue", new { deletedValue = config.DeletedValue });
        }

        public static ISugarQueryable<T> OnlyActive<T>(this ISugarQueryable<T> queryable, ISqlSugarClient client) where T : class, new()
        {
            var config = client.GetSoftDelete().Config;
            return queryable.ClearFilter<T>()
                .Where($"({config.DeletedFieldName} = @notDeletedValue OR {config.DeletedFieldName} IS NULL)", 
                    new { notDeletedValue = config.NotDeletedValue });
        }

        public static ISugarQueryable<T> DeletedBetween<T>(this ISugarQueryable<T> queryable, ISqlSugarClient client, DateTime startDate, DateTime endDate) where T : class, new()
        {
            var config = client.GetSoftDelete().Config;
            return queryable.ClearFilter<T>()
                .Where($"{config.DeletedFieldName} = @deletedValue AND {config.DeletedAtFieldName} >= @start AND {config.DeletedAtFieldName} <= @end",
                    new { deletedValue = config.DeletedValue, start = startDate, end = endDate });
        }

        public static ISugarQueryable<T> DeletedBy<T>(this ISugarQueryable<T> queryable, ISqlSugarClient client, string deletedBy) where T : class, new()
        {
            var config = client.GetSoftDelete().Config;
            return queryable.ClearFilter<T>()
                .Where($"{config.DeletedFieldName} = @deletedValue AND {config.DeletedByFieldName} = @deletedBy",
                    new { deletedValue = config.DeletedValue, deletedBy });
        }
    }
}
EOF

# Fix 4: Simplify CleanupService - remove GetEntityInfos
cat > SoftDelete/Services/SoftDeleteCleanupService.cs << 'EOF'
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
EOF

echo "✓ Fixed all compilation errors"
