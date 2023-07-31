using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{

    public class Order
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SugarColumn(ColumnDescription ="名称")]
        public string Name { get; set; }
        [SugarColumn(ColumnDescription = "价格")]
        public decimal Price { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
        [SugarColumn(IsNullable =true)]
        public int CustomId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<OrderItem> Items { get; set; }
    }
}
