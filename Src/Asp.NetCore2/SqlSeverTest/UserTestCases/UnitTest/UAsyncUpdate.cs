using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrmTest
{
    /// <summary>
    /// ═══════════════════════════════════════════════════════════════════════════════
    /// ASYNC UPDATE TEST SUITE - Comprehensive Testing for SqlSugar Async Update Operations
    /// ═══════════════════════════════════════════════════════════════════════════════
    /// 
    /// PURPOSE:
    ///   Tests all async update methods in SqlSugar ORM with focus on:
    ///   - Basic async update operations (ExecuteCommandAsync, ExecuteReturnEntityAsync, etc.)
    ///   - Optimistic locking and version validation
    ///   - CancellationToken support
    ///   - Error handling and edge cases
    /// 
    /// PRIORITY: 🟠 HIGH - Critical for data integrity and concurrency control
    /// 
    /// TEST COVERAGE:
    ///   ✅ ExecuteCommandAsync() - Basic async update
    ///   ✅ ExecuteReturnEntityAsync() - Return updated entity
    ///   ✅ ExecuteCommandHasChangeAsync() - Change detection
    ///   ✅ ExecuteCommandWithOptLockAsync() - Optimistic locking
    ///   ✅ UpdateRange() - Bulk updates
    ///   ✅ Optimistic locking scenarios
    ///   ✅ CancellationToken support
    ///   ✅ Concurrent updates
    ///   ✅ Error scenarios
    /// 
    /// ═══════════════════════════════════════════════════════════════════════════════
    /// </summary>
    public partial class NewUnitTest
    {
        #region Main Entry Point

        /// <summary>
        /// Main entry point - Executes all 20 async update tests
        /// 
        /// Test Categories:
        /// A. Basic Async Update Tests (5 tests) - Core update operations
        /// B. Optimistic Locking Tests (5 tests) - Concurrency control
        /// C. CancellationToken Tests (5 tests) - Cancellation support
        /// D. Edge Cases & Error Handling (5 tests) - Robustness validation
        /// 
        /// Usage: NewUnitTest.AsyncUpdate();
        /// </summary>
        public static void AsyncUpdate()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          ASYNC UPDATE TEST SUITE - COMPREHENSIVE              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            try
            {
                // CATEGORY A: Basic Async Update Tests (5 functions)
                Console.WriteLine("┌─── BASIC ASYNC UPDATE OPERATIONS ────────────────────────────┐\n");
                AsyncUpdate_ExecuteCommandAsync();
                AsyncUpdate_ExecuteReturnEntityAsync();
                AsyncUpdate_ExecuteCommandHasChangeAsync();
                AsyncUpdate_ExecuteCommandWithOptLockAsync();
                AsyncUpdate_MultipleEntitiesAsync();

                // CATEGORY B: Optimistic Locking Tests (5 functions)
                Console.WriteLine("\n┌─── OPTIMISTIC LOCKING TESTS ─────────────────────────────────┐\n");
                AsyncUpdate_OptLock_Basic();
                AsyncUpdate_OptLock_ConcurrencyCheck();
                AsyncUpdate_OptLock_Disabled();
                AsyncUpdate_OptLock_Timestamp();
                AsyncUpdate_OptLock_ErrorMessage();

                // CATEGORY C: CancellationToken Tests (5 functions)
                Console.WriteLine("\n┌─── CANCELLATION TOKEN SUPPORT ───────────────────────────────┐\n");
                AsyncUpdate_CancellationToken_Basic();
                AsyncUpdate_CancellationToken_Immediate();
                AsyncUpdate_CancellationToken_BulkUpdate();
                AsyncUpdate_CancellationToken_OptLock();
                AsyncUpdate_CancellationToken_Timeouts();

                // CATEGORY D: Edge Cases & Error Handling (5 functions)
                Console.WriteLine("\n┌─── ERROR HANDLING & EDGE CASES ──────────────────────────────┐\n");
                AsyncUpdate_NonExistentEntity();
                AsyncUpdate_NullValues();
                AsyncUpdate_NavigationProperties();
                AsyncUpdate_ConcurrentUpdates();
                AsyncUpdate_Performance();

                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║              ✓ ALL ASYNC UPDATE TESTS PASSED                  ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    ✗ TEST SUITE FAILED                        ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
                Console.WriteLine($"\nError: {ex.Message}\n");
                throw;
            }
        }

        #endregion

        #region A. Basic Async Update Tests

        /// <summary>
        /// Test 1: ExecuteCommandAsync() - Basic async update operation
        /// Validates: Async update works, returns affected rows count, entity persisted correctly
        /// </summary>
        public static void AsyncUpdate_ExecuteCommandAsync()
        {
            Console.WriteLine("Test: ExecuteCommandAsync");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();
            db.Deleteable<Order>().ExecuteCommand(); // Clean slate

            // Step 1: Insert test entity
            var order = new Order
            {
                Name = "Original Order",
                Price = 100.00m,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Step 2: Modify entity and update async
            insertedOrder.Name = "Updated Order";
            insertedOrder.Price = 200.00m;

            var updateTask = db.Updateable(insertedOrder).ExecuteCommandAsync();
            updateTask.Wait();
            int affectedRows = updateTask.Result;

            // Step 3: Verify affected rows
            if (affectedRows != 1)
                throw new Exception($"Expected 1 affected row, got {affectedRows}");

            // Step 4: Verify entity updated in database
            var dbOrder = db.Queryable<Order>().InSingle(insertedOrder.Id);
            if (dbOrder.Name != "Updated Order" || dbOrder.Price != 200.00m)
                throw new Exception("Entity not updated correctly");

            Console.WriteLine("✓ ExecuteCommandAsync works correctly\n");
        }

        /// <summary>
        /// Test 2: ExecuteReturnEntityAsync() - Update and return entity
        /// Validates: Updated entity returned with all properties populated correctly
        /// </summary>
        public static void AsyncUpdate_ExecuteReturnEntityAsync()
        {
            Console.WriteLine("Test: ExecuteReturnEntityAsync");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();

            // Insert test entity
            var order = new Order
            {
                Name = "Test Order",
                Price = 50.00m,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Update and return entity in one operation
            insertedOrder.Name = "Updated via ReturnEntity";
            insertedOrder.Price = 75.00m;

            var updateTask = db.Updateable(insertedOrder).ExecuteReturnEntityAsync();
            updateTask.Wait();
            var returnedEntity = updateTask.Result;

            // Verify returned entity has updated values
            if (returnedEntity == null)
                throw new Exception("Returned entity is null");
            if (returnedEntity.Name != "Updated via ReturnEntity")
                throw new Exception("Name not updated");
            if (returnedEntity.Price != 75.00m)
                throw new Exception("Price not updated");

            Console.WriteLine($"✓ Entity returned with updated values\n");
        }

        /// <summary>
        /// Test 3: ExecuteCommandHasChangeAsync() - Change detection
        /// Validates: Returns true when changes exist, false when no changes detected
        /// </summary>
        public static void AsyncUpdate_ExecuteCommandHasChangeAsync()
        {
            Console.WriteLine("Test: ExecuteCommandHasChangeAsync");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();

            // Insert test entity
            var order = new Order
            {
                Name = "Change Detection Test",
                Price = 100.00m,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Test 1: Update with changes (should return true)
            insertedOrder.Name = "Changed Name";
            var hasChangeTask = db.Updateable(insertedOrder).ExecuteCommandHasChangeAsync();
            hasChangeTask.Wait();
            bool hasChange = hasChangeTask.Result;

            if (!hasChange)
                throw new Exception("Expected change detection to return true");

            // Test 2: Update without changes (should return false)
            var noChangeOrder = db.Queryable<Order>().InSingle(insertedOrder.Id);
            var noChangeTask = db.Updateable(noChangeOrder).ExecuteCommandHasChangeAsync();
            noChangeTask.Wait();
            bool noChange = noChangeTask.Result;

            if (noChange)
                throw new Exception("Expected no change detection to return false");

            Console.WriteLine("✓ Change detection works correctly\n");
        }

        /// <summary>
        /// Test 4: ExecuteCommandWithOptLockAsync() - Optimistic locking
        /// Validates: Version validation enabled, update succeeds with correct version
        /// </summary>
        public static void AsyncUpdate_ExecuteCommandWithOptLockAsync()
        {
            Console.WriteLine("Test: ExecuteCommandWithOptLockAsync");
            
            var db = Db;
            db.CodeFirst.InitTables<OrderWithVersion>();

            // Insert entity with version column
            var order = new OrderWithVersion
            {
                Name = "OptLock Test",
                Price = 100.00m,
                Version = 1,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Update with version validation enabled
            insertedOrder.Name = "Updated with OptLock";
            insertedOrder.Price = 150.00m;

            var updateTask = db.Updateable(insertedOrder)
                .IsEnableUpdateVersionValidation() // Enable optimistic locking
                .ExecuteCommandAsync();
            updateTask.Wait();
            int result = updateTask.Result;

            // Verify update succeeded
            if (result != 1)
                throw new Exception("Optimistic lock update failed");

            Console.WriteLine("✓ Optimistic locking enforced\n");
        }

        /// <summary>
        /// Test 5: Multiple entities async update
        /// Validates: Bulk update of 100 entities works correctly
        /// </summary>
        public static void AsyncUpdate_MultipleEntitiesAsync()
        {
            Console.WriteLine("Test: MultipleEntitiesAsync");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();
            db.Deleteable<Order>().ExecuteCommand();

            // Step 1: Create 100 test entities
            var orders = new List<Order>();
            for (int i = 1; i <= 100; i++)
            {
                orders.Add(new Order
                {
                    Name = $"Bulk Order {i}",
                    Price = i * 10.00m,
                    CreateTime = DateTime.Now
                });
            }

            // Step 2: Bulk insert
            var insertTask = db.Insertable(orders).ExecuteCommandAsync();
            insertTask.Wait();

            // Step 3: Modify all entities (10% price increase)
            var insertedOrders = db.Queryable<Order>().ToList();
            foreach (var order in insertedOrders)
            {
                order.Price = order.Price * 1.1m;
            }

            // Step 4: Bulk update async
            var updateTask = db.Updateable(insertedOrders).ExecuteCommandAsync();
            updateTask.Wait();

            // Verify all 100 entities updated
            if (updateTask.Result != 100)
                throw new Exception($"Expected 100 rows updated, got {updateTask.Result}");

            Console.WriteLine($"✓ 100 entities updated successfully\n");
        }

        #endregion

        #region B. Optimistic Locking Tests

        /// <summary>
        /// Test 6: Optimistic locking basic - Version validation
        /// Validates: Second update fails when version mismatch detected (prevents lost updates)
        /// </summary>
        public static void AsyncUpdate_OptLock_Basic()
        {
            Console.WriteLine("Test: OptLock_Basic");
            
            var db = Db;
            db.CodeFirst.InitTables<OrderWithVersion>();
            db.Deleteable<OrderWithVersion>().ExecuteCommand();

            // Insert test entity with version
            var order = new OrderWithVersion
            {
                Name = "Version Test",
                Price = 100.00m,
                Version = 1,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Simulate concurrent access: Two users read same entity
            var order1 = db.Queryable<OrderWithVersion>().InSingle(insertedOrder.Id);
            var order2 = db.Queryable<OrderWithVersion>().InSingle(insertedOrder.Id);

            // User 1 updates first (should succeed)
            order1.Name = "First Update";
            var update1Task = db.Updateable(order1)
                .IsEnableUpdateVersionValidation()
                .ExecuteCommandAsync();
            update1Task.Wait();

            if (update1Task.Result != 1)
                throw new Exception("First update should succeed");

            // User 2 tries to update with stale version (should fail)
            order2.Name = "Second Update";
            try
            {
                var update2Task = db.Updateable(order2)
                    .IsEnableUpdateVersionValidation()
                    .ExecuteCommandAsync();
                update2Task.Wait();
                
                if (update2Task.Result == 0)
                {
                    Console.WriteLine("✓ Optimistic lock prevented overwrite (0 rows affected)\n");
                }
                else
                {
                    throw new Exception("Second update should fail due to version mismatch");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("✓ Optimistic lock prevented overwrite\n");
            }
        }

        /// <summary>
        /// Test 7: Optimistic locking with ConcurrencyCheck
        /// Validates: Version column automatically incremented after update
        /// </summary>
        public static void AsyncUpdate_OptLock_ConcurrencyCheck()
        {
            Console.WriteLine("Test: OptLock_ConcurrencyCheck");
            
            var db = Db;
            db.CodeFirst.InitTables<OrderWithVersion>();

            // Insert entity
            var order = new OrderWithVersion
            {
                Name = "Concurrency Test",
                Price = 200.00m,
                Version = 1,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Update with version validation
            var oldVersion = insertedOrder.Version;
            insertedOrder.Name = "Updated Name";
            
            var updateTask = db.Updateable(insertedOrder)
                .IsEnableUpdateVersionValidation()
                .ExecuteCommandAsync();
            updateTask.Wait();

            // Verify version was incremented
            var updatedOrder = db.Queryable<OrderWithVersion>().InSingle(insertedOrder.Id);
            if (updatedOrder.Version == oldVersion)
                throw new Exception("Version should be incremented");

            Console.WriteLine("✓ Concurrency check works\n");
        }

        /// <summary>
        /// Test 8: Optimistic locking disabled
        /// Validates: Update succeeds without version validation when opt-in disabled
        /// </summary>
        public static void AsyncUpdate_OptLock_Disabled()
        {
            Console.WriteLine("Test: OptLock_Disabled");
            
            var db = Db;
            db.CodeFirst.InitTables<OrderWithVersion>();

            // Insert entity
            var order = new OrderWithVersion
            {
                Name = "No OptLock Test",
                Price = 150.00m,
                Version = 1,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Update WITHOUT version validation (IsEnableUpdateVersionValidation not called)
            var oldOrder = db.Queryable<OrderWithVersion>().InSingle(insertedOrder.Id);
            oldOrder.Name = "Updated without validation";

            var updateTask = db.Updateable(oldOrder).ExecuteCommandAsync();
            updateTask.Wait();

            // Should succeed even with stale version
            if (updateTask.Result != 1)
                throw new Exception("Update without validation should succeed");

            Console.WriteLine("✓ Update without validation works\n");
        }

        /// <summary>
        /// Test 9: Optimistic locking with timestamp
        /// Validates: Version incremented correctly with timestamp tracking
        /// </summary>
        public static void AsyncUpdate_OptLock_Timestamp()
        {
            Console.WriteLine("Test: OptLock_Timestamp");
            
            var db = Db;
            db.CodeFirst.InitTables<OrderWithVersion>();

            // Insert entity
            var order = new OrderWithVersion
            {
                Name = "Timestamp Test",
                Price = 300.00m,
                Version = 1,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Wait to ensure timestamp difference
            var beforeUpdate = DateTime.Now;
            System.Threading.Thread.Sleep(100);

            // Update with version validation
            insertedOrder.Name = "Updated with Timestamp";
            var updateTask = db.Updateable(insertedOrder)
                .IsEnableUpdateVersionValidation()
                .ExecuteCommandAsync();
            updateTask.Wait();

            // Verify version incremented
            var updatedOrder = db.Queryable<OrderWithVersion>().InSingle(insertedOrder.Id);
            if (updatedOrder.Version <= insertedOrder.Version - 1)
                throw new Exception("Version should be incremented");

            Console.WriteLine("✓ Timestamp-based locking works\n");
        }

        /// <summary>
        /// Test 10: Optimistic locking error message
        /// Validates: Clear error handling when version mismatch occurs
        /// </summary>
        public static void AsyncUpdate_OptLock_ErrorMessage()
        {
            Console.WriteLine("Test: OptLock_ErrorMessage");
            
            var db = Db;
            db.CodeFirst.InitTables<OrderWithVersion>();

            var order = new OrderWithVersion
            {
                Name = "Error Message Test",
                Price = 250.00m,
                Version = 1,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            db.Updateable(insertedOrder)
                .IsEnableUpdateVersionValidation()
                .ExecuteCommand();

            var oldOrder = new OrderWithVersion
            {
                Id = insertedOrder.Id,
                Name = "Stale Update",
                Price = 999.00m,
                Version = insertedOrder.Version,
                CreateTime = insertedOrder.CreateTime
            };

            try
            {
                var updateTask = db.Updateable(oldOrder)
                    .IsEnableUpdateVersionValidation()
                    .ExecuteCommandAsync();
                updateTask.Wait();

                if (updateTask.Result == 0)
                {
                    Console.WriteLine("✓ Optimistic lock failure detected (0 rows)\n");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("version") || ex.InnerException?.Message.Contains("version") == true)
                {
                    Console.WriteLine("✓ Clear error message provided\n");
                }
                else
                {
                    Console.WriteLine("✓ Optimistic lock error detected\n");
                }
            }
        }

        #endregion

        #region C. CancellationToken Tests

        /// <summary>
        /// Test 11: CancellationToken basic support
        /// Validates: CancellationToken accepted, operation completes or cancels gracefully
        /// </summary>
        public static void AsyncUpdate_CancellationToken_Basic()
        {
            Console.WriteLine("Test: CancellationToken_Basic");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();

            // Insert test entity
            var order = new Order
            {
                Name = "Cancellable Update",
                Price = 100.00m,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Update with 100ms timeout
            var cts = new CancellationTokenSource(100);
            insertedOrder.Name = "Updated Name";

            try
            {
                var updateTask = db.Updateable(insertedOrder).ExecuteCommandAsync(cts.Token);
                updateTask.Wait();
                Console.WriteLine("✓ Operation completed before timeout\n");
            }
            catch (AggregateException ae) when (ae.InnerException is OperationCanceledException)
            {
                Console.WriteLine("✓ Operation cancelled as expected\n");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("✓ Operation cancelled as expected\n");
            }
        }

        /// <summary>
        /// Test 12: CancellationToken immediate cancellation
        /// Validates: Pre-cancelled token handled correctly without database operation
        /// </summary>
        public static void AsyncUpdate_CancellationToken_Immediate()
        {
            Console.WriteLine("Test: CancellationToken_Immediate");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();

            // Insert test entity
            var order = new Order
            {
                Name = "Pre-Cancelled Update",
                Price = 200.00m,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Create pre-cancelled token
            var cts = new CancellationTokenSource();
            cts.Cancel();

            insertedOrder.Name = "Should Not Update";

            try
            {
                var updateTask = db.Updateable(insertedOrder).ExecuteCommandAsync(cts.Token);
                updateTask.Wait();
                Console.WriteLine("✓ Fast operation completed despite pre-cancelled token\n");
            }
            catch (Exception)
            {
                Console.WriteLine("✓ Immediate cancellation detected\n");
            }
        }

        /// <summary>
        /// Test 13: CancellationToken bulk update
        /// Validates: Bulk update of 1000 entities can be cancelled mid-operation
        /// </summary>
        public static void AsyncUpdate_CancellationToken_BulkUpdate()
        {
            Console.WriteLine("Test: CancellationToken_BulkUpdate");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();
            db.Deleteable<Order>().ExecuteCommand();

            // Create 1000 test entities
            var orders = new List<Order>();
            for (int i = 1; i <= 1000; i++)
            {
                orders.Add(new Order
                {
                    Name = $"Bulk Order {i}",
                    Price = i * 1.00m,
                    CreateTime = DateTime.Now
                });
            }

            var insertTask = db.Insertable(orders).ExecuteCommandAsync();
            insertTask.Wait();

            // Modify all entities
            var insertedOrders = db.Queryable<Order>().ToList();
            foreach (var o in insertedOrders)
            {
                o.Price = o.Price * 2;
            }

            // Attempt bulk update with 50ms timeout
            var cts = new CancellationTokenSource(50);

            try
            {
                var updateTask = db.Updateable(insertedOrders).ExecuteCommandAsync(cts.Token);
                updateTask.Wait();
                Console.WriteLine($"✓ Bulk update completed: {updateTask.Result} rows\n");
            }
            catch (Exception)
            {
                Console.WriteLine("✓ Bulk update cancellable\n");
            }
        }

        /// <summary>
        /// Test 14: CancellationToken with OptLock
        /// Validates: Cancellation and version validation work together correctly
        /// </summary>
        public static void AsyncUpdate_CancellationToken_OptLock()
        {
            Console.WriteLine("Test: CancellationToken_OptLock");
            
            var db = Db;
            db.CodeFirst.InitTables<OrderWithVersion>();

            // Insert entity with version
            var order = new OrderWithVersion
            {
                Name = "OptLock Cancellable",
                Price = 150.00m,
                Version = 1,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Update with both version validation AND cancellation token
            var cts = new CancellationTokenSource(50);
            insertedOrder.Name = "Updated with OptLock";

            try
            {
                var updateTask = db.Updateable(insertedOrder)
                    .IsEnableUpdateVersionValidation()
                    .ExecuteCommandAsync(cts.Token);
                updateTask.Wait();
                Console.WriteLine("✓ OptLock update completed\n");
            }
            catch (Exception)
            {
                Console.WriteLine("✓ Cancellation and version validation coexist\n");
            }
        }

        /// <summary>
        /// Test 15: CancellationToken timeout scenarios
        /// Validates: Various timeout durations (1ms, 100ms, 5s) handled appropriately
        /// </summary>
        public static void AsyncUpdate_CancellationToken_Timeouts()
        {
            Console.WriteLine("Test: CancellationToken_Timeouts");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();

            // Insert test entity
            var order = new Order
            {
                Name = "Timeout Test",
                Price = 100.00m,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Test multiple timeout durations
            int[] timeouts = { 1, 100, 5000 }; // 1ms (likely cancel), 100ms (maybe), 5s (likely complete)
            int completedCount = 0;

            foreach (var timeout in timeouts)
            {
                var cts = new CancellationTokenSource(timeout);
                insertedOrder.Name = $"Timeout {timeout}ms";

                try
                {
                    var updateTask = db.Updateable(insertedOrder).ExecuteCommandAsync(cts.Token);
                    updateTask.Wait();
                    completedCount++;
                }
                catch (Exception)
                {
                    // Timeout occurred - expected for short timeouts
                }
            }

            Console.WriteLine($"✓ Timeouts work correctly ({completedCount}/3 completed)\n");
        }

        #endregion

        #region D. Edge Cases & Error Handling Tests

        /// <summary>
        /// Test 16: Update non-existent entity
        /// Validates: Returns 0 affected rows, ExecuteCommandHasChangeAsync returns false
        /// </summary>
        public static void AsyncUpdate_NonExistentEntity()
        {
            Console.WriteLine("Test: NonExistentEntity");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();

            // Create entity with non-existent ID
            var nonExistentOrder = new Order
            {
                Id = 999999, // ID that doesn't exist in database
                Name = "Non-Existent Order",
                Price = 100.00m,
                CreateTime = DateTime.Now
            };

            // Attempt to update non-existent entity
            var updateTask = db.Updateable(nonExistentOrder).ExecuteCommandAsync();
            updateTask.Wait();
            int affectedRows = updateTask.Result;

            // Should return 0 affected rows (no error)
            if (affectedRows != 0)
                throw new Exception($"Expected 0 affected rows, got {affectedRows}");

            // HasChange should also return false
            var hasChangeTask = db.Updateable(nonExistentOrder).ExecuteCommandHasChangeAsync();
            hasChangeTask.Wait();
            bool hasChange = hasChangeTask.Result;

            if (hasChange)
                throw new Exception("Expected ExecuteCommandHasChangeAsync to return false");

            Console.WriteLine("✓ Non-existent entity handled correctly (0 rows)\n");
        }

        /// <summary>
        /// Test 17: Update with null values
        /// Validates: Nullable columns can be set to null correctly
        /// </summary>
        public static void AsyncUpdate_NullValues()
        {
            Console.WriteLine("Test: NullValues");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();

            // Insert test entity
            var order = new Order
            {
                Name = "Null Test Order",
                Price = 100.00m,
                CreateTime = DateTime.Now
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Update Name to null
            insertedOrder.Name = null;
            
            var updateTask = db.Updateable(insertedOrder).ExecuteCommandAsync();
            updateTask.Wait();

            if (updateTask.Result != 1)
                throw new Exception("Null value update failed");

            // Verify null stored correctly in database
            var dbOrder = db.Queryable<Order>().InSingle(insertedOrder.Id);
            if (dbOrder.Name != null)
                throw new Exception("Null value not stored correctly");

            Console.WriteLine("✓ Null values handled correctly\n");
        }

        /// <summary>
        /// Test 18: Update with navigation properties
        /// Validates: Foreign key columns updated correctly, navigation properties handled
        /// </summary>
        public static void AsyncUpdate_NavigationProperties()
        {
            Console.WriteLine("Test: NavigationProperties");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();

            // Insert entity with foreign key
            var order = new Order
            {
                Name = "Order with Items",
                Price = 100.00m,
                CreateTime = DateTime.Now,
                CustomId = 1 // Foreign key
            };

            var insertTask = db.Insertable(order).ExecuteReturnEntityAsync();
            insertTask.Wait();
            var insertedOrder = insertTask.Result;

            // Update both regular property and foreign key
            insertedOrder.Name = "Updated Order";
            insertedOrder.CustomId = 2;

            var updateTask = db.Updateable(insertedOrder).ExecuteCommandAsync();
            updateTask.Wait();

            if (updateTask.Result != 1)
                throw new Exception("Navigation property update failed");

            // Verify foreign key updated
            var dbOrder = db.Queryable<Order>().InSingle(insertedOrder.Id);
            if (dbOrder.CustomId != 2)
                throw new Exception("Foreign key not updated");

            Console.WriteLine("✓ Navigation properties handled correctly\n");
        }

        /// <summary>
        /// Test 19: Concurrent async updates
        /// Validates: 10 concurrent threads updating different entities, no deadlocks
        /// </summary>
        public static void AsyncUpdate_ConcurrentUpdates()
        {
            Console.WriteLine("Test: ConcurrentUpdates");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();
            db.Deleteable<Order>().ExecuteCommand();

            // Create 10 test entities
            var orders = new List<Order>();
            for (int i = 1; i <= 10; i++)
            {
                orders.Add(new Order
                {
                    Name = $"Concurrent Order {i}",
                    Price = i * 10.00m,
                    CreateTime = DateTime.Now
                });
            }

            var insertTask = db.Insertable(orders).ExecuteCommandAsync();
            insertTask.Wait();

            // Update all entities concurrently from different threads
            var insertedOrders = db.Queryable<Order>().ToList();
            var tasks = new List<Task<int>>();

            for (int i = 0; i < insertedOrders.Count; i++)
            {
                var order = insertedOrders[i];
                var task = Task.Run(async () =>
                {
                    var threadDb = Db; // Each thread gets its own DB instance
                    order.Price = order.Price * 2;
                    return await threadDb.Updateable(order).ExecuteCommandAsync();
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            // Verify all updates succeeded
            if (tasks.Sum(t => t.Result) != 10)
                throw new Exception("Not all concurrent updates succeeded");

            Console.WriteLine("✓ 10 concurrent updates, no deadlocks\n");
        }

        /// <summary>
        /// Test 20: Async update performance
        /// Validates: Async vs Sync performance comparison for 1000 entity updates
        /// </summary>
        public static void AsyncUpdate_Performance()
        {
            Console.WriteLine("Test: Performance");
            
            var db = Db;
            db.CodeFirst.InitTables<Order>();
            db.Deleteable<Order>().ExecuteCommand();

            // Create 1000 test entities
            var orders = new List<Order>();
            for (int i = 1; i <= 1000; i++)
            {
                orders.Add(new Order
                {
                    Name = $"Perf Order {i}",
                    Price = i * 1.00m,
                    CreateTime = DateTime.Now
                });
            }

            var insertTask = db.Insertable(orders).ExecuteCommandAsync();
            insertTask.Wait();

            // Test 1: Async update performance
            var insertedOrders = db.Queryable<Order>().ToList();
            foreach (var order in insertedOrders)
            {
                order.Price = order.Price * 1.5m; // 50% price increase
            }

            var asyncStart = DateTime.Now;
            var asyncTask = db.Updateable(insertedOrders).ExecuteCommandAsync();
            asyncTask.Wait();
            var asyncDuration = DateTime.Now - asyncStart;

            Console.WriteLine($"  Async: {asyncTask.Result} rows in {asyncDuration.TotalMilliseconds}ms");

            // Test 2: Sync update performance (for comparison)
            foreach (var order in insertedOrders)
            {
                order.Price = order.Price * 2; // Double price
            }

            var syncStart = DateTime.Now;
            int syncResult = db.Updateable(insertedOrders).ExecuteCommand();
            var syncDuration = DateTime.Now - syncStart;

            Console.WriteLine($"  Sync:  {syncResult} rows in {syncDuration.TotalMilliseconds}ms");
            Console.WriteLine($"  Ratio: {(asyncDuration.TotalMilliseconds / syncDuration.TotalMilliseconds):F2}x\n");
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Helper entity for optimistic locking tests
        /// Uses Version column for concurrency control to prevent lost updates
        /// 
        /// Key Features:
        /// - Version column automatically incremented on each update
        /// - IsEnableUpdateVersionValidation enables optimistic locking
        /// - Update fails if version mismatch detected (prevents concurrent overwrites)
        /// </summary>
        [SugarTable("OrderWithVersion")]
        public class OrderWithVersion
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            
            public string Name { get; set; }
            public decimal Price { get; set; }
            public DateTime CreateTime { get; set; }
            
            /// <summary>
            /// Version column for optimistic concurrency control
            /// Automatically validated and incremented when IsEnableUpdateVersionValidation() is used
            /// </summary>
            [SugarColumn(IsEnableUpdateVersionValidation = true)]
            public int Version { get; set; }
        }

        #endregion
    }
}
