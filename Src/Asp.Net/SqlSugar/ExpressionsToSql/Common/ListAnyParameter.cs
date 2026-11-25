using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class ListAnyParameter
    {
        public string Name { get; internal set; }
        public string Sql { get; internal set; }
        public List<EntityColumnInfo> Columns { get; internal set; }
        public Func<string,string> ConvetColumnFunc { get; internal set; }
    }
}
