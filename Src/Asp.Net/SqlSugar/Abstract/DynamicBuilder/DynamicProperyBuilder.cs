using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SqlSugar 
{

    public class DynamicProperyBuilder
    {
        public static DynamicProperyBuilder CopyNew() 
        {
            return new DynamicProperyBuilder();
        }
        public DynamicBuilder baseBuilder;
        public DynamicProperyBuilder CreateProperty(string propertyName, Type properyType, SugarColumn table)
        {
            PropertyMetadata addItem = new PropertyMetadata();
            addItem.Name = propertyName;
            addItem.Type = properyType;
            addItem.CustomAttributes = new List<CustomAttributeBuilder>() { baseBuilder.GetProperty(table) };
            baseBuilder.propertyAttr.Add(addItem);
            return this;
        }

        public Type BuilderType()
        {
            return DynamicBuilderHelper.CreateDynamicClass(baseBuilder.entityName, baseBuilder.propertyAttr, TypeAttributes.Public, baseBuilder.entityAttr, baseBuilder.baseType, baseBuilder.interfaces);
        }
    }
}
