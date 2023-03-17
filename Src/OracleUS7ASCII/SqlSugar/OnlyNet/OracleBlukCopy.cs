using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class OracleBlukCopy
    {
        public List<IGrouping<int, DbColumnInfo>> DbColumnInfoList { get;  set; }
        public InsertBuilder InsertBuilder { get;  set; }
        public ISqlBuilder Builder { get;  set; }
        public SqlSugarProvider Context { get;  set; }
        public object[] Inserts { get;  set; }

        public int ExecuteBulkCopy()
        {

            throw new Exception("Only.net CORE is supported");

        }

        public async Task<int> ExecuteBulkCopyAsync()
        {
            await Task.Delay(0);
            throw new Exception("Only.net CORE is supported");
        }
    }

}