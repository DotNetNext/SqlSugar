using System;

using SqlSugar;

namespace OrmTest
{
    public static class UnitOneToOneN2
    {
        public  static void Init()
        {
            var db = NewUnitTest.Db;

            ////建表 
            //db.CodeFirst.InitTables<Country, Province, City>();
            //db.DbMaintenance.TruncateTable<Country, Province, City>();

            //用例代码 
            var result = db.Queryable<Country>()
                .Where(Country => Country.Province.City.CityId == 1)
                .ToSqlString();//用例代码

            if (!result.Contains("City1.[ProvinceId]=Province0.[ProvinceId]"))
            {
                throw new Exception("unit error");
            }
        }


        //建类
        public class Country
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int CountryId { get; set; }
            public int ProvinceId { get; set; }
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToOne, nameof(ProvinceId))]
            public Province Province { get; set; }
        }

        public class Province
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int ProvinceId { get; set; }
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToOne, nameof(ProvinceId), nameof(UnitOneToOneN2.City.ProvinceId))]
            public City City { get; set; }
        }

        public class City
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int CityId { get; set; }
            public int ProvinceId { get; set; }
        }
    }
}