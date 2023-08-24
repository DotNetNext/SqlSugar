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

        public int ExecuteCommand() 
        {
            if (PageSize == 0) { PageSize = 1000; }
            var result = 0;
            this.Context.Utilities.PageEach(DataList, PageSize, pageItem =>
            {
                result += this.Context.Insertable(pageItem).AS(TableName).EnableDiffLogEventIF(IsEnableDiffLogEvent,DiffModel).InsertColumns(InsertColumns.ToArray()).ExecuteCommand();
            });
            return result;
        }
        public async Task<int> ExecuteCommandAsync()
        {
            if (PageSize == 0) { PageSize = 1000; }
            var result = 0;
            await this.Context.Utilities.PageEachAsync(DataList, PageSize,async pageItem =>
            {
                result += await this.Context.Insertable(pageItem).AS(TableName).EnableDiffLogEventIF(IsEnableDiffLogEvent, DiffModel).InsertColumns(InsertColumns.ToArray()).ExecuteCommandAsync();
            });
            return result;
        }

        public List<long> ExecuteReturnSnowflakeIdList()
        {
            if (PageSize == 0) { PageSize = 1000; }
            var result = new List<long>();
            this.Context.Utilities.PageEach(DataList, PageSize, pageItem =>
            {
                result.AddRange( this.Context.Insertable(pageItem).AS(TableName).EnableDiffLogEventIF(IsEnableDiffLogEvent, DiffModel).InsertColumns(InsertColumns.ToArray()).ExecuteReturnSnowflakeIdList());
            });
            return result;
        }
        public async Task<List<long>> ExecuteReturnSnowflakeIdListAsync()
        {
            if (PageSize == 0) { PageSize = 1000; }
            var result = new List<long>();
            await  this.Context.Utilities.PageEachAsync(DataList, PageSize,async pageItem =>
            {
                result.AddRange(await  this.Context.Insertable(pageItem).AS(TableName).EnableDiffLogEventIF(IsEnableDiffLogEvent, DiffModel).InsertColumns(InsertColumns.ToArray()).ExecuteReturnSnowflakeIdListAsync());
            });
            return result;
        }
    }
}
