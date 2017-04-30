using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class MapColumn : ExpTestBase
    {
        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer });
            return db;
        }
    }
}
