using System.Collections.Generic;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Collectors
{
    /// <summary>
    /// Interface for performance data collectors
    /// </summary>
    public interface IPerformanceCollector
    {
        void StartTracking(string sql, SugarParameter[] parameters);
        void StopTracking(string sql, int rowCount, bool isSuccess, string errorMessage = null);
        List<QueryPerformanceMetric> GetMetrics(int count = 0);
        void Clear();
    }
}