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
            Db.DeleteNav<School>(s => s.Id.Equals(4))
                .Include(s => s.Grades).ExecuteCommandAsync().GetAwaiter().GetResult();
            Console.ReadLine();

        }
    }

    [SugarTable("unitSchooldfadsaa")]

    public class School
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string? Name { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(Grade.SchoolId))]
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
