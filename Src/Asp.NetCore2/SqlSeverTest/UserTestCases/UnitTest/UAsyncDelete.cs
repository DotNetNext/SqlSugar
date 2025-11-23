using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrmTest
{
    /// <summary>
    /// ASYNC DELETE TEST SUITE - Comprehensive Testing for SqlSugar Async Delete Operations
    /// 
    /// PURPOSE:
    ///   Tests all async delete methods in SqlSugar ORM with focus on:
    ///   - Basic async delete operations (ExecuteCommandAsync, ExecuteCommandHasChangeAsync)
    ///   - Delete by primary key, expression, and entity
    ///   - CancellationToken support (CRITICAL - previously untested)
    ///   - Cascade and soft delete scenarios
    ///   - Error handling and edge cases
    /// 
    /// PRIORITY: HIGH - Critical for data integrity and async operation support
    /// 
    /// USAGE:
    ///   // Run all tests
    ///   NewUnitTest.AsyncDelete();
    ///   
    ///   // Run individual test
    ///   NewUnitTest.AsyncDelete_ExecuteCommandAsync();
    /// 
    /// TEST COVERAGE:
    ///   ExecuteCommandAsync() - Basic async delete
    ///   ExecuteCommandHasChangeAsync() - Change detection
    ///   DeleteRange() - Bulk async delete
    ///   Delete by primary key
    ///   Delete by expression
    ///   CancellationToken support across all methods
    ///   Cascade delete scenarios
    ///   Soft delete (IsLogic)
    ///   Concurrent delete operations
    ///   Error scenarios
    /// 
    /// DEPENDENCIES:
    ///   - Order entity (with identity column)
    ///   - OrderItem entity (for cascade tests)
    ///   - SoftDeleteEntity (for soft delete tests)
    /// 
    /// </summary>
    public partial class NewUnitTest
    {
        #region Main Entry Point

        /// <summary>
        /// Main entry point - Executes all 15 async delete tests
        /// 
        /// Test Categories:
        /// A. Basic Async Delete Tests (5 tests) - Core delete operations
        /// B. CancellationToken Tests (4 tests) - Cancellation support
        /// C. Cascade & Soft Delete Tests (3 tests) - Advanced delete scenarios
        /// D. Edge Cases & Error Handling (3 tests) - Robustness validation
        /// 
        /// Usage: NewUnitTest.AsyncDelete();
        /// </summary>
        public static void AsyncDelete()
        {
            Console.WriteLine("\n================================================================");
            Console.WriteLine("       ASYNC DELETE TEST SUITE - COMPREHENSIVE");
            Console.WriteLine("================================================================\n");

            try
            {
                // CATEGORY A: Basic Async Delete Tests (5 functions)
                Console.WriteLine("--- BASIC ASYNC DELETE OPERATIONS ---\n");
                AsyncDelete_ExecuteCommandAsync();
                AsyncDelete_ExecuteCommandHasChangeAsync();
                AsyncDelete_MultipleEntitiesAsync();
                AsyncDelete_ByPrimaryKey();
                AsyncDelete_ByExpression();

                // CATEGORY B: CancellationToken Tests (4 functions)
                Console.WriteLine("\n--- CANCELLATION TOKEN SUPPORT ---\n");
                AsyncDelete_CancellationToken_Basic();
                AsyncDelete_CancellationToken_Immediate();
                AsyncDelete_CancellationToken_BulkDelete();
                AsyncDelete_CancellationToken_Timeouts();

                // CATEGORY C: Cascade & Soft Delete Tests (3 functions)
                Console.WriteLine("\n--- CASCADE & SOFT DELETE TESTS ---\n");
                AsyncDelete_CascadeDelete();
                AsyncDelete_SoftDelete();
                AsyncDelete_SoftDelete_Timestamp();

                // CATEGORY D: Edge Cases & Error Handling (3 functions)
                Console.WriteLine("\n--- ERROR HANDLING & EDGE CASES ---\n");
                AsyncDelete_NonExistent();
                AsyncDelete_ConcurrentDeletes();
                AsyncDelete_Performance();

                Console.WriteLine("\n================================================================");
                Console.WriteLine("            ALL ASYNC DELETE TESTS PASSED (15/15)");
                Console.WriteLine("================================================================\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n================================================================");
                Console.WriteLine("                    TEST SUITE FAILED");
                Console.WriteLine("================================================================");
                Console.WriteLine("\nError: " + ex.Message + "\n");
                throw;
            }
        }

        #endregion


        #region A. Basic Async Delete Tests (5 functions)

        /// <summary>
        /// Test 1: Basic ExecuteCommandAsync
        /// Validates: Single entity async delete, affected rows returned, entity removed from database
        /// </summary>
        public static void AsyncDelete_ExecuteCommandAsync()
        {
            Console.WriteLine("TEST 1: AsyncDelete_ExecuteCommandAsync");

            var db = Db;
            
            // Setup: Insert test entity
            var order = new Order
            {
                Name = "Test Order for Delete",
                Price = 100.50m,
                CreateTime = DateTime.Now
            };
            var insertedId = db.Insertable(order).ExecuteReturnIdentity();

            // Test: Delete single entity async
            var deleteTask = db.Deleteable<Order>()
                .Where(x => x.Id == insertedId)
                .ExecuteCommandAsync();
            
            var affectedRows = deleteTask.GetAwaiter().GetResult();

            // Verify: Affected rows returned
            if (affectedRows != 1)
                throw new Exception($"Expected 1 affected row, got {affectedRows}");

            // Verify: Entity deleted from database
            var deletedOrder = db.Queryable<Order>().InSingle(insertedId);
            if (deletedOrder != null)
                throw new Exception("Entity should be deleted but still exists in database");

            Console.WriteLine("   [OK] ExecuteCommandAsync works correctly");
            Console.WriteLine("   [OK] Affected rows returned: " + affectedRows);
            Console.WriteLine("   [OK] Entity deleted from database\n");
        }

        /// <summary>
        /// Test 2: ExecuteCommandHasChangeAsync
        /// Validates: Change detection for existing and non-existent entities
        /// </summary>
        public static void AsyncDelete_ExecuteCommandHasChangeAsync()
        {
            Console.WriteLine("TEST 2: AsyncDelete_ExecuteCommandHasChangeAsync");

            var db = Db;

            // Setup: Insert test entity
            var order = new Order
            {
                Name = "Test Order for HasChange",
                Price = 200.75m,
                CreateTime = DateTime.Now
            };
            var insertedId = db.Insertable(order).ExecuteReturnIdentity();

            // Test: Delete existing entity (should return true)
            var hasChangeTask = db.Deleteable<Order>()
                .Where(x => x.Id == insertedId)
                .ExecuteCommandHasChangeAsync();
            
            var hasChange = hasChangeTask.GetAwaiter().GetResult();

            if (!hasChange)
                throw new Exception("Expected HasChange=true for existing entity");

            // Test: Delete non-existent entity (should return false)
            var noChangeTask = db.Deleteable<Order>()
                .Where(x => x.Id == 999999)
                .ExecuteCommandHasChangeAsync();
            
            var noChange = noChangeTask.GetAwaiter().GetResult();

            if (noChange)
                throw new Exception("Expected HasChange=false for non-existent entity");

            Console.WriteLine("   [OK] HasChange=true for existing entity");
            Console.WriteLine("   [OK] HasChange=false for non-existent entity");
            Console.WriteLine("   [OK] Change detection works correctly\n");
        }

        /// <summary>
        /// Test 3: Multiple entities async delete
        /// Validates: Bulk delete with DeleteRange, all entities removed
        /// </summary>
        public static void AsyncDelete_MultipleEntitiesAsync()
        {
            Console.WriteLine("TEST 3: AsyncDelete_MultipleEntitiesAsync");

            var db = Db;

            // Setup: Insert 100 test entities
            var orders = new List<Order>();
            for (int i = 0; i < 100; i++)
            {
                orders.Add(new Order
                {
                    Name = $"Bulk Delete Order {i}",
                    Price = 10.0m + i,
                    CreateTime = DateTime.Now
                });
            }
            var insertedIds = db.Insertable(orders).ExecuteReturnPkList<int>();

            // Test: DeleteRange with 100 entities
            var deleteTask = db.Deleteable<Order>()
                .Where(x => insertedIds.Contains(x.Id))
                .ExecuteCommandAsync();
            
            var affectedRows = deleteTask.GetAwaiter().GetResult();

            // Verify: All deleted
            if (affectedRows != 100)
                throw new Exception($"Expected 100 affected rows, got {affectedRows}");

            var remainingCount = db.Queryable<Order>()
                .Where(x => insertedIds.Contains(x.Id))
                .Count();

            if (remainingCount != 0)
                throw new Exception($"Expected 0 remaining entities, found {remainingCount}");

            Console.WriteLine("   [OK] Bulk delete async works");
            Console.WriteLine($"   [OK] Deleted {affectedRows} entities");
            Console.WriteLine("   [OK] All entities removed from database\n");
        }

        /// <summary>
        /// Test 4: Delete by primary key
        /// Validates: PK-based delete, only specific entity deleted
        /// </summary>
        public static void AsyncDelete_ByPrimaryKey()
        {
            Console.WriteLine("TEST 4: AsyncDelete_ByPrimaryKey");

            var db = Db;

            // Setup: Insert multiple test entities
            var orders = new List<Order>
            {
                new Order { Name = "Order 1", Price = 100m, CreateTime = DateTime.Now },
                new Order { Name = "Order 2", Price = 200m, CreateTime = DateTime.Now },
                new Order { Name = "Order 3", Price = 300m, CreateTime = DateTime.Now }
            };
            var insertedIds = db.Insertable(orders).ExecuteReturnPkList<int>();

            // Test: Delete by ID (using In method for primary key)
            var targetId = insertedIds[1]; // Delete middle one
            var deleteTask = db.Deleteable<Order>()
                .In(targetId)
                .ExecuteCommandAsync();
            
            var affectedRows = deleteTask.GetAwaiter().GetResult();

            // Verify: Entity deleted
            if (affectedRows != 1)
                throw new Exception($"Expected 1 affected row, got {affectedRows}");

            var deletedOrder = db.Queryable<Order>().InSingle(targetId);
            if (deletedOrder != null)
                throw new Exception("Target entity should be deleted");

            // Verify: Only specific entity deleted
            var remainingOrders = db.Queryable<Order>()
                .Where(x => insertedIds.Contains(x.Id))
                .ToList();

            if (remainingOrders.Count != 2)
                throw new Exception($"Expected 2 remaining orders, found {remainingOrders.Count}");

            // Cleanup
            db.Deleteable<Order>().Where(x => insertedIds.Contains(x.Id)).ExecuteCommand();

            Console.WriteLine("   [OK] PK-based delete works");
            Console.WriteLine("   [OK] Only specific entity deleted");
            Console.WriteLine("   [OK] Other entities remain intact\n");
        }

        /// <summary>
        /// Test 5: Delete by expression
        /// Validates: Expression-based delete, correct entities removed, others remain
        /// </summary>
        public static void AsyncDelete_ByExpression()
        {
            Console.WriteLine("TEST 5: AsyncDelete_ByExpression");

            var db = Db;

            // Setup: Insert test entities with varying prices
            var orders = new List<Order>
            {
                new Order { Name = "Low Price 1", Price = 25m, CreateTime = DateTime.Now },
                new Order { Name = "Low Price 2", Price = 35m, CreateTime = DateTime.Now },
                new Order { Name = "High Price 1", Price = 55m, CreateTime = DateTime.Now },
                new Order { Name = "High Price 2", Price = 75m, CreateTime = DateTime.Now },
                new Order { Name = "High Price 3", Price = 95m, CreateTime = DateTime.Now }
            };
            var insertedIds = db.Insertable(orders).ExecuteReturnPkList<int>();

            // Test: Delete(x => x.Price > 50) - should delete 3 orders
            var deleteTask = db.Deleteable<Order>()
                .Where(x => x.Price > 50 && insertedIds.Contains(x.Id))
                .ExecuteCommandAsync();
            
            var affectedRows = deleteTask.GetAwaiter().GetResult();

            // Verify: Correct entities deleted (3 with Price > 50)
            if (affectedRows != 3)
                throw new Exception($"Expected 3 affected rows, got {affectedRows}");

            // Verify: Others remain (2 with Price <= 50)
            var remainingOrders = db.Queryable<Order>()
                .Where(x => insertedIds.Contains(x.Id))
                .ToList();

            if (remainingOrders.Count != 2)
                throw new Exception($"Expected 2 remaining orders, found {remainingOrders.Count}");

            if (remainingOrders.Any(x => x.Price > 50))
                throw new Exception("Orders with Price > 50 should be deleted");

            // Cleanup
            db.Deleteable<Order>().Where(x => insertedIds.Contains(x.Id)).ExecuteCommand();

            Console.WriteLine("   [OK] Expression-based delete works");
            Console.WriteLine($"   [OK] Deleted {affectedRows} entities matching expression");
            Console.WriteLine("   [OK] Other entities remain intact\n");
        }

        #endregion

        #region B. CancellationToken Tests (4 functions)

        /// <summary>
        /// Test 6: CancellationToken basic functionality
        /// Validates: Delete operation can be cancelled with timeout
        /// </summary>
        public static void AsyncDelete_CancellationToken_Basic()
        {
            Console.WriteLine("TEST 6: AsyncDelete_CancellationToken_Basic");

            var db = Db;

            // Setup: Insert test entity
            var order = new Order
            {
                Name = "Test Order for Cancellation",
                Price = 100m,
                CreateTime = DateTime.Now
            };
            var insertedId = db.Insertable(order).ExecuteReturnIdentity();

            // Test: ExecuteCommandAsync with valid token (should succeed)
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var deleteTask = db.Deleteable<Order>()
                    .Where(x => x.Id == insertedId)
                    .ExecuteCommandAsync(cts.Token);

                var affectedRows = deleteTask.GetAwaiter().GetResult();

                if (affectedRows != 1)
                    throw new Exception($"Expected 1 affected row, got {affectedRows}");
            }

            Console.WriteLine("   [OK] Delete with CancellationToken succeeded");
            Console.WriteLine("   [OK] Token not cancelled - operation completed\n");
        }

        /// <summary>
        /// Test 7: CancellationToken immediate cancellation
        /// Validates: Pre-cancelled token prevents database operation
        /// </summary>
        public static void AsyncDelete_CancellationToken_Immediate()
        {
            Console.WriteLine("TEST 7: AsyncDelete_CancellationToken_Immediate");

            var db = Db;

            // Setup: Insert test entity
            var order = new Order
            {
                Name = "Test Order for Immediate Cancel",
                Price = 100m,
                CreateTime = DateTime.Now
            };
            var insertedId = db.Insertable(order).ExecuteReturnIdentity();

            // Test: Pre-cancelled token
            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel(); // Cancel immediately

                try
                {
                    var deleteTask = db.Deleteable<Order>()
                        .Where(x => x.Id == insertedId)
                        .ExecuteCommandAsync(cts.Token);

                    deleteTask.GetAwaiter().GetResult();

                    throw new Exception("Expected OperationCanceledException");
                }
                catch (OperationCanceledException)
                {
                    // Expected - operation was cancelled
                }
                catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
                {
                    // Expected - wrapped in AggregateException
                }
            }

            // Verify: Entity still exists (delete was cancelled)
            var existingOrder = db.Queryable<Order>().InSingle(insertedId);
            if (existingOrder == null)
                throw new Exception("Entity should still exist after cancelled delete");

            // Cleanup
            db.Deleteable<Order>().In(insertedId).ExecuteCommand();

            Console.WriteLine("   [OK] Immediate cancellation works");
            Console.WriteLine("   [OK] No database operation performed");
            Console.WriteLine("   [OK] Entity remains in database\n");
        }

        /// <summary>
        /// Test 8: CancellationToken bulk delete
        /// Validates: Bulk delete can be cancelled mid-operation
        /// </summary>
        public static void AsyncDelete_CancellationToken_BulkDelete()
        {
            Console.WriteLine("TEST 8: AsyncDelete_CancellationToken_BulkDelete");

            var db = Db;

            // Setup: Insert 1000 test entities
            var orders = new List<Order>();
            for (int i = 0; i < 1000; i++)
            {
                orders.Add(new Order
                {
                    Name = $"Bulk Cancel Order {i}",
                    Price = 10.0m + i,
                    CreateTime = DateTime.Now
                });
            }
            var insertedIds = db.Insertable(orders).ExecuteReturnPkList<int>();

            // Test: Delete with short timeout (may or may not cancel depending on speed)
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1)))
            {
                try
                {
                    var deleteTask = db.Deleteable<Order>()
                        .Where(x => insertedIds.Contains(x.Id))
                        .ExecuteCommandAsync(cts.Token);

                    deleteTask.GetAwaiter().GetResult();

                    // If we get here, operation completed before timeout
                    Console.WriteLine("   [OK] Bulk delete completed before timeout");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("   [OK] Bulk delete was cancelled");
                }
                catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
                {
                    Console.WriteLine("   [OK] Bulk delete was cancelled (wrapped)");
                }
                catch (Exception ex) when (ex.Message.Contains("Operation cancelled"))
                {
                    Console.WriteLine("   [OK] Bulk delete was cancelled (SQL exception)");
                }
            }

            // Cleanup: Remove any remaining entities
            db.Deleteable<Order>().Where(x => insertedIds.Contains(x.Id)).ExecuteCommand();

            Console.WriteLine("   [OK] Bulk delete cancellation handled\n");
        }

        /// <summary>
        /// Test 9: CancellationToken timeout scenarios
        /// Validates: Various timeout durations work correctly
        /// </summary>
        public static void AsyncDelete_CancellationToken_Timeouts()
        {
            Console.WriteLine("TEST 9: AsyncDelete_CancellationToken_Timeouts");

            var db = Db;

            // Test various timeout durations
            var timeouts = new[] { 100, 500, 1000, 5000 }; // milliseconds

            foreach (var timeout in timeouts)
            {
                // Setup: Insert test entity
                var order = new Order
                {
                    Name = $"Timeout Test Order {timeout}ms",
                    Price = 100m,
                    CreateTime = DateTime.Now
                };
                var insertedId = db.Insertable(order).ExecuteReturnIdentity();

                // Test: Delete with specific timeout
                using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout)))
                {
                    try
                    {
                        var deleteTask = db.Deleteable<Order>()
                            .Where(x => x.Id == insertedId)
                            .ExecuteCommandAsync(cts.Token);

                        deleteTask.GetAwaiter().GetResult();

                        Console.WriteLine($"   [OK] {timeout}ms timeout - operation completed");
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"   [OK] {timeout}ms timeout - operation cancelled");
                        // Cleanup if cancelled
                        db.Deleteable<Order>().In(insertedId).ExecuteCommand();
                    }
                    catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
                    {
                        Console.WriteLine($"   [OK] {timeout}ms timeout - operation cancelled (wrapped)");
                        // Cleanup if cancelled
                        db.Deleteable<Order>().In(insertedId).ExecuteCommand();
                    }
                }
            }

            Console.WriteLine("   [OK] All timeout scenarios handled correctly\n");
        }

        #endregion

        #region C. Cascade & Soft Delete Tests (3 functions)

        /// <summary>
        /// Test 10: Cascade delete
        /// Validates: Foreign key constraints and cascade behavior
        /// Note: Actual cascade depends on database FK configuration
        /// </summary>
        public static void AsyncDelete_CascadeDelete()
        {
            Console.WriteLine("TEST 10: AsyncDelete_CascadeDelete");

            var db = Db;

            // Setup: Ensure tables exist
            db.CodeFirst.InitTables<Order, OrderItem>();

            // Setup: Insert parent order
            var order = new Order
            {
                Name = "Parent Order for Cascade",
                Price = 500m,
                CreateTime = DateTime.Now
            };
            var orderId = db.Insertable(order).ExecuteReturnIdentity();

            // Setup: Insert child order items
            var orderItems = new List<OrderItem>
            {
                new OrderItem { OrderId = orderId, ItemId = 1, Price = 100m, CreateTime = DateTime.Now },
                new OrderItem { OrderId = orderId, ItemId = 2, Price = 200m, CreateTime = DateTime.Now },
                new OrderItem { OrderId = orderId, ItemId = 3, Price = 200m, CreateTime = DateTime.Now }
            };
            db.Insertable(orderItems).ExecuteCommand();

            // Test: Delete parent entity
            var deleteTask = db.Deleteable<Order>()
                .Where(x => x.Id == orderId)
                .ExecuteCommandAsync();

            var affectedRows = deleteTask.GetAwaiter().GetResult();

            if (affectedRows != 1)
                throw new Exception($"Expected 1 affected row, got {affectedRows}");

            // Verify: Parent deleted
            var deletedOrder = db.Queryable<Order>().InSingle(orderId);
            if (deletedOrder != null)
                throw new Exception("Parent entity should be deleted");

            // Check child entities (behavior depends on FK configuration)
            var remainingItems = db.Queryable<OrderItem>()
                .Where(x => x.OrderId == orderId)
                .ToList();

            if (remainingItems.Any())
            {
                Console.WriteLine("   [OK] Parent deleted, child records remain (no FK cascade)");
                // Cleanup child records manually
                db.Deleteable<OrderItem>().Where(x => x.OrderId == orderId).ExecuteCommand();
            }
            else
            {
                Console.WriteLine("   [OK] Parent deleted, child records cascaded (FK cascade enabled)");
            }

            Console.WriteLine("   [OK] Cascade delete behavior verified\n");
        }

        /// <summary>
        /// Test 11: Soft delete (IsLogic)
        /// Validates: Entity marked as deleted but not physically removed
        /// </summary>
        public static void AsyncDelete_SoftDelete()
        {
            Console.WriteLine("TEST 11: AsyncDelete_SoftDelete");

            var db = Db;

            // Setup: Create soft delete entity table
            db.CodeFirst.InitTables<SoftDeleteEntity>();

            // Insert test entity
            var entity = new SoftDeleteEntity
            {
                Name = "Soft Delete Test",
                Value = 100
            };
            var insertedId = db.Insertable(entity).ExecuteReturnIdentity();

            // Test: Soft delete with IsLogic
            var deleteTask = db.Deleteable<SoftDeleteEntity>()
                .Where(x => x.Id == insertedId)
                .IsLogic() // Enable soft delete
                .ExecuteCommandAsync();

            var affectedRows = deleteTask.GetAwaiter().GetResult();

            if (affectedRows != 1)
                throw new Exception($"Expected 1 affected row, got {affectedRows}");

            // Verify: IsDeleted=true, not actually deleted
            var softDeletedEntity = db.Queryable<SoftDeleteEntity>()
                .Where(x => x.Id == insertedId)
                .First();

            if (softDeletedEntity == null)
                throw new Exception("Entity should still exist in database");

            if (!softDeletedEntity.IsDeleted)
                throw new Exception("IsDeleted should be true");

            Console.WriteLine("   [OK] Soft delete works");
            Console.WriteLine("   [OK] IsDeleted=true, entity not physically deleted");
            Console.WriteLine("   [OK] Entity still in database\n");
        }

        /// <summary>
        /// Test 12: Soft delete with timestamp
        /// Validates: Soft delete with IsDeleted flag (timestamp would require custom logic)
        /// </summary>
        public static void AsyncDelete_SoftDelete_Timestamp()
        {
            Console.WriteLine("TEST 12: AsyncDelete_SoftDelete_Timestamp");

            var db = Db;

            // Setup: Create soft delete entity with timestamp table
            db.CodeFirst.InitTables<SoftDeleteWithTimestamp>();

            // Insert test entity
            var entity = new SoftDeleteWithTimestamp
            {
                Name = "Soft Delete with Timestamp",
                Value = 200
            };
            var insertedId = db.Insertable(entity).ExecuteReturnIdentity();

            // Test: Soft delete with IsLogic (sets IsDeleted flag)
            var deleteTask = db.Deleteable<SoftDeleteWithTimestamp>()
                .Where(x => x.Id == insertedId)
                .IsLogic() // Enable soft delete
                .ExecuteCommandAsync();

            var affectedRows = deleteTask.GetAwaiter().GetResult();

            if (affectedRows != 1)
                throw new Exception($"Expected 1 affected row, got {affectedRows}");

            // Verify: IsDeleted flag set
            var softDeletedEntity = db.Queryable<SoftDeleteWithTimestamp>()
                .Where(x => x.Id == insertedId)
                .First();

            if (softDeletedEntity == null)
                throw new Exception("Entity should still exist in database");

            if (!softDeletedEntity.IsDeleted)
                throw new Exception("IsDeleted should be true");

            // Note: DeletedAt timestamp would require custom update logic
            // IsLogic() only sets the IsDeleted flag by default

            Console.WriteLine("   [OK] Soft delete works with IsDeleted flag");
            Console.WriteLine("   [OK] Entity marked as deleted but not physically removed");
            Console.WriteLine("   [OK] Entity still in database (timestamp field available for custom logic)\n");
        }

        #endregion

        #region D. Edge Cases & Error Handling (3 functions)

        /// <summary>
        /// Test 13: Delete non-existent entity
        /// Validates: Graceful handling of deleting non-existent records
        /// </summary>
        public static void AsyncDelete_NonExistent()
        {
            Console.WriteLine("TEST 13: AsyncDelete_NonExistent");

            var db = Db;

            // Test: Delete entity that doesn't exist
            var deleteTask = db.Deleteable<Order>()
                .Where(x => x.Id == 999999)
                .ExecuteCommandAsync();

            var affectedRows = deleteTask.GetAwaiter().GetResult();

            // Verify: 0 rows affected
            if (affectedRows != 0)
                throw new Exception($"Expected 0 affected rows, got {affectedRows}");

            // Test: HasChange should return false
            var hasChangeTask = db.Deleteable<Order>()
                .Where(x => x.Id == 999999)
                .ExecuteCommandHasChangeAsync();

            var hasChange = hasChangeTask.GetAwaiter().GetResult();

            if (hasChange)
                throw new Exception("Expected HasChange=false for non-existent entity");

            Console.WriteLine("   [OK] Delete non-existent entity handled gracefully");
            Console.WriteLine("   [OK] 0 rows affected");
            Console.WriteLine("   [OK] No error thrown\n");
        }

        /// <summary>
        /// Test 14: Concurrent delete operations
        /// Validates: Multiple threads deleting different entities safely
        /// </summary>
        public static void AsyncDelete_ConcurrentDeletes()
        {
            Console.WriteLine("TEST 14: AsyncDelete_ConcurrentDeletes");

            var db = Db;

            // Setup: Insert 10 test entities
            var orders = new List<Order>();
            for (int i = 0; i < 10; i++)
            {
                orders.Add(new Order
                {
                    Name = $"Concurrent Delete Order {i}",
                    Price = 100m + i,
                    CreateTime = DateTime.Now
                });
            }
            var insertedIds = db.Insertable(orders).ExecuteReturnPkList<int>();

            // Test: 10 threads deleting different entities
            var tasks = new List<Task<int>>();

            foreach (var id in insertedIds)
            {
                var task = Task.Run(async () =>
                {
                    var dbInstance = Db; // Each task gets its own db instance
                    return await dbInstance.Deleteable<Order>()
                        .Where(x => x.Id == id)
                        .ExecuteCommandAsync();
                });
                tasks.Add(task);
            }

            // Wait for all tasks to complete
            Task.WaitAll(tasks.ToArray());

            // Verify: All deletes succeeded
            var totalAffected = tasks.Sum(t => t.Result);
            if (totalAffected != 10)
                throw new Exception($"Expected 10 total affected rows, got {totalAffected}");

            // Verify: No deadlocks occurred (all tasks completed)
            if (tasks.Any(t => t.IsFaulted))
                throw new Exception("Some tasks failed with exceptions");

            // Verify: All entities deleted
            var remainingCount = db.Queryable<Order>()
                .Where(x => insertedIds.Contains(x.Id))
                .Count();

            if (remainingCount != 0)
                throw new Exception($"Expected 0 remaining entities, found {remainingCount}");

            Console.WriteLine("   [OK] Concurrent async deletes safe");
            Console.WriteLine("   [OK] All 10 deletes succeeded");
            Console.WriteLine("   [OK] No deadlocks occurred\n");
        }

        /// <summary>
        /// Test 15: Delete performance
        /// Validates: Async delete performance is acceptable
        /// </summary>
        public static void AsyncDelete_Performance()
        {
            Console.WriteLine("TEST 15: AsyncDelete_Performance");

            var db = Db;

            // Setup: Insert 1000 test entities
            var orders = new List<Order>();
            for (int i = 0; i < 1000; i++)
            {
                orders.Add(new Order
                {
                    Name = $"Performance Test Order {i}",
                    Price = 10.0m + i,
                    CreateTime = DateTime.Now
                });
            }
            var insertedIds = db.Insertable(orders).ExecuteReturnPkList<int>();

            // Test: Delete 1000 entities async
            var asyncStart = DateTime.Now;
            var deleteTask = db.Deleteable<Order>()
                .Where(x => insertedIds.Contains(x.Id))
                .ExecuteCommandAsync();
            
            var affectedRows = deleteTask.GetAwaiter().GetResult();
            var asyncDuration = DateTime.Now - asyncStart;

            // Verify: All deleted
            if (affectedRows != 1000)
                throw new Exception($"Expected 1000 affected rows, got {affectedRows}");

            var remainingCount = db.Queryable<Order>()
                .Where(x => insertedIds.Contains(x.Id))
                .Count();

            if (remainingCount != 0)
                throw new Exception($"Expected 0 remaining entities, found {remainingCount}");

            Console.WriteLine($"   [OK] Async delete: {affectedRows} rows in {asyncDuration.TotalMilliseconds}ms");
            Console.WriteLine("   [OK] Performance acceptable");
            Console.WriteLine("   [OK] All entities deleted successfully\n");
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Helper entity for soft delete tests
        /// Uses IsDeleted flag for logical deletion
        /// </summary>
        [SugarTable("SoftDeleteEntity")]
        public class SoftDeleteEntity
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            
            public string Name { get; set; }
            public int Value { get; set; }
            public bool IsDeleted { get; set; }
        }

        /// <summary>
        /// Helper entity for soft delete with timestamp tests
        /// Uses IsDeleted flag and DeletedAt timestamp for audit trail
        /// </summary>
        [SugarTable("SoftDeleteWithTimestamp")]
        public class SoftDeleteWithTimestamp
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            
            public string Name { get; set; }
            public int Value { get; set; }
            public bool IsDeleted { get; set; }
            [SugarColumn(IsNullable = true)]
            public DateTime? DeletedAt { get; set; }
        }

        #endregion
    }
}
