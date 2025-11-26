using System;
using System.Collections.Generic;

namespace SqlSugar.PerformanceAnalyzer.Models
{
    /// <summary>
    /// Represents an optimization suggestion for improving query performance
    /// </summary>
    public class OptimizationSuggestion
    {
        public Guid Id { get; set; }
        public OptimizationType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Recommendation { get; set; }
        public string SuggestedSql { get; set; }
        public SeverityLevel Severity { get; set; }
        public int EstimatedImpactPercent { get; set; }
        public string AffectedQuery { get; set; }
        public List<string> AffectedTables { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public OptimizationSuggestion()
        {
            Id = Guid.NewGuid();
            AffectedTables = new List<string>();
            Metadata = new Dictionary<string, object>();
            CreatedAt = DateTime.UtcNow;
        }
        
        public string GetImpactDescription()
        {
            if (EstimatedImpactPercent >= 70)
                return "Very High Impact";
            if (EstimatedImpactPercent >= 50)
                return "High Impact";
            if (EstimatedImpactPercent >= 30)
                return "Medium Impact";
            return "Low Impact";
        }
    }
}