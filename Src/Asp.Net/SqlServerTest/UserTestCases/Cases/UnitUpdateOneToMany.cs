using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;

namespace OrmTest
{
    public class UnitUpdateOneToMany
    {
       public  static void Init()
        {
            var db = NewUnitTest.Db; 
            //建表 
            db.CodeFirst.InitTables<Country, Province>();
            db.DbMaintenance.TruncateTable<Country, Province>();

            //插入一级导航数据
            var country = new Country
            {
                Id = 1, 
                Name= "Test",
                Provinces = new List<Province> {
                    new Province{
                        Id = 1,
                        CountryId = 1,
                         IsDeleted= false,
                    },
                }
            };
         
            db.QueryFilter.AddTableFilter<Province>(it => it.IsDeleted == false);
            //用例代码 
            var result = db.InsertNav(country)
                .Include(it => it.Provinces) 
                .ExecuteCommand();//用例代码

            var list2 = db.Queryable<Country>().Includes(x => x.Provinces).ToList();
            country = new Country
            {
                Id = 1,
                Name = "中国",
                Provinces = new List<Province> {
                    new Province{
                        Id = 2,
                        CountryId = 1,
                         IsDeleted= false,
                    },
                }
            };
            //用例代码 
            db.UpdateNav(country)
                .Include(it => it.Provinces,new UpdateNavOptions() { OneToManyEnableLogicDelete=true }) 
                .ExecuteCommand();//
           
            var list3=db.Queryable<Country>().Includes(x => x.Provinces).ToList();
            db.QueryFilter.Clear<Province>();
            var list4 = db.Queryable<Country>().Includes(x => x.Provinces).ToList();
            if (list3.First().Provinces.Count != 1 || list4.First().Provinces.Count != 2) 
            {
                throw new Exception("unit error");
            }

        }


        //建类
        [SugarTable("Unitaaa1Countradsfay")]
        public class Country
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToMany, nameof(Province.CountryId))]
            public List<Province> Provinces { get; set; }
            public string Name { get; set; }
        }
        [SugarTable("Unitaaa1Proviasdfance")]
        public class Province
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            public long CountryId { get; set; }
    
            public bool IsDeleted { get; set; }
        }
   
    }
}