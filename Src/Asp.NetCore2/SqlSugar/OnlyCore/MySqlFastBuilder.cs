using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class MySqlFastBuilder : FastBuilder, IFastBuilder
    {
        private async Task<int> MySqlConnectorBulkCopy(DataTable dt)
        {
            try
            {
                this.Context.Open();
                var tran = (MySqlTransaction)this.Context.Ado.Transaction;
                var connection = (MySqlConnection)this.Context.Ado.Connection;
                MySqlBulkCopy bulkCopy = new MySqlBulkCopy(connection, tran);
                bulkCopy.DestinationTableName= dt.TableName; 
                await bulkCopy.WriteToServerAsync(dt);
                return dt.Rows.Count;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseDb();
            }
        }
    }
}