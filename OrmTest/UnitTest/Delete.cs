using OrmTest.Models;
using OrmTest.UnitTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Delete : UnitTestBase
    {
        private Delete() { }
        public Delete(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init()
        {
            var db = GetInstance();
            //by entity
            var s1= db.Deleteable<Student>().Where(new Student() { Id = 1 }).ToSql();
            //use lock
            var s2 = db.Deleteable<Student>().With(SqlWith.RowLock).ToSql();
            //by primary key
            var s3 = db.Deleteable<Student>().In(1).ToSql();
            //by primary key array
            var s4 = db.Deleteable<Student>().In(new int[] { 1,2}).ToSql();
            //by expression
            var s5 = db.Deleteable<Student>().Where(it=>it.Id==1).ToSql();
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true });
            return db;
        }
    }
}
