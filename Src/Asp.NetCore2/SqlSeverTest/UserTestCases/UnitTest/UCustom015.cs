using SqlSeverTest.UserTestCases;
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
            db.CodeFirst.InitTables<Country111Info>();
            db.DbMaintenance.TruncateTable("Country_1111");
            db.DbMaintenance.TruncateTable("Province_1111");
            db.DbMaintenance.TruncateTable("Country111Info");
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
            [SqlSugar.SugarColumn(   ColumnName = "pid")]
            public int Id { get; set; }
            public string Name { get; set; }
            [SugarColumn(ColumnName = "coid")]
            public int CountryId { get; set; } 
        }
     
    }
}
