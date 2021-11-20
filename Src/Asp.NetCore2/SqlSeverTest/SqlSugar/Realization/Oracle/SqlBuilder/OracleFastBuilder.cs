using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class OracleFastBuilder : FastBuilder, IFastBuilder
    {
        public Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            throw new NotImplementedException();
        }
    }
}
