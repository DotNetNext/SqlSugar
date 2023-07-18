using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public struct DiscriminatorObject
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
