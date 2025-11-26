using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Analyzers
{
    /// <summary>
    /// Analyzes query complexity and suggests optimizations
    /// </summary>
    public class QueryComplexityAnalyzer : IQueryAnalyzer
    {
        private readonly PerformanceAnalyzerConfig _config;
        
        public QueryComplexityAnalyzer(PerformanceAnalyzerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        public List<OptimizationSuggestion> Analyze(List<QueryPerformanceMetric> metrics)
        {
            var suggestions = new List<OptimizationSuggestion>();
            
            foreach (var metric in metrics.Where(m => m.ComplexityScore > 50))
            {
                var suggestion = AnalyzeComplexity(metric);
                if (suggestion != null)
                {
                    suggestions.Add(suggestion);
                }
            }
            
            return suggestions;
        }
        
        private OptimizationSuggestion AnalyzeComplexity(QueryPerformanceMetric metric)
        {
            var issues = new List<string>();
            
            if (metric.JoinCount > 5)
            {
                issues.Add($"High number of JOINs ({metric.JoinCount})");
            }
            
            if (metric.SubqueryCount > 3)
            {
                issues.Add($"Multiple subqueries ({metric.SubqueryCount})");
            }
            
            if (HasCartesianProduct(metric.SqlText))
            {
                issues.Add("Potential Cartesian product detected");
            }
            
            if (HasSelectStar(metric.SqlText))
            {
                issues.Add("Using SELECT * - specify only needed columns");
            }
            
            if (!issues.Any())
                return null;
            
            return new OptimizationSuggestion
            {
                Type = OptimizationType.QueryRewrite,
                Title = "High Query Complexity",
                Description = $"Query has complexity score of {metric.ComplexityScore}/100. Issues: {string.Join(", ", issues)}",
                Recommendation = GenerateRecommendation(metric, issues),
                Severity = metric.ComplexityScore > 80 ? SeverityLevel.High : SeverityLevel.Medium,
                EstimatedImpactPercent = Math.Min(metric.ComplexityScore, 70),
                AffectedQuery = metric.NormalizedSql
            };
        }
        
        private bool HasCartesianProduct(string sql)
        {
            // Simple check: multiple FROM/JOIN without WHERE conditions
            var fromCount = Regex.Matches(sql, @"\bFROM\b|\bJOIN\b", RegexOptions.IgnoreCase).Count;
            var whereExists = Regex.IsMatch(sql, @"\bWHERE\b", RegexOptions.IgnoreCase);
            
            return fromCount > 1 && !whereExists;
        }
        
        private bool HasSelectStar(string sql)
        {
            return Regex.IsMatch(sql, @"SELECT\s+\*", RegexOptions.IgnoreCase);
        }
        
        private string GenerateRecommendation(QueryPerformanceMetric metric, List<string> issues)
        {
            var recommendations = new List<string>();
            
            if (metric.JoinCount > 5)
            {
                recommendations.Add("Consider breaking the query into smaller queries");
                recommendations.Add("Review if all JOINs are necessary");
            }
            
            if (metric.SubqueryCount > 3)
            {
                recommendations.Add("Consider using CTEs (WITH clause) for better readability");
                recommendations.Add("Some subqueries might be replaceable with JOINs");
            }
            
            if (HasSelectStar(metric.SqlText))
            {
                recommendations.Add("Specify only the columns you need instead of SELECT *");
            }
            
            return string.Join(". ", recommendations);
        }
    }
}