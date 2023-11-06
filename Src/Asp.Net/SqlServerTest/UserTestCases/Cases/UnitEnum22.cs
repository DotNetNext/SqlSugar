using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UnitEnum22
    {
        public static void Init()
        {
            var _db = NewUnitTest.Db;
            _db.CodeFirst.InitTables<TestTbl>();

         

            TestTbl updateTestTbl = new TestTbl()
            {
                Name = DateTime.Now.Ticks.ToString(),
                 Status=Status.B
            };
            var success =   _db.Updateable(updateTestTbl)
                .IgnoreColumns(ignoreAllNullColumns: true, ignoreAllDefaultValue: true)
                .Where(x => x.Id == 1)
                .ToSql ();
            if (!success.Key.Contains("status")) { throw new Exception("unit error"); }

            updateTestTbl.Status = Status.A;

            var success2 = _db.Updateable(updateTestTbl)
                  .IgnoreColumns(ignoreAllNullColumns: true, ignoreAllDefaultValue: true)
                  .Where(x => x.Id == 1)
                  .ToSql();

            if (success2.Key.Contains("status")) { throw new Exception("unit error"); }

            //Assert.NotNull(testTbl);
            //Assert.Equal(flag, testTbl.Flag);
            //Assert.Equal(status, testTbl.Status);

        }

        public enum Status
        {
            A = 0,
            B = 1
        }
        [SugarTable("test_tbl")]
        public sealed class TestTbl
        {
            [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            [SugarColumn(ColumnName = "name")]
            public string Name { get; set; }
            [SugarColumn(ColumnName = "flag")]
            public int Flag { get; set; }
            [SugarColumn(ColumnName = "status")]
            public Status  Status { get; set; }
        }
    }
}
