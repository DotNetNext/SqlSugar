using DuckDB;
using DuckDB.NET.Data; 
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.DuckDB
{
    public class DuckDBFastBuilder : FastBuilder, IFastBuilder
    {
        public Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            throw new NotImplementedException();
        }
    }
}
