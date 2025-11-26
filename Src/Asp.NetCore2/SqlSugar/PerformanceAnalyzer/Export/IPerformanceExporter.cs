using System.Collections.Generic;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Export
{
    /// <summary>
    /// Interface for performance data export/import
    /// </summary>
    public interface IPerformanceExporter
    {
        void ExportToJson(string filePath, PerformanceReport report);
        void ExportToCsv(string filePath, PerformanceReport report);
        PerformanceReport ImportFromJson(string filePath);
        List<QueryPerformanceMetric> ImportMetricsFromJson(string filePath);
    }
}