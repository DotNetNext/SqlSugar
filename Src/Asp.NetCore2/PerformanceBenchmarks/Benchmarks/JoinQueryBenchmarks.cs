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
    /// Join query benchmarks comparing SqlSugar with other ORMs
    /// 联接查询基准测试，比较 SqlSugar 与其他 ORM
    /// </summary>
    [MemoryDiagnoser]
    [RankColumn]
    public class JoinQueryBenchmarks
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
            // Insert customers
            // 插入客户
            var customers = new List<BenchmarkCustomer>();
            for (int i = 1; i <= 50; i++)
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
            _sqlSugarDb.Insertable(customers).ExecuteCommand();

            // Insert products
            // 插入产品
            var products = new List<BenchmarkProduct>();
            for (int i = 1; i <= 30; i++)
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
                    CreatedDate = DateTime.Now.AddDays(-i)
                });
            }
            _sqlSugarDb.Insertable(products).ExecuteCommand();

            // Insert orders
            // 插入订单
            var orders = new List<BenchmarkOrder>();
            for (int i = 1; i <= 100; i++)
            {
                orders.Add(new BenchmarkOrder
                {
                    CustomerId = (i % 50) + 1,
                    OrderNumber = $"ORD{i:D6}",
                    OrderDate = DateTime.Now.AddDays(-i),
                    TotalAmount = 100.00m * i,
                    Status = i % 3 == 0 ? "Completed" : "Pending",
                    ShippingAddress = $"{i} Shipping Street",
                    CreatedDate = DateTime.Now.AddDays(-i)
                });
            }
            var orderIds = _sqlSugarDb.Insertable(orders).ExecuteReturnPkList<int>();

            // Insert order items
            // 插入订单项
            var orderItems = new List<BenchmarkOrderItem>();
            foreach (var orderId in orderIds)
            {
                for (int j = 1; j <= 2; j++)
                {
                    orderItems.Add(new BenchmarkOrderItem
                    {
                        OrderId = orderId,
                        ProductId = (j % 30) + 1,
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
        /// SqlSugar: Simple inner join
        /// SqlSugar: 简单内联接
        /// </summary>
        [Benchmark]
        public List<object> SqlSugar_InnerJoin()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder, BenchmarkCustomer>(
                (o, c) => o.CustomerId == c.CustomerId)
                .Select<object>((o, c) => new
                {
                    o.OrderId,
                    o.OrderNumber,
                    o.TotalAmount,
                    CustomerName = c.CustomerName,
                    CustomerEmail = c.Email
                }) 
                .ToList();
        }

        /// <summary>
        /// Dapper: Simple inner join
        /// Dapper: 简单内联接
        /// </summary>
        [Benchmark]
        public List<dynamic> Dapper_InnerJoin()
        {
            var sql = @"SELECT o.OrderId, o.OrderNumber, o.TotalAmount, 
                              c.CustomerName, c.Email as CustomerEmail
                       FROM BenchmarkOrder o
                       INNER JOIN BenchmarkCustomer c ON o.CustomerId = c.CustomerId";

            return _dapperConnection.Query(sql).ToList();
        }

        /// <summary>
        /// SqlSugar: Left join
        /// SqlSugar: 左联接
        /// </summary>
        [Benchmark]
        public List<object> SqlSugar_LeftJoin()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .LeftJoin<BenchmarkCustomer>((o, c) => o.CustomerId == c.CustomerId)
                .Select<object>((o, c) => new
                {
                    o.OrderId,
                    o.OrderNumber,
                    CustomerName = c.CustomerName
                }) 
                .ToList();
        }

        /// <summary>
        /// Dapper: Left join
        /// Dapper: 左联接
        /// </summary>
        [Benchmark]
        public List<dynamic> Dapper_LeftJoin()
        {
            var sql = @"SELECT o.OrderId, o.OrderNumber, c.CustomerName
                       FROM BenchmarkOrder o
                       LEFT JOIN BenchmarkCustomer c ON o.CustomerId = c.CustomerId";

            return _dapperConnection.Query(sql).ToList();
        }

        /// <summary>
        /// SqlSugar: Multiple joins
        /// SqlSugar: 多表联接
        /// </summary>
        [Benchmark]
        public List<object> SqlSugar_MultipleJoins()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder, BenchmarkCustomer, BenchmarkOrderItem, BenchmarkProduct>(
                (o, c, oi, p) => new JoinQueryInfos(
                    JoinType.Left, o.CustomerId == c.CustomerId,
                    JoinType.Left, o.OrderId == oi.OrderId,
                    JoinType.Left, oi.ProductId == p.ProductId
                ))
                .Select<object>((o, c, oi, p) => new
                {
                    o.OrderId,
                    o.OrderNumber,
                    CustomerName = c.CustomerName,
                    ProductName = p.ProductName,
                    oi.Quantity,
                    oi.TotalPrice
                }) 
                .ToList();
        }

        /// <summary>
        /// Dapper: Multiple joins
        /// Dapper: 多表联接
        /// </summary>
        [Benchmark]
        public List<dynamic> Dapper_MultipleJoins()
        {
            var sql = @"SELECT o.OrderId, o.OrderNumber, c.CustomerName, 
                              p.ProductName, oi.Quantity, oi.TotalPrice
                       FROM BenchmarkOrder o
                       LEFT JOIN BenchmarkCustomer c ON o.CustomerId = c.CustomerId
                       LEFT JOIN BenchmarkOrderItem oi ON o.OrderId = oi.OrderId
                       LEFT JOIN BenchmarkProduct p ON oi.ProductId = p.ProductId";

            return _dapperConnection.Query(sql).ToList();
        }

        /// <summary>
        /// SqlSugar: Join with where condition
        /// SqlSugar: 带条件的联接查询
        /// </summary>
        [Benchmark]
        public List<object> SqlSugar_JoinWithWhere()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder, BenchmarkCustomer>(
                (o, c) => o.CustomerId == c.CustomerId)
                .Where((o, c) => o.Status == "Completed" && c.IsActive == true)
                .Select<object>((o, c) => new
                {
                    o.OrderId,
                    o.OrderNumber,
                    CustomerName = c.CustomerName
                }) 
                .ToList();
        }

        /// <summary>
        /// Dapper: Join with where condition
        /// Dapper: 带条件的联接查询
        /// </summary>
        [Benchmark]
        public List<dynamic> Dapper_JoinWithWhere()
        {
            var sql = @"SELECT o.OrderId, o.OrderNumber, c.CustomerName
                       FROM BenchmarkOrder o
                       INNER JOIN BenchmarkCustomer c ON o.CustomerId = c.CustomerId
                       WHERE o.Status = @Status AND c.IsActive = @IsActive";

            return _dapperConnection.Query(sql, new { Status = "Completed", IsActive = true }).ToList();
        }

        /// <summary>
        /// SqlSugar: Join with group by
        /// SqlSugar: 带分组的联接查询
        /// </summary>
        [Benchmark]
        public List<object> SqlSugar_JoinWithGroupBy()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder, BenchmarkCustomer>(
                (o, c) => o.CustomerId == c.CustomerId)
                .GroupBy((o, c) => c.CustomerId)
                .Select<object>((o, c) => new
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    OrderCount = SqlFunc.AggregateCount(o.OrderId),
                    TotalAmount = SqlFunc.AggregateSum(o.TotalAmount)
                }) 
                .ToList();
        }

        /// <summary>
        /// Dapper: Join with group by
        /// Dapper: 带分组的联接查询
        /// </summary>
        [Benchmark]
        public List<dynamic> Dapper_JoinWithGroupBy()
        {
            var sql = @"SELECT c.CustomerId, c.CustomerName, 
                              COUNT(o.OrderId) as OrderCount,
                              SUM(o.TotalAmount) as TotalAmount
                       FROM BenchmarkCustomer c
                       INNER JOIN BenchmarkOrder o ON c.CustomerId = o.CustomerId
                       GROUP BY c.CustomerId, c.CustomerName";

            return _dapperConnection.Query(sql).ToList();
        }
    }
}
