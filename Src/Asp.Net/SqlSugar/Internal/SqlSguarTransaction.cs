using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.Internal
{
    /// <summary>
    /// todo: 支持多级子事务
    /// </summary>
    internal class SqlSguarTransaction : ISugarTransaction
    {
        private readonly SqlSugarClient _client;

        public SqlSguarTransaction(SqlSugarClient client)
        {
            _client = client;
        }

        /// <inheritdoc/>
        public void Commit()
        {
            _client.CommitTran();
        }

        /// <summary>
        /// 如果事务已经提交，则销毁资源，如果没有提交，则回滚
        /// </summary>
        /// <remarks> 该功能由 <see cref="System.Data.IDbTransaction"/> 提供</remarks>
        public void Dispose()
        {
            _client.Ado.Transaction?.Dispose();
            _client.Ado.Transaction = null;
            _client.AllClientEach(db =>
            {
                db.Ado.Transaction?.Dispose();
                db.Ado.Transaction = null;
            });
        }

        /// <inheritdoc/>
        public void Rollback()
        {
            _client.RollbackTran();
        }
    }
}
