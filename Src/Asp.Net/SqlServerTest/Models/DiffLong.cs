using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class DiffLong
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
