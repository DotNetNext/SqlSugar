﻿using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
using System.Linq;
namespace OrmTest
{
    public class CrossDatabase03
    {
        public static void Init()
        {
            var db = new SqlSugarClient(new List<ConnectionConfig>()
            {
                new ConnectionConfig(){ConfigId="OrderDb",DbType=DbType.SqlServer,ConnectionString="server=.;uid=sa;pwd=sasa;database=SQLSUGAR4XTEST.a;Encrypt=True;TrustServerCertificate=True",IsAutoCloseConnection=true},
                new ConnectionConfig(){ConfigId="OrderItemDb",DbType=DbType.SqlServer,ConnectionString="server=.;uid=sa;pwd=sasa;database=SQLSUGAR4XTEST2.a;Encrypt=True;TrustServerCertificate=True",IsAutoCloseConnection=true  }
            });
            db.GetConnection("OrderDb").DbMaintenance.CreateDatabase();
            db.GetConnection("OrderItemDb").DbMaintenance.CreateDatabase();
            db.Aop.OnLogExecuting = (sql, p) =>Console.WriteLine( UtilMethods.GetNativeSql(sql, p));

            db.GetConnection("OrderDb").CodeFirst.InitTables<Order>();
            db.GetConnection("OrderItemDb").CodeFirst.InitTables<OrderItem>();

            db.GetConnection("OrderDb").DbMaintenance.TruncateTable<Order>();
            db.GetConnection("OrderItemDb").DbMaintenance.TruncateTable<OrderItem>();

            db.GetConnection("OrderDb").Insertable(new Order() { Id = 1, CreateTime = DateTime.Now, Name = "a", Price = 10, CustomId = 1 }).ExecuteCommand();
            db.GetConnection("OrderItemDb").Insertable(new OrderItem() { OrderId = 1, CreateTime = DateTime.Now, Price = 10 }).ExecuteCommand();


            db.Queryable<Order>().AsWithAttr()
                .LeftJoin<OrderItem>((x,y)=>x.Id==y.OrderId)
                .ToList();

            var sql1=db.Queryable<OrderItem>().Where(it=>!it.CreateTime.HasValue).ToSqlString();
            if (!sql1.Contains("NOT( [CreateTime]<>'' AND [CreateTime] IS NOT NULL )")) 
            {
                throw new Exception("error");
            }
            db.GetConnection("OrderItemDb").Ado.ExecuteCommand(sql1);
    
        }
        [SqlSugar.Tenant("OrderDb")]
        [SqlSugar.SugarTable("Order8")]
        public class Order
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(IsNullable = true)]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; }
            [Navigate(NavigateType.OneToMany, nameof(OrderItem.OrderId))]
            public List<OrderItem> Items { get; set; }
        }
        [SqlSugar.SugarTable("OrderDetail8")]
        [SqlSugar.Tenant("OrderItemDb")]
        public class OrderItem
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int ItemId { get; set; }
            public int OrderId { get; set; }
            public decimal? Price { get; set; }
            [SqlSugar.SugarColumn(IsNullable = true)]
            public DateTime? CreateTime { get; set; }
            [Navigate(NavigateType.OneToOne, nameof(OrderId))]
            public Order Order { get; set; }
        }
    }
}
