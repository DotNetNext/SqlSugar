using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.Access 
{
    public class AccessBlukCopy
    {
        internal List<IGrouping<int, DbColumnInfo>> DbColumnInfoList { get;   set; }
        internal SqlSugarProvider Context { get;   set; }
        internal ISqlBuilder Builder { get; set; }
        internal InsertBuilder InsertBuilder { get; set; }
        internal object[] Inserts { get;  set; }

        public int ExecuteBulkCopy()
        {
            throw new NotSupportedException();
        }

        public async Task<int> ExecuteBulkCopyAsync()
        {
            throw new NotSupportedException();
        }
    }
}
