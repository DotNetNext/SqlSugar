using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class PropertyMetadata
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public List<CustomAttributeBuilder> CustomAttributes { get; set; }
    }
}
