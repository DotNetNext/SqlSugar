using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class Mapping:ExpTestBase
    {
        private Mapping() { }
        public Mapping(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init() {

            var db = GetInstance();
            var s1= db.Queryable<Student>().ToSql();
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new AttrbuitesCofnig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true, EntityNamespace= "OrmTest.Models" });
            return db;
        }
    }
}
