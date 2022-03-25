using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    [SugarTable("OrderDetail")]
    public class OrderItemInfo
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public decimal? Price { get; set; }
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? CreateTime { get; set; }
        [SugarColumn(IsIgnore = true)]
        public Order Order { get; set; }
    }
    [SugarTable("Order")]
    public class OrderInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<OrderItem> Items { get; set; }
    }
    public class ABMapping
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int AId { get; set; }
        public int BId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public A A { get; set; }
        [SugarColumn(IsIgnore = true)]
        public B B { get; set; }

    }
    public class A
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<B> BList { get; set; }
    }
    public class B
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
