using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgSqlTest.UserTestCases
{
    internal class Unit1sdgsaaysdfa
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<TestClass1>();
            db.CodeFirst.InitTables<TestClass2>();
            db.DbMaintenance.TruncateTable<TestClass1, TestClass2>();
            var tc = new TestClass1
            {
                Version=1,
                Name = "123",
                List = new List<TestClass2>() { new TestClass2 { Name = "abc", Version = 1, }, new TestClass2 { Version = 1, Name = "def" } }
            };
            tc = db.InsertNav(tc).Include(e => e.List).ExecuteReturnEntity();
            var rows=db.UpdateNav(tc, new UpdateNavRootOptions
            {
                IsOptLock = true
            }).Include(e => e.List)
              .ExecuteCommand();
        }
    }
}
    [SugarTable("Unitsfafa1123f")]
    public class TestClass1
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }
        [SugarColumn(ColumnName = "Version", IsEnableUpdateVersionValidation = true)]
        public long? Version { get; set; }
        [SugarColumn(ColumnName = "名称1", Length = 32)]
        public string? Name { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(TestClass2.Id1))]
        public List<TestClass2> List { get; set; }
    }

    [SugarTable("Unitsddfa12212")]
    public class TestClass2
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }
        [SugarColumn(ColumnName = "Version", IsEnableUpdateVersionValidation = true)]
        public long? Version { get; set; }
        [SugarColumn(ColumnName = "名称1", Length = 32)]
        public string? Name { get; set; }
        [SugarColumn(ColumnName = "名称2", Length = 32)]
        public long? Id1 { get; set; }
    }
 