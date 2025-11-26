using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlSugar.SoftDelete
{
    public interface ISoftDeleteProvider
    {
        SoftDeleteConfig Config { get; }
        void Enable();
        void Disable();
        bool SoftDelete<T>(object id, string reason = null) where T : class, new();
        int SoftDeleteBatch<T>(Expression<Func<T, bool>> whereExpression, string reason = null) where T : class, new();
        bool Restore<T>(object id) where T : class, new();
        int RestoreBatch<T>(Expression<Func<T, bool>> whereExpression) where T : class, new();
        bool PermanentDelete<T>(object id) where T : class, new();
        List<SoftDeleteInfo> GetSoftDeletedEntities<T>(int pageIndex = 1, int pageSize = 20) where T : class, new();
        SoftDeleteStatistics GetStatistics();
        int CleanupExpiredDeletes();
    }
}
