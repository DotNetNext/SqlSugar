﻿using System;
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
            var cacheManager = context.RewritableMethods.GetCacheInstance<IDataReaderEntityBuilder<T>>();
            string key = "DataReaderToList." + fields + context.CurrentConnectionConfig.DbType + type.FullName;
            IDataReaderEntityBuilder<T> entytyList = null;
            if (cacheManager.ContainsKey(key))
            {
                entytyList = cacheManager[key];
            }
            else
            {
                entytyList =new IDataReaderEntityBuilder<T>(context,dataReader).CreateBuilder(type);
                cacheManager.Add(key, entytyList);
            }
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
            using (IDataReader re = dataReader)
            {
                while (re.Read())
                {
                    if (PubConst.DicOO == type)
                    {
                        var kv = new KeyValuePair<object, object>(dataReader.GetValue(0), re.GetValue(1));
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<object, object>)));
                    }
                    else if (PubConst.DicIS == type)
                    {
                        var kv = new KeyValuePair<int, string>(dataReader.GetValue(0).ObjToInt(), re.GetValue(1).ObjToString());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, string>)));
                    }
                    else if (PubConst.Dicii == type)
                    {
                        var kv = new KeyValuePair<int, int>(dataReader.GetValue(0).ObjToInt(), re.GetValue(1).ObjToInt());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, int>)));
                    }
                    else if (PubConst.DicSi == type)
                    {
                        var kv = new KeyValuePair<string, int>(dataReader.GetValue(0).ObjToString(), re.GetValue(1).ObjToInt());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, int>)));
                    }
                    else if (PubConst.DicSo == type)
                    {
                        var kv = new KeyValuePair<string, object>(dataReader.GetValue(0).ObjToString(), re.GetValue(1));
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, object>)));
                    }
                    else if (PubConst.DicSS == type)
                    {
                        var kv = new KeyValuePair<string, string>(dataReader.GetValue(0).ObjToString(), dataReader.GetValue(1).ObjToString());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, string>)));
                    }
                    else
                    {
                        Check.Exception(true, ErrorMessage.NotSupportedDictionary);
                    }
                }
            }
            return reval;
        }

        protected List<T> GetArrayList<T>(Type type, IDataReader dataReader)
        {
            List<T> reval = new List<T>();
            using (IDataReader re = dataReader)
            {
                int count = dataReader.FieldCount;
                var childType = type.GetElementType();
                while (re.Read())
                {
                    object[] array = new object[count];
                    for (int i = 0; i < count; i++)
                    {
                        array[i] = Convert.ChangeType(re.GetValue(i), childType);
                    }
                    if (childType == PubConst.StringType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToString()).ToArray(), type));
                    else if (childType == PubConst.ObjType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? null : (object)it).ToArray(), type));
                    else if (childType == PubConst.BoolType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToBool()).ToArray(), type));
                    else if (childType == PubConst.ByteType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? 0 : (byte)it).ToArray(), type));
                    else if (childType == PubConst.DecType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToDecimal()).ToArray(), type));
                    else if (childType == PubConst.GuidType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? Guid.Empty : (Guid)it).ToArray(), type));
                    else if (childType == PubConst.DateType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? DateTime.MinValue : (DateTime)it).ToArray(), type));
                    else if (childType == PubConst.IntType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToInt()).ToArray(), type));
                    else
                        Check.Exception(true, ErrorMessage.NotSupportedArray);
                }
            }
            return reval;
        }

        protected List<T> GetValueTypeList<T>(Type type, IDataReader dataReader)
        {
            List<T> reval = new List<T>();
            using (IDataReader re = dataReader)
            {
                while (re.Read())
                {
                    var value = re.GetValue(0);
                    if (value == DBNull.Value)
                    {
                        reval.Add(default(T));
                    }
                    else
                    {
                        reval.Add((T)Convert.ChangeType(re.GetValue(0), type));
                    }
                }
            }
            return reval;
        }
    }
}
