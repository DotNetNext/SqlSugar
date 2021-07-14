using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class CallContextAsync<T>
    {
        static ConcurrentDictionary<string, AsyncLocal<T>> state = new ConcurrentDictionary<string, AsyncLocal<T>>();
        public static void SetData(string name, T data) =>
            state.GetOrAdd(name, _ => new AsyncLocal<T>()).Value = data;
        public static T GetData(string name) =>
            state.TryGetValue(name, out AsyncLocal<T> data) ? data.Value : default(T);
    }

    public class CallContextThread<T>
    {
        static ConcurrentDictionary<string, ThreadLocal<T>> state = new ConcurrentDictionary<string, ThreadLocal<T>>();
        public static void SetData(string name, T data) =>
            state.GetOrAdd(name, _ => new ThreadLocal<T>()).Value = data;
        public static T GetData(string name) =>
            state.TryGetValue(name, out ThreadLocal<T> data) ? data.Value : default(T);
    }
}
