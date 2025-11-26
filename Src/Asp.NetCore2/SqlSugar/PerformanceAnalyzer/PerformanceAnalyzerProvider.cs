using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar.PerformanceAnalyzer.Advisors;
using SqlSugar.PerformanceAnalyzer.Analyzers;
using SqlSugar.PerformanceAnalyzer.Collectors;
using SqlSugar.PerformanceAnalyzer.Export;
using SqlSugar.PerformanceAnalyzer.Models;
using SqlSugar.PerformanceAnalyzer.Reporters;

namespace SqlSugar.PerformanceAnalyzer
{
    /// <summary>
    /// Main provider for SqlSugar Performance Analyzer
    /// </summary>
    public class PerformanceAnalyzerProvider
    {
        private readonly SqlSugarClient _client;
        private readonly PerformanceAnalyzerConfig _config;
        private readonly QueryPerformanceCollector _collector;
        private readonly PerformanceAggregator _aggregator;
        private readonly QueryOptimizationAdvisor _advisor;
        private readonly IndexAdvisor _indexAdvisor;
        private readonly NPlusOneDetector _nPlusOneDetector;
        private readonly PerformanceExporter _exporter;
        private readonly HtmlReporter _htmlReporter;
        private readonly JsonReporter _jsonReporter;
        private readonly CsvReporter _csvReporter;
        
        public PerformanceAnalyzerProvider(SqlSugarClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _config = new PerformanceAnalyzerConfig();
            
            var dbType = client.CurrentConnectionConfig?.DbType ?? DbType.SqlServer;
            
            _collector = new QueryPerformanceCollector(_config, dbType);
            _aggregator = new PerformanceAggregator(_config);
            _advisor = new QueryOptimizationAdvisor(_config, dbType);
            _indexAdvisor = new IndexAdvisor(_config, dbType);
            _nPlusOneDetector = new NPlusOneDetector(_config);
            _exporter = new PerformanceExporter();
            _htmlReporter = new HtmlReporter();
            _jsonReporter = new JsonReporter();
            _csvReporter = new CsvReporter();
        }
        
        /// <summary>
        /// Gets the current configuration
        /// </summary>
        public PerformanceAnalyzerConfig Config => _config;
        
        /// <summary>
        /// Enable performance tracking
        /// </summary>
        public void Enable()
        {
            _config.IsEnabled = true;
            AttachToAop();
        }
        
        /// <summary>
        /// Disable performance tracking
        /// </summary>
        public void Disable()
        {
            _config.IsEnabled = false;
        }
        
        /// <summary>
        /// Configure the performance analyzer
        /// </summary>
        public void Configure(Action<PerformanceAnalyzerConfig> configAction)
        {
            configAction?.Invoke(_config);
        }
        
        /// <summary>
        /// Clear all collected metrics
        /// </summary>
        public void Clear()
        {
            _collector.Clear();
        }
        
        /// <summary>
        /// Get performance statistics
        /// </summary>
        public PerformanceStatistics GetStatistics()
        {
            var metrics = _collector.GetMetrics();
            return _aggregator.Aggregate(metrics);
        }
        
        /// <summary>
        /// Get top N slowest queries
        /// </summary>
        public List<QueryPerformanceMetric> GetSlowQueries(int count = 10)
        {
            var metrics = _collector.GetMetrics();
            return metrics
                .Where(m => m.IsSlowQuery)
                .OrderByDescending(m => m.ExecutionTimeMs)
                .Take(count)
                .ToList();
        }
        
        /// <summary>
        /// Get all optimization suggestions
        /// </summary>
        public List<OptimizationSuggestion> GetOptimizationSuggestions()
        {
            var metrics = _collector.GetMetrics();
            return _advisor.GetSuggestions(metrics);
        }
        
        /// <summary>
        /// Detect N+1 query issues
        /// </summary>
        public List<NPlusOneIssue> DetectNPlusOneQueries()
        {
            var metrics = _collector.GetMetrics();
            return _nPlusOneDetector.Detect(metrics);
        }
        
        /// <summary>
        /// Get index recommendations
        /// </summary>
        public List<IndexRecommendation> GetIndexRecommendations()
        {
            var metrics = _collector.GetMetrics();
            return _indexAdvisor.GetIndexRecommendations(metrics);
        }
        
        /// <summary>
        /// Generate a complete performance report
        /// </summary>
        public PerformanceReport GenerateReport()
        {
            var metrics = _collector.GetMetrics();
            var report = _aggregator.GenerateReport(metrics);
            
            // Add suggestions and issues
            report.Suggestions = GetOptimizationSuggestions();
            report.NPlusOneIssues = DetectNPlusOneQueries();
            report.IndexRecommendations = GetIndexRecommendations();
            
            report.GenerateSummary();
            
            return report;
        }
        
        /// <summary>
        /// Generate HTML report
        /// </summary>
        public string GenerateHtmlReport()
        {
            var report = GenerateReport();
            return _htmlReporter.Generate(report);
        }
        
        /// <summary>
        /// Generate JSON report
        /// </summary>
        public string GenerateJsonReport()
        {
            var report = GenerateReport();
            return _jsonReporter.Generate(report);
        }
        
        /// <summary>
        /// Generate CSV report
        /// </summary>
        public string GenerateCsvReport()
        {
            var report = GenerateReport();
            return _csvReporter.Generate(report);
        }
        
        /// <summary>
        /// Export report to JSON file
        /// </summary>
        public void ExportToJson(string filePath)
        {
            var report = GenerateReport();
            _exporter.ExportToJson(filePath, report);
        }
        
        /// <summary>
        /// Export report to CSV file
        /// </summary>
        public void ExportToCsv(string filePath)
        {
            var report = GenerateReport();
            _exporter.ExportToCsv(filePath, report);
        }
        
        /// <summary>
        /// Import baseline report from JSON file
        /// </summary>
        public PerformanceReport ImportBaseline(string filePath)
        {
            return _exporter.ImportFromJson(filePath);
        }
        
        /// <summary>
        /// Compare current performance with baseline
        /// </summary>
        public PerformanceComparison CompareWithBaseline(PerformanceReport baseline)
        {
            var current = GetStatistics();
            
            return new PerformanceComparison
            {
                BaselineDate = baseline.GeneratedAt,
                CurrentDate = DateTime.UtcNow,
                PerformanceChangePercent = CalculatePerformanceChange(baseline.Statistics, current),
                RegressionCount = CountRegressions(baseline.Statistics, current),
                ImprovementCount = CountImprovements(baseline.Statistics, current)
            };
        }
        
        /// <summary>
        /// Get dashboard data
        /// </summary>
        public Dictionary<string, object> GetDashboardData()
        {
            var metrics = _collector.GetMetrics();
            return _aggregator.GetDashboardData(metrics);
        }
        
        /// <summary>
        /// Get all collected metrics
        /// </summary>
        public List<QueryPerformanceMetric> GetAllMetrics()
        {
            return _collector.GetMetrics();
        }
        
        /// <summary>
        /// Get recent metrics
        /// </summary>
        public List<QueryPerformanceMetric> GetRecentMetrics(int count = 100)
        {
            return _collector.GetMetrics(count);
        }
        
        private void AttachToAop()
        {
            // Hook into SqlSugar's AOP events
            // Note: AOP properties are set-only, we cannot chain existing handlers
            _client.Aop.OnLogExecuting = (sql, pars) =>
            {
                if (_config.IsEnabled)
                {
                    _collector.StartTracking(sql, pars);
                }
            };
            
            _client.Aop.OnLogExecuted = (sql, pars) =>
            {
                if (_config.IsEnabled)
                {
                    _collector.StopTracking(sql, 0, true);
                }
            };
        }
        
        private double CalculatePerformanceChange(PerformanceStatistics baseline, PerformanceStatistics current)
        {
            if (baseline.AverageExecutionTime == 0) return 0;
            
            return ((current.AverageExecutionTime - baseline.AverageExecutionTime) / baseline.AverageExecutionTime) * 100;
        }
        
        private int CountRegressions(PerformanceStatistics baseline, PerformanceStatistics current)
        {
            int count = 0;
            
            if (current.AverageExecutionTime > baseline.AverageExecutionTime * 1.1) count++;
            if (current.P95ExecutionTime > baseline.P95ExecutionTime * 1.1) count++;
            if (current.SlowQueryCount > baseline.SlowQueryCount) count++;
            
            return count;
        }
        
        private int CountImprovements(PerformanceStatistics baseline, PerformanceStatistics current)
        {
            int count = 0;
            
            if (current.AverageExecutionTime < baseline.AverageExecutionTime * 0.9) count++;
            if (current.P95ExecutionTime < baseline.P95ExecutionTime * 0.9) count++;
            if (current.SlowQueryCount < baseline.SlowQueryCount) count++;
            
            return count;
        }
    }
    
    /// <summary>
    /// Performance comparison result
    /// </summary>
    public class PerformanceComparison
    {
        public DateTime BaselineDate { get; set; }
        public DateTime CurrentDate { get; set; }
        public double PerformanceChangePercent { get; set; }
        public int RegressionCount { get; set; }
        public int ImprovementCount { get; set; }
    }
}