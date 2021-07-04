using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SingleColumnEntity<T>
    {
        public T ColumnName { get; set; }
    }
}
