using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial class DbBindAccessory
    {
        protected List<T> GetEntityList<T>(SqlSugarClient context, IDataReader dataReader)
        {
            Type type = typeof(T);
            var fieldNames = GetDataReaderNames(dataReader);
            string cacheKey = GetCacheKey(type,fieldNames);
            IDataReaderEntityBuilder<T> entytyList = context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey, () =>
            {
                var cacheResult = new IDataReaderEntityBuilder<T>(context, dataReader,fieldNames).CreateBuilder(type);
                return cacheResult;
            });
            List<T> result = new List<T>();
            try
            {
                if (dataReader == null) return result;
                while (dataReader.Read())
                {
                    result.Add(entytyList.Build(dataReader));
                }
            }
            catch (Exception ex)
            {
                Check.Exception(true, ErrorMessage.EntityMappingError, ex.Message);
            }
            return result;
        }

        private  string GetCacheKey(Type type,List<string> keys)
        {
            StringBuilder sb = new StringBuilder("DataReaderToList.");
            sb.Append(type.FullName);
            sb.Append(".");
            foreach (var item in keys)
            {
                sb.Append(item);
            }
            return sb.ToString();
        }

        private List<string> GetDataReaderNames(IDataReader dataReader)
        {
            List<string> keys = new List<string>();
            var count = dataReader.FieldCount;
            for (int i = 0; i < count; i++)
            {
                keys.Add(dataReader.GetName(i));
            }
            return keys;
        }

        protected List<T> GetKeyValueList<T>(Type type, IDataReader dataReader)
        {
            List<T> result = new List<T>();
            while (dataReader.Read())
            {
                if (UtilConstants.DicOO == type)
                {
                    var kv = new KeyValuePair<object, object>(dataReader.GetValue(0), dataReader.GetValue(1));
                    result.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<object, object>)));
                }
                else if (UtilConstants.DicIS == type)
                {
                    var kv = new KeyValuePair<int, string>(dataReader.GetValue(0).ObjToInt(), dataReader.GetValue(1).ObjToString());
                    result.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, string>)));
                }
                else if (UtilConstants.Dicii == type)
                {
                    var kv = new KeyValuePair<int, int>(dataReader.GetValue(0).ObjToInt(), dataReader.GetValue(1).ObjToInt());
                    result.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, int>)));
                }
                else if (UtilConstants.DicSi == type)
                {
                    var kv = new KeyValuePair<string, int>(dataReader.GetValue(0).ObjToString(), dataReader.GetValue(1).ObjToInt());
                    result.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, int>)));
                }
                else if (UtilConstants.DicSo == type)
                {
                    var kv = new KeyValuePair<string, object>(dataReader.GetValue(0).ObjToString(), dataReader.GetValue(1));
                    result.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, object>)));
                }
                else if (UtilConstants.DicSS == type)
                {
                    var kv = new KeyValuePair<string, string>(dataReader.GetValue(0).ObjToString(), dataReader.GetValue(1).ObjToString());
                    result.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, string>)));
                }
                else
                {
                    Check.Exception(true, ErrorMessage.NotSupportedDictionary);
                }
            }
            return result;
        }

        protected List<T> GetArrayList<T>(Type type, IDataReader dataReader)
        {
            List<T> result = new List<T>();
            int count = dataReader.FieldCount;
            var childType = type.GetElementType();
            while (dataReader.Read())
            {
                object[] array = new object[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = Convert.ChangeType(dataReader.GetValue(i), childType);
                }
                if (childType == UtilConstants.StringType)
                    result.Add((T)Convert.ChangeType(array.Select(it => it.ObjToString()).ToArray(), type));
                else if (childType == UtilConstants.ObjType)
                    result.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? null : (object)it).ToArray(), type));
                else if (childType == UtilConstants.BoolType)
                    result.Add((T)Convert.ChangeType(array.Select(it => it.ObjToBool()).ToArray(), type));
                else if (childType == UtilConstants.ByteType)
                    result.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? 0 : (byte)it).ToArray(), type));
                else if (childType == UtilConstants.DecType)
                    result.Add((T)Convert.ChangeType(array.Select(it => it.ObjToDecimal()).ToArray(), type));
                else if (childType == UtilConstants.GuidType)
                    result.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? Guid.Empty : (Guid)it).ToArray(), type));
                else if (childType == UtilConstants.DateType)
                    result.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? DateTime.MinValue : (DateTime)it).ToArray(), type));
                else if (childType == UtilConstants.IntType)
                    result.Add((T)Convert.ChangeType(array.Select(it => it.ObjToInt()).ToArray(), type));
                else
                    Check.Exception(true, ErrorMessage.NotSupportedArray);
            }
            return result;
        }

        protected List<T> GetValueTypeList<T>(Type type, IDataReader dataReader)
        {
            List<T> result = new List<T>();
            while (dataReader.Read())
            {
                var value = dataReader.GetValue(0);
                if (type == UtilConstants.GuidType)
                {
                    value = Guid.Parse(value.ToString());
                }
                if (value == DBNull.Value)
                {
                    result.Add(default(T));
                }
                else if (type.IsEnum)
                {
                    result.Add((T)Enum.Parse(type, value.ObjToString()));
                }
                else
                {
                    result.Add((T)Convert.ChangeType(value, UtilMethods.GetUnderType(type)));
                }
            }
            return result;
        }
    }
}
