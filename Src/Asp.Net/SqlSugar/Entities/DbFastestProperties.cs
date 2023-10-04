using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class DbFastestProperties
    {
        public bool HasOffsetTime { get; set; }
        public string[] WhereColumns { get; set; }
        public bool IsOffIdentity { get;  set; }
        public bool IsMerge { get; set; }
    }
}
