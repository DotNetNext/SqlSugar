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
        IStorageable<T> Saveable(string inserMessage = null, string updateMessage = null);
        IStorageable<T> SplitError(Func<StorageableInfo<T>, bool> conditions, string message = null);
        IStorageable<T> SplitIgnore(Func<StorageableInfo<T>, bool> conditions, string message = null);
        IStorageable<T> SplitDelete(Func<StorageableInfo<T>, bool> conditions, string message = null);
        IStorageable<T> SplitOther(Func<StorageableInfo<T>, bool> conditions, string message = null);
        StorageableResult<T> ToStorage();
        IStorageable<T> As(string tableName);
    }

    public class StorageableInfo<T> where T : class, new()
    {
        public T Item { get; set; }
        internal List<T> Database { get; set; }
        internal string[] PkFields { get; set; }
        public bool Any(Func<T,bool> expression)
        {
            return Database.Any(expression);
        }
        public bool NotAny(Func<T, bool> expression)
        {
            return !Database.Any(expression);
        }
        public bool Any()
        {
            var  list = Database.Where(it=>true);
            foreach (var pk in PkFields)
            {
                list = list.Where(it =>IsEquals(it, pk));
            }
            return list.Any();
        }

        private bool IsEquals(T it, string pk)
        {
            var leftValue = it.GetType().GetProperty(pk).GetValue(it, null);
            var rightValue = Item.GetType().GetProperty(pk).GetValue(Item, null);
            var left = leftValue.ObjToString();
            var rigth = rightValue.ObjToString();
            if (it.GetType().GetProperty(pk).PropertyType == UtilConstants.DecType)
            {
                return Convert.ToDecimal(leftValue) == Convert.ToDecimal(rightValue);
            }
            else
            {
                return left == rigth;
            }
        }

        public bool NotAny()
        {
            return !Any();
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
        internal List<EntityColumnInfo> _WhereColumnList { get;  set; }
        internal string _AsName { get;  set; }
        internal SqlSugarProvider _Context { get;  set; }

        public int BulkCopy()
        {
            return this._Context.Fastest<T>().AS(_AsName).BulkCopy(InsertList.Select(it=>it.Item).ToList());
        }
        public Task<int> BulkCopyAsync()
        {
            return this._Context.Fastest<T>().AS(_AsName).BulkCopyAsync(InsertList.Select(it => it.Item).ToList());
        }

        public int BulkUpdate()
        {
            var isWhereColums = _WhereColumnList != null && _WhereColumnList.Any();
            if (isWhereColums)
            {
                var updateColumns = this._Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => !it.IsPrimarykey && !it.IsIdentity && !it.IsOnlyIgnoreUpdate && !it.IsIgnore).Select(it => it.DbColumnName ?? it.PropertyName).ToArray();
                return BulkUpdate(updateColumns);
            }
            else
            {
                return this._Context.Fastest<T>().AS(_AsName).BulkUpdate(UpdateList.Select(it => it.Item).ToList());
            }
        }
        public Task<int> BulkUpdateAsync()
        {
            var isWhereColums = _WhereColumnList != null && _WhereColumnList.Any();
            if (isWhereColums)
            {
                var updateColumns = this._Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => !it.IsPrimarykey && !it.IsIdentity && !it.IsOnlyIgnoreUpdate && !it.IsIgnore).Select(it => it.DbColumnName ?? it.PropertyName).ToArray();
                return BulkUpdateAsync(updateColumns);
            }
            else
            {
                return this._Context.Fastest<T>().AS(_AsName).BulkUpdateAsync(UpdateList.Select(it => it.Item).ToList());
            }
        }
        public int BulkUpdate(params string[] UpdateColumns)
        {

            Check.Exception(UpdateColumns==null, "UpdateColumns is null");
            if (_WhereColumnList != null && _WhereColumnList.Any())
            {
                return this._Context.Fastest<T>().AS(_AsName).BulkUpdate(UpdateList.Select(it => it.Item).ToList(), _WhereColumnList.Select(it => it.DbColumnName).ToArray(), UpdateColumns);
            }
            else 
            {
                var pkColumns = this._Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToArray();
                Check.Exception(pkColumns.Count()==0,"need primary key");
                return this._Context.Fastest<T>().AS(_AsName).BulkUpdate(UpdateList.Select(it => it.Item).ToList(), pkColumns, UpdateColumns);
            }
        }
        public async Task<int> BulkUpdateAsync(params string[] UpdateColumns)
        {
            Check.Exception(UpdateColumns == null, "UpdateColumns is null");
            if (_WhereColumnList != null && _WhereColumnList.Any())
            {
                return  await this._Context.Fastest<T>().AS(_AsName).BulkUpdateAsync(UpdateList.Select(it => it.Item).ToList(), _WhereColumnList.Select(it => it.DbColumnName).ToArray(), UpdateColumns);
            }
            else
            {
                var pkColumns = this._Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToArray();
                Check.Exception(pkColumns.Count() == 0, "need primary key");
                return await this._Context.Fastest<T>().AS(_AsName).BulkUpdateAsync(UpdateList.Select(it => it.Item).ToList(), pkColumns, UpdateColumns);
            }
        }
    }
}
