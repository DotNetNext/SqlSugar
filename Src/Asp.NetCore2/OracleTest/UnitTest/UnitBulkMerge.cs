using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class UnitBulkMerge
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitaafdsTest>();
            db.DbMaintenance.TruncateTable<UnitaafdsTest>();
            var count=db.Fastest<UnitaafdsTest>()
                .BulkMerge(new List<UnitaafdsTest>() { new UnitaafdsTest() { Id=Guid.NewGuid()
                , CreateTime=DateTime.Now, Name="a"} });
            var list=db.Queryable<UnitaafdsTest>().ToList();
            list[0].Name = "j";
            var count2 = db.Fastest<UnitaafdsTest>()
             .BulkMerge(list);
            var list2 = db.Queryable<UnitaafdsTest>().ToList();
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
        public DateTime CreateTime{ get; set; }
    }
}
