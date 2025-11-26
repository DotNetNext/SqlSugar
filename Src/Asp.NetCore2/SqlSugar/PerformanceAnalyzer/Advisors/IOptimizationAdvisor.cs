using System.Collections.Generic;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Advisors
{
    /// <summary>
    /// Interface for optimization advisors
    /// </summary>
    public interface IOptimizationAdvisor
    {
        List<OptimizationSuggestion> GetSuggestions(List<QueryPerformanceMetric> metrics);
    }
}