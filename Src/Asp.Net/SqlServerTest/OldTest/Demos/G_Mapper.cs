using OrmTest.Demo;
using OrmTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class Mapper : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            db.CurrentConnectionConfig.InitKeyType = SqlSugar.InitKeyType.Attribute;

            //create tables
            db.CodeFirst.SetStringDefaultLength(100).InitTables(typeof(MyOrder),typeof(OrderItem),typeof(Person));

            //init test data
            db.DbMaintenance.TruncateTable("MyOrder");
            db.DbMaintenance.TruncateTable("OrderItem");
            db.DbMaintenance.TruncateTable("Person");
            db.Insertable(new MyOrder() { orderName = "no1", orgId = "org1", masterPersonId=1 }).ExecuteCommand();
            db.Insertable(new MyOrder() { orderName = "no2", orgId = "org2",masterPersonId=2 }).ExecuteCommand();

            db.Insertable(new OrderItem() { masterOrderId=1}).ExecuteCommand();
            db.Insertable(new OrderItem() {  masterOrderId=1}).ExecuteCommand();
            db.Insertable(new OrderItem() { masterOrderId=2 }).ExecuteCommand();
            db.Insertable(new OrderItem() { masterOrderId=2 }).ExecuteCommand();


            db.Insertable(new Person() {  orgId = "org1"}).ExecuteCommand();
            db.Insertable(new Person() { orgId ="org1" }).ExecuteCommand();


            //demo

           var list= db.Queryable<MyOrder>()
                .Mapper(it => it.masterPerson, it => it.masterPersonId)
                .Mapper(it => it.Persons, it => it.orgId)
                .Mapper(it => it.OrderItems, it => it.OrderItems.First().masterOrderId)
                .Mapper(it => it.OrderItemSignle, it => it.OrderItemSignle.masterOrderId)
                .Mapper(it => {
                    it.orderName = it.orderName + "aa";//
                })
                .ToList();

            var list2 = db.Queryable<MyOrder>()
             .Mapper(it => it.masterPerson, it => it.masterPersonId)
             .Mapper(it => it.Persons, it => it.orgId)
             .Mapper(it => it.OrderItems, it => it.OrderItems.First().masterOrderId)
             .Mapper(it => it.OrderItemSignle, it => it.OrderItemSignle.masterOrderId)
             .Mapper(it => {
                 it.orderName = it.orderName + "aa";//
                })
             .ToListAsync();
            list2.Wait();

        }
        public class MyOrder
        {
            [SqlSugar.SugarColumn(IsPrimaryKey=true,IsIdentity =true)]
            public int orderId { get; set; }
            public string orderName { get; set; }
            public string orgId { get; set; }
            public int masterPersonId { get; set; }
            [SqlSugar.SugarColumn(IsIgnore = true)]
            public List<OrderItem> OrderItems { get; set; }
            [SqlSugar.SugarColumn(IsIgnore = true)]
            public OrderItem OrderItemSignle { get; set; }
            [SqlSugar.SugarColumn(IsIgnore = true)]
            public List<Person> Persons { get; set; }
            [SqlSugar.SugarColumn(IsIgnore = true)]
            public Person masterPerson { get; set; }

        }
        public class Person
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int PersonId { get; set; }
            public string orgId { get; set; }
        }
        public class OrderItem
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int itemId { get; set; }
            public int masterOrderId { get; set; }
        }
    }
}
