using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface  IFastBuilder
    {
        SqlSugarProvider Context { get; set; }
        Task<int> UpdateByTempAsync(string tableName,string tempName,string [] updateColumns,string[] whereColumns);
        Task<int> ExecuteBulkCopyAsync(DataTable dt);
        Task CreateTempAsync<T>(DataTable dt) where T : class, new();
        void CloseDb();
    }
}
