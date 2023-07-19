using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    [SugarTable("ORDERTEST")]
    public class Order
    {
        [SugarColumn(IsPrimaryKey = true)]
        public decimal Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
        [SugarColumn(IsNullable =true)]
        public decimal CustomId { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string Idname { get; set; }
    }
}
