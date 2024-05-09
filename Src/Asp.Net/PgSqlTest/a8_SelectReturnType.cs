using SqlSugar;
using System;
using System.Collections.Generic;

namespace OrmTest
{
    internal class _a8_SelectReturnType 
    {
        internal static void Init()
        {
            var db = DbHelper.GetNewDb();

            //Create table
            //建表
            db.CodeFirst.InitTables<Student>();
            //init data
            db.Insertable(new Student() { CreateTime = DateTime.Now, Name = "aa" }).ExecuteCommand();


            // 返回匿名对象 (Return anonymous objects)
            var dynamicList = db.Queryable<Student>().Select<dynamic>().ToList();
            // SELECT * FROM Student_a8

            // 手动：返回匿名集合，支持跨程序集 (Manually: Return an anonymous collection, supporting cross-assembly)
            List<dynamic> dynamicListCrossAssembly = db.Queryable<Student>().Select(it => (dynamic)new { id = it.Id }).ToList();
            // SELECT id AS id FROM Student_a8

            // 手动：返回匿名集合，不能跨程序集 (Manually: Return an anonymous collection, within the same assembly)
            var dynamicListWithinAssembly = db.Queryable<Student>().Select(it => new { id = it.Id }).ToList();
            // SELECT id AS id FROM Student_a8

            // 手动：返回类集合-手动 (Manually: Return a class collection manually)
            List<Student> classList = db.Queryable<Student>().Select(it => new Student { Id = it.Id }).ToList();
            // SELECT id AS Id FROM Student_a8

            // 自动返回DTO集合: 请升级至 5.1.3.2 版本 (Automatically return DTO collections: Upgrade to version 5.1.3.2)
            var listDto = db.Queryable<Student>().Select<StudentDto>().ToList();

            // 自动返回DTO: 请升级至 5.1.3.35 版本 (Automatically return DTO: Upgrade to version 5.1.3.35)
            var listDtoAutoMap = db.Queryable<Student>()
                .Select(it => new StudentDto
                {
                    AppendColumn = 100 // 手动指定一列在自动映射 (Manually specify a column in automatic mapping)
                },
                true) // true 表示开启自动映射 (true indicates enabling automatic mapping)
                .ToList();
        }

        // 学生表的实体类 (Entity class for the Student table)
        [SugarTable("Student_a8")]
        public class Student
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreateTime { get; set; }
        }

        // DTO 数据传输对象 (DTO - Data Transfer Object)
        public class StudentDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreateTime { get; set; }
            public int AppendColumn { get; set; }
        }
    }
}