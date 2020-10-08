using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public abstract partial class DbBindProvider : DbBindAccessory, IDbBind
    {
        #region Properties
        public virtual SqlSugarProvider Context { get; set; }
        public abstract List<KeyValuePair<string, CSharpDataType>> MappingTypes { get; }
        #endregion

        #region Public methods
        public virtual string GetDbTypeName(string csharpTypeName)
        {
            if (csharpTypeName == UtilConstants.ByteArrayType.Name)
                return "varbinary";
            if (csharpTypeName.ToLower() == "int32")
                csharpTypeName = "int";
            if (csharpTypeName.ToLower() == "int16")
                csharpTypeName = "short";
            if (csharpTypeName.ToLower() == "int64")
                csharpTypeName = "long";
            if (csharpTypeName.ToLower().IsIn("boolean", "bool"))
                csharpTypeName = "bool";
            var mappings = this.MappingTypes.Where(it => it.Value.ToString().Equals(csharpTypeName, StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (mappings!=null&&mappings.Count>0) 
                return mappings.First().Key;
            else
                return "varchar";
        }
        public string GetCsharpTypeName(string dbTypeName)
        {
            var mappings = this.MappingTypes.Where(it => it.Key == dbTypeName);
            return mappings.HasValue() ? mappings.First().Key : "string";
        }
        public virtual string GetConvertString(string dbTypeName)
        {
            string result = string.Empty;
            switch (dbTypeName.ToLower())
            {
                #region Int
                case "int":
                    result = "Convert.ToInt32";
                    break;
                #endregion

                #region String
                case "nchar":
                case "char":
                case "ntext":
                case "nvarchar":
                case "varchar":
                case "text":
                    result = "Convert.ToString";
                    break;
                #endregion

                #region Long
                case "bigint":
                    result = "Convert.ToInt64";
                    break;
                #endregion

                #region Bool
                case "bit":
                    result = "Convert.ToBoolean";
                    break;

                #endregion

                #region Datetime
                case "timestamp":
                case "smalldatetime":
                case "datetime":
                case "date":
                case "datetime2":
                    result = "Convert.ToDateTime";
                    break;
                #endregion

                #region Decimal
                case "smallmoney":
                case "single":
                case "numeric":
                case "money":
                case "decimal":
                    result = "Convert.ToDecimal";
                    break;
                #endregion

                #region Double
                case "float":
                    result = "Convert.ToDouble";
                    break;
                #endregion

                #region Byte[]
                case "varbinary":
                case "binary":
                case "image":
                    result = "byte[]";
                    break;
                #endregion

                #region Float
                case "real":
                    result = "Convert.ToSingle";
                    break;
                #endregion

                #region Short
                case "smallint":
                    result = "Convert.ToInt16";
                    break;
                #endregion

                #region Byte
                case "tinyint":
                    result = "Convert.ToByte";
                    break;

                #endregion

                #region Guid
                case "uniqueidentifier":
                    result = "Guid.Parse";
                    break;
                #endregion

                #region Null
                default:
                    result = null;
                    break;
                    #endregion
            }
            return result;
        }
        public virtual string GetPropertyTypeName(string dbTypeName)
        {
            dbTypeName = dbTypeName.ToLower();
            var propertyTypes = MappingTypes.Where(it => it.Key.Equals(dbTypeName, StringComparison.CurrentCultureIgnoreCase));
            if (dbTypeName == "int32")
            {
                return "int";
            }
            else if (dbTypeName == "int64")
            {
                return "long";
            }
            else if (dbTypeName == "int16")
            {
                return "short";
            }
            else if (propertyTypes == null)
            {
                return "other";
            }
            else if (dbTypeName.IsContainsIn("xml", "string", "String"))
            {
                return "string";
            }
            else if (dbTypeName.IsContainsIn("boolean", "bool"))
            {
                return "bool";
            }
            else if (propertyTypes == null || propertyTypes.Count() == 0)
            {
                return "object";
            }
            else if (propertyTypes.First().Value == CSharpDataType.byteArray)
            {
                return "byte[]";
            }
            else
            {
                return propertyTypes.First().Value.ToString();
            }
        }
        public virtual List<T> DataReaderToList<T>(Type type, IDataReader dataReader)
        {
            using (dataReader)
            {
                if (type.Name.StartsWith("KeyValuePair"))
                {
                    return GetKeyValueList<T>(type, dataReader);
                }
                else if (type.IsValueType() || type == UtilConstants.StringType || type == UtilConstants.ByteArrayType)
                {
                    return GetValueTypeList<T>(type, dataReader);
                }
                else if (type.IsArray)
                {
                    return GetArrayList<T>(type, dataReader);
                }
                else
                {
                    return GetEntityList<T>(Context, dataReader);
                }
            }
        }
        public virtual async Task<List<T>> DataReaderToListAsync<T>(Type type, IDataReader dataReader)
        {
            using (dataReader)
            {
                if (type.Name.StartsWith("KeyValuePair"))
                {
                    return await GetKeyValueListAsync<T>(type, dataReader);
                }
                else if (type.IsValueType() || type == UtilConstants.StringType || type == UtilConstants.ByteArrayType)
                {
                    return await GetValueTypeListAsync<T>(type, dataReader);
                }
                else if (type.IsArray)
                {
                    return await GetArrayListAsync<T>(type, dataReader);
                }
                else
                {
                    return await GetEntityListAsync<T>(Context, dataReader);
                }
            }
        }
        public virtual List<T> DataReaderToListNoUsing<T>(Type type, IDataReader dataReader)
        {
            if (type.Name.StartsWith("KeyValuePair"))
            {
                return GetKeyValueList<T>(type, dataReader);
            }
            else if (type.IsValueType() || type == UtilConstants.StringType || type == UtilConstants.ByteArrayType)
            {
                return GetValueTypeList<T>(type, dataReader);
            }
            else if (type.IsArray)
            {
                return GetArrayList<T>(type, dataReader);
            }
            else
            {
                return GetEntityList<T>(Context, dataReader);
            }
        }
        public virtual Task<List<T>> DataReaderToListNoUsingAsync<T>(Type type, IDataReader dataReader)
        {
            if (type.Name.StartsWith("KeyValuePair"))
            {
                return GetKeyValueListAsync<T>(type, dataReader);
            }
            else if (type.IsValueType() || type == UtilConstants.StringType || type == UtilConstants.ByteArrayType)
            {
                return GetValueTypeListAsync<T>(type, dataReader);
            }
            else if (type.IsArray)
            {
                return GetArrayListAsync<T>(type, dataReader);
            }
            else
            {
                return GetEntityListAsync<T>(Context, dataReader);
            }
        }
        #endregion

        #region Throw rule

        public virtual List<string> IntThrow
        {
            get
            {
                return new List<string>() { "datetime", "byte" };
            }
        }
        public virtual List<string> ShortThrow
        {
            get
            {
                return new List<string>() { "datetime", "guid" };
            }
        }
        public virtual List<string> DecimalThrow
        {
            get
            {
                return new List<string>() { "datetime", "byte", "guid" };
            }
        }
        public virtual List<string> DoubleThrow
        {
            get
            {
                return new List<string>() { "datetime", "byte", "guid" };
            }
        }
        public virtual List<string> DateThrow
        {
            get
            {
                return new List<string>() { "int32", "decimal", "double", "byte", "guid" };
            }
        }
        public virtual List<string> GuidThrow
        {
            get
            {
                return new List<string>() { "int32", "datetime", "decimal", "double", "byte" };
            }
        }
        public virtual List<string> StringThrow
        {
            get
            {
                return new List<string>() { "int32", "datetime", "decimal", "double", "byte", "guid" };
            }
        }
        #endregion
    }
}
