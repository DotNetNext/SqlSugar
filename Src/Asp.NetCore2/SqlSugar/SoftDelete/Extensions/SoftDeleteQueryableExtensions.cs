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
