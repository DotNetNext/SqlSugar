using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SugarConnection:IDisposable
    {
        public IDbConnection conn { get; set; }
        public bool IsAutoClose { get; set; }
        public ISqlSugarClient Context { get; set; }

        public void Dispose()
        {
            conn.Close();
            this.Context.CurrentConnectionConfig.IsAutoCloseConnection = IsAutoClose;
        }
    }
}
