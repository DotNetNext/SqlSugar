using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{

    public class UnitNavDynamic2
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables(typeof(UnitPerson011), typeof(UnitAddress011));
            db.DbMaintenance.TruncateTable(typeof(UnitPerson011), typeof(UnitAddress011));
            var id = Guid.NewGuid();
            db.Insertable(new UnitAddress011()
            {

                Street = "a",
                Id = id
            }).ExecuteCommand();
            db.Insertable(new UnitPerson011()
            {
                AddressId = id,
                AddressId2 = id,
                Id = id,
                Name = "a",
            }).ExecuteCommand();
            var list = db.Queryable<UnitAddress011>().Includes(x => x.Persons).ToList();
            var list2 = db.Queryable<UnitPerson011>()
                .Includes(x => x.adds)
                .Includes(x => x.adds2).ToList();

            var list3 = db.Queryable<UnitAddress011>()
             .Includes(it=>it.Persons)
             .Where(it => it.Persons.Any())
             .ToList(); 
 
        }
        [SqlSugar.SugarTable("Unitsdd0x1ddx1")]
        public class UnitPerson011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; }
            public string Name { get; set; }
            public Guid AddressId { get; set; }
            public Guid AddressId2 { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.Dynamic, "[{m:\"AddressId\",c:\"Id\"},{m:\"AddressId\",c:\"Id\"}]")]
            public UnitAddress011 adds { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.Dynamic, "[{m:\"AddressId2\",c:\"Id\"},{m:\"AddressId2\",c:\"Id\"}]")]
            public UnitAddress011 adds2 { get; set; }
        }
        [SqlSugar.SugarTable("Unitadfadfssaaress0x1x1")]
        public class UnitAddress011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; }
            public string Street { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.Dynamic, "[{m:\"Id\",c:\"AddressId\"},{m:\"Id\",c:\"AddressId\"}]")]
            public List<UnitPerson011> Persons { get; set; }
            //[SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(UnitPerson011.AddressId2))]
            //public List<UnitPerson011> Persons2 { get; set; }
        }
    }


}
