using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Analyzers
{
    /// <summary>
    /// Analyzes queries and recommends missing indexes
    /// </summary>
    public class IndexAnalyzer : IQueryAnalyzer
    {
        private readonly PerformanceAnalyzerConfig _config;
        private readonly DbType _dbType;
        
        public IndexAnalyzer(PerformanceAnalyzerConfig config, DbType dbType)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _dbType = dbType;
        }
        
        public List<OptimizationSuggestion> Analyze(List<QueryPerformanceMetric> metrics)
        {
            var suggestions = new List<OptimizationSuggestion>();
            var recommendations = AnalyzeQueries(metrics);
            
            foreach (var rec in recommendations)
            {
                suggestions.Add(new OptimizationSuggestion
                {
                    Type = OptimizationType.MissingIndex,
                    Title = $"Missing Index on {rec.TableName}",
                    Description = rec.Reason,
                    Recommendation = $"Create index on columns: {string.Join(", ", rec.Columns)}",
                    SuggestedSql = rec.CreateIndexSql,
                    Severity = rec.EstimatedImpactPercent > 50 ? SeverityLevel.High : SeverityLevel.Medium,
                    EstimatedImpactPercent = rec.EstimatedImpactPercent,
                    AffectedTables = new List<string> { rec.TableName }
                });
            }
            
            return suggestions;
        }
        
        public List<IndexRecommendation> AnalyzeQueries(List<QueryPerformanceMetric> metrics)
        {
            if (!_config.EnableIndexAnalysis || metrics.Count < _config.MinQueriesForAnalysis)
            {
                return new List<IndexRecommendation>();
            }
            
            var recommendations = new List<IndexRecommendation>();
            var slowQueries = metrics.Where(m => m.IsSlowQuery && m.QueryType == QueryType.Select).ToList();
            
            var queryGroups = slowQueries
                .GroupBy(m => ExtractTableName(m.SqlText))
                .Where(g => !string.IsNullOrEmpty(g.Key));
            
            foreach (var group in queryGroups)
            {
                var tableName = group.Key;
                var tableQueries = group.ToList();
                
                var whereColumns = ExtractWhereColumns(tableQueries);
                if (whereColumns.Any())
                {
                    var rec = new IndexRecommendation
                    {
                        TableName = tableName,
                        Columns = whereColumns.Take(3).ToList(), // Limit to 3 columns
                        Reason = $"Frequently used in WHERE clauses across {tableQueries.Count} slow queries",
                        QueryCount = tableQueries.Count,
                        TotalQueryTime = tableQueries.Sum(q => q.ExecutionTimeMs),
                        EstimatedImpactPercent = CalculateImpact(tableQueries)
                    };
                    
                    rec.GenerateCreateIndexSql(_dbType);
                    recommendations.Add(rec);
                }
            }
            
            return recommendations.OrderByDescending(r => r.EstimatedImpactPercent).ToList();
        }
        
        private string ExtractTableName(string sql)
        {
            try
            {
                var match = Regex.Match(sql, @"FROM\s+([\w\[\]\`]+)", RegexOptions.IgnoreCase);
                return match.Success ? match.Groups[1].Value.Trim('[', ']', '`', ' ') : null;
            }
            catch
            {
                return null;
            }
        }
        
        private List<string> ExtractWhereColumns(List<QueryPerformanceMetric> queries)
        {
            var columnFrequency = new Dictionary<string, int>();
            
            foreach (var query in queries)
            {
                var columns = ExtractWhereColumnsFromSql(query.SqlText);
                foreach (var col in columns)
                {
                    if (!columnFrequency.ContainsKey(col))
                        columnFrequency[col] = 0;
                    columnFrequency[col]++;
                }
            }
            
            return columnFrequency
                .OrderByDescending(kvp => kvp.Value)
                .Take(3)
                .Select(kvp => kvp.Key)
                .ToList();
        }
        
        private List<string> ExtractWhereColumnsFromSql(string sql)
        {
            var columns = new List<string>();
            
            try
            {
                // Extract WHERE clause
                var whereMatch = Regex.Match(sql, @"WHERE\s+(.+?)(?:GROUP BY|ORDER BY|$)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (!whereMatch.Success)
                    return columns;
                
                var whereClause = whereMatch.Groups[1].Value;
                
                // Extract column names (simplified pattern)
                var columnMatches = Regex.Matches(whereClause, @"(\w+)\s*(?:=|>|<|>=|<=|!=|LIKE|IN)", RegexOptions.IgnoreCase);
                
                foreach (Match match in columnMatches)
                {
                    var columnName = match.Groups[1].Value;
                    if (!IsReservedWord(columnName))
                    {
                        columns.Add(columnName);
                    }
                }
            }
            catch
            {
                // Ignore parsing errors
            }
            
            return columns.Distinct().ToList();
        }
        
        private bool IsReservedWord(string word)
        {
            var reserved = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "AND", "OR", "NOT", "SELECT", "FROM", "WHERE", "ORDER", "BY", "GROUP"
            };
            return reserved.Contains(word);
        }
        
        private int CalculateImpact(List<QueryPerformanceMetric> queries)
        {
            var avgTime = queries.Average(q => q.ExecutionTimeMs);
            
            if (avgTime > 5000)
                return 80;
            if (avgTime > 2000)
                return 60;
            if (avgTime > 1000)
                return 40;
            return 20;
        }
    }
}