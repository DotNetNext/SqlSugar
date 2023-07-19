using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;

namespace OrmTest
{
    public class UnitUpdateOneToMany2
    {
       public  static void Init()
        {
            var db = NewUnitTest.Db;
            //建表 
            db.CodeFirst.InitTables<Country<Province>, Province>();
            db.DbMaintenance.TruncateTable<Country<Province>, Province>();

            Test1(db);
            Test2(db);
        }

        private static void Test1(SqlSugarClient db)
        {
            //插入一级导航数据
            var country = new Country<Province>
            {
                Id = 1,
                Name = "Test",
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

            var list2 = db.Queryable<Country<Province>>().Includes(x => x.Provinces).ToList();
         

            var ps = db.Queryable<Province>().ToList();
            var ps2 = db.Queryable<Province2>().ToList();
        }

        private static void Test2(SqlSugarClient db)
        {
            //插入一级导航数据
            var country = new Country<Province2>
            {
                Id = 2,
                Name = "Test",
                Provinces = new List<Province2> {
                    new Province2{
                        Id = 2,
                        CountryId = 2,
                         IsDeleted= false,
                    },
                }
            };

            db.QueryFilter.AddTableFilter<Province2>(it => it.IsDeleted == false);
            //用例代码 
            var result = db.InsertNav(country)
                .Include(it => it.Provinces)
                .ExecuteCommand();//用例代码

            var list2 = db.Queryable<Country<Province2>>().Includes(x => x.Provinces).ToList();
          

            var ps = db.Queryable<Province>().ToList();
            var ps2 = db.Queryable<Province2>().ToList();
        }
        //建类
        [SugarTable("Unitaaa1CountradsfayXX")]
        public class Country<T>
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToMany, nameof(Province.CountryId))]
            public List<T> Provinces { get; set; }
            public string Name { get; set; }
        }
        [SugarTable("Unitaaa1ProviasdfanceXX",Discrimator ="Type:1")]
        public class Province
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            public long CountryId { get; set; }
    
            public bool IsDeleted { get; set; }
        }

        [SugarTable("Unitaaa1ProviasdfanceXX", Discrimator = "Type:2")]
        public class Province2
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            public long CountryId { get; set; }

            public bool IsDeleted { get; set; }
        }
    }
}