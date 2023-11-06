using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest.Models
{
    [SqlSugar.SugarTable("UnitMyOrder")]
    public class MYOrder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [SqlSugar.SugarColumn(Length = 18, DecimalDigits = 4)]
        public decimal Price { get; set; }

    }
}
