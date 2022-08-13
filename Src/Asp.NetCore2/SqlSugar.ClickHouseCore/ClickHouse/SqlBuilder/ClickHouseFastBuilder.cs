using ClickHouse;
using ClickHouse.Client.ADO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.ClickHouse
{
    public class ClickHouseFastBuilder : FastBuilder, IFastBuilder
    {
     
        public ClickHouseFastBuilder(EntityInfo entityInfo)
        {
            throw new NotSupportedException("NotSupportedException");
        }

        public override string UpdateSql { get; set; } = @"UPDATE  {1}    SET {0}  FROM   {2}  AS TE  WHERE {3}
";

 
        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            await Task.FromResult(0);
            throw new NotSupportedException("NotSupportedException");
        }

        private  void BulkCopy(DataTable dt, string copyString, ClickHouseConnection conn, List<DbColumnInfo> columns)
        {
            throw new NotSupportedException("NotSupportedException");
        }
      
        
    }
}
