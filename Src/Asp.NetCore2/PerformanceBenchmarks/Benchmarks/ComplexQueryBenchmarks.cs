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
    /// Complex query benchmarks for SqlSugar
    /// SqlSugar 复杂查询基准测试
    /// </summary>
    [MemoryDiagnoser]
    [RankColumn]
    public class ComplexQueryBenchmarks
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
            _sqlSugarDb.Open();
            InsertTestData();
        }

        /// <summary>
        /// Cleanup method called after benchmarks
        /// 基准测试后调用的清理方法
        /// </summary>
        [GlobalCleanup]
        public void Cleanup()
        {
            _sqlSugarDb.Close();
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
                    City = i % 3 == 0 ? "CityA" : (i % 3 == 1 ? "CityB" : "CityC"),
                    Country = "TestCountry",
                    RegistrationDate = DateTime.Now.AddDays(-i),
                    IsActive = i % 2 == 0
                });
            }
            _sqlSugarDb.Insertable(customers).ExecuteCommand();

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
                    IsAvailable = i % 3 != 0,
                    CreatedDate = DateTime.Now.AddDays(-i)
                });
            }
            _sqlSugarDb.Insertable(products).ExecuteCommand();

            var orders = new List<BenchmarkOrder>();
            for (int i = 1; i <= 200; i++)
            {
                orders.Add(new BenchmarkOrder
                {
                    CustomerId = (i % 100) + 1,
                    OrderNumber = $"ORD{i:D6}",
                    OrderDate = DateTime.Now.AddDays(-i),
                    TotalAmount = 100.00m * i,
                    Status = i % 3 == 0 ? "Completed" : (i % 3 == 1 ? "Pending" : "Cancelled"),
                    ShippingAddress = $"{i} Shipping Street",
                    CreatedDate = DateTime.Now.AddDays(-i)
                });
            }
            _sqlSugarDb.Insertable(orders).ExecuteCommand();
        }

        /// <summary>
        /// SqlSugar: Subquery
        /// SqlSugar: 子查询
        /// </summary>
        [Benchmark]
        public List<BenchmarkCustomer> SqlSugar_Subquery()
        {
            return _sqlSugarDb.Queryable<BenchmarkCustomer>()
                .Where(c => SqlFunc.Subqueryable<BenchmarkOrder>()
                    .Where(o => o.CustomerId == c.CustomerId && o.Status == "Completed")
                    .Any())
                .ToList();
        }

        /// <summary>
        /// Dapper: Subquery
        /// Dapper: 子查询
        /// </summary>
        [Benchmark]
        public List<BenchmarkCustomer> Dapper_Subquery()
        {
            var sql = @"SELECT * FROM BenchmarkCustomer c
                       WHERE EXISTS (
                           SELECT 1 FROM BenchmarkOrder o 
                           WHERE o.CustomerId = c.CustomerId AND o.Status = @Status
                       )";

            return _dapperConnection.Query<BenchmarkCustomer>(sql, new { Status = "Completed" }).ToList();
        }

        /// <summary>
        /// SqlSugar: Group by with having
        /// SqlSugar: 分组查询带 Having
        /// </summary>
        [Benchmark]
        public List<object> SqlSugar_GroupByWithHaving()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .GroupBy(o => o.CustomerId)
                .Having(o => SqlFunc.AggregateCount(o.OrderId) > 1)
                .Select<object>(o => new
                {
                    CustomerId = o.CustomerId,
                    OrderCount = SqlFunc.AggregateCount(o.OrderId),
                    TotalAmount = SqlFunc.AggregateSum(o.TotalAmount),
                    AvgAmount = SqlFunc.AggregateAvg(o.TotalAmount)
                }) 
                .ToList();
        }

        /// <summary>
        /// Dapper: Group by with having
        /// Dapper: 分组查询带 Having
        /// </summary>
        [Benchmark]
        public List<dynamic> Dapper_GroupByWithHaving()
        {
            var sql = @"SELECT CustomerId, 
                              COUNT(OrderId) as OrderCount,
                              SUM(TotalAmount) as TotalAmount,
                              AVG(TotalAmount) as AvgAmount
                       FROM BenchmarkOrder
                       GROUP BY CustomerId
                       HAVING COUNT(OrderId) > @Count";

            return _dapperConnection.Query(sql, new { Count = 1 }).ToList();
        }

        /// <summary>
        /// SqlSugar: Union query
        /// SqlSugar: 联合查询
        /// </summary>
        [Benchmark]
        public List<object> SqlSugar_UnionQuery()
        {
            var query1 = _sqlSugarDb.Queryable<BenchmarkCustomer>()
                .Where(c => c.City == "CityA")
                .Select<object>(c => new { c.CustomerId, c.CustomerName });

            var query2 = _sqlSugarDb.Queryable<BenchmarkCustomer>()
                .Where(c => c.City == "CityB")
                .Select<object>(c => new { c.CustomerId, c.CustomerName });

            return _sqlSugarDb.UnionAll(query1, query2).ToList();
        }

        /// <summary>
        /// Dapper: Union query
        /// Dapper: 联合查询
        /// </summary>
        [Benchmark]
        public List<dynamic> Dapper_UnionQuery()
        {
            var sql = @"SELECT CustomerId, CustomerName FROM BenchmarkCustomer WHERE City = @City1
                       UNION ALL
                       SELECT CustomerId, CustomerName FROM BenchmarkCustomer WHERE City = @City2";

            return _dapperConnection.Query(sql, new { City1 = "CityA", City2 = "CityB" }).ToList();
        }

        /// <summary>
        /// SqlSugar: Include navigation properties
        /// SqlSugar: 包含导航属性
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> SqlSugar_IncludeNavigation()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .Includes(o => o.Customer)
                .Where(o => o.Status == "Completed")
                .Take(50)
                .ToList();
        }

        /// <summary>
        /// SqlSugar: Multiple includes
        /// SqlSugar: 多个导航属性
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> SqlSugar_MultipleIncludes()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .Includes(o => o.Customer)
                .Includes(o => o.OrderItems)
                .Where(o => o.Status == "Completed")
                .Take(20)
                .ToList();
        }

        /// <summary>
        /// SqlSugar: Dynamic query with expression
        /// SqlSugar: 动态表达式查询
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> SqlSugar_DynamicExpression()
        {
            var exp = Expressionable.Create<BenchmarkOrder>();
            exp.And(o => o.TotalAmount > 5000);
            exp.Or(o => o.Status == "Completed");

            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .Where(exp.ToExpression())
                .OrderByDescending(o => o.OrderDate)
                .Take(50)
                .ToList();
        }

        /// <summary>
        /// SqlSugar: Distinct query
        /// SqlSugar: 去重查询
        /// </summary>
        [Benchmark]
        public List<string> SqlSugar_DistinctQuery()
        {
            return _sqlSugarDb.Queryable<BenchmarkCustomer>()
                .Select(c => c.City)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Dapper: Distinct query
        /// Dapper: 去重查询
        /// </summary>
        [Benchmark]
        public List<string> Dapper_DistinctQuery()
        {
            var sql = "SELECT DISTINCT City FROM BenchmarkCustomer";
            return _dapperConnection.Query<string>(sql).ToList();
        }

        /// <summary>
        /// SqlSugar: Aggregate functions
        /// SqlSugar: 聚合函数
        /// </summary>
        [Benchmark]
        public object SqlSugar_AggregateFunctions()
        {
            return _sqlSugarDb.Queryable<BenchmarkOrder>().Select<object>(it => new
            {
                Count = SqlFunc.AggregateCount(it.OrderId),
                Sum = SqlFunc.AggregateSum(it.TotalAmount),
                Avg = SqlFunc.AggregateAvg(it.TotalAmount),
                Max = SqlFunc.AggregateMax(it.TotalAmount),
                Min = SqlFunc.AggregateMin(it.TotalAmount),
            }).First();
        }

        /// <summary>
        /// Dapper: Aggregate functions
        /// Dapper: 聚合函数
        /// </summary>
        [Benchmark]
        public dynamic Dapper_AggregateFunctions()
        {
            var sql = @"SELECT COUNT(*) as Count, 
                              SUM(TotalAmount) as Sum,
                              AVG(TotalAmount) as Avg,
                              MAX(TotalAmount) as Max,
                              MIN(TotalAmount) as Min
                       FROM BenchmarkOrder";

            return _dapperConnection.QueryFirst(sql);
        }

        /// <summary>
        /// SqlSugar: In query with list
        /// SqlSugar: In 查询
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> SqlSugar_InQuery()
        {
            var statuses = new List<string> { "Completed", "Pending" };
            return _sqlSugarDb.Queryable<BenchmarkOrder>()
                .Where(o => statuses.Contains(o.Status))
                .ToList();
        }

        /// <summary>
        /// Dapper: In query with list
        /// Dapper: In 查询
        /// </summary>
        [Benchmark]
        public List<BenchmarkOrder> Dapper_InQuery()
        {
            var sql = "SELECT * FROM BenchmarkOrder WHERE Status IN @Statuses";
            var statuses = new List<string> { "Completed", "Pending" };
            return _dapperConnection.Query<BenchmarkOrder>(sql, new { Statuses = statuses }).ToList();
        }
    }
}
