using SqlSugar.MongoDbCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDbTest
{
    [SqlSugar.SugarTable("OrderDetail")]
    public class OrderItem : MongoDbBase
    { 
        public int OrderId { get; set; }
        public decimal? Price { get; set; }
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? CreateTime { get; set; }
    }
}
