using System;
namespace SqlSugar
{
    /// <summary>
    /// http存储对象接口
    /// </summary>
    /// <typeparam name="V"></typeparam>
    internal abstract class IHttpStorageObject<V>
    {

        public int Minutes = 60;
        public int Hour = 60 * 60;
        public int Day = 60 * 60 * 24;
        public System.Web.HttpContext context = System.Web.HttpContext.Current;
        public abstract void Add(string key, V value);
        public abstract bool ContainsKey(string key);
        public abstract V Get(string key);
        public abstract global::System.Collections.Generic.IEnumerable<string> GetAllKey();
        public abstract void Remove(string key);
        public abstract void RemoveAll();
        public abstract void RemoveAll(Func<string, bool> removeExpression);
        public abstract V this[string key] { get; }
    }
}
