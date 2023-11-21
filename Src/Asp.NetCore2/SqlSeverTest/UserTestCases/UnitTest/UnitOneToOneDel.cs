using SqlSeverTest.UserTestCases;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class UnitOneToOneDel
    {
        public static void Init()
        {
            var Db = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = Config.ConnectionString,
                IsAutoCloseConnection = true
            });
            Db.CodeFirst.InitTables<School, Grade>();
            Db.DbMaintenance.TruncateTable<School, Grade>();
            Db.Insertable(new School()
            {
                Id = 1,
                Name = "a"
            }).ExecuteCommand();
            Db.Insertable(new Grade()
            {
                 Name= "b",
                  SchoolId=1,
            }).ExecuteCommand();
            NewUnitTest.Db.Queryable<School>()
                .IncludeLeftJoin(x => x.Grades)
                .GroupBy(x => x.Grades.Id)
                .Select(x => new
                {
                    name = x.Grades.Id
                })
                .ToList();
            Db.DeleteNav<School>(s => s.Id.Equals(1))
                .Include(s => s.Grades).ExecuteCommandAsync().GetAwaiter().GetResult();
            //Console.ReadLine();

        }
    }

    [SugarTable("unitSchooldfadsaa")]

    public class School
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string? Name { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(Id), nameof(Grade.SchoolId))]
        public Grade Grades { get; set; }
    }
    [SugarTable("unitGradedfadsaa")]
    public class Grade
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int SchoolId { get; set; }
    }
}
