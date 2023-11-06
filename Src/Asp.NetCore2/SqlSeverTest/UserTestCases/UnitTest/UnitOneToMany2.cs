using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace OrmTest
{
    internal class UnitOneToMany2
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitPerson011, UnitAddress011, UnitCity>();
            db.DbMaintenance.TruncateTable<UnitPerson011, UnitAddress011, UnitCity>();

            var address = new UnitAddress011
            {
                Street = "123 Main Street",
                CityId = 1, Persons = new List<UnitPerson011>() {
                 new UnitPerson011
                        {
                            Name = "John Doe"

                        }
                 },
                City = new UnitCity() {
                    AddressId = 1,
                    Name = "city"
                }
            };

            db.InsertNav(address)
                .IncludeByNameString("Persons")
                .IncludeByNameString("City").ExecuteCommand();


            var list = db.Queryable<UnitAddress011>().Includes(x => x.Persons).Includes(x => x.City).ToList();


            db.DeleteNav(address).IncludeByNameString("Persons")
                .IncludeByNameString("City").ExecuteCommand();

            db.CodeFirst.InitTables<UnitTesta>();
            db.CurrentConnectionConfig.MoreSettings = new SqlSugar.ConnMoreSettings() { EnableCodeFirstUpdatePrecision = true };
            db.CodeFirst.InitTables<UNITTESTA>();
            var data= db.DbMaintenance.GetColumnInfosByTableName("UNITTESTA", false);
            if (data.First().DecimalDigits != 6) 
            {
                throw new Exception("unit error");
            }
        }
        public class UnitTesta 
        {
            [SqlSugar.SugarColumn(Length =18,DecimalDigits =4)]
            public decimal xx { get; set; }
        }
        public class UNITTESTA
        {
            [SqlSugar.SugarColumn(Length = 18, DecimalDigits = 6)]
            public decimal xx { get; set; }
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
