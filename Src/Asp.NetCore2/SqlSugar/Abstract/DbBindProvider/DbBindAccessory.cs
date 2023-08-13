using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class DbBindAccessory
    {
        public QueryBuilder QueryBuilder { get; set; }

        protected List<T> GetEntityList<T>(SqlSugarProvider context, IDataReader dataReader)
        {
            Type type = typeof(T);
            string types = null;
            var fieldNames = GetDataReaderNames(dataReader,ref types);
            string cacheKey = GetCacheKey(type,fieldNames) + types;
            var dataAfterFunc = context.CurrentConnectionConfig?.AopEvents?.DataExecuted;
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
                    //try
                    //{
                        var addItem = entytyList.Build(dataReader);
                        if (this.QueryBuilder?.QueryableFormats?.Any() == true) 
                        {
                          FormatT(addItem);
                        }
                        result.Add(addItem);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Check.Exception(true, ErrorMessage.EntityMappingError, ex.Message);
                    //}
                    SetAppendColumns(dataReader);
                }
                ExecuteDataAfterFun(context, dataAfterFunc, result);
            }
            catch(Exception ex)
            {
                if (ex.Message == "Common Language Runtime detected an invalid program.")
                {
                    Check.Exception(true, ErrorMessage.EntityMappingError, ex.Message);
                }
                else
                {
                    throw;
                }
            }
            return result;
        }

        protected async Task<List<T>> GetEntityListAsync<T>(SqlSugarProvider context, IDataReader dataReader)
        {
            Type type = typeof(T);
            string types = null;
            var fieldNames = GetDataReaderNames(dataReader,ref types);
            string cacheKey = GetCacheKey(type, fieldNames)+types;
            var dataAfterFunc = context.CurrentConnectionConfig?.AopEvents?.DataExecuted;
            IDataReaderEntityBuilder<T> entytyList = context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey, () =>
            {
                var cacheResult = new IDataReaderEntityBuilder<T>(context, dataReader, fieldNames).CreateBuilder(type);
                return cacheResult;
            });
            List<T> result = new List<T>();
            try
            {
                if (dataReader == null) return result;
                while (await((DbDataReader)dataReader).ReadAsync())
                {
                    //try
                    //{
                    var addItem = entytyList.Build(dataReader);
                    if (this.QueryBuilder?.QueryableFormats?.Any() == true)
                    {
                        FormatT(addItem);
                    }
                    result.Add(addItem);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Check.Exception(true, ErrorMessage.EntityMappingError, ex.Message);
                    //}
                    SetAppendColumns(dataReader);
                }
                ExecuteDataAfterFun(context, dataAfterFunc, result);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Common Language Runtime detected an invalid program.")
                {
                    Check.Exception(true, ErrorMessage.EntityMappingError, ex.Message);
                }
                else
                {
                    throw;
                }
            }
            return result;
        }

        private void FormatT<T>(T addItem)
        {
            var formats = this.QueryBuilder.QueryableFormats;
            var columns = this.QueryBuilder.Context.EntityMaintenance.GetEntityInfoWithAttr(typeof(T))
                .Columns.Where(it => formats.Any(y => y.PropertyName == it.PropertyName)).ToList();
            if (columns.Any())
            {
                foreach (var item in formats)
                {
                    var columnInfo = columns.FirstOrDefault(it => it.PropertyName == item.PropertyName);
                    var value = columnInfo.PropertyInfo.GetValue(addItem);
                    value = UtilMethods.GetFormatValue(value, item);
                    columnInfo.PropertyInfo.SetValue(addItem, value);
                }
            }
        }

        private static void ExecuteDataAfterFun<T>(SqlSugarProvider context, Action<object, DataAfterModel> dataAfterFunc, List<T> result)
        {
            if (dataAfterFunc != null)
            {
                var entity = context.EntityMaintenance.GetEntityInfo<T>();
                foreach (var item in result)
                {
                    dataAfterFunc(item, new DataAfterModel()
                    {
                        EntityColumnInfos = entity.Columns,
                        Entity = entity,
                        EntityValue = item
                    });
                }
            }
        }

        private string GetCacheKey(Type type,List<string> keys)
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

        private void SetAppendColumns(IDataReader dataReader)
        {
            if (QueryBuilder != null && QueryBuilder.AppendColumns != null && QueryBuilder.AppendColumns.Any())
            {
                if (QueryBuilder.AppendValues == null)
                    QueryBuilder.AppendValues = new List<List<QueryableAppendColumn>>();
                List<QueryableAppendColumn> addItems = new List<QueryableAppendColumn>();
                foreach (var item in QueryBuilder.AppendColumns)
                {
                    var vi = dataReader.GetOrdinal(item.AsName);
                    var value = dataReader.GetValue(vi);
                    addItems.Add(new QueryableAppendColumn()
                    {
                        Name = item.Name,
                        AsName = item.AsName,
                        Value = value
                    });
                }
                QueryBuilder.AppendValues.Add(addItems);
            }
            if (QueryBuilder?.AppendNavInfo != null) 
            {
                var navResult = new AppendNavResult();
                foreach (var item in QueryBuilder?.AppendNavInfo.AppendProperties)
                {
                    var vi = dataReader.GetOrdinal("SugarNav_" + item.Key);
                    var value = dataReader.GetValue(vi);
                    navResult.result.Add("SugarNav_"+item.Key,value);
                }
                QueryBuilder?.AppendNavInfo.Result.Add(navResult);
            }
        }

        private List<string> GetDataReaderNames(IDataReader dataReader,ref string types)
        {
            List<string> keys = new List<string>();
            StringBuilder sbTypes = new StringBuilder();
            var count = dataReader.FieldCount;
            for (int i = 0; i < count; i++)
            {
                keys.Add(dataReader.GetName(i));
                var type = dataReader.GetFieldType(i);
                if (type != null)
                {
                    sbTypes.Append(type.Name.Substring(0, 2));
                }
            }
            types = sbTypes.ToString();
            return keys;
        }

        protected List<T> GetKeyValueList<T>(Type type, IDataReader dataReader)
        {
            List<T> result = new List<T>();
            while (dataReader.Read())
            {
                GetKeyValueList(type, dataReader, result);
            }
            return result;
        }

        protected async Task<List<T>> GetKeyValueListAsync<T>(Type type, IDataReader dataReader)
        {
            List<T> result = new List<T>();
            while (await ((DbDataReader)dataReader).ReadAsync())
            {
                GetKeyValueList(type, dataReader, result);
            }
            return result;
        }

        private static void GetKeyValueList<T>(Type type, IDataReader dataReader, List<T> result)
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


        protected async Task<List<T>> GetArrayListAsync<T>(Type type, IDataReader dataReader)
        {
            List<T> result = new List<T>();
            int count = dataReader.FieldCount;
            var childType = type.GetElementType();
            while (await((DbDataReader)dataReader).ReadAsync())
            {
                GetArrayList(type, dataReader, result, count, childType);
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
                GetArrayList(type, dataReader, result, count, childType);
            }
            return result;
        }



        private static void GetArrayList<T>(Type type, IDataReader dataReader, List<T> result, int count, Type childType)
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

        protected List<T> GetValueTypeList<T>(Type type, IDataReader dataReader)
        {
            List<T> result = new List<T>();
            while (dataReader.Read())
            {
                GetValueTypeList(type, dataReader, result);
            }
            return result;
        }
        protected async Task<List<T>> GetValueTypeListAsync<T>(Type type, IDataReader dataReader)
        {
            List<T> result = new List<T>();
            while (await ((DbDataReader)dataReader).ReadAsync())
            {
                GetValueTypeList(type, dataReader, result);
            }
            return result;
        }

        private static void GetValueTypeList<T>(Type type, IDataReader dataReader, List<T> result)
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
                result.Add((T)UtilMethods.ChangeType2(value, type));
            }
        }
    }
}
