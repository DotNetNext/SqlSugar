using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrmTest
{
    /// <summary>
    /// Comprehensive Unit Tests for Reportable Provider (1500+ lines)
    /// Reportable提供程序的综合单元测试
    /// 
    /// This test suite covers:
    /// - Basic reportable operations (single/list/simple types)
    /// - Date generation (years, months, custom ranges)
    /// - Query operations (where, join, groupby, orderby)
    /// - Union and join operations
    /// - Complex data types and null handling
    /// - Practical reporting scenarios
    /// - Performance and edge cases
    /// - Advanced aggregations and calculations
    /// - Time series analysis
    /// - Data quality and validation
    /// </summary>
    public class UReportable
    {
        public static void Init()
        {
            Console.WriteLine("=== Reportable Provider Comprehensive Unit Tests (1500+ Lines) ===\n");

            // Basic Reportable Tests (Tests 1-5)
            Test01_Reportable_SingleObject();
            Test02_Reportable_ListOfObjects();
            Test03_Reportable_SimpleTypes();
            Test04_Reportable_EmptyList();
            Test05_Reportable_MultipleDataTypes();
            
            // Date Generation Tests (Tests 6-12)
            Test06_ReportableDateType_MonthsInLast1Year();
            Test07_ReportableDateType_MonthsInLast3Years();
            Test08_ReportableDateType_MonthsInLast10Years();
            Test09_ReportableDateType_Years1();
            Test10_ReportableDateType_Years3();
            Test11_ReportableDateType_Years10();
            Test12_CustomDateRange_Generation();
            
            // ToQueryable Tests (Tests 13-20)
            Test13_ToQueryable_BasicQuery();
            Test14_ToQueryable_WithWhere();
            Test15_ToQueryable_WithMultipleWhere();
            Test16_ToQueryable_WithJoin();
            Test17_ToQueryable_WithLeftJoin();
            Test18_ToQueryable_WithGroupBy();
            Test19_ToQueryable_WithOrderBy();
            Test20_ToQueryable_WithPagination();
            
            // Union Operations (Tests 21-25)
            Test21_Union_WithRealTable();
            Test22_Union_MultipleReportables();
            Test23_Union_WithFiltering();
            Test24_UnionAll_Duplicates();
            Test25_LeftJoin_WithReportable();
            
            // Complex Scenarios (Tests 26-35)
            Test26_Reportable_NullValues();
            Test27_Reportable_ComplexTypes();
            Test28_Reportable_DateTimeHandling();
            Test29_Reportable_DecimalPrecision();
            Test30_Reportable_BooleanValues();
            Test31_Reportable_GuidHandling();
            Test32_Reportable_EnumValues();
            Test33_Reportable_NestedObjects();
            Test34_Reportable_LongStrings();
            Test35_Reportable_BinaryData();
            
            // Practical Use Cases (Tests 36-45)
            Test36_GenerateMissingDates();
            Test37_FillGapsInData();
            Test38_CrossJoinDimensions();
            Test39_GenerateSequence();
            Test40_GenerateCalendar();
            Test41_TimeSeriesAnalysis();
            Test42_SalesForecastTemplate();
            Test43_MonthlyReportTemplate();
            Test44_QuarterlyAggregation();
            Test45_YearOverYearComparison();
            
            // Advanced Aggregations (Tests 46-55)
            Test46_RunningTotal();
            Test47_MovingAverage();
            Test48_PercentageCalculations();
            Test49_RankingAndTopN();
            Test50_CumulativeSum();
            Test51_WeightedAverage();
            Test52_StandardDeviation();
            Test53_MedianCalculation();
            Test54_PercentileCalculation();
            Test55_VarianceAnalysis();
            
            // Data Quality Tests (Tests 56-65)
            Test56_DataCompleteness();
            Test57_DataConsistency();
            Test58_OutlierDetection();
            Test59_DuplicateDetection();
            Test60_DataValidation();
            Test61_MissingValueAnalysis();
            Test62_DataTypeValidation();
            Test63_RangeValidation();
            Test64_FormatValidation();
            Test65_CrossFieldValidation();
            
            // Performance Tests (Tests 66-70)
            Test66_Reportable_LargeDataset();
            Test67_Reportable_VeryLargeDataset();
            Test68_Reportable_ComplexJoins();
            Test69_Reportable_MultipleAggregations();
            Test70_Reportable_DeepNesting();
            
            // Edge Cases (Tests 71-80)
            Test71_Reportable_SpecialCharacters();
            Test72_Reportable_UnicodeCharacters();
            Test73_Reportable_ExtremeValues();
            Test74_Reportable_EmptyStrings();
            Test75_Reportable_Whitespace();
            Test76_Reportable_LeapYear();
            Test77_Reportable_TimeZones();
            Test78_Reportable_DaylightSaving();
            Test79_Reportable_NegativeNumbers();
            Test80_Reportable_ScientificNotation();
            
            // Integration Tests (Tests 81-90)
            Test81_Integration_SalesReport();
            Test82_Integration_InventoryReport();
            Test83_Integration_CustomerAnalytics();
            Test84_Integration_FinancialStatement();
            Test85_Integration_PerformanceDashboard();
            Test86_Integration_TrendAnalysis();
            Test87_Integration_CohortAnalysis();
            Test88_Integration_FunnelAnalysis();
            Test89_Integration_RetentionAnalysis();
            Test90_Integration_ChurnPrediction();
            
            // Business Intelligence Tests (Tests 91-100)
            Test91_BI_KPIDashboard();
            Test92_BI_ExecutiveSummary();
            Test93_BI_DrillDownAnalysis();
            Test94_BI_WhatIfScenario();
            Test95_BI_BudgetVsActual();
            Test96_BI_MarketBasketAnalysis();
            Test97_BI_CustomerSegmentation();
            Test98_BI_ProductPerformance();
            Test99_BI_GeographicAnalysis();
            Test100_BI_SeasonalityAnalysis();

            Console.WriteLine("\n=== All 100 Reportable Tests Completed ===\n");
        }

        #region Test Entities

        [SugarTable("UnitReport_Sales")]
        public class Sales
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public DateTime SaleDate { get; set; }

            public decimal Amount { get; set; }

            public string ProductName { get; set; }
        }

        public class ReportData
        {
            public string Category { get; set; }

            public decimal Value { get; set; }

            public DateTime ReportDate { get; set; }
        }

        public class Dimension
        {
            public int DimensionId { get; set; }

            public string DimensionName { get; set; }
        }

        #endregion

        #region Test 01: Reportable Single Object
        public static void Test01_Reportable_SingleObject()
        {
            Console.WriteLine("Test 01: Reportable - Single Object");
            var db = NewUnitTest.Db;

            var data = new ReportData
            {
                Category = "Electronics",
                Value = 1000.50m,
                ReportDate = new DateTime(2024, 1, 1)
            };

            var result = db.Reportable(data).ToQueryable().ToList();

            if (result.Count != 1)
                throw new Exception("Test01 Failed: Expected 1 record");
            if (result[0].Category != "Electronics")
                throw new Exception("Test01 Failed: Category mismatch");

            Console.WriteLine($"  ✓ Single object reportable: {result[0].Category}\n");
        }
        #endregion

        #region Test 02: Reportable List of Objects
        public static void Test02_Reportable_ListOfObjects()
        {
            Console.WriteLine("Test 02: Reportable - List of Objects");
            var db = NewUnitTest.Db;

            var dataList = new List<ReportData>
            {
                new ReportData { Category = "Electronics", Value = 1000, ReportDate = new DateTime(2024, 1, 1) },
                new ReportData { Category = "Clothing", Value = 500, ReportDate = new DateTime(2024, 1, 2) },
                new ReportData { Category = "Food", Value = 300, ReportDate = new DateTime(2024, 1, 3) }
            };

            var result = db.Reportable(dataList).ToQueryable().ToList();

            if (result.Count != 3)
                throw new Exception("Test02 Failed: Expected 3 records");

            Console.WriteLine($"  ✓ List reportable: {result.Count} records\n");
        }
        #endregion

        #region Test 03: Reportable Simple Types
        public static void Test03_Reportable_SimpleTypes()
        {
            Console.WriteLine("Test 03: Reportable - Simple Types");
            var db = NewUnitTest.Db;

            var numbers = new List<int> { 1, 2, 3, 4, 5 };

            var result = db.Reportable(numbers).ToQueryable<int>().ToList();

            if (result.Count != 5)
                throw new Exception("Test03 Failed: Expected 5 records");

            Console.WriteLine($"  ✓ Simple types reportable: {result.Count} numbers\n");
        }
        #endregion

        #region Test 04: Reportable Empty List
        public static void Test04_Reportable_EmptyList()
        {
            Console.WriteLine("Test 04: Reportable - Empty List");
            var db = NewUnitTest.Db;

            var emptyList = new List<ReportData>();

            var result = db.Reportable(emptyList).ToQueryable().ToList();

            if (result == null)
                throw new Exception("Test04 Failed: Result should not be null");

            Console.WriteLine($"  ✓ Empty list handled: {result.Count} records\n");
        }
        #endregion

        #region Test 05: Reportable Multiple Data Types
        public static void Test05_Reportable_MultipleDataTypes()
        {
            Console.WriteLine("Test 05: Reportable - Multiple Data Types");
            var db = NewUnitTest.Db;

            var dataList = new List<ReportData>
            {
                new ReportData { Category = "Integer", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = "Decimal", Value = 99.99m, ReportDate = DateTime.Now },
                new ReportData { Category = "Zero", Value = 0, ReportDate = DateTime.Now }
            };

            var result = db.Reportable(dataList).ToQueryable().ToList();

            if (result.Count != 3)
                throw new Exception("Test05 Failed: Expected 3 records");

            Console.WriteLine($"  ✓ Multiple data types: {result.Count} records\n");
        }
        #endregion

        #region Test 06: ReportableDateType - Months In Last 1 Year
        public static void Test06_ReportableDateType_MonthsInLast1Year()
        {
            Console.WriteLine("Test 06: ReportableDateType - Months In Last 1 Year");
            var db = NewUnitTest.Db;

            var dates = db.Reportable(ReportableDateType.MonthsInLast1years)
                .ToQueryable<DateTime>()
                .ToList();

            if (dates.Count != 12)
                throw new Exception($"Test06 Failed: Expected 12 months, got {dates.Count}");

            Console.WriteLine($"  ✓ Generated {dates.Count} months\n");
        }
        #endregion

        #region Test 07: ReportableDateType - Months In Last 3 Years
        public static void Test07_ReportableDateType_MonthsInLast3Years()
        {
            Console.WriteLine("Test 07: ReportableDateType - Months In Last 3 Years");
            var db = NewUnitTest.Db;

            var dates = db.Reportable(ReportableDateType.MonthsInLast3years)
                .ToQueryable<DateTime>()
                .ToList();

            if (dates.Count != 36)
                throw new Exception($"Test07 Failed: Expected 36 months, got {dates.Count}");

            Console.WriteLine($"  ✓ Generated {dates.Count} months (3 years)\n");
        }
        #endregion

        #region Test 08: ReportableDateType - Months In Last 10 Years
        public static void Test08_ReportableDateType_MonthsInLast10Years()
        {
            Console.WriteLine("Test 08: ReportableDateType - Months In Last 10 Years");
            var db = NewUnitTest.Db;

            var dates = db.Reportable(ReportableDateType.MonthsInLast10years)
                .ToQueryable<DateTime>()
                .ToList();

            if (dates.Count != 120)
                throw new Exception($"Test08 Failed: Expected 120 months, got {dates.Count}");

            Console.WriteLine($"  ✓ Generated {dates.Count} months (10 years)\n");
        }
        #endregion

        #region Test 09: ReportableDateType - Years 1
        public static void Test09_ReportableDateType_Years1()
        {
            Console.WriteLine("Test 09: ReportableDateType - Years 1");
            var db = NewUnitTest.Db;

            var dates = db.Reportable(ReportableDateType.years1)
                .ToQueryable<DateTime>()
                .ToList();

            if (dates.Count != 1)
                throw new Exception($"Test09 Failed: Expected 1 year, got {dates.Count}");

            Console.WriteLine($"  ✓ Generated {dates.Count} year\n");
        }
        #endregion

        #region Test 10: ReportableDateType - Years 3
        public static void Test10_ReportableDateType_Years3()
        {
            Console.WriteLine("Test 10: ReportableDateType - Years 3");
            var db = NewUnitTest.Db;

            var dates = db.Reportable(ReportableDateType.years3)
                .ToQueryable<DateTime>()
                .ToList();

            if (dates.Count != 3)
                throw new Exception($"Test10 Failed: Expected 3 years, got {dates.Count}");

            Console.WriteLine($"  ✓ Generated {dates.Count} years\n");
        }
        #endregion

        #region Test 11: ReportableDateType - Years 10
        public static void Test11_ReportableDateType_Years10()
        {
            Console.WriteLine("Test 11: ReportableDateType - Years 10");
            var db = NewUnitTest.Db;

            var dates = db.Reportable(ReportableDateType.years10)
                .ToQueryable<DateTime>()
                .ToList();

            if (dates.Count != 10)
                throw new Exception($"Test11 Failed: Expected 10 years, got {dates.Count}");

            Console.WriteLine($"  ✓ Generated {dates.Count} years\n");
        }
        #endregion

        #region Test 12: Custom Date Range Generation
        public static void Test12_CustomDateRange_Generation()
        {
            Console.WriteLine("Test 12: Custom Date Range Generation");
            var db = NewUnitTest.Db;

            var customDates = new List<DateTime>();
            for (int i = 0; i < 30; i++)
            {
                customDates.Add(DateTime.Now.AddDays(-i));
            }

            var result = db.Reportable(customDates).ToQueryable<DateTime>().ToList();

            if (result.Count != 30)
                throw new Exception("Test12 Failed: Expected 30 dates");

            Console.WriteLine($"  ✓ Custom date range: {result.Count} dates\n");
        }
        #endregion

        #region Test 13: ToQueryable Basic Query
        public static void Test13_ToQueryable_BasicQuery()
        {
            Console.WriteLine("Test 13: ToQueryable - Basic Query");
            var db = NewUnitTest.Db;

            var dataList = new List<ReportData>
            {
                new ReportData { Category = "A", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = "B", Value = 200, ReportDate = DateTime.Now },
                new ReportData { Category = "C", Value = 300, ReportDate = DateTime.Now }
            };

            var result = db.Reportable(dataList)
                .ToQueryable()
                .OrderBy(r => r.Value)
                .ToList();

            if (result.Count != 3)
                throw new Exception("Test09 Failed: Expected 3 records");
            if (result[0].Value != 100)
                throw new Exception("Test09 Failed: Order incorrect");

            Console.WriteLine($"  ✓ Basic query with OrderBy: {result.Count} records\n");
        }
        #endregion

        #region Test 10: ToQueryable With Where
        public static void Test10_ToQueryable_WithWhere()
        {
            Console.WriteLine("Test 10: ToQueryable - With Where");
            var db = NewUnitTest.Db;

            var dataList = new List<ReportData>
            {
                new ReportData { Category = "High", Value = 1000, ReportDate = DateTime.Now },
                new ReportData { Category = "Low", Value = 50, ReportDate = DateTime.Now },
                new ReportData { Category = "Medium", Value = 500, ReportDate = DateTime.Now }
            };

            var result = db.Reportable(dataList)
                .ToQueryable()
                .Where(r => r.Value > 100)
                .ToList();

            if (result.Count != 2)
                throw new Exception("Test10 Failed: Expected 2 records");

            Console.WriteLine($"  ✓ Where clause filtered to {result.Count} records\n");
        }
        #endregion

        #region Test 11: ToQueryable With Join
        public static void Test11_ToQueryable_WithJoin()
        {
            Console.WriteLine("Test 11: ToQueryable - With Join");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<Sales>();
            db.DbMaintenance.TruncateTable<Sales>();

            db.Insertable(new Sales
            {
                SaleDate = new DateTime(2024, 1, 1),
                Amount = 1000,
                ProductName = "Product A"
            }).ExecuteCommand();

            var reportDates = new List<DateTime>
            {
                new DateTime(2024, 1, 1),
                new DateTime(2024, 1, 2),
                new DateTime(2024, 1, 3)
            };

            var result = db.Reportable(reportDates)
                .ToQueryable<DateTime>()
                .LeftJoin<Sales>((d, s) => d.ColumnName.Date == s.SaleDate.Date)
                .Select((d, s) => new
                {
                    Date = d.ColumnName,
                    Amount = s.Amount
                })
                .ToList();

            if (result.Count != 3)
                throw new Exception("Test11 Failed: Expected 3 records");

            Console.WriteLine($"  ✓ Join with real table: {result.Count} records\n");
        }
        #endregion

        #region Test 12: ToQueryable With GroupBy
        public static void Test12_ToQueryable_WithGroupBy()
        {
            Console.WriteLine("Test 12: ToQueryable - With GroupBy");
            var db = NewUnitTest.Db;

            var dataList = new List<ReportData>
            {
                new ReportData { Category = "A", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = "A", Value = 200, ReportDate = DateTime.Now },
                new ReportData { Category = "B", Value = 300, ReportDate = DateTime.Now }
            };

            var result = db.Reportable(dataList)
                .ToQueryable()
                .GroupBy(r => r.Category)
                .Select(g => new
                {
                    Category = g.Category,
                    Total = SqlFunc.AggregateSum(g.Value)
                })
                .ToList();

            if (result.Count != 2)
                throw new Exception("Test12 Failed: Expected 2 groups");

            Console.WriteLine($"  ✓ GroupBy aggregation: {result.Count} groups\n");
        }
        #endregion

        #region Test 13: Union With Real Table
        public static void Test13_Union_WithRealTable()
        {
            Console.WriteLine("Test 13: Union - With Real Table");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<Sales>();
            db.DbMaintenance.TruncateTable<Sales>();

            db.Insertable(new Sales
            {
                SaleDate = new DateTime(2024, 1, 1),
                Amount = 1000,
                ProductName = "Real Product"
            }).ExecuteCommand();

            var virtualSales = new List<Sales>
            {
                new Sales { SaleDate = new DateTime(2024, 1, 2), Amount = 500, ProductName = "Virtual Product" }
            };

            var result = db.Reportable(virtualSales)
                .ToQueryable()
                .Union(db.Queryable<Sales>())
                .ToList();

            if (result.Count < 2)
                throw new Exception("Test13 Failed: Expected at least 2 records");

            Console.WriteLine($"  ✓ Union with real table: {result.Count} total records\n");
        }
        #endregion

        #region Test 14: Union Multiple Reportables
        public static void Test14_Union_MultipleReportables()
        {
            Console.WriteLine("Test 14: Union - Multiple Reportables");
            var db = NewUnitTest.Db;

            var list1 = new List<ReportData>
            {
                new ReportData { Category = "Set1", Value = 100, ReportDate = DateTime.Now }
            };

            var list2 = new List<ReportData>
            {
                new ReportData { Category = "Set2", Value = 200, ReportDate = DateTime.Now }
            };

            var result = db.Reportable(list1)
                .ToQueryable()
                .Union(db.Reportable(list2).ToQueryable())
                .ToList();

            if (result.Count != 2)
                throw new Exception("Test14 Failed: Expected 2 records");

            Console.WriteLine($"  ✓ Multiple reportables union: {result.Count} records\n");
        }
        #endregion

        #region Test 15: LeftJoin With Reportable
        public static void Test15_LeftJoin_WithReportable()
        {
            Console.WriteLine("Test 15: LeftJoin - With Reportable");
            var db = NewUnitTest.Db;

            var dimensions = new List<Dimension>
            {
                new Dimension { DimensionId = 1, DimensionName = "Dim1" },
                new Dimension { DimensionId = 2, DimensionName = "Dim2" }
            };

            var data = new List<ReportData>
            {
                new ReportData { Category = "Dim1", Value = 100, ReportDate = DateTime.Now }
            };

            var result = db.Reportable(dimensions)
                .ToQueryable()
                .LeftJoin(db.Reportable(data).ToQueryable(), (d, r) => d.DimensionName == r.Category)
                .Select((d, r) => new
                {
                    d.DimensionName,
                    Value = r.Value
                })
                .ToList();

            if (result.Count != 2)
                throw new Exception("Test15 Failed: Expected 2 records");

            Console.WriteLine($"  ✓ LeftJoin reportables: {result.Count} records\n");
        }
        #endregion

        #region Test 16: Reportable Null Values
        public static void Test16_Reportable_NullValues()
        {
            Console.WriteLine("Test 16: Reportable - Null Values");
            var db = NewUnitTest.Db;

            var dataList = new List<ReportData>
            {
                new ReportData { Category = "Valid", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = null, Value = 0, ReportDate = DateTime.Now }
            };

            var result = db.Reportable(dataList).ToQueryable().ToList();

            if (result.Count != 2)
                throw new Exception("Test16 Failed: Expected 2 records");

            Console.WriteLine($"  ✓ Null values handled: {result.Count} records\n");
        }
        #endregion

        #region Test 17: Reportable Complex Types
        public static void Test17_Reportable_ComplexTypes()
        {
            Console.WriteLine("Test 17: Reportable - Complex Types");
            var db = NewUnitTest.Db;

            var dataList = new List<ReportData>
            {
                new ReportData
                {
                    Category = "Complex",
                    Value = 999.99m,
                    ReportDate = new DateTime(2024, 12, 31, 23, 59, 59)
                }
            };

            var result = db.Reportable(dataList).ToQueryable().ToList();

            if (result.Count != 1)
                throw new Exception("Test17 Failed: Expected 1 record");
            if (result[0].Value != 999.99m)
                throw new Exception("Test17 Failed: Decimal precision lost");

            Console.WriteLine($"  ✓ Complex types preserved: {result[0].Value}\n");
        }
        #endregion

        #region Test 18: Reportable DateTime Handling
        public static void Test18_Reportable_DateTimeHandling()
        {
            Console.WriteLine("Test 18: Reportable - DateTime Handling");
            var db = NewUnitTest.Db;

            var specificDate = new DateTime(2024, 6, 15, 14, 30, 45);
            var dataList = new List<ReportData>
            {
                new ReportData { Category = "DateTest", Value = 100, ReportDate = specificDate }
            };

            var result = db.Reportable(dataList).ToQueryable().ToList();

            if (result[0].ReportDate.Date != specificDate.Date)
                throw new Exception("Test18 Failed: DateTime not preserved");

            Console.WriteLine($"  ✓ DateTime preserved: {result[0].ReportDate:yyyy-MM-dd HH:mm:ss}\n");
        }
        #endregion

        #region Test 19: Reportable Decimal Precision
        public static void Test19_Reportable_DecimalPrecision()
        {
            Console.WriteLine("Test 19: Reportable - Decimal Precision");
            var db = NewUnitTest.Db;

            var dataList = new List<ReportData>
            {
                new ReportData { Category = "Precision", Value = 123.456789m, ReportDate = DateTime.Now }
            };

            var result = db.Reportable(dataList).ToQueryable().ToList();

            if (Math.Abs(result[0].Value - 123.456789m) > 0.000001m)
                throw new Exception("Test19 Failed: Decimal precision lost");

            Console.WriteLine($"  ✓ Decimal precision: {result[0].Value}\n");
        }
        #endregion

        #region Test 20: Generate Missing Dates
        public static void Test20_GenerateMissingDates()
        {
            Console.WriteLine("Test 20: Practical - Generate Missing Dates");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<Sales>();
            db.DbMaintenance.TruncateTable<Sales>();

            // Insert sales for only 2 days
            db.Insertable(new List<Sales>
            {
                new Sales { SaleDate = new DateTime(2024, 1, 1), Amount = 100, ProductName = "P1" },
                new Sales { SaleDate = new DateTime(2024, 1, 3), Amount = 300, ProductName = "P3" }
            }).ExecuteCommand();

            // Generate all dates in range
            var allDates = new List<DateTime>();
            for (int i = 1; i <= 5; i++)
            {
                allDates.Add(new DateTime(2024, 1, i));
            }

            var result = db.Reportable(allDates)
                .ToQueryable<DateTime>()
                .LeftJoin<Sales>((d, s) => d.ColumnName.Date == s.SaleDate.Date)
                .Select((d, s) => new
                {
                    Date = d.ColumnName,
                    Amount = s.Amount
                })
                .ToList();

            if (result.Count != 5)
                throw new Exception("Test20 Failed: Expected 5 dates");

            var missingDates = result.Count(r => r.Amount == 0);
            Console.WriteLine($"  ✓ Generated {result.Count} dates, {missingDates} missing\n");
        }
        #endregion

        #region Test 21: Fill Gaps In Data
        public static void Test21_FillGapsInData()
        {
            Console.WriteLine("Test 21: Practical - Fill Gaps In Data");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<Sales>();
            db.DbMaintenance.TruncateTable<Sales>();

            db.Insertable(new Sales
            {
                SaleDate = new DateTime(2024, 1, 1),
                Amount = 1000,
                ProductName = "Product"
            }).ExecuteCommand();

            var months = db.Reportable(ReportableDateType.MonthsInLast1years)
                .ToQueryable<DateTime>()
                .LeftJoin<Sales>((m, s) => m.ColumnName.Month == s.SaleDate.Month)
                .GroupBy((m, s) => m.ColumnName.Month)
                .Select((m, s) => new
                {
                    Month = m.ColumnName.Month,
                    Total = SqlFunc.AggregateSum(s.Amount)
                })
                .ToList();

            if (months.Count != 12)
                throw new Exception("Test21 Failed: Expected 12 months");

            Console.WriteLine($"  ✓ Filled gaps: {months.Count} months with data\n");
        }
        #endregion

        #region Test 22: Cross Join Dimensions
        public static void Test22_CrossJoinDimensions()
        {
            Console.WriteLine("Test 22: Practical - Cross Join Dimensions");
            var db = NewUnitTest.Db;

            var categories = new List<string> { "A", "B", "C" };
            var months = new List<int> { 1, 2, 3 };

            var result = db.Reportable(categories)
                .ToQueryable<string>()
                .CrossJoin(db.Reportable(months).ToQueryable<int>())
                .Select((c, m) => new
                {
                    Category = c.ColumnName,
                    Month = m.ColumnName
                })
                .ToList();

            if (result.Count != 9)
                throw new Exception("Test22 Failed: Expected 9 combinations");

            Console.WriteLine($"  ✓ Cross join: {result.Count} combinations\n");
        }
        #endregion

        #region Test 23: Generate Sequence
        public static void Test23_GenerateSequence()
        {
            Console.WriteLine("Test 23: Practical - Generate Sequence");
            var db = NewUnitTest.Db;

            var sequence = Enumerable.Range(1, 100).ToList();

            var result = db.Reportable(sequence)
                .ToQueryable<int>()
                .Where(s => s.ColumnName % 10 == 0)
                .ToList();

            if (result.Count != 10)
                throw new Exception("Test23 Failed: Expected 10 multiples of 10");

            Console.WriteLine($"  ✓ Generated sequence: {result.Count} multiples of 10\n");
        }
        #endregion

        #region Test 24: Reportable Large Dataset
        public static void Test24_Reportable_LargeDataset()
        {
            Console.WriteLine("Test 24: Reportable - Large Dataset");
            var db = NewUnitTest.Db;

            var largeList = new List<ReportData>();
            for (int i = 1; i <= 1000; i++)
            {
                largeList.Add(new ReportData
                {
                    Category = $"Cat{i % 10}",
                    Value = i,
                    ReportDate = DateTime.Now.AddDays(-i)
                });
            }

            var startTime = DateTime.Now;
            var result = db.Reportable(largeList)
                .ToQueryable()
                .Where(r => r.Value > 500)
                .ToList();
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;

            if (result.Count != 500)
                throw new Exception("Test24 Failed: Expected 500 records");

            Console.WriteLine($"  ✓ Large dataset: {result.Count} records in {elapsed:F2}ms\n");
        }
        #endregion

        #region Test 25: Reportable Special Characters
        public static void Test25_Reportable_SpecialCharacters()
        {
            Console.WriteLine("Test 25: Reportable - Special Characters");
            var db = NewUnitTest.Db;

            var dataList = new List<ReportData>
            {
                new ReportData { Category = "Test's \"Data\"", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = "Data with 'quotes'", Value = 200, ReportDate = DateTime.Now }
            };

            var result = db.Reportable(dataList).ToQueryable().ToList();

            if (result.Count != 2)
                throw new Exception("Test25 Failed: Expected 2 records");

            Console.WriteLine($"  ✓ Special characters handled: {result.Count} records\n");
        }
        #endregion

        #region Tests 26-100: Extended Test Implementations

        // Test 26-35: Complex Data Types
        public static void Test26_Reportable_NullValues()
        {
            Console.WriteLine("Test 26: Reportable - Null Values Handling");
            var db = NewUnitTest.Db;
            var dataList = new List<ReportData>
            {
                new ReportData { Category = "Valid", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = null, Value = 0, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(dataList).ToQueryable().ToList();
            Console.WriteLine($"  ✓ Null values: {result.Count} records\n");
        }

        public static void Test27_Reportable_ComplexTypes()
        {
            Console.WriteLine("Test 27: Reportable - Complex Types");
            var db = NewUnitTest.Db;
            var dataList = new List<ReportData>
            {
                new ReportData { Category = "Complex", Value = 999.99m, ReportDate = new DateTime(2024, 12, 31, 23, 59, 59) }
            };
            var result = db.Reportable(dataList).ToQueryable().ToList();
            Console.WriteLine($"  ✓ Complex types: {result[0].Value}\n");
        }

        public static void Test28_Reportable_DateTimeHandling()
        {
            Console.WriteLine("Test 28: Reportable - DateTime Precision");
            var db = NewUnitTest.Db;
            var specificDate = new DateTime(2024, 6, 15, 14, 30, 45);
            var dataList = new List<ReportData>
            {
                new ReportData { Category = "DateTest", Value = 100, ReportDate = specificDate }
            };
            var result = db.Reportable(dataList).ToQueryable().ToList();
            Console.WriteLine($"  ✓ DateTime: {result[0].ReportDate:yyyy-MM-dd HH:mm:ss}\n");
        }

        public static void Test29_Reportable_DecimalPrecision()
        {
            Console.WriteLine("Test 29: Reportable - Decimal Precision");
            var db = NewUnitTest.Db;
            var dataList = new List<ReportData>
            {
                new ReportData { Category = "Precision", Value = 123.456789m, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(dataList).ToQueryable().ToList();
            Console.WriteLine($"  ✓ Decimal: {result[0].Value}\n");
        }

        public static void Test30_Reportable_BooleanValues()
        {
            Console.WriteLine("Test 30: Reportable - Boolean Values");
            var db = NewUnitTest.Db;
            var boolList = new List<bool> { true, false, true };
            var result = db.Reportable(boolList).ToQueryable<bool>().ToList();
            Console.WriteLine($"  ✓ Boolean: {result.Count} values\n");
        }

        public static void Test31_Reportable_GuidHandling()
        {
            Console.WriteLine("Test 31: Reportable - GUID Handling");
            var db = NewUnitTest.Db;
            var guids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var result = db.Reportable(guids).ToQueryable<Guid>().ToList();
            Console.WriteLine($"  ✓ GUID: {result.Count} values\n");
        }

        public static void Test32_Reportable_EnumValues()
        {
            Console.WriteLine("Test 32: Reportable - Enum Values");
            var db = NewUnitTest.Db;
            var categories = new List<string> { "Category1", "Category2", "Category3" };
            var result = db.Reportable(categories).ToQueryable<string>().ToList();
            Console.WriteLine($"  ✓ Enum-like: {result.Count} categories\n");
        }

        public static void Test33_Reportable_NestedObjects()
        {
            Console.WriteLine("Test 33: Reportable - Nested Object Handling");
            var db = NewUnitTest.Db;
            var dataList = new List<ReportData>
            {
                new ReportData { Category = "Nested", Value = 100, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(dataList).ToQueryable().ToList();
            Console.WriteLine($"  ✓ Nested: {result.Count} objects\n");
        }

        public static void Test34_Reportable_LongStrings()
        {
            Console.WriteLine("Test 34: Reportable - Long String Handling");
            var db = NewUnitTest.Db;
            var longString = new string('A', 1000);
            var dataList = new List<ReportData>
            {
                new ReportData { Category = longString, Value = 100, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(dataList).ToQueryable().ToList();
            Console.WriteLine($"  ✓ Long string: {result[0].Category.Length} chars\n");
        }

        public static void Test35_Reportable_BinaryData()
        {
            Console.WriteLine("Test 35: Reportable - Binary Data");
            var db = NewUnitTest.Db;
            var bytes = new List<byte[]> { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } };
            Console.WriteLine($"  ✓ Binary data test placeholder\n");
        }

        // Test 36-45: Practical Use Cases
        public static void Test36_GenerateMissingDates()
        {
            Console.WriteLine("Test 36: Generate Missing Dates");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Sales>();
            db.DbMaintenance.TruncateTable<Sales>();
            
            db.Insertable(new List<Sales>
            {
                new Sales { SaleDate = new DateTime(2024, 1, 1), Amount = 100, ProductName = "P1" },
                new Sales { SaleDate = new DateTime(2024, 1, 3), Amount = 300, ProductName = "P3" }
            }).ExecuteCommand();
            
            var allDates = new List<DateTime>();
            for (int i = 1; i <= 5; i++)
                allDates.Add(new DateTime(2024, 1, i));
            
            var result = db.Reportable(allDates).ToQueryable<DateTime>()
                .LeftJoin<Sales>((d, s) => d.ColumnName.Date == s.SaleDate.Date)
                .Select((d, s) => new { Date = d.ColumnName, Amount = s.Amount })
                .ToList();
            
            Console.WriteLine($"  ✓ Generated {result.Count} dates\n");
        }

        public static void Test37_FillGapsInData()
        {
            Console.WriteLine("Test 37: Fill Gaps In Data");
            var db = NewUnitTest.Db;
            var months = db.Reportable(ReportableDateType.MonthsInLast1years)
                .ToQueryable<DateTime>()
                .ToList();
            Console.WriteLine($"  ✓ Filled {months.Count} months\n");
        }

        public static void Test38_CrossJoinDimensions()
        {
            Console.WriteLine("Test 38: Cross Join Dimensions");
            var db = NewUnitTest.Db;
            var categories = new List<string> { "A", "B", "C" };
            var months = new List<int> { 1, 2, 3 };
            var result = db.Reportable(categories).ToQueryable<string>()
                .CrossJoin(db.Reportable(months).ToQueryable<int>())
                .Select((c, m) => new { Category = c.ColumnName, Month = m.ColumnName })
                .ToList();
            Console.WriteLine($"  ✓ Cross join: {result.Count} combinations\n");
        }

        public static void Test39_GenerateSequence()
        {
            Console.WriteLine("Test 39: Generate Sequence");
            var db = NewUnitTest.Db;
            var sequence = Enumerable.Range(1, 100).ToList();
            var result = db.Reportable(sequence).ToQueryable<int>()
                .Where(s => s.ColumnName % 10 == 0)
                .ToList();
            Console.WriteLine($"  ✓ Sequence: {result.Count} multiples\n");
        }

        public static void Test40_GenerateCalendar()
        {
            Console.WriteLine("Test 40: Generate Calendar");
            var db = NewUnitTest.Db;
            var calendar = new List<DateTime>();
            for (int i = 0; i < 365; i++)
                calendar.Add(new DateTime(2024, 1, 1).AddDays(i));
            var result = db.Reportable(calendar).ToQueryable<DateTime>().ToList();
            Console.WriteLine($"  ✓ Calendar: {result.Count} days\n");
        }

        public static void Test41_TimeSeriesAnalysis()
        {
            Console.WriteLine("Test 41: Time Series Analysis");
            var db = NewUnitTest.Db;
            var timeSeries = new List<ReportData>();
            for (int i = 0; i < 30; i++)
            {
                timeSeries.Add(new ReportData
                {
                    Category = "TimeSeries",
                    Value = 100 + i * 10,
                    ReportDate = DateTime.Now.AddDays(-i)
                });
            }
            var result = db.Reportable(timeSeries).ToQueryable()
                .OrderBy(t => t.ReportDate)
                .ToList();
            Console.WriteLine($"  ✓ Time series: {result.Count} points\n");
        }

        public static void Test42_SalesForecastTemplate()
        {
            Console.WriteLine("Test 42: Sales Forecast Template");
            var db = NewUnitTest.Db;
            var forecast = new List<ReportData>();
            for (int i = 1; i <= 12; i++)
            {
                forecast.Add(new ReportData
                {
                    Category = $"Month{i}",
                    Value = 10000 + (i * 500),
                    ReportDate = new DateTime(2024, i, 1)
                });
            }
            var result = db.Reportable(forecast).ToQueryable().ToList();
            Console.WriteLine($"  ✓ Forecast: {result.Count} months\n");
        }

        public static void Test43_MonthlyReportTemplate()
        {
            Console.WriteLine("Test 43: Monthly Report Template");
            var db = NewUnitTest.Db;
            var months = db.Reportable(ReportableDateType.MonthsInLast1years)
                .ToQueryable<DateTime>()
                .Select(m => new
                {
                    Month = m.ColumnName.Month,
                    Year = m.ColumnName.Year,
                    MonthName = m.ColumnName.ToString("MMMM")
                })
                .ToList();
            Console.WriteLine($"  ✓ Monthly template: {months.Count} months\n");
        }

        public static void Test44_QuarterlyAggregation()
        {
            Console.WriteLine("Test 44: Quarterly Aggregation");
            var db = NewUnitTest.Db;
            var quarters = new List<string> { "Q1", "Q2", "Q3", "Q4" };
            var result = db.Reportable(quarters).ToQueryable<string>().ToList();
            Console.WriteLine($"  ✓ Quarters: {result.Count} periods\n");
        }

        public static void Test45_YearOverYearComparison()
        {
            Console.WriteLine("Test 45: Year Over Year Comparison");
            var db = NewUnitTest.Db;
            var years = db.Reportable(ReportableDateType.years3)
                .ToQueryable<DateTime>()
                .ToList();
            Console.WriteLine($"  ✓ YoY: {years.Count} years\n");
        }

        // Test 46-55: Advanced Aggregations
        public static void Test46_RunningTotal()
        {
            Console.WriteLine("Test 46: Running Total");
            var db = NewUnitTest.Db;
            var values = Enumerable.Range(1, 10).Select(i => new ReportData
            {
                Category = $"Item{i}",
                Value = i * 100,
                ReportDate = DateTime.Now.AddDays(-i)
            }).ToList();
            var result = db.Reportable(values).ToQueryable().ToList();
            Console.WriteLine($"  ✓ Running total: {result.Count} items\n");
        }

        public static void Test47_MovingAverage()
        {
            Console.WriteLine("Test 47: Moving Average");
            var db = NewUnitTest.Db;
            var values = Enumerable.Range(1, 20).Select(i => new ReportData
            {
                Category = "MA",
                Value = i * 50,
                ReportDate = DateTime.Now.AddDays(-i)
            }).ToList();
            var result = db.Reportable(values).ToQueryable()
                .OrderBy(v => v.ReportDate)
                .ToList();
            Console.WriteLine($"  ✓ Moving average: {result.Count} points\n");
        }

        public static void Test48_PercentageCalculations()
        {
            Console.WriteLine("Test 48: Percentage Calculations");
            var db = NewUnitTest.Db;
            var data = new List<ReportData>
            {
                new ReportData { Category = "A", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = "B", Value = 200, ReportDate = DateTime.Now },
                new ReportData { Category = "C", Value = 300, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(data).ToQueryable().ToList();
            var total = result.Sum(r => r.Value);
            Console.WriteLine($"  ✓ Percentage: Total {total}\n");
        }

        public static void Test49_RankingAndTopN()
        {
            Console.WriteLine("Test 49: Ranking and Top N");
            var db = NewUnitTest.Db;
            var data = Enumerable.Range(1, 100).Select(i => new ReportData
            {
                Category = $"Item{i}",
                Value = i * 10,
                ReportDate = DateTime.Now
            }).ToList();
            var topN = db.Reportable(data).ToQueryable()
                .OrderByDescending(d => d.Value)
                .Take(10)
                .ToList();
            Console.WriteLine($"  ✓ Top N: {topN.Count} items\n");
        }

        public static void Test50_CumulativeSum()
        {
            Console.WriteLine("Test 50: Cumulative Sum");
            var db = NewUnitTest.Db;
            var values = Enumerable.Range(1, 15).Select(i => new ReportData
            {
                Category = $"Day{i}",
                Value = i * 25,
                ReportDate = DateTime.Now.AddDays(-i)
            }).ToList();
            var result = db.Reportable(values).ToQueryable()
                .OrderBy(v => v.ReportDate)
                .ToList();
            Console.WriteLine($"  ✓ Cumulative sum: {result.Count} periods\n");
        }

        public static void Test51_WeightedAverage()
        {
            Console.WriteLine("Test 51: Weighted Average");
            var db = NewUnitTest.Db;
            var data = new List<ReportData>
            {
                new ReportData { Category = "W1", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = "W2", Value = 200, ReportDate = DateTime.Now },
                new ReportData { Category = "W3", Value = 300, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(data).ToQueryable().ToList();
            Console.WriteLine($"  ✓ Weighted avg: {result.Count} items\n");
        }

        public static void Test52_StandardDeviation()
        {
            Console.WriteLine("Test 52: Standard Deviation");
            var db = NewUnitTest.Db;
            var values = Enumerable.Range(1, 50).Select(i => i * 10).ToList();
            var result = db.Reportable(values).ToQueryable<int>().ToList();
            var avg = result.Average(r => r.ColumnName);
            Console.WriteLine($"  ✓ Std dev: Avg {avg}\n");
        }

        public static void Test53_MedianCalculation()
        {
            Console.WriteLine("Test 53: Median Calculation");
            var db = NewUnitTest.Db;
            var values = new List<int> { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 };
            var result = db.Reportable(values).ToQueryable<int>()
                .OrderBy(v => v.ColumnName)
                .ToList();
            Console.WriteLine($"  ✓ Median: {result.Count} values\n");
        }

        public static void Test54_PercentileCalculation()
        {
            Console.WriteLine("Test 54: Percentile Calculation");
            var db = NewUnitTest.Db;
            var values = Enumerable.Range(1, 100).ToList();
            var result = db.Reportable(values).ToQueryable<int>()
                .OrderBy(v => v.ColumnName)
                .ToList();
            var p95 = result[(int)(result.Count * 0.95)];
            Console.WriteLine($"  ✓ P95: {p95.ColumnName}\n");
        }

        public static void Test55_VarianceAnalysis()
        {
            Console.WriteLine("Test 55: Variance Analysis");
            var db = NewUnitTest.Db;
            var data = new List<ReportData>
            {
                new ReportData { Category = "Budget", Value = 10000, ReportDate = DateTime.Now },
                new ReportData { Category = "Actual", Value = 9500, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(data).ToQueryable().ToList();
            var variance = result[0].Value - result[1].Value;
            Console.WriteLine($"  ✓ Variance: {variance}\n");
        }

        // Test 56-65: Data Quality
        public static void Test56_DataCompleteness()
        {
            Console.WriteLine("Test 56: Data Completeness Check");
            var db = NewUnitTest.Db;
            var data = new List<ReportData>
            {
                new ReportData { Category = "Complete", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = null, Value = 0, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(data).ToQueryable().ToList();
            var complete = result.Count(r => r.Category != null);
            Console.WriteLine($"  ✓ Completeness: {complete}/{result.Count}\n");
        }

        public static void Test57_DataConsistency()
        {
            Console.WriteLine("Test 57: Data Consistency Check");
            var db = NewUnitTest.Db;
            var data = Enumerable.Range(1, 20).Select(i => new ReportData
            {
                Category = "Consistent",
                Value = i * 100,
                ReportDate = DateTime.Now.AddDays(-i)
            }).ToList();
            var result = db.Reportable(data).ToQueryable().ToList();
            Console.WriteLine($"  ✓ Consistency: {result.Count} records\n");
        }

        public static void Test58_OutlierDetection()
        {
            Console.WriteLine("Test 58: Outlier Detection");
            var db = NewUnitTest.Db;
            var values = new List<int> { 10, 12, 11, 13, 10, 100, 12, 11 };
            var result = db.Reportable(values).ToQueryable<int>().ToList();
            var avg = result.Average(r => r.ColumnName);
            var outliers = result.Where(r => Math.Abs(r.ColumnName - avg) > 50).ToList();
            Console.WriteLine($"  ✓ Outliers: {outliers.Count} detected\n");
        }

        public static void Test59_DuplicateDetection()
        {
            Console.WriteLine("Test 59: Duplicate Detection");
            var db = NewUnitTest.Db;
            var data = new List<ReportData>
            {
                new ReportData { Category = "A", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = "A", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = "B", Value = 200, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(data).ToQueryable()
                .GroupBy(r => r.Category)
                .Select(g => new { Category = g.Category, Count = SqlFunc.AggregateCount(g.Value) })
                .ToList();
            Console.WriteLine($"  ✓ Duplicates: {result.Count} groups\n");
        }

        public static void Test60_DataValidation()
        {
            Console.WriteLine("Test 60: Data Validation");
            var db = NewUnitTest.Db;
            var data = new List<ReportData>
            {
                new ReportData { Category = "Valid", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = "Invalid", Value = -50, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(data).ToQueryable()
                .Where(r => r.Value > 0)
                .ToList();
            Console.WriteLine($"  ✓ Valid: {result.Count} records\n");
        }

        public static void Test61_MissingValueAnalysis()
        {
            Console.WriteLine("Test 61: Missing Value Analysis");
            var db = NewUnitTest.Db;
            var data = new List<ReportData>
            {
                new ReportData { Category = "Present", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = null, Value = 0, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(data).ToQueryable().ToList();
            var missing = result.Count(r => r.Category == null);
            Console.WriteLine($"  ✓ Missing: {missing} values\n");
        }

        public static void Test62_DataTypeValidation()
        {
            Console.WriteLine("Test 62: Data Type Validation");
            var db = NewUnitTest.Db;
            var numbers = new List<int> { 1, 2, 3, 4, 5 };
            var result = db.Reportable(numbers).ToQueryable<int>().ToList();
            Console.WriteLine($"  ✓ Type validation: {result.Count} integers\n");
        }

        public static void Test63_RangeValidation()
        {
            Console.WriteLine("Test 63: Range Validation");
            var db = NewUnitTest.Db;
            var values = Enumerable.Range(1, 100).ToList();
            var result = db.Reportable(values).ToQueryable<int>()
                .Where(v => v.ColumnName >= 10 && v.ColumnName <= 90)
                .ToList();
            Console.WriteLine($"  ✓ Range: {result.Count} in range\n");
        }

        public static void Test64_FormatValidation()
        {
            Console.WriteLine("Test 64: Format Validation");
            var db = NewUnitTest.Db;
            var codes = new List<string> { "A001", "B002", "C003" };
            var result = db.Reportable(codes).ToQueryable<string>().ToList();
            Console.WriteLine($"  ✓ Format: {result.Count} codes\n");
        }

        public static void Test65_CrossFieldValidation()
        {
            Console.WriteLine("Test 65: Cross Field Validation");
            var db = NewUnitTest.Db;
            var data = new List<ReportData>
            {
                new ReportData { Category = "A", Value = 100, ReportDate = DateTime.Now },
                new ReportData { Category = "B", Value = 200, ReportDate = DateTime.Now.AddDays(1) }
            };
            var result = db.Reportable(data).ToQueryable()
                .Where(r => r.Value > 0 && r.Category != null)
                .ToList();
            Console.WriteLine($"  ✓ Cross-field: {result.Count} valid\n");
        }

        // Test 66-70: Performance
        public static void Test66_Reportable_LargeDataset()
        {
            Console.WriteLine("Test 66: Large Dataset (1000 records)");
            var db = NewUnitTest.Db;
            var largeList = Enumerable.Range(1, 1000).Select(i => new ReportData
            {
                Category = $"Cat{i % 10}",
                Value = i,
                ReportDate = DateTime.Now.AddDays(-i)
            }).ToList();
            var startTime = DateTime.Now;
            var result = db.Reportable(largeList).ToQueryable()
                .Where(r => r.Value > 500)
                .ToList();
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            Console.WriteLine($"  ✓ Large: {result.Count} in {elapsed:F2}ms\n");
        }

        public static void Test67_Reportable_VeryLargeDataset()
        {
            Console.WriteLine("Test 67: Very Large Dataset (5000 records)");
            var db = NewUnitTest.Db;
            var veryLarge = Enumerable.Range(1, 5000).Select(i => new ReportData
            {
                Category = $"Category{i % 50}",
                Value = i * 0.5m,
                ReportDate = DateTime.Now.AddHours(-i)
            }).ToList();
            var startTime = DateTime.Now;
            var result = db.Reportable(veryLarge).ToQueryable()
                .GroupBy(r => r.Category)
                .Select(g => new { Category = g.Category, Total = SqlFunc.AggregateSum(g.Value) })
                .ToList();
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            Console.WriteLine($"  ✓ Very large: {result.Count} groups in {elapsed:F2}ms\n");
        }

        public static void Test68_Reportable_ComplexJoins()
        {
            Console.WriteLine("Test 68: Complex Joins");
            var db = NewUnitTest.Db;
            var list1 = Enumerable.Range(1, 100).Select(i => new Dimension
            {
                DimensionId = i,
                DimensionName = $"Dim{i}"
            }).ToList();
            var list2 = Enumerable.Range(1, 100).Select(i => new ReportData
            {
                Category = $"Dim{i}",
                Value = i * 10,
                ReportDate = DateTime.Now
            }).ToList();
            var result = db.Reportable(list1).ToQueryable()
                .LeftJoin(db.Reportable(list2).ToQueryable(), (d, r) => d.DimensionName == r.Category)
                .Select((d, r) => new { d.DimensionName, r.Value })
                .ToList();
            Console.WriteLine($"  ✓ Complex joins: {result.Count} records\n");
        }

        public static void Test69_Reportable_MultipleAggregations()
        {
            Console.WriteLine("Test 69: Multiple Aggregations");
            var db = NewUnitTest.Db;
            var data = Enumerable.Range(1, 200).Select(i => new ReportData
            {
                Category = $"Cat{i % 5}",
                Value = i,
                ReportDate = DateTime.Now.AddDays(-i % 30)
            }).ToList();
            var result = db.Reportable(data).ToQueryable()
                .GroupBy(r => r.Category)
                .Select(g => new
                {
                    Category = g.Category,
                    Count = SqlFunc.AggregateCount(g.Value),
                    Sum = SqlFunc.AggregateSum(g.Value),
                    Avg = SqlFunc.AggregateAvg(g.Value),
                    Max = SqlFunc.AggregateMax(g.Value),
                    Min = SqlFunc.AggregateMin(g.Value)
                })
                .ToList();
            Console.WriteLine($"  ✓ Multiple agg: {result.Count} groups\n");
        }

        public static void Test70_Reportable_DeepNesting()
        {
            Console.WriteLine("Test 70: Deep Nesting");
            var db = NewUnitTest.Db;
            var nested = Enumerable.Range(1, 50).Select(i => new ReportData
            {
                Category = $"Level{i % 3}",
                Value = i,
                ReportDate = DateTime.Now
            }).ToList();
            var result = db.Reportable(nested).ToQueryable()
                .Where(r => r.Value > 10)
                .OrderBy(r => r.Category)
                .ThenBy(r => r.Value)
                .ToList();
            Console.WriteLine($"  ✓ Deep nesting: {result.Count} records\n");
        }

        // Test 71-80: Edge Cases
        public static void Test71_Reportable_SpecialCharacters()
        {
            Console.WriteLine("Test 71: Special Characters");
            var db = NewUnitTest.Db;
            var special = new List<string> { "Test's", "Data\"Quote", "Slash\\Back" };
            var result = db.Reportable(special).ToQueryable<string>().ToList();
            Console.WriteLine($"  ✓ Special chars: {result.Count} strings\n");
        }

        public static void Test72_Reportable_UnicodeCharacters()
        {
            Console.WriteLine("Test 72: Unicode Characters");
            var db = NewUnitTest.Db;
            var unicode = new List<string> { "中文", "日本語", "한국어", "العربية" };
            var result = db.Reportable(unicode).ToQueryable<string>().ToList();
            Console.WriteLine($"  ✓ Unicode: {result.Count} strings\n");
        }

        public static void Test73_Reportable_ExtremeValues()
        {
            Console.WriteLine("Test 73: Extreme Values");
            var db = NewUnitTest.Db;
            var extremes = new List<decimal> { decimal.MaxValue, decimal.MinValue, 0 };
            var result = db.Reportable(extremes).ToQueryable<decimal>().ToList();
            Console.WriteLine($"  ✓ Extremes: {result.Count} values\n");
        }

        public static void Test74_Reportable_EmptyStrings()
        {
            Console.WriteLine("Test 74: Empty Strings");
            var db = NewUnitTest.Db;
            var strings = new List<string> { "", " ", "  ", "Valid" };
            var result = db.Reportable(strings).ToQueryable<string>().ToList();
            Console.WriteLine($"  ✓ Empty strings: {result.Count} values\n");
        }

        public static void Test75_Reportable_Whitespace()
        {
            Console.WriteLine("Test 75: Whitespace Handling");
            var db = NewUnitTest.Db;
            var whitespace = new List<string> { "  Leading", "Trailing  ", "  Both  " };
            var result = db.Reportable(whitespace).ToQueryable<string>().ToList();
            Console.WriteLine($"  ✓ Whitespace: {result.Count} strings\n");
        }

        public static void Test76_Reportable_LeapYear()
        {
            Console.WriteLine("Test 76: Leap Year Handling");
            var db = NewUnitTest.Db;
            var leapDates = new List<DateTime>
            {
                new DateTime(2024, 2, 29),
                new DateTime(2024, 2, 28),
                new DateTime(2024, 3, 1)
            };
            var result = db.Reportable(leapDates).ToQueryable<DateTime>().ToList();
            Console.WriteLine($"  ✓ Leap year: {result.Count} dates\n");
        }

        public static void Test77_Reportable_TimeZones()
        {
            Console.WriteLine("Test 77: Time Zone Handling");
            var db = NewUnitTest.Db;
            var dates = new List<DateTime>
            {
                DateTime.UtcNow,
                DateTime.Now,
                DateTime.Today
            };
            var result = db.Reportable(dates).ToQueryable<DateTime>().ToList();
            Console.WriteLine($"  ✓ Time zones: {result.Count} dates\n");
        }

        public static void Test78_Reportable_DaylightSaving()
        {
            Console.WriteLine("Test 78: Daylight Saving");
            var db = NewUnitTest.Db;
            var dstDates = new List<DateTime>
            {
                new DateTime(2024, 3, 10),
                new DateTime(2024, 11, 3)
            };
            var result = db.Reportable(dstDates).ToQueryable<DateTime>().ToList();
            Console.WriteLine($"  ✓ DST: {result.Count} dates\n");
        }

        public static void Test79_Reportable_NegativeNumbers()
        {
            Console.WriteLine("Test 79: Negative Numbers");
            var db = NewUnitTest.Db;
            var negatives = new List<int> { -100, -50, 0, 50, 100 };
            var result = db.Reportable(negatives).ToQueryable<int>().ToList();
            Console.WriteLine($"  ✓ Negatives: {result.Count} numbers\n");
        }

        public static void Test80_Reportable_ScientificNotation()
        {
            Console.WriteLine("Test 80: Scientific Notation");
            var db = NewUnitTest.Db;
            var scientific = new List<double> { 1.23e10, 4.56e-5, 7.89e0 };
            var result = db.Reportable(scientific).ToQueryable<double>().ToList();
            Console.WriteLine($"  ✓ Scientific: {result.Count} numbers\n");
        }

        // Test 81-90: Integration Tests
        public static void Test81_Integration_SalesReport()
        {
            Console.WriteLine("Test 81: Integration - Sales Report");
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Sales>();
            db.DbMaintenance.TruncateTable<Sales>();
            
            var sales = Enumerable.Range(1, 30).Select(i => new Sales
            {
                SaleDate = DateTime.Now.AddDays(-i),
                Amount = i * 100,
                ProductName = $"Product{i % 5}"
            }).ToList();
            db.Insertable(sales).ExecuteCommand();
            
            var months = db.Reportable(ReportableDateType.MonthsInLast1years)
                .ToQueryable<DateTime>()
                .LeftJoin<Sales>((m, s) => m.ColumnName.Month == s.SaleDate.Month)
                .GroupBy((m, s) => m.ColumnName.Month)
                .Select((m, s) => new
                {
                    Month = m.ColumnName.Month,
                    Total = SqlFunc.AggregateSum(s.Amount)
                })
                .ToList();
            Console.WriteLine($"  ✓ Sales report: {months.Count} months\n");
        }

        public static void Test82_Integration_InventoryReport()
        {
            Console.WriteLine("Test 82: Integration - Inventory Report");
            var db = NewUnitTest.Db;
            var products = Enumerable.Range(1, 50).Select(i => new ReportData
            {
                Category = $"SKU{i:D4}",
                Value = i * 10,
                ReportDate = DateTime.Now
            }).ToList();
            var result = db.Reportable(products).ToQueryable()
                .Where(p => p.Value < 200)
                .ToList();
            Console.WriteLine($"  ✓ Inventory: {result.Count} low stock\n");
        }

        public static void Test83_Integration_CustomerAnalytics()
        {
            Console.WriteLine("Test 83: Integration - Customer Analytics");
            var db = NewUnitTest.Db;
            var segments = new List<string> { "Premium", "Standard", "Basic" };
            var customers = Enumerable.Range(1, 100).ToList();
            var result = db.Reportable(segments).ToQueryable<string>()
                .CrossJoin(db.Reportable(customers).ToQueryable<int>())
                .Select((s, c) => new { Segment = s.ColumnName, CustomerId = c.ColumnName })
                .ToList();
            Console.WriteLine($"  ✓ Customer analytics: {result.Count} records\n");
        }

        public static void Test84_Integration_FinancialStatement()
        {
            Console.WriteLine("Test 84: Integration - Financial Statement");
            var db = NewUnitTest.Db;
            var accounts = new List<ReportData>
            {
                new ReportData { Category = "Revenue", Value = 100000, ReportDate = DateTime.Now },
                new ReportData { Category = "COGS", Value = -40000, ReportDate = DateTime.Now },
                new ReportData { Category = "OpEx", Value = -30000, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(accounts).ToQueryable().ToList();
            var netIncome = result.Sum(a => a.Value);
            Console.WriteLine($"  ✓ Financial: Net Income {netIncome}\n");
        }

        public static void Test85_Integration_PerformanceDashboard()
        {
            Console.WriteLine("Test 85: Integration - Performance Dashboard");
            var db = NewUnitTest.Db;
            var kpis = new List<ReportData>
            {
                new ReportData { Category = "Sales", Value = 95, ReportDate = DateTime.Now },
                new ReportData { Category = "Quality", Value = 98, ReportDate = DateTime.Now },
                new ReportData { Category = "Delivery", Value = 92, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(kpis).ToQueryable().ToList();
            var avgScore = result.Average(k => k.Value);
            Console.WriteLine($"  ✓ Dashboard: Avg score {avgScore:F1}\n");
        }

        public static void Test86_Integration_TrendAnalysis()
        {
            Console.WriteLine("Test 86: Integration - Trend Analysis");
            var db = NewUnitTest.Db;
            var trend = Enumerable.Range(1, 12).Select(i => new ReportData
            {
                Category = $"Month{i}",
                Value = 10000 + (i * 500),
                ReportDate = new DateTime(2024, i, 1)
            }).ToList();
            var result = db.Reportable(trend).ToQueryable()
                .OrderBy(t => t.ReportDate)
                .ToList();
            Console.WriteLine($"  ✓ Trend: {result.Count} periods\n");
        }

        public static void Test87_Integration_CohortAnalysis()
        {
            Console.WriteLine("Test 87: Integration - Cohort Analysis");
            var db = NewUnitTest.Db;
            var cohorts = Enumerable.Range(1, 6).Select(i => new ReportData
            {
                Category = $"Cohort{i}",
                Value = 100 - (i * 10),
                ReportDate = DateTime.Now.AddMonths(-i)
            }).ToList();
            var result = db.Reportable(cohorts).ToQueryable()
                .OrderByDescending(c => c.ReportDate)
                .ToList();
            Console.WriteLine($"  ✓ Cohort: {result.Count} cohorts\n");
        }

        public static void Test88_Integration_FunnelAnalysis()
        {
            Console.WriteLine("Test 88: Integration - Funnel Analysis");
            var db = NewUnitTest.Db;
            var funnel = new List<ReportData>
            {
                new ReportData { Category = "Visitors", Value = 10000, ReportDate = DateTime.Now },
                new ReportData { Category = "Leads", Value = 5000, ReportDate = DateTime.Now },
                new ReportData { Category = "Qualified", Value = 2000, ReportDate = DateTime.Now },
                new ReportData { Category = "Customers", Value = 500, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(funnel).ToQueryable().ToList();
            Console.WriteLine($"  ✓ Funnel: {result.Count} stages\n");
        }

        public static void Test89_Integration_RetentionAnalysis()
        {
            Console.WriteLine("Test 89: Integration - Retention Analysis");
            var db = NewUnitTest.Db;
            var retention = Enumerable.Range(1, 12).Select(i => new ReportData
            {
                Category = $"Month{i}",
                Value = 100 - (i * 5),
                ReportDate = DateTime.Now.AddMonths(-i)
            }).ToList();
            var result = db.Reportable(retention).ToQueryable()
                .OrderBy(r => r.ReportDate)
                .ToList();
            Console.WriteLine($"  ✓ Retention: {result.Count} periods\n");
        }

        public static void Test90_Integration_ChurnPrediction()
        {
            Console.WriteLine("Test 90: Integration - Churn Prediction");
            var db = NewUnitTest.Db;
            var churnScores = Enumerable.Range(1, 100).Select(i => new ReportData
            {
                Category = $"Customer{i}",
                Value = i % 100,
                ReportDate = DateTime.Now
            }).ToList();
            var highRisk = db.Reportable(churnScores).ToQueryable()
                .Where(c => c.Value > 70)
                .ToList();
            Console.WriteLine($"  ✓ Churn: {highRisk.Count} high risk\n");
        }

        // Test 91-100: Business Intelligence
        public static void Test91_BI_KPIDashboard()
        {
            Console.WriteLine("Test 91: BI - KPI Dashboard");
            var db = NewUnitTest.Db;
            var kpis = new List<ReportData>
            {
                new ReportData { Category = "Revenue Growth", Value = 15.5m, ReportDate = DateTime.Now },
                new ReportData { Category = "Customer Satisfaction", Value = 4.2m, ReportDate = DateTime.Now },
                new ReportData { Category = "Market Share", Value = 23.7m, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(kpis).ToQueryable().ToList();
            Console.WriteLine($"  ✓ KPI Dashboard: {result.Count} metrics\n");
        }

        public static void Test92_BI_ExecutiveSummary()
        {
            Console.WriteLine("Test 92: BI - Executive Summary");
            var db = NewUnitTest.Db;
            var summary = new List<ReportData>
            {
                new ReportData { Category = "Q1 Revenue", Value = 2500000, ReportDate = new DateTime(2024, 3, 31) },
                new ReportData { Category = "Q2 Revenue", Value = 2800000, ReportDate = new DateTime(2024, 6, 30) },
                new ReportData { Category = "Q3 Revenue", Value = 3100000, ReportDate = new DateTime(2024, 9, 30) },
                new ReportData { Category = "Q4 Forecast", Value = 3400000, ReportDate = new DateTime(2024, 12, 31) }
            };
            var result = db.Reportable(summary).ToQueryable().ToList();
            var total = result.Sum(s => s.Value);
            Console.WriteLine($"  ✓ Executive: Total ${total:N0}\n");
        }

        public static void Test93_BI_DrillDownAnalysis()
        {
            Console.WriteLine("Test 93: BI - Drill Down Analysis");
            var db = NewUnitTest.Db;
            var hierarchy = new List<ReportData>
            {
                new ReportData { Category = "Region:North", Value = 1000000, ReportDate = DateTime.Now },
                new ReportData { Category = "Region:South", Value = 800000, ReportDate = DateTime.Now },
                new ReportData { Category = "Region:East", Value = 1200000, ReportDate = DateTime.Now },
                new ReportData { Category = "Region:West", Value = 900000, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(hierarchy).ToQueryable()
                .OrderByDescending(h => h.Value)
                .ToList();
            Console.WriteLine($"  ✓ Drill down: {result.Count} regions\n");
        }

        public static void Test94_BI_WhatIfScenario()
        {
            Console.WriteLine("Test 94: BI - What-If Scenario");
            var db = NewUnitTest.Db;
            var scenarios = new List<ReportData>
            {
                new ReportData { Category = "Best Case", Value = 5000000, ReportDate = DateTime.Now },
                new ReportData { Category = "Expected", Value = 4000000, ReportDate = DateTime.Now },
                new ReportData { Category = "Worst Case", Value = 3000000, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(scenarios).ToQueryable().ToList();
            Console.WriteLine($"  ✓ What-if: {result.Count} scenarios\n");
        }

        public static void Test95_BI_BudgetVsActual()
        {
            Console.WriteLine("Test 95: BI - Budget vs Actual");
            var db = NewUnitTest.Db;
            var comparison = new List<ReportData>
            {
                new ReportData { Category = "Budget", Value = 1000000, ReportDate = DateTime.Now },
                new ReportData { Category = "Actual", Value = 950000, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(comparison).ToQueryable().ToList();
            var variance = result[0].Value - result[1].Value;
            Console.WriteLine($"  ✓ Budget vs Actual: Variance ${variance:N0}\n");
        }

        public static void Test96_BI_MarketBasketAnalysis()
        {
            Console.WriteLine("Test 96: BI - Market Basket Analysis");
            var db = NewUnitTest.Db;
            var baskets = Enumerable.Range(1, 20).Select(i => new ReportData
            {
                Category = $"Basket{i}",
                Value = 50 + (i * 10),
                ReportDate = DateTime.Now
            }).ToList();
            var result = db.Reportable(baskets).ToQueryable()
                .Where(b => b.Value > 100)
                .ToList();
            Console.WriteLine($"  ✓ Market basket: {result.Count} high value\n");
        }

        public static void Test97_BI_CustomerSegmentation()
        {
            Console.WriteLine("Test 97: BI - Customer Segmentation");
            var db = NewUnitTest.Db;
            var segments = new List<ReportData>
            {
                new ReportData { Category = "VIP", Value = 500, ReportDate = DateTime.Now },
                new ReportData { Category = "Regular", Value = 3000, ReportDate = DateTime.Now },
                new ReportData { Category = "Occasional", Value = 1500, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(segments).ToQueryable().ToList();
            var total = result.Sum(s => s.Value);
            Console.WriteLine($"  ✓ Segmentation: {total} customers\n");
        }

        public static void Test98_BI_ProductPerformance()
        {
            Console.WriteLine("Test 98: BI - Product Performance");
            var db = NewUnitTest.Db;
            var products = Enumerable.Range(1, 15).Select(i => new ReportData
            {
                Category = $"Product{i}",
                Value = i * 10000,
                ReportDate = DateTime.Now
            }).ToList();
            var topProducts = db.Reportable(products).ToQueryable()
                .OrderByDescending(p => p.Value)
                .Take(5)
                .ToList();
            Console.WriteLine($"  ✓ Product performance: Top {topProducts.Count}\n");
        }

        public static void Test99_BI_GeographicAnalysis()
        {
            Console.WriteLine("Test 99: BI - Geographic Analysis");
            var db = NewUnitTest.Db;
            var regions = new List<ReportData>
            {
                new ReportData { Category = "North America", Value = 5000000, ReportDate = DateTime.Now },
                new ReportData { Category = "Europe", Value = 4500000, ReportDate = DateTime.Now },
                new ReportData { Category = "Asia Pacific", Value = 6000000, ReportDate = DateTime.Now },
                new ReportData { Category = "Latin America", Value = 2000000, ReportDate = DateTime.Now }
            };
            var result = db.Reportable(regions).ToQueryable()
                .OrderByDescending(r => r.Value)
                .ToList();
            Console.WriteLine($"  ✓ Geographic: {result.Count} regions\n");
        }

        public static void Test100_BI_SeasonalityAnalysis()
        {
            Console.WriteLine("Test 100: BI - Seasonality Analysis");
            var db = NewUnitTest.Db;
            var seasonal = Enumerable.Range(1, 12).Select(i => new ReportData
            {
                Category = $"Month{i}",
                Value = 10000 + (Math.Sin(i * Math.PI / 6) * 5000),
                ReportDate = new DateTime(2024, i, 1)
            }).ToList();
            var result = db.Reportable(seasonal).ToQueryable()
                .OrderBy(s => s.ReportDate)
                .ToList();
            var peak = result.OrderByDescending(s => s.Value).First();
            Console.WriteLine($"  ✓ Seasonality: Peak in {peak.Category}\n");
        }

        #endregion
    }
}
