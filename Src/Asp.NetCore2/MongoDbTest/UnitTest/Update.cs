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
            db.Updateable<Student>()
                .SetColumns(it => new Student() { Name = "yy" }).Where(it => it.Name == "xx").ExecuteCommand();

           var datas= db.Queryable<Student>().Where(it => it.Id == list2.Last().Id).ToList();
           if(datas.Count!=1|| datas.First().Name!="yy") Cases.ThrowUnitError();

            db.Updateable(new Student() { Name = "yy", Age = 1111, SchoolId = "1", CreateDateTime = DateTime.Now })
                 .WhereColumns(it => it.Name).ExecuteCommand();
            var data=db.Queryable<Student>().Where(it => it.Name == "yy").ToList();
            if(data.First().Age!=1111|| data.Count!=1) Cases.ThrowUnitError();

            db.Insertable(new Student() { Age = 1, Name = "ss", SchoolId = "111", CreateDateTime = DateTime.Now }).ExecuteCommand();
            db.Updateable(new List<Student>() 
            {
               new Student() { Name = "yy", Age = 22222, SchoolId = "1", CreateDateTime = DateTime.Now },
               new Student() { Name = "ss", Age = 33333, SchoolId = "1", CreateDateTime = DateTime.Now }
            }
            ).WhereColumns(it => it.Name).ExecuteCommand();
            var list3=db.Queryable<Student>().Where(it => it.Age == 33333).ToList();
            if (list3.First().Name != "ss"|| list3.Count!=1) Cases.ThrowUnitError();
            var list4 = db.Queryable<Student>().Where(it => it.Age == 22222).ToList();
            if (list4.First().Name != "yy" || list4.Count != 1) Cases.ThrowUnitError();
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
