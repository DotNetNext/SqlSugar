using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    [SqlSugar.SugarTable("OrderDetail")]
    public class OrderItem
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public long ItemId { get; set; }
        public long OrderId { get; set; }
        public decimal? Price { get; set; }
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? CreateTime { get; set; }
    }
}
