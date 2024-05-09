using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{ 
    public class _4_Subquery
    {
        public static void Init()
        {

            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Student, School>();
            db.DbMaintenance.TruncateTable<Student, School>();

            db.Insertable(new Student() { Id = 1, SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new School() { Id = 1, Name = "Harvard School" }).ExecuteCommand();
            db.Insertable(new Student() { Id = 2, SchoolId = 2 }).ExecuteCommand();
            db.Insertable(new School() { Id = 2, Name = "haha School" }).ExecuteCommand();

            //subquery select
            var result = db.Queryable<Student>()
               .Select(st => new
               {
                   SchoolName = SqlFunc.Subqueryable<School>().Where(s => s.Id == st.SchoolId).Select(s => s.Name),
                   MaxSchoolId = SqlFunc.Subqueryable<School>().Where(s => s.Id == st.SchoolId).Select(s => SqlFunc.AggregateMax(s.Id)),
                   MaxSchoolId2 = SqlFunc.Subqueryable<School>().Where(s => s.Id == st.SchoolId).Max(s => s.Id),
               })
               .ToList();

            //Exists:
            //SELECT [Id],[SchoolId] FROM [Student0402] [it]  WHERE (EXISTS ( SELECT * FROM [School0402] [s]  WHERE ( [Id] = [it].[SchoolId] ) ))
            var result2 = db.Queryable<Student>()
              .Where(it => SqlFunc.Subqueryable<School>().Where(s => s.Id == it.SchoolId).Any())
              .ToList();

            //In:
            //SELECT [Id],[SchoolId] FROM [Student0402] [it]  WHERE  [Id]  in (SELECT [Id] FROM [School0402] [s]  GROUP BY [Id])
            var result3 = db.Queryable<Student>()
            .Where(it => it.Id == SqlFunc.Subqueryable<School>().GroupBy(s => s.Id).Select(s => s.Id))
            .ToList();

            //Equal:
            //SELECT [Id],[SchoolId] FROM [Student0402] [it]  WHERE ( [Id] =(SELECT TOP 1 [s].[Id] FROM [School0402] [s] ))
            var result4 = db.Queryable<Student>()
            .Where(it => it.Id == SqlFunc.Subqueryable<School>().Select(s => s.Id))
            .ToList();
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
