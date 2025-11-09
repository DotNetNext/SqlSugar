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
    /// Update operation benchmarks comparing SqlSugar with other ORMs
    /// 更新操作基准测试，比较 SqlSugar 与其他 ORM
    /// </summary>
    [MemoryDiagnoser]
    [RankColumn]
    public class UpdateBenchmarks
    {
        private SqlSugarClient _sqlSugarDb;
        private SqlConnection _dapperConnection;
        private List<int> _testCustomerIds;

        /// <summary>
        /// Setup method called before benchmarks
        /// 基准测试前调用的设置方法
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            BenchmarkConfig.InitializeDatabase();
            BenchmarkConfig.CleanupDatabase();

            _sqlSugarDb = BenchmarkConfig.GetSqlServerDb();
            _dapperConnection = new SqlConnection(BenchmarkConfig.SqlServerConnection);
            _dapperConnection.Open();

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
                    IsActive = true
                });
            }
            _testCustomerIds = _sqlSugarDb.Insertable(customers).ExecuteReturnPkList<int>();
        }

        /// <summary>
        /// SqlSugar: Single update by entity
        /// SqlSugar: 按实体单条更新
        /// </summary>
        [Benchmark]
        public int SqlSugar_SingleUpdate()
        {
            var customer = _sqlSugarDb.Queryable<BenchmarkCustomer>()
                .InSingle(_testCustomerIds[0]);
            
            customer.CustomerName = "Updated Customer";
            customer.Email = "updated@test.com";

            return _sqlSugarDb.Updateable(customer).ExecuteCommand();
        }

        /// <summary>
        /// Dapper: Single update
        /// Dapper: 单条更新
        /// </summary>
        [Benchmark]
        public int Dapper_SingleUpdate()
        {
            var sql = @"UPDATE BenchmarkCustomer 
                       SET CustomerName = @CustomerName, Email = @Email
                       WHERE CustomerId = @CustomerId";

            return _dapperConnection.Execute(sql, new
            {
                CustomerId = _testCustomerIds[0],
                CustomerName = "Updated Customer",
                Email = "updated@test.com"
            });
        }

        /// <summary>
        /// SqlSugar: Update specific columns
        /// SqlSugar: 更新指定列
        /// </summary>
        [Benchmark]
        public int SqlSugar_UpdateColumns()
        {
            return _sqlSugarDb.Updateable<BenchmarkCustomer>()
                .SetColumns(c => new BenchmarkCustomer
                {
                    CustomerName = "Updated Name",
                    Email = "updated@test.com"
                })
                .Where(c => c.CustomerId == _testCustomerIds[1])
                .ExecuteCommand();
        }

        /// <summary>
        /// SqlSugar: Batch update
        /// SqlSugar: 批量更新
        /// </summary>
        [Benchmark]
        public int SqlSugar_BatchUpdate()
        {
            var customers = _sqlSugarDb.Queryable<BenchmarkCustomer>()
                .Where(c => _testCustomerIds.Take(20).Contains(c.CustomerId))
                .ToList();

            foreach (var customer in customers)
            {
                customer.IsActive = false;
                customer.Email = $"batch_updated_{customer.CustomerId}@test.com";
            }

            return _sqlSugarDb.Updateable(customers).ExecuteCommand();
        }

        /// <summary>
        /// Dapper: Batch update
        /// Dapper: 批量更新
        /// </summary>
        [Benchmark]
        public int Dapper_BatchUpdate()
        {
            var sql = @"UPDATE BenchmarkCustomer 
                       SET IsActive = @IsActive, Email = @Email
                       WHERE CustomerId = @CustomerId";

            var updates = new List<object>();
            for (int i = 0; i < 20; i++)
            {
                updates.Add(new
                {
                    CustomerId = _testCustomerIds[i],
                    IsActive = false,
                    Email = $"batch_updated_{_testCustomerIds[i]}@test.com"
                });
            }

            return _dapperConnection.Execute(sql, updates);
        }

        /// <summary>
        /// SqlSugar: Update with condition
        /// SqlSugar: 条件更新
        /// </summary>
        [Benchmark]
        public int SqlSugar_UpdateWithCondition()
        {
            return _sqlSugarDb.Updateable<BenchmarkCustomer>()
                .SetColumns(c => c.IsActive == false)
                .Where(c => c.City == "TestCity")
                .ExecuteCommand();
        }

        /// <summary>
        /// Dapper: Update with condition
        /// Dapper: 条件更新
        /// </summary>
        [Benchmark]
        public int Dapper_UpdateWithCondition()
        {
            var sql = @"UPDATE BenchmarkCustomer 
                       SET IsActive = @IsActive
                       WHERE City = @City";

            return _dapperConnection.Execute(sql, new
            {
                IsActive = false,
                City = "TestCity"
            });
        }

        /// <summary>
        /// SqlSugar: Update ignore columns
        /// SqlSugar: 更新时忽略指定列
        /// </summary>
        [Benchmark]
        public int SqlSugar_UpdateIgnoreColumns()
        {
            var customer = _sqlSugarDb.Queryable<BenchmarkCustomer>()
                .InSingle(_testCustomerIds[2]);
            
            customer.CustomerName = "Updated Customer";
            customer.Email = "updated@test.com";
            customer.Phone = "999-999-9999";

            return _sqlSugarDb.Updateable(customer)
                .IgnoreColumns(c => new { c.Phone })
                .ExecuteCommand();
        }

        /// <summary>
        /// SqlSugar: Update only specified columns
        /// SqlSugar: 仅更新指定列
        /// </summary>
        [Benchmark]
        public int SqlSugar_UpdateOnlyColumns()
        {
            var customer = _sqlSugarDb.Queryable<BenchmarkCustomer>()
                .InSingle(_testCustomerIds[3]);
            
            customer.CustomerName = "Updated Customer";
            customer.Email = "updated@test.com";
            customer.Phone = "999-999-9999";

            return _sqlSugarDb.Updateable(customer)
                .UpdateColumns(c => new { c.CustomerName, c.Email })
                .ExecuteCommand();
        }
    }
}
