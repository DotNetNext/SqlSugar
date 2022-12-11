using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal static class AsyncHelper
    {
        public static async Task<DbTransaction> BeginTransactionAsync(this DbConnection db)
        {
            await Task.Delay(0);
            //.net frame work no BeginTransactionAsync
            return db.BeginTransaction();
        }
        public static async Task CloseAsync(this DbConnection db)
        {
            await Task.Delay(0);
            //.net frame work no CloseAsync
            db.Close();
        }
        public static async Task RollbackAsync(this DbTransaction db)
        {
            db.Rollback();
            //.net frame work no RollbackAsync
            await Task.Delay(0);
        }
        public static async Task CommitAsync(this DbTransaction db)
        {
            db.Commit();
            //.net frame work no CommitAsyncAsync
            await Task.Delay(0);
        }
    }
}
