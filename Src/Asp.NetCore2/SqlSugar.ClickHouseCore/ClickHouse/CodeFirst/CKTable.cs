using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.ClickHouse 
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CKTable : Attribute
    {
        public string engineValue { get; set; }
        public CKTable(string engineValue) 
        {
            this.engineValue = engineValue;
        }
    }
}
