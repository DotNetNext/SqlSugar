using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Analyzers
{
    /// <summary>
    /// Detects N+1 query patterns
    /// </summary>
    public class NPlusOneDetector : IQueryAnalyzer
    {
        private readonly PerformanceAnalyzerConfig _config;
        
        public NPlusOneDetector(PerformanceAnalyzerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        public List<OptimizationSuggestion> Analyze(List<QueryPerformanceMetric> metrics)
        {
            var suggestions = new List<OptimizationSuggestion>();
            var issues = Detect(metrics);
            
            foreach (var issue in issues)
            {
                suggestions.Add(new OptimizationSuggestion
                {
                    Type = OptimizationType.NPlusOneQuery,
                    Title = "N+1 Query Pattern Detected",
                    Description = issue.OptimizationSuggestion,
                    Recommendation = issue.SuggestedIncludeQuery,
                    Severity = SeverityLevel.High,
                    EstimatedImpactPercent = issue.EstimatedSavingsPercent,
                    AffectedQuery = issue.ChildQuery,
                    AffectedTables = issue.AffectedTables
                });
            }
            
            return suggestions;
        }
        
        public List<NPlusOneIssue> Detect(List<QueryPerformanceMetric> metrics)
        {
            if (!_config.EnableNPlusOneDetection || metrics.Count < _config.MinQueriesForAnalysis)
            {
                return new List<NPlusOneIssue>();
            }
            
            var issues = new List<NPlusOneIssue>();
            var queryGroups = metrics
                .Where(m => m.QueryType == QueryType.Select)
                .GroupBy(m => m.NormalizedSql)
                .Where(g => g.Count() >= _config.NPlusOneThreshold)
                .ToList();
            
            foreach (var group in queryGroups)
            {
                var pattern = DetectPattern(group.ToList());
                if (pattern != null)
                {
                    issues.Add(pattern);
                }
            }
            
            return issues;
        }
        
        private NPlusOneIssue DetectPattern(List<QueryPerformanceMetric> similarQueries)
        {
            if (similarQueries.Count < _config.NPlusOneThreshold) 
                return null;
            
            // Check if queries are executed in rapid succession (potential N+1)
            var timeSpan = similarQueries.Max(q => q.StartTime) - similarQueries.Min(q => q.StartTime);
            if (timeSpan.TotalSeconds > 10) 
                return null; // Not N+1 if spread over time
            
            var firstQuery = similarQueries.First();
            var tableName = ExtractTableName(firstQuery.SqlText);
            
            if (string.IsNullOrEmpty(tableName)) 
                return null;
            
            return new NPlusOneIssue
            {
                ParentQuery = "SELECT FROM parent table",
                ChildQuery = firstQuery.NormalizedSql,
                ExecutionCount = similarQueries.Count,
                TotalExecutionTime = similarQueries.Sum(q => q.ExecutionTimeMs),
                AverageExecutionTime = similarQueries.Average(q => q.ExecutionTimeMs),
                AffectedTables = new List<string> { tableName },
                OptimizationSuggestion = GenerateOptimizationSuggestion(tableName, similarQueries.Count),
                SuggestedIncludeQuery = GenerateIncludeQuery(tableName),
                EstimatedSavingsPercent = CalculateEstimatedSavings(similarQueries.Count)
            };
        }
        
        private string ExtractTableName(string sql)
        {
            try
            {
                var match = Regex.Match(sql, @"FROM\s+([\w\[\]\`]+)", RegexOptions.IgnoreCase);
                return match.Success ? match.Groups[1].Value.Trim('[', ']', '`') : null;
            }
            catch
            {
                return null;
            }
        }
        
        private string GenerateOptimizationSuggestion(string tableName, int count)
        {
            return $"N+1 query detected: {count} similar queries to {tableName}. " +
                   $"Consider using .Includes() or eager loading to reduce database round trips from {count} to 1.";
        }
        
        private string GenerateIncludeQuery(string tableName)
        {
            return $".Includes(x => x.{tableName})";
        }
        
        private int CalculateEstimatedSavings(int queryCount)
        {
            return Math.Min((int)((queryCount - 1) / (double)queryCount * 100), 95);
        }
    }
}