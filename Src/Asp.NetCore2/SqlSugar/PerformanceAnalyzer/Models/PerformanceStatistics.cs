using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.PerformanceAnalyzer.Models
{
    /// <summary>
    /// Aggregated performance statistics
    /// </summary>
    public class PerformanceStatistics
    {
        public int TotalQueries { get; set; }
        public int SlowQueryCount { get; set; }
        public int FailedQueryCount { get; set; }
        public double AverageExecutionTime { get; set; }
        public double MinExecutionTime { get; set; }
        public double MaxExecutionTime { get; set; }
        public double P50ExecutionTime { get; set; }
        public double P95ExecutionTime { get; set; }
        public double P99ExecutionTime { get; set; }
        public double TotalExecutionTime { get; set; }
        public Dictionary<QueryType, int> QueryTypeDistribution { get; set; }
        public Dictionary<string, int> TableAccessFrequency { get; set; }
        public List<QueryPerformanceMetric> TopSlowQueries { get; set; }
        public List<QueryPerformanceMetric> MostFrequentQueries { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        public double QueriesPerSecond => Duration.TotalSeconds > 0 ? TotalQueries / Duration.TotalSeconds : 0;
        
        public PerformanceStatistics()
        {
            QueryTypeDistribution = new Dictionary<QueryType, int>();
            TableAccessFrequency = new Dictionary<string, int>();
            TopSlowQueries = new List<QueryPerformanceMetric>();
            MostFrequentQueries = new List<QueryPerformanceMetric>();
            StartTime = DateTime.UtcNow;
        }
        
        public void Calculate(List<QueryPerformanceMetric> metrics)
        {
            if (metrics == null || !metrics.Any())
            {
                EndTime = DateTime.UtcNow;
                return;
            }
            
            TotalQueries = metrics.Count;
            FailedQueryCount = metrics.Count(m => !m.IsSuccess);
            SlowQueryCount = metrics.Count(m => m.IsSlowQuery);
            
            var times = metrics.Select(m => m.ExecutionTimeMs).OrderBy(t => t).ToList();
            AverageExecutionTime = times.Average();
            MinExecutionTime = times.First();
            MaxExecutionTime = times.Last();
            TotalExecutionTime = times.Sum();
            
            P50ExecutionTime = GetPercentile(times, 0.50);
            P95ExecutionTime = GetPercentile(times, 0.95);
            P99ExecutionTime = GetPercentile(times, 0.99);
            
            QueryTypeDistribution = metrics
                .GroupBy(m => m.QueryType)
                .ToDictionary(g => g.Key, g => g.Count());
                
            TopSlowQueries = metrics
                .OrderByDescending(m => m.ExecutionTimeMs)
                .Take(10)
                .ToList();
                
            MostFrequentQueries = metrics
                .GroupBy(m => m.NormalizedSql)
                .Select(g => new 
                { 
                    Query = g.First(), 
                    Count = g.Count() 
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .Select(x => x.Query)
                .ToList();
                
            EndTime = DateTime.UtcNow;
        }
        
        private double GetPercentile(List<double> sortedData, double percentile)
        {
            if (sortedData == null || !sortedData.Any())
                return 0;
                
            int index = (int)Math.Ceiling(percentile * sortedData.Count) - 1;
            return sortedData[Math.Max(0, Math.Min(index, sortedData.Count - 1))];
        }
    }
}