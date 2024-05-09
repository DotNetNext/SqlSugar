using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom015
    {

        public static void Init()
        {
            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });
            db.DbMaintenance.CreateDatabase();
        
            db.CodeFirst.InitTables<Country1111>();
            db.CodeFirst.InitTables<Province1111>();
            db.CodeFirst.InitTables<Country111Info, City1111,Street1111>();
            db.DbMaintenance.TruncateTable("Country_1111");
            db.DbMaintenance.TruncateTable("Province_1111");
            db.DbMaintenance.TruncateTable("Country111Info");
            db.DbMaintenance.TruncateTable("City_1111");
            db.DbMaintenance.TruncateTable("Street_1111");
            var c = new Country1111()
            {
                 Id=1,
                  Name="中国",InfoId=1
            };
            var ps =  new List<Province1111>(){
                            new Province1111{
                                 Id=1001,
                                 Name="江苏", CountryId=1
                                 
                            },
                           new Province1111{
                                 Id=1002,
                                 Name="上海",  CountryId=1
                                  
                            },
                           new Province1111{
                                 Id=1003,
                                 Name="北京", CountryId=1
                                  
                            }
                       };

            db.Insertable(c).ExecuteCommand();
            db.Insertable(ps).ExecuteCommand();
            db.Insertable(new City1111() { 
             Id=1, Name="南通",  pid=1001
            }).ExecuteCommand();
            db.Insertable(new City1111()
            {
                Id = 2,
                Name = "苏州",
                pid = 1001
            }).ExecuteCommand();
            //db.Insertable(ps).ExecuteCommand();
            db.Insertable(new Street1111()
            {
                Id = 1,
                Name = "南通小区",
                cid = 1
            }).ExecuteCommand();
            db.Insertable(new Country111Info {  Id=1, Name="infoa"}).ExecuteCommand();
            db.Aop.OnLogExecuted = (sq, p) =>
            {
                Console.WriteLine(sq);
            };

            var list = db.Queryable<Country1111>()
            .Includes(x => x.Info)
            .ToList();
            var list2 = db.Queryable<Country1111>()
              .Includes(x => x.Provinces.OrderByDescending(x111 => x111.Id).ToList())
              .ToList();
            var list3 = db.Queryable<Country1111>()
              .Includes(it=>it.Provinces,it=>it.city1111s,it=>it.streets)
              .Where(z=>z.Provinces.Any(y=>y.city1111s.Any(x=>x.streets.Any())))
              .ToList();
            if (list3.Count() != 1) throw new Exception("unit error");
        }

        [SugarTable("Country_1111")]
        public class Country1111
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true, ColumnName = "cid")]
            public int Id { get; set; }
            public string Name { get; set; }
            public int InfoId { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(InfoId))]
            public Country111Info Info { get; set; }

            [Navigate(NavigateType.OneToMany,nameof(Province1111.CountryId))]
            public List<Province1111> Provinces { get; set; }
        }

        public class Country111Info
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true,ColumnName = "infoId")]
            public int Id { get; set; }
            public string Name { get; set; } 
        }

        [SugarTable("Province_1111")]
        public class Province1111
        {
            [SqlSugar.SugarColumn(   ColumnName = "pid",IsPrimaryKey =true)]
            public int Id { get; set; }
            public string Name { get; set; }
            [SugarColumn(ColumnName = "coid")]
            public int CountryId { get; set; } 
            [Navigate(NavigateType.OneToMany,nameof(City1111.pid))]
            public List<City1111> city1111s { get; set; }   
        }

        [SugarTable("City_1111")]
        public class City1111
        {
            [SqlSugar.SugarColumn(ColumnName = "cid",IsPrimaryKey =true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int pid { get; set; }
            [Navigate(NavigateType.OneToMany, nameof(Street1111.cid))]
            public List<Street1111> streets { get; set; }

        }
        [SugarTable("Street_1111")]
        public class Street1111
        {
            [SqlSugar.SugarColumn(ColumnName = "sid",IsPrimaryKey =true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int cid { get; set; }

        }
    }
}
