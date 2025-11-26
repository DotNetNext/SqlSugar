using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlSugar.SoftDelete
{
    /// <summary>
    /// Extension methods for integrating Soft Delete with SqlSugarClient
    /// </summary>
    public static class SoftDeleteExtensions
    {
        private const string SOFT_DELETE_KEY = "__SoftDelete__";

        /// <summary>
        /// Get or create SoftDelete provider for this client
        /// </summary>
        public static ISoftDeleteProvider GetSoftDelete(this ISqlSugarClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            if (!client.TempItems.ContainsKey(SOFT_DELETE_KEY))
            {
                var sqlSugarClient = client as SqlSugarClient ?? throw new InvalidOperationException("Client must be SqlSugarClient or derived type");
                client.TempItems[SOFT_DELETE_KEY] = new SoftDeleteProvider(sqlSugarClient);
            }

            return client.TempItems[SOFT_DELETE_KEY] as ISoftDeleteProvider;
        }

        /// <summary>
        /// Enable soft delete with default configuration
        /// </summary>
        public static void EnableSoftDelete(this ISqlSugarClient client)
        {
            var provider = client.GetSoftDelete();
            provider.Enable();
        }

        /// <summary>
        /// Enable soft delete with custom configuration
        /// </summary>
        public static void EnableSoftDelete(this ISqlSugarClient client, Action<SoftDeleteConfig> configAction)
        {
            var provider = client.GetSoftDelete();
            configAction?.Invoke(provider.Config);
            provider.Enable();
        }

        /// <summary>
        /// Disable soft delete
        /// </summary>
        public static void DisableSoftDelete(this ISqlSugarClient client)
        {
            var provider = client.GetSoftDelete();
            provider.Disable();
        }

        /// <summary>
        /// Soft delete entity by ID
        /// </summary>
        public static bool SoftDelete<T>(this ISqlSugarClient client, object id, string reason = null) where T : class, new()
        {
            return client.GetSoftDelete().SoftDelete<T>(id, reason);
        }

        /// <summary>
        /// Soft delete multiple entities by condition
        /// </summary>
        public static int SoftDeleteBatch<T>(this ISqlSugarClient client, Expression<Func<T, bool>> whereExpression, string reason = null) where T : class, new()
        {
            return client.GetSoftDelete().SoftDeleteBatch(whereExpression, reason);
        }

        /// <summary>
        /// Restore soft deleted entity by ID
        /// </summary>
        public static bool RestoreSoftDeleted<T>(this ISqlSugarClient client, object id) where T : class, new()
        {
            return client.GetSoftDelete().Restore<T>(id);
        }

        /// <summary>
        /// Restore multiple soft deleted entities by condition
        /// </summary>
        public static int RestoreSoftDeletedBatch<T>(this ISqlSugarClient client, Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return client.GetSoftDelete().RestoreBatch(whereExpression);
        }

        /// <summary>
        /// Permanently delete entity (bypass soft delete)
        /// </summary>
        public static bool PermanentDelete<T>(this ISqlSugarClient client, object id) where T : class, new()
        {
            return client.GetSoftDelete().PermanentDelete<T>(id);
        }

        /// <summary>
        /// Get list of soft deleted entities
        /// </summary>
        public static List<SoftDeleteInfo> GetSoftDeletedEntities<T>(this ISqlSugarClient client, int pageIndex = 1, int pageSize = 20) where T : class, new()
        {
            return client.GetSoftDelete().GetSoftDeletedEntities<T>(pageIndex, pageSize);
        }

        /// <summary>
        /// Get soft delete statistics
        /// </summary>
        public static SoftDeleteStatistics GetSoftDeleteStats(this ISqlSugarClient client)
        {
            return client.GetSoftDelete().GetStatistics();
        }

        /// <summary>
        /// Cleanup expired soft deletes
        /// </summary>
        public static int CleanupExpiredSoftDeletes(this ISqlSugarClient client)
        {
            return client.GetSoftDelete().CleanupExpiredDeletes();
        }
    }
}
