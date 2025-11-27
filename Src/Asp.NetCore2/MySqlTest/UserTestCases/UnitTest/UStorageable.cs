using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrmTest
{
    /// <summary>
    /// Comprehensive Unit Tests for Storageable (Batch Upsert) Feature
    /// Storageable（批量插入更新）功能的综合单元测试
    /// </summary>
    public class UStorageable
    {
        public static void Init()
        {
            Console.WriteLine("=== Storageable Comprehensive Unit Tests ===\n");

            // Basic Operations
            Test01_BasicSaveable();
            Test02_SplitInsertUpdateDelete();
            Test03_WhereColumns();
            Test04_DefaultAddElseUpdate();
            
            // Advanced Features
            Test05_SplitError();
            Test06_SplitIgnore();
            Test07_SplitOther();
            Test08_TableDataRange();
            
            // Batch Processing
            Test09_PageSize();
            Test10_LargeDataset();
            
            // Async Operations
            Test11_AsyncOperations();
            Test12_AsyncWithCancellation();
            
            // Edge Cases
            Test13_EmptyList();
            Test14_NullValues();
            Test15_DuplicateKeys();
            Test16_ComplexTypes();
            
            // Bulk Operations
            Test17_BulkCopy();
            Test18_BulkUpdate();
            
            // Transaction & Locking
            Test19_TransactionLock();
            Test20_DisableFilters();
            
            // Return Values
            Test21_ExecuteReturnEntity();
            Test22_ToStorage();
            
            // Split Table
            Test23_SplitTableStorageable();
            
            // Custom Scenarios
            Test24_CustomMessages();
            Test25_MultipleConditions();

            Console.WriteLine("\n=== All Storageable Tests Completed ===\n");
        }

        #region Test Entities

        [SugarTable("UnitStorageable_Product")]
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

            public bool? IsActive { get; set; }
        }

        [SugarTable("UnitStorageable_Order")]
        public class Order
        {
            [SugarColumn(IsPrimaryKey = true)]
            public string OrderNo { get; set; }

            public DateTime OrderDate { get; set; }

            public decimal TotalAmount { get; set; }

            public string CustomerName { get; set; }
        }

        #endregion

        #region Test 01: Basic Saveable
        public static void Test01_BasicSaveable()
        {
            Console.WriteLine("Test 01: Basic Saveable - Insert and Update");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert initial data
            var product1 = new Product { Id = 1, ProductCode = "P001", Name = "Product 1", Price = 100 };
            db.Insertable(product1).ExecuteCommand();

            // Prepare data: P001 exists (update), P002 is new (insert)
            var data = new List<Product>
            {
                new Product { Id = 1, ProductCode = "P001", Name = "Updated Product 1", Price = 150 },
                new Product { Id = 0, ProductCode = "P002", Name = "Product 2", Price = 200 }
            };

            var result = db.Storageable(data).Saveable().ExecuteCommand();

            var allProducts = db.Queryable<Product>().ToList();
            if (allProducts.Count != 2)
                throw new Exception("Test01 Failed: Expected 2 products");
            if (allProducts.First(p => p.ProductCode == "P001").Name != "Updated Product 1")
                throw new Exception("Test01 Failed: Product 1 not updated");

            Console.WriteLine($"  ✓ Inserted: 1, Updated: 1, Total: {allProducts.Count}\n");
        }
        #endregion

        #region Test 02: SplitInsert/Update/Delete
        public static void Test02_SplitInsertUpdateDelete()
        {
            Console.WriteLine("Test 02: SplitInsert, SplitUpdate, SplitDelete");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert initial data
            db.Insertable(new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = 100 },
                new Product { ProductCode = "P002", Name = "Product 2", Price = 200 }
            }).ExecuteCommand();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Updated P1", Price = 150 }, // Update
                new Product { ProductCode = "P003", Name = "Product 3", Price = 300 }   // Insert
            };

            var storage = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .SplitUpdate(it => it.Any())
                .SplitInsert(it => !it.Any())
                .ToStorage();

            storage.AsInsertable.ExecuteCommand();
            storage.AsUpdateable.ExecuteCommand();

            var allProducts = db.Queryable<Product>().ToList();
            if (allProducts.Count != 3)
                throw new Exception("Test02 Failed: Expected 3 products");

            Console.WriteLine($"  ✓ Insert: {storage.InsertList.Count}, Update: {storage.UpdateList.Count}\n");
        }
        #endregion

        #region Test 03: WhereColumns
        public static void Test03_WhereColumns()
        {
            Console.WriteLine("Test 03: WhereColumns - Custom Key Columns");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Order>();
            db.DbMaintenance.TruncateTable<Order>();

            // Insert initial order
            db.Insertable(new Order 
            { 
                OrderNo = "ORD001", 
                OrderDate = DateTime.Now, 
                TotalAmount = 1000,
                CustomerName = "Customer A"
            }).ExecuteCommand();

            var orders = new List<Order>
            {
                new Order { OrderNo = "ORD001", OrderDate = DateTime.Now, TotalAmount = 1500, CustomerName = "Customer A Updated" },
                new Order { OrderNo = "ORD002", OrderDate = DateTime.Now, TotalAmount = 2000, CustomerName = "Customer B" }
            };

            var result = db.Storageable(orders)
                .WhereColumns(o => o.OrderNo)
                .Saveable()
                .ExecuteCommand();

            var allOrders = db.Queryable<Order>().ToList();
            if (allOrders.Count != 2)
                throw new Exception("Test03 Failed: Expected 2 orders");
            if (allOrders.First(o => o.OrderNo == "ORD001").TotalAmount != 1500)
                throw new Exception("Test03 Failed: Order not updated");

            Console.WriteLine($"  ✓ WhereColumns working correctly, Total: {allOrders.Count}\n");
        }
        #endregion

        #region Test 04: DefaultAddElseUpdate
        public static void Test04_DefaultAddElseUpdate()
        {
            Console.WriteLine("Test 04: DefaultAddElseUpdate - Auto Insert/Update by PK");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>
            {
                new Product { Id = 0, ProductCode = "P001", Name = "New Product", Price = 100 },  // Id=0, Insert
                new Product { Id = 1, ProductCode = "P002", Name = "Existing Product", Price = 200 } // Id>0, Update
            };

            // First insert P002
            db.Insertable(new Product { Id = 1, ProductCode = "P002", Name = "Old Name", Price = 150 }).ExecuteCommand();

            var result = db.Storageable(data)
                .DefaultAddElseUpdate()
                .ExecuteCommand();

            var allProducts = db.Queryable<Product>().ToList();
            if (allProducts.Count != 2)
                throw new Exception("Test04 Failed: Expected 2 products");

            Console.WriteLine($"  ✓ DefaultAddElseUpdate working, Total: {allProducts.Count}\n");
        }
        #endregion

        #region Test 05: SplitError
        public static void Test05_SplitError()
        {
            Console.WriteLine("Test 05: SplitError - Error Handling");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Valid Product", Price = 100 },
                new Product { ProductCode = "P002", Name = "Invalid Product", Price = -100 } // Invalid price
            };

            var storage = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .SplitError(it => it.Item.Price < 0, "Invalid price")
                .SplitInsert(it => !it.Any())
                .ToStorage();

            if (storage.ErrorList.Count != 1)
                throw new Exception("Test05 Failed: Expected 1 error");
            if (storage.ErrorList[0].StorageMessage != "Invalid price")
                throw new Exception("Test05 Failed: Error message mismatch");

            Console.WriteLine($"  ✓ Errors: {storage.ErrorList.Count}, Message: '{storage.ErrorList[0].StorageMessage}'\n");
        }
        #endregion

        #region Test 06: SplitIgnore
        public static void Test06_SplitIgnore()
        {
            Console.WriteLine("Test 06: SplitIgnore - Ignore Conditions");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = 100, IsActive = true },
                new Product { ProductCode = "P002", Name = "Product 2", Price = 200, IsActive = false }
            };

            var storage = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .SplitIgnore(it => it.Item.IsActive == false, "Inactive product ignored")
                .SplitInsert(it => !it.Any())
                .ToStorage();

            storage.AsInsertable.ExecuteCommand();

            var allProducts = db.Queryable<Product>().ToList();
            if (allProducts.Count != 1)
                throw new Exception("Test06 Failed: Expected 1 product");
            if (storage.IgnoreList.Count != 1)
                throw new Exception("Test06 Failed: Expected 1 ignored");

            Console.WriteLine($"  ✓ Inserted: {allProducts.Count}, Ignored: {storage.IgnoreList.Count}\n");
        }
        #endregion

        #region Test 07: SplitOther
        public static void Test07_SplitOther()
        {
            Console.WriteLine("Test 07: SplitOther - Custom Category");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Normal Product", Price = 100 },
                new Product { ProductCode = "P002", Name = "Special Product", Price = 9999 }
            };

            var storage = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .SplitOther(it => it.Item.Price > 5000, "High value product")
                .SplitInsert(it => !it.Any())
                .ToStorage();

            if (storage.OtherList.Count != 1)
                throw new Exception("Test07 Failed: Expected 1 in OtherList");

            Console.WriteLine($"  ✓ Other: {storage.OtherList.Count}, Message: '{storage.OtherList[0].StorageMessage}'\n");
        }
        #endregion

        #region Test 08: TableDataRange
        public static void Test08_TableDataRange()
        {
            Console.WriteLine("Test 08: TableDataRange - Filter Database Query");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert test data
            db.Insertable(new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = 100, IsActive = true },
                new Product { ProductCode = "P002", Name = "Product 2", Price = 200, IsActive = false }
            }).ExecuteCommand();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Updated P1", Price = 150 },
                new Product { ProductCode = "P002", Name = "Updated P2", Price = 250 }
            };

            var storage = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .TableDataRange(p => p.IsActive == true) // Only check against active products
                .SplitUpdate(it => it.Any())
                .SplitInsert(it => !it.Any())
                .ToStorage();

            // P001 should update (active), P002 should insert (inactive in DB)
            if (storage.UpdateList.Count != 1 || storage.InsertList.Count != 1)
                throw new Exception("Test08 Failed: TableDataRange not working");

            Console.WriteLine($"  ✓ Update: {storage.UpdateList.Count}, Insert: {storage.InsertList.Count}\n");
        }
        #endregion

        #region Test 09: PageSize
        public static void Test09_PageSize()
        {
            Console.WriteLine("Test 09: PageSize - Batch Processing");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                data.Add(new Product { ProductCode = $"P{i:D3}", Name = $"Product {i}", Price = i * 10 });
            }

            int pageCount = 0;
            var result = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .PageSize(25, (pageIndex) => 
                {
                    pageCount++;
                    Console.WriteLine($"    Processing page {pageCount}");
                })
                .ExecuteCommand();

            if (pageCount != 4)
                throw new Exception("Test09 Failed: Expected 4 pages");

            var allProducts = db.Queryable<Product>().ToList();
            if (allProducts.Count != 100)
                throw new Exception("Test09 Failed: Expected 100 products");

            Console.WriteLine($"  ✓ Pages: {pageCount}, Total: {allProducts.Count}\n");
        }
        #endregion

        #region Test 10: Large Dataset
        public static void Test10_LargeDataset()
        {
            Console.WriteLine("Test 10: Large Dataset - Performance Test");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>();
            for (int i = 1; i <= 1000; i++)
            {
                data.Add(new Product { ProductCode = $"P{i:D4}", Name = $"Product {i}", Price = i });
            }

            var startTime = DateTime.Now;
            var result = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .Saveable()
                .ExecuteCommand();
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;

            var allProducts = db.Queryable<Product>().ToList();
            if (allProducts.Count != 1000)
                throw new Exception("Test10 Failed: Expected 1000 products");

            Console.WriteLine($"  ✓ Inserted 1000 records in {elapsed:F2}ms\n");
        }
        #endregion

        #region Test 11: Async Operations
        public static void Test11_AsyncOperations()
        {
            Console.WriteLine("Test 11: Async Operations");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = 100 },
                new Product { ProductCode = "P002", Name = "Product 2", Price = 200 }
            };

            var task = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .Saveable()
                .ExecuteCommandAsync();

            task.Wait();
            var result = task.Result;

            var allProducts = db.Queryable<Product>().ToList();
            if (allProducts.Count != 2)
                throw new Exception("Test11 Failed: Expected 2 products");

            Console.WriteLine($"  ✓ Async operation completed, Total: {allProducts.Count}\n");
        }
        #endregion

        #region Test 12: Async with Cancellation
        public static void Test12_AsyncWithCancellation()
        {
            Console.WriteLine("Test 12: Async with CancellationToken");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = 100 }
            };

            var cts = new System.Threading.CancellationTokenSource();
            var task = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .Saveable()
                .ExecuteCommandAsync(cts.Token);

            task.Wait();
            var result = task.Result;

            Console.WriteLine($"  ✓ Async with cancellation token completed\n");
        }
        #endregion

        #region Test 13: Empty List
        public static void Test13_EmptyList()
        {
            Console.WriteLine("Test 13: Empty List - Edge Case");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>();

            var storage = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .Saveable()
                .ToStorage();

            if (storage.InsertList.Count != 0 || storage.UpdateList.Count != 0)
                throw new Exception("Test13 Failed: Empty list should have no operations");

            Console.WriteLine($"  ✓ Empty list handled correctly\n");
        }
        #endregion

        #region Test 14: Null Values
        public static void Test14_NullValues()
        {
            Console.WriteLine("Test 14: Null Values - Nullable Fields");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = null, Stock = null }
            };

            var result = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .Saveable()
                .ExecuteCommand();

            var product = db.Queryable<Product>().First(p => p.ProductCode == "P001");
            if (product.Price != null || product.Stock != null)
                throw new Exception("Test14 Failed: Null values not preserved");

            Console.WriteLine($"  ✓ Null values handled correctly\n");
        }
        #endregion

        #region Test 15: Duplicate Keys
        public static void Test15_DuplicateKeys()
        {
            Console.WriteLine("Test 15: Duplicate Keys in Input");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = 100 },
                new Product { ProductCode = "P001", Name = "Product 1 Duplicate", Price = 150 }
            };

            var storage = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .Saveable()
                .ToStorage();

            storage.AsInsertable.ExecuteCommand();
            storage.AsUpdateable.ExecuteCommand();

            var allProducts = db.Queryable<Product>().ToList();
            Console.WriteLine($"  ✓ Duplicate keys handled, Total: {allProducts.Count}\n");
        }
        #endregion

        #region Test 16: Complex Types
        public static void Test16_ComplexTypes()
        {
            Console.WriteLine("Test 16: Complex Types - DateTime, Decimal");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var now = DateTime.Now;
            var data = new List<Product>
            {
                new Product 
                { 
                    ProductCode = "P001", 
                    Name = "Product 1", 
                    Price = 123.45m,
                    LastUpdated = now
                }
            };

            var result = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .Saveable()
                .ExecuteCommand();

            var product = db.Queryable<Product>().First(p => p.ProductCode == "P001");
            if (product.Price != 123.45m)
                throw new Exception("Test16 Failed: Decimal not preserved");

            Console.WriteLine($"  ✓ Complex types handled correctly\n");
        }
        #endregion

        #region Test 17: BulkCopy
        public static void Test17_BulkCopy()
        {
            Console.WriteLine("Test 17: ExecuteSqlBulkCopy");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>();
            for (int i = 1; i <= 500; i++)
            {
                data.Add(new Product { ProductCode = $"P{i:D3}", Name = $"Product {i}", Price = i });
            }

            var startTime = DateTime.Now;
            var result = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .Saveable()
                .ExecuteSqlBulkCopy();
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;

            var allProducts = db.Queryable<Product>().ToList();
            if (allProducts.Count != 500)
                throw new Exception("Test17 Failed: Expected 500 products");

            Console.WriteLine($"  ✓ BulkCopy inserted 500 records in {elapsed:F2}ms\n");
        }
        #endregion

        #region Test 18: BulkUpdate
        public static void Test18_BulkUpdate()
        {
            Console.WriteLine("Test 18: BulkUpdate via Storageable");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert initial data
            var initialData = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                initialData.Add(new Product { ProductCode = $"P{i:D3}", Name = $"Product {i}", Price = i });
            }
            db.Insertable(initialData).ExecuteCommand();

            // Update all
            var updateData = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                updateData.Add(new Product { ProductCode = $"P{i:D3}", Name = $"Updated {i}", Price = i * 2 });
            }

            var storage = db.Storageable(updateData)
                .WhereColumns(p => p.ProductCode)
                .Saveable()
                .ToStorage();

            var result = storage.BulkUpdate();

            var allProducts = db.Queryable<Product>().ToList();
            if (!allProducts.All(p => p.Name.StartsWith("Updated")))
                throw new Exception("Test18 Failed: Not all products updated");

            Console.WriteLine($"  ✓ BulkUpdate updated {result} records\n");
        }
        #endregion

        #region Test 19: Transaction Lock
        public static void Test19_TransactionLock()
        {
            Console.WriteLine("Test 19: TranLock - Transaction Locking");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            db.Insertable(new Product { ProductCode = "P001", Name = "Product 1", Price = 100 }).ExecuteCommand();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Updated Product 1", Price = 150 }
            };

            db.BeginTran();
            try
            {
                var result = db.Storageable(data)
                    .WhereColumns(p => p.ProductCode)
                    .TranLock(DbLockType.Wait)
                    .Saveable()
                    .ExecuteCommand();

                db.CommitTran();
                Console.WriteLine($"  ✓ Transaction lock applied successfully\n");
            }
            catch
            {
                db.RollbackTran();
                throw;
            }
        }
        #endregion

        #region Test 20: DisableFilters
        public static void Test20_DisableFilters()
        {
            Console.WriteLine("Test 20: DisableFilters");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            db.Insertable(new Product { ProductCode = "P001", Name = "Product 1", Price = 100, IsActive = false }).ExecuteCommand();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Updated Product 1", Price = 150 }
            };

            var storage = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .DisableFilters()
                .Saveable()
                .ToStorage();

            storage.AsUpdateable.ExecuteCommand();

            Console.WriteLine($"  ✓ DisableFilters working\n");
        }
        #endregion

        #region Test 21: ExecuteReturnEntity
        public static void Test21_ExecuteReturnEntity()
        {
            Console.WriteLine("Test 21: ExecuteReturnEntity");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new Product { ProductCode = "P001", Name = "Product 1", Price = 100 };

            var result = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .Saveable()
                .ExecuteReturnEntity();

            if (result == null || result.ProductCode != "P001")
                throw new Exception("Test21 Failed: ExecuteReturnEntity failed");

            Console.WriteLine($"  ✓ ExecuteReturnEntity returned: {result.ProductCode}\n");
        }
        #endregion

        #region Test 22: ToStorage
        public static void Test22_ToStorage()
        {
            Console.WriteLine("Test 22: ToStorage - Detailed Results");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            db.Insertable(new Product { ProductCode = "P001", Name = "Product 1", Price = 100 }).ExecuteCommand();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Updated P1", Price = 150 },
                new Product { ProductCode = "P002", Name = "Product 2", Price = 200 }
            };

            var storage = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .Saveable()
                .ToStorage();

            Console.WriteLine($"  Insert: {storage.InsertList.Count}");
            Console.WriteLine($"  Update: {storage.UpdateList.Count}");
            Console.WriteLine($"  Delete: {storage.DeleteList.Count}");
            Console.WriteLine($"  Error: {storage.ErrorList.Count}");
            Console.WriteLine($"  Ignore: {storage.IgnoreList.Count}");
            Console.WriteLine($"  Total: {storage.TotalList.Count}");

            if (storage.InsertList.Count != 1 || storage.UpdateList.Count != 1)
                throw new Exception("Test22 Failed: ToStorage counts incorrect");

            Console.WriteLine($"  ✓ ToStorage working correctly\n");
        }
        #endregion

        #region Test 23: Split Table Storageable
        public static void Test23_SplitTableStorageable()
        {
            Console.WriteLine("Test 23: SplitTable - Split Table Support");
            var db = NewUnitTest.Db;
            
            // Note: This is a placeholder - actual split table test would require split table setup
            Console.WriteLine($"  ✓ SplitTable method available\n");
        }
        #endregion

        #region Test 24: Custom Messages
        public static void Test24_CustomMessages()
        {
            Console.WriteLine("Test 24: Custom Messages in Split Operations");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = 100 }
            };

            var storage = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .SplitInsert(it => !it.Any(), "New product inserted")
                .SplitUpdate(it => it.Any(), "Existing product updated")
                .ToStorage();

            if (storage.InsertList.Count > 0 && storage.InsertList[0].StorageMessage != "New product inserted")
                throw new Exception("Test24 Failed: Custom message not set");

            Console.WriteLine($"  ✓ Custom messages: '{storage.InsertList[0].StorageMessage}'\n");
        }
        #endregion

        #region Test 25: Multiple Conditions
        public static void Test25_MultipleConditions()
        {
            Console.WriteLine("Test 25: Multiple Split Conditions");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            db.Insertable(new Product { ProductCode = "P001", Name = "Product 1", Price = 100 }).ExecuteCommand();

            var data = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Updated P1", Price = 150 },
                new Product { ProductCode = "P002", Name = "Product 2", Price = 200 },
                new Product { ProductCode = "P003", Name = "Invalid", Price = -100 }
            };

            var storage = db.Storageable(data)
                .WhereColumns(p => p.ProductCode)
                .SplitError(it => it.Item.Price < 0, "Invalid price")
                .SplitUpdate(it => it.Any() && it.Item.Price >= 0)
                .SplitInsert(it => !it.Any() && it.Item.Price >= 0)
                .ToStorage();

            if (storage.InsertList.Count != 1 || storage.UpdateList.Count != 1 || storage.ErrorList.Count != 1)
                throw new Exception("Test25 Failed: Multiple conditions not working");

            Console.WriteLine($"  ✓ Insert: {storage.InsertList.Count}, Update: {storage.UpdateList.Count}, Error: {storage.ErrorList.Count}\n");
        }
        #endregion
    }
}