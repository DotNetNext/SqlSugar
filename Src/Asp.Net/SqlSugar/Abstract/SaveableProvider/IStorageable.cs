using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class Storageable<T> : IStorageable<T> where T : class, new()
    {
        List<StorageableInfo<T>> datas = new List<StorageableInfo<T>>();

        List<KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>> whereFuncs = new List<KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>,string>>();
        SqlSugarProvider Context { get; set; }
        Expression<Func<T, object>> columns;
        public Storageable(List<T> datas, SqlSugarProvider context)
        {
            this.Context = context;
            if (datas == null)
                datas = new List<T>();
            this.datas = datas.Select(it => new StorageableInfo<T>()
            {
                Item = it
            }).ToList();
        }

        public IStorageable<T> SplitInsert(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>,string>(StorageType.Insert, conditions, message));
            return this;
        }
        public IStorageable<T> SplitDelete(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Delete, conditions, message));
            return this;
        }
        public IStorageable<T> SplitUpdate(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Update, conditions, message));
            return this;
        }
        public IStorageable<T> SplitError(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Error, conditions, message));
            return this;
        }

        public IStorageable<T> SplitIgnore(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Ignore, conditions, message));
            return this;
        }

        public IStorageable<T> SplitOther(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Other, conditions, message));
            return this;
        }

        public StorageableResult<T> ToStorage()
        {
            if (this.datas.Count == 0)
                return new StorageableResult<T>();
            foreach (var item in whereFuncs.OrderByDescending(it => (int)it.key))
            {
                List<StorageableInfo<T>> whereList = datas.Where(it => it.StorageType == null).ToList();
                Func<StorageableInfo<T>, bool> exp = item.value1;
                var list = whereList.Where(exp).ToList();
                foreach (var it in list)
                {
                    it.StorageType = item.key;
                    it.StorageMessage = item.value2;
                }
            }
            var delete = datas.Where(it => it.StorageType == StorageType.Delete).ToList();
            var update = datas.Where(it => it.StorageType == StorageType.Update).ToList();
            var inset = datas.Where(it => it.StorageType == StorageType.Insert).ToList();
            var error = datas.Where(it => it.StorageType == StorageType.Error).ToList();
            var ignore = datas.Where(it => it.StorageType == StorageType.Ignore).ToList();
            var other = datas.Where(it => it.StorageType == StorageType.Other).ToList();
            StorageableResult<T> result = new StorageableResult<T>()
            {
                AsDeleteable = this.Context.Deleteable(delete.Select(it => it.Item).ToList()),
                AsUpdateable = this.Context.Updateable(update.Select(it => it.Item).ToList()),
                AsInsertable = this.Context.Insertable(inset.Select(it => it.Item).ToList()),
                OtherList = other,
                InsertList = inset,
                DeleteList = delete,
                UpdateList = update,
                ErrorList = error,
                IgnoreList = ignore,
                TotalList = datas
            };
            return result;
        }


        public IStorageable<T> WhereColumns(Expression<Func<T, object>> columns)
        {
            this.columns = columns;
            return this;
        }

    }
}
