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
        public String String { get; set; }
        public decimal Decimal { get; set; }
        public long Long { get; set; }
        public int Int { get; set; }
        public Byte[] ByteArray { get; set; }
    }
}
