﻿using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.Odbc 
{
   
    public class OdbcFastBuilder:FastBuilder,IFastBuilder
    {
        public override bool IsActionUpdateColumns { get; set; } = true;
        public override DbFastestProperties DbFastestProperties { get; set; } = new DbFastestProperties() {
          HasOffsetTime=true
        };
        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {

            SqlBulkCopy bulkCopy = GetBulkCopyInstance();
            bulkCopy.DestinationTableName = dt.TableName;
            try
            {
                await bulkCopy.WriteToServerAsync(dt);
            }
            catch (Exception ex)
            {
                CloseDb();
                throw ex;
            }
            CloseDb();
            return dt.Rows.Count;
        }
        public SqlBulkCopy GetBulkCopyInstance()
        {
            SqlBulkCopy copy;
            if (this.Context.Ado.Transaction == null)
            {
                copy = new SqlBulkCopy((SqlConnection)this.Context.Ado.Connection);
            }
            else
            {
                copy = new SqlBulkCopy((SqlConnection)this.Context.Ado.Connection, SqlBulkCopyOptions.CheckConstraints, (SqlTransaction)this.Context.Ado.Transaction);
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
