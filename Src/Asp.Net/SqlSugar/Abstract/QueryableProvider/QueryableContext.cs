using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class MapperContext<T>
    {
        public ISqlSugarClient context { get; set; }

        public List<T> list { get; set; }
        public Dictionary<string,object> TempChildLists { get; set; }
    }
}
