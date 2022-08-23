using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UnitTestReturnPkList
    {
        public static void Init()
        {
            TestIdentity();
            TestLong();
            TestGuid();
        }

        private static void TestGuid()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitGuid123>();
            var ids = db.Insertable(new UnitGuid123() { Name = "a" }).ExecuteReturnPkList<Guid>();
            if (ids.Count() != 1) { throw new Exception("unit error"); }
            if (db.Queryable<UnitGuid123>().In(ids).Count() !=1) { throw new Exception("unit error"); }
            var ids2 = db.Insertable(new List<UnitGuid123>() { new UnitGuid123() { Name = "c" }, new UnitGuid123() { Name = "c" } }).ExecuteReturnPkList<Guid>();
            if (ids2.Count() != 2) { throw new Exception("unit error"); }
            if (db.Queryable<UnitGuid123>().In(ids2).Count() != 2) { throw new Exception("unit error"); }
        }

        private static void TestLong()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitLong1231>();
            var ids = db.Insertable(new UnitLong1231() { Name = "a" }).ExecuteReturnPkList<long>();
            if (ids.Count(z => z > 0) != 1) { throw new Exception("unit error"); }
            var ids2 = db.Insertable(new List<UnitLong1231>() { new UnitLong1231() { Name = "c" }, new UnitLong1231() { Name = "c" } }).ExecuteReturnPkList<long>();
            if (ids2.Count(z => z > 0) != 2) { throw new Exception("unit error"); }
            if(db.Queryable<UnitLong1231>().In(ids2).Count()!=2) { throw new Exception("unit error"); }
        }

        private static void TestIdentity()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitIdentity01>();
            var ids = db.Insertable(new UnitIdentity01() { Name = "a" }).ExecuteReturnPkList<int>();
            if (ids.Count(z => z > 0) != 1) { throw new Exception("unit error"); }
            var ids2 = db.Insertable(new List<UnitIdentity01>() { new UnitIdentity01() { Name = "c" }, new UnitIdentity01() { Name = "c" } }).ExecuteReturnPkList<int>();
            if (ids2.Count(z => z > 0) != 2) { throw new Exception("unit error"); }
            if (db.Queryable<UnitIdentity01>().In(ids2).Count() != 2) { throw new Exception("unit error"); }
            var list = new List<UnitIdentity01>();
            for (int i = 0; i < 1000; i++)
            {
                list.Add(new UnitIdentity01() { Name = "a" });
            }
            var ids3 = db.Insertable(list).ExecuteReturnPkList<int>();
            if (db.Queryable<UnitIdentity01>().In(ids3).Count() != 1000) { throw new Exception("unit error"); }
            db.DbMaintenance.TruncateTable<UnitIdentity01>();
        }

        public class UnitGuid123
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public Guid id { get; set; }

            public string Name { get; set; }
        }
        public class UnitLong1231
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public long id { get; set; }

            public string Name { get; set; }
        }

        public class UnitIdentity01 
        {
            [SqlSugar.SugarColumn(IsIdentity =true,IsPrimaryKey =true)]
            public int id { get; set; }

            public string Name { get; set; }
        }
    }
}
