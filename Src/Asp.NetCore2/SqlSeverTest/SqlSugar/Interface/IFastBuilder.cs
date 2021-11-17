using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface  IFastBuilder
    {
        SqlSugarProvider Context { get; set; }

        Task<int> ExecuteBulkCopyAsync(DataTable dt);
    }
}
