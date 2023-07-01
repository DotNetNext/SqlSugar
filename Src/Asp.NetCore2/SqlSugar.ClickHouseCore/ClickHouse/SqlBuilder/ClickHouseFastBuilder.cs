using ClickHouse;
using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar.ClickHouse
{
    public class ClickHouseFastBuilder : FastBuilder, IFastBuilder
    {
 
        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            ClickHouseBulkCopy bulkCopy;
            var conn = new ClickHouseConnection(this.Context.CurrentConnectionConfig.ConnectionString);
            bulkCopy = new ClickHouseBulkCopy(conn);
            await conn.OpenAsync();
            bulkCopy.BatchSize = 100000;
            bulkCopy.DestinationTableName = dt.TableName;
            try
            {
                await bulkCopy.WriteToServerAsync(dt, new CancellationToken());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                await conn.CloseAsync();
            } 
            return dt.Rows.Count;
        }
        public override Task CreateTempAsync<T>(DataTable dt) 
        {
            throw new Exception("The development of BulkCopyUpdate ");
        }
        //public async Task<ClickHouseBulkCopy> GetBulkCopyInstance()
        //{
         
        //    return copy;
        //}
    }
}
