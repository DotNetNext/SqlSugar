using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class OracleFastBuilder:IFastBuilder
    {
        public string CharacterSet { get; set; }
        public OracleFastBuilder(EntityInfo entityInfo) 
        {
            throw new Exception("Only.net CORE is supported");
        }

        public SqlSugarProvider Context { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsActionUpdateColumns { get; set; }
        public DbFastestProperties DbFastestProperties { get; set; } = new DbFastestProperties();

        public void CloseDb()
        {
            throw new NotImplementedException();
        }

        public Task CreateTempAsync<T>(DataTable dt) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            throw new NotImplementedException();
        }
    }
}
