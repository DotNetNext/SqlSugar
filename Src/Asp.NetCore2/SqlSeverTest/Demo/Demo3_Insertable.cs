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
                 new Order() { Id = 11, Name = "order11", Price=0 },
                 new Order() { Id = 12, Name = "order12" , Price=0}
            };

            //Ignore  CreateTime
            db.Insertable(insertObj).IgnoreColumns(it => new { it.CreateTime }).ExecuteReturnIdentity();//get identity
            db.Insertable(insertObj).IgnoreColumns("CreateTime").ExecuteReturnIdentity();

            //Only  insert  Name and Price
            db.Insertable(insertObj).InsertColumns(it => new { it.Name, it.Price }).ExecuteReturnIdentity();
            db.Insertable(insertObj).InsertColumns("Name", "Price").ExecuteReturnIdentity();

            //ignore null columns
            db.Insertable(insertObjs).ExecuteCommand();//get change row count
            db.Insertable(insertObjs).AS("ORDER").ExecuteCommand();//get change row count
            db.Insertable(insertObjs.First()).AS("ORDER").ExecuteCommand();//get change row count
            //Use Lock
            db.Insertable(insertObj).With(SqlWith.UpdLock).ExecuteCommand();

            insertObjs = new List<Order> {
                 new Order() { Id = 11, Name = "order11", Price=1 },
                 new Order() { Id = 12, Name = "order12" , Price=20, CreateTime=DateTime.Now, CustomId=1}
            };
            db.Insertable(insertObjs).UseSqlServer().ExecuteBulkCopy();

            db.CodeFirst.InitTables<RootTable0, TwoItem, TwoItem2, TwoItem3>();
            db.CodeFirst.InitTables<ThreeItem2>();
            db.DbMaintenance.TruncateTable("RootTable0");
            db.DbMaintenance.TruncateTable("TwoItem");
            db.DbMaintenance.TruncateTable("TwoItem2");
            db.DbMaintenance.TruncateTable("TwoItem3");
            db.DbMaintenance.TruncateTable("ThreeItem2");
            Console.WriteLine("SubInsert Start");

            db.Insertable(new Order()
            {
                Name = "订单 1",
                CustomId = 1,
                Price = 100,
                CreateTime = DateTime.Now,
                Id = 0,
                Items = new List<OrderItem>() {
                      new OrderItem(){
                           CreateTime=DateTime.Now,
                           OrderId=0,
                            Price=1,
                             ItemId=1
                       },
                      new OrderItem(){
                           CreateTime=DateTime.Now,
                           OrderId=0,
                            Price=2,
                             ItemId=2
                       }
                 }
            })
            .AddSubList(it => it.Items.First().OrderId).ExecuteCommand();



            db.Insertable(new List<RootTable0>() {
                new RootTable0()
            {
                 Name="aa",
                   TwoItem2=new TwoItem2() {
                      Id="1",
                       ThreeItem2=new List<ThreeItem2>(){
                            new ThreeItem2(){ Name="a", TwoItem2Id="1" },
                            new ThreeItem2(){ Id=2, Name="a2", TwoItem2Id="2" }
                        }
                   },
                   TwoItem=new TwoItem()
                   {
                       Name ="itema" ,
                       RootId=2
                   },
                   TwoItem3=new List<TwoItem3>(){
                       new TwoItem3(){  Id=0, Name="a",Desc="" },

                   }
            },
                new RootTable0()
            {
                 Name="bb",
                   TwoItem2=new TwoItem2() {
                      Id="2"
                  },
                    TwoItem=new TwoItem()
                   {
                       Name ="itemb" ,
                       RootId=2,

                   },
                    TwoItem3=new List<TwoItem3>(){
                       new TwoItem3(){ Id=1, Name="b",Desc="" },
                              new TwoItem3(){ Id=2, Name="b1",Desc="1" },
                   }
            }
            })
           .AddSubList(it => it.TwoItem.RootId)
           .AddSubList(it => new SubInsertTree()
           {
               Expression = it.TwoItem2.RootId,
               ChildExpression = new List<SubInsertTree>() {
                       new SubInsertTree(){
                            Expression=it.TwoItem2.ThreeItem2.First().TwoItem2Id
                       }
                  }
           })
           .AddSubList(it => it.TwoItem3)
           .ExecuteCommand();

            SubNoIdentity(db);
            SubIdentity(db);
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
            db.CodeFirst.InitTables<Country1, Province1, City1,Area1>();
            db.DbMaintenance.TruncateTable("Country1");
            db.DbMaintenance.TruncateTable("Province1");
            db.DbMaintenance.TruncateTable("City1");
            db.DbMaintenance.TruncateTable("Area1");
            var list = new List<Country1>()
            {
               new Country1(){
                     Id=1,
                       Name="中国",
                     Provinces=new List<Province1>(){
                            new Province1{
                                 Id=1001,
                                 Name="江苏",
                                 citys=new List<City1>(){
                                      new City1(){
                                      ProvinceId=1001,
                                      Name ="昆山",
                                      area=new List<Area1>() {
                                            new Area1(){
                                                   CityId=000,
                                                   Name="江苏小县城"

                                            }
                                      }

                                      }
                                  },

                            },
                           new Province1{
                                 Id=1002,
                                 Name="上海",
                                  citys=new List<City1>(){
                                      new City1(){
                                     ProvinceId=1002,
                                      Name ="陆家嘴"
                                      }


                                  }
                            },
                           new Province1{
                                 Id=1003,
                                 Name="北京",
                                  citys=new List<City1>(){
                                      new City1(){
                                       ProvinceId=1003 ,
                                      Name ="中官村"
                                      }


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
            };
            //开始插入
            db.Insertable(list)
            .AddSubList(it => new SubInsertTree()
            {
                Expression = it.Provinces.First().CountryId,//CountryId自动填充
                ChildExpression = new List<SubInsertTree>() {
                    new SubInsertTree(){
                      Expression=it.Provinces.First()
                                      .citys.First()
                                      .ProvinceId,//ProvinceId自动填充,
                     ChildExpression=new List<SubInsertTree>(){
                             new SubInsertTree(){
                               Expression = it.Provinces.First().citys.First().area.First().CityId
                             }

                         }
                    }
                 }
            })
            //如果有多个子结果这里还能在.AddSubList
            .ExecuteCommand();

            var list2=db.Queryable<Country1>().Includes(x => x.Provinces, x => x.citys, x => x.area).ToList();
        }
    }
}
