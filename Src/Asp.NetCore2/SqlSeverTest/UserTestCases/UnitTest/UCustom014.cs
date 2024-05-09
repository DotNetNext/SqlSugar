using SqlSeverTest.UserTestCases;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom014
    {

        public static void Init()
        {
            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });
        
            db.CodeFirst.InitTables<Country111, Province111, City111>();
            db.DbMaintenance.TruncateTable("Country_111");
            db.DbMaintenance.TruncateTable("Province_111");
            db.DbMaintenance.TruncateTable("City_111");
            db.Insertable(new List<Country111>()
            {
                 new Country111(){
                     Id=1,
                      Name="中国",
                       Provinces=new List<Province111>(){
                            new Province111{
                                 Id=1001,
                                 Name="江苏",
                                  citys=new List<City111>(){
                                       new City111(){ Id=1001001, Name="南通" },
                                       new City111(){ Id=1001002, Name="南京" }
                                  }
                            },
                           new Province111{
                                 Id=1002,
                                 Name="上海",
                                  citys=new List<City111>(){
                                       new City111(){ Id=1002001, Name="徐汇" },
                                       new City111(){ Id=1002002, Name="普陀" }
                                  }
                            },
                           new Province111{
                                 Id=1003,
                                 Name="北京",
                                 citys=new List<City111>(){
                                       new City111(){ Id=1003001, Name="北京A" },
                                       new City111(){ Id=1003002, Name="北京B" }
                                  }
                            }
                       }
                 },
                 new Country111(){
                      Name="美国",
                      Id=2,
                      Provinces=new List<Province111>()
                      {
                          new Province111(){
                               Name="美国小A",
                               Id=20001
                          },
                         new Province111(){
                               Name="美国小b",
                               Id=20002
                          }
                      }
                  },
                 new Country111(){
                      Name="英国",
                      Id=3
                  }
            })
            .AddSubList(it => new SubInsertTree()
                          {
                              Expression = it.Provinces.First().CountryId,
                              ChildExpression = new List<SubInsertTree>() {
                      new SubInsertTree(){
                           Expression=it.Provinces.First().citys.First().ProvinceId
                      }
                 }
                          }).ExecuteCommand();
            db.Aop.OnLogExecuted = (sq, p) =>
            {
                Console.WriteLine(sq);
            };
            var list=db.Queryable<Country111>()
                .Includes(x => x.Provinces.OrderByDescending(x111=>x111.Id).ToList())
                .ToList();

            var list2 = db.Queryable<Country111>()
              .Includes(x => x.Provinces)
              .Where(x=>x.Provinces.Count()>2)
              .ToList();

            var list3 = db.Queryable<Country111>()
                .Includes(x => x.Provinces,x=>x.citys)
                .ToList();
        }

        [SugarTable("Country_111")]
        public class Country111
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "cid")]
            public int Id { get; set; }
            public string Name { get; set; }

            [Navigate(NavigateType.OneToMany,nameof(Province111.CountryId))]
            public List<Province111> Provinces { get; set; }
        }
        [SugarTable("Province_111")]
        public class Province111
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "pid")]
            public int Id { get; set; }
            public string Name { get; set; }
            [SugarColumn(ColumnName = "coid")]
            public int CountryId { get; set; }
            [Navigate(NavigateType.OneToMany, nameof(City111.ProvinceId))]
            public List<City111> citys { get; set; }
        }
        [SugarTable("City_111")]
        public class City111
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName   = "cid")]
            public int Id { get; set; }
            [SugarColumn(ColumnName="ppid")]
            public int ProvinceId { get; set; }
            public string Name { get; set; }
        }
    }
}
