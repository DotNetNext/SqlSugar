using SqlSugar;

namespace OrmTest
{
    public class _4_Subquery1_Unit
    {
        public static void Init()
        {
            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Student, School>();
            var exp1 = Expressionable.Create<School>().And(s => s.Id == 1).ToExpression();
            var resulta1 = db.Queryable<Student>()
                    .Where(a => SqlFunc.Subqueryable<School>().Where(exp1).Where(s => s.Id == a.SchoolId).Any())
                    .Select(a => a).ToList();

            //db.CodeFirst.InitTables<UnitStudentasdfa, UnitSchool123>();
            //var exp2 = Expressionable.Create<UnitSchool123>().And(s => s.Id == 1).ToExpression();
            //var resulta2 = db.Queryable<UnitStudentasdfa>()
            //        .Where(a => SqlFunc.Subqueryable<UnitSchool123>().Where(exp2).Where(s => s.Id == a.SchoolId).Any())
            //        .Select(a => a).ToList();
        }

        public class UnitSchool123
        {
            public int Id { get; set; }
        }
        public class UnitStudentasdfa
        {
            public int SchoolId { get; set; }
        }

        [SugarTable("Student0402")]
        public class Student
        {
            public int Id { get; set; }
            public int SchoolId { get; set; }
        }
        [SugarTable("School0402")]
        public class School
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
