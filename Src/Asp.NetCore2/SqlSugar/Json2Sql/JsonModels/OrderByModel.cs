using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    public class OrderByModel 
    {
        public string FieldName { get; set; }
        public OrderByType OrderByType { get; set; }
    }
}
