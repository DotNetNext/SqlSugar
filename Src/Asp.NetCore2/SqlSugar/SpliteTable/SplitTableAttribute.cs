using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SplitTableAttribute : Attribute
    {
        public SplitType SplitType { get; set; }
        public SplitTableAttribute(SplitType splitType) 
        {
            this.SplitType = splitType;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class SplitFieldAttribute : Attribute
    {
 
        public SplitFieldAttribute()
        {
         
        }
    }
}
