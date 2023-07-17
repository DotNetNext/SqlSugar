using System;
using System.Linq;
using System.Runtime.InteropServices;
using SqlSugar;

namespace OrmTest
{
    public static class UnitOneToOneN2
    {
        public  static void Init()
        {
            var db = NewUnitTest.Db;

            ////建表 
        
            //db.DbMaintenance.TruncateTable<Country, Province, City>();

            //用例代码 
            var result = db.Queryable<Country>()
                .Where(Country => Country.Province.City.CityId == 1)
                .ToSqlString();//用例代码

            if (!result.Contains("[UnitCity1].[ProvinceId]=[UnitProvince0].[ProvinceId]"))
            {
                throw new Exception("unit error");
            }
            db.CodeFirst.InitTables<Country, Province, City>();
            db.DbMaintenance.TruncateTable<Country, Province, City>();
            db.Insertable(new Country()
            {
                CountryId = 1,
                ProvinceId = 1,
            }).ExecuteCommand();
            db.Insertable(new Province()
            {
                ProvinceId=1
            }).ExecuteCommand();
            db.Insertable(new City()
            {
                ProvinceId=1,
                 CityId=1
            }).ExecuteCommand();
            db.Insertable(new Country()
            {
                CountryId = 333,
                ProvinceId = 2,
            }).ExecuteCommand();
            db.Insertable(new Province()
            {
                ProvinceId = 2
            }).ExecuteCommand();
            db.Insertable(new City()
            {
                ProvinceId = 2,
                CityId = 111
            }).ExecuteCommand();
            var result2 = db.Queryable<Country, Province, City>(
                (co, pr, ci) => co.ProvinceId == pr.ProvinceId && ci.ProvinceId == ci.ProvinceId)
           .Select((co, pr, ci) => new Country()
           {
                CountryId= co.CountryId,
                 ProvinceId= co.ProvinceId,
                 Province=new Province() 
                 {
                      ProvinceId= pr.ProvinceId,
                       City=new City() 
                       {
                            ProvinceId=ci.ProvinceId,
                             CityId=ci.CityId,
                       }
                 }
           })
           .ToList();

            if (result2.First().Province.City.ProvinceId != 1||
                result2.Last().Province.City.ProvinceId != 2) 
            {
                throw new Exception("unit error");
            }

            var result3 = db.Queryable<Country, Province, City>(
               (co, pr, ci) => co.ProvinceId == pr.ProvinceId && ci.ProvinceId == ci.ProvinceId)
          .Select((co, pr, ci) => new Country()
          {
              CountryId = co.CountryId,
              ProvinceId = co.ProvinceId,
              Province = new Province()
              {
                  ProvinceId = pr.ProvinceId,
                  City = new City()
                  {
                      ProvinceId = ci.ProvinceId,
                      CityId = ci.CityId,
                  }
              }
          })
          .ToList();


            if (result3.First().Province.City.ProvinceId != 1 ||
                result3.Last().Province.City.ProvinceId != 2)
            {
                throw new Exception("unit error");
            }

        }


        //建类
        [SugarTable("UnitCountry")]
        public class Country
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int CountryId { get; set; }
            public int ProvinceId { get; set; }
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToOne, nameof(ProvinceId))]
            public Province Province { get; set; }
        }
        [SugarTable("UnitProvince")]
        public class Province
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int ProvinceId { get; set; }
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToOne, nameof(ProvinceId), nameof(UnitOneToOneN2.City.ProvinceId))]
            public City City { get; set; }
        }
        [SugarTable("UnitCity")]
        public class City
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int CityId { get; set; }
            public int ProvinceId { get; set; }
        }
    }
}