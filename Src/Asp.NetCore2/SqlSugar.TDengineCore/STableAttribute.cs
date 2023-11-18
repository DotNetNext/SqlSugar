using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.TDengine
{
    public class STableAttribute:Attribute
    {
        public string Tags { get; set; }
        public string STablelName { get; set; }
    }
}
