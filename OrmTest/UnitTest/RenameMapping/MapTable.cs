using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class MapTable : ExpTestBase
    {
        public void Init()
        {
            var db = GetInstance();
            for (int i = 0; i < 200; i++)
            {
                var x = db.Queryable<Student>().ToList(); 
            }

        }
        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true });
            return db;
        }
    }
}
