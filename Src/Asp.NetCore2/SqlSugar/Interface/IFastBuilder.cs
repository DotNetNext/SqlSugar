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
        EntityInfo FastEntityInfo { get; set; }
        bool IsActionUpdateColumns { get; set; }
        DbFastestProperties DbFastestProperties { get; set; }
        SqlSugarProvider Context { get; set; }
        string CharacterSet { get; set; }
        Task<int> UpdateByTempAsync(string tableName,string tempName,string [] updateColumns,string[] whereColumns);
        Task<int> ExecuteBulkCopyAsync(DataTable dt);
        Task CreateTempAsync<T>(DataTable dt) where T : class, new();
        void CloseDb();
        Task<int> Merge<T>(string tableName,DataTable dt, EntityInfo entityInfo, string[] whereColumns, string[] updateColumns,List<T> datas) where T : class, new();
    }
}
