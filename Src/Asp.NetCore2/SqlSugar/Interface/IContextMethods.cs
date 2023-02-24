﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial interface IContextMethods
    {
        SqlSugarProvider Context { get; set; }
        QueryBuilder QueryBuilder { get; set; }
        ExpandoObject DataReaderToExpandoObject(IDataReader reader);
        List<ExpandoObject> DataReaderToExpandoObjectList(IDataReader reader);
        Task<List<ExpandoObject>> DataReaderToExpandoObjectListAsync(IDataReader dataReader);
        List<ExpandoObject> DataReaderToExpandoObjectListNoUsing(IDataReader reader);
        Task<List<ExpandoObject>> DataReaderToExpandoObjectListAsyncNoUsing(IDataReader dataReader);
        List<T> DataReaderToList<T>(IDataReader reader);
        List<T> DataReaderToSelectJsonList<T>(IDataReader reader);
        List<T> DataReaderToSelectArrayList<T>(IDataReader reader);
        Task<List<T>> DataReaderToSelectArrayListAsync<T>(IDataReader reader);
        Task<List<T>> DataReaderToSelectJsonListAsync<T>(IDataReader reader);
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
        DataTable ListToDataTable<T>(List<T> list);
        DataTable ListToDataTableWithAttr<T>(List<T> list);
        Dictionary<string, object> DataTableToDictionary(DataTable table);
        List<Dictionary<string, object>> DataTableToDictionaryList(DataTable table);
        ICacheService GetReflectionInoCacheInstance();
        void RemoveCacheAll();
        void RemoveCacheAll<T>();
        void RemoveCache<T>(string key);
        void PageEach<T>(IEnumerable<T> pageItems, int pageSize, Action<List<T>> action);
        Task PageEachAsync<T>(IEnumerable<T> pageItems, int pageSize, Func<List<T>, Task> action);
        Task PageEachAsync<T, ResultType>(IEnumerable<T> pageItems, int pageSize, Func<List<T>, Task<ResultType>> action);
        List<IConditionalModel> JsonToConditionalModels(string json);
        DataTable DictionaryListToDataTable(List<Dictionary<string, object>> dictionaryList);
        List<T> ToTree<T>(List<T> list, Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, Expression<Func<T, object>> pkExpression,  object rootValue);
    }
}
