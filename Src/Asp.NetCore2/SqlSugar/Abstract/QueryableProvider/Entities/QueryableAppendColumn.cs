using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class QueryableAppendColumn
    {
        public string AsName { get; set; }
        public int Index { get; set; }
        public object Value { get; set; }
        public string Name { get;  set; }
    }
}
