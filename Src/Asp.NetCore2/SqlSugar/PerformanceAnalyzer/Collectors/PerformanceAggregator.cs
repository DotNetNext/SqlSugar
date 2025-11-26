using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Collectors
{
    /// <summary>
    /// Aggregates performance metrics into statistics
    /// </summary>
    public class PerformanceAggregator
    {
        private readonly PerformanceAnalyzerConfig _config;
        
        public PerformanceAggregator(PerformanceAnalyzerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        public PerformanceStatistics Aggregate(List<QueryPerformanceMetric> metrics)
        {
            var stats = new PerformanceStatistics();
            
            if (metrics == null || !metrics.Any())
            {
                stats.EndTime = DateTime.UtcNow;
                return stats;
            }
            
            stats.Calculate(metrics);
            CalculateTableAccessFrequency(metrics, stats);
            
            return stats;
        }
        
        public PerformanceReport GenerateReport(List<QueryPerformanceMetric> metrics)
        {
            var report = new PerformanceReport
            {
                ReportTitle = $"Performance Report - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                Statistics = Aggregate(metrics)
            };
            
            report.GenerateSummary();
            
            return report;
        }
        
        private void CalculateTableAccessFrequency(List<QueryPerformanceMetric> metrics, PerformanceStatistics stats)
        {
            try
            {
                var tableFrequency = new Dictionary<string, int>();
                
                foreach (var metric in metrics)
                {
                    var tables = ExtractTableNames(metric.SqlText);
                    foreach (var table in tables)
                    {
                        if (!tableFrequency.ContainsKey(table))
                            tableFrequency[table] = 0;
                        tableFrequency[table]++;
                    }
                }
                
                stats.TableAccessFrequency = tableFrequency
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(20)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            catch
            {
                // Ignore errors in table extraction
            }
        }
        
        private List<string> ExtractTableNames(string sql)
        {
            var tables = new List<string>();
            
            if (string.IsNullOrEmpty(sql))
                return tables;
            
            try
            {
                // Simple pattern matching for table names after FROM and JOIN
                var pattern = @"(?:FROM|JOIN)\s+([\w\[\]\`\.]+)";
                var matches = System.Text.RegularExpressions.Regex.Matches(
                    sql, 
                    pattern, 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
                
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    if (match.Groups.Count > 1)
                    {
                        var tableName = match.Groups[1].Value
                            .Trim('[', ']', '`', ' ')
                            .Split('.').Last(); // Get table name without schema
                        
                        if (!string.IsNullOrEmpty(tableName))
                            tables.Add(tableName);
                    }
                }
            }
            catch
            {
                // Ignore parsing errors
            }
            
            return tables.Distinct().ToList();
        }
        
        public Dictionary<string, object> GetDashboardData(List<QueryPerformanceMetric> metrics)
        {
            var stats = Aggregate(metrics);
            
            return new Dictionary<string, object>
            {
                ["TotalQueries"] = stats.TotalQueries,
                ["QueriesPerSecond"] = stats.QueriesPerSecond,
                ["AverageResponseTime"] = stats.AverageExecutionTime,
                ["SlowQueryCount"] = stats.SlowQueryCount,
                ["ErrorRate"] = stats.TotalQueries > 0 
                    ? (double)stats.FailedQueryCount / stats.TotalQueries * 100 
                    : 0,
                ["P95ResponseTime"] = stats.P95ExecutionTime,
                ["TopSlowQueries"] = stats.TopSlowQueries.Take(5).Select(q => new
                {
                    q.SqlText,
                    q.ExecutionTimeMs,
                    q.RowCount
                }).ToList(),
                ["QueryTypeDistribution"] = stats.QueryTypeDistribution
            };
        }
    }
}