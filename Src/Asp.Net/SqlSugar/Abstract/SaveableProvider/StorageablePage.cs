using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class StorageablePage<T> where T : class,new()
    {
        internal DbLockType? lockType { get; set; }

        public SqlSugarProvider Context { get;  set; }
        public List<T> Data { get;   set; }
        public int PageSize { get; internal set; }
        public Action<int> ActionCallBack { get; internal set; }
        public string TableName { get; internal set; }
        public Expression<Func<T, object>> whereExpression { get; internal set; }

        public int ExecuteCommand()
        {
            if (Data.Count() == 1 && Data.First() == null)
            {
                return 0;
            }
            if (PageSize == 0) { PageSize = 1000; }
            var result = 0;
            var isNoTran = this.Context.Ado.IsNoTran();
            try
            {
                if (isNoTran)
                {
                    this.Context.Ado.BeginTran();
                }
                this.Context.Utilities.PageEach(Data, PageSize, pageItem =>
                {
                    result += this.Context.Storageable(pageItem).As(TableName).TranLock(lockType).WhereColumns(whereExpression).ExecuteCommand();
                    if (ActionCallBack != null) 
                    {
                        ActionCallBack(result);
                    }
                });
                if (isNoTran)
                {
                    this.Context.Ado.CommitTran();
                }
            }
            catch (Exception)
            {
                if (isNoTran)
                {
                    this.Context.Ado.RollbackTran();
                }
                throw;
            }
            return result;
        }
        public async Task<int> ExecuteCommandAsync(CancellationToken? cancellationToken=null)
        {
            if (cancellationToken != null)
                this.Context.Ado.CancellationToken = cancellationToken.Value;
            if (Data.Count() == 1 && Data.First() == null)
            {
                return 0;
            }
            if (PageSize == 0) { PageSize = 1000; }
            var result = 0;
            var isNoTran = this.Context.Ado.IsNoTran();
            try
            {
                if (isNoTran)
                {
                    await this.Context.Ado.BeginTranAsync();
                }
                await this.Context.Utilities.PageEachAsync(Data, PageSize, async pageItem =>
                {
                    result += await this.Context.Storageable(pageItem).As(TableName).TranLock(lockType).WhereColumns(whereExpression).ExecuteCommandAsync();
                    if (ActionCallBack != null)
                    {
                        ActionCallBack(result);
                    }
                });
                if (isNoTran)
                {
                    await this.Context.Ado.CommitTranAsync();
                }
            }
            catch (Exception)
            {
                if (isNoTran)
                {
                    await this.Context.Ado.RollbackTranAsync();
                }
                throw;
            }
            return result;
        }
        public int ExecuteSqlBulkCopy()
        {
            if (Data.Count() == 1 && Data.First() == null)
            {
                return 0;
            }
            if (PageSize == 0) { PageSize = 1000; }
            var result = 0;
            var isNoTran = this.Context.Ado.IsNoTran();
            try
            {
                 
                this.Context.Utilities.PageEach(Data, PageSize, pageItem =>
                {
                    result += this.Context.Storageable(pageItem).As(TableName).TranLock(lockType).WhereColumns(whereExpression).ExecuteSqlBulkCopy();
                    if (ActionCallBack != null)
                    {
                        ActionCallBack(result);
                    }
                }); 
            }
            catch (Exception)
            { 
                throw;
            }
            return result;
        }
        public async Task<int> ExecuteSqlBulkCopyAsync(CancellationToken? cancellationToken = null)
        {
            if(cancellationToken!=null)
              this.Context.Ado.CancellationToken = cancellationToken.Value;
            if (Data.Count() == 1 && Data.First() == null)
            {
                return 0;
            }
            if (PageSize == 0) { PageSize = 1000; }
            var result = 0;
            var isNoTran = this.Context.Ado.IsNoTran();
            try
            { 
                await this.Context.Utilities.PageEachAsync(Data, PageSize, async pageItem =>
                {
                    result += await this.Context.Storageable(pageItem).As(TableName).TranLock(lockType).WhereColumns(whereExpression).ExecuteSqlBulkCopyAsync();
                    if (ActionCallBack != null)
                    {
                        ActionCallBack(result);
                    }
                }); 
            }
            catch (Exception)
            { 
                throw;
            }
            return result;
        }

    }
}
