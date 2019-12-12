using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformanceTest
{
    public class Config
    {
       public static  string connectionString = "server=.;uid=sa;pwd=haosql;database=SqlSugarTest";
       public static ConnectionConfig SugarConfig =new ConnectionConfig() {  IsAutoCloseConnection=true,InitKeyType = InitKeyType.SystemTable, ConnectionString = Config.connectionString, DbType = DbType.SqlServer };
        public static SqlSugarClient GetSugarConn()
        {
            return new SqlSugarClient(SugarConfig);
        }
    }
}
