using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformanceTest
{
    public class Config
    {
       public static  string connectionString = "server=.;uid=sa;pwd=sasa;database=SqlSugarTest";
        public static SqlSugarClient GetSugarConn()
        {
            return new SqlSugarClient(new ConnectionConfig() { InitKeyType = InitKeyType.SystemTable, ConnectionString = Config.connectionString, DbType = DbType.SqlServer });
        }
    }
}
