using System;
using System.Collections.Generic;
using System.Data; 
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IFastest<T> where T:class,new()
    {
        IFastest<T> RemoveDataCache();
        IFastest<T> RemoveDataCache(string cacheKey);
        IFastest<T> AS(string tableName);
        IFastest<T> PageSize(int Size);
        IFastest<T> OffIdentity();
        IFastest<T> SetCharacterSet(string CharacterSet);
        IFastest<T> EnableDataAop();
        int BulkCopy(List<T> datas);
        Task<int> BulkCopyAsync(List<T> datas);
        int BulkCopy(string tableName,DataTable dataTable);
        int BulkCopy(DataTable dataTable);
        Task<int> BulkCopyAsync(string tableName, DataTable dataTable);
        Task<int> BulkCopyAsync(DataTable dataTable);

        int BulkUpdate(List<T> datas);
        Task<int> BulkUpdateAsync(List<T> datas);
        int BulkUpdate(List<T> datas, string[] whereColumns, string[] updateColumns);
        int BulkUpdate(List<T> datas, string[] whereColumns);
        Task<int> BulkUpdateAsync(List<T> datas, string[] whereColumns);
        Task<int> BulkUpdateAsync(List<T> datas, string[] whereColumns, string[] updateColumns);
        int BulkUpdate(string tableName,DataTable dataTable, string[] whereColumns, string[] updateColumns);
        int BulkUpdate(DataTable dataTable, string[] whereColumns, string[] updateColumns);
        int BulkUpdate(DataTable dataTable, string[] whereColumns);
        Task<int> BulkUpdateAsync(string tableName, DataTable dataTable, string[] whereColumns, string[] updateColumns);
        Task<int> BulkUpdateAsync(DataTable dataTable, string[] whereColumns);
        SplitFastest<T> SplitTable();
        Task<int> BulkMergeAsync(List<T> datas);
        int BulkMerge(List<T> datas);
        Task<int> BulkMergeAsync(List<T> datas, string[] whereColumns);
        int BulkMerge(List<T> datas, string[] whereColumns);
        Task<int> BulkMergeAsync(List<T> datas, string[] whereColumns, string[] updateColumns);
        int BulkMerge(List<T> datas, string[] whereColumns, string[] updateColumns);

    }
}
