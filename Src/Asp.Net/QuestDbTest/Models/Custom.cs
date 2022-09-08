using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    [SqlSugar.SugarTable("Custom_1")]
    public class Custom
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
