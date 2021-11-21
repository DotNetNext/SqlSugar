using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Bulk()
        {
            Db.CodeFirst.InitTables<UnitIdentity1>();
            Db.DbMaintenance.TruncateTable<UnitIdentity1>();
           var data = new UnitIdentity1()
            {
                Name = "jack"
            };
            Db.Fastest<UnitIdentity1>().BulkCopy(new List<UnitIdentity1>() {
              data
            });
            var list=Db.Queryable<UnitIdentity1>().ToList();
            if (list.Count != 1 || data.Name != list.First().Name) 
            {
                throw new Exception("unit Bulk");
            }
            data.Name = "2";
            Db.Fastest<UnitIdentity1>().BulkCopy(new List<UnitIdentity1>() {
              data,
              data
            });
            list = Db.Queryable<UnitIdentity1>().ToList();
            if (list.Count != 3 || !list.Any(it=>it.Name=="2"))
            {
                throw new Exception("unit Bulk");
            }
            Db.Fastest<UnitIdentity1>().BulkUpdate(new List<UnitIdentity1>() {
               new UnitIdentity1(){
                Id=1,
                 Name="222"
               },
                 new UnitIdentity1(){
                Id=2,
                 Name="111"
               }
            });
            list = Db.Queryable<UnitIdentity1>().ToList();
            if (list.First(it=>it.Id==1).Name!="222")
            {
                throw new Exception("unit Bulk");
            }
            if (list.First(it => it.Id == 2).Name != "111")
            {
                throw new Exception("unit Bulk");
            }
            if (list.First(it => it.Id == 3).Name != "2")
            {
                throw new Exception("unit Bulk");
            }

            Db.CodeFirst.InitTables<UnitIdentity111>();
            Db.DbMaintenance.TruncateTable<UnitIdentity111>();
            var count=Db.Fastest<UnitIdentity111111111>().AS("UnitIdentity111").BulkCopy(new List<UnitIdentity111111111> { 
              new UnitIdentity111111111(){ Id=1, Name="jack" }
            });
            if (count == 0) 
            {
                throw new Exception("unit Bulk");
            }
            count = Db.Fastest<UnitIdentity111111111>().AS("UnitIdentity111").BulkUpdate(new List<UnitIdentity111111111> {
              new UnitIdentity111111111(){ Id=1, Name="jack" }
            });
            if (count == 0)
            {
                throw new Exception("unit Bulk");
            }
            Db.CodeFirst.InitTables<UnitTable001>();
            Db.Fastest<UnitTable001>().BulkUpdate(new List<UnitTable001> {
              new UnitTable001(){   Id=1, table="a" }
            });
        }
    }

    public class UnitTable001
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public int Id { get; set; }
        public string table { get; set; }
    }
    public class UnitIdentity111
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class UnitIdentity111111111
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UnitIdentity1 
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
