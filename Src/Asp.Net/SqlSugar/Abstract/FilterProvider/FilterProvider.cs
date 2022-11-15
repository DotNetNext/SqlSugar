using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using static SqlSugar.QueryFilterProvider;

namespace SqlSugar
{
    public class QueryFilterProvider : IFilter
    {
        internal SqlSugarProvider Context { get; set; }
        private List<SqlFilterItem> _Filters { get; set; }
        private List<SqlFilterItem> _BackUpFilters { get; set; }

        public IFilter Add(SqlFilterItem filter)
        {
            if (_Filters == null)
                _Filters = new List<SqlFilterItem>();
            //if (this.Context.CurrentConnectionConfig.IsShardSameThread)
            //{
            //    if (!_Filters.Select(it => it.FilterValue(this.Context).Sql).Contains(filter.FilterValue(this.Context).Sql))
            //        _Filters.Add(filter);
            //}
            //else
            //{
                _Filters.Add(filter);
            //}
            return this;
        }

        public void Remove(string filterName)
        {
            if (_Filters == null)
                _Filters = new List<SqlFilterItem>();
            _Filters.RemoveAll(it => it.FilterName == filterName);
        }

        public List<SqlFilterItem> GeFilterList
        {
            get
            {
                if (_Filters == null)
                    _Filters = new List<SqlFilterItem>();
                return _Filters;
            }
        }
        public void Clear()
        {
            _Filters = new List<SqlFilterItem>();
        }
        public void ClearAndBackup()
        {
            _BackUpFilters = _Filters;
            _Filters = new List<SqlFilterItem>();
        }

        public void Restore() 
        {
            _Filters = _BackUpFilters;
            if (_Filters == null) 
            {
                _Filters = new List<SqlFilterItem>();
            }
        }

        public void AddTableFilter<T>(Expression<Func<T,bool>> expression, FilterJoinPosition filterJoinType = FilterJoinPosition.On) where T : class,new()
        {
            var isOn = filterJoinType == FilterJoinPosition.On;
            var tableFilter = new TableFilterItem<T>(expression, isOn);
            this.Add(tableFilter);
        }
        public void AddTableFilterIF<T>(bool isAppendFilter,Expression<Func<T, bool>> expression, FilterJoinPosition filterJoinType = FilterJoinPosition.On) where T : class, new()
        {
            if (isAppendFilter) 
            {
                AddTableFilter(expression, filterJoinType);
            }
        }
        public void AddTableFilter(Type type,Expression expression, FilterJoinPosition filterJoinType = FilterJoinPosition.On)
        {
            var isOn = filterJoinType == FilterJoinPosition.On;
            this.Add(new TableFilterItem<object>(type, expression, isOn));
        }

        public void AddTableFilterIF(bool isAppendFilter, Type type, Expression expression, FilterJoinPosition posType = FilterJoinPosition.On)
        {
            if (isAppendFilter)
            {
                AddTableFilter(type, expression, posType);
            }
        }
        public enum FilterJoinPosition
        {
            On=0,
            Where=1
        }
    }
}
