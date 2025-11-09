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
    /// Delete operation benchmarks comparing SqlSugar with other ORMs
    /// 删除操作基准测试，比较 SqlSugar 与其他 ORM
    /// </summary>
    [MemoryDiagnoser]
    [RankColumn]
    public class DeleteBenchmarks
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
            BenchmarkConfig.InitializeDatabase();
            _sqlSugarDb = BenchmarkConfig.GetSqlServerDb();
            _dapperConnection = new SqlConnection(BenchmarkConfig.SqlServerConnection);
            _dapperConnection.Open();
            _sqlSugarDb.Open();
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
            _sqlSugarDb.Close();
        }

        /// <summary>
        /// Iteration setup - insert test data before each iteration
        /// 迭代设置 - 每次迭代前插入测试数据
        /// </summary>
        [IterationSetup]
        public void IterationSetup()
        {
            _sqlSugarDb.DbMaintenance.TruncateTable<BenchmarkCustomer>();
            
            var customers = new List<BenchmarkCustomer>();
            for (int i = 1; i <= 100; i++)
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
            _sqlSugarDb.Insertable(customers).ExecuteCommand();
        }

        /// <summary>
        /// SqlSugar: Delete by entity
        /// SqlSugar: 按实体删除
        /// </summary>
        [Benchmark]
        public int SqlSugar_DeleteByEntity()
        {
            var customer = _sqlSugarDb.Queryable<BenchmarkCustomer>()
                .First(c => c.CustomerName == "Customer 1");

            return _sqlSugarDb.Deleteable(customer).ExecuteCommand();
        }

        /// <summary>
        /// Dapper: Delete by ID
        /// Dapper: 按 ID 删除
        /// </summary>
        [Benchmark]
        public int Dapper_DeleteById()
        {
            var customerId = _dapperConnection.QueryFirst<int>(
                "SELECT TOP 1 CustomerId FROM BenchmarkCustomer WHERE CustomerName = @Name",
                new { Name = "Customer 2" }
            );

            var sql = "DELETE FROM BenchmarkCustomer WHERE CustomerId = @CustomerId";
            return _dapperConnection.Execute(sql, new { CustomerId = customerId });
        }

        /// <summary>
        /// SqlSugar: Delete by ID
        /// SqlSugar: 按 ID 删除
        /// </summary>
        [Benchmark]
        public int SqlSugar_DeleteById()
        {
            var customerId = _sqlSugarDb.Queryable<BenchmarkCustomer>()
                .Where(c => c.CustomerName == "Customer 3")
                .Select(c => c.CustomerId)
                .First();

            return _sqlSugarDb.Deleteable<BenchmarkCustomer>()
                .In(customerId)
                .ExecuteCommand();
        }

        /// <summary>
        /// SqlSugar: Delete with condition
        /// SqlSugar: 条件删除
        /// </summary>
        [Benchmark]
        public int SqlSugar_DeleteWithCondition()
        {
            return _sqlSugarDb.Deleteable<BenchmarkCustomer>()
                .Where(c => c.IsActive == false && c.City == "TestCity")
                .ExecuteCommand();
        }

        /// <summary>
        /// Dapper: Delete with condition
        /// Dapper: 条件删除
        /// </summary>
        [Benchmark]
        public int Dapper_DeleteWithCondition()
        {
            var sql = @"DELETE FROM BenchmarkCustomer 
                       WHERE IsActive = @IsActive AND City = @City";

            return _dapperConnection.Execute(sql, new
            {
                IsActive = false,
                City = "TestCity"
            });
        }

        /// <summary>
        /// SqlSugar: Delete by ID list
        /// SqlSugar: 按 ID 列表删除
        /// </summary>
        [Benchmark]
        public int SqlSugar_DeleteByIdList()
        {
            var customerIds = _sqlSugarDb.Queryable<BenchmarkCustomer>()
                .Where(c => c.CustomerName.Contains("Customer"))
                .Take(10)
                .Select(c => c.CustomerId)
                .ToList();

            return _sqlSugarDb.Deleteable<BenchmarkCustomer>()
                .In(customerIds)
                .ExecuteCommand();
        }

        /// <summary>
        /// Dapper: Delete by ID list
        /// Dapper: 按 ID 列表删除
        /// </summary>
        [Benchmark]
        public int Dapper_DeleteByIdList()
        {
            var customerIds = _dapperConnection.Query<int>(
                "SELECT TOP 10 CustomerId FROM BenchmarkCustomer WHERE CustomerName LIKE @Pattern",
                new { Pattern = "%Customer%" }
            ).ToList();

            var sql = "DELETE FROM BenchmarkCustomer WHERE CustomerId IN @CustomerIds";
            return _dapperConnection.Execute(sql, new { CustomerIds = customerIds });
        }

        /// <summary>
        /// SqlSugar: Delete all records
        /// SqlSugar: 删除所有记录
        /// </summary>
        [Benchmark]
        public int SqlSugar_DeleteAll()
        {
            return _sqlSugarDb.Deleteable<BenchmarkCustomer>()
                .Where(c => c.CustomerId > 0)
                .ExecuteCommand();
        }

        /// <summary>
        /// Dapper: Delete all records
        /// Dapper: 删除所有记录
        /// </summary>
        [Benchmark]
        public int Dapper_DeleteAll()
        {
            var sql = "DELETE FROM BenchmarkCustomer WHERE CustomerId > @Id";
            return _dapperConnection.Execute(sql, new { Id = 0 });
        }
    }
}
