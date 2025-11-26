using System;
using System.Collections.Generic;

namespace SqlSugar.PerformanceAnalyzer.Models
{
    /// <summary>
    /// Represents a detected N+1 query issue
    /// </summary>
    public class NPlusOneIssue
    {
        public Guid Id { get; set; }
        public string ParentQuery { get; set; }
        public string ChildQuery { get; set; }
        public int ExecutionCount { get; set; }
        public double TotalExecutionTime { get; set; }
        public double AverageExecutionTime { get; set; }
        public List<string> AffectedTables { get; set; }
        public string OptimizationSuggestion { get; set; }
        public string SuggestedIncludeQuery { get; set; }
        public int EstimatedSavingsPercent { get; set; }
        public DateTime DetectedAt { get; set; }
        
        public NPlusOneIssue()
        {
            Id = Guid.NewGuid();
            AffectedTables = new List<string>();
            DetectedAt = DateTime.UtcNow;
        }
    }
}