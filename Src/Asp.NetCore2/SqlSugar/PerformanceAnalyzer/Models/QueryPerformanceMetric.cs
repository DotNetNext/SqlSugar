using System;
using System.Collections.Generic;

namespace SqlSugar.PerformanceAnalyzer.Models
{
    /// <summary>
    /// Represents performance metrics for a single query execution
    /// </summary>
    public class QueryPerformanceMetric
    {
        public Guid Id { get; set; }
        public string SqlText { get; set; }
        public string NormalizedSql { get; set; }
        public List<SugarParameter> Parameters { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double ExecutionTimeMs { get; set; }
        public int RowCount { get; set; }
        public QueryType QueryType { get; set; }
        public DbType DbType { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public int ThreadId { get; set; }
        public long MemoryBefore { get; set; }
        public long MemoryAfter { get; set; }
        public long MemoryDelta => MemoryAfter - MemoryBefore;
        public Dictionary<string, string> Tags { get; set; }
        public bool IsSlowQuery { get; set; }
        public int ComplexityScore { get; set; }
        public int TableCount { get; set; }
        public int JoinCount { get; set; }
        public int SubqueryCount { get; set; }
        public bool? UsesIndexes { get; set; }
        public long? EstimatedRowsScanned { get; set; }
        
        public QueryPerformanceMetric()
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            Parameters = new List<SugarParameter>();
            Tags = new Dictionary<string, string>();
        }
    }
}