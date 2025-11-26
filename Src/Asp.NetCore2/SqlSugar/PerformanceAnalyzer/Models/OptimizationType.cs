using System;

namespace SqlSugar.PerformanceAnalyzer.Models
{
    public enum OptimizationType
    {
        MissingIndex,
        NPlusOneQuery,
        FullTableScan,
        InefficientJoin,
        SubqueryOptimization,
        CachingOpportunity,
        BatchOperation,
        QueryRewrite,
        Other
    }
    
    public enum SeverityLevel
    {
        Low,
        Medium,
        High,
        Critical
    }
}