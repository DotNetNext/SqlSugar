using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UNavDynamic111N
    {

        public static void Init() 
        {

            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitaSchoolA, UnitaStudentA,UnitaBookA>();
            db.DbMaintenance.TruncateTable<UnitaSchoolA, UnitaStudentA, UnitaBookA>();
            db.Insertable(new UnitaSchoolA() { SchoolId = 1, CityId = 1001001, School_Name = "北大" }).ExecuteCommand();
            db.Insertable(new UnitaSchoolA() { SchoolId = 2, CityId = 2, School_Name = "清华" }).ExecuteCommand();
            db.Insertable(new UnitaSchoolA() { SchoolId = 3, CityId = 3, School_Name = "青鸟" }).ExecuteCommand();

            db.Insertable(new UnitaStudentA() { StudentId = 1, SchoolId = 1, Name = "北大jack" }).ExecuteCommand();
            db.Insertable(new UnitaStudentA() { StudentId = 2, SchoolId = 1, Name = "北大tom" }).ExecuteCommand();
            db.Insertable(new UnitaStudentA() { StudentId = 3, SchoolId = 2, Name = "清华jack" }).ExecuteCommand();
            db.Insertable(new UnitaStudentA() { StudentId = 4, SchoolId = 2, Name = "清华tom" }).ExecuteCommand();
            db.Insertable(new UnitaStudentA() { StudentId = 5, SchoolId = null, Name = "清华tom" }).ExecuteCommand();
            db.Insertable(new UnitaStudentA() { StudentId = 6, SchoolId = 3, Name = "青鸟学生" }).ExecuteCommand();

            db.Insertable(new UnitaBookA() { BookId = 1, Names = "java", studenId = 1 }).ExecuteCommand();
            db.Insertable(new UnitaBookA() { BookId = 2, Names = "c#2", studenId = 2 }).ExecuteCommand();
            db.Insertable(new UnitaBookA() { BookId = 3, Names = "c#1", studenId = 2 }).ExecuteCommand();
            db.Insertable(new UnitaBookA() { BookId = 4, Names = "php", studenId = 3 }).ExecuteCommand();
            db.Insertable(new UnitaBookA() { BookId = 5, Names = "js", studenId = 4 }).ExecuteCommand();
            db.Insertable(new UnitaBookA() { BookId = 6, Names = "北大jack", studenId = 1 }).ExecuteCommand();

            var list =db.Queryable<UnitaStudentA>()
                .Includes(x => x.SchoolA).Where(x=>x.SchoolA.School_Name!=null).ToList();

            var list2 = db.Queryable<UnitaStudentA>()
                .Includes(x => x.Books).Where(x=>x.Books.Any()).ToList();

            var list3 = db.Queryable<UnitaStudentA>()
              .IncludesAllFirstLayer().ToList();

            if (list3.First().Books.Count() == 0||list3.First().SchoolA==null) 
            {
                throw new Exception("unit error");
            }
        }
        public class UnitaStudentA
        {
            public int StudentId { get; set; }
            public string Name { get; set; }
            [SugarColumn(IsNullable = true)]
            public int? SchoolId { get; set; }
            [Navigate(NavigateType.OneToOne, nameof(SchoolId),nameof(UnitaSchoolA.SchoolId))]
            public UnitaSchoolA SchoolA { get; set; }
            [Navigate(NavigateType.OneToMany, nameof(UnitaBookA.studenId),nameof(StudentId))]
            public List<UnitaBookA> Books { get; set; }


        }
        public class UnitaSchoolA
        {
            public int SchoolId { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CityId { get; set; }
            [SugarColumn(ColumnName = "SchoolName")]
            public string School_Name { get; set; }

        }
        public class UnitaBookA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int BookId { get; set; }
            [SugarColumn(ColumnName = "Name")]
            public string Names { get; set; }
            public int studenId { get; set; }
        }
    }
}
