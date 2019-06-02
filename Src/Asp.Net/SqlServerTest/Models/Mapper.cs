using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Models
{
    [SugarTable("OrderDetail")]
    public class OrderItemInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public decimal Pirce { get; set; }
        public int OrderId { get; set; }
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
}
