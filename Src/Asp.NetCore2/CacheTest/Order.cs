using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 namespace CacheTest

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
    }
}
