using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
namespace Test
{
    public static class TestHelper 
    {

        public static void InitDatabase(JsonClient jsonToSqlClient)
        {
            jsonToSqlClient.Context.DbMaintenance.CreateDatabase();//创建测试库 Create test database
            jsonToSqlClient.Context.CodeFirst.InitTables<Order, OrderItem>();//创建测试表 Create test table
        }
    }
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
        [SugarColumn(IsIgnore = true)]
        public List<OrderItem> Items { get; set; }
    }
    [SqlSugar.SugarTable("OrderDetail")]
    public class OrderItem
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public decimal? Price { get; set; }
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? CreateTime { get; set; }
    }
}
