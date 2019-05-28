using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class QueryFilterProvider : IFilter
    {
        internal SqlSugarProvider Context { get; set; }
        private List<SqlFilterItem> _Filters { get; set; }

        public IFilter Add(SqlFilterItem filter)
        {
            if (_Filters == null)
                _Filters = new List<SqlFilterItem>();
            if (this.Context.CurrentConnectionConfig.IsShardSameThread)
            {
                if (!_Filters.Select(it => it.FilterValue(this.Context).Sql).Contains(filter.FilterValue(this.Context).Sql))
                    _Filters.Add(filter);
            }
            else
            {
                _Filters.Add(filter);
            }
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
    }
}
