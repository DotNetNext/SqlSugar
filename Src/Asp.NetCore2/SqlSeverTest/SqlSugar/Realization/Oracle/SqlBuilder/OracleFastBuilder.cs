using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class OracleFastBuilder : FastBuilder, IFastBuilder
    {
        private OracleBulkCopy GetBulkCopyInstance()

        {
            if (this.Context.Ado.Connection.State == ConnectionState.Closed)
            {
                this.Context.Ado.Connection.Open();
            }

            OracleBulkCopy copy;
            if (this.Context.Ado.Transaction == null)
            {
                copy = new OracleBulkCopy((OracleConnection)this.Context.Ado.Connection, Oracle.ManagedDataAccess.Client.OracleBulkCopyOptions.Default);
            }
            else
            {
                copy = new OracleBulkCopy((OracleConnection)this.Context.Ado.Connection, OracleBulkCopyOptions.UseInternalTransaction);
            }
            return copy;

        }
        public Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            OracleBulkCopy copy = GetBulkCopyInstance();
            try
            {
                copy.DestinationTableName = dt.TableName;
                copy.WriteToServer(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                CloseDb();
            }
            return Task.FromResult(dt.Rows.Count);
        }
    }
}
