using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{

    [SqlSugar.SugarTable("ORDERINFO")]
    public class Order
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, OracleSequenceName = "seq_newsId")]
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
        [SqlSugar.SugarColumn(IsNullable =true)]
        public int CustomId { get; set; }
    }
}
