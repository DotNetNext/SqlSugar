using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrmTest;
using SqlSugar;

namespace OrmTest
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
            for (int i = 0; i < 1; i++)
            {
                db.Fastest<UnitaafdsTest>()
               .BulkMerge(list);
            }
            db.CodeFirst.InitTables<Unitfadfayy>();
            db.DbMaintenance.TruncateTable<Unitfadfayy>();
            List<Unitfadfayy> addItems = new List<Unitfadfayy>();
            addItems.Add(new Unitfadfayy() { CreateTime = DateTime.Now, Name = "a" });
            db.Fastest<Unitfadfayy>().BulkMerge(addItems);
            var list3=db.Queryable<Unitfadfayy>().ToList();
            list3.First().Name = "j";
            db.Fastest<Unitfadfayy>().BulkMerge(list3);
            var list4 = db.Queryable<Unitfadfayy>().ToList();
            db.CodeFirst.InitTables<TableA>();
            db.Insertable(new TableA() { key1 = Guid.NewGuid() + "" })
                .ExecuteCommand();
            db.Insertable(new TableA() { key1 = Guid.NewGuid() + "" })
                .ExecuteCommand();
            var datas = db.Queryable<TableA>().ToList();
            db.Fastest<TableA>().PageSize(10000).BulkMerge(datas);
        }
    }
    [SugarTable("Unitadfadf")]
    public class TableA
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string key1 { get; set; } = "a";
        [SugarColumn(IsPrimaryKey = true)]
        public string key2 { get; set; } = "b";
        [SugarColumn(IsPrimaryKey = true)]
        public string key3 { get; set; } = "c";
        public string str1 { get; set; } = "c";
        public string str2 { get; set; } = "c";
        public DateTime? date1 { get; set; } = DateTime.Now;
        public DateTime? date2 { get; set; } = DateTime.Now;
        public decimal? num1 { get; set; } = 0;
        public decimal? num2 { get; set; } = 0;
    }
    public class Unitfadfayy 
    {

        [SqlSugar.SugarColumn(IsPrimaryKey = true,IsIdentity =true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }
    public class UnitaafdsTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
