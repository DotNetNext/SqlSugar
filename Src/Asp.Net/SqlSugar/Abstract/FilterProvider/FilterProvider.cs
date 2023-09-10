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
        public void Clear<T>()
        {
            _Filters = _Filters.Where(it => !(it is TableFilterItem<T>)).ToList();
        }
        public void Clear(params Type [] types)
        {
            _Filters = _Filters.Where(it => !types.Contains(it.type)).ToList();
        }
        public void Clear<T,T2>()
        {
            _Filters = _Filters.Where(it => !(it is TableFilterItem<T>) && !(it is TableFilterItem<T2>)).ToList();
        }
        public void Clear<T, T2,T3>()
        {
            _Filters = _Filters.Where(it => !(it is TableFilterItem<T>) && !(it is TableFilterItem<T2>) && !(it is TableFilterItem<T3>)).ToList();
        }
        public void ClearAndBackup()
        {
            _BackUpFilters = _Filters;
            _Filters = new List<SqlFilterItem>();
        }

        public void ClearAndBackup<T>()
        {
            _BackUpFilters = _Filters;
            _Filters = _BackUpFilters.Where(it=>!(it is TableFilterItem<T>)).ToList();
        }

        public void ClearAndBackup<T,T2>()
        {
            _BackUpFilters = _Filters;
            _Filters = _BackUpFilters.Where(it => !(it is TableFilterItem<T>)&&!(it is TableFilterItem<T2>)).ToList();
        }

        public void ClearAndBackup<T, T2 , T3>()
        {
            _BackUpFilters = _Filters;
            _Filters = _BackUpFilters.Where(it => !(it is TableFilterItem<T>) && !(it is TableFilterItem<T2>) && !(it is TableFilterItem<T3>)).ToList();
        }

        public void ClearAndBackup(params Type[] types)
        {
            _BackUpFilters = _Filters;
            _Filters = _BackUpFilters.Where(it =>!types.Contains(it.type)).ToList();
        }

        public void Restore() 
        {
            _Filters = _BackUpFilters;
            if (_Filters == null) 
            {
                _Filters = new List<SqlFilterItem>();
            }
        }

        public QueryFilterProvider AddTableFilter<T>(Expression<Func<T,bool>> expression, FilterJoinPosition filterJoinType = FilterJoinPosition.On) 
        {
            var isOn = filterJoinType == FilterJoinPosition.On;
            var tableFilter = new TableFilterItem<T>(expression, isOn);
            this.Add(tableFilter);
            return this;
        }
        public QueryFilterProvider AddTableFilterIF<T>(bool isAppendFilter,Expression<Func<T, bool>> expression, FilterJoinPosition filterJoinType = FilterJoinPosition.On)  
        {
            if (isAppendFilter) 
            {
                AddTableFilter(expression, filterJoinType);
            }
            return this;
        }
        public QueryFilterProvider AddTableFilter(Type type,string shortName, FormattableString expString, FilterJoinPosition filterJoinType = FilterJoinPosition.On)
        {
            var exp = DynamicCoreHelper.GetWhere(type, shortName, expString);
            return AddTableFilter(type, exp, filterJoinType);
        }
        public QueryFilterProvider AddTableFilter(Type type,Expression expression, FilterJoinPosition filterJoinType = FilterJoinPosition.On)
        {
            var isOn = filterJoinType == FilterJoinPosition.On;
            this.Add(new TableFilterItem<object>(type, expression, isOn));
            return this;
        }

        public QueryFilterProvider AddTableFilterIF(bool isAppendFilter, Type type, Expression expression, FilterJoinPosition posType = FilterJoinPosition.On)
        {
            if (isAppendFilter)
            {
                AddTableFilter(type, expression, posType);
            }
            return this;
        }
        public enum FilterJoinPosition
        {
            On=0,
            Where=1
        }
    }
}
