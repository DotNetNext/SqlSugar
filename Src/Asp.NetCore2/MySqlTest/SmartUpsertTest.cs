using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrmTest
{
    /// <summary>
    /// Smart Upsert Test Cases - Demonstrates advanced conflict resolution strategies
    /// 智能插入更新测试用例 - 演示高级冲突解决策略
    /// </summary>
    public class SmartUpsertTest
    {
        public static void Init()
        {
            Console.WriteLine("=== Smart Upsert Feature Tests ===\n");

            Test1_BasicSmartUpsert();
            Test2_UpdateNonNullStrategy();
            Test3_IncrementOnConflict();
            Test4_ColumnSpecificStrategies();
            Test5_ConditionalUpdate();
            Test6_AuditTrailAndCallbacks();
            Test7_BatchProcessing();
            Test8_SkipOnConflict();
            Test9_CustomUpdateLogic();
            Test10_AsyncOperations();

            Console.WriteLine("\n=== All Smart Upsert Tests Completed ===\n");
        }

        #region Test 1: Basic Smart Upsert
        public static void Test1_BasicSmartUpsert()
        {
            Console.WriteLine("Test 1: Basic Smart Upsert with UpdateAll Strategy");
            var db = DbHelper.GetNewDb();

            // Create table
            db.CodeFirst.InitTables<Product>();

            // Insert initial data
            var products = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = 100, Stock = 50 },
                new Product { ProductCode = "P002", Name = "Product 2", Price = 200, Stock = 30 }
            };
            db.Insertable(products).ExecuteCommand();

            // Update with SmartUpsert - P001 exists, P003 is new
            var upsertData = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Updated Product 1", Price = 150, Stock = 60 },
                new Product { ProductCode = "P003", Name = "Product 3", Price = 300, Stock = 40 }
            };

            var result = db.SmartUpsert(upsertData)
                .WhereColumns(p => new { p.ProductCode })
                .SetStrategy(ConflictResolutionStrategy.UpdateAll)
                .ExecuteCommand();

            Console.WriteLine($"  Total: {result.TotalCount}, Inserted: {result.InsertedCount}, Updated: {result.UpdatedCount}");
            Console.WriteLine($"  Execution Time: {result.ExecutionTimeMs}ms");
            Console.WriteLine($"  Success: {result.IsSuccess}\n");

            // Verify results
            var allProducts = db.Queryable<Product>().ToList();
            Console.WriteLine($"  Total products in DB: {allProducts.Count}");
            foreach (var p in allProducts.OrderBy(x => x.ProductCode))
            {
                Console.WriteLine($"    {p.ProductCode}: {p.Name}, Price={p.Price}, Stock={p.Stock}");
            }
            Console.WriteLine();
        }
        #endregion

        #region Test 2: UpdateNonNull Strategy
        public static void Test2_UpdateNonNullStrategy()
        {
            Console.WriteLine("Test 2: UpdateNonNull Strategy - Only update non-null values");
            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Product>();

            // Insert initial data
            var product = new Product { ProductCode = "P001", Name = "Original Product", Price = 100, Stock = 50 };
            db.Insertable(product).ExecuteCommand();

            // Update with some null values - only non-null fields should update
            var updateData = new Product 
            { 
                ProductCode = "P001", 
                Name = "Updated Name",  // Will update
                Price = null,           // Will NOT update (null)
                Stock = 75              // Will update
            };

            var result = db.SmartUpsert(updateData)
                .WhereColumns(p => new { p.ProductCode })
                .SetStrategy(ConflictResolutionStrategy.UpdateNonNull)
                .ExecuteCommand();

            Console.WriteLine($"  Updated: {result.UpdatedCount}");

            var updated = db.Queryable<Product>().Where(p => p.ProductCode == "P001").First();
            Console.WriteLine($"  Result: Name={updated.Name}, Price={updated.Price}, Stock={updated.Stock}");
            Console.WriteLine($"  Price should still be 100 (not updated because incoming was null)\n");
        }
        #endregion

        #region Test 3: Increment On Conflict
        public static void Test3_IncrementOnConflict()
        {
            Console.WriteLine("Test 3: IncrementOnConflict Strategy - Add to existing values");
            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Product>();

            // Insert initial inventory
            var product = new Product { ProductCode = "P001", Name = "Product 1", Price = 100, Stock = 50 };
            db.Insertable(product).ExecuteCommand();

            // Receive new stock - should ADD to existing, not replace
            var newStock = new Product { ProductCode = "P001", Name = "Product 1", Price = 100, Stock = 25 };

            var result = db.SmartUpsert(newStock)
                .WhereColumns(p => new { p.ProductCode })
                .SetStrategy(ConflictResolutionStrategy.UpdateAll)
                .SetColumnStrategy(p => p.Stock, ConflictResolutionStrategy.IncrementOnConflict)
                .ExecuteCommand();

            var updated = db.Queryable<Product>().Where(p => p.ProductCode == "P001").First();
            Console.WriteLine($"  Original Stock: 50, Added: 25, Final Stock: {updated.Stock}");
            Console.WriteLine($"  Expected: 75, Actual: {updated.Stock}\n");
        }
        #endregion

        #region Test 4: Column-Specific Strategies
        public static void Test4_ColumnSpecificStrategies()
        {
            Console.WriteLine("Test 4: Different strategies for different columns");
            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Product>();

            // Insert initial data
            var product = new Product 
            { 
                ProductCode = "P001", 
                Name = "Product 1", 
                Price = 100, 
                Stock = 50,
                LastUpdated = DateTime.Now.AddDays(-1)
            };
            db.Insertable(product).ExecuteCommand();

            // Update with mixed strategies
            var updateData = new Product 
            { 
                ProductCode = "P001", 
                Name = "Updated Name",
                Price = 90,              // Lower price - should NOT update (UpdateIfGreater)
                Stock = 20,              // Should ADD to existing (IncrementOnConflict)
                LastUpdated = DateTime.Now
            };

            var result = db.SmartUpsert(updateData)
                .WhereColumns(p => new { p.ProductCode })
                .SetStrategy(ConflictResolutionStrategy.UpdateAll)
                .SetColumnStrategy(p => p.Price, ConflictResolutionStrategy.UpdateIfGreater)
                .SetColumnStrategy(p => p.Stock, ConflictResolutionStrategy.IncrementOnConflict)
                .ExecuteCommand();

            var updated = db.Queryable<Product>().Where(p => p.ProductCode == "P001").First();
            Console.WriteLine($"  Name: {updated.Name} (should be 'Updated Name')");
            Console.WriteLine($"  Price: {updated.Price} (should still be 100, not 90)");
            Console.WriteLine($"  Stock: {updated.Stock} (should be 70 = 50 + 20)\n");
        }
        #endregion

        #region Test 5: Conditional Update
        public static void Test5_ConditionalUpdate()
        {
            Console.WriteLine("Test 5: Conditional Update - Only update if condition is met");
            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Product>();

            var product = new Product 
            { 
                ProductCode = "P001", 
                Name = "Product 1", 
                Price = 100,
                LastUpdated = DateTime.Now
            };
            db.Insertable(product).ExecuteCommand();

            // Try to update with older timestamp - should be skipped
            var oldUpdate = new Product 
            { 
                ProductCode = "P001", 
                Name = "Old Update", 
                Price = 150,
                LastUpdated = DateTime.Now.AddDays(-1)
            };

            var result = db.SmartUpsert(oldUpdate)
                .WhereColumns(p => new { p.ProductCode })
                .UpdateWhen((existing, incoming) => incoming.LastUpdated > existing.LastUpdated)
                .ExecuteCommand();

            Console.WriteLine($"  Updated: {result.UpdatedCount}, Skipped: {result.SkippedCount}");

            var current = db.Queryable<Product>().Where(p => p.ProductCode == "P001").First();
            Console.WriteLine($"  Name should still be 'Product 1': {current.Name}\n");
        }
        #endregion

        #region Test 6: Audit Trail and Callbacks
        public static void Test6_AuditTrailAndCallbacks()
        {
            Console.WriteLine("Test 6: Audit Trail and Callbacks");
            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Product>();

            var products = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Existing Product", Price = 100, Stock = 50 },
                new Product { ProductCode = "P002", Name = "New Product", Price = 200, Stock = 30 }
            };

            // Insert P001
            db.Insertable(products[0]).ExecuteCommand();

            var insertCount = 0;
            var updateCount = 0;
            var conflictCount = 0;

            var result = db.SmartUpsert(products)
                .WhereColumns(p => new { p.ProductCode })
                .EnableAuditTrail()
                .OnInsert(item => 
                {
                    insertCount++;
                    Console.WriteLine($"  [INSERT] {item.ProductCode}: {item.Name}");
                })
                .OnUpdate((existing, incoming) => 
                {
                    updateCount++;
                    Console.WriteLine($"  [UPDATE] {existing.ProductCode}: {existing.Name} -> {incoming.Name}");
                })
                .OnConflict((existing, incoming) => 
                {
                    conflictCount++;
                    Console.WriteLine($"  [CONFLICT] {existing.ProductCode}");
                })
                .ExecuteCommand();

            Console.WriteLine($"\n  Summary: {insertCount} inserts, {updateCount} updates, {conflictCount} conflicts\n");
        }
        #endregion

        #region Test 7: Batch Processing
        public static void Test7_BatchProcessing()
        {
            Console.WriteLine("Test 7: Batch Processing with Page Size");
            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Product>();

            // Generate large dataset
            var products = new List<Product>();
            for (int i = 1; i <= 5000; i++)
            {
                products.Add(new Product 
                { 
                    ProductCode = $"P{i:D4}", 
                    Name = $"Product {i}", 
                    Price = i * 10, 
                    Stock = i 
                });
            }

            var result = db.SmartUpsert(products)
                .WhereColumns(p => new { p.ProductCode })
                .PageSize(1000)  // Process in batches of 1000
                .SetStrategy(ConflictResolutionStrategy.UpdateAll)
                .ExecuteCommand();

            Console.WriteLine($"  Processed {result.TotalCount} records");
            Console.WriteLine($"  Inserted: {result.InsertedCount}");
            Console.WriteLine($"  Execution Time: {result.ExecutionTimeMs}ms");
            Console.WriteLine($"  Average: {result.ExecutionTimeMs / (double)result.TotalCount:F2}ms per record\n");
        }
        #endregion

        #region Test 8: Skip On Conflict
        public static void Test8_SkipOnConflict()
        {
            Console.WriteLine("Test 8: SkipOnConflict Strategy - Keep existing data");
            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Product>();

            var product = new Product { ProductCode = "P001", Name = "Original", Price = 100, Stock = 50 };
            db.Insertable(product).ExecuteCommand();

            var updateData = new Product { ProductCode = "P001", Name = "Should Not Update", Price = 999, Stock = 999 };

            var result = db.SmartUpsert(updateData)
                .WhereColumns(p => new { p.ProductCode })
                .SetStrategy(ConflictResolutionStrategy.SkipOnConflict)
                .ExecuteCommand();

            Console.WriteLine($"  Skipped: {result.SkippedCount}");

            var current = db.Queryable<Product>().Where(p => p.ProductCode == "P001").First();
            Console.WriteLine($"  Name should still be 'Original': {current.Name}");
            Console.WriteLine($"  Price should still be 100: {current.Price}\n");
        }
        #endregion

        #region Test 9: Custom Update Logic
        public static void Test9_CustomUpdateLogic()
        {
            Console.WriteLine("Test 9: Custom Update Logic");
            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Product>();

            var product = new Product { ProductCode = "P001", Name = "Product", Price = 100, Stock = 50 };
            db.Insertable(product).ExecuteCommand();

            var updateData = new Product { ProductCode = "P001", Name = "Updated", Price = 150, Stock = 30 };

            var result = db.SmartUpsert(updateData)
                .WhereColumns(p => new { p.ProductCode })
                .SetColumnCustomLogic(p => p.Price, (existing, incoming) =>
                {
                    // Custom logic: Use average of existing and incoming price
                    var existingPrice = (decimal?)existing ?? 0;
                    var incomingPrice = (decimal?)incoming ?? 0;
                    return (existingPrice + incomingPrice) / 2;
                })
                .ExecuteCommand();

            var updated = db.Queryable<Product>().Where(p => p.ProductCode == "P001").First();
            Console.WriteLine($"  Price (average of 100 and 150): {updated.Price}");
            Console.WriteLine($"  Expected: 125, Actual: {updated.Price}\n");
        }
        #endregion

        #region Test 10: Async Operations
        public static void Test10_AsyncOperations()
        {
            Console.WriteLine("Test 10: Async Smart Upsert");
            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Product>();

            var products = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = 100, Stock = 50 },
                new Product { ProductCode = "P002", Name = "Product 2", Price = 200, Stock = 30 }
            };

            var task = db.SmartUpsert(products)
                .WhereColumns(p => new { p.ProductCode })
                .SetStrategy(ConflictResolutionStrategy.UpdateAll)
                .ExecuteCommandAsync();

            task.Wait();
            var result = task.Result;

            Console.WriteLine($"  Async operation completed");
            Console.WriteLine($"  Inserted: {result.InsertedCount}, Updated: {result.UpdatedCount}");
            Console.WriteLine($"  Execution Time: {result.ExecutionTimeMs}ms\n");
        }
        #endregion
    }

    #region Test Entity
    [SugarTable("SmartUpsert_Product")]
    public class Product
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string ProductCode { get; set; }

        [SugarColumn(Length = 200)]
        public string Name { get; set; }

        [SugarColumn(DecimalDigits = 2)]
        public decimal? Price { get; set; }

        public int? Stock { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
    #endregion
}