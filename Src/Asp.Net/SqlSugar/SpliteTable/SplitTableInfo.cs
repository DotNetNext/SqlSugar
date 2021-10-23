using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SplitTableInfo
    {
        public string TableName { get; set; }
        public DateTime Date { get; set; }
    }

    internal class SplitTableSort 
    {
        public string Name { get; set; }
        public int Sort { get; set; }
    }
}
