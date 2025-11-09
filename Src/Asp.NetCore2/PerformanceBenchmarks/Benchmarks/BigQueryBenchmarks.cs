using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.Data.SqlClient;
using PerformanceBenchmarks.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceBenchmarks.Benchmarks
{
    /// <summary>
    /// Big query
    /// 查询大数据
    /// </summary>
    [MemoryDiagnoser]
    [RankColumn]
    public class BigQueryBenchmarks
    {
        private SqlSugarClient _sqlSugarDb;
        private SqlConnection _dapperConnection; 
        /// <summary>
        /// Setup method called before benchmarks
        /// 基准测试前调用的设置方法
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            // Initialize database
            // 初始化数据库
            BenchmarkConfig.InitializeDatabase();
            BenchmarkConfig.CleanupDatabase();

            _sqlSugarDb = BenchmarkConfig.GetSqlServerDb();
            _dapperConnection = new SqlConnection(BenchmarkConfig.SqlServerConnection);
            _dapperConnection.Open();

            // Insert test data
            // 插入测试数据
            InsertTestData();
        }

        /// <summary>
        /// Cleanup method called after benchmarks
        /// 基准测试后调用的清理方法
        /// </summary>
        [GlobalCleanup]
        public void Cleanup()
        {
            _dapperConnection?.Close();
            _dapperConnection?.Dispose();
            BenchmarkConfig.CleanupDatabase();
        }

        /// <summary>
        /// Insert test data for benchmarks
        /// 为基准测试插入测试数据
        /// </summary>
        private void InsertTestData()
        {
            // Insert customers
            // 插入客户
            var customers = new List<BenchmarkCustomer>();
            for (int i = 1; i <= 100000; i++)
            {
                customers.Add(new BenchmarkCustomer
                {
                    CustomerName = $"Customer {i}",
                    Email = $"customer{i}@test.com",
                    Phone = $"123-456-{i:D4}",
                    Address = $"{i} Main Street",
                    City = "TestCity",
                    Country = "TestCountry",
                    RegistrationDate = DateTime.Now.AddDays(-i),
                    IsActive = i % 2 == 0
                });
            }
            _sqlSugarDb.Fastest< BenchmarkCustomer>().BulkCopy(customers);
             
        }

        /// <summary>
        /// SqlSugar: Query 100000 rows
        /// SqlSugar: 查询10万
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> SqlSugar_SimpleQuery()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder>().ToList();
        }

        /// <summary>
        /// Dapper: Query 100000 rows
        /// Dapper: 查询10万
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> Dapper_SimpleQuery()
        {
            return _dapperConnection.Query<BenchmarkOrder>(
                "SELECT * FROM BenchmarkOrder  "
            ).ToList();
        } 
    }
}
