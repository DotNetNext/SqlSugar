using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public class Delete
    {
        internal static void Init()
        {
            var db = DBHelper.DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            var id=db.Insertable(new Student() { Age = 1, Name = "11", SchoolId = "111", CreateDateTime = DateTime.Now.AddDays(1) }).ExecuteReturnPkList<string>();
            var ids=db.Insertable(new List<Student>() {
            new Student() { Age = 2, Name = "22", SchoolId = "222", CreateDateTime = DateTime.Now.AddDays(-1) },
            new Student() { Age = 3, Name = "33", SchoolId = "333", CreateDateTime = DateTime.Now.AddDays(1) }
            }).ExecuteCommandAsync().GetAwaiter().GetResult();
            var count=db.Queryable<Student>().Count();
            var rows=db.Deleteable<Student>().Where(it => it.CreateDateTime < DateTime.Now).ExecuteCommand();
            var count2 = db.Queryable<Student>().Count();
            if (count2 != 2) Cases.ThrowUnitError();
            var rows2 = db.Deleteable<Student>().In(id).ExecuteCommand();
            var count3 = db.Queryable<Student>().Count();
            if (count3 != 1) Cases.ThrowUnitError();
            var rows3 = db.Deleteable<Student>().In(db.Queryable<Student>().First().Id).ExecuteCommandAsync().GetAwaiter().GetResult();
            var count4 = db.Queryable<Student>().Count();
            if (count4 !=0) Cases.ThrowUnitError(); 
        }
        [SqlSugar.SugarTable("UnitStudent1ssdds3z1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public string SchoolId { get; set; }

            public int Age { get; set; }

            public DateTime CreateDateTime { get; set; }
        }
    }
}
