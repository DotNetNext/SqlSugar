using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IFastest<T> where T:class,new()
    {
        IFastest<T> AS(string tableName);
        IFastest<T> PageSize(int Size);
        int BulkCopy(List<T> datas);
        Task<int> BulkCopyAsync(List<T> datas);

        int BulkUpdate(List<T> datas);
        Task<int> BulkUpdateAsync(List<T> datas);
        int BulkUpdate(List<T> datas, string[] whereColumns, string[] updateColumns);
        Task<int> BulkUpdateAsync(List<T> datas, string[] whereColumns, string[] updateColumns);

        SplitFastest<T> SplitTable();
    }
}
