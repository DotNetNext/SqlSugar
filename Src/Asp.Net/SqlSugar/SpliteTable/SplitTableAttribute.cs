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
        public Type CustomSplitTableService { get; set; }
        public SplitTableAttribute(SplitType splitType) 
        {
            this.SplitType = splitType;
        }
        public SplitTableAttribute(SplitType splitType,Type customSplitTableService)
        {
            this.SplitType = splitType;
            if (!customSplitTableService.GetInterfaces().Any(it => it == typeof(ISplitTableService))) 
            {
                Check.ExceptionEasy("customSplitTableService in SplitTableAttribute(SplitType splitType,Type customSplitTableService) must be inherited ISplitTableService", " SplitTableAttribute(SplitType splitType,Type customSplitTableService)  中的 customSplitTableService 必须继承 ISplitTableService");
            }
            this.CustomSplitTableService= customSplitTableService;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class SplitFieldAttribute : Attribute
    {
 
        public SplitFieldAttribute()
        {
         
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class TimeDbSplitFieldAttribute : Attribute
    {
        public DateType? DateType { get; set; }  
        public TimeDbSplitFieldAttribute(DateType type)
        {
            DateType = type;
        }
    }
}
