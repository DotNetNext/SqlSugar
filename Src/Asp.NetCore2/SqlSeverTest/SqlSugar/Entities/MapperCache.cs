using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class MapperCache<T>
    {
        private Dictionary<string, object> caches = new Dictionary<string, object>();
        private List<T> _list { get; set; }
        private ISqlSugarClient _context { get; set; }
        public int GetIndex { get; set; }
        private MapperCache()
        {
        }
        public MapperCache(List<T> list, ISqlSugarClient context)
        {
            _list = list;
            _context = context;
        }
        public Result Get<Result>(Func<List<T>, Result> action)
        {
            GetIndex++;
            string key = "Get" +typeof(Result)+action.GetHashCode()+action.Method.Name;
            if (caches.ContainsKey(key))
            {
                return (Result)caches[key];
            }
            else
            {
                var result = action(_list);
                caches.Add(key, result);
                return result;
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, double?> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, double?>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, double> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, double>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, decimal?> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, decimal?>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, decimal> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, decimal>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, int?> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result,int?>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, int> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, int>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, long?> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, long?>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, long> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, long>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, string> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, string>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, Guid> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, Guid>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, Guid?> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, Guid?>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, DateTime> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, DateTime>(action, key);
            }
        }
        public List<Result> GetListByPrimaryKeys<Result>(Func<T, DateTime?> action) where Result : class, new()
        {
            {
                string key = "GetListById" + typeof(Result) + action.GetHashCode().ToString();
                return GetListByPrimaryKeys<Result, DateTime?>(action, key);
            }
        }
        private List<Result> GetListByPrimaryKeys<Result,FieldType>(Func<T, FieldType> action, string key) where Result : class, new()
        {
            if (caches.ContainsKey(key))
            {
                return (List<Result>)caches[key];
            }
            else
            {
                var ids = _list.Select(action).ToList().Distinct().ToList();
                var result = _context.Queryable<Result>().In(ids).ToList();
                caches.Add(key, result);
                return result;
            }
        }
    }
}
