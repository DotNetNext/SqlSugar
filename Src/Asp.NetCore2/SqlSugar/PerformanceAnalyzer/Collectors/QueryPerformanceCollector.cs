using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Collectors
{
    /// <summary>
    /// Collects and tracks query performance metrics
    /// </summary>
    public class QueryPerformanceCollector : IPerformanceCollector
    {
        private readonly ConcurrentDictionary<string, QueryPerformanceMetric> _activeQueries;
        private readonly ConcurrentQueue<QueryPerformanceMetric> _completedQueries;
        private readonly PerformanceAnalyzerConfig _config;
        private readonly ReaderWriterLockSlim _lock;
        private readonly DbType _dbType;
        private int _totalQueries;
        
        public QueryPerformanceCollector(PerformanceAnalyzerConfig config, DbType dbType)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _dbType = dbType;
            _activeQueries = new ConcurrentDictionary<string, QueryPerformanceMetric>();
            _completedQueries = new ConcurrentQueue<QueryPerformanceMetric>();
            _lock = new ReaderWriterLockSlim();
            _totalQueries = 0;
        }
        
        public void StartTracking(string sql, SugarParameter[] parameters)
        {
            if (!_config.IsEnabled) return;
            if (_config.EnableSampling && !ShouldSample()) return;
            
            var metric = new QueryPerformanceMetric
            {
                SqlText = sql,
                NormalizedSql = NormalizeSql(sql),
                Parameters = parameters?.ToList() ?? new List<SugarParameter>(),
                StartTime = DateTime.UtcNow,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                DbType = _dbType
            };
            
            if (_config.EnableMemoryTracking)
            {
                metric.MemoryBefore = GC.GetTotalMemory(false);
            }
            
            if (_config.EnableStackTraceCapture)
            {
                metric.StackTrace = new StackTrace(2, true).ToString();
            }
            
            metric.QueryType = DetermineQueryType(sql);
            
            var key = GenerateKey(sql, Thread.CurrentThread.ManagedThreadId);
            _activeQueries.TryAdd(key, metric);
        }
        
        public void StopTracking(string sql, int rowCount, bool isSuccess, string errorMessage = null)
        {
            if (!_config.IsEnabled) return;
            
            var key = GenerateKey(sql, Thread.CurrentThread.ManagedThreadId);
            if (_activeQueries.TryRemove(key, out var metric))
            {
                metric.EndTime = DateTime.UtcNow;
                metric.ExecutionTimeMs = (metric.EndTime - metric.StartTime).TotalMilliseconds;
                metric.RowCount = rowCount;
                metric.IsSuccess = isSuccess;
                metric.ErrorMessage = errorMessage;
                
                if (_config.EnableMemoryTracking)
                {
                    metric.MemoryAfter = GC.GetTotalMemory(false);
                }
                
                metric.IsSlowQuery = metric.ExecutionTimeMs >= _config.SlowQueryThreshold;
                metric.ComplexityScore = CalculateComplexityScore(metric);
                
                _completedQueries.Enqueue(metric);
                Interlocked.Increment(ref _totalQueries);
                
                // Cleanup old queries if cache is full
                while (_completedQueries.Count > _config.MaxCachedQueries)
                {
                    _completedQueries.TryDequeue(out _);
                }
            }
        }
        
        public List<QueryPerformanceMetric> GetMetrics(int count = 0)
        {
            _lock.EnterReadLock();
            try
            {
                var metrics = _completedQueries.ToList();
                return count > 0 ? metrics.TakeLast(count).ToList() : metrics;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        
        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _activeQueries.Clear();
                while (_completedQueries.TryDequeue(out _)) { }
                _totalQueries = 0;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        private string GenerateKey(string sql, int threadId)
        {
            return $"{sql.GetHashCode()}_{threadId}_{DateTime.UtcNow.Ticks}";
        }
        
        private bool ShouldSample()
        {
            return new Random().NextDouble() <= _config.SamplingRate;
        }
        
        private string NormalizeSql(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return sql;
            
            try
            {
                // Replace parameter values with placeholders
                var normalized = Regex.Replace(sql, @"@\w+", "@p", RegexOptions.IgnoreCase);
                normalized = Regex.Replace(normalized, @"'\w+'", "'?'", RegexOptions.IgnoreCase);
                normalized = Regex.Replace(normalized, @"\b\d+\b", "?");
                return normalized;
            }
            catch
            {
                return sql;
            }
        }
        
        private QueryType DetermineQueryType(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return QueryType.Other;
            
            var upperSql = sql.TrimStart().ToUpperInvariant();
            if (upperSql.StartsWith("SELECT")) return QueryType.Select;
            if (upperSql.StartsWith("INSERT")) return QueryType.Insert;
            if (upperSql.StartsWith("UPDATE")) return QueryType.Update;
            if (upperSql.StartsWith("DELETE")) return QueryType.Delete;
            if (upperSql.StartsWith("MERGE")) return QueryType.Merge;
            if (upperSql.StartsWith("CREATE TABLE")) return QueryType.CreateTable;
            return QueryType.Other;
        }
        
        private int CalculateComplexityScore(QueryPerformanceMetric metric)
        {
            int score = 0;
            var sql = metric.SqlText?.ToUpperInvariant() ?? "";
            
            try
            {
                // Count JOINs
                metric.JoinCount = Regex.Matches(sql, @"\bJOIN\b").Count;
                score += metric.JoinCount * 10;
                
                // Count subqueries
                metric.SubqueryCount = Regex.Matches(sql, @"\(\s*SELECT").Count;
                score += metric.SubqueryCount * 15;
                
                // Count tables (rough estimate)
                metric.TableCount = Regex.Matches(sql, @"\bFROM\b|\bJOIN\b").Count;
                score += metric.TableCount * 5;
                
                // Add score for DISTINCT, GROUP BY, HAVING, etc.
                if (sql.Contains("DISTINCT")) score += 5;
                if (sql.Contains("GROUP BY")) score += 10;
                if (sql.Contains("HAVING")) score += 10;
                if (sql.Contains("UNION")) score += 15;
            }
            catch
            {
                // Ignore calculation errors
            }
            
            return Math.Min(score, 100);
        }
    }
}