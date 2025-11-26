using System;
using Newtonsoft.Json;
using SqlSugar.PerformanceAnalyzer.Models;

namespace SqlSugar.PerformanceAnalyzer.Reporters
{
    /// <summary>
    /// Generates JSON performance reports
    /// </summary>
    public class JsonReporter : IPerformanceReporter
    {
        public string Generate(PerformanceReport report)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                
                return JsonConvert.SerializeObject(report, settings);
            }
            catch (Exception ex)
            {
                return $"{{\"error\": \"Failed to generate JSON report: {ex.Message}\"}}";
            }
        }
    }
}