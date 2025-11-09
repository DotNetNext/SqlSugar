using BenchmarkDotNet.Running;
using System;

namespace PerformanceBenchmarks
{
    /// <summary>
    /// Main program entry point for SqlSugar performance benchmarks
    /// SqlSugar 性能基准测试的主程序入口点
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("SqlSugar Performance Benchmark Suite");
            Console.WriteLine("SqlSugar 性能基准测试套件");
            Console.WriteLine("===========================================");
            Console.WriteLine();

            if (args.Length > 0 && args[0] == "--test")
            {
                // Run quick tests without benchmarking
                // 运行快速测试而不进行基准测试
                RunQuickTests();
            }
            else
            {
                // Run full benchmark suite
                // 运行完整的基准测试套件
                RunBenchmarks();
            }
        }

        /// <summary>
        /// Run all benchmark tests
        /// 运行所有基准测试
        /// </summary>
        static void RunBenchmarks()
        {
            Console.WriteLine("Starting benchmark tests...");
            Console.WriteLine("开始基准测试...");
            Console.WriteLine();

            // Run query benchmarks
            // 运行查询基准测试
            BenchmarkRunner.Run<Benchmarks.QueryBenchmarks>();

            // Run insert benchmarks
            // 运行插入基准测试
            BenchmarkRunner.Run<Benchmarks.InsertBenchmarks>();

            // Run update benchmarks
            // 运行更新基准测试
            BenchmarkRunner.Run<Benchmarks.UpdateBenchmarks>();

            // Run delete benchmarks
            // 运行删除基准测试
            BenchmarkRunner.Run<Benchmarks.DeleteBenchmarks>();

            // Run bulk operation benchmarks
            // 运行批量操作基准测试
            BenchmarkRunner.Run<Benchmarks.BulkOperationBenchmarks>();

            // Run join query benchmarks
            // 运行联接查询基准测试
            BenchmarkRunner.Run<Benchmarks.JoinQueryBenchmarks>();

            // Run complex query benchmarks
            // 运行复杂查询基准测试
            BenchmarkRunner.Run<Benchmarks.ComplexQueryBenchmarks>();

            Console.WriteLine();
            Console.WriteLine("所有基准测试完成！");
        }

        /// <summary>
        /// Run quick tests for validation
        /// 运行快速验证测试
        /// </summary>
        /// <remarks>
        /// This method runs comprehensive validation tests to ensure all components
        /// are working correctly before running the full benchmark suite.
        /// 此方法运行全面的验证测试，以确保在运行完整基准测试套件之前所有组件都正常工作。
        /// </remarks>
        static void RunQuickTests()
        {
            Console.WriteLine("Running quick validation tests...");
            Console.WriteLine("运行快速验证测试...");
            Console.WriteLine();

            try
            {
                // Run comprehensive validation tests
                // 运行全面的验证测试
                var allTestsPassed = Tests.ValidationTests.RunAll();

                if (!allTestsPassed)
                {
                    Console.WriteLine();
                    Console.WriteLine("  Some tests failed. Please check the configuration.");
                    Console.WriteLine("  某些测试失败。请检查配置。");
                    Environment.Exit(1);
                }

                Console.WriteLine();
                Console.WriteLine(" All validation tests passed! Ready to run benchmarks.");
                Console.WriteLine("  所有验证测试通过！准备运行基准测试。");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"  Test failed: {ex.Message}");
                Console.WriteLine($"  测试失败: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("Stack trace:");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }
        }
    }
}
