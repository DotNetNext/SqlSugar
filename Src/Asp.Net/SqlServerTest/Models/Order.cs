using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{

    public class Order
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        [SqlSugar.SugarColumn(IsNullable =true)]
        public DateTime CreateTime { get; set; }
    }
}
