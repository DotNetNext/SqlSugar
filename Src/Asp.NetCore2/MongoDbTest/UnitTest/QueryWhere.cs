using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    public class QueryWhere
    {
        public static void Init() 
        {
            var db = DBHelper.DbHelper.GetNewDb(); 
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            db.Insertable(new Student() { Name = "jack",Bool=true, SchoolId =2 }).ExecuteCommand();
            db.Insertable(new Student() { Name = "tom_null", Bool = false,BoolNull=true, SchoolId =3 ,SchoolIdNull=4}).ExecuteCommand();
            var list=db.Queryable<Student>().ToList();
            if (list.First() is { } first && (first.BoolNull != null || first.SchoolIdNull != null)) Cases.ThrowUnitError();
            if (list.Last() is { } last && (last.BoolNull != true || last.SchoolIdNull != 4)) Cases.ThrowUnitError();
        }
        [SqlSugar.SugarTable("UnitStudent1ssss23s131")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public bool Bool { get; set; }
            public bool? BoolNull { get; set; }

            public int SchoolId { get; set; }
            public int? SchoolIdNull { get; set; }
        }
    }
}
