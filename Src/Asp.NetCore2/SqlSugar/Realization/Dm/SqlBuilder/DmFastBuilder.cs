using Dm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
   
    public class DmFastBuilder:FastBuilder,IFastBuilder
    {
        public override bool IsActionUpdateColumns { get; set; } = true;
        public override DbFastestProperties DbFastestProperties { get; set; } = new DbFastestProperties() {
          HasOffsetTime=true
        };
        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {

            DmBulkCopy bulkCopy = GetBulkCopyInstance();
            bulkCopy.DestinationTableName = dt.TableName;
            try
            {
                bulkCopy.WriteToServer(dt);
                await Task.Delay(0);//No Support Async
            }
            catch (Exception ex)
            {
                CloseDb();
                throw ex;
            }
            CloseDb();
            return dt.Rows.Count;
        }
        public DmBulkCopy GetBulkCopyInstance()
        {
            DmBulkCopy copy;
            if (this.Context.Ado.Transaction == null)
            {
                copy = new DmBulkCopy((DmConnection)this.Context.Ado.Connection);
            }
            else
            {
                copy = new DmBulkCopy((DmConnection)this.Context.Ado.Connection, DmBulkCopyOptions.Default, (DmTransaction)this.Context.Ado.Transaction);
            }
            if (this.Context.Ado.Connection.State == ConnectionState.Closed)
            {
                this.Context.Ado.Connection.Open();
            }
            copy.BulkCopyTimeout = this.Context.Ado.CommandTimeOut;
            return copy;
        }

    }
}
