using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public static class DynamicBuilderHelper
    {
        public static Type CreateDynamicClass(string className, List<PropertyMetadata> properties, TypeAttributes attributes = TypeAttributes.Public, List<CustomAttributeBuilder> classCustomAttributes = null, Type baseType = null, Type[] interfaces = null)
        {
            TypeBuilder typeBuilder = EmitTool.CreateTypeBuilder(className, attributes, baseType, interfaces);

            if (classCustomAttributes != null)
            {
                foreach (var attributeBuilder in classCustomAttributes)
                {
                    typeBuilder.SetCustomAttribute(attributeBuilder);
                }
            }

            foreach (PropertyMetadata property in properties)
            {
                var type = property.Type;
                if (type == typeof(DynamicOneselfType)) 
                {
                    type = typeBuilder;
                }
                else if (type == typeof(DynamicOneselfTypeList))
                {
                    type = typeof(List<>).MakeGenericType(typeBuilder);
                } 
                EmitTool.CreateProperty(typeBuilder, property.Name, type, property.CustomAttributes);
            }

            Type dynamicType = typeBuilder.CreateTypeInfo().AsType();

            return dynamicType;
        }
    }
}
