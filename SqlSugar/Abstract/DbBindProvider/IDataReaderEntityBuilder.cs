using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
namespace SqlSugar
{
    ///<summary>
    /// ** description：IDataReader Entity Builder
    /// ** author：sunkaixuan
    /// ** date：2017/4/2
    /// ** qq:610262374
    /// </summary>
    public partial class IDataReaderEntityBuilder<T>
    {
        #region Properies
        private SqlSugarClient Context = null;
        private IDataReaderEntityBuilder<T> DynamicBuilder;
        private IDataRecord DataRecord;
        private List<string> ReaderKeys { get; set; }
        private IDataReaderEntityBuilder()
        {
        }
        #endregion

        #region Constructor

        public IDataReaderEntityBuilder(SqlSugarClient context, IDataRecord dataRecord)
        {
            this.Context = context;
            this.DataRecord = dataRecord;
            this.DynamicBuilder = new IDataReaderEntityBuilder<T>();
            this.ReaderKeys = new List<string>();
        } 
        #endregion

        #region Fields
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
        private static readonly MethodInfo getEntity = typeof(IDataRecordExtensions).GetMethod("GetEntity", new Type[] { typeof(SqlSugarClient) });
        private delegate T Load(IDataRecord dataRecord);
        private Load handler;
        #endregion

        #region Public methods
        public T Build(IDataRecord dataRecord)
        {
            return handler(dataRecord);
        }

        public IDataReaderEntityBuilder<T> CreateBuilder(Type type)
        {
            for (int i = 0; i < this.DataRecord.FieldCount; i++)
            {
                this.ReaderKeys.Add(this.DataRecord.GetName(i));
            }
            DynamicMethod method = new DynamicMethod("SqlSugarEntity", type,
            new Type[] { typeof(IDataRecord) }, type, true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(type);
            generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            var mappingColumns = Context.MappingColumns.Where(it => it.EntityName.Equals(type.Name, StringComparison.CurrentCultureIgnoreCase)).ToList();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                string fileName = propertyInfo.Name;
                if (mappingColumns != null)
                {
                    var mappInfo = mappingColumns.SingleOrDefault(it => it.EntityName.Equals(propertyInfo.Name));
                    if (mappInfo != null)
                    {
                        fileName = mappInfo.DbColumnName;
                    }
                }
                if (Context.IgnoreColumns != null && Context.IgnoreColumns.Any(it => it.PropertyName.Equals(propertyInfo.Name, StringComparison.CurrentCultureIgnoreCase)
                         && it.EntityName.Equals(type.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    continue;
                }
                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    if (propertyInfo.PropertyType.IsClass())
                    {
                        BindClass(generator, result, propertyInfo);
                    }
                    else
                    {
                        if (this.ReaderKeys.Any(it => it.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            BindField(generator, result, propertyInfo, fileName);
                        }
                    }
                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            DynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
            return DynamicBuilder;
        } 
        #endregion

        #region Private methods
        private void BindClass(ILGenerator generator, LocalBuilder result, PropertyInfo propertyInfo)
        {


        }
        private void BindField(ILGenerator generator, LocalBuilder result, PropertyInfo propertyInfo, string fileName)
        {
            int i = DataRecord.GetOrdinal(fileName);
            Label endIfLabel = generator.DefineLabel();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4, i);
            generator.Emit(OpCodes.Callvirt, isDBNullMethod);
            generator.Emit(OpCodes.Brtrue, endIfLabel);
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4, i);
            BindMethod(generator, propertyInfo, i);
            generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
            generator.MarkLabel(endIfLabel);
        }
        private void BindMethod(ILGenerator generator, PropertyInfo bindProperty, int ordinal)
        {
            var isNullableType = false;
            var bindType = PubMethod.GetUnderType(bindProperty, ref isNullableType);
            string dbTypeName = DataRecord.GetDataTypeName(ordinal);
            string propertyName = bindProperty.Name;
            var bind = Context.Ado.DbBind;
            List<string> guidThrow = bind.GuidThrow;
            List<string> intThrow = bind.IntThrow;
            List<string> stringThrow = bind.StringThrow;
            List<string> decimalThrow = bind.DecimalThrow;
            List<string> doubleThrow = bind.DoubleThrow;
            List<string> dateThrow = bind.DateThrow;
            List<string> shortThrow = bind.ShortThrow;
            MethodInfo method = null;
            var transformedPropertyName = bind.GetCSharpType(dbTypeName);
            var objTypeName = bindType.Name.ToLower();
            var isEnum = bindType.IsEnum;
            if (isEnum)
            {
                transformedPropertyName = "enum";
            }
            else if (transformedPropertyName.IsIn("byte[]", "other", "object") || dbTypeName.Contains("hierarchyid"))
            {
                generator.Emit(OpCodes.Call, getValueMethod);
                generator.Emit(OpCodes.Unbox_Any, bindProperty.PropertyType);
                return;
            }
            if (isNullableType)
            {
                switch (transformedPropertyName)
                {
                    case "int":
                        CheckType(intThrow, objTypeName, transformedPropertyName, propertyName);
                        var isNotInt = objTypeName != "int32";
                        if (isNotInt)
                            method = getOtherNull.MakeGenericMethod(bindType);
                        else
                            method = getConvertInt32; break;
                    case "bool":
                        if (objTypeName != "bool" && objTypeName != "boolean")
                            method = getOtherNull.MakeGenericMethod(bindType);
                        else
                            method = getConvertBoolean; break;
                    case "string":
                        CheckType(stringThrow, objTypeName, transformedPropertyName, propertyName);
                        method = getString; break;
                    case "DateTime":
                        CheckType(dateThrow, objTypeName, transformedPropertyName, propertyName);
                        if (objTypeName != "datetime")
                            method = getOtherNull.MakeGenericMethod(bindType);
                        else
                            method = getConvertDateTime; break;
                    case "decimal":
                        CheckType(decimalThrow, objTypeName, transformedPropertyName, propertyName);
                        var isNotDecimal = objTypeName != "decimal";
                        if (isNotDecimal)
                            method = getOtherNull.MakeGenericMethod(bindType);
                        else
                            method = getConvertDecimal; break;
                    case "double":
                        CheckType(doubleThrow, objTypeName, transformedPropertyName, propertyName);
                        var isNotDouble = objTypeName != "double";
                        if (isNotDouble)
                            method = getOtherNull.MakeGenericMethod(bindType);
                        else
                            method = getConvertDouble; break;
                    case "Guid":
                        CheckType(guidThrow, objTypeName, transformedPropertyName, propertyName);
                        if (objTypeName != "guid")
                            method = getOtherNull.MakeGenericMethod(bindType);
                        else
                            method = getConvertGuid; break;
                    case "byte":
                        method = getConvertByte; break;
                    case "enum":
                        method = getConvertEnum_Null.MakeGenericMethod(bindType); break;
                    case "short":
                        CheckType(shortThrow, objTypeName, transformedPropertyName, propertyName);
                        var isNotShort = objTypeName != "int16" && objTypeName != "short";
                        if (isNotShort)
                            method = getOtherNull.MakeGenericMethod(bindType);
                        else
                            method = getConvertInt16;
                        break;
                    default:
                        method = getOtherNull.MakeGenericMethod(bindType); break;
                }
                generator.Emit(OpCodes.Call, method);
            }
            else
            {
                switch (transformedPropertyName)
                {
                    case "int":
                        CheckType(intThrow, objTypeName, transformedPropertyName, propertyName);
                        var isNotInt = objTypeName != "int32";
                        if (isNotInt)
                            method = getOther.MakeGenericMethod(bindType);
                        else
                            method = getInt32; break;
                    case "bool":
                        if (objTypeName != "bool" && objTypeName != "boolean")
                            method = getOther.MakeGenericMethod(bindType);
                        else
                            method = getBoolean; break;
                    case "string":
                        CheckType(stringThrow, objTypeName, transformedPropertyName, propertyName);
                        method = getString; break;
                    case "DateTime":
                        CheckType(dateThrow, objTypeName, transformedPropertyName, propertyName);
                        if (objTypeName != "datetime")
                            method = getOther.MakeGenericMethod(bindType);
                        else
                            method = getDateTime; break;
                    case "decimal":
                        CheckType(decimalThrow, objTypeName, transformedPropertyName, propertyName);
                        var isNotDecimal = objTypeName != "decimal";
                        if (isNotDecimal)
                            method = getOther.MakeGenericMethod(bindType);
                        else
                            method = getDecimal; break;
                    case "double":
                        CheckType(doubleThrow, objTypeName, transformedPropertyName, propertyName);
                        var isNotDouble = objTypeName != "double";
                        if (isNotDouble)
                            method = getOther.MakeGenericMethod(bindType);
                        else
                            method = getDouble; break;
                    case "guid":
                        CheckType(guidThrow, objTypeName, transformedPropertyName, propertyName);
                        if (objTypeName != "guid")
                            method = getOther.MakeGenericMethod(bindType);
                        else
                            method = getGuid; break;
                    case "byte":
                        method = getByte; break;
                    case "enum":
                        method = getEnum; break;
                    case "short":
                        CheckType(shortThrow, objTypeName, transformedPropertyName, propertyName);
                        var isNotShort = objTypeName != "int16" && objTypeName != "short";
                        if (isNotShort)
                            method = getOther.MakeGenericMethod(bindType);
                        else
                            method = getInt16;
                        break;
                    default: method = getOther.MakeGenericMethod(bindType); break; ;
                }
                generator.Emit(OpCodes.Call, method);
                if (method == getValueMethod)
                {
                    generator.Emit(OpCodes.Unbox_Any, bindProperty.PropertyType);
                }
            }
        }
        private void CheckType(List<string> errorTypes, string objType, string transformedPropertyName, string propertyName)
        {
            var isAny = errorTypes.Contains(objType);
            if (isAny)
            {
                throw new SqlSugarException(string.Format("{0} can't  convert {1} to {2}", propertyName, transformedPropertyName, objType));
            }
        } 
        #endregion
    }
}
