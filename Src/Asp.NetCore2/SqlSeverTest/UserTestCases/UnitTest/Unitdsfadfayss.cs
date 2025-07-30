using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlSugar;

namespace OrmTest;

public static class Unitdfafaa
{
    public static void Init() 
    {
        Main().GetAwaiter().GetResult();
    }
    public static async Task Main()
    {
        await Task.CompletedTask;
        var db = NewUnitTest.Db;

        db.Aop.OnLogExecuted = (s, parameters) => Console.WriteLine(s);

        db.DbMaintenance.CreateDatabase();

        //建表 
        db.CodeFirst.InitTables<Students>();
        db.CodeFirst.InitTables<StudentsSub>();
        db.CodeFirst.InitTables<StudentsGender>();
        db.CodeFirst.InitTables<StudentLogs>();

        var data = new List<Students>()
        {
            new Students() { Id = 1, Name = "李雷" },
            new Students() { Id = 2, Name = "韩梅梅", ParentId = 1 },
            new Students() { Id = 3, Name = "王强", ParentId  = 1 },
            new Students() { Id = 4, Name = "赵敏", ParentId  = 1 },
        };

        await db.Storageable(data).ExecuteCommandAsync();

        var data2 = new List<StudentsSub>()
        {
            new StudentsSub() { Id = 1, StudentId = 1, SubName = "李雷的子1" },
            new StudentsSub() { Id = 2, StudentId = 2, SubName = "韩梅梅的子1" },
            new StudentsSub() { Id = 3, StudentId = 3, SubName = "王强的子1" },
            new StudentsSub() { Id = 4, StudentId = 4, SubName = "赵敏的子1" },
        };

        await db.Storageable(data2).ExecuteCommandAsync();

        var result = await db.Queryable<Students>()
            .Includes(s => s.Gender)
           .LeftJoin<StudentLogs>((s, sl) => s.Id == sl.StudentId)
           .Select((s, sl) => new StudentsDto()
           {
               LogContent = sl.LogContent,
               Subs = SqlFunc.Subqueryable<StudentsSub>()
                   .Where(ss => ss.StudentId == s.Id)
                   .ToList<StudentsSubDto>(),
           }, true)
           .ToListAsync();
        if (!result.First().Subs.Any()) throw new Exception("unit error");
        Console.WriteLine(JsonConvert.SerializeObject(result));
         
    }

    [SugarTable("unitStudentssdsaaa")]
    public class Students
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(DefaultValue = "0")]
        public int GenderId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(StudentsGender.Id))]
        public StudentsGender Gender { get; set; }

        public string Name { get; set; }

        public long ParentId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(StudentsSub.StudentId))]
        public List<StudentsSub> Subs { get; set; }
    }
    [SugarTable("unitStudentsGendersaaa")]
    public class StudentsGender
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }

        public string Name { get; set; }
    }
    [SugarTable("unitStudentLogssaaa")]
    public class StudentLogs
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string LogContent { get; set; }
    }
    [SugarTable("unitStudentsSubsaaa")]

    public class StudentsSub
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }

        public long StudentId { get; set; }

        [SugarColumn(IsNullable = true)]
        public string SubName { get; set; }
    }

    public class StudentsDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public long ParentId { get; set; }

        public string LogContent { get; set; }
        public List<StudentsSubDto> Subs { get; set; }
    }

    public class StudentsSubDto
    {
        public int Id { get; set; }

        public long StudentId { get; set; }

        public string SubName { get; set; }
    }
}