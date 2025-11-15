using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    internal class UnitDateTimeFunc
    {
        public static void Init() 
        {
            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<Student>();//清空重置
            db.Insertable(new Student() { Name = "error time", Bool = true, CreateDateTime = DateTime.Parse("2025-09-15 00:00:01") }).ExecuteCommand();
            db.Insertable(new Student() { Name = "OK time", Bool = true, CreateDateTime = DateTime.Parse("2025-09-16 00:00:01") }).ExecuteCommand();
            db.Insertable(new Student() { Name = "error time2", Bool = true, CreateDateTime = DateTime.Parse("2025-09-17 00:00:01") }).ExecuteCommand();

            var list1 = db.Queryable<Student>().Where(a => a.CreateDateTime >= DateTime.Parse("2025-09-16 00:00:00") && a.CreateDateTime <= DateTime.Parse("2025-09-16 23:59:59")).ToList();
            if (list1.First().Name != "OK time") Cases.ThrowUnitError();

            var list2=db.Queryable<Student>().Where(a => DateTime.Parse("2025-09-16 00:00:00")<=a.CreateDateTime  && a.CreateDateTime <= DateTime.Parse("2025-09-16 23:59:59")).ToList();
            if (list2.First().Name!= "OK time") Cases.ThrowUnitError();
        }
        [SqlSugar.SugarTable("UnitStudent1ssss23s131")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public bool Bool { get; set; }
            public bool? BoolNull { get; set; }

            public int SchoolId { get; set; }
            public int? SchoolIdNull { get; set; }

            public DateTime CreateDateTime { get; set; }
        }
    }
}
