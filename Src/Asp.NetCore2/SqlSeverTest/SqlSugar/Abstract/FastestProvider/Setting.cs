using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class FastestProvider<T> : IFastest<T> where T : class, new()
    {
        private string AsName { get; set; }
        private int Size { get; set; }
        private string CacheKey { get; set; }
        private string CacheKeyLike { get; set; }
        public IFastest<T> RemoveDataCache() 
        {
            CacheKey = typeof(T).FullName;
            return this;
        }
        public IFastest<T> RemoveDataCache(string cacheKey) 
        {
            CacheKeyLike = this.context.EntityMaintenance.GetTableName<T>();
            return this;
        }
        public IFastest<T> AS(string tableName)
        {
            this.AsName = tableName;
            return this;
        }
        public IFastest<T> PageSize(int size)
        {
            this.Size = size;
            return this;
        }
        public SplitFastest<T> SplitTable() 
        {
            SplitFastest<T> result = new SplitFastest<T>();
            result.FastestProvider = this;
            return result;
        }
    }
}
