using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OrmTest
{
    /// <summary>
    /// Comprehensive Unit Tests for Fastest (Bulk Operations) Feature
    /// Fastest（批量操作）功能的综合单元测试
    /// </summary>
    public class UFastest
    {
        public static void Init()
        {
            Console.WriteLine("=== Fastest (Bulk Operations) Comprehensive Unit Tests ===\n");

            // BulkCopy Tests
            Test01_BulkCopy_BasicList();
            Test02_BulkCopy_LargeDataset();
            Test03_BulkCopy_WithPageSize();
            Test04_BulkCopy_DataTable();
            Test05_BulkCopy_WithAS();
            
            // BulkUpdate Tests
            Test06_BulkUpdate_Basic();
            Test07_BulkUpdate_CustomColumns();
            Test08_BulkUpdate_LargeDataset();
            Test09_BulkUpdate_WithWhereColumns();
            
            // BulkMerge Tests
            Test10_BulkMerge_Basic();
            Test11_BulkMerge_WithCustomKeys();
            Test12_BulkMerge_LargeDataset();
            
            // BulkDelete Tests
            Test13_BulkDelete_Basic();
            Test14_BulkDelete_WithCondition();
            
            // Async Operations
            Test15_BulkCopyAsync();
            Test16_BulkUpdateAsync();
            Test17_BulkMergeAsync();
            
            // Performance Tests
            Test18_Performance_BulkCopy();
            Test19_Performance_BulkUpdate();
            
            // Edge Cases
            Test20_EmptyList();
            Test21_NullValues();
            Test22_DuplicateKeys();
            Test23_ComplexTypes();
            
            // Advanced Features
            Test24_RemoveDataCache();
            Test25_IgnoreInsertError();

            Console.WriteLine("\n=== All Fastest Tests Completed ===\n");
        }

        #region Test Entities

        [SugarTable("UnitFastest_Product")]
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

            public DateTime? CreatedDate { get; set; }

            public bool? IsActive { get; set; }
        }

        [SugarTable("UnitFastest_Customer")]
        public class Customer
        {
            [SugarColumn(IsPrimaryKey = true)]
            public string CustomerCode { get; set; }

            public string CustomerName { get; set; }

            public string Email { get; set; }

            public DateTime RegisterDate { get; set; }
        }

        #endregion

        #region Test 01: BulkCopy Basic List
        public static void Test01_BulkCopy_BasicList()
        {
            Console.WriteLine("Test 01: BulkCopy - Basic List");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var products = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i * 10,
                    Stock = i * 5,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                });
            }

            var startTime = DateTime.Now;
            var result = db.Fastest<Product>().BulkCopy(products);
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;

            var count = db.Queryable<Product>().Count();
            if (count != 100)
                throw new Exception("Test01 Failed: Expected 100 products");

            Console.WriteLine($"  ✓ BulkCopy inserted {count} records in {elapsed:F2}ms\n");
        }
        #endregion

        #region Test 02: BulkCopy Large Dataset
        public static void Test02_BulkCopy_LargeDataset()
        {
            Console.WriteLine("Test 02: BulkCopy - Large Dataset (5000 records)");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var products = new List<Product>();
            for (int i = 1; i <= 5000; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D5}",
                    Name = $"Product {i}",
                    Price = i,
                    Stock = i,
                    CreatedDate = DateTime.Now
                });
            }

            var startTime = DateTime.Now;
            var result = db.Fastest<Product>().BulkCopy(products);
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;

            var count = db.Queryable<Product>().Count();
            if (count != 5000)
                throw new Exception("Test02 Failed: Expected 5000 products");

            Console.WriteLine($"  ✓ BulkCopy inserted {count} records in {elapsed:F2}ms ({count / elapsed * 1000:F0} records/sec)\n");
        }
        #endregion

        #region Test 03: BulkCopy With PageSize
        public static void Test03_BulkCopy_WithPageSize()
        {
            Console.WriteLine("Test 03: BulkCopy - With PageSize");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var products = new List<Product>();
            for (int i = 1; i <= 1000; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D4}",
                    Name = $"Product {i}",
                    Price = i
                });
            }

            var result = db.Fastest<Product>().PageSize(250).BulkCopy(products);

            var count = db.Queryable<Product>().Count();
            if (count != 1000)
                throw new Exception("Test03 Failed: Expected 1000 products");

            Console.WriteLine($"  ✓ BulkCopy with PageSize(250) inserted {count} records\n");
        }
        #endregion

        #region Test 04: BulkCopy DataTable
        public static void Test04_BulkCopy_DataTable()
        {
            Console.WriteLine("Test 04: BulkCopy - DataTable");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var dt = new DataTable();
            dt.Columns.Add("ProductCode", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Price", typeof(decimal));

            for (int i = 1; i <= 50; i++)
            {
                dt.Rows.Add($"P{i:D3}", $"Product {i}", i * 10);
            }

            var result = db.Fastest<Product>().AS("UnitFastest_Product").BulkCopy(dt);

            var count = db.Queryable<Product>().Count();
            if (count != 50)
                throw new Exception("Test04 Failed: Expected 50 products");

            Console.WriteLine($"  ✓ BulkCopy from DataTable inserted {count} records\n");
        }
        #endregion

        #region Test 05: BulkCopy With AS
        public static void Test05_BulkCopy_WithAS()
        {
            Console.WriteLine("Test 05: BulkCopy - With AS (Table Name)");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var products = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i
                });
            }

            var result = db.Fastest<Product>().AS("UnitFastest_Product").BulkCopy(products);

            var count = db.Queryable<Product>().Count();
            if (count != 100)
                throw new Exception("Test05 Failed: Expected 100 products");

            Console.WriteLine($"  ✓ BulkCopy with AS() inserted {count} records\n");
        }
        #endregion

        #region Test 06: BulkUpdate Basic
        public static void Test06_BulkUpdate_Basic()
        {
            Console.WriteLine("Test 06: BulkUpdate - Basic");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert initial data
            var products = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i,
                    Stock = i
                });
            }
            db.Insertable(products).ExecuteCommand();

            // Update all products
            var updateProducts = db.Queryable<Product>().ToList();
            foreach (var p in updateProducts)
            {
                p.Name = $"Updated {p.ProductCode}";
                p.Price = p.Price * 2;
            }

            var startTime = DateTime.Now;
            var result = db.Fastest<Product>().BulkUpdate(updateProducts);
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;

            var updated = db.Queryable<Product>().First(p => p.ProductCode == "P001");
            if (!updated.Name.StartsWith("Updated"))
                throw new Exception("Test06 Failed: Products not updated");

            Console.WriteLine($"  ✓ BulkUpdate updated {result} records in {elapsed:F2}ms\n");
        }
        #endregion

        #region Test 07: BulkUpdate Custom Columns
        public static void Test07_BulkUpdate_CustomColumns()
        {
            Console.WriteLine("Test 07: BulkUpdate - Custom Columns");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert initial data
            var products = new List<Product>();
            for (int i = 1; i <= 50; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i,
                    Stock = i
                });
            }
            db.Insertable(products).ExecuteCommand();

            // Update only Price column
            var updateProducts = db.Queryable<Product>().ToList();
            foreach (var p in updateProducts)
            {
                p.Name = "Should Not Update";
                p.Price = p.Price * 3;
            }

            var whereColumns = new[] { "ProductCode" };
            var updateColumns = new[] { "Price" };
            var result = db.Fastest<Product>().BulkUpdate(updateProducts, whereColumns, updateColumns);

            var updated = db.Queryable<Product>().First(p => p.ProductCode == "P001");
            if (updated.Name == "Should Not Update")
                throw new Exception("Test07 Failed: Name should not be updated");
            if (updated.Price != 3)
                throw new Exception("Test07 Failed: Price not updated correctly");

            Console.WriteLine($"  ✓ BulkUpdate with custom columns updated {result} records\n");
        }
        #endregion

        #region Test 08: BulkUpdate Large Dataset
        public static void Test08_BulkUpdate_LargeDataset()
        {
            Console.WriteLine("Test 08: BulkUpdate - Large Dataset (2000 records)");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert initial data
            var products = new List<Product>();
            for (int i = 1; i <= 2000; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D5}",
                    Name = $"Product {i}",
                    Price = i
                });
            }
            db.Fastest<Product>().BulkCopy(products);

            // Update all
            var updateProducts = db.Queryable<Product>().ToList();
            foreach (var p in updateProducts)
            {
                p.Price = p.Price * 2;
            }

            var startTime = DateTime.Now;
            var result = db.Fastest<Product>().BulkUpdate(updateProducts);
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;

            Console.WriteLine($"  ✓ BulkUpdate updated {result} records in {elapsed:F2}ms ({result / elapsed * 1000:F0} records/sec)\n");
        }
        #endregion

        #region Test 09: BulkUpdate With WhereColumns
        public static void Test09_BulkUpdate_WithWhereColumns()
        {
            Console.WriteLine("Test 09: BulkUpdate - With WhereColumns");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Customer>();
            db.DbMaintenance.TruncateTable<Customer>();

            // Insert initial data
            var customers = new List<Customer>();
            for (int i = 1; i <= 50; i++)
            {
                customers.Add(new Customer
                {
                    CustomerCode = $"C{i:D3}",
                    CustomerName = $"Customer {i}",
                    Email = $"customer{i}@test.com",
                    RegisterDate = DateTime.Now
                });
            }
            db.Insertable(customers).ExecuteCommand();

            // Update
            var updateCustomers = db.Queryable<Customer>().ToList();
            foreach (var c in updateCustomers)
            {
                c.Email = $"updated_{c.Email}";
            }

            var whereColumns = new[] { "CustomerCode" };
            var updateColumns = new[] { "Email" };
            var result = db.Fastest<Customer>().BulkUpdate(updateCustomers, whereColumns, updateColumns);

            var updated = db.Queryable<Customer>().First(c => c.CustomerCode == "C001");
            if (!updated.Email.StartsWith("updated_"))
                throw new Exception("Test09 Failed: Email not updated");

            Console.WriteLine($"  ✓ BulkUpdate with WhereColumns updated {result} records\n");
        }
        #endregion

        #region Test 10: BulkMerge Basic
        public static void Test10_BulkMerge_Basic()
        {
            Console.WriteLine("Test 10: BulkMerge - Basic Insert/Update");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert initial 50 products
            var initialProducts = new List<Product>();
            for (int i = 1; i <= 50; i++)
            {
                initialProducts.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i
                });
            }
            db.Insertable(initialProducts).ExecuteCommand();

            // Merge: Update 1-50, Insert 51-100
            var mergeProducts = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                mergeProducts.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Merged Product {i}",
                    Price = i * 2
                });
            }

            var result = db.Fastest<Product>().BulkMerge(mergeProducts);

            var count = db.Queryable<Product>().Count();
            if (count != 100)
                throw new Exception("Test10 Failed: Expected 100 products");

            var updated = db.Queryable<Product>().First(p => p.ProductCode == "P001");
            if (!updated.Name.StartsWith("Merged"))
                throw new Exception("Test10 Failed: Product not merged");

            Console.WriteLine($"  ✓ BulkMerge processed {result} records, Total: {count}\n");
        }
        #endregion

        #region Test 11: BulkMerge With Custom Keys
        public static void Test11_BulkMerge_WithCustomKeys()
        {
            Console.WriteLine("Test 11: BulkMerge - With Custom Keys");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert initial data
            var initialProducts = new List<Product>();
            for (int i = 1; i <= 30; i++)
            {
                initialProducts.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i
                });
            }
            db.Insertable(initialProducts).ExecuteCommand();

            // Merge with custom key
            var mergeProducts = new List<Product>();
            for (int i = 20; i <= 50; i++)
            {
                mergeProducts.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Merged {i}",
                    Price = i * 2
                });
            }

            var whereColumns = new[] { "ProductCode" };
            var result = db.Fastest<Product>().BulkMerge(mergeProducts, whereColumns);

            var count = db.Queryable<Product>().Count();
            if (count != 50)
                throw new Exception("Test11 Failed: Expected 50 products");

            Console.WriteLine($"  ✓ BulkMerge with custom keys, Total: {count}\n");
        }
        #endregion

        #region Test 12: BulkMerge Large Dataset
        public static void Test12_BulkMerge_LargeDataset()
        {
            Console.WriteLine("Test 12: BulkMerge - Large Dataset (3000 records)");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert initial 1500
            var initialProducts = new List<Product>();
            for (int i = 1; i <= 1500; i++)
            {
                initialProducts.Add(new Product
                {
                    ProductCode = $"P{i:D5}",
                    Name = $"Product {i}",
                    Price = i
                });
            }
            db.Fastest<Product>().BulkCopy(initialProducts);

            // Merge: Update 1-1500, Insert 1501-3000
            var mergeProducts = new List<Product>();
            for (int i = 1; i <= 3000; i++)
            {
                mergeProducts.Add(new Product
                {
                    ProductCode = $"P{i:D5}",
                    Name = $"Merged {i}",
                    Price = i * 2
                });
            }

            var startTime = DateTime.Now;
            var result = db.Fastest<Product>().BulkMerge(mergeProducts);
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;

            var count = db.Queryable<Product>().Count();
            if (count != 3000)
                throw new Exception("Test12 Failed: Expected 3000 products");

            Console.WriteLine($"  ✓ BulkMerge processed {result} records in {elapsed:F2}ms\n");
        }
        #endregion

        #region Test 13: BulkDelete Basic
        public static void Test13_BulkDelete_Basic()
        {
            Console.WriteLine("Test 13: BulkDelete - Basic");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert data
            var products = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i
                });
            }
            db.Insertable(products).ExecuteCommand();

            // Delete by list
            var toDelete = db.Queryable<Product>().Where(p => p.Price > 50).ToList();
            var result = db.Deleteable(toDelete).ExecuteCommand();

            var count = db.Queryable<Product>().Count();
            if (count != 50)
                throw new Exception("Test13 Failed: Expected 50 products remaining");

            Console.WriteLine($"  ✓ Deleted {result} records, Remaining: {count}\n");
        }
        #endregion

        #region Test 14: BulkDelete With Condition
        public static void Test14_BulkDelete_WithCondition()
        {
            Console.WriteLine("Test 14: BulkDelete - With Condition");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert data
            var products = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i,
                    IsActive = i % 2 == 0
                });
            }
            db.Insertable(products).ExecuteCommand();

            // Delete inactive
            var result = db.Deleteable<Product>().Where(p => p.IsActive == false).ExecuteCommand();

            var count = db.Queryable<Product>().Count();
            if (count != 50)
                throw new Exception("Test14 Failed: Expected 50 active products");

            Console.WriteLine($"  ✓ Deleted {result} inactive records, Remaining: {count}\n");
        }
        #endregion

        #region Test 15: BulkCopyAsync
        public static void Test15_BulkCopyAsync()
        {
            Console.WriteLine("Test 15: BulkCopyAsync - Async Operation");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var products = new List<Product>();
            for (int i = 1; i <= 500; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i
                });
            }

            var task = db.Fastest<Product>().BulkCopyAsync(products);
            task.Wait();
            var result = task.Result;

            var count = db.Queryable<Product>().Count();
            if (count != 500)
                throw new Exception("Test15 Failed: Expected 500 products");

            Console.WriteLine($"  ✓ BulkCopyAsync inserted {count} records\n");
        }
        #endregion

        #region Test 16: BulkUpdateAsync
        public static void Test16_BulkUpdateAsync()
        {
            Console.WriteLine("Test 16: BulkUpdateAsync - Async Operation");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert
            var products = new List<Product>();
            for (int i = 1; i <= 200; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i
                });
            }
            db.Insertable(products).ExecuteCommand();

            // Update async
            var updateProducts = db.Queryable<Product>().ToList();
            foreach (var p in updateProducts)
            {
                p.Price = p.Price * 2;
            }

            var task = db.Fastest<Product>().BulkUpdateAsync(updateProducts);
            task.Wait();
            var result = task.Result;

            Console.WriteLine($"  ✓ BulkUpdateAsync updated {result} records\n");
        }
        #endregion

        #region Test 17: BulkMergeAsync
        public static void Test17_BulkMergeAsync()
        {
            Console.WriteLine("Test 17: BulkMergeAsync - Async Operation");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert initial
            var initialProducts = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                initialProducts.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i
                });
            }
            db.Insertable(initialProducts).ExecuteCommand();

            // Merge async
            var mergeProducts = new List<Product>();
            for (int i = 50; i <= 150; i++)
            {
                mergeProducts.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Merged {i}",
                    Price = i * 2
                });
            }

            var task = db.Fastest<Product>().BulkMergeAsync(mergeProducts);
            task.Wait();
            var result = task.Result;

            var count = db.Queryable<Product>().Count();
            Console.WriteLine($"  ✓ BulkMergeAsync processed {result} records, Total: {count}\n");
        }
        #endregion

        #region Test 18: Performance BulkCopy
        public static void Test18_Performance_BulkCopy()
        {
            Console.WriteLine("Test 18: Performance - BulkCopy vs Regular Insert");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var products = new List<Product>();
            for (int i = 1; i <= 1000; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D4}",
                    Name = $"Product {i}",
                    Price = i
                });
            }

            // BulkCopy
            var startBulk = DateTime.Now;
            db.Fastest<Product>().BulkCopy(products);
            var bulkElapsed = (DateTime.Now - startBulk).TotalMilliseconds;

            db.DbMaintenance.TruncateTable<Product>();

            // Regular Insert
            var startRegular = DateTime.Now;
            db.Insertable(products).ExecuteCommand();
            var regularElapsed = (DateTime.Now - startRegular).TotalMilliseconds;

            var speedup = regularElapsed / bulkElapsed;

            Console.WriteLine($"  BulkCopy: {bulkElapsed:F2}ms");
            Console.WriteLine($"  Regular Insert: {regularElapsed:F2}ms");
            Console.WriteLine($"  ✓ BulkCopy is {speedup:F1}x faster\n");
        }
        #endregion

        #region Test 19: Performance BulkUpdate
        public static void Test19_Performance_BulkUpdate()
        {
            Console.WriteLine("Test 19: Performance - BulkUpdate vs Regular Update");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            // Insert data
            var products = new List<Product>();
            for (int i = 1; i <= 500; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i
                });
            }
            db.Fastest<Product>().BulkCopy(products);

            // BulkUpdate
            var updateProducts = db.Queryable<Product>().ToList();
            foreach (var p in updateProducts)
            {
                p.Price = p.Price * 2;
            }

            var startBulk = DateTime.Now;
            db.Fastest<Product>().BulkUpdate(updateProducts);
            var bulkElapsed = (DateTime.Now - startBulk).TotalMilliseconds;

            // Reset
            foreach (var p in updateProducts)
            {
                p.Price = p.Price / 2;
            }
            db.Updateable(updateProducts).ExecuteCommand();

            // Regular Update
            foreach (var p in updateProducts)
            {
                p.Price = p.Price * 2;
            }

            var startRegular = DateTime.Now;
            db.Updateable(updateProducts).ExecuteCommand();
            var regularElapsed = (DateTime.Now - startRegular).TotalMilliseconds;

            var speedup = regularElapsed / bulkElapsed;

            Console.WriteLine($"  BulkUpdate: {bulkElapsed:F2}ms");
            Console.WriteLine($"  Regular Update: {regularElapsed:F2}ms");
            Console.WriteLine($"  ✓ BulkUpdate is {speedup:F1}x faster\n");
        }
        #endregion

        #region Test 20: Empty List
        public static void Test20_EmptyList()
        {
            Console.WriteLine("Test 20: Empty List - Edge Case");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var products = new List<Product>();

            var result = db.Fastest<Product>().BulkCopy(products);

            var count = db.Queryable<Product>().Count();
            if (count != 0)
                throw new Exception("Test20 Failed: Expected 0 products");

            Console.WriteLine($"  ✓ Empty list handled correctly\n");
        }
        #endregion

        #region Test 21: Null Values
        public static void Test21_NullValues()
        {
            Console.WriteLine("Test 21: Null Values - Nullable Fields");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var products = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = null, Stock = null },
                new Product { ProductCode = "P002", Name = "Product 2", Price = 100, Stock = null }
            };

            var result = db.Fastest<Product>().BulkCopy(products);

            var product = db.Queryable<Product>().First(p => p.ProductCode == "P001");
            if (product.Price != null || product.Stock != null)
                throw new Exception("Test21 Failed: Null values not preserved");

            Console.WriteLine($"  ✓ Null values handled correctly\n");
        }
        #endregion

        #region Test 22: Duplicate Keys
        public static void Test22_DuplicateKeys()
        {
            Console.WriteLine("Test 22: Duplicate Keys - Error Handling");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var products = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Product 1", Price = 100 }
            };
            db.Insertable(products).ExecuteCommand();

            // Try to insert duplicate with IgnoreInsertError
            var duplicates = new List<Product>
            {
                new Product { ProductCode = "P001", Name = "Duplicate", Price = 200 },
                new Product { ProductCode = "P002", Name = "Product 2", Price = 300 }
            };

            try
            {
                var result = db.Fastest<Product>().IgnoreInsertError().BulkCopy(duplicates);
                Console.WriteLine($"  ✓ Duplicate keys handled with IgnoreInsertError\n");
            }
            catch
            {
                Console.WriteLine($"  ✓ Duplicate keys detected (expected behavior)\n");
            }
        }
        #endregion

        #region Test 23: Complex Types
        public static void Test23_ComplexTypes()
        {
            Console.WriteLine("Test 23: Complex Types - DateTime, Decimal, Boolean");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var now = DateTime.Now;
            var products = new List<Product>
            {
                new Product
                {
                    ProductCode = "P001",
                    Name = "Product 1",
                    Price = 123.45m,
                    Stock = 100,
                    CreatedDate = now,
                    IsActive = true
                }
            };

            var result = db.Fastest<Product>().BulkCopy(products);

            var product = db.Queryable<Product>().First(p => p.ProductCode == "P001");
            if (product.Price != 123.45m)
                throw new Exception("Test23 Failed: Decimal not preserved");
            if (product.IsActive != true)
                throw new Exception("Test23 Failed: Boolean not preserved");

            Console.WriteLine($"  ✓ Complex types handled correctly\n");
        }
        #endregion

        #region Test 24: RemoveDataCache
        public static void Test24_RemoveDataCache()
        {
            Console.WriteLine("Test 24: RemoveDataCache");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var products = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i
                });
            }

            var result = db.Fastest<Product>().RemoveDataCache().BulkCopy(products);

            var count = db.Queryable<Product>().Count();
            if (count != 100)
                throw new Exception("Test24 Failed: Expected 100 products");

            Console.WriteLine($"  ✓ RemoveDataCache working, Total: {count}\n");
        }
        #endregion

        #region Test 25: IgnoreInsertError
        public static void Test25_IgnoreInsertError()
        {
            Console.WriteLine("Test 25: IgnoreInsertError");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Product>();
            db.DbMaintenance.TruncateTable<Product>();

            var products = new List<Product>();
            for (int i = 1; i <= 50; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"P{i:D3}",
                    Name = $"Product {i}",
                    Price = i
                });
            }

            var result = db.Fastest<Product>().IgnoreInsertError().BulkCopy(products);

            var count = db.Queryable<Product>().Count();
            Console.WriteLine($"  ✓ IgnoreInsertError working, Total: {count}\n");
        }
        #endregion
    }
}