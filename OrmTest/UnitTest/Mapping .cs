using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class Mapping:UnitTestBase
    {
        private Mapping() { }
        public Mapping(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init() {

            var db = GetInstance();
            db.IgnoreColumns.Add("1", "1");
            var s1= db.Queryable<Student>().Where(it=>it.Id==1).ToSql();

            var x2 = GetInstance();
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new AttribuiteCofnig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true, EntityNamespace= "OrmTest.Models" });
            return db;
        }
    }
}
