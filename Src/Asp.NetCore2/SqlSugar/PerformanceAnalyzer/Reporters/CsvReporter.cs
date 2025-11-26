using System;
using System.Linq;
using System.Text;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Reporters
{
    /// <summary>
    /// Generates CSV performance reports
    /// </summary>
    public class CsvReporter : IPerformanceReporter
    {
        public string Generate(PerformanceReport report)
        {
            var csv = new StringBuilder();
            
            // Summary Section
            csv.AppendLine("# Performance Report Summary");
            csv.AppendLine($"Generated At,{report.GeneratedAt:yyyy-MM-dd HH:mm:ss}");
            csv.AppendLine($"Total Queries,{report.Statistics.TotalQueries}");
            csv.AppendLine($"Slow Queries,{report.Statistics.SlowQueryCount}");
            csv.AppendLine($"Failed Queries,{report.Statistics.FailedQueryCount}");
            csv.AppendLine($"Average Execution Time (ms),{report.Statistics.AverageExecutionTime:F2}");
            csv.AppendLine($"P95 Execution Time (ms),{report.Statistics.P95ExecutionTime:F2}");
            csv.AppendLine($"P99 Execution Time (ms),{report.Statistics.P99ExecutionTime:F2}");
            csv.AppendLine();
            
            // Top Slow Queries
            csv.AppendLine("# Top Slow Queries");
            csv.AppendLine("Rank,SQL,Execution Time (ms),Row Count");
            int rank = 1;
            foreach (var query in report.Statistics.TopSlowQueries.Take(10))
            {
                csv.AppendLine($"{rank++},\"{EscapeCsv(query.SqlText)}\",{query.ExecutionTimeMs:F2},{query.RowCount}");
            }
            csv.AppendLine();
            
            // Optimization Suggestions
            csv.AppendLine("# Optimization Suggestions");
            csv.AppendLine("Type,Title,Severity,Impact %,Description,Recommendation");
            foreach (var suggestion in report.Suggestions)
            {
                csv.AppendLine($"{suggestion.Type},{suggestion.Title},{suggestion.Severity},{suggestion.EstimatedImpactPercent},\"{EscapeCsv(suggestion.Description)}\",\"{EscapeCsv(suggestion.Recommendation)}\"");
            }
            csv.AppendLine();
            
            // Index Recommendations
            csv.AppendLine("# Index Recommendations");
            csv.AppendLine("Table,Columns,Impact %,CREATE INDEX SQL");
            foreach (var rec in report.IndexRecommendations)
            {
                csv.AppendLine($"{rec.TableName},\"{string.Join(", ", rec.Columns)}\",{rec.EstimatedImpactPercent},\"{EscapeCsv(rec.CreateIndexSql)}\"");
            }
            
            return csv.ToString();
        }
        
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
                
            return value.Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", "");
        }
    }
}