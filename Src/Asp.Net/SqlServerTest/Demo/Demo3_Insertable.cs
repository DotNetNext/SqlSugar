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

            //Use Lock
            db.Insertable(insertObj).With(SqlWith.UpdLock).ExecuteCommand();

            insertObjs = new List<Order> {
                 new Order() { Id = 11, Name = "order11", Price=1 },
                 new Order() { Id = 12, Name = "order12" , Price=20, CreateTime=DateTime.Now, CustomId=1}
            };
            db.Insertable(insertObjs).UseSqlServer().ExecuteBlueCopy();

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
                       }
                 }
            })
            .AddSubList(it => it.Items.First().OrderId).ExecuteReturnPrimaryKey();

            db.CodeFirst.InitTables<SubInsertTest, SubInsertTestItem, SubInsertTestItem1, SubInsertTestItem2>();
            db.Insertable(new SubInsertTest()
            {
                 Name="aa",
                  SubInsertTestItem1=new SubInsertTestItem1() {
                      a="nn"
                  },
                   SubInsertTestItem=new SubInsertTestItem()
                   {
                       Name ="item" ,
                       TestId=2
                   }
            })
           .AddSubList(it => it.SubInsertTestItem1)
           .AddSubList(it => it.SubInsertTestItem.TestId)
            .ExecuteReturnPrimaryKey();
     
            Console.WriteLine("#### Insertable End ####");

        }
    }
}
