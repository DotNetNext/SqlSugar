using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class UpdateablePage<T>  where T:class,new()
    {
        public  T[] DataList { get;   set; }
        public  SqlSugarProvider Context { get;   set; }
        public int PageSize { get; internal set; }
        public string TableName { get; internal set; }
        public bool IsEnableDiffLogEvent { get; internal set; }
        public DiffLogModel DiffModel { get; internal set; }
        public List<string> UpdateColumns { get; internal set; }
        public string[] WhereColumnList { get; internal set; }

        public UpdateableFilter<T> EnableQueryFilter() 
        {
            return new UpdateableFilter<T>()
            {
                 Context= Context,
                  DataList= DataList,
                   DiffModel= DiffModel,
                    IsEnableDiffLogEvent= IsEnableDiffLogEvent,
                     PageSize=PageSize,
                      TableName=TableName,
                       UpdateColumns=UpdateColumns
            };
        }
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
                    result += this.Context.Updateable(pageItem).AS(TableName).WhereColumns(WhereColumnList).EnableDiffLogEventIF(IsEnableDiffLogEvent, DiffModel).UpdateColumns(UpdateColumns.ToArray()).ExecuteCommand();
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
                    await this.Context.Ado.BeginTranAsync();
                }
                await this.Context.Utilities.PageEachAsync(DataList, PageSize, async pageItem =>
                {
                    result += await this.Context.Updateable(pageItem).AS(TableName).WhereColumns(WhereColumnList).EnableDiffLogEventIF(IsEnableDiffLogEvent, DiffModel).UpdateColumns(UpdateColumns.ToArray()).ExecuteCommandAsync();
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
    }
}
