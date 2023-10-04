using System;
using System.Collections.Generic;
using System.Text;
using OrmTest;

namespace SqlSeverTest.UnitTest
{
    internal class UnitBulkMerge
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitaafdsTest>();
            db.DbMaintenance.TruncateTable<UnitaafdsTest>();
            var count = db.Fastest<UnitaafdsTest>()
                .BulkMerge(new List<UnitaafdsTest>() { new UnitaafdsTest() { Id=Guid.NewGuid()
                , CreateTime=DateTime.Now, Name="a"} });
            var list = db.Queryable<UnitaafdsTest>().ToList();
            if (list[0].Name != "a") { throw new Exception("unit error"); }
            list[0].Name = "j";
            var count2 = db.Fastest<UnitaafdsTest>()
             .BulkMerge(list);
            var list2 = db.Queryable<UnitaafdsTest>().ToList();
            if (list2[0].Name != "j" || count2 != 1) { throw new Exception("unit error"); }
            list2.Add(new UnitaafdsTest()
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                Name = "a"
            });
            list2.Add(new UnitaafdsTest()
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                Name = "a"
            });
            db.Fastest<UnitaafdsTest>()
                .BulkMerge(list2);
            var count3 = db.Queryable<UnitaafdsTest>()
            .Count();
            if (count3 != 3)
            {
                throw new Exception("unit error");
            }
            for (int i = 0; i < 1000; i++)
            {
                db.Fastest<UnitaafdsTest>()
               .BulkMerge(list);
            }
        }
    }
    public class UnitaafdsTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
