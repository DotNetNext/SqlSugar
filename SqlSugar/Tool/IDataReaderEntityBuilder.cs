using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：DataReader实体生成
    /// ** 创始时间：2010-2-28
    /// ** 修改时间：-
    /// ** 作者：网络
    /// ** 使用说明：
    /// </summary>
    public class IDataReaderEntityBuilder<T>
    {
        private static readonly MethodInfo getValueMethod =
        typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod =
            typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private delegate T Load(IDataRecord dataRecord);

        private Load handler;

        public T Build(IDataRecord dataRecord)
        {
            return handler(dataRecord);
        }
        public static IDataReaderEntityBuilder<T> CreateBuilder(Type type,IDataRecord dataRecord)
        {
          
            {
                IDataReaderEntityBuilder<T> dynamicBuilder = new IDataReaderEntityBuilder<T>();
                DynamicMethod method = new DynamicMethod("DynamicCreateEntity", type,
                        new Type[] { typeof(IDataRecord) }, type, true);
                ILGenerator generator = method.GetILGenerator();
                LocalBuilder result = generator.DeclareLocal(type);
                generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Stloc, result);
                for (int i = 0; i < dataRecord.FieldCount; i++)
                {
                    PropertyInfo propertyInfo = type.GetProperty(dataRecord.GetName(i));
                    Label endIfLabel = generator.DefineLabel();
                    if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                        generator.Emit(OpCodes.Brtrue, endIfLabel);
                        generator.Emit(OpCodes.Ldloc, result);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, getValueMethod);
                        bool nullable = false;
                        var unType = GfetUnType(propertyInfo, ref nullable);
                        var ieEnum = unType.IsEnum;
                        if (ieEnum)
                        {
                         MethodInfo method_EnumCast = null; 
                        if (nullable) 
                        {
                            method_EnumCast = typeof(EnumMethod).GetMethod("ConvertToEnum_Nullable").MakeGenericMethod(unType); 
                         } 
                         else 
                          {
                              method_EnumCast = typeof(EnumMethod).GetMethod("ConvertToEnum").MakeGenericMethod(unType); 
                          } 
 
                          generator.Emit(OpCodes.Call, method_EnumCast); 
                        }
                        else
                        {
                            generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                        }
                        generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                        generator.MarkLabel(endIfLabel);
                    }
                }
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ret);
                dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
                return dynamicBuilder;
            }
        }

        private static Type GfetUnType(PropertyInfo propertyInfo,ref bool nullable)
        {
            Type unType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            nullable = unType != null;
            unType = unType ?? propertyInfo.PropertyType;
            return unType;
        }
    }
}
