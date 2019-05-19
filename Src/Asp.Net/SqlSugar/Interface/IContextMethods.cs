using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial interface IContextMethods
    {
        SqlSugarProvider Context { get; set; }
        ExpandoObject DataReaderToExpandoObject(IDataReader reader);
        List<ExpandoObject> DataReaderToExpandoObjectList(IDataReader reader);
        Task<List<ExpandoObject>> DataReaderToExpandoObjectListAsync(IDataReader dataReader);
        List<ExpandoObject> DataReaderToExpandoObjectListNoUsing(IDataReader reader);
        Task<List<ExpandoObject>> DataReaderToExpandoObjectListAsyncNoUsing(IDataReader dataReader);
        List<T> DataReaderToList<T>(IDataReader reader);
        List<T> DataReaderToListNoUsing<T>(IDataReader reader);
        Task<List<T>> DataReaderToListAsync<T>(IDataReader dataReader);
        Task<List<T>> DataReaderToListAsyncNoUsing<T>(IDataReader dataReader);
        string SerializeObject(object value);
        string SerializeObject(object value, Type type);
        T DeserializeObject<T>(string value);
        T TranslateCopy<T>(T sourceObject);
        SqlSugarProvider CopyContext(bool isCopyEvents = false);
        dynamic DataTableToDynamic(DataTable table);
        List<T> DataTableToList<T>(DataTable table);
        Dictionary<string, object> DataTableToDictionary(DataTable table);
        ICacheService GetReflectionInoCacheInstance();
        void RemoveCacheAll();
        void RemoveCacheAll<T>();
        void RemoveCache<T>(string key);
        void PageEach<T>(IEnumerable<T> pageItems, int pageSize, Action<List<T>> action);
    }
}
