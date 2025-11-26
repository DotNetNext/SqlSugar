using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SqlSugar.PerformanceAnalyzer.Models;
using SqlSugar.PerformanceAnalyzer.Reporters;

namespace SqlSugar.PerformanceAnalyzer.Export
{
    /// <summary>
    /// Handles export and import of performance data
    /// </summary>
    public class PerformanceExporter : IPerformanceExporter
    {
        private readonly JsonReporter _jsonReporter;
        private readonly CsvReporter _csvReporter;
        
        public PerformanceExporter()
        {
            _jsonReporter = new JsonReporter();
            _csvReporter = new CsvReporter();
        }
        
        public void ExportToJson(string filePath, PerformanceReport report)
        {
            try
            {
                var json = _jsonReporter.Generate(report);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to export to JSON: {ex.Message}", ex);
            }
        }
        
        public void ExportToCsv(string filePath, PerformanceReport report)
        {
            try
            {
                var csv = _csvReporter.Generate(report);
                File.WriteAllText(filePath, csv);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to export to CSV: {ex.Message}", ex);
            }
        }
        
        public PerformanceReport ImportFromJson(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }
                
                var json = File.ReadAllText(filePath);
                var report = JsonConvert.DeserializeObject<PerformanceReport>(json);
                
                return report ?? new PerformanceReport();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to import from JSON: {ex.Message}", ex);
            }
        }
        
        public List<QueryPerformanceMetric> ImportMetricsFromJson(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }
                
                var json = File.ReadAllText(filePath);
                var metrics = JsonConvert.DeserializeObject<List<QueryPerformanceMetric>>(json);
                
                return metrics ?? new List<QueryPerformanceMetric>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to import metrics from JSON: {ex.Message}", ex);
            }
        }
        
        public void ExportMetricsToJson(string filePath, List<QueryPerformanceMetric> metrics)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                
                var json = JsonConvert.SerializeObject(metrics, settings);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to export metrics to JSON: {ex.Message}", ex);
            }
        }
    }
}