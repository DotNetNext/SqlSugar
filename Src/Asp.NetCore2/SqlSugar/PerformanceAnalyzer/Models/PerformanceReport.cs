using System;
using System.Collections.Generic;

namespace SqlSugar.PerformanceAnalyzer.Models
{
    /// <summary>
    /// Complete performance report with statistics and recommendations
    /// </summary>
    public class PerformanceReport
    {
        public Guid Id { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string ReportTitle { get; set; }
        public PerformanceStatistics Statistics { get; set; }
        public List<OptimizationSuggestion> Suggestions { get; set; }
        public List<NPlusOneIssue> NPlusOneIssues { get; set; }
        public List<IndexRecommendation> IndexRecommendations { get; set; }
        public string Summary { get; set; }
        
        public PerformanceReport()
        {
            Id = Guid.NewGuid();
            GeneratedAt = DateTime.UtcNow;
            Statistics = new PerformanceStatistics();
            Suggestions = new List<OptimizationSuggestion>();
            NPlusOneIssues = new List<NPlusOneIssue>();
            IndexRecommendations = new List<IndexRecommendation>();
        }
        
        public void GenerateSummary()
        {
            var summary = $"Performance Report - {GeneratedAt:yyyy-MM-dd HH:mm:ss}\n";
            summary += $"Total Queries: {Statistics.TotalQueries}\n";
            summary += $"Slow Queries: {Statistics.SlowQueryCount} ({GetPercentage(Statistics.SlowQueryCount, Statistics.TotalQueries)}%)\n";
            summary += $"Average Execution Time: {Statistics.AverageExecutionTime:F2}ms\n";
            summary += $"P95 Execution Time: {Statistics.P95ExecutionTime:F2}ms\n";
            summary += $"Optimization Suggestions: {Suggestions.Count}\n";
            summary += $"N+1 Issues Detected: {NPlusOneIssues.Count}\n";
            summary += $"Index Recommendations: {IndexRecommendations.Count}\n";
            
            Summary = summary;
        }
        
        private double GetPercentage(int part, int total)
        {
            return total > 0 ? Math.Round((double)part / total * 100, 2) : 0;
        }
    }
}