using System;
using System.Collections.Generic;

using SqlSugar;

namespace OrmTest
{
    public class UnitInsertNav3
    {
       public  static void Init()
        {
            var db = NewUnitTest.Db; 
            //建表 
            db.CodeFirst.InitTables<Country, Province, City>();
            db.DbMaintenance.TruncateTable<Country, Province, City>();

            //插入一级导航数据
            var country = new Country
            {
                Id = 1,
                Provinces = new List<Province> {
                    new Province{
                        Id = 1,
                        CountryId = 1
                    },
                }
            };
            db.InsertNav(country)
                .Include(it => it.Provinces)
                .ExecuteCommand();

            //构造二级导航数据
            country.Provinces[0].Cities = new List<City>
            {
                new City
                {
                    Id = 1,
                    ProvinceId = 1
                }
            };

            //用例代码 
            var result = db.InsertNav(country)
                .Include(it => it.Provinces,new InsertNavOptions() {  OneToManyIfExistsNoInsert=true})
                .ThenInclude(it => it.Cities, new InsertNavOptions() { OneToManyIfExistsNoInsert = true })
                .ExecuteCommand();//用例代码
            if (!db.Queryable<City>().Where(it => it.Id == 1).Any())
                throw new Exception("未插入二级导航数据");
        }


        //建类
        [SugarTable("Unitaaa1Country")]
        public class Country
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToMany, nameof(Province.CountryId))]
            public List<Province> Provinces { get; set; }
        }
        [SugarTable("Unitaaa1Province")]
        public class Province
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            public long CountryId { get; set; }
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToMany, nameof(City.ProvinceId))]
            public List<City> Cities { get; set; }
        }
        [SugarTable("Unitaaa1City")]
        public class City
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            public long ProvinceId { get; set; }
        }
    }
}