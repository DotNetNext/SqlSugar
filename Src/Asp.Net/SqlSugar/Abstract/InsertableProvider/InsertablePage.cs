using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class InsertablePage<T> where T:class,new()
    {
        public int PageSize { get; set; }
        public SqlSugarProvider Context { get;   set; }
        public T[] DataList { get;   set; }
        public string TableName { get; internal set; }
        public List<string> InsertColumns { get; internal set; }
        public bool IsEnableDiffLogEvent { get; internal set; }
        public DiffLogModel DiffModel { get; internal set; }
        public bool IsOffIdentity { get; internal set; }
        public bool IsInsertColumnsNull { get; internal set; }

        public int ExecuteCommand() 
        {
            if (DataList.Count() == 1 && DataList.First() == null) 
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
                this.Context.Utilities.PageEach(DataList, PageSize, pageItem =>
                {
                    result += this.Context.Insertable(pageItem).AS(TableName).IgnoreColumnsNull(this.IsInsertColumnsNull).OffIdentity(IsOffIdentity).EnableDiffLogEventIF(IsEnableDiffLogEvent, DiffModel).InsertColumns(InsertColumns.ToArray()).ExecuteCommand();
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
        public async Task<int> ExecuteCommandAsync()
        {
            if (DataList.Count() == 1 && DataList.First() == null)
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
                    await  this.Context.Ado.BeginTranAsync();
                }
                await this.Context.Utilities.PageEachAsync(DataList, PageSize, async pageItem  =>
                {
                    result +=await this.Context.Insertable(pageItem).AS(TableName).IgnoreColumnsNull(this.IsInsertColumnsNull).OffIdentity(IsOffIdentity).EnableDiffLogEventIF(IsEnableDiffLogEvent, DiffModel).InsertColumns(InsertColumns.ToArray()).ExecuteCommandAsync();
                });
                if (isNoTran)
                {
                    await  this.Context.Ado.CommitTranAsync();
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

        public List<long> ExecuteReturnSnowflakeIdList()
        {
            if (DataList.Count() == 1 && DataList.First() == null)
            {
                return  new List<long>();
            }
            if (PageSize == 0) { PageSize = 1000; }
            var result = new List<long>();
            var isNoTran = this.Context.Ado.IsNoTran();
            try
            {
                if (isNoTran)
                {
                    this.Context.Ado.BeginTran();
                }
                this.Context.Utilities.PageEach(DataList, PageSize, pageItem =>
                {
                    result.AddRange(this.Context.Insertable(pageItem).AS(TableName).OffIdentity(IsOffIdentity).EnableDiffLogEventIF(IsEnableDiffLogEvent, DiffModel).InsertColumns(InsertColumns.ToArray()).ExecuteReturnSnowflakeIdList());
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
        public async Task<List<long>> ExecuteReturnSnowflakeIdListAsync()
        {
            if (DataList.Count() == 1 && DataList.First() == null)
            {
                return new List<long>();
            }
            if (PageSize == 0) { PageSize = 1000; }
            var result = new List<long>();
            var isNoTran = this.Context.Ado.IsNoTran();
            try
            {
                if (isNoTran)
                {
                    await this.Context.Ado.BeginTranAsync();
                }
                await this.Context.Utilities.PageEachAsync(DataList, PageSize, async pageItem =>
                {
                    result.AddRange(await this.Context.Insertable(pageItem).AS(TableName).OffIdentity(IsOffIdentity).EnableDiffLogEventIF(IsEnableDiffLogEvent, DiffModel).InsertColumns(InsertColumns.ToArray()).ExecuteReturnSnowflakeIdListAsync());
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

        public InsertablePage<T> IgnoreColumnsNull(bool isIgnoreNull = true)
        {
            this.PageSize = 1;
            this.IsInsertColumnsNull = isIgnoreNull;
            return this;
        }
    }
}
