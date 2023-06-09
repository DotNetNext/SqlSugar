using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class QueryMethodInfo
    {
        public object QueryableObj { get; internal set; }
        public SqlSugarProvider Context { get; internal set; }
    }
}
