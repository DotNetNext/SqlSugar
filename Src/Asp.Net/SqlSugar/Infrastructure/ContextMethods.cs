using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class ContextMethods : IContextMethods
    {
        public SqlSugarClient Context { get; set; }
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
                    var addItem = reader.GetValue(i);
                    if (addItem == DBNull.Value)
                        addItem = null;
                    dic.Add(reader.GetName(i), addItem);
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
            using (reader)
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
                    var addItem = reader.GetValue(i);
                    if (addItem == DBNull.Value)
                        addItem = null;
                    result.Add(reader.GetName(i), addItem);
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
            using (reader)
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
                                        if (item.PropertyType.IsIn(UtilConstants.IntType, UtilConstants.DecType, UtilConstants.DobType, UtilConstants.ByteType))
                                        {
                                            addValue = 0;
                                        }
                                        else if (item.PropertyType == UtilConstants.GuidType)
                                        {
                                            addValue = Guid.Empty;
                                        }
                                        else if (item.PropertyType == UtilConstants.DateType)
                                        {
                                            addValue = DateTime.MinValue;
                                        }
                                        else if (item.PropertyType == UtilConstants.StringType)
                                        {
                                            addValue = null;
                                        }
                                        else
                                        {
                                            addValue = null;
                                        }
                                    }
                                    else
                                    {
                                        if (item.PropertyType == UtilConstants.IntType)
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
                }
                return reval;
            }
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
                        var addItem = readerValues[key];
                        if (addItem == DBNull.Value)
                            addItem = null;
                        result.Add(name, addItem);
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
            DependencyManagement.TryJsonNet();
            return Context.CurrentConnectionConfig.ConfigureExternalServices.SerializeService.SerializeObject(value);
        }

   

        /// <summary>
        /// Serialize Object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T DeserializeObject<T>(string value)
        {
            DependencyManagement.TryJsonNet();
            return Context.CurrentConnectionConfig.ConfigureExternalServices.SerializeService.DeserializeObject<T>(value);
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
        public SqlSugarClient CopyContext(bool isCopyEvents=false)
        {
            var newClient = new SqlSugarClient(this.TranslateCopy(Context.CurrentConnectionConfig));
            newClient.MappingColumns = this.TranslateCopy(Context.MappingColumns);
            newClient.MappingTables = this.TranslateCopy(Context.MappingTables);
            newClient.IgnoreColumns = this.TranslateCopy(Context.IgnoreColumns);
            if (isCopyEvents) {
                newClient.Ado.IsEnableLogEvent = Context.Ado.IsEnableLogEvent;
                newClient.Ado.LogEventStarting = Context.Ado.LogEventStarting;
                newClient.Ado.LogEventCompleted = Context.Ado.LogEventCompleted;
                newClient.Ado.ProcessingEventStartingSQL = Context.Ado.ProcessingEventStartingSQL;
            }
            return newClient;
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
                    var addItem = row[col];
                    if (addItem == DBNull.Value)
                        addItem = null;
                    childRow.Add(col.ColumnName, addItem);
                }
                deserializeObject.Add(childRow);
            }
            return this.DeserializeObject<dynamic>(this.SerializeObject(deserializeObject));

        }
        #endregion

        #region Cache
        public ICacheService GetReflectionInoCacheInstance()
        {
            return Context.CurrentConnectionConfig.ConfigureExternalServices.ReflectionInoCache;
        }

        public void RemoveCacheAll()
        {
            ReflectionInoHelper.RemoveAllCache();
        }

        public void RemoveCacheAll<T>()
        {
            ReflectionInoCore<T>.GetInstance().RemoveAllCache();
        }

        public void RemoveCache<T>(string key)
        {
            ReflectionInoCore<T>.GetInstance().Remove(key);
        }
        #endregion


    }
}
