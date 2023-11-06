using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
namespace OrmTest
{
    public class UnitNavDynamic
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            var list=db.Queryable<UnitAddress011>().Includes(x => x.Persons).ToList();
            var list2 = db.Queryable<UnitPerson011>()
                .Includes(x => x.adds)
                .Includes(x => x.adds2).ToList();
            if (list.First().Persons.Count() != 1) 
            {
                throw new Exception("unit error");
            }
            if (list2.First().adds==null)
            {
                throw new Exception("unit error");
            }
            if (list2.Last().adds2 == null)
            {
                throw new Exception("unit error");
            }
        }
        [SqlSugar.SugarTable("UnitPerson0x1x1")]
        public class UnitPerson011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int AddressId { get; set; }
            public int AddressId2 { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.Dynamic, "[{m:\"AddressId\",c:\"Id\"},{m:\"AddressId\",c:\"Id\"}]",  "Id>0")]
            public UnitAddress011 adds { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.Dynamic, "[{m:\"AddressId2\",c:\"Id\"},{m:\"AddressId2\",c:\"Id\"}]", "Id>0")]
            public UnitAddress011 adds2 { get; set; }
        }
        [SqlSugar.SugarTable("UnitAddress0x1x1")]
        public class UnitAddress011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Street { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.Dynamic, "[{m:\"Id\",c:\"AddressId\"},{m:\"Id\",c:\"AddressId\"}]")]
             public List<UnitPerson011> Persons { get; set; }
            //[SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(UnitPerson011.AddressId2))]
            //public List<UnitPerson011> Persons2 { get; set; }
        }
    } 
}
