using System;

namespace SqlSugar.PerformanceAnalyzer.Models
{
    /// <summary>
    /// Configuration settings for the Performance Analyzer
    /// </summary>
    public class PerformanceAnalyzerConfig
    {
        /// <summary>
        /// Whether performance tracking is enabled
        /// </summary>
        public bool IsEnabled { get; set; }
        
        /// <summary>
        /// Threshold in milliseconds to mark a query as slow
        /// </summary>
        public int SlowQueryThreshold { get; set; } = 1000;
        
        /// <summary>
        /// Threshold in milliseconds to mark a query as very slow
        /// </summary>
        public int VerySlowQueryThreshold { get; set; } = 5000;
        
        /// <summary>
        /// Enable N+1 query detection
        /// </summary>
        public bool EnableNPlusOneDetection { get; set; } = true;
        
        /// <summary>
        /// Enable index analysis and recommendations
        /// </summary>
        public bool EnableIndexAnalysis { get; set; } = true;
        
        /// <summary>
        /// Track memory usage per query
        /// </summary>
        public bool EnableMemoryTracking { get; set; } = false;
        
        /// <summary>
        /// Capture stack traces for debugging
        /// </summary>
        public bool EnableStackTraceCapture { get; set; } = false;
        
        /// <summary>
        /// Maximum number of queries to keep in cache
        /// </summary>
        public int MaxCachedQueries { get; set; } = 10000;
        
        /// <summary>
        /// How long to keep cached performance data
        /// </summary>
        public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromHours(24);
        
        /// <summary>
        /// Enable sampling mode for high-traffic scenarios
        /// </summary>
        public bool EnableSampling { get; set; } = false;
        
        /// <summary>
        /// Sampling rate (0.0 to 1.0). 1.0 = track all queries
        /// </summary>
        public double SamplingRate { get; set; } = 1.0;
        
        /// <summary>
        /// Minimum query execution count to trigger N+1 detection
        /// </summary>
        public int NPlusOneThreshold { get; set; } = 5;
        
        /// <summary>
        /// Minimum number of queries required for analysis
        /// </summary>
        public int MinQueriesForAnalysis { get; set; } = 10;
        
        public PerformanceAnalyzerConfig()
        {
            IsEnabled = false;
        }
    }
}