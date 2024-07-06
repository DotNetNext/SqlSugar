using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class _7_GroupQuery
    {
        public static void Init()
        {
            var db = DbHelper.GetNewDb();
            List<Student> students = new List<Student>
            {
                new Student { Id = 1, Name = "Alice", Age = 20 },
                new Student { Id = 2, Name = "Bob", Age = 22 },
                new Student { Id = 3, Name = "Alice", Age = 21 },
                new Student { Id = 4, Name = "Charlie", Age = 19 },
                new Student { Id = 5, Name = "Bob", Age = 20 }
            };

            // Initialize database table structure; create if not exists
            db.CodeFirst.InitTables<Student>();

            // Truncate all data in the specified table
            db.DbMaintenance.TruncateTable<Student>();

            // Insert datas
            db.Insertable(students).ExecuteCommand();

            // Grouping Query Example
            var groupedStudents = db.Queryable<Student>()
                                     .GroupBy(s => s.Name)
                                     .Select(g => new
                                     {
                                         Name = g.Name, // Student Name
                                         Count = SqlFunc.AggregateCount(g.Id), // Count of Students
                                         AverageAge = SqlFunc.AggregateAvg(g.Age), // Average Age
                                         MaxAge = SqlFunc.AggregateMax(g.Age), // Maximum Age
                                         MinAge = SqlFunc.AggregateMin(g.Age) // Minimum Age
                                     })
                                     .ToList();


            // Distinct Query Example
            var distinctNames = students.Select(s => s.Name).Distinct().ToList();


            // Group First Record Example
            var groupFirstRecord = db.Queryable<Student>()
                                       .Select(g => new
                                       {
                                           index = SqlFunc.RowNumber(SqlFunc.Desc(g.Id), g.Name),
                                           Id = g.Id,
                                           Name = g.Name,
                                           Age =g.Age 
                                       })
                                       .MergeTable()
                                       .Where(it => it.index == 1)
                                       .ToList();
        }

        [SqlSugar.SugarTable("Student07")]
        public class Student
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; } // Student ID
            public string Name { get; set; } // Student Name
            public int Age { get; set; } // Student Age
        }
    }
}
