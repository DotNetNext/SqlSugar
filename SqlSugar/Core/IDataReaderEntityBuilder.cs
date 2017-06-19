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
    /// ** 创始时间：2016-8-7
    /// ** 修改时间：-
    /// ** 作者：孙凯旋
    /// ** 使用说明：
    /// </summary>
    public class IDataReaderEntityBuilder<T>
    {
        //is db null method
        private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });


        //default method
        private static readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });


        //dr valueType method
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


        //convert method
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
        private static readonly MethodInfo getConvertToEnum_Nullable = typeof(IDataRecordExtensions).GetMethod("GetConvertEnum_Nullable");
        private static readonly MethodInfo getOtherNull = typeof(IDataRecordExtensions).GetMethod("GetOtherNull");
        private static readonly MethodInfo getOther = typeof(IDataRecordExtensions).GetMethod("GetOther");



        /// <summary>
        /// 声名委托类型
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <returns></returns>
        private delegate T Load(IDataRecord dataRecord);

        /// <summary>
        /// 声名事件，当执行CreateBuilder后，EMIT将动态创建最高性能的实体绑定对象
        /// </summary>
        private Load handler;

        /// <summary>
        /// 将dataRecord的值绑定到T
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <returns></returns>
        public T Build(IDataRecord dataRecord)
        {
            return handler(dataRecord);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataRecord"></param>
        /// <returns></returns>
        public static IDataReaderEntityBuilder<T> CreateBuilder(Type type, IDataRecord dataRecord)
        {
                IDataReaderEntityBuilder<T> dynamicBuilder = new IDataReaderEntityBuilder<T>();
                DynamicMethod method = new DynamicMethod("DynamicCreateEntity", type,
                        new Type[] { typeof(IDataRecord) }, type, true);
                ILGenerator generator = method.GetILGenerator();
                LocalBuilder result = generator.DeclareLocal(type);
                generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Stloc, result);
                string cacheKey = "SqlSugarClient.InitAttributes";
                var cm = CacheManager<List<KeyValue>>.GetInstance();
                var tFieldNames = typeof(T).GetProperties().Select(it => it.Name).ToList();
                for (int i = 0; i < dataRecord.FieldCount; i++)
                {
                    string dbFieldName = dataRecord.GetName(i);
                    if (cm.ContainsKey(cacheKey) && cm[cacheKey].Any(it => it.Value == dbFieldName))
                    {
                        var classFieldName= cm[cacheKey].Single(it => it.Value == dbFieldName).Key;
                        if (tFieldNames.Any(it => it == classFieldName))//T包含映射属性
                        {
                            dbFieldName = classFieldName;
                        }
                    }
                    //实体类的字段名与数据库大小写不一致可以映射
                    if (!tFieldNames.Any(it => it == dbFieldName) && tFieldNames.Any(it => it.ToLower() == dbFieldName.ToLower()))
                    {
                        dbFieldName = tFieldNames.Single(it => it.ToLower() == dbFieldName.ToLower());
                    }
                    PropertyInfo propertyInfo = type.GetProperty(dbFieldName);
                    Label endIfLabel = generator.DefineLabel();
                    if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                    {
                        bool isNullable = false;
                        var underType = SqlSugarTool.GetUnderType(propertyInfo, ref isNullable);

                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                        generator.Emit(OpCodes.Brtrue, endIfLabel);
                        generator.Emit(OpCodes.Ldloc, result);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        GeneratorCallMethod(generator, underType, isNullable, propertyInfo, dataRecord.GetDataTypeName(i), dbFieldName);
                        generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                        generator.MarkLabel(endIfLabel);
                    }
                }
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ret);
                dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
                return dynamicBuilder;
        }


        private static void CheckType(List<string> errorTypes, string objType, string dbType, string field)
        {
            var isAny = errorTypes.Contains(objType);
            if (isAny)
            {
                throw new SqlSugarException(string.Format("{0} can't  convert {1} to {2}", field, dbType, objType));
            }
        }


        /// <summary>
        /// 动态获取IDataRecord里面的函数
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="type"></param>
        /// <param name="isNullable"></param>
        /// <param name="pro"></param>
        /// <param name="dbTypeName"></param>
        /// <param name="fieldName"></param>
        private static void GeneratorCallMethod(ILGenerator generator, Type type, bool isNullable, PropertyInfo pro, string dbTypeName, string fieldName)
        {
            List<string> guidThrow = new List<string>() { "int32", "datetime", "decimal", "double", "byte", "string" };//数据库为GUID有错的实体类形
            List<string> intThrow = new List<string>() { "datetime", "byte" };//数据库为int有错的实体类形
            List<string> stringThrow = new List<string>() { "int32", "datetime", "decimal", "double", "byte", "guid" };//数据库为vachar有错的实体类形
            List<string> decimalThrow = new List<string>() { "datetime", "byte", "guid" };
            List<string> doubleThrow = new List<string>() { "datetime", "byte", "guid" };
            List<string> dateThrow = new List<string>() { "int32", "decimal", "double", "byte", "guid" };
            List<string> shortThrow = new List<string>() { "datetime", "guid" };
            List<string> byteThrow = new List<string>() { "datetime", "guid" };
            MethodInfo method = null;
            var typeName = SqlSugarTool.ChangeDBTypeToCSharpType(dbTypeName);
            var objTypeName = type.Name.ToLower();
            var isEnum = type.IsEnum;
            if (isEnum)
            {
                typeName = "ENUMNAME";
            }
            else if (dbTypeName.Contains("hierarchyid") || typeName == "byte[]" || objTypeName == "object" )
            {
                generator.Emit(OpCodes.Call, getValueMethod);
                generator.Emit(OpCodes.Unbox_Any, pro.PropertyType);//找不到类型才执行拆箱（类型转换）
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
                    case "long":
                        CheckType(intThrow, objTypeName, typeName, fieldName);
                        var isNotLong = objTypeName != "int64";
                        if (isNotLong)
                            method = getOtherNull.MakeGenericMethod(type);
                        else
                            method = getConvetInt64; break;
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
                    case "float":
                        CheckType(decimalThrow, objTypeName, typeName, fieldName);
                        if (objTypeName != "float" && objTypeName != "single")
                            method = getOtherNull.MakeGenericMethod(type);
                        else
                            method = getConvertFloat; break;
                    case "guid":
                        CheckType(guidThrow, objTypeName, typeName, fieldName);
                        if (objTypeName != "guid")
                            method = getOtherNull.MakeGenericMethod(type);
                        else
                            method = getConvertGuid; break;
                    case "byte":
                        CheckType(byteThrow, objTypeName, typeName, fieldName);
                        if (objTypeName != "byte")
                            method = getOtherNull.MakeGenericMethod(type);
                        else
                            method = getConvertByte; break;
                    case "ENUMNAME":
                        method = getConvertToEnum_Nullable.MakeGenericMethod(type); break;
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
                    case "long":
                        CheckType(intThrow, objTypeName, typeName, fieldName);
                        var isNotLong = objTypeName != "int64";
                        if (isNotLong)
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getInt64; break;
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
                    case "float":
                        CheckType(decimalThrow, objTypeName, typeName, fieldName);
                        if (objTypeName != "float" && objTypeName != "single")
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getFloat; break;
                    case "guid":
                        CheckType(guidThrow, objTypeName, typeName, fieldName);
                        if (objTypeName != "guid")
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getGuid; break;
                    case "byte":
                        CheckType(byteThrow, objTypeName, typeName, fieldName);
                        if (objTypeName != "byte")
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getByte; break;
                    case "ENUMNAME":
                        method = getValueMethod; break;
                    case "short":
                        CheckType(shortThrow, objTypeName, typeName, fieldName);
                        var isNotShort = objTypeName != "int16" && objTypeName != "short";
                        if (isNotShort)
                            method = getOther.MakeGenericMethod(type);
                        else
                            method = getInt16;
                        break;
                    default:
                        method = getOther.MakeGenericMethod(type);
                        break; ;

                }

                generator.Emit(OpCodes.Call, method);

                if (method == getValueMethod)
                {
                    generator.Emit(OpCodes.Unbox_Any, pro.PropertyType);//找不到类型才执行拆箱（类型转换）
                }
            }


        }
  
    }
}
