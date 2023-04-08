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
        private bool IsCache = false;
        public static DynamicProperyBuilder CopyNew() 
        {
            return new DynamicProperyBuilder();
        }
        public DynamicBuilder baseBuilder;
        public DynamicProperyBuilder CreateProperty(string propertyName, Type properyType, SugarColumn column=null)
        {
            if (column == null) 
            {
                column = new SugarColumn()
                {
                     ColumnName=propertyName
                };
            }
            PropertyMetadata addItem = new PropertyMetadata();
            addItem.Name = propertyName;
            addItem.Type = properyType;
            addItem.CustomAttributes = new List<CustomAttributeBuilder>() { baseBuilder.GetProperty(column) };
            baseBuilder.propertyAttr.Add(addItem);
            return this;
        }
        public DynamicProperyBuilder WithCache(bool isCache=true)
        {
            IsCache = isCache;
            return this;
        }
        public Type BuilderType()
        {
            if (IsCache)
            {
                var key = baseBuilder.entityName + string.Join("_", baseBuilder.propertyAttr.Select(it => it.Name + it.Type.Name));
                return  new ReflectionInoCacheService().GetOrCreate(key,() =>
                {
                    var result = DynamicBuilderHelper.CreateDynamicClass(baseBuilder.entityName, baseBuilder.propertyAttr, TypeAttributes.Public, baseBuilder.entityAttr, baseBuilder.baseType, baseBuilder.interfaces);
                    return result;
                });
            }
            else
            {
                var result = DynamicBuilderHelper.CreateDynamicClass(baseBuilder.entityName, baseBuilder.propertyAttr, TypeAttributes.Public, baseBuilder.entityAttr, baseBuilder.baseType, baseBuilder.interfaces);
                return result;
            }
        }

       
    }
}
