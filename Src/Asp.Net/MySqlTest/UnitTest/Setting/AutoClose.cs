using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class AutoClose : UnitTestBase
    {
        public AutoClose(int eachCount)
        {
            this.Count = eachCount;
        }
        public void Init()
        {
            //IsAutoCloseConnection
            for (int i = 0; i < this.Count; i++)
            {
                var db = GetInstance();
                var x = db.Queryable<Student>().ToList();
            }
        }
        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.MySql, IsAutoCloseConnection = true });
            return db;
        }
    }
}
