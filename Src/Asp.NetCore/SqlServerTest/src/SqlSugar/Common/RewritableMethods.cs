using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace SqlSugar
{
    public class RewritableMethods : IRewritableMethods
    {
        #region DataReader

        /// <summary>
        ///DataReader to Dynamic
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ExpandoObject DataReaderToExpandoObject(IDataReader reader)
        {
            ExpandoObject result = new ExpandoObject();
            var dic = ((IDictionary<string, object>)result);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    dic.Add(reader.GetName(i), reader.GetValue(i));
                }
                catch
                {
                    dic.Add(reader.GetName(i), null);
                }
            }
            return result;
        }

        /// <summary>
        ///DataReader to Dynamic List
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<ExpandoObject> DataReaderToExpandoObjectList(IDataReader reader)
        {
            List<ExpandoObject> result = new List<ExpandoObject>();
            if (reader != null && !reader.IsClosed)
            {
                while (reader.Read())
                {
                    result.Add(DataReaderToExpandoObject(reader));
                }
            }
            return result;
        }

        /// <summary>
        ///DataReader to DataReaderToDictionary
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Dictionary<string, object> DataReaderToDictionary(IDataReader reader)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    result.Add(reader.GetName(i), reader.GetValue(i));
                }
                catch
                {
                    result.Add(reader.GetName(i), null);
                }
            }
            return result;
        }

        /// <summary>
        /// DataReaderToDynamicList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<T> DataReaderToDynamicList<T>(IDataReader reader)
        {
            var tType = typeof(T);
            var classProperties = tType.GetProperties().ToList();
            var reval = new List<T>();
            if (reader != null && !reader.IsClosed)
            {
                while (reader.Read())
                {
                    var readerValues = DataReaderToDictionary(reader);
                    var result = new Dictionary<string, object>();
                    foreach (var item in classProperties)
                    {
                        var name = item.Name;
                        var typeName = tType.Name;
                        if (item.PropertyType.IsClass())
                        {
                            result.Add(name, DataReaderToDynamicList_Part(readerValues, item, reval));
                        }
                        else
                        {
                            if (readerValues.ContainsKey(name))
                            {
                                var addValue = readerValues[name];
                                if (addValue == DBNull.Value)
                                {
                                    if (item.PropertyType.IsIn(PubConst.IntType, PubConst.DecType, PubConst.DobType, PubConst.ByteType))
                                    {
                                        addValue = 0;
                                    }
                                    else if (item.PropertyType == PubConst.GuidType)
                                    {
                                        addValue = Guid.Empty;
                                    }
                                    else if (item.PropertyType == PubConst.DateType)
                                    {
                                        addValue = DateTime.MinValue;
                                    }
                                }
                                else
                                {
                                    if (item.PropertyType == PubConst.IntType)
                                    {
                                        addValue = Convert.ToInt32(addValue);
                                    }
                                }
                                result.Add(name, addValue);
                            }
                        }
                    }
                    var stringValue = SerializeObject(result);
                    reval.Add((T)DeserializeObject<T>(stringValue));
                }
                reader.Close();
            }
            return reval;
        }

        private Dictionary<string, object> DataReaderToDynamicList_Part<T>(Dictionary<string, object> readerValues, PropertyInfo item, List<T> reval)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var type = item.PropertyType;
            var classProperties = type.GetProperties().ToList();
            foreach (var prop in classProperties)
            {
                var name = prop.Name;
                var typeName = type.Name;
                if (prop.PropertyType.IsClass())
                {
                    result.Add(name, DataReaderToDynamicList_Part(readerValues, prop, reval));
                }
                else
                {
                    var key = typeName + "." + name;
                    if (readerValues.ContainsKey(key))
                    {
                        result.Add(name, readerValues[key]);
                    }
                }
            }
            return result;
        }
        #endregion

        #region Serialize
        /// <summary>
        /// Serialize Object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Serialize Object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T DeserializeObject<T>(string value)
        {
            if (value.IsValuable())
            {
                value = value.Replace(":{}", ":null");
            }
            var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.DeserializeObject<T>(value,jSetting);
        }
        #endregion

        #region Copy Object
        /// <summary>
        /// Copy new Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceObject"></param>
        /// <returns></returns>
        public T TranslateCopy<T>(T sourceObject)
        {
            if (sourceObject == null) return default(T);
            else
            {
                var jsonString = SerializeObject(sourceObject);
                return DeserializeObject<T>(jsonString);
            }
        }
        #endregion

        #region DataTable
        public dynamic DataTableToDynamic(DataTable table)
        {
            List<Dictionary<string, object>> deserializeObject = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                deserializeObject.Add(childRow);
            }
            return JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(deserializeObject));

        }
        #endregion

        #region Cache
        public ICacheManager<T> GetCacheInstance<T>()
        {
            return CacheManager<T>.GetInstance();
        }

        public void RemoveCacheAll()
        {
            CacheManager.RemoveAllCache();
        }

        public void RemoveCacheAll<T>()
        {
            CacheManager<T>.GetInstance().RemoveAllCache();
        }

        public void RemoveCache<T>(string key)
        {
            CacheManager<T>.GetInstance().Remove(key);
        }
        #endregion


    }
}
