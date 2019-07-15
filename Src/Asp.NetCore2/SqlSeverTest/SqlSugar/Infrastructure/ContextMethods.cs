﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class ContextMethods : IContextMethods
    {
        public SqlSugarProvider Context { get; set; }
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
        ///DataReader to Dynamic List
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public async Task<List<ExpandoObject>> DataReaderToExpandoObjectListAsync(IDataReader reader)
        {
            using (reader)
            {
                List<ExpandoObject> result = new List<ExpandoObject>();
                if (reader != null && !reader.IsClosed)
                {
                    while (await((DbDataReader)reader).ReadAsync())
                    {
                        result.Add(DataReaderToExpandoObject(reader));
                    }
                }
                return result;
            }
        }


        /// <summary>
        ///DataReader to Dynamic List
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<ExpandoObject> DataReaderToExpandoObjectListNoUsing(IDataReader reader)
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
        ///DataReader to Dynamic List
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public async Task<List<ExpandoObject>> DataReaderToExpandoObjectListAsyncNoUsing(IDataReader reader)
        {
            List<ExpandoObject> result = new List<ExpandoObject>();
            if (reader != null && !reader.IsClosed)
            {
                while (await ((DbDataReader)reader).ReadAsync())
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
        ///DataReader to DataReaderToDictionary
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Dictionary<string, object> DataReaderToDictionary(IDataReader reader, Type type)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i);
                try
                {
                    name = this.Context.EntityMaintenance.GetPropertyName(name, type);
                    var addItem = reader.GetValue(i);
                    if (addItem == DBNull.Value)
                        addItem = null;
                    result.Add(name, addItem);
                }
                catch
                {
                    result.Add(name, null);
                }
            }
            return result;
        }

        /// <summary>
        /// DataReaderToList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<T> DataReaderToList<T>(IDataReader reader)
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
                        Dictionary<string, object> result = DataReaderToList(reader, tType, classProperties, reval);
                        var stringValue = SerializeObject(result);
                        reval.Add((T)DeserializeObject<T>(stringValue));
                    }
                }
                return reval;
            }
        }
        /// <summary>
        /// DataReaderToList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<T> DataReaderToListNoUsing<T>(IDataReader reader)
        {
                var tType = typeof(T);
                var classProperties = tType.GetProperties().ToList();
                var reval = new List<T>();
                if (reader != null && !reader.IsClosed)
                {
                    while (reader.Read())
                    {
                        Dictionary<string, object> result = DataReaderToList(reader, tType, classProperties, reval);
                        var stringValue = SerializeObject(result);
                        reval.Add((T)DeserializeObject<T>(stringValue));
                    }
                }
                return reval;
        }
        /// <summary>
        /// DataReaderToList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public async Task<List<T>> DataReaderToListAsync<T>(IDataReader reader)
        {
            using (reader)
            {
                var tType = typeof(T);
                var classProperties = tType.GetProperties().ToList();
                var reval = new List<T>();
                if (reader != null && !reader.IsClosed)
                {
                    while (await ((DbDataReader)reader).ReadAsync())
                    {
                        Dictionary<string, object> result = DataReaderToList(reader, tType, classProperties, reval);
                        var stringValue = SerializeObject(result);
                        reval.Add((T)DeserializeObject<T>(stringValue));
                    }
                }
                return reval;
            }
        }
        /// <summary>
        /// DataReaderToList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public async Task<List<T>> DataReaderToListAsyncNoUsing<T>(IDataReader reader)
        {
            var tType = typeof(T);
            var classProperties = tType.GetProperties().ToList();
            var reval = new List<T>();
            if (reader != null && !reader.IsClosed)
            {
                while (await ((DbDataReader)reader).ReadAsync())
                {
                    Dictionary<string, object> result = DataReaderToList(reader, tType, classProperties, reval);
                    var stringValue = SerializeObject(result);
                    reval.Add((T)DeserializeObject<T>(stringValue));
                }
            }
            return reval;
        }

        private Dictionary<string, object> DataReaderToList<T>(IDataReader reader, Type tType, List<PropertyInfo> classProperties, List<T> reval)
        {
            var readerValues = DataReaderToDictionary(reader, tType);
            var result = new Dictionary<string, object>();
            foreach (var item in classProperties)
            {
                var name = item.Name;
                var typeName = tType.Name;
                Type columnType = item.PropertyType;
                if (item.PropertyType.IsGenericType && item.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                { 
                    columnType = item.PropertyType.GetGenericArguments()[0];
                }

                if (columnType.IsClass())
                {
                    result.Add(name, DataReaderToDynamicList_Part(readerValues, item, reval));
                }
                else
                {
                    if (readerValues.Any(it => it.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var addValue = readerValues.ContainsKey(name) ? readerValues[name] : readerValues.First(it => it.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase)).Value;
                        if (addValue == DBNull.Value || addValue == null)
                        {
                            if (columnType.IsIn(UtilConstants.IntType, UtilConstants.DecType, UtilConstants.DobType, UtilConstants.ByteType))
                            {
                                addValue = 0;
                            }
                            else if (columnType == UtilConstants.GuidType)
                            {
                                addValue = Guid.Empty;
                            }
                            else if (columnType == UtilConstants.DateType)
                            {
                                addValue = DateTime.MinValue;
                            }
                            else if (columnType == UtilConstants.StringType)
                            {
                                addValue = null;
                            }
                            else
                            {
                                addValue = null;
                            }
                        }
                        else if (columnType == UtilConstants.IntType)
                        {
                            addValue = Convert.ToInt32(addValue);
                        }
                        else if (columnType == UtilConstants.LongType)
                        {
                            addValue = Convert.ToInt64(addValue);
                        }
                        result.Add(name, addValue);
                    }
                }
            }

            return result;
        }

        private Dictionary<string, object> DataReaderToDynamicList_Part<T>(Dictionary<string, object> readerValues, PropertyInfo item, List<T> reval)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var type = item.PropertyType;
            if (UtilConstants.SugarType == type)
            {
                return result;
            }
            if (type.FullName.IsCollectionsList())
            {
                return null;
            }
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
                    var info = readerValues.Select(it => it.Key).FirstOrDefault(it => it.ToLower() == key.ToLower());
                    if (info != null)
                    {
                        var addItem = readerValues[info];
                        if (addItem == DBNull.Value)
                            addItem = null;
                        if (prop.PropertyType == UtilConstants.IntType)
                        {
                            addItem = addItem.ObjToInt();
                        }
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
        public string SerializeObject(object value, Type type)
        {
            DependencyManagement.TryJsonNet();
            if (type.IsAnonymousType())
            {
                return Context.CurrentConnectionConfig.ConfigureExternalServices.SerializeService.SerializeObject(value);
            }
            else
            {
                var isSugar = this.Context.EntityMaintenance.GetEntityInfo(type).Columns.Any(it=>it.NoSerialize || it.SerializeDateTimeFormat.HasValue());
                if (isSugar)
                {
                    return Context.CurrentConnectionConfig.ConfigureExternalServices.SerializeService.SugarSerializeObject(value);
                }
                else
                {
                    return Context.CurrentConnectionConfig.ConfigureExternalServices.SerializeService.SerializeObject(value);
                }
            }
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
        public SqlSugarProvider CopyContext(bool isCopyEvents = false)
        {
            var newClient = new SqlSugarProvider(this.TranslateCopy(Context.CurrentConnectionConfig));
            newClient.CurrentConnectionConfig.ConfigureExternalServices = Context.CurrentConnectionConfig.ConfigureExternalServices;
            newClient.MappingColumns = this.TranslateCopy(Context.MappingColumns);
            newClient.MappingTables = this.TranslateCopy(Context.MappingTables);
            newClient.IgnoreColumns = this.TranslateCopy(Context.IgnoreColumns);
            newClient.IgnoreInsertColumns = this.TranslateCopy(Context.IgnoreInsertColumns);
            if (isCopyEvents)
            {
                newClient.QueryFilter = Context.QueryFilter;
                newClient.CurrentConnectionConfig.AopEvents = Context.CurrentConnectionConfig.AopEvents;
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
        public List<T> DataTableToList<T>(DataTable table)
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
            return this.DeserializeObject<List<T>>(this.SerializeObject(deserializeObject));
        }
        public Dictionary<string, object> DataTableToDictionary(DataTable table)
        {
           return table.Rows.Cast<DataRow>().ToDictionary(x => x[0].ToString(), x => x[1]);
        }

        #endregion

        #region Cache
        public ICacheService GetReflectionInoCacheInstance()
        {
            return Context.CurrentConnectionConfig.ConfigureExternalServices.ReflectionInoCacheService;
        }

        public void RemoveCacheAll()
        {
            ReflectionInoHelper.RemoveAllCache();
            InstanceFactory.RemoveCache();
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

        #region
        public void PageEach<T>(IEnumerable<T> pageItems,int pageSize, Action<List<T>> action)
        {
            if (pageItems != null&& pageItems.Any())
            {
                int totalRecord = pageItems.Count();
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                for (int i = 1; i <= pageCount; i++)
                {
                    var list = pageItems.Skip((i - 1) * pageSize).Take(pageSize).ToList();
                    action(list);
                }
            }
        }
        #endregion
    }
}
