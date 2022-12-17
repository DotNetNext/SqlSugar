using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Demo3_Insertable
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Insertable Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                }
            });

            var insertObj = new Order() { Id = 1, Name = "order1", Price = 0 };
            var insertObjs = new List<Order> {
                 new Order() { Id = 11, Name = "XX", Price=0 },
                 new Order() { Id = 12, Name = "XX2" , Price=0}
            };

            var x=db.Insertable(insertObjs).RemoveDataCache().IgnoreColumns(it=>it.CreateTime).UseParameter().ExecuteCommand();

            //Ignore  CreateTime
            db.Insertable(insertObj).IgnoreColumns(it => new { it.CreateTime }).ExecuteReturnIdentity();//get identity
            db.Insertable(insertObj).IgnoreColumns("CreateTime").ExecuteReturnIdentity();

            //Only  insert  Name and Price
            db.Insertable(insertObj).InsertColumns(it => new { it.Name, it.Price }).ExecuteReturnIdentity();
            db.Insertable(insertObj).InsertColumns("Name", "Price").ExecuteReturnIdentity();

            //ignore null columns
            db.Insertable(insertObjs).ExecuteCommand();//get change row count

            //Use Lock
            db.Insertable(insertObj).With(SqlWith.UpdLock).ExecuteCommand();

            insertObjs = new List<Order> {
                 new Order() { Id = 11, Name = "order11", Price=1 },
                 new Order() { Id = 12, Name = "order12" , Price=20, CreateTime=DateTime.Now, CustomId=1}
            };
            db.Insertable(insertObjs).UseSqlServer().ExecuteBulkCopy();
            var dt = db.Queryable<Order>().Take(5).ToDataTable();
            dt.TableName = "Order";
            db.Insertable(dt).UseSqlServer().ExecuteBulkCopy();
            db.CodeFirst.InitTables<RootTable0, TwoItem, TwoItem2, TwoItem3>();
            db.CodeFirst.InitTables<ThreeItem2>();
            db.DbMaintenance.TruncateTable("RootTable0");
            db.DbMaintenance.TruncateTable("TwoItem");
            db.DbMaintenance.TruncateTable("TwoItem2");
            db.DbMaintenance.TruncateTable("TwoItem3");
            db.DbMaintenance.TruncateTable("ThreeItem2");
            Console.WriteLine("SubInsert Start");

  
            var dict = new Dictionary<string, object>();
            dict.Add("name", "1");
            dict.Add("CreateTime", DateTime.Now);
            dict.Add("Price", 1);
            db.Insertable(dict).AS("[Order]").ExecuteCommand();

            db.Insertable(new List<Order>()).UseParameter().ExecuteCommand();

            db.Fastest<Order>().BulkCopy(insertObjs);


            var dataTable= db.Queryable<Order>().Select("id,name,Price").Take(2).ToDataTable();
            int result= db.Fastest<System.Data.DataTable>().AS("order").BulkCopy("order", dataTable);
            int result2 = db.Fastest<System.Data.DataTable>().AS("order").BulkCopy( dataTable);
            Console.WriteLine("#### Insertable End ####");

        }

        private static void SubNoIdentity(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<Country, Province, City>();
            db.DbMaintenance.TruncateTable("Country");
            db.DbMaintenance.TruncateTable("Province");
            db.DbMaintenance.TruncateTable("City");
            db.Insertable(new List<Country>()
            {
                 new Country(){
                     Id=1,
                      Name="中国",
                       Provinces=new List<Province>(){
                            new Province{
                                 Id=1001,
                                 Name="江苏",
                                  citys=new List<City>(){
                                       new City(){ Id=1001001, Name="南通" },
                                       new City(){ Id=1001002, Name="南京" }
                                  }
                            },
                           new Province{
                                 Id=1002,
                                 Name="上海",
                                  citys=new List<City>(){
                                       new City(){ Id=1002001, Name="徐汇" },
                                       new City(){ Id=1002002, Name="普陀" }
                                  }
                            },
                           new Province{
                                 Id=1003,
                                 Name="北京",
                                 citys=new List<City>(){
                                       new City(){ Id=1003001, Name="北京A" },
                                       new City(){ Id=1003002, Name="北京B" }
                                  }
                            }
                       }
                 },
                 new Country(){
                      Name="美国",
                      Id=2,
                      Provinces=new List<Province>()
                      {
                          new Province(){
                               Name="美国小A",
                               Id=20001
                          },
                         new Province(){
                               Name="美国小b",
                               Id=20002
                          }
                      }
                  },
                 new Country(){
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
            })
            .ExecuteCommand();

            var list = db.Queryable<Country>()
                                 .Mapper(it => it.Provinces, it => it.Provinces.First().CountryId)
                                 .Mapper(it =>
                                 {
                                     foreach (var item in it.Provinces)
                                     {
                                         item.citys = db.Queryable<City>().Where(y => y.ProvinceId == item.Id).ToList();
                                     }
                                 })
                                 .ToList();
        }
        private static void SubIdentity(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<Country1, Province1, City1>();
            db.DbMaintenance.TruncateTable("Country1");
            db.DbMaintenance.TruncateTable("Province1");
            db.DbMaintenance.TruncateTable("City1");
            db.Insertable(new List<Country1>()
            {
                 new Country1(){
                     Id=1,
                      Name="中国",
                       Provinces=new List<Province1>(){
                            new Province1{
                                 Id=1001,
                                 Name="江苏",
                                  citys=new List<City1>(){
                                       new City1(){ Id=1001001, Name="南通" },
                                       new City1(){ Id=1001002, Name="南京" }
                                  }
                            },
                           new Province1{
                                 Id=1002,
                                 Name="上海",
                                  citys=new List<City1>(){
                                       new City1(){ Id=1002001, Name="徐汇" },
                                       new City1(){ Id=1002002, Name="普陀" }
                                  }
                            },
                           new Province1{
                                 Id=1003,
                                 Name="北京",
                                 citys=new List<City1>(){
                                       new City1(){ Id=1003001, Name="北京A" },
                                       new City1(){ Id=1003002, Name="北京B" }
                                  }
                            }
                       }
                 },
                 new Country1(){
                      Name="美国",
                      Id=2,
                      Provinces=new List<Province1>()
                      {
                          new Province1(){
                               Name="美国小A",
                               Id=20001
                          },
                         new Province1(){
                               Name="美国小b",
                               Id=20002
                          }
                      }
                  },
                 new Country1(){
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
            })
            .ExecuteCommand();

            var list = db.Queryable<Country1>()
                                 .Mapper(it => it.Provinces, it => it.Provinces.First().CountryId)
                                 .Mapper(it =>
                                 {
                                     foreach (var item in it.Provinces)
                                     {
                                         item.citys = db.Queryable<City1>().Where(y => y.ProvinceId == item.Id).ToList();
                                     }
                                 })
                                 .ToList();
        }
    }
}
