using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class EmitTool
    {
        internal static ModuleBuilder CreateModuleBuilder()
        {
            AssemblyBuilder assemblyBuilder = CreateAssembly();
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
            return moduleBuilder;
        }

        internal static AssemblyBuilder CreateAssembly()
        {
            AssemblyName assemblyName = new AssemblyName($"DynamicAssembly_{Guid.NewGuid():N}");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            return assemblyBuilder;
        }

        internal static TypeBuilder CreateTypeBuilder(string className, TypeAttributes attributes, Type baseType, Type[] interfaces)
        {
            ModuleBuilder moduleBuilder = EmitTool.CreateModuleBuilder();
            TypeBuilder typeBuilder = moduleBuilder.DefineType(className, attributes, baseType, interfaces);
            return typeBuilder;
        }
        internal static PropertyBuilder CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType, IEnumerable<CustomAttributeBuilder> propertyCustomAttributes = null)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField($"_{propertyName}", propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);

            MethodBuilder getterBuilder = typeBuilder.DefineMethod($"get_{propertyName}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getterIL = getterBuilder.GetILGenerator();
            getterIL.Emit(OpCodes.Ldarg_0);
            getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getterIL.Emit(OpCodes.Ret);

            MethodBuilder setterBuilder = typeBuilder.DefineMethod($"set_{propertyName}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propertyType });
            ILGenerator setterIL = setterBuilder.GetILGenerator();
            setterIL.Emit(OpCodes.Ldarg_0);
            setterIL.Emit(OpCodes.Ldarg_1);
            setterIL.Emit(OpCodes.Stfld, fieldBuilder);
            setterIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterBuilder);
            propertyBuilder.SetSetMethod(setterBuilder);

            if (propertyCustomAttributes != null)
            {
                foreach (var attributeBuilder in propertyCustomAttributes)
                {
                    propertyBuilder.SetCustomAttribute(attributeBuilder);
                }
            }

            return propertyBuilder;
        }

    }
}
