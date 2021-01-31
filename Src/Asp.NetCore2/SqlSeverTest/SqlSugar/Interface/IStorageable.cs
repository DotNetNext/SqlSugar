using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public interface IStorageable<T> where T : class, new()
    {
        IStorageable<T> WhereColumns(Expression<Func<T, object>> columns);
        IStorageable<T> SplitInsert(Func<StorageableInfo<T>, bool> conditions, string message=null);
        IStorageable<T> SplitUpdate(Func<StorageableInfo<T>, bool> conditions, string message = null);
        IStorageable<T> SplitError(Func<StorageableInfo<T>, bool> conditions, string message = null);
        IStorageable<T> SplitIgnore(Func<StorageableInfo<T>, bool> conditions, string message = null);
        IStorageable<T> SplitDelete(Func<StorageableInfo<T>, bool> conditions, string message = null);
        IStorageable<T> SplitOther(Func<StorageableInfo<T>, bool> conditions, string message = null);
        StorageableResult<T> ToStorage();
    }

    public class StorageableInfo<T> where T : class, new()
    {
        public T Item { get; set; }
        internal List<T> Database { get; set; }
        public bool Any(Func<T,bool> expression)
        {
            return Database.Any(expression);
        }
        public bool NotAny(Func<T, bool> expression)
        {
            return !Database.Any(expression);
        }
    }

    public class StorageableMessage<T> : StorageableInfo<T> where T : class, new()
    {
        public string StorageMessage { get; set; }
        public StorageType? StorageType { get; set; }
    }

    public enum StorageType
    {
        Insert=0,
        Update=1,
        Delete=2,
        Error=3,
        Other=4,
        Ignore=5,
    }
    internal struct KeyValuePair<TKey, TValue,TValue2>
    {
        public TKey key;
        public TValue value1;
        public TValue2 value2;
        public KeyValuePair(TKey key, TValue value1, TValue2 value2)
        {
            this.key = key;
            this.value1 = value1;
            this.value2 = value2;
        }
    }

    public class StorageableResult<T> where T : class, new()
    {
        public List<StorageableMessage<T>> TotalList { get; set; }
        public List<StorageableMessage<T>> InsertList { get; set; }
        public List<StorageableMessage<T>> UpdateList { get; set; }
        public List<StorageableMessage<T>> DeleteList { get; set; }
        public List<StorageableMessage<T>> ErrorList { get; set; }
        public List<StorageableMessage<T>> IgnoreList { get; set; }
        public List<StorageableMessage<T>> OtherList { get; set; }
        public IInsertable<T> AsInsertable { get; set; }
        public IUpdateable<T> AsUpdateable { get; set; }
        public IDeleteable<T> AsDeleteable { get; set; }
    }
}
