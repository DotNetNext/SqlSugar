using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Reporters
{
    /// <summary>
    /// Interface for performance report generators
    /// </summary>
    public interface IPerformanceReporter
    {
        string Generate(PerformanceReport report);
    }
}