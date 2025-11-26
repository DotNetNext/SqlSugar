using System;
using System.Linq;
using System.Linq.Expressions;

namespace SqlSugar.SoftDelete
{
    public class SoftDeleteFilterManager
    {
        private readonly SqlSugarClient _client;
        private readonly SoftDeleteConfig _config;
        private bool _filtersApplied = false;

        public SoftDeleteFilterManager(SqlSugarClient client, SoftDeleteConfig config)
        {
            _client = client;
            _config = config;
        }

        public void ApplyFilters()
        {
            if (_filtersApplied || !_config.AutoFilter) return;
            _filtersApplied = true;
            // Filters are applied per-query via QueryableExtensions instead of globally
        }

        public void RemoveFilters()
        {
            if (!_filtersApplied) return;
            _filtersApplied = false;
        }

        public void ApplyFilterForType<T>(Expression<Func<T, bool>> filter) where T : class
        {
            _client.QueryFilter.AddTableFilter(filter);
        }
    }
}
