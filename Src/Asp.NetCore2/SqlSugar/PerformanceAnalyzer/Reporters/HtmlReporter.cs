using System;
using System.Linq;
using System.Text;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Reporters
{
    /// <summary>
    /// Generates HTML performance reports
    /// </summary>
    public class HtmlReporter : IPerformanceReporter
    {
        public string Generate(PerformanceReport report)
        {
            var html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='en'>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine($"    <title>{report.ReportTitle}</title>");
            html.AppendLine("    <style>");
            html.AppendLine(GetStyles());
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            html.AppendLine($"    <h1>{report.ReportTitle}</h1>");
            html.AppendLine($"    <p class='timestamp'>Generated at: {report.GeneratedAt:yyyy-MM-dd HH:mm:ss}</p>");
            
            // Summary Section
            html.AppendLine("    <div class='section'>");
            html.AppendLine("        <h2>Summary</h2>");
            html.AppendLine(GenerateSummaryHtml(report.Statistics));
            html.AppendLine("    </div>");
            
            // Top Slow Queries
            if (report.Statistics.TopSlowQueries.Any())
            {
                html.AppendLine("    <div class='section'>");
                html.AppendLine("        <h2>Top 10 Slowest Queries</h2>");
                html.AppendLine(GenerateSlowQueriesTable(report.Statistics.TopSlowQueries));
                html.AppendLine("    </div>");
            }
            
            // Optimization Suggestions
            if (report.Suggestions.Any())
            {
                html.AppendLine("    <div class='section'>");
                html.AppendLine("        <h2>Optimization Suggestions</h2>");
                html.AppendLine(GenerateSuggestionsHtml(report.Suggestions));
                html.AppendLine("    </div>");
            }
            
            // N+1 Issues
            if (report.NPlusOneIssues.Any())
            {
                html.AppendLine("    <div class='section'>");
                html.AppendLine("        <h2>N+1 Query Issues</h2>");
                html.AppendLine(GenerateNPlusOneHtml(report.NPlusOneIssues));
                html.AppendLine("    </div>");
            }
            
            // Index Recommendations
            if (report.IndexRecommendations.Any())
            {
                html.AppendLine("    <div class='section'>");
                html.AppendLine("        <h2>Index Recommendations</h2>");
                html.AppendLine(GenerateIndexRecommendationsHtml(report.IndexRecommendations));
                html.AppendLine("    </div>");
            }
            
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return html.ToString();
        }
        
        private string GetStyles()
        {
            return @"
                body { font-family: Arial, sans-serif; margin: 20px; background-color: #f5f5f5; }
                h1 { color: #333; }
                h2 { color: #555; border-bottom: 2px solid #ddd; padding-bottom: 5px; }
                .timestamp { color: #777; font-size: 14px; }
                .section { background: white; padding: 20px; margin: 20px 0; border-radius: 5px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
                .metric { display: inline-block; margin: 10px 20px 10px 0; padding: 10px; background: #f0f0f0; border-radius: 3px; }
                .metric-label { font-weight: bold; color: #555; }
                .metric-value { font-size: 20px; color: #333; }
                table { width: 100%; border-collapse: collapse; margin-top: 10px; }
                th { background-color: #4CAF50; color: white; padding: 10px; text-align: left; }
                td { padding: 8px; border-bottom: 1px solid #ddd; }
                tr:hover { background-color: #f5f5f5; }
                .severity-high { color: #d32f2f; font-weight: bold; }
                .severity-medium { color: #f57c00; }
                .severity-low { color: #388e3c; }
                .sql-code { font-family: 'Courier New', monospace; background: #f5f5f5; padding: 5px; border-radius: 3px; font-size: 12px; }
            ";
        }
        
        private string GenerateSummaryHtml(PerformanceStatistics stats)
        {
            return $@"
                <div class='metric'>
                    <div class='metric-label'>Total Queries</div>
                    <div class='metric-value'>{stats.TotalQueries}</div>
                </div>
                <div class='metric'>
                    <div class='metric-label'>Slow Queries</div>
                    <div class='metric-value'>{stats.SlowQueryCount}</div>
                </div>
                <div class='metric'>
                    <div class='metric-label'>Avg Time</div>
                    <div class='metric-value'>{stats.AverageExecutionTime:F2}ms</div>
                </div>
                <div class='metric'>
                    <div class='metric-label'>P95 Time</div>
                    <div class='metric-value'>{stats.P95ExecutionTime:F2}ms</div>
                </div>
                <div class='metric'>
                    <div class='metric-label'>P99 Time</div>
                    <div class='metric-value'>{stats.P99ExecutionTime:F2}ms</div>
                </div>
                <div class='metric'>
                    <div class='metric-label'>Failed Queries</div>
                    <div class='metric-value'>{stats.FailedQueryCount}</div>
                </div>
            ";
        }
        
        private string GenerateSlowQueriesTable(System.Collections.Generic.List<QueryPerformanceMetric> queries)
        {
            var html = new StringBuilder();
            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Rank</th><th>SQL</th><th>Time (ms)</th><th>Rows</th></tr>");
            
            int rank = 1;
            foreach (var query in queries.Take(10))
            {
                var sql = query.SqlText?.Length > 100 ? query.SqlText.Substring(0, 100) + "..." : query.SqlText;
                html.AppendLine($"<tr><td>{rank++}</td><td class='sql-code'>{System.Web.HttpUtility.HtmlEncode(sql)}</td><td>{query.ExecutionTimeMs:F2}</td><td>{query.RowCount}</td></tr>");
            }
            
            html.AppendLine("</table>");
            return html.ToString();
        }
        
        private string GenerateSuggestionsHtml(System.Collections.Generic.List<OptimizationSuggestion> suggestions)
        {
            var html = new StringBuilder();
            foreach (var suggestion in suggestions.Take(20))
            {
                var severityClass = suggestion.Severity == SeverityLevel.High ? "severity-high" : 
                                  suggestion.Severity == SeverityLevel.Medium ? "severity-medium" : "severity-low";
                
                html.AppendLine($"<div style='margin: 15px 0; padding: 10px; background: #f9f9f9; border-left: 4px solid #4CAF50;'>");
                html.AppendLine($"<h4>{suggestion.Title} <span class='{severityClass}'>({suggestion.Severity})</span></h4>");
                html.AppendLine($"<p><strong>Impact:</strong> {suggestion.EstimatedImpactPercent}%</p>");
                html.AppendLine($"<p>{suggestion.Description}</p>");
                html.AppendLine($"<p><strong>Recommendation:</strong> {suggestion.Recommendation}</p>");
                if (!string.IsNullOrEmpty(suggestion.SuggestedSql))
                {
                    html.AppendLine($"<pre class='sql-code'>{System.Web.HttpUtility.HtmlEncode(suggestion.SuggestedSql)}</pre>");
                }
                html.AppendLine("</div>");
            }
            return html.ToString();
        }
        
        private string GenerateNPlusOneHtml(System.Collections.Generic.List<NPlusOneIssue> issues)
        {
            var html = new StringBuilder();
            foreach (var issue in issues)
            {
                html.AppendLine($"<div style='margin: 15px 0; padding: 10px; background: #ffebee; border-left: 4px solid #f44336;'>");
                html.AppendLine($"<h4>N+1 Pattern Detected</h4>");
                html.AppendLine($"<p><strong>Execution Count:</strong> {issue.ExecutionCount}</p>");
                html.AppendLine($"<p><strong>Total Time:</strong> {issue.TotalExecutionTime:F2}ms</p>");
                html.AppendLine($"<p><strong>Suggestion:</strong> {issue.OptimizationSuggestion}</p>");
                html.AppendLine($"<p><strong>Recommended Fix:</strong> <code>{issue.SuggestedIncludeQuery}</code></p>");
                html.AppendLine("</div>");
            }
            return html.ToString();
        }
        
        private string GenerateIndexRecommendationsHtml(System.Collections.Generic.List<IndexRecommendation> recommendations)
        {
            var html = new StringBuilder();
            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Table</th><th>Columns</th><th>Impact</th><th>CREATE INDEX SQL</th></tr>");
            
            foreach (var rec in recommendations)
            {
                html.AppendLine($"<tr>");
                html.AppendLine($"<td>{rec.TableName}</td>");
                html.AppendLine($"<td>{string.Join(", ", rec.Columns)}</td>");
                html.AppendLine($"<td>{rec.EstimatedImpactPercent}%</td>");
                html.AppendLine($"<td class='sql-code'>{System.Web.HttpUtility.HtmlEncode(rec.CreateIndexSql)}</td>");
                html.AppendLine("</tr>");
            }
            
            html.AppendLine("</table>");
            return html.ToString();
        }
    }
}