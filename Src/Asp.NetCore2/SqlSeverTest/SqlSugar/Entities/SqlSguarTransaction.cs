using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar  
{
    public class SqlSguarTransaction:IDisposable
    {
        private readonly SqlSugarClient context;

        public SqlSguarTransaction(SqlSugarClient client)
        {
            context = client;
            context.BeginTran();
        }
        public void CommitTran()
        {
            context.CommitTran();
        }
        public void RollbackTran()
        {
            context.RollbackTran();
        }
        public void Dispose()
        {
            context.RollbackTran();
        }
    }
}
