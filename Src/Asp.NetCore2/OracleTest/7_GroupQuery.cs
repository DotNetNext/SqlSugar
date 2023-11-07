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
             
            // 初始化数据库表结构，如果表不存在则创建 (Initialize database table structure; create if not exists)
             db.CodeFirst.InitTables<Student>();

            // 清空指定表中的所有数据 (Truncate all data in the specified table)
            db.DbMaintenance.TruncateTable<Student>();

            //插入记录(Insert datas)
            db.Insertable(students).ExecuteCommand();

            // 分组查询示例 (Grouping Query Example)
            var groupedStudents = db.Queryable<Student>()
                                     .GroupBy(s => s.Name)
                                     .Select(g => new
                                     {
                                         Name = g.Name, // 学生姓名 (Student Name)
                                         Count = SqlFunc.AggregateCount(g.Id), // 学生数量 (Count of Students)
                                         AverageAge = SqlFunc.AggregateAvg(g.Age), // 平均年龄 (Average Age)
                                         MaxAge = SqlFunc.AggregateMax(g.Age), // 最大年龄 (Maximum Age)
                                         MinAge = SqlFunc.AggregateMin(g.Age) // 最小年龄 (Minimum Age)
                                     })
                                     .ToList();


            // 去重查询示例 (Distinct Query Example)
            var distinctNames = students.Select(s => s.Name).Distinct().ToList();


            // 分组取第一条记录示例 (Group First Record Example)
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
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; } // 学生ID (Student ID)
            public string Name { get; set; } // 学生姓名 (Student Name)
            public int Age { get; set; } // 学生年龄 (Student Age)
        }
    }
}
