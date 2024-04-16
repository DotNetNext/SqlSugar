using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
using System.Linq;
using System.Net.Http.Headers;
namespace OrmTest
{
    public class CrossDatabase02
    {
        public static void Init()
        {
            var db = new SqlSugarClient(new List<ConnectionConfig>()
            {
                new ConnectionConfig(){DbLinkName= "SQLSUGAR4XTEST.DBO",ConfigId="OrderDb",DbType=DbType.SqlServer,ConnectionString="server=.;uid=sa;pwd=sasa;database=SQLSUGAR4XTEST",IsAutoCloseConnection=true},
                new ConnectionConfig(){ConfigId="OrderItemDb",DbType=DbType.SqlServer,ConnectionString="server=.;uid=sa;pwd=sasa;database=SQLSUGAR4XTEST2",IsAutoCloseConnection=true  }
            }); 

            db.GetConnection("OrderDb").Aop.OnLogExecuting = (sql, p) =>Console.WriteLine( UtilMethods.GetNativeSql(sql, p));
            db.GetConnection("OrderItemDb").Aop.OnLogExecuting = (sql, p) => Console.WriteLine(UtilMethods.GetNativeSql(sql, p));

            db.GetConnection("OrderDb").CodeFirst.InitTables<Order>();
            db.GetConnection("OrderItemDb").CodeFirst.InitTables<OrderItem>();

            db.GetConnection("OrderDb").DbMaintenance.TruncateTable<Order>();
            db.GetConnection("OrderItemDb").DbMaintenance.TruncateTable<OrderItem>();

            db.GetConnection("OrderDb").Insertable(new Order() { Id = 1, CreateTime = DateTime.Now, Name = "a", Price = 10, CustomId = 1 }).ExecuteCommand();
            db.GetConnection("OrderItemDb").Insertable(new OrderItem() { OrderId = 1, CreateTime = DateTime.Now, Price = 10 }).ExecuteCommand();


            var list= db.QueryableWithAttr<OrderItem>()
                .Includes(it=>it.Order)
                .Where(it=>it.Order.Id==1)
                .ToList();

            db.CodeFirst.InitTables<UnitPkadfa1>(); 
            db.CodeFirst.InitTables<UnitPkadfa2>();
            db.CodeFirst.InitTables<UnitPkadfa2>();

            var userInfo = db.Queryable<UnitPkadfa2>().ToDataTable();

            var x = db.Fastest<System.Data.DataTable>().PageSize(10)
                .AS("UnitPkadfa2")
                .BulkUpdate(userInfo, new string[] { "Id" });


            db.DbMaintenance.DropTable<UnitPkadfa2>();
            db.CodeFirst.InitTables<UnitIndexaa1>();
            db.Aop.DataExecuting = (x, y) =>
            {

            };
            db.DbMaintenance.TruncateTable<UnitIndexaa1>();
            db.Insertable(new UnitIndexaa1() { id = 1 }).ExecuteCommand();
            db.Updateable(new UnitIndexaa1() { id = 1 }).ExecuteCommand();
        }

        public class UnitIndexaa1 
        {
             string[] ids = new string[] { };
            [SugarColumn(IsIgnore =true)]
            public object this[int i] 
            {
                get {
                 return ids[i];
                } 
            }
            [SugarColumn(IsPrimaryKey =true)]
            public int id { get; set; }
            [SugarColumn(IsNullable =true)]

            public string Name { get; set; }
        }

        [SugarTable("UnitPkadfa")]
        public class UnitPkadfa1
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
        [SugarTable("UnitPkadfa")]
        public class UnitPkadfa2
        {
            [SugarColumn(IsPrimaryKey =true)]
            public string Id { get; set; }
            [SugarColumn(IsPrimaryKey = true)]
            public string Name { get; set; }
        }

        [SqlSugar.Tenant("OrderDb")]
        [SqlSugar.SugarTable("Order811")]
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
        [SqlSugar.SugarTable("OrderDetail111")]
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
