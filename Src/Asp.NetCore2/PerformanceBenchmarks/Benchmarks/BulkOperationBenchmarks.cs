using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.Data.SqlClient;
using PerformanceBenchmarks.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;

namespace PerformanceBenchmarks.Benchmarks
{
    /// <summary>
    /// Bulk operation benchmarks for SqlSugar
    /// SqlSugar 批量操作基准测试
    /// </summary>
    [MemoryDiagnoser]
    [RankColumn]
    public class BulkOperationBenchmarks
    {
        private SqlSugarClient _sqlSugarDb;
        private SqlConnection _dapperConnection;
        private List<BenchmarkCustomer> _testCustomers;
        private List<BenchmarkProduct> _testProducts;

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

            PrepareTestData();
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
        /// Prepare test data
        /// 准备测试数据
        /// </summary>
        private void PrepareTestData()
        {
            _testCustomers = new List<BenchmarkCustomer>();
            for (int i = 1; i <= 1000; i++)
            {
                _testCustomers.Add(new BenchmarkCustomer
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

            _testProducts = new List<BenchmarkProduct>();
            for (int i = 1; i <= 1000; i++)
            {
                _testProducts.Add(new BenchmarkProduct
                {
                    ProductName = $"Product {i}",
                    ProductCode = $"PRD{i:D5}",
                    Category = $"Category {i % 10}",
                    UnitPrice = 10.00m * i,
                    StockQuantity = 100 * i,
                    Description = $"Description for product {i}",
                    IsAvailable = true,
                    CreatedDate = DateTime.Now.AddDays(-i)
                });
            }
        }

        /// <summary>
        /// SqlSugar: BulkCopy insert 1000 records
        /// SqlSugar: BulkCopy 插入 1000 条记录
        /// </summary>
        [Benchmark]
        public int SqlSugar_BulkCopy_1000()
        {
            return _sqlSugarDb.Fastest<BenchmarkCustomer>()
                .BulkCopy(_testCustomers);
        }

        /// <summary>
        /// SqlSugar: Regular batch insert 1000 records
        /// SqlSugar: 常规批量插入 1000 条记录
        /// </summary>
        [Benchmark]
        public int SqlSugar_BatchInsert_1000()
        {
            return _sqlSugarDb.Insertable(_testCustomers).ExecuteCommand();
        }

        /// <summary>
        /// Dapper: Batch insert 1000 records
        /// Dapper: 批量插入 1000 条记录
        /// </summary>
        [Benchmark]
        public int Dapper_BatchInsert_1000()
        {
            var sql = @"INSERT INTO BenchmarkCustomer 
                       (CustomerName, Email, Phone, Address, City, Country, RegistrationDate, IsActive)
                       VALUES (@CustomerName, @Email, @Phone, @Address, @City, @Country, @RegistrationDate, @IsActive)";

            return _dapperConnection.Execute(sql, _testCustomers);
        }

        /// <summary>
        /// SqlSugar: BulkCopy with page size
        /// SqlSugar: 分页 BulkCopy
        /// </summary>
        [Benchmark]
        public int SqlSugar_BulkCopy_PageSize()
        {
            return _sqlSugarDb.Fastest<BenchmarkProduct>()
                .PageSize(500)
                .BulkCopy(_testProducts);
        }

        /// <summary>
        /// SqlSugar: BulkUpdate 1000 records
        /// SqlSugar: 批量更新 1000 条记录
        /// </summary>
        [Benchmark]
        public int SqlSugar_BulkUpdate_1000()
        {
            // First insert data
            // 首先插入数据
            _sqlSugarDb.Fastest<BenchmarkCustomer>().BulkCopy(_testCustomers);

            // Modify data
            // 修改数据
            var customers = _sqlSugarDb.Queryable<BenchmarkCustomer>().ToList();
            foreach (var customer in customers)
            {
                customer.Email = $"updated_{customer.CustomerId}@test.com";
                customer.IsActive = !customer.IsActive;
            }

            // Bulk update
            // 批量更新
            return _sqlSugarDb.Fastest<BenchmarkCustomer>().BulkUpdate(customers);
        }

        /// <summary>
        /// SqlSugar: Storageable (Insert or Update) 1000 records
        /// SqlSugar: 存储（插入或更新）1000 条记录
        /// </summary>
        [Benchmark]
        public int SqlSugar_Storageable_1000()
        {
            return _sqlSugarDb.Storageable(_testCustomers).ExecuteCommand();
        }

        /// <summary>
        /// SqlSugar: BulkMerge 1000 records
        /// SqlSugar: 批量合并 1000 条记录
        /// </summary>
        [Benchmark]
        public int SqlSugar_BulkMerge_1000()
        {
            return _sqlSugarDb.Fastest<BenchmarkProduct>().BulkMerge(_testProducts);
        }

        /// <summary>
        /// SqlSugar: Batch delete 1000 records
        /// SqlSugar: 批量删除 1000 条记录
        /// </summary>
        [Benchmark]
        public int SqlSugar_BatchDelete_1000()
        {
            // First insert data
            // 首先插入数据
            _sqlSugarDb.Fastest<BenchmarkCustomer>().BulkCopy(_testCustomers);

            // Get all customers
            // 获取所有客户
            var customers = _sqlSugarDb.Queryable<BenchmarkCustomer>().ToList();

            // Batch delete
            // 批量删除
            return _sqlSugarDb.Deleteable(customers).ExecuteCommand();
        }

        /// <summary>
        /// SqlSugar: BulkCopy with DataTable
        /// SqlSugar: 使用 DataTable 的 BulkCopy
        /// </summary>
        [Benchmark]
        public int SqlSugar_BulkCopy_DataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("CustomerName", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("City", typeof(string));
            dt.Columns.Add("Country", typeof(string));
            dt.Columns.Add("RegistrationDate", typeof(DateTime));
            dt.Columns.Add("IsActive", typeof(bool));

            foreach (var customer in _testCustomers)
            {
                dt.Rows.Add(
                    customer.CustomerName,
                    customer.Email,
                    customer.Phone,
                    customer.Address,
                    customer.City,
                    customer.Country,
                    customer.RegistrationDate,
                    customer.IsActive
                );
            }

            return _sqlSugarDb.Fastest<DataTable>()
                .AS("BenchmarkCustomer")
                .BulkCopy(dt);
        }
    }
}
