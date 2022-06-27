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
            db.CodeFirst.InitTables<UnitaSchoolA, UnitaStudentA>();
            db.DbMaintenance.TruncateTable<UnitaSchoolA, UnitaStudentA>();
            db.Insertable(new UnitaSchoolA() { SchoolId = 1, CityId = 1001001, School_Name = "北大" }).ExecuteCommand();
            db.Insertable(new UnitaSchoolA() { SchoolId = 2, CityId = 2, School_Name = "清华" }).ExecuteCommand();
            db.Insertable(new UnitaSchoolA() { SchoolId = 3, CityId = 3, School_Name = "青鸟" }).ExecuteCommand();

            db.Insertable(new UnitaStudentA() { StudentId = 1, SchoolId = 1, Name = "北大jack" }).ExecuteCommand();
            db.Insertable(new UnitaStudentA() { StudentId = 2, SchoolId = 1, Name = "北大tom" }).ExecuteCommand();
            db.Insertable(new UnitaStudentA() { StudentId = 3, SchoolId = 2, Name = "清华jack" }).ExecuteCommand();
            db.Insertable(new UnitaStudentA() { StudentId = 4, SchoolId = 2, Name = "清华tom" }).ExecuteCommand();
            db.Insertable(new UnitaStudentA() { StudentId = 5, SchoolId = null, Name = "清华tom" }).ExecuteCommand();
            db.Insertable(new UnitaStudentA() { StudentId = 6, SchoolId = 3, Name = "青鸟学生" }).ExecuteCommand();

            var list=db.Queryable<UnitaStudentA>()
                .Includes(x => x.SchoolA).ToList();
        }
        public class UnitaStudentA
        {
            public int StudentId { get; set; }
            public string Name { get; set; }
            [SugarColumn(IsNullable = true)]
            public int? SchoolId { get; set; }
            [Navigate(NavigateType.OneToOne, nameof(SchoolId),nameof(UnitaSchoolA.SchoolId))]
            public UnitaSchoolA SchoolA { get; set; }
 

        }
        public class UnitaSchoolA
        {
            public int SchoolId { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CityId { get; set; }
            [SugarColumn(ColumnName = "SchoolName")]
            public string School_Name { get; set; }

        }
    }
}
