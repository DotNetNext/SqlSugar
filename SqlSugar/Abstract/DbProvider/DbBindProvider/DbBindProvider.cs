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
        public virtual SqlSugarClient Context { get; set; }
        public virtual List<string> GuidThrow
        {
            get
            {
                return new List<string>() { "int32", "datetime", "decimal", "double", "byte", "string" };
            }
        }
        public virtual List<string> IntThrow
        {
            get
            {
                return new List<string>() { "datetime", "byte" };
            }
        }
        public virtual List<string> StringThrow
        {
            get
            {
                return new List<string>() { "int32", "datetime", "decimal", "double", "byte", "guid" };
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
        public virtual List<string> ShortThrow
        {
            get
            {
                return new List<string>() { "datetime", "guid" };
            }
        }
        public virtual List<T> DataReaderToList<T>(Type type, IDataReader dataReader,string fields)
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
                return GetEntityList<T>(type, Context, dataReader,fields);
            }
        }
        public abstract string ChangeDBTypeToCSharpType(string typeName);
    }
}
