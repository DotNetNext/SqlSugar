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
    /// Query operation benchmarks comparing SqlSugar with other ORMs
    /// 查询操作基准测试，比较 SqlSugar 与其他 ORM
    /// </summary>
    [MemoryDiagnoser]
    [RankColumn]
    public class QueryBenchmarks
    {
        private SqlSugarClient _sqlSugarDb;
        private SqlConnection _dapperConnection;
        private List<int> _testOrderIds;

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

            // Insert products
            // 插入产品
            var products = new List<BenchmarkProduct>();
            for (int i = 1; i <= 50; i++)
            {
                products.Add(new BenchmarkProduct
                {
                    ProductName = $"Product {i}",
                    ProductCode = $"PRD{i:D5}",
                    Category = $"Category {i % 5}",
                    UnitPrice = 10.00m * i,
                    StockQuantity = 100 * i,
                    Description = $"Description for product {i}",
                    IsAvailable = true,
                    CreatedDate = DateTime.Now.AddDays(-i)
                });
            }
            _sqlSugarDb.Insertable(products).ExecuteCommand();

            // Insert orders
            // 插入订单
            var orders = new List<BenchmarkOrder>();
            _testOrderIds = new List<int>();
            for (int i = 1; i <= 200; i++)
            {
                orders.Add(new BenchmarkOrder
                {
                    CustomerId = (i % 100) + 1,
                    OrderNumber = $"ORD{i:D6}",
                    OrderDate = DateTime.Now.AddDays(-i),
                    TotalAmount = 100.00m * i,
                    Status = i % 3 == 0 ? "Completed" : "Pending",
                    ShippingAddress = $"{i} Shipping Street",
                    CreatedDate = DateTime.Now.AddDays(-i)
                });
            }
            _testOrderIds = _sqlSugarDb.Insertable(orders).ExecuteReturnPkList<int>();

            // Insert order items
            // 插入订单项
            var orderItems = new List<BenchmarkOrderItem>();
            foreach (var orderId in _testOrderIds)
            {
                for (int j = 1; j <= 3; j++)
                {
                    orderItems.Add(new BenchmarkOrderItem
                    {
                        OrderId = orderId,
                        ProductId = (j % 50) + 1,
                        Quantity = j,
                        UnitPrice = 10.00m * j,
                        Discount = 0.1m,
                        TotalPrice = 10.00m * j * (1 - 0.1m)
                    });
                }
            }
            _sqlSugarDb.Insertable(orderItems).ExecuteCommand();
        }

        /// <summary>
        /// SqlSugar: Simple query benchmark
        /// SqlSugar: 简单查询基准测试
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> SqlSugar_SimpleQuery()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .Where(o => o.Status == "Completed")
                .ToList();
        }

        /// <summary>
        /// Dapper: Simple query benchmark
        /// Dapper: 简单查询基准测试
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> Dapper_SimpleQuery()
        {
            return _dapperConnection.Query<BenchmarkOrder>(
                "SELECT * FROM BenchmarkOrder WHERE Status = @Status",
                new { Status = "Completed" }
            ).ToList();
        }

        /// <summary>
        /// SqlSugar: Query with pagination
        /// SqlSugar: 分页查询
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> SqlSugar_PaginationQuery()
        {
            int pageIndex = 1;
            int pageSize = 20; 
            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .OrderBy(o => o.OrderId)
                .ToPageList(pageIndex, pageSize);
        }

        /// <summary>
        /// Dapper: Query with pagination
        /// Dapper: 分页查询
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> Dapper_PaginationQuery()
        {
            return _dapperConnection.Query<BenchmarkOrder>(
                "SELECT * FROM BenchmarkOrder ORDER BY OrderId OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY"
            ).ToList();
        }

        /// <summary>
        /// SqlSugar: Query with complex conditions
        /// SqlSugar: 复杂条件查询
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> SqlSugar_ComplexQuery()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .Where(o => o.Status == "Completed" && o.TotalAmount > 5000)
                .OrderByDescending(o => o.OrderDate)
                .Take(50)
                .ToList();
        }

        /// <summary>
        /// Dapper: Query with complex conditions
        /// Dapper: 复杂条件查询
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> Dapper_ComplexQuery()
        {
            return _dapperConnection.Query<BenchmarkOrder>(
                @"SELECT TOP 50 * FROM BenchmarkOrder 
                  WHERE Status = @Status AND TotalAmount > @Amount
                  ORDER BY OrderDate DESC",
                new { Status = "Completed", Amount = 5000m }
            ).ToList();
        }

        /// <summary>
        /// SqlSugar: Query single record by ID
        /// SqlSugar: 按 ID 查询单条记录
        /// </summary>
        [Benchmark]
        public BenchmarkOrder SqlSugar_QueryById()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .InSingle(_testOrderIds[0]);
        }

        /// <summary>
        /// Dapper: Query single record by ID
        /// Dapper: 按 ID 查询单条记录
        /// </summary>
        [Benchmark]
        public BenchmarkOrder Dapper_QueryById()
        {
            return _dapperConnection.QueryFirstOrDefault<BenchmarkOrder>(
                "SELECT * FROM BenchmarkOrder WHERE OrderId = @OrderId",
                new { OrderId = _testOrderIds[0] }
            );
        }

        /// <summary>
        /// SqlSugar: Query with dynamic conditions
        /// SqlSugar: 动态条件查询
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> SqlSugar_DynamicQuery()
        {
            var exp = Expressionable.Create<BenchmarkOrder>();
            exp.And(o => o.Status == "Completed");
            exp.And(o => o.TotalAmount > 1000);
            
            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .Where(exp.ToExpression())
                .ToList();
        }

        /// <summary>
        /// SqlSugar: Query with select specific columns
        /// SqlSugar: 查询指定列
        /// </summary>
        [Benchmark]
        public List<object> SqlSugar_SelectColumns()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .Select<object>(o => new
                {
                    o.OrderId,
                    o.OrderNumber,
                    o.TotalAmount
                }) 
                .ToList();
        }

        /// <summary>
        /// Dapper: Query with select specific columns
        /// Dapper: 查询指定列
        /// </summary>
        [Benchmark]
        public List<dynamic> Dapper_SelectColumns()
        {
            return _dapperConnection.Query(
                "SELECT OrderId, OrderNumber, TotalAmount FROM BenchmarkOrder"
            ).ToList();
        }
    }
}
