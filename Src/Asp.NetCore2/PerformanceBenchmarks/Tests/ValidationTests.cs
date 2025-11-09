using PerformanceBenchmarks.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceBenchmarks.Tests
{
    /// <summary>
    /// Validation tests to ensure all benchmark components work correctly
    /// 验证测试以确保所有基准测试组件正常工作
    /// </summary>
    /// <remarks>
    /// This class provides quick validation tests that can be run before executing
    /// the full benchmark suite to ensure everything is configured correctly.
    /// 此类提供快速验证测试，可在执行完整基准测试套件之前运行，以确保所有配置正确。
    /// </remarks>
    public class ValidationTests
    {
        /// <summary>
        /// Run all validation tests
        /// 运行所有验证测试
        /// </summary>
        /// <returns>True if all tests pass / 如果所有测试通过则返回 true</returns>
        public static bool RunAll()
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("Running Validation Tests");
            Console.WriteLine("运行验证测试");
            Console.WriteLine("===========================================");
            Console.WriteLine();

            var allPassed = true;

            // Test 1: Database Connection
            // 测试 1: 数据库连接
            allPassed &= TestDatabaseConnection();

            // Test 2: Entity Creation
            // 测试 2: 实体创建
            allPassed &= TestEntityCreation();

            // Test 3: Basic CRUD Operations
            // 测试 3: 基本 CRUD 操作
            allPassed &= TestBasicCrudOperations();

            // Test 4: Bulk Operations
            // 测试 4: 批量操作
            allPassed &= TestBulkOperations();

            // Test 5: Join Queries
            // 测试 5: 联接查询
            allPassed &= TestJoinQueries();

            // Test 6: Navigation Properties
            // 测试 6: 导航属性
            allPassed &= TestNavigationProperties();

            Console.WriteLine();
            Console.WriteLine("===========================================");
            if (allPassed)
            {
                Console.WriteLine("✓ All validation tests PASSED!");
                Console.WriteLine("✓ 所有验证测试通过！");
            }
            else
            {
                Console.WriteLine("✗ Some validation tests FAILED!");
                Console.WriteLine("✗ 某些验证测试失败！");
            }
            Console.WriteLine("===========================================");

            return allPassed;
        }

        /// <summary>
        /// Test database connection and initialization
        /// 测试数据库连接和初始化
        /// </summary>
        private static bool TestDatabaseConnection()
        {
            Console.Write("Test 1: Database Connection... ");
            try
            {
                var db = BenchmarkConfig.GetSqlServerDb();
                
                // Try to create database if not exists
                // 尝试创建数据库（如果不存在）
                db.DbMaintenance.CreateDatabase();
                
                // Initialize tables
                // 初始化表
                BenchmarkConfig.InitializeDatabase();
                
                Console.WriteLine("✓ PASSED");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAILED: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test entity creation and table initialization
        /// 测试实体创建和表初始化
        /// </summary>
        private static bool TestEntityCreation()
        {
            Console.Write("Test 2: Entity Creation... ");
            try
            {
                var db = BenchmarkConfig.GetSqlServerDb();
                
                // Verify all tables exist
                // 验证所有表是否存在
                var customerTableExists = db.DbMaintenance.IsAnyTable("BenchmarkCustomer", false);
                var productTableExists = db.DbMaintenance.IsAnyTable("BenchmarkProduct", false);
                var orderTableExists = db.DbMaintenance.IsAnyTable("BenchmarkOrder", false);
                var orderItemTableExists = db.DbMaintenance.IsAnyTable("BenchmarkOrderItem", false);

                if (customerTableExists && productTableExists && orderTableExists && orderItemTableExists)
                {
                    Console.WriteLine("✓ PASSED");
                    return true;
                }
                else
                {
                    Console.WriteLine("✗ FAILED: Some tables do not exist");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAILED: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test basic CRUD operations (Create, Read, Update, Delete)
        /// 测试基本 CRUD 操作（创建、读取、更新、删除）
        /// </summary>
        private static bool TestBasicCrudOperations()
        {
            Console.Write("Test 3: Basic CRUD Operations... ");
            try
            {
                var db = BenchmarkConfig.GetSqlServerDb();
                
                // Clean up first
                // 首先清理
                db.DbMaintenance.TruncateTable<BenchmarkCustomer>();

                // CREATE: Insert a customer
                // 创建：插入客户
                var customer = new BenchmarkCustomer
                {
                    CustomerName = "Test Customer",
                    Email = "test@test.com",
                    Phone = "123-456-7890",
                    Address = "123 Test St",
                    City = "TestCity",
                    Country = "TestCountry",
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };
                var customerId = db.Insertable(customer).ExecuteReturnIdentity();

                // READ: Query the customer
                // 读取：查询客户
                var retrievedCustomer = db.Queryable<BenchmarkCustomer>().InSingle(customerId);
                if (retrievedCustomer == null || retrievedCustomer.CustomerName != "Test Customer")
                {
                    Console.WriteLine("✗ FAILED: Read operation failed");
                    return false;
                }

                // UPDATE: Update the customer
                // 更新：更新客户
                retrievedCustomer.CustomerName = "Updated Customer";
                var updateResult = db.Updateable(retrievedCustomer).ExecuteCommand();
                if (updateResult <= 0)
                {
                    Console.WriteLine("✗ FAILED: Update operation failed");
                    return false;
                }

                // Verify update
                // 验证更新
                var updatedCustomer = db.Queryable<BenchmarkCustomer>().InSingle(customerId);
                if (updatedCustomer.CustomerName != "Updated Customer")
                {
                    Console.WriteLine("✗ FAILED: Update verification failed");
                    return false;
                }

                // DELETE: Delete the customer
                // 删除：删除客户
                var deleteResult = db.Deleteable<BenchmarkCustomer>().In(customerId).ExecuteCommand();
                if (deleteResult <= 0)
                {
                    Console.WriteLine("✗ FAILED: Delete operation failed");
                    return false;
                }

                // Verify deletion
                // 验证删除
                var deletedCustomer = db.Queryable<BenchmarkCustomer>().InSingle(customerId);
                if (deletedCustomer != null)
                {
                    Console.WriteLine("✗ FAILED: Delete verification failed");
                    return false;
                }

                Console.WriteLine("✓ PASSED");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAILED: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test bulk operations (BulkCopy, BulkUpdate, etc.)
        /// 测试批量操作（BulkCopy、BulkUpdate 等）
        /// </summary>
        private static bool TestBulkOperations()
        {
            Console.Write("Test 4: Bulk Operations... ");
            try
            {
                var db = BenchmarkConfig.GetSqlServerDb();
                
                // Clean up first
                // 首先清理
                db.DbMaintenance.TruncateTable<BenchmarkProduct>();

                // Create test data
                // 创建测试数据
                var products = new List<BenchmarkProduct>();
                for (int i = 1; i <= 100; i++)
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

                // Test BulkCopy
                // 测试 BulkCopy
                var bulkCopyResult = db.Fastest<BenchmarkProduct>().BulkCopy(products);
                if (bulkCopyResult <= 0)
                {
                    Console.WriteLine("✗ FAILED: BulkCopy operation failed");
                    return false;
                }

                // Verify count
                // 验证数量
                var count = db.Queryable<BenchmarkProduct>().Count();
                if (count != 100)
                {
                    Console.WriteLine($"✗ FAILED: Expected 100 records, got {count}");
                    return false;
                }

                // Test batch insert
                // 测试批量插入
                db.DbMaintenance.TruncateTable<BenchmarkProduct>();
                var batchInsertResult = db.Insertable(products).ExecuteCommand();
                if (batchInsertResult <= 0)
                {
                    Console.WriteLine("✗ FAILED: Batch insert operation failed");
                    return false;
                }

                Console.WriteLine("✓ PASSED");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAILED: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test join queries
        /// 测试联接查询
        /// </summary>
        private static bool TestJoinQueries()
        {
            Console.Write("Test 5: Join Queries... ");
            try
            {
                var db = BenchmarkConfig.GetSqlServerDb();
                
                // Clean up
                // 清理
                db.DbMaintenance.TruncateTable<BenchmarkCustomer>();
                db.DbMaintenance.TruncateTable<BenchmarkOrder>();

                // Insert test data
                // 插入测试数据
                var customer = new BenchmarkCustomer
                {
                    CustomerName = "Join Test Customer",
                    Email = "join@test.com",
                    Phone = "123-456-7890",
                    Address = "123 Join St",
                    City = "JoinCity",
                    Country = "JoinCountry",
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };
                var customerId = db.Insertable(customer).ExecuteReturnIdentity();

                var order = new BenchmarkOrder
                {
                    CustomerId = customerId,
                    OrderNumber = "ORD001",
                    OrderDate = DateTime.Now,
                    TotalAmount = 1000.00m,
                    Status = "Completed",
                    ShippingAddress = "123 Ship St",
                    CreatedDate = DateTime.Now
                };
                db.Insertable(order).ExecuteCommand();

                // Test inner join
                // 测试内联接
                var joinResult = db.Queryable<BenchmarkOrder, BenchmarkCustomer>(
                    (o, c) => o.CustomerId == c.CustomerId)
                    .Select((o, c) => new
                    {
                        o.OrderNumber,
                        CustomerName = c.CustomerName
                    })
                    .ToList();

                if (joinResult.Count == 0)
                {
                    Console.WriteLine("✗ FAILED: Join query returned no results");
                    return false;
                }

                Console.WriteLine("✓ PASSED");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAILED: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test navigation properties
        /// 测试导航属性
        /// </summary>
        private static bool TestNavigationProperties()
        {
            Console.Write("Test 6: Navigation Properties... ");
            try
            {
                var db = BenchmarkConfig.GetSqlServerDb();
                
                // Clean up
                // 清理
                db.DbMaintenance.TruncateTable<BenchmarkCustomer>();
                db.DbMaintenance.TruncateTable<BenchmarkOrder>();

                // Insert test data
                // 插入测试数据
                var customer = new BenchmarkCustomer
                {
                    CustomerName = "Nav Test Customer",
                    Email = "nav@test.com",
                    Phone = "123-456-7890",
                    Address = "123 Nav St",
                    City = "NavCity",
                    Country = "NavCountry",
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };
                var customerId = db.Insertable(customer).ExecuteReturnIdentity();

                var order = new BenchmarkOrder
                {
                    CustomerId = customerId,
                    OrderNumber = "NAV001",
                    OrderDate = DateTime.Now,
                    TotalAmount = 500.00m,
                    Status = "Pending",
                    ShippingAddress = "123 Nav Ship St",
                    CreatedDate = DateTime.Now
                };
                db.Insertable(order).ExecuteCommand();

                // Test Includes (navigation properties)
                // 测试 Includes（导航属性）
                var orders = db.Queryable<BenchmarkOrder>()
                    .Includes(o => o.Customer)
                    .Where(o => o.CustomerId == customerId)
                    .ToList();

                if (orders.Count == 0)
                {
                    Console.WriteLine("✗ FAILED: No orders found");
                    return false;
                }

                if (orders[0].Customer == null)
                {
                    Console.WriteLine("✗ FAILED: Navigation property not loaded");
                    return false;
                }

                if (orders[0].Customer.CustomerName != "Nav Test Customer")
                {
                    Console.WriteLine("✗ FAILED: Navigation property data incorrect");
                    return false;
                }

                Console.WriteLine("✓ PASSED");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAILED: {ex.Message}");
                return false;
            }
        }
    }
}
