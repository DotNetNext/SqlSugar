using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public abstract partial class DbBindProvider : DbBindAccessory, IDbBind
    {
        #region Properties
        public virtual SqlSugarClient Context { get; set; }
        #endregion

        #region Public methods
        public abstract string GetPropertyType(string dbTypeName);
        public virtual string GetConvertString(string dbTypeName)
        {
            string reval = string.Empty;
            switch (dbTypeName.ToLower())
            {
                #region Int
                case "int":
                    reval = "Convert.ToInt32";
                    break;
                #endregion

                #region String
                case "nchar":
                case "char":
                case "ntext":
                case "nvarchar":
                case "varchar":
                case "text":
                    reval = "Convert.ToString";
                    break;
                #endregion

                #region Long
                case "bigint":
                    reval = "Convert.ToInt64";
                    break;
                #endregion

                #region Bool
                case "bit":
                    reval = "Convert.ToBoolean";
                    break;

                #endregion

                #region Datetime
                case "timestamp":
                case "smalldatetime":
                case "datetime":
                case "date":
                case "datetime2":
                    reval = "Convert.ToDateTime";
                    break;
                #endregion

                #region Decimal
                case "smallmoney":
                case "single":
                case "numeric":
                case "money":
                case "decimal":
                    reval = "Convert.ToDecimal";
                    break;
                #endregion

                #region Double
                case "float":
                    reval = "Convert.ToDouble";
                    break;
                #endregion

                #region Byte[]
                case "varbinary":
                case "binary":
                case "image":
                    reval = "byte[]";
                    break;
                #endregion

                #region Float
                case "real":
                    reval = "Convert.ToSingle";
                    break;
                #endregion

                #region Short
                case "smallint":
                    reval = "Convert.ToInt16";
                    break;
                #endregion

                #region Byte
                case "tinyint":
                    reval = "Convert.ToByte";
                    break;

                #endregion

                #region Guid
                case "uniqueidentifier":
                    reval = "Guid.Parse";
                    break;
                #endregion

                #region Null
                default:
                    reval = null;
                    break;
                    #endregion
            }
            return reval;
        }
        public virtual List<T> DataReaderToList<T>(Type type, IDataReader dataReader, string fields)
        {
            using (dataReader)
            {
                if (type.Name.Contains("KeyValuePair"))
                {
                    return GetKeyValueList<T>(type, dataReader);
                }
                else if (type.IsValueType || type == PubConst.StringType)
                {
                    return GetValueTypeList<T>(type, dataReader);
                }
                else if (type.IsArray)
                {
                    return GetArrayList<T>(type, dataReader);
                }
                else
                {
                    return GetEntityList<T>(Context, dataReader, fields);
                }
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
                return new List<string>() { "int32", "datetime", "decimal", "double", "byte", "string" };
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
