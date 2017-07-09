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
            var t1= db.Deleteable<Student>().Where(new Student() { Id = 1 }).ToSql();
            base.Check(@"DELETE FROM `STudent` WHERE `Id` IN ('1') ",
               null,
               t1.Key,
               null, "Delte t1 error"
           );
            //use lock
            var t2 = db.Deleteable<Student>().With(SqlWith.RowLock).ToSql();
            base.Check(@"DELETE FROM `STudent`",
               null,
               t2.Key,
               null, "Delte t2 error"
           );

            //by primary key
            var t3 = db.Deleteable<Student>().In(1).ToSql();
            base.Check(@"DELETE FROM `STudent` WHERE `Id` IN ('1') ",
               null,
               t3.Key,
               null, "Delte tt error"
           );
            //by primary key array
            var t4 = db.Deleteable<Student>().In(new int[] { 1,2}).ToSql();
            base.Check(@"DELETE FROM `STudent` WHERE `Id` IN ('1','2') ", null, t4.Key, null, "Update t4 error");

            //by expression
            var t5 = db.Deleteable<Student>().Where(it=>it.Id==1).ToSql();
            base.Check(@"DELETE FROM `STudent` WHERE ( `ID` = @Id0 ) ", new List<SugarParameter>() {
                new SugarParameter("@Id0",1)
            }, t5.Key, t5.Value, "Delte t5 error");

            var t6 = db.Deleteable<Student>().Where("id=@id",new { id=1}).ToSql();
            base.Check(@"DELETE FROM `STudent` WHERE id=@id", new List<SugarParameter>() {
                new SugarParameter("@id",1)
            }, t6.Key, t6.Value, "Delte t6 error");
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.Sqlite, IsAutoCloseConnection = true });
            return db;
        }
    }
}
