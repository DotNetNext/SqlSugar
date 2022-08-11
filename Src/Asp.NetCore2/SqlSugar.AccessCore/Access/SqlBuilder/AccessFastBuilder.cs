using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.Access 
{
   
    public class AccessFastBuilder : FastBuilder,IFastBuilder
    {
        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {

            throw new NotSupportedException();
        }
        public SqlBulkCopy GetBulkCopyInstance()
        {
            throw new NotSupportedException();
        }

    }
}
