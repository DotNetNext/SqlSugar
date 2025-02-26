using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.TDengine
{
    public class STableAttribute:Attribute
    {
        public string Tags { get; set; }
        public string Tag1 { get; set; }
        public string Tag2 { get; set; }
        public string Tag3 { get; set; }
        public string Tag4 { get; set; }
        public string STableName { get; set; }
    }
}
