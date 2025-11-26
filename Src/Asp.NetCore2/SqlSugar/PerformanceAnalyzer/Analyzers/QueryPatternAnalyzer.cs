using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Analyzers
{
    /// <summary>
    /// Analyzes query patterns for optimization opportunities
    /// </summary>
    public class QueryPatternAnalyzer : IQueryAnalyzer
    {
        private readonly PerformanceAnalyzerConfig _config;
        
        public QueryPatternAnalyzer(PerformanceAnalyzerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        public List<OptimizationSuggestion> Analyze(List<QueryPerformanceMetric> metrics)
        {
            var suggestions = new List<OptimizationSuggestion>();
            
            // Detect repeated queries (caching opportunity)
            var cachingSuggestions = DetectCachingOpportunities(metrics);
            suggestions.AddRange(cachingSuggestions);
            
            // Detect batch operation opportunities
            var batchSuggestions = DetectBatchOpportunities(metrics);
            suggestions.AddRange(batchSuggestions);
            
            return suggestions;
        }
        
        private List<OptimizationSuggestion> DetectCachingOpportunities(List<QueryPerformanceMetric> metrics)
        {
            var suggestions = new List<OptimizationSuggestion>();
            
            var repeatedQueries = metrics
                .GroupBy(m => m.NormalizedSql)
                .Where(g => g.Count() > 10)
                .Select(g => new
                {
                    Query = g.Key,
                    Count = g.Count(),
                    AvgTime = g.Average(m => m.ExecutionTimeMs),
                    TotalTime = g.Sum(m => m.ExecutionTimeMs)
                })
                .OrderByDescending(x => x.TotalTime)
                .Take(5);
            
            foreach (var query in repeatedQueries)
            {
                suggestions.Add(new OptimizationSuggestion
                {
                    Type = OptimizationType.CachingOpportunity,
                    Title = "Caching Opportunity Detected",
                    Description = $"Query executed {query.Count} times with average time {query.AvgTime:F2}ms",
                    Recommendation = "Consider caching the results of this frequently executed query",
                    Severity = query.Count > 50 ? SeverityLevel.High : SeverityLevel.Medium,
                    EstimatedImpactPercent = Math.Min((int)(query.Count * 0.8), 90),
                    AffectedQuery = query.Query
                });
            }
            
            return suggestions;
        }
        
        private List<OptimizationSuggestion> DetectBatchOpportunities(List<QueryPerformanceMetric> metrics)
        {
            var suggestions = new List<OptimizationSuggestion>();
            
            // Detect multiple INSERTs/UPDATEs in short time span
            var writes = metrics
                .Where(m => m.QueryType == QueryType.Insert || m.QueryType == QueryType.Update)
                .OrderBy(m => m.StartTime)
                .ToList();
            
            for (int i = 0; i < writes.Count - 5; i++)
            {
                var batch = writes.Skip(i).Take(10).ToList();
                var timeSpan = (batch.Last().StartTime - batch.First().StartTime).TotalSeconds;
                
                if (timeSpan < 1.0) // Within 1 second
                {
                    suggestions.Add(new OptimizationSuggestion
                    {
                        Type = OptimizationType.BatchOperation,
                        Title = "Batch Operation Opportunity",
                        Description = $"{batch.Count} {batch.First().QueryType} operations within {timeSpan:F2} seconds",
                        Recommendation = "Consider using batch insert/update methods like BulkCopy() or BulkUpdate()",
                        Severity = SeverityLevel.Medium,
                        EstimatedImpactPercent = 50,
                        AffectedQuery = batch.First().NormalizedSql
                    });
                    
                    break; // Only report once
                }
            }
            
            return suggestions;
        }
    }
}