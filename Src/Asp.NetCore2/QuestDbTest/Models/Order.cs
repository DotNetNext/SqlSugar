using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    [SugarTable("order_1")]
    public class Order
    {
        //不支持自增和主键 （标识主键是用来更新用的）
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
       // [SugarColumn(IsNullable = true,SqlParameterDbType =System.Data.DbType.Date)]
        public DateTime CreateTime { get; set; }
        [SugarColumn(IsNullable =true)]
        public long CustomId { get; set; }
        
        public double Value { get; set; }
        
        [SugarColumn(IsIgnore = true)]
        public List<OrderItem> Items { get; set; }
    }
}
