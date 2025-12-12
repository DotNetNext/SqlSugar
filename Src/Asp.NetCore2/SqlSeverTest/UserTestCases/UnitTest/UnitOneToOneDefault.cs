using SqlSeverTest.UserTestCases;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    internal class UnitOneToOneDefault
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
            StaticConfig.QueryOneToOneEnableDefaultValue = true;
            Db.Insertable(new School()
            {
                Id = 1,
                Name = "有导航"
            }).ExecuteCommand();
            Db.Insertable(new School()
            {
                Id = 2,
                Name = "无导航"
            }).ExecuteCommand();
            Db.Insertable(new Grade()
            {
                Name = "b",
                SchoolId = 1,
            }).ExecuteCommand();
      
            var list = NewUnitTest.Db.Queryable<School>()
            .Includes(x => x.Grades)
            .ToList();
            StaticConfig.QueryOneToOneEnableDefaultValue = false;
            if (list.First().Grades.Name != "b" || list.Last().Grades.Name != null) throw new Exception("unit error");

            var list2 = NewUnitTest.Db.Queryable<School>()
            .Includes(x => x.Grades)
            .ToList();
            if (list2.First().Grades.Name != "b" || list2.Last().Grades != null) throw new Exception("unit error");
        }

        [SugarTable("unitSchooldf2adsa222a")]

        public class School
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            [Navigate(NavigateType.OneToOne, nameof(Id), nameof(Grade.SchoolId))]
            public Grade Grades { get; set; } = new OneToOneInitializer<Grade>();
        }
        [SugarTable("unitGradedfad33424saa")]
        public class Grade
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string? Name { get; set; }
            public int SchoolId { get; set; }

            [SugarColumn(IsNullable = true)]
            public decimal? a { get; set; }
        }
    }
}
