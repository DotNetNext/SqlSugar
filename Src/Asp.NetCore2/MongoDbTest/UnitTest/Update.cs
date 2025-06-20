using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    public class Update
    {
        internal static void Init()
        {
            var db = DBHelper.DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            db.Insertable(new Student() { Age = 1, Name = "11", SchoolId = "111", CreateDateTime = DateTime.Now }).ExecuteCommandAsync().GetAwaiter().GetResult();
            db.Insertable(new List<Student>() {
            new Student() { Age = 2, Name = "22", SchoolId = "222", CreateDateTime = DateTime.Now },
            new Student() { Age = 3, Name = "33", SchoolId = "333", CreateDateTime = DateTime.Now }
            }).ExecuteCommandAsync().GetAwaiter().GetResult();

            var list=db.Queryable<Student>().ToList();
            db.Insertable(new Student() { Age = 1, Name = "xx", SchoolId = "111", CreateDateTime = DateTime.Now }).ExecuteCommandAsync().GetAwaiter().GetResult();
            foreach (var item in list)
            {
                item.Name = item.Name + "haha";
            }
            db.Updateable(list).ExecuteCommand();
            var list2 = db.Queryable<Student>().ToList();
            foreach (var item in list2)
            {
                if (item.Name == "xx") continue;
                if (!item.Name.EndsWith("haha")) Cases.ThrowUnitError();
            }
        }
        [SqlSugar.SugarTable("UnitStudentdghhuesd3z1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public string SchoolId { get; set; }

            public int Age { get; set; }

            public DateTime CreateDateTime { get; set; }
        }
    }
}
