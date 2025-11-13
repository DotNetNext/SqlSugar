using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrmTest
{
    /// <summary>
    /// Test cases for extended SqlFunc methods
    /// Demonstrates usage of newly added SQL functions
    /// </summary>
    public class ExtendedSqlFuncTests
    {
        public static void Init()
        {
            Console.WriteLine("=== Extended SqlFunc Tests ===");
            
            var db = NewUnitTest.Db;

            // Create test table
            db.CodeFirst.InitTables<TestProduct>();
            
            // Insert test data
            db.Deleteable<TestProduct>().ExecuteCommand();
            var products = new List<TestProduct>
            {
                new TestProduct { Id = 1, Name = "Apple", Price = 1.99m, Description = "Fresh Apple", CreatedDate = DateTime.Now.AddDays(-10), Quantity = 100 },
                new TestProduct { Id = 2, Name = "Banana", Price = 0.99m, Description = "Yellow Banana", CreatedDate = DateTime.Now.AddDays(-5), Quantity = 150 },
                new TestProduct { Id = 3, Name = "Cherry", Price = 2.99m, Description = "Sweet Cherry", CreatedDate = DateTime.Now.AddDays(-3), Quantity = 75 },
                new TestProduct { Id = 4, Name = "Date", Price = 3.99m, Description = "Dried Date", CreatedDate = DateTime.Now.AddDays(-1), Quantity = 50 },
                new TestProduct { Id = 5, Name = "Elderberry", Price = 4.99m, Description = "Rare Elderberry", CreatedDate = DateTime.Now, Quantity = 25 }
            };
            db.Insertable(products).ExecuteCommand();

            Console.WriteLine("Test data created successfully");

            // Run test categories
            TestStringFunctions(db);
            TestMathFunctions(db);
            TestDateFunctions(db);
            TestConversionFunctions(db);
            TestAggregateFunctions(db);
            TestWindowFunctions(db);
            TestConditionalFunctions(db);
            TestUtilityFunctions(db);

            Console.WriteLine("\n=== All Extended SqlFunc Tests Completed ===");
        }

        #region String Function Tests

        private static void TestStringFunctions(SqlSugarClient db)
        {
            Console.WriteLine("\n--- String Function Tests ---");

            try
            {
                // Test Reverse
                var reverseTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => new { 
                        Original = x.Name, 
                        Reversed = SqlFunc.Reverse(x.Name) 
                    })
                    .First();
                Console.WriteLine($"✓ Reverse: {reverseTest.Original} -> {reverseTest.Reversed}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Reverse: {ex.Message}");
            }

            try
            {
                // Test Replicate
                var replicateTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Replicate(x.Name, 3))
                    .First();
                Console.WriteLine($"✓ Replicate: {replicateTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Replicate: {ex.Message}");
            }

            try
            {
                // Test ConcatWs
                var concatTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.ConcatWs(" - ", x.Name, x.Description))
                    .First();
                Console.WriteLine($"✓ ConcatWs: {concatTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ ConcatWs: {ex.Message}");
            }

            try
            {
                // Test PadRight
                var padTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.PadRight(x.Name, 20, '*'))
                    .First();
                Console.WriteLine($"✓ PadRight: '{padTest}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ PadRight: {ex.Message}");
            }

            try
            {
                // Test Ascii
                var asciiTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Ascii(x.Name))
                    .First();
                Console.WriteLine($"✓ Ascii: {asciiTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ascii: {ex.Message}");
            }

            try
            {
                // Test Format
                var formatTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Format(x.Price, "C"))
                    .First();
                Console.WriteLine($"✓ Format: {formatTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Format: {ex.Message}");
            }
        }

        #endregion

        #region Math Function Tests

        private static void TestMathFunctions(SqlSugarClient db)
        {
            Console.WriteLine("\n--- Math Function Tests ---");

            try
            {
                // Test Power
                var powerTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Power(2.0, 3.0))
                    .First();
                Console.WriteLine($"✓ Power: 2^3 = {powerTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Power: {ex.Message}");
            }

            try
            {
                // Test Sqrt
                var sqrtTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Sqrt(16.0))
                    .First();
                Console.WriteLine($"✓ Sqrt: √16 = {sqrtTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Sqrt: {ex.Message}");
            }

            try
            {
                // Test Log
                var logTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Log(100.0))
                    .First();
                Console.WriteLine($"✓ Log: ln(100) = {logTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Log: {ex.Message}");
            }

            try
            {
                // Test Log10
                var log10Test = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Log10(1000.0))
                    .First();
                Console.WriteLine($"✓ Log10: log₁₀(1000) = {log10Test}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Log10: {ex.Message}");
            }

            try
            {
                // Test PI
                var piTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.PI())
                    .First();
                Console.WriteLine($"✓ PI: π = {piTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ PI: {ex.Message}");
            }

            try
            {
                // Test Sin, Cos, Tan
                var trigTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => new { 
                        Sin = SqlFunc.Sin(SqlFunc.PI() / 2),
                        Cos = SqlFunc.Cos(0.0),
                        Tan = SqlFunc.Tan(SqlFunc.PI() / 4)
                    })
                    .First();
                Console.WriteLine($"✓ Trig: sin(π/2)={trigTest.Sin}, cos(0)={trigTest.Cos}, tan(π/4)={trigTest.Tan}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Trig functions: {ex.Message}");
            }

            try
            {
                // Test Sign
                var signTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Sign(x.Price))
                    .First();
                Console.WriteLine($"✓ Sign: {signTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Sign: {ex.Message}");
            }
        }

        #endregion

        #region Date Function Tests

        private static void TestDateFunctions(SqlSugarClient db)
        {
            Console.WriteLine("\n--- Date Function Tests ---");

            try
            {
                // Test EndOfMonth
                var eomTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.EndOfMonth(x.CreatedDate))
                    .First();
                Console.WriteLine($"✓ EndOfMonth: {eomTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ EndOfMonth: {ex.Message}");
            }

            try
            {
                // Test Quarter
                var quarterTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Quarter(x.CreatedDate))
                    .First();
                Console.WriteLine($"✓ Quarter: Q{quarterTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Quarter: {ex.Message}");
            }

            try
            {
                // Test DayOfYear
                var dayOfYearTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.DayOfYear(x.CreatedDate))
                    .First();
                Console.WriteLine($"✓ DayOfYear: Day {dayOfYearTest} of year");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ DayOfYear: {ex.Message}");
            }

            try
            {
                // Test GetUtcDate
                var utcTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.GetUtcDate())
                    .First();
                Console.WriteLine($"✓ GetUtcDate: {utcTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ GetUtcDate: {ex.Message}");
            }

            try
            {
                // Test DateFromParts
                var datePartsTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.DateFromParts(2025, 12, 25))
                    .First();
                Console.WriteLine($"✓ DateFromParts: {datePartsTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ DateFromParts: {ex.Message}");
            }
        }

        #endregion

        #region Conversion Function Tests

        private static void TestConversionFunctions(SqlSugarClient db)
        {
            Console.WriteLine("\n--- Conversion Function Tests ---");

            try
            {
                // Test TryCast
                var tryCastTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.TryCast<int>(x.Price))
                    .First();
                Console.WriteLine($"✓ TryCast: {tryCastTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ TryCast: {ex.Message}");
            }

            try
            {
                // Test Cast
                var castTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Cast<string>(x.Price))
                    .First();
                Console.WriteLine($"✓ Cast: {castTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Cast: {ex.Message}");
            }
        }

        #endregion

        #region Aggregate Function Tests

        private static void TestAggregateFunctions(SqlSugarClient db)
        {
            Console.WriteLine("\n--- Aggregate Function Tests ---");

            try
            {
                // Test StringAgg (SQL Server 2017+)
                var stringAggTest = db.Queryable<TestProduct>()
                    .Select(x => SqlFunc.StringAgg(x.Name, ", "))
                    .First();
                Console.WriteLine($"✓ StringAgg: {stringAggTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ StringAgg: {ex.Message}");
            }

            try
            {
                // Test StDev
                var stdevTest = db.Queryable<TestProduct>()
                    .Select(x => SqlFunc.StDev(x.Price))
                    .First();
                Console.WriteLine($"✓ StDev: {stdevTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ StDev: {ex.Message}");
            }

            try
            {
                // Test Var
                var varTest = db.Queryable<TestProduct>()
                    .Select(x => SqlFunc.Var(x.Price))
                    .First();
                Console.WriteLine($"✓ Var: {varTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Var: {ex.Message}");
            }
        }

        #endregion

        #region Window Function Tests

        private static void TestWindowFunctions(SqlSugarClient db)
        {
            Console.WriteLine("\n--- Window Function Tests ---");

            try
            {
                // Test Lead
                var leadTest = db.Queryable<TestProduct>()
                    .OrderBy(x => x.Id)
                    .Select(x => new { 
                        Id = x.Id,
                        Name = x.Name,
                        NextName = SqlFunc.Lead(x.Name)
                    })
                    .ToList();
                Console.WriteLine($"✓ Lead: Found {leadTest.Count} rows with lead values");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Lead: {ex.Message}");
            }

            try
            {
                // Test Lag
                var lagTest = db.Queryable<TestProduct>()
                    .OrderBy(x => x.Id)
                    .Select(x => new { 
                        Id = x.Id,
                        Name = x.Name,
                        PrevName = SqlFunc.Lag(x.Name)
                    })
                    .ToList();
                Console.WriteLine($"✓ Lag: Found {lagTest.Count} rows with lag values");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Lag: {ex.Message}");
            }

            try
            {
                // Test FirstValue
                var firstValueTest = db.Queryable<TestProduct>()
                    .OrderBy(x => x.Price)
                    .Select(x => new { 
                        Name = x.Name,
                        FirstPrice = SqlFunc.FirstValue(x.Price)
                    })
                    .ToList();
                Console.WriteLine($"✓ FirstValue: Found {firstValueTest.Count} rows");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FirstValue: {ex.Message}");
            }

            try
            {
                // Test Ntile
                var ntileTest = db.Queryable<TestProduct>()
                    .OrderBy(x => x.Price)
                    .Select(x => new { 
                        Name = x.Name,
                        PriceGroup = SqlFunc.Ntile(3)
                    })
                    .ToList();
                Console.WriteLine($"✓ Ntile: Divided {ntileTest.Count} rows into 3 groups");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ntile: {ex.Message}");
            }

            try
            {
                // Test PercentRank
                var percentRankTest = db.Queryable<TestProduct>()
                    .OrderBy(x => x.Price)
                    .Select(x => new { 
                        Name = x.Name,
                        PercentRank = SqlFunc.PercentRank()
                    })
                    .ToList();
                Console.WriteLine($"✓ PercentRank: Calculated for {percentRankTest.Count} rows");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ PercentRank: {ex.Message}");
            }
        }

        #endregion

        #region Conditional Function Tests

        private static void TestConditionalFunctions(SqlSugarClient db)
        {
            Console.WriteLine("\n--- Conditional Function Tests ---");

            try
            {
                // Test Coalesce with 3 parameters
                var coalesceTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Coalesce(x.Description, x.Name, "Unknown"))
                    .First();
                Console.WriteLine($"✓ Coalesce (3 params): {coalesceTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Coalesce: {ex.Message}");
            }

            try
            {
                // Test NullIf
                var nullIfTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.NullIf(x.Name, "Apple"))
                    .First();
                Console.WriteLine($"✓ NullIf: {nullIfTest ?? "NULL"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ NullIf: {ex.Message}");
            }

            try
            {
                // Test Greatest
                var greatestTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Greatest(x.Price, 5.0m, 10.0m))
                    .First();
                Console.WriteLine($"✓ Greatest: {greatestTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Greatest: {ex.Message}");
            }

            try
            {
                // Test Least
                var leastTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.Least(x.Price, 5.0m, 10.0m))
                    .First();
                Console.WriteLine($"✓ Least: {leastTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Least: {ex.Message}");
            }
        }

        #endregion

        #region Utility Function Tests

        private static void TestUtilityFunctions(SqlSugarClient db)
        {
            Console.WriteLine("\n--- Utility Function Tests ---");

            try
            {
                // Test DatabaseName
                var dbNameTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.DatabaseName())
                    .First();
                Console.WriteLine($"✓ DatabaseName: {dbNameTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ DatabaseName: {ex.Message}");
            }

            try
            {
                // Test CurrentUser
                var userTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.CurrentUser())
                    .First();
                Console.WriteLine($"✓ CurrentUser: {userTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ CurrentUser: {ex.Message}");
            }

            try
            {
                // Test HostName
                var hostTest = db.Queryable<TestProduct>()
                    .Where(x => x.Id == 1)
                    .Select(x => SqlFunc.HostName())
                    .First();
                Console.WriteLine($"✓ HostName: {hostTest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ HostName: {ex.Message}");
            }
        }

        #endregion

        #region Test Entity

        [SugarTable("ExtendedSqlFunc_TestProduct")]
        public class TestProduct
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }
            
            public string Name { get; set; }
            
            public decimal Price { get; set; }
            
            public string Description { get; set; }
            
            public DateTime CreatedDate { get; set; }
            
            public int Quantity { get; set; }
        }

        #endregion
    }
}
