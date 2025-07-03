using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq; 
using SqlSugar;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    public class QuerySelect2
    {
        internal static void Init()
        { 
            var db =DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();

            db.Insertable(new Student() { Name = "jack", SchoolId = "1" }).ExecuteCommand();
            db.Insertable(new Student2() { Name = "tom", Age=100 }).ExecuteCommand();

            var list=db.Queryable<Student3>().ToList();
            var first = list.First();
            var last = list.Last();

            if (first.SchoolId != "1" || first.Name != "jack"||first.Age!=0) Cases.ThrowUnitError();
            if (last.SchoolId != null || last.Name != "tom"|| last.Age!=100) Cases.ThrowUnitError();
        } 
        [SqlSugar.SugarTable("UnitStudent1231sds3z1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public string SchoolId { get; set; }

        }
        [SqlSugar.SugarTable("UnitStudent1231sds3z1")]
        public class Student2 : MongoDbBase
        {
            public string Name { get; set; }
            public int Age { get; set; } 
        }
        [SqlSugar.SugarTable("UnitStudent1231sds3z1")]
        public class Student3 : MongoDbBase
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string SchoolId { get; set; }
        }
    }
}
