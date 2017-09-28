using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial class DbBindAccessory
    {
        protected List<T> GetEntityList<T>(SqlSugarClient context, IDataReader dataReader, string fields)
        {
            Type type = typeof(T);
            string key = "DataReaderToList." + fields + context.CurrentConnectionConfig.DbType + type.FullName;
            IDataReaderEntityBuilder<T> entytyList = context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(key, () =>
            {
                var cacheResult = new IDataReaderEntityBuilder<T>(context, dataReader).CreateBuilder(type);
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

        protected List<T> GetKeyValueList<T>(Type type, IDataReader dataReader)
        {
            List<T> reval = new List<T>();
            while (dataReader.Read())
            {
                if (UtilConstants.DicOO == type)
                {
                    var kv = new KeyValuePair<object, object>(dataReader.GetValue(0), dataReader.GetValue(1));
                    reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<object, object>)));
                }
                else if (UtilConstants.DicIS == type)
                {
                    var kv = new KeyValuePair<int, string>(dataReader.GetValue(0).ObjToInt(), dataReader.GetValue(1).ObjToString());
                    reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, string>)));
                }
                else if (UtilConstants.Dicii == type)
                {
                    var kv = new KeyValuePair<int, int>(dataReader.GetValue(0).ObjToInt(), dataReader.GetValue(1).ObjToInt());
                    reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, int>)));
                }
                else if (UtilConstants.DicSi == type)
                {
                    var kv = new KeyValuePair<string, int>(dataReader.GetValue(0).ObjToString(), dataReader.GetValue(1).ObjToInt());
                    reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, int>)));
                }
                else if (UtilConstants.DicSo == type)
                {
                    var kv = new KeyValuePair<string, object>(dataReader.GetValue(0).ObjToString(), dataReader.GetValue(1));
                    reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, object>)));
                }
                else if (UtilConstants.DicSS == type)
                {
                    var kv = new KeyValuePair<string, string>(dataReader.GetValue(0).ObjToString(), dataReader.GetValue(1).ObjToString());
                    reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, string>)));
                }
                else
                {
                    Check.Exception(true, ErrorMessage.NotSupportedDictionary);
                }
            }
            return reval;
        }

        protected List<T> GetArrayList<T>(Type type, IDataReader dataReader)
        {
            List<T> reval = new List<T>();
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
                    reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToString()).ToArray(), type));
                else if (childType == UtilConstants.ObjType)
                    reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? null : (object)it).ToArray(), type));
                else if (childType == UtilConstants.BoolType)
                    reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToBool()).ToArray(), type));
                else if (childType == UtilConstants.ByteType)
                    reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? 0 : (byte)it).ToArray(), type));
                else if (childType == UtilConstants.DecType)
                    reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToDecimal()).ToArray(), type));
                else if (childType == UtilConstants.GuidType)
                    reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? Guid.Empty : (Guid)it).ToArray(), type));
                else if (childType == UtilConstants.DateType)
                    reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? DateTime.MinValue : (DateTime)it).ToArray(), type));
                else if (childType == UtilConstants.IntType)
                    reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToInt()).ToArray(), type));
                else
                    Check.Exception(true, ErrorMessage.NotSupportedArray);
            }
            return reval;
        }

        protected List<T> GetValueTypeList<T>(Type type, IDataReader dataReader)
        {
            List<T> reval = new List<T>();
            while (dataReader.Read())
            {
                var value = dataReader.GetValue(0);
                if (value == DBNull.Value)
                {
                    reval.Add(default(T));
                }
                else
                {
                    reval.Add((T)Convert.ChangeType(dataReader.GetValue(0), UtilMethods.GetUnderType(type)));
                }
            }
            return reval;
        }
    }
}
