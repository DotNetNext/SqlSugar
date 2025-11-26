using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar.PerformanceAnalyzer.Analyzers;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Advisors
{
    /// <summary>
    /// Specialized advisor for index recommendations
    /// </summary>
    public class IndexAdvisor : IOptimizationAdvisor
    {
        private readonly PerformanceAnalyzerConfig _config;
        private readonly DbType _dbType;
        
        public IndexAdvisor(PerformanceAnalyzerConfig config, DbType dbType)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _dbType = dbType;
        }
        
        public List<OptimizationSuggestion> GetSuggestions(List<QueryPerformanceMetric> metrics)
        {
            var analyzer = new IndexAnalyzer(_config, _dbType);
            var recommendations = analyzer.AnalyzeQueries(metrics);
            
            return ConvertToSuggestions(recommendations);
        }
        
        public List<IndexRecommendation> GetIndexRecommendations(List<QueryPerformanceMetric> metrics)
        {
            var analyzer = new IndexAnalyzer(_config, _dbType);
            return analyzer.AnalyzeQueries(metrics);
        }
        
        private List<OptimizationSuggestion> ConvertToSuggestions(List<IndexRecommendation> recommendations)
        {
            return recommendations.Select(rec => new OptimizationSuggestion
            {
                Type = OptimizationType.MissingIndex,
                Title = $"Index Recommendation for {rec.TableName}",
                Description = rec.Reason,
                Recommendation = $"Create an index on columns: {string.Join(", ", rec.Columns)}",
                SuggestedSql = rec.CreateIndexSql,
                Severity = rec.EstimatedImpactPercent > 60 ? SeverityLevel.High : SeverityLevel.Medium,
                EstimatedImpactPercent = rec.EstimatedImpactPercent,
                AffectedTables = new List<string> { rec.TableName }
            }).ToList();
        }
    }
}