using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class UnitGridSave
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitPerson011, UnitAddress011>();
            db.DbMaintenance.TruncateTable<UnitPerson011, UnitAddress011>();

            List<UnitAddress011> list = db.Queryable<UnitAddress011>().ToList();
            db.Tracking(list);
            list.Add(new UnitAddress011()
            {
                 Street="street jack",
                 Persons = new List<UnitPerson011>() { new UnitPerson011() {   Name="jack1"   } },
                  Persons2 = new List<UnitPerson011>() { new UnitPerson011() { Name = "jack2" } }
            });
            db.GridSave(list).IncludesAllFirstLayer().ExecuteCommand();
            var list2=db.Queryable<UnitAddress011>()
                .Includes(it => it.Persons)
                .Includes(it => it.Persons2).ToList();

            if (list2.First().Persons.Count != 1 || list2.First().Persons2.Count != 1 ||
               list2.First().Persons.First().Name != "jack1" ||
                 list2.First().Persons2.First().Name != "jack2") 
            {
                throw new Exception("unit error");
            }

            db.DeleteNav(list).IncludesAllFirstLayer().ExecuteCommand();
            if (db.Queryable<UnitAddress011>().Count() != 0|| db.Queryable<UnitPerson011>().Count() != 0) 
            {
                throw new Exception("unit error");
            }
            db.InsertNav(list).IncludesAllFirstLayer().ExecuteCommand();
            var list3 = db.Queryable<UnitAddress011>()
           .Includes(it => it.Persons)
           .Includes(it => it.Persons2).ToList();

            if (list3.First().Persons.Count != 1 || list3.First().Persons2.Count != 1 ||
             list3.First().Persons.First().Name != "jack1" ||
               list3.First().Persons2.First().Name != "jack2")
            {
                throw new Exception("unit error");
            }
        }
    }
    [SqlSugar.SugarTable("UnitPerson0x1x1")]
    public class UnitPerson011
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int AddressId { get; set; }
        public int AddressId2{ get; set; }
    }
    [SqlSugar.SugarTable("UnitAddress0x1x1")]
    public class UnitAddress011
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Street { get; set; }
        [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(UnitPerson011.AddressId))]
        public List<UnitPerson011> Persons { get; set; }
        [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(UnitPerson011.AddressId2))]
        public List<UnitPerson011> Persons2 { get; set; }
    }
}
