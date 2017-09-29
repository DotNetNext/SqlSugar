using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class CacheKey
    {
        public string[] Tables { get; set; }
        public List<string> IdentificationList { get; set; }
    }
}
