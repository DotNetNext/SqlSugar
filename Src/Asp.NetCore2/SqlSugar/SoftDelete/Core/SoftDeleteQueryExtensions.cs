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
