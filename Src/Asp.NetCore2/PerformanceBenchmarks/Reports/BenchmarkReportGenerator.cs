using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PerformanceBenchmarks.Reports
{
    /// <summary>
    /// Benchmark report generator
    /// 基准测试报告生成器
    /// </summary>
    public class BenchmarkReportGenerator
    {
        /// <summary>
        /// Generate HTML report
        /// 生成 HTML 报告
        /// </summary>
        /// <param name="results">Benchmark results / 基准测试结果</param>
        /// <param name="outputPath">Output file path / 输出文件路径</param>
        public static void GenerateHtmlReport(Dictionary<string, BenchmarkResult> results, string outputPath)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <title>SqlSugar Performance Benchmark Report</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine("        h1 { color: #333; }");
            html.AppendLine("        table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
            html.AppendLine("        th, td { border: 1px solid #ddd; padding: 12px; text-align: left; }");
            html.AppendLine("        th { background-color: #4CAF50; color: white; }");
            html.AppendLine("        tr:nth-child(even) { background-color: #f2f2f2; }");
            html.AppendLine("        .winner { background-color: #d4edda; font-weight: bold; }");
            html.AppendLine("        .summary { background-color: #e7f3ff; padding: 15px; margin: 20px 0; border-radius: 5px; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <h1>SqlSugar Performance Benchmark Report</h1>");
            html.AppendLine($"    <p>Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            
            html.AppendLine("    <div class='summary'>");
            html.AppendLine("        <h2>Summary / 摘要</h2>");
            html.AppendLine($"        <p>Total Benchmarks: {results.Count}</p>");
            html.AppendLine("        <p>This report compares SqlSugar performance with other ORMs.</p>");
            html.AppendLine("        <p>本报告比较了 SqlSugar 与其他 ORM 的性能表现。</p>");
            html.AppendLine("    </div>");

            html.AppendLine("    <table>");
            html.AppendLine("        <tr>");
            html.AppendLine("            <th>Benchmark Name / 基准测试名称</th>");
            html.AppendLine("            <th>Mean Time / 平均时间</th>");
            html.AppendLine("            <th>Memory Allocated / 内存分配</th>");
            html.AppendLine("            <th>Rank / 排名</th>");
            html.AppendLine("        </tr>");

            foreach (var result in results)
            {
                var rowClass = result.Value.Rank == 1 ? "class='winner'" : "";
                html.AppendLine($"        <tr {rowClass}>");
                html.AppendLine($"            <td>{result.Key}</td>");
                html.AppendLine($"            <td>{result.Value.MeanTime}</td>");
                html.AppendLine($"            <td>{result.Value.MemoryAllocated}</td>");
                html.AppendLine($"            <td>{result.Value.Rank}</td>");
                html.AppendLine("        </tr>");
            }

            html.AppendLine("    </table>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            File.WriteAllText(outputPath, html.ToString());
        }

        /// <summary>
        /// Generate Markdown report
        /// 生成 Markdown 报告
        /// </summary>
        /// <param name="results">Benchmark results / 基准测试结果</param>
        /// <param name="outputPath">Output file path / 输出文件路径</param>
        public static void GenerateMarkdownReport(Dictionary<string, BenchmarkResult> results, string outputPath)
        {
            var md = new StringBuilder();
            md.AppendLine("# SqlSugar Performance Benchmark Report");
            md.AppendLine();
            md.AppendLine($"**Generated on:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            md.AppendLine();
            md.AppendLine("## Summary / 摘要");
            md.AppendLine();
            md.AppendLine($"- Total Benchmarks: {results.Count}");
            md.AppendLine("- This report compares SqlSugar performance with other ORMs.");
            md.AppendLine("- 本报告比较了 SqlSugar 与其他 ORM 的性能表现。");
            md.AppendLine();
            md.AppendLine("## Results / 结果");
            md.AppendLine();
            md.AppendLine("| Benchmark Name | Mean Time | Memory Allocated | Rank |");
            md.AppendLine("|----------------|-----------|------------------|------|");

            foreach (var result in results)
            {
                var winner = result.Value.Rank == 1 ? " 🏆" : "";
                md.AppendLine($"| {result.Key}{winner} | {result.Value.MeanTime} | {result.Value.MemoryAllocated} | {result.Value.Rank} |");
            }

            md.AppendLine();
            md.AppendLine("## Notes / 说明");
            md.AppendLine();
            md.AppendLine("- 🏆 indicates the best performing method in each category");
            md.AppendLine("- 🏆 表示每个类别中性能最佳的方法");

            File.WriteAllText(outputPath, md.ToString());
        }

        /// <summary>
        /// Generate JSON report
        /// 生成 JSON 报告
        /// </summary>
        /// <param name="results">Benchmark results / 基准测试结果</param>
        /// <param name="outputPath">Output file path / 输出文件路径</param>
        public static void GenerateJsonReport(Dictionary<string, BenchmarkResult> results, string outputPath)
        {
            var json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine($"  \"generatedOn\": \"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\",");
            json.AppendLine($"  \"totalBenchmarks\": {results.Count},");
            json.AppendLine("  \"results\": [");

            var isFirst = true;
            foreach (var result in results)
            {
                if (!isFirst)
                    json.AppendLine(",");
                
                json.AppendLine("    {");
                json.AppendLine($"      \"name\": \"{result.Key}\",");
                json.AppendLine($"      \"meanTime\": \"{result.Value.MeanTime}\",");
                json.AppendLine($"      \"memoryAllocated\": \"{result.Value.MemoryAllocated}\",");
                json.AppendLine($"      \"rank\": {result.Value.Rank}");
                json.Append("    }");
                
                isFirst = false;
            }

            json.AppendLine();
            json.AppendLine("  ]");
            json.AppendLine("}");

            File.WriteAllText(outputPath, json.ToString());
        }
    }

    /// <summary>
    /// Benchmark result model
    /// 基准测试结果模型
    /// </summary>
    public class BenchmarkResult
    {
        /// <summary>
        /// Mean execution time
        /// 平均执行时间
        /// </summary>
        public string MeanTime { get; set; }

        /// <summary>
        /// Memory allocated
        /// 内存分配
        /// </summary>
        public string MemoryAllocated { get; set; }

        /// <summary>
        /// Performance rank
        /// 性能排名
        /// </summary>
        public int Rank { get; set; }
    }
}
