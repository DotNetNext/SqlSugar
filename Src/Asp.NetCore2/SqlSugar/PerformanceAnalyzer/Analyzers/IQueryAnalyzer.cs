using System.Collections.Generic;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Analyzers
{
    /// <summary>
    /// Interface for query analyzers
    /// </summary>
    public interface IQueryAnalyzer
    {
        List<OptimizationSuggestion> Analyze(List<QueryPerformanceMetric> metrics);
    }
}