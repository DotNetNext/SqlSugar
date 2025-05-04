using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDbTest
{
    [SqlSugar.SugarTable("OrderDetail")]
    public class OrderItem
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true, IsIdentity =true, ColumnName = "_Id")]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal? Price { get; set; }
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? CreateTime { get; set; }
    }
}
