using SqlSugar;
using System.Collections.Generic;
using System;

namespace OrmTest
{ 
    public class Unitadfadsyyy
    {
        public static void Init()
        {
            var db = new SqlSugarScope(new List<ConnectionConfig>()
        {
            new()
            {
                ConfigId = "Main",
                ConnectionString = $@"DataSource={Environment.CurrentDirectory}\test12.db",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true
            }
        }, client => { client.Aop.OnLogExecuting = (s, parameters) => Console.WriteLine(s); });

            db.CodeFirst.InitTables<Student1231231, StarStudent12313131>();

            var list=db.Queryable<Student1231231>()
                .IncludeLeftJoin(s => s.StartStudent)
                .Where(s => s.Name.StartsWith("张"))
                .Select(s => new StudentDto()
                {
                    StarLevel = s.StartStudent.StarLevel,
                },true)
                .ToList();
        }

        public class Student1231231
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public string Name { get; set; }

            public string Age { get; set; }

            public string Phone { get; set; }
            public string Address { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(Id), nameof(StarStudent12313131.StudentId))]
            public StarStudent12313131 StartStudent { get; set; }
        }

        public class StarStudent12313131
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public int StudentId { get; set; }

            public int StarLevel { get; set; }
        }

        public class StudentDto
        {
            public int Id { get; set; }

            public string Name { get; set; }
            public string Age { get; set; }

            public string Phone { get; set; }
            public string Address { get; set; }

            public int StarLevel { get; set; }
        }
    }
}