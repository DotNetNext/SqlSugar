using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
namespace SqlSugar
{
    public partial class IDataReaderEntityBuilder<T>
    {
        #region fields
        private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private static readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo getBoolean = typeof(IDataRecord).GetMethod("GetBoolean", new Type[] { typeof(int) });
        private static readonly MethodInfo getByte = typeof(IDataRecord).GetMethod("GetByte", new Type[] { typeof(int) });
        private static readonly MethodInfo getDateTime = typeof(IDataRecord).GetMethod("GetDateTime", new Type[] { typeof(int) });
        private static readonly MethodInfo getDecimal = typeof(IDataRecord).GetMethod("GetDecimal", new Type[] { typeof(int) });
        private static readonly MethodInfo getDouble = typeof(IDataRecord).GetMethod("GetDouble", new Type[] { typeof(int) });
        private static readonly MethodInfo getFloat = typeof(IDataRecord).GetMethod("GetFloat", new Type[] { typeof(int) });
        private static readonly MethodInfo getGuid = typeof(IDataRecord).GetMethod("GetGuid", new Type[] { typeof(int) });
        private static readonly MethodInfo getInt16 = typeof(IDataRecord).GetMethod("GetInt16", new Type[] { typeof(int) });
        private static readonly MethodInfo getInt32 = typeof(IDataRecord).GetMethod("GetInt32", new Type[] { typeof(int) });
        private static readonly MethodInfo getInt64 = typeof(IDataRecord).GetMethod("GetInt64", new Type[] { typeof(int) });
        private static readonly MethodInfo getString = typeof(IDataRecord).GetMethod("GetString", new Type[] { typeof(int) });
        private static readonly MethodInfo getEnum = typeof(IDataRecordExtensions).GetMethod("getEnum");
        private static readonly MethodInfo getConvertFloat = typeof(IDataRecordExtensions).GetMethod("GetConvertFloat");
        private static readonly MethodInfo getConvertBoolean = typeof(IDataRecordExtensions).GetMethod("GetConvertBoolean");
        private static readonly MethodInfo getConvertByte = typeof(IDataRecordExtensions).GetMethod("GetConvertByte");
        private static readonly MethodInfo getConvertChar = typeof(IDataRecordExtensions).GetMethod("GetConvertChar");
        private static readonly MethodInfo getConvertDateTime = typeof(IDataRecordExtensions).GetMethod("GetConvertDateTime");
        private static readonly MethodInfo getConvertDecimal = typeof(IDataRecordExtensions).GetMethod("GetConvertDecimal");
        private static readonly MethodInfo getConvertDouble = typeof(IDataRecordExtensions).GetMethod("GetConvertDouble");
        private static readonly MethodInfo getConvertGuid = typeof(IDataRecordExtensions).GetMethod("GetConvertGuid");
        private static readonly MethodInfo getConvertInt16 = typeof(IDataRecordExtensions).GetMethod("GetConvertInt16");
        private static readonly MethodInfo getConvertInt32 = typeof(IDataRecordExtensions).GetMethod("GetConvertInt32");
        private static readonly MethodInfo getConvetInt64 = typeof(IDataRecordExtensions).GetMethod("GetConvetInt64");
        private static readonly MethodInfo getConvertEnum_Null = typeof(IDataRecordExtensions).GetMethod("GetConvertEnum_Null");
        private static readonly MethodInfo getOtherNull = typeof(IDataRecordExtensions).GetMethod("GetOtherNull");
        private static readonly MethodInfo getOther = typeof(IDataRecordExtensions).GetMethod("GetOther");
        private static readonly MethodInfo getEntity= typeof(IDataRecordExtensions).GetMethod("getEntity");
        private delegate T Load(IDataRecord dataRecord);
        private Load handler;
        #endregion

        #region functions
        public T Build(IDataRecord dataRecord)
        {
            return handler(dataRecord);
        }

        public static IDataReaderEntityBuilder<T> CreateBuilder(Type type, IDataRecord dataRecord, SqlSugarClient context)
        {
            IDataReaderEntityBuilder<T> dynamicBuilder = new IDataReaderEntityBuilder<T>();
            DynamicMethod method = new DynamicMethod("DynamicCreateEntity", type,
            new Type[] { typeof(IDataRecord) }, type, true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(type);
            generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            var mappingColumns = context.MappingColumns.Where(it => it.EntityName.Equals(type.Name,StringComparison.CurrentCultureIgnoreCase));
            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                string dbFieldName = dataRecord.GetName(i);
                string propName = dbFieldName;
                if (mappingColumns != null)
                {
                    var mappingInfo = mappingColumns.SingleOrDefault(it => it.DbColumnName.Equals(dbFieldName, StringComparison.CurrentCultureIgnoreCase));
                    if (mappingInfo != null)
                    {
                        propName = mappingInfo.EntityPropertyName;
                    }
                }
                PropertyInfo propertyInfo = type.GetProperty(type.GetProperties().Single(it=>it.Name.Equals(propName,StringComparison.CurrentCultureIgnoreCase)).Name);
                Label endIfLabel = generator.DefineLabel();
                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    bool isNullable = false;
                    var underType = PubMethod.GetUnderType(propertyInfo, ref isNullable);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    GeneratorCallMethod(generator, underType, isNullable, propertyInfo, dataRecord.GetDataTypeName(i), propName, context);
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                    generator.MarkLabel(endIfLabel);
                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
            return dynamicBuilder;
        }
        #endregion

        #region helpers
        private static void CheckType(List<string> errorTypes, string objType, string dbType, string field)
        {
            var isAny = errorTypes.Contains(objType);
            if (isAny)
            {
                throw new SqlSugarException(string.Format("{0} can't  convert {1} to {2}", field, dbType, objType));
            }
        }

        private static void GeneratorCallMethod(ILGenerator generator, Type type, bool isNullable, PropertyInfo pro, string dbTypeName, string fieldName, SqlSugarClient context)
        {
            var bind = context.Database.DbBind;
            List<string> guidThrow = bind.GuidThrow;
            List<string> intThrow = bind.IntThrow;
            List<string> stringThrow = bind.StringThrow;
            List<string> decimalThrow = bind.DecimalThrow;
            List<string> doubleThrow = bind.DoubleThrow;
            List<string> dateThrow = bind.DateThrow;
            List<string> shortThrow = bind.ShortThrow;
            MethodInfo method = null;
            var typeName = bind.ChangeDBTypeToCSharpType(dbTypeName);
            var objTypeName = type.Name.ToLower();
            var isEnum = type.IsEnum;
            if (isEnum)
            {
                typeName = "enum";
            }
            else if (typeName.IsIn("byte[]", "other", "object") || dbTypeName.Contains("hierarchyid"))
            {
                generator.Emit(OpCodes.Call, getValueMethod);
                generator.Emit(OpCodes.Unbox_Any, pro.PropertyType);
                return;
            }
            if (isNullable)
            {
                switch (typeName)
                {
                    case "int":
                        CheckType(intThrow, objTypeName, typeName, fieldName);
                        var isNotInt = objTypeName != "int32";
                        if (isNotInt)
                            method = getOtherNull.MakeGenericMethod(type);
                        else
                            method = getConvertInt32; break;
                    case "bool":
                        if (objTypeName != "bool" && objTypeName != "boolean")
                            method = getOtherNull.MakeGenericMethod(type);
                        else
                            method = getConvertBoolean; break;
                    case "string":
                        CheckType(stringThrow, objTypeName, typeName, fieldName);
                        method = getString; break;
                    case "dateTime":
                        CheckType(dateThrow, objTypeName, typeName, fieldName);
                        if (objTypeName != "datetime")
                            method = getOtherNull.MakeGenericMethod(type);
                        else
                            method = getConvertDateTime; break;
                    case "decimal":
                        CheckType(decimalThrow, objTypeName, typeName, fieldName);
                        var isNotDecimal = objTypeName != "decimal";
                        if (isNotDecimal)
                            method = getOtherNull.MakeGenericMethod(type);
                        else
                            method = getConvertDecimal; break;
                    case "double":
                        CheckType(doubleThrow, objTypeName, typeName, fieldName);
                        var isNotDouble = objTypeName != "double";
                        if (isNotDouble)
                            method = getOtherNull.MakeGenericMethod(type);
                        else
                            method = getConvertDouble; break;
                    case "guid":
                        CheckType(guidThrow, objTypeName, typeName, fieldName);
                        if (objTypeName != "guid")
                            method = getOtherNull.MakeGenericMethod(type);
                        else
                            method = getConvertGuid; break;
                    case "byte":
                        method = getConvertByte; break;
                    case "enum":
                        method = getConvertEnum_Null.MakeGenericMethod(type); break;
                    case "short":
                        CheckType(shortThrow, objTypeName, typeName, fieldName);
                        var isNotShort = objTypeName != "int16" && objTypeName != "short";
                        if (isNotShort)
                            method = getOtherNull.MakeGenericMethod(type);
                        else
                            method = getConvertInt16;
                        break;
                    default:
                        method = getOtherNull.MakeGenericMethod(type); break;
                }
                generator.Emit(OpCodes.Call, method);
            }
            else
            {
                switch (typeName)
                {
                    case "int":
                        CheckType(intThrow, objTypeName, typeName, fieldName);
                        var isNotInt = objTypeName != "int32";
                        if (isNotInt)
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getInt32; break;
                    case "bool":
                        if (objTypeName != "bool" && objTypeName != "boolean")
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getBoolean; break;
                    case "string":
                        CheckType(stringThrow, objTypeName, typeName, fieldName);
                        method = getString; break;
                    case "dateTime":
                        CheckType(dateThrow, objTypeName, typeName, fieldName);
                        if (objTypeName != "datetime")
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getDateTime; break;
                    case "decimal":
                        CheckType(decimalThrow, objTypeName, typeName, fieldName);
                        var isNotDecimal = objTypeName != "decimal";
                        if (isNotDecimal)
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getDecimal; break;
                    case "double":
                        CheckType(doubleThrow, objTypeName, typeName, fieldName);
                        var isNotDouble = objTypeName != "double";
                        if (isNotDouble)
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getDouble; break;
                    case "guid":
                        CheckType(guidThrow, objTypeName, typeName, fieldName);
                        if (objTypeName != "guid")
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getGuid; break;
                    case "byte":
                        method = getByte; break;
                    case "enum":
                        method = getEnum; break;
                    case "short":
                        CheckType(shortThrow, objTypeName, typeName, fieldName);
                        var isNotShort = objTypeName != "int16" && objTypeName != "short";
                        if (isNotShort)
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getInt16;
                        break;
                    default: method = getOther.MakeGenericMethod(type); break; ;
                }
                generator.Emit(OpCodes.Call, method);
                if (method == getValueMethod)
                {
                    generator.Emit(OpCodes.Unbox_Any, pro.PropertyType);
                }
            }
        }
        #endregion
    }
}
