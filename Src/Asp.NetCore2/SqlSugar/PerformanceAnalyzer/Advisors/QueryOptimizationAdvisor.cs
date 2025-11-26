using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar.PerformanceAnalyzer.Analyzers;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Advisors
{
    /// <summary>
    /// Provides comprehensive query optimization advice
    /// </summary>
    public class QueryOptimizationAdvisor : IOptimizationAdvisor
    {
        private readonly PerformanceAnalyzerConfig _config;
        private readonly DbType _dbType;
        private readonly List<IQueryAnalyzer> _analyzers;
        
        public QueryOptimizationAdvisor(PerformanceAnalyzerConfig config, DbType dbType)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _dbType = dbType;
            
            // Initialize all analyzers
            _analyzers = new List<IQueryAnalyzer>
            {
                new NPlusOneDetector(config),
                new IndexAnalyzer(config, dbType),
                new QueryComplexityAnalyzer(config),
                new QueryPatternAnalyzer(config)
            };
        }
        
        public List<OptimizationSuggestion> GetSuggestions(List<QueryPerformanceMetric> metrics)
        {
            var allSuggestions = new List<OptimizationSuggestion>();
            
            if (metrics == null || !metrics.Any())
                return allSuggestions;
            
            // Run all analyzers
            foreach (var analyzer in _analyzers)
            {
                try
                {
                    var suggestions = analyzer.Analyze(metrics);
                    if (suggestions != null && suggestions.Any())
                    {
                        allSuggestions.AddRange(suggestions);
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue with other analyzers
                    Console.WriteLine($"Analyzer error: {ex.Message}");
                }
            }
            
            // Deduplicate and prioritize
            return PrioritizeSuggestions(allSuggestions);
        }
        
        private List<OptimizationSuggestion> PrioritizeSuggestions(List<OptimizationSuggestion> suggestions)
        {
            // Remove duplicates based on affected query
            var unique = suggestions
                .GroupBy(s => s.AffectedQuery ?? s.Description)
                .Select(g => g.OrderByDescending(s => s.EstimatedImpactPercent).First())
                .ToList();
            
            // Sort by priority: Severity > Impact > Type
            return unique
                .OrderByDescending(s => s.Severity)
                .ThenByDescending(s => s.EstimatedImpactPercent)
                .ThenBy(s => s.Type)
                .ToList();
        }
    }
}