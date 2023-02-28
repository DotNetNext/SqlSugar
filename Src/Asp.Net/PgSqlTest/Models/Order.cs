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

        public string Name { get; set; }
        public decimal Price { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
        [SugarColumn(IsNullable =true)]
        public int CustomId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<OrderItem> Items { get; set; }

        [SugarColumn(ColumnDataType = "text []", IsArray = true, IsNullable = true)]
        public string[] Pics { get; set; }

        [SugarColumn(ColumnDataType = "integer []", IsArray = true, IsNullable = true)]
        public int[] Hits { get; set; }
    }
}
