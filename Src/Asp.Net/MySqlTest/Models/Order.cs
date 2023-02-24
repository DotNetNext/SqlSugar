﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    [SugarIndex("id_order_name", nameof(Order.Name), OrderByType.Asc)]
    public class Order
    {
     
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true,ColumnDescription ="主键")]
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
        [SugarColumn(IsNullable =true)]
        public int CustomId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<OrderItem> Items { get; set; }
    }
}
