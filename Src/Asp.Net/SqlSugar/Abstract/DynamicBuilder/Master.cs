using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class DynamicBuilder
    {
        List<PropertyMetadata> propertyAttr = new List<PropertyMetadata>();
        List<CustomAttributeBuilder> entityAttr = new List<CustomAttributeBuilder>();
        string entityName { get; set; }
        Type baseType = null;
        Type[] interfaces = null;
        private SqlSugarProvider context;

        public DynamicBuilder(SqlSugarProvider context)
        {
            this.context = context;
        }

        public DynamicBuilder CreateClass(string entityName, SugarTable table, Type baseType = null, Type[] interfaces = null)
        {
            this.baseType = baseType;
            this.interfaces = interfaces;
            this.entityName = entityName;
            this.entityAttr = new List<CustomAttributeBuilder>() { GetEntity(table) };
            return this;
        }
        public DynamicBuilder CreateProperty(string propertyName, Type properyType, SugarColumn table)
        {
            PropertyMetadata addItem = new PropertyMetadata();
            addItem.Name = propertyName;
            addItem.Type = properyType;
            addItem.CustomAttributes = new List<CustomAttributeBuilder>() { GetProperty(table) };
            this.propertyAttr.Add(addItem);
            return this;
        }

        public Type BuilderType()
        {
            return DynamicBuilderHelper.CreateDynamicClass(this.entityName, propertyAttr, TypeAttributes.Public, this.entityAttr, baseType, interfaces);
        }
    }
}
