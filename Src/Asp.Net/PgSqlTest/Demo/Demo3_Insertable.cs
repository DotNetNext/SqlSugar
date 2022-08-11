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
                DbType = DbType.PostgreSQL,
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

            var insertObj = new Order() { Id = 1, Name = "order1",Price=0 };
            var updateObjs = new List<Order> {
                 new Order() { Id = 11, Name = "order11", Price=0 },
                 new Order() { Id = 12, Name = "order12" , Price=0}
            };

            var x = db.Insertable(updateObjs).RemoveDataCache().IgnoreColumns(it => it.CreateTime).UseParameter().ExecuteCommand();

            //Ignore  CreateTime
            db.Insertable(insertObj).IgnoreColumns(it => new { it.CreateTime }).ExecuteReturnIdentity();//get identity
            db.Insertable(insertObj).IgnoreColumns("CreateTime").ExecuteReturnIdentity();

            //Only  insert  Name and Price
            db.Insertable(insertObj).InsertColumns(it => new { it.Name, it.Price }).ExecuteReturnIdentity();
            db.Insertable(insertObj).InsertColumns("Name", "Price").ExecuteReturnIdentity();

            //ignore null columns
            db.Insertable(updateObjs).ExecuteCommand();//get change row count

            //Use Lock
            db.Insertable(insertObj).With(SqlWith.UpdLock).ExecuteCommand();


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

            Console.WriteLine("#### Insertable End ####");
        }
    }
}
