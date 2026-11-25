using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// Vastbase 专属 ContextMethods。再声明 IContextMethods 激活 C# 接口重映射:
    /// 经 IContextMethods 引用调用时,下列方法的派发落到本类的 new 实现
    /// (基类 ContextMethods 同名成员在具体类引用下仍可直接调用,二者并存)。
    /// 构造期由 VastbaseV5Support.Install 装配(Utilities 换入),模式在构造时固化。
    /// </summary>
    public class VastbaseV5ContextMethods : ContextMethods, IContextMethods
    {
        private readonly bool _isBabelfish;

        public VastbaseV5ContextMethods(SqlSugarProvider context, bool isBabelfish)
        {
            this.Context = context;
            _isBabelfish = isBabelfish;
        }

        /// <summary>
        /// 元数据缓存按兼容模式分键:包装基类解析出的缓存实例(用户配置优先),
        /// 键加 vb-bf:/vb-pg: 前缀,PG 与 Babelfish 连接的元数据条目互不串用。
        /// </summary>
        public new ICacheService GetReflectionInoCacheInstance()
        {
            return new VastbaseV5ModeCacheService(base.GetReflectionInoCacheInstance(), _isBabelfish);
        }

        // Babelfish 数组列降级 text 读回(Select 单列路径 Sync/Async):PG 数组
        // 字面量文本 → 目标数组类型。PG 模式驱动返回原生数组(不走此分支)。
        public new List<T> DataReaderToSelectArrayList<T>(IDataReader reader)
        {
            List<T> result = new List<T>();
            using (reader)
            {
                while (reader.Read())
                {
                    var value = reader.GetValue(0);
                    if (value == null || value == DBNull.Value)
                    {
                        result.Add(default(T));
                    }
                    else
                    {
                        var tType = typeof(T);
                        if (value is string str && tType.IsArray)
                        {
                            result.Add((T)VastbaseV5PgArrayParser.Parse(str, tType.GetElementType()));
                        }
                        else
                        {
                            result.Add((T)value);
                        }
                    }
                }
            }

            return result;
        }

        public new async Task<List<T>> DataReaderToSelectArrayListAsync<T>(IDataReader reader)
        {
            List<T> result = new List<T>();
            using (reader)
            {
                while (await ((System.Data.Common.DbDataReader)reader).ReadAsync())
                {
                    var value = reader.GetValue(0);
                    if (value == null || value == DBNull.Value)
                    {
                        result.Add(default(T));
                    }
                    else
                    {
                        var tType = typeof(T);
                        if (value is string str && tType.IsArray)
                        {
                            result.Add((T)VastbaseV5PgArrayParser.Parse(str, tType.GetElementType()));
                        }
                        else
                        {
                            result.Add((T)value);
                        }
                    }
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 元数据缓存的模式键适配器:包装用户配置的 ICacheService(或框架默认实现),
    /// 全部键加兼容模式前缀——同一进程内 PG 与 Babelfish 连接的反射元数据条目
    /// 互不串用(条目内容依赖构建时的连接模式:列名归一与 json 映射均按模式分派)。
    /// </summary>
    internal sealed class VastbaseV5ModeCacheService : ICacheService
    {
        private readonly ICacheService _inner;
        private readonly string _prefix;

        public VastbaseV5ModeCacheService(ICacheService inner, bool isBabelfish)
        {
            _inner = inner;
            _prefix = isBabelfish ? "vb-bf:" : "vb-pg:";
        }

        public void Add<V>(string key, V value)
        {
            _inner.Add(_prefix + key, value);
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            _inner.Add(_prefix + key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            return _inner.ContainsKey<V>(_prefix + key);
        }

        public V Get<V>(string key)
        {
            return _inner.Get<V>(_prefix + key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            return _inner.GetAllKey<V>();
        }

        public void Remove<V>(string key)
        {
            _inner.Remove<V>(_prefix + key);
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            return _inner.GetOrCreate(_prefix + cacheKey, create, cacheDurationInSeconds);
        }
    }
}
