using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class UnitOneToMany
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitPerson011, UnitAddress011, UnitCity>();
            db.DbMaintenance.TruncateTable<UnitPerson011, UnitAddress011, UnitCity>();

            var address = new UnitAddress011
            {
                Street = "123 Main Street",
                 CityId=1
            };
            int addressId = db.Insertable(address).ExecuteReturnIdentity();

            // 创建 UnitPerson011 对象并插入记录
            var person = new UnitPerson011
            {
                Name = "John Doe",
                AddressId = addressId
            };

            db.Insertable(new UnitCity() { AddressId = 1, Id = 1, Name = "a" }).ExecuteCommand();

            int personId = db.Insertable(person).ExecuteReturnIdentity();

            var list = db.Queryable<UnitAddress011>().Includes(x => x.Persons).Includes(x=>x.City).ToList();
            db.UpdateNav(list)
                .IncludeByNameString("Persons")
                .IncludeByNameString("City").ExecuteCommand();
        }

        [SqlSugar.SugarTable("UnitPerson01x1")]
        public class UnitPerson011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int AddressId { get; set; }
        }

        [SqlSugar.SugarTable("UnitCityaa")]
        public class UnitCity
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int AddressId { get; set; }
        }

        [SqlSugar.SugarTable("UnitAddressx011")]
        public class UnitAddress011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Street { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(UnitPerson011.AddressId))]
            public List<UnitPerson011> Persons { get; set; }
            [SqlSugar.SugarColumn(IsNullable =true)]
            public int CityId { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToOne, nameof(CityId))]
            public UnitCity City { get; set; }
        }
    }
}
