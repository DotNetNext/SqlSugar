using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.Data.SqlClient;
using PerformanceBenchmarks.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace PerformanceBenchmarks.Benchmarks
{
    /// <summary>
    /// Insert operation benchmarks comparing SqlSugar with other ORMs
    /// 插入操作基准测试，比较 SqlSugar 与其他 ORM
    /// </summary>
    [MemoryDiagnoser]
    [RankColumn]
    public class InsertBenchmarks
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
        /// Iteration setup - clean tables before each iteration
        /// 迭代设置 - 每次迭代前清理表
        /// </summary>
        [IterationSetup]
        public void IterationSetup()
        {
            _sqlSugarDb.DbMaintenance.TruncateTable<BenchmarkCustomer>();
            _sqlSugarDb.DbMaintenance.TruncateTable<BenchmarkProduct>();
        }

        /// <summary>
        /// SqlSugar: Single insert benchmark
        /// SqlSugar: 单条插入基准测试
        /// </summary>
        [Benchmark]
        public int SqlSugar_SingleInsert()
        {
            var customer = new BenchmarkCustomer
            {
                CustomerName = "Test Customer",
                Email = "test@test.com",
                Phone = "123-456-7890",
                Address = "123 Test Street",
                City = "TestCity",
                Country = "TestCountry",
                RegistrationDate = DateTime.Now,
                IsActive = true
            };

            return _sqlSugarDb.Insertable(customer).ExecuteReturnIdentity();
        }

        /// <summary>
        /// Dapper: Single insert benchmark
        /// Dapper: 单条插入基准测试
        /// </summary>
        [Benchmark]
        public int Dapper_SingleInsert()
        {
            var sql = @"INSERT INTO BenchmarkCustomer 
                       (CustomerName, Email, Phone, Address, City, Country, RegistrationDate, IsActive)
                       VALUES (@CustomerName, @Email, @Phone, @Address, @City, @Country, @RegistrationDate, @IsActive);
                       SELECT CAST(SCOPE_IDENTITY() as int)";

            return _dapperConnection.ExecuteScalar<int>(sql, new
            {
                CustomerName = "Test Customer",
                Email = "test@test.com",
                Phone = "123-456-7890",
                Address = "123 Test Street",
                City = "TestCity",
                Country = "TestCountry",
                RegistrationDate = DateTime.Now,
                IsActive = true
            });
        }

        /// <summary>
        /// SqlSugar: Batch insert (100 records)
        /// SqlSugar: 批量插入（100条记录）
        /// </summary>
        [Benchmark]
        public int SqlSugar_BatchInsert_100()
        {
            var customers = new List<BenchmarkCustomer>();
            for (int i = 0; i < 100; i++)
            {
                customers.Add(new BenchmarkCustomer
                {
                    CustomerName = $"Customer {i}",
                    Email = $"customer{i}@test.com",
                    Phone = $"123-456-{i:D4}",
                    Address = $"{i} Main Street",
                    City = "TestCity",
                    Country = "TestCountry",
                    RegistrationDate = DateTime.Now,
                    IsActive = i % 2 == 0
                });
            }

            return _sqlSugarDb.Insertable(customers).ExecuteCommand();
        }

        /// <summary>
        /// Dapper: Batch insert (100 records)
        /// Dapper: 批量插入（100条记录）
        /// </summary>
        [Benchmark]
        public int Dapper_BatchInsert_100()
        {
            var customers = new List<object>();
            for (int i = 0; i < 100; i++)
            {
                customers.Add(new
                {
                    CustomerName = $"Customer {i}",
                    Email = $"customer{i}@test.com",
                    Phone = $"123-456-{i:D4}",
                    Address = $"{i} Main Street",
                    City = "TestCity",
                    Country = "TestCountry",
                    RegistrationDate = DateTime.Now,
                    IsActive = i % 2 == 0
                });
            }

            var sql = @"INSERT INTO BenchmarkCustomer 
                       (CustomerName, Email, Phone, Address, City, Country, RegistrationDate, IsActive)
                       VALUES (@CustomerName, @Email, @Phone, @Address, @City, @Country, @RegistrationDate, @IsActive)";

            return _dapperConnection.Execute(sql, customers);
        }

        /// <summary>
        /// SqlSugar: Insert and return identity
        /// SqlSugar: 插入并返回自增ID
        /// </summary>
        [Benchmark]
        public int SqlSugar_InsertReturnIdentity()
        {
            var product = new BenchmarkProduct
            {
                ProductName = "Test Product",
                ProductCode = "PRD00001",
                Category = "Test Category",
                UnitPrice = 99.99m,
                StockQuantity = 100,
                Description = "Test product description",
                IsAvailable = true,
                CreatedDate = DateTime.Now
            };

            return _sqlSugarDb.Insertable(product).ExecuteReturnIdentity();
        }

        /// <summary>
        /// Dapper: Insert and return identity
        /// Dapper: 插入并返回自增ID
        /// </summary>
        [Benchmark]
        public int Dapper_InsertReturnIdentity()
        {
            var sql = @"INSERT INTO BenchmarkProduct 
                       (ProductName, ProductCode, Category, UnitPrice, StockQuantity, Description, IsAvailable, CreatedDate)
                       VALUES (@ProductName, @ProductCode, @Category, @UnitPrice, @StockQuantity, @Description, @IsAvailable, @CreatedDate);
                       SELECT CAST(SCOPE_IDENTITY() as int)";

            return _dapperConnection.ExecuteScalar<int>(sql, new
            {
                ProductName = "Test Product",
                ProductCode = "PRD00001",
                Category = "Test Category",
                UnitPrice = 99.99m,
                StockQuantity = 100,
                Description = "Test product description",
                IsAvailable = true,
                CreatedDate = DateTime.Now
            });
        }

        /// <summary>
        /// SqlSugar: Batch insert with return PKs
        /// SqlSugar: 批量插入并返回主键列表
        /// </summary>
        [Benchmark]
        public List<int> SqlSugar_BatchInsertReturnPKs()
        {
            var products = new List<BenchmarkProduct>();
            for (int i = 0; i < 50; i++)
            {
                products.Add(new BenchmarkProduct
                {
                    ProductName = $"Product {i}",
                    ProductCode = $"PRD{i:D5}",
                    Category = $"Category {i % 5}",
                    UnitPrice = 10.00m * i,
                    StockQuantity = 100,
                    Description = $"Description {i}",
                    IsAvailable = true,
                    CreatedDate = DateTime.Now
                });
            }

            return _sqlSugarDb.Insertable(products).ExecuteReturnPkList<int>();
        }

        /// <summary>
        /// SqlSugar: Insert with ignore columns
        /// SqlSugar: 插入时忽略指定列
        /// </summary>
        [Benchmark]
        public int SqlSugar_InsertIgnoreColumns()
        {
            var customer = new BenchmarkCustomer
            {
                CustomerName = "Test Customer",
                Email = "test@test.com",
                Phone = "123-456-7890",
                Address = "123 Test Street",
                City = "TestCity",
                Country = "TestCountry",
                RegistrationDate = DateTime.Now,
                IsActive = true
            };

            return _sqlSugarDb.Insertable(customer)
                .IgnoreColumns(c => new { c.Email, c.Phone })
                .ExecuteCommand();
        }
    }
}
