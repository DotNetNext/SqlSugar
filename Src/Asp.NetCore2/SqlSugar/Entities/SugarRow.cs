using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Data;
using System.Collections;
using System.ComponentModel;

namespace SqlSugar
{
#nullable enable
    /// <summary>
    /// SqlSugar 的动态行对象，支持大小写匹配的字段名和不匹配的属性名
    /// </summary>
    public class SugarRow : DynamicObject
    {

        private readonly Dictionary<string, object?> _dic = new Dictionary<string, object?>();

        public SugarRow(IDictionary<string, object?> row)
        {
            _dic = new Dictionary<string, object?>(row);
        }

        public SugarRow()
        {

        }

        public SugarRow(DataRow row)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                _dic[column.ColumnName] = row[column.ColumnName];
            }
        }



        /// <summary>
        /// 如果存在大小写不一致的列名，则无法设置新值， 因为此对象主要用于Get操作
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            if (TryGetKey(binder.Name, out var key))
            {
                _dic[key] = value;
                return true;
            }
            _dic[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            if (TryGetKey(binder.Name, out var key))
            {
                return _dic.TryGetValue(key, out result);
            }
            return _dic.TryGetValue(binder.Name, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dic.Keys;
        }

        /// <summary>
        /// 获取存在的键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="existKey"></param>
        /// <remarks>这里将导致设置值的时候，无法设置大小写不一致的值</remarks>
        /// <returns></returns>
        private bool TryGetKey(string key, out string? existKey)
        {
            if (_dic.ContainsKey(key))
            {
                existKey = key;
                return true;
            }
            if (_dic.Keys.Any(x => string.Equals(x, key, StringComparison.OrdinalIgnoreCase)))
            {

                existKey = _dic.Keys.First(x => string.Equals(x, key, StringComparison.OrdinalIgnoreCase));
                return true;
            }

            existKey = key;

            return false;
        }


        #region IDictionary<string,object?> Members


        public ICollection<string> Keys => _dic.Keys;

        public ICollection<object?> Values => _dic.Values;

        public int Count => _dic.Count;



        public object? this[string key]
        {
            get
            {
                if (TryGetKey(key, out var pkey))
                {
                    return _dic[pkey];
                }
                return _dic[key];
            }
            set
            {
                if (TryGetKey(key, out var pkey))
                {
                    _dic[pkey] = value;
                    return;
                }
                _dic[key] = value;
            }
        }



        public bool ContainsKey(string key)
        {
            return TryGetKey(key, out _);
        }

        public bool ContainsKeyCaseSensitive(string key)
        {
            return _dic.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            if (TryGetKey(key, out var pkey))
            {
                _dic.Remove(pkey);
                return true;
            }
            return false;
        }

        public bool RemoveCaseSensitive(string key)
        {
            return _dic.Remove(key);
        }

        public bool TryGetValue(string key, out object? value)
        {
            if (TryGetKey(key, out var pkey))
            {
                return _dic.TryGetValue(pkey, out value);
            }
            return _dic.TryGetValue(key, out value);
        }

        public bool TryGetValueCaseSensitive(string key, out object? value)
        {
            return _dic.TryGetValue(key, out value);
        }

        public void SetValueCaseSensitive(string key, object? value)
        {
            _dic[key] = value;
        }

        #endregion


        #region 操作符重载

        public static explicit operator ExpandoObject(SugarRow row)
        {
            var obj = new ExpandoObject();
            var dic = (IDictionary<string, object?>)obj;
            foreach (var key in row.Keys)
            {
                dic[key] = row[key];
            }
            return obj;
        }

        #endregion
    }
#nullable restore
}
