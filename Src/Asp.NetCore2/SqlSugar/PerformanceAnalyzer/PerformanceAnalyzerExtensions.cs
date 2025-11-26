using System;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer
{
    /// <summary>
    /// Extension methods for Performance Analyzer
    /// </summary>
    public static class PerformanceAnalyzerExtensions
    {
        private const string ANALYZER_KEY = "__PerformanceAnalyzer__";
        
        /// <summary>
        /// Get or create performance analyzer instance for this client
        /// </summary>
        public static PerformanceAnalyzerProvider GetPerformanceAnalyzer(this ISqlSugarClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            
            // Store analyzer in TempItems to maintain single instance per client
            if (!client.TempItems.ContainsKey(ANALYZER_KEY))
            {
                // Cast to SqlSugarClient - works with SqlSugarClient, SqlSugarScope, etc.
                var sqlSugarClient = client as SqlSugarClient ?? throw new InvalidOperationException("Client must be SqlSugarClient or derived type");
                client.TempItems[ANALYZER_KEY] = new PerformanceAnalyzerProvider(sqlSugarClient);
            }
            
            return client.TempItems[ANALYZER_KEY] as PerformanceAnalyzerProvider;
        }
        
        /// <summary>
        /// Enable performance analyzer with default configuration
        /// </summary>
        public static void EnablePerformanceAnalyzer(this ISqlSugarClient client)
        {
            var analyzer = client.GetPerformanceAnalyzer();
            analyzer.Enable();
        }
        
        /// <summary>
        /// Enable performance analyzer with custom configuration
        /// </summary>
        public static void EnablePerformanceAnalyzer(this ISqlSugarClient client, Action<PerformanceAnalyzerConfig> configAction)
        {
            var analyzer = client.GetPerformanceAnalyzer();
            analyzer.Configure(configAction);
            analyzer.Enable();
        }
        
        /// <summary>
        /// Quick method to get performance statistics
        /// </summary>
        public static PerformanceStatistics GetPerformanceStats(this ISqlSugarClient client)
        {
            var analyzer = client.GetPerformanceAnalyzer();
            return analyzer.GetStatistics();
        }
    }
}