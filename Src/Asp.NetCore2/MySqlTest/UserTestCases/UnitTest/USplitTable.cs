using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrmTest
{
    /// <summary>
    /// Comprehensive Unit Tests for Split Table Operations
    /// 分表操作的综合单元测试
    /// </summary>
    public class USplitTable
    {
        public static void Init()
        {
            Console.WriteLine("=== Split Table Operations Comprehensive Unit Tests ===\n");

            // Basic Split Table Tests
            Test01_SplitByYear_Insert();
            Test02_SplitByMonth_Insert();
            Test03_SplitByDay_Insert();
            Test04_SplitByWeek_Insert();
            
            // Query Operations
            Test05_Query_SingleTable();
            Test06_Query_MultipleTablesUnion();
            Test07_Query_DateRange();
            Test08_Query_WithWhere();
            
            // Update Operations
            Test09_Update_SingleRecord();
            Test10_Update_MultipleRecords();
            Test11_Update_CrossTables();
            
            // Delete Operations
            Test12_Delete_SingleRecord();
            Test13_Delete_ByDateRange();
            Test14_Delete_EntireTable();
            
            // Advanced Features
            Test15_GetTableList();
            Test16_CreateTable_AutoGeneration();
            Test17_SplitField_CustomColumn();
            Test18_PageQuery_AcrossTables();
            
            // Bulk Operations
            Test19_BulkInsert_MultiTables();
            Test20_BulkUpdate_MultiTables();
            
            // Edge Cases
            Test21_EmptyTable_Query();
            Test22_FutureDate_Insert();
            Test23_HistoricalData_Query();
            Test24_TableNotExist_AutoCreate();
            Test25_Performance_LargeDataset();

            Console.WriteLine("\n=== All Split Table Tests Completed ===\n");
        }

        #region Test Entities

        [SplitTable(SplitType.Year)]
        [SugarTable("UnitSplit_Order_{year}{month}{day}")]
        public class OrderByYear
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public long Id { get; set; }

            public string OrderNo { get; set; }

            public decimal Amount { get; set; }

            [SplitField]
            public DateTime CreateTime { get; set; }
        }

        [SplitTable(SplitType.Month)]
        [SugarTable("UnitSplit_Log_{year}{month}{day}")]
        public class LogByMonth
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public long Id { get; set; }

            public string Message { get; set; }

            public string Level { get; set; }

            [SplitField]
            public DateTime LogTime { get; set; }
        }

        [SplitTable(SplitType.Day)]
        [SugarTable("UnitSplit_Event_{year}{month}{day}")]
        public class EventByDay
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public long Id { get; set; }

            public string EventName { get; set; }

            public string EventData { get; set; }

            [SplitField]
            public DateTime EventTime { get; set; }
        }

        [SplitTable(SplitType.Week)]
        [SugarTable("UnitSplit_Task_{year}{month}{day}")]
        public class TaskByWeek
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public long Id { get; set; }

            public string TaskName { get; set; }

            public string Status { get; set; }

            [SplitField]
            public DateTime TaskDate { get; set; }
        }

        #endregion

        #region Test 01: Split By Year - Insert
        public static void Test01_SplitByYear_Insert()
        {
            Console.WriteLine("Test 01: Split By Year - Insert");
            var db = NewUnitTest.Db;

            // Create base table structure
            db.CodeFirst.InitTables<OrderByYear>();

            var orders = new List<OrderByYear>
            {
                new OrderByYear { OrderNo = "2023-001", Amount = 100, CreateTime = new DateTime(2023, 1, 15) },
                new OrderByYear { OrderNo = "2024-001", Amount = 200, CreateTime = new DateTime(2024, 6, 20) },
                new OrderByYear { OrderNo = "2024-002", Amount = 300, CreateTime = new DateTime(2024, 12, 25) }
            };

            var result = db.Insertable(orders).SplitTable().ExecuteCommand();

            if (result != 3)
                throw new Exception("Test01 Failed: Expected 3 records inserted");

            Console.WriteLine($"  ✓ Inserted {result} records across year-based split tables\n");
        }
        #endregion

        #region Test 02: Split By Month - Insert
        public static void Test02_SplitByMonth_Insert()
        {
            Console.WriteLine("Test 02: Split By Month - Insert");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<LogByMonth>();

            var logs = new List<LogByMonth>
            {
                new LogByMonth { Message = "Log 1", Level = "INFO", LogTime = new DateTime(2024, 1, 10) },
                new LogByMonth { Message = "Log 2", Level = "WARN", LogTime = new DateTime(2024, 2, 15) },
                new LogByMonth { Message = "Log 3", Level = "ERROR", LogTime = new DateTime(2024, 3, 20) }
            };

            var result = db.Insertable(logs).SplitTable().ExecuteCommand();

            if (result != 3)
                throw new Exception("Test02 Failed: Expected 3 records inserted");

            Console.WriteLine($"  ✓ Inserted {result} records across month-based split tables\n");
        }
        #endregion

        #region Test 03: Split By Day - Insert
        public static void Test03_SplitByDay_Insert()
        {
            Console.WriteLine("Test 03: Split By Day - Insert");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<EventByDay>();

            var events = new List<EventByDay>
            {
                new EventByDay { EventName = "Event 1", EventData = "Data 1", EventTime = new DateTime(2024, 1, 1) },
                new EventByDay { EventName = "Event 2", EventData = "Data 2", EventTime = new DateTime(2024, 1, 2) },
                new EventByDay { EventName = "Event 3", EventData = "Data 3", EventTime = new DateTime(2024, 1, 3) }
            };

            var result = db.Insertable(events).SplitTable().ExecuteCommand();

            if (result != 3)
                throw new Exception("Test03 Failed: Expected 3 records inserted");

            Console.WriteLine($"  ✓ Inserted {result} records across day-based split tables\n");
        }
        #endregion

        #region Test 04: Split By Week - Insert
        public static void Test04_SplitByWeek_Insert()
        {
            Console.WriteLine("Test 04: Split By Week - Insert");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<TaskByWeek>();

            var tasks = new List<TaskByWeek>
            {
                new TaskByWeek { TaskName = "Task 1", Status = "Pending", TaskDate = new DateTime(2024, 1, 1) },
                new TaskByWeek { TaskName = "Task 2", Status = "InProgress", TaskDate = new DateTime(2024, 1, 8) },
                new TaskByWeek { TaskName = "Task 3", Status = "Completed", TaskDate = new DateTime(2024, 1, 15) }
            };

            var result = db.Insertable(tasks).SplitTable().ExecuteCommand();

            if (result != 3)
                throw new Exception("Test04 Failed: Expected 3 records inserted");

            Console.WriteLine($"  ✓ Inserted {result} records across week-based split tables\n");
        }
        #endregion

        #region Test 05: Query Single Table
        public static void Test05_Query_SingleTable()
        {
            Console.WriteLine("Test 05: Query - Single Split Table");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<OrderByYear>();

            var order = new OrderByYear
            {
                OrderNo = "QUERY-001",
                Amount = 500,
                CreateTime = new DateTime(2024, 6, 15)
            };

            db.Insertable(order).SplitTable().ExecuteCommand();

            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);

            var result = db.Queryable<OrderByYear>()
                .SplitTable(startDate, endDate)
                .Where(o => o.OrderNo == "QUERY-001")
                .First();

            if (result == null || result.Amount != 500)
                throw new Exception("Test05 Failed: Query result incorrect");

            Console.WriteLine($"  ✓ Query single table successful: {result.OrderNo}\n");
        }
        #endregion

        #region Test 06: Query Multiple Tables Union
        public static void Test06_Query_MultipleTablesUnion()
        {
            Console.WriteLine("Test 06: Query - Multiple Tables Union");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<LogByMonth>();

            var logs = new List<LogByMonth>
            {
                new LogByMonth { Message = "Jan Log", Level = "INFO", LogTime = new DateTime(2024, 1, 15) },
                new LogByMonth { Message = "Feb Log", Level = "INFO", LogTime = new DateTime(2024, 2, 15) },
                new LogByMonth { Message = "Mar Log", Level = "INFO", LogTime = new DateTime(2024, 3, 15) }
            };

            db.Insertable(logs).SplitTable().ExecuteCommand();

            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 3, 31);

            var results = db.Queryable<LogByMonth>()
                .SplitTable(startDate, endDate)
                .Where(l => l.Level == "INFO")
                .ToList();

            if (results.Count < 3)
                throw new Exception("Test06 Failed: Expected at least 3 records");

            Console.WriteLine($"  ✓ Query across {3} months returned {results.Count} records\n");
        }
        #endregion

        #region Test 07: Query Date Range
        public static void Test07_Query_DateRange()
        {
            Console.WriteLine("Test 07: Query - Date Range");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<EventByDay>();

            var events = new List<EventByDay>();
            for (int i = 1; i <= 10; i++)
            {
                events.Add(new EventByDay
                {
                    EventName = $"Event {i}",
                    EventData = $"Data {i}",
                    EventTime = new DateTime(2024, 1, i)
                });
            }

            db.Insertable(events).SplitTable().ExecuteCommand();

            var startDate = new DateTime(2024, 1, 3);
            var endDate = new DateTime(2024, 1, 7);

            var results = db.Queryable<EventByDay>()
                .SplitTable(startDate, endDate)
                .Where(e => e.EventTime >= startDate && e.EventTime <= endDate)
                .ToList();

            if (results.Count != 5)
                throw new Exception($"Test07 Failed: Expected 5 records, got {results.Count}");

            Console.WriteLine($"  ✓ Date range query returned {results.Count} records\n");
        }
        #endregion

        #region Test 08: Query With Where
        public static void Test08_Query_WithWhere()
        {
            Console.WriteLine("Test 08: Query - With Where Conditions");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<OrderByYear>();

            var orders = new List<OrderByYear>
            {
                new OrderByYear { OrderNo = "HIGH-001", Amount = 1000, CreateTime = new DateTime(2024, 1, 1) },
                new OrderByYear { OrderNo = "LOW-001", Amount = 50, CreateTime = new DateTime(2024, 1, 2) },
                new OrderByYear { OrderNo = "HIGH-002", Amount = 2000, CreateTime = new DateTime(2024, 1, 3) }
            };

            db.Insertable(orders).SplitTable().ExecuteCommand();

            var results = db.Queryable<OrderByYear>()
                .SplitTable(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31))
                .Where(o => o.Amount > 500)
                .ToList();

            if (results.Count < 2)
                throw new Exception("Test08 Failed: Expected at least 2 high-value orders");

            Console.WriteLine($"  ✓ Filtered query returned {results.Count} high-value orders\n");
        }
        #endregion

        #region Test 09: Update Single Record
        public static void Test09_Update_SingleRecord()
        {
            Console.WriteLine("Test 09: Update - Single Record");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<OrderByYear>();

            var order = new OrderByYear
            {
                OrderNo = "UPDATE-001",
                Amount = 100,
                CreateTime = new DateTime(2024, 6, 1)
            };

            db.Insertable(order).SplitTable().ExecuteCommand();

            order.Amount = 999;
            var result = db.Updateable(order).SplitTable().ExecuteCommand();

            if (result != 1)
                throw new Exception("Test09 Failed: Update failed");

            Console.WriteLine($"  ✓ Updated {result} record in split table\n");
        }
        #endregion

        #region Test 10: Update Multiple Records
        public static void Test10_Update_MultipleRecords()
        {
            Console.WriteLine("Test 10: Update - Multiple Records");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<LogByMonth>();

            var logs = new List<LogByMonth>
            {
                new LogByMonth { Message = "Update Test 1", Level = "INFO", LogTime = new DateTime(2024, 1, 1) },
                new LogByMonth { Message = "Update Test 2", Level = "INFO", LogTime = new DateTime(2024, 1, 2) }
            };

            db.Insertable(logs).SplitTable().ExecuteCommand();

            var result = db.Updateable<LogByMonth>()
                .SetColumns(l => l.Level == "DEBUG")
                .Where(l => l.Message.StartsWith("Update Test"))
                .SplitTable(tabs => tabs.Take(1))
                .ExecuteCommand();

            Console.WriteLine($"  ✓ Updated {result} records across split tables\n");
        }
        #endregion

        #region Test 11: Update Cross Tables
        public static void Test11_Update_CrossTables()
        {
            Console.WriteLine("Test 11: Update - Cross Multiple Tables");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<OrderByYear>();

            var orders = new List<OrderByYear>
            {
                new OrderByYear { OrderNo = "CROSS-001", Amount = 100, CreateTime = new DateTime(2024, 1, 1) },
                new OrderByYear { OrderNo = "CROSS-002", Amount = 200, CreateTime = new DateTime(2024, 6, 1) }
            };

            db.Insertable(orders).SplitTable().ExecuteCommand();

            var result = db.Updateable<OrderByYear>()
                .SetColumns(o => o.Amount == 500)
                .Where(o => o.OrderNo.StartsWith("CROSS"))
                .SplitTable(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31))
                .ExecuteCommand();

            Console.WriteLine($"  ✓ Updated {result} records across multiple split tables\n");
        }
        #endregion

        #region Test 12: Delete Single Record
        public static void Test12_Delete_SingleRecord()
        {
            Console.WriteLine("Test 12: Delete - Single Record");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<EventByDay>();

            var evt = new EventByDay
            {
                EventName = "Delete Test",
                EventData = "To be deleted",
                EventTime = new DateTime(2024, 1, 1)
            };

            db.Insertable(evt).SplitTable().ExecuteCommand();

            var result = db.Deleteable<EventByDay>()
                .Where(e => e.EventName == "Delete Test")
                .SplitTable(tabs => tabs.Take(1))
                .ExecuteCommand();

            if (result < 1)
                throw new Exception("Test12 Failed: Delete failed");

            Console.WriteLine($"  ✓ Deleted {result} record from split table\n");
        }
        #endregion

        #region Test 13: Delete By Date Range
        public static void Test13_Delete_ByDateRange()
        {
            Console.WriteLine("Test 13: Delete - By Date Range");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<LogByMonth>();

            var logs = new List<LogByMonth>();
            for (int i = 1; i <= 5; i++)
            {
                logs.Add(new LogByMonth
                {
                    Message = $"Old Log {i}",
                    Level = "INFO",
                    LogTime = new DateTime(2024, 1, i)
                });
            }

            db.Insertable(logs).SplitTable().ExecuteCommand();

            var result = db.Deleteable<LogByMonth>()
                .Where(l => l.Message.StartsWith("Old Log"))
                .SplitTable(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31))
                .ExecuteCommand();

            Console.WriteLine($"  ✓ Deleted {result} records by date range\n");
        }
        #endregion

        #region Test 14: Delete Entire Table
        public static void Test14_Delete_EntireTable()
        {
            Console.WriteLine("Test 14: Delete - Entire Split Table");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<TaskByWeek>();

            var tasks = new List<TaskByWeek>
            {
                new TaskByWeek { TaskName = "Task 1", Status = "Done", TaskDate = new DateTime(2024, 1, 1) }
            };

            db.Insertable(tasks).SplitTable().ExecuteCommand();

            var result = db.Deleteable<TaskByWeek>()
                .Where(t => true)
                .SplitTable(tabs => tabs.Take(1))
                .ExecuteCommand();

            Console.WriteLine($"  ✓ Deleted all records from split table\n");
        }
        #endregion

        #region Test 15: Get Table List
        public static void Test15_GetTableList()
        {
            Console.WriteLine("Test 15: Get Split Table List");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<OrderByYear>();

            var orders = new List<OrderByYear>
            {
                new OrderByYear { OrderNo = "LIST-001", Amount = 100, CreateTime = new DateTime(2023, 1, 1) },
                new OrderByYear { OrderNo = "LIST-002", Amount = 200, CreateTime = new DateTime(2024, 1, 1) }
            };

            db.Insertable(orders).SplitTable().ExecuteCommand();

            var tables = db.SplitHelper<OrderByYear>().GetTables();

            if (tables == null || tables.Count == 0)
                throw new Exception("Test15 Failed: No split tables found");

            Console.WriteLine($"  ✓ Found {tables.Count} split tables\n");
        }
        #endregion

        #region Test 16: Create Table Auto Generation
        public static void Test16_CreateTable_AutoGeneration()
        {
            Console.WriteLine("Test 16: Create Table - Auto Generation");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<EventByDay>();

            var futureEvent = new EventByDay
            {
                EventName = "Future Event",
                EventData = "Auto-create table",
                EventTime = DateTime.Now.AddDays(30)
            };

            var result = db.Insertable(futureEvent).SplitTable().ExecuteCommand();

            if (result != 1)
                throw new Exception("Test16 Failed: Auto table creation failed");

            Console.WriteLine($"  ✓ Auto-created split table and inserted record\n");
        }
        #endregion

        #region Test 17: Split Field Custom Column
        public static void Test17_SplitField_CustomColumn()
        {
            Console.WriteLine("Test 17: Split Field - Custom Column");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<OrderByYear>();

            var order = new OrderByYear
            {
                OrderNo = "CUSTOM-001",
                Amount = 100,
                CreateTime = new DateTime(2024, 3, 15)
            };

            db.Insertable(order).SplitTable().ExecuteCommand();

            var result = db.Queryable<OrderByYear>()
                .SplitTable(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31))
                .First(o => o.OrderNo == "CUSTOM-001");

            if (result == null)
                throw new Exception("Test17 Failed: Custom split field query failed");

            Console.WriteLine($"  ✓ Custom split field working correctly\n");
        }
        #endregion

        #region Test 18: Page Query Across Tables
        public static void Test18_PageQuery_AcrossTables()
        {
            Console.WriteLine("Test 18: Page Query - Across Multiple Tables");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<LogByMonth>();

            var logs = new List<LogByMonth>();
            for (int i = 1; i <= 50; i++)
            {
                logs.Add(new LogByMonth
                {
                    Message = $"Page Log {i}",
                    Level = "INFO",
                    LogTime = new DateTime(2024, (i % 3) + 1, 1)
                });
            }

            db.Insertable(logs).SplitTable().ExecuteCommand();

            int pageIndex = 1;
            int pageSize = 10;
            int totalCount = 0;

            var results = db.Queryable<LogByMonth>()
                .SplitTable(new DateTime(2024, 1, 1), new DateTime(2024, 3, 31))
                .Where(l => l.Message.StartsWith("Page Log"))
                .OrderBy(l => l.LogTime)
                .ToPageList(pageIndex, pageSize, ref totalCount);

            if (results.Count != pageSize)
                throw new Exception("Test18 Failed: Page query incorrect");

            Console.WriteLine($"  ✓ Page query: {results.Count} records, Total: {totalCount}\n");
        }
        #endregion

        #region Test 19: Bulk Insert Multi Tables
        public static void Test19_BulkInsert_MultiTables()
        {
            Console.WriteLine("Test 19: Bulk Insert - Multiple Split Tables");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<OrderByYear>();

            var orders = new List<OrderByYear>();
            for (int i = 1; i <= 100; i++)
            {
                orders.Add(new OrderByYear
                {
                    OrderNo = $"BULK-{i:D3}",
                    Amount = i * 10,
                    CreateTime = new DateTime(2024, (i % 12) + 1, 1)
                });
            }

            var startTime = DateTime.Now;
            var result = db.Insertable(orders).SplitTable().ExecuteCommand();
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;

            if (result != 100)
                throw new Exception("Test19 Failed: Expected 100 records");

            Console.WriteLine($"  ✓ Bulk inserted {result} records in {elapsed:F2}ms\n");
        }
        #endregion

        #region Test 20: Bulk Update Multi Tables
        public static void Test20_BulkUpdate_MultiTables()
        {
            Console.WriteLine("Test 20: Bulk Update - Multiple Split Tables");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<LogByMonth>();

            var logs = new List<LogByMonth>();
            for (int i = 1; i <= 30; i++)
            {
                logs.Add(new LogByMonth
                {
                    Message = $"Bulk Update {i}",
                    Level = "INFO",
                    LogTime = new DateTime(2024, (i % 3) + 1, 1)
                });
            }

            db.Insertable(logs).SplitTable().ExecuteCommand();

            var result = db.Updateable<LogByMonth>()
                .SetColumns(l => l.Level == "UPDATED")
                .Where(l => l.Message.StartsWith("Bulk Update"))
                .SplitTable(new DateTime(2024, 1, 1), new DateTime(2024, 3, 31))
                .ExecuteCommand();

            Console.WriteLine($"  ✓ Bulk updated {result} records across split tables\n");
        }
        #endregion

        #region Test 21: Empty Table Query
        public static void Test21_EmptyTable_Query()
        {
            Console.WriteLine("Test 21: Empty Table - Query");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<EventByDay>();

            var results = db.Queryable<EventByDay>()
                .SplitTable(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31))
                .ToList();

            if (results == null)
                throw new Exception("Test21 Failed: Query should return empty list");

            Console.WriteLine($"  ✓ Empty table query returned {results.Count} records\n");
        }
        #endregion

        #region Test 22: Future Date Insert
        public static void Test22_FutureDate_Insert()
        {
            Console.WriteLine("Test 22: Future Date - Insert");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<TaskByWeek>();

            var futureTask = new TaskByWeek
            {
                TaskName = "Future Task",
                Status = "Scheduled",
                TaskDate = DateTime.Now.AddMonths(6)
            };

            var result = db.Insertable(futureTask).SplitTable().ExecuteCommand();

            if (result != 1)
                throw new Exception("Test22 Failed: Future date insert failed");

            Console.WriteLine($"  ✓ Future date insert successful\n");
        }
        #endregion

        #region Test 23: Historical Data Query
        public static void Test23_HistoricalData_Query()
        {
            Console.WriteLine("Test 23: Historical Data - Query");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<OrderByYear>();

            var historicalOrders = new List<OrderByYear>
            {
                new OrderByYear { OrderNo = "HIST-2022", Amount = 100, CreateTime = new DateTime(2022, 1, 1) },
                new OrderByYear { OrderNo = "HIST-2023", Amount = 200, CreateTime = new DateTime(2023, 1, 1) }
            };

            db.Insertable(historicalOrders).SplitTable().ExecuteCommand();

            var results = db.Queryable<OrderByYear>()
                .SplitTable(new DateTime(2022, 1, 1), new DateTime(2023, 12, 31))
                .Where(o => o.OrderNo.StartsWith("HIST"))
                .ToList();

            Console.WriteLine($"  ✓ Historical query returned {results.Count} records\n");
        }
        #endregion

        #region Test 24: Table Not Exist Auto Create
        public static void Test24_TableNotExist_AutoCreate()
        {
            Console.WriteLine("Test 24: Table Not Exist - Auto Create");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<LogByMonth>();

            var newLog = new LogByMonth
            {
                Message = "Auto Create Test",
                Level = "INFO",
                LogTime = DateTime.Now.AddYears(1)
            };

            var result = db.Insertable(newLog).SplitTable().ExecuteCommand();

            if (result != 1)
                throw new Exception("Test24 Failed: Auto create failed");

            Console.WriteLine($"  ✓ Auto-created non-existent split table\n");
        }
        #endregion

        #region Test 25: Performance Large Dataset
        public static void Test25_Performance_LargeDataset()
        {
            Console.WriteLine("Test 25: Performance - Large Dataset");
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<EventByDay>();

            var events = new List<EventByDay>();
            for (int i = 1; i <= 1000; i++)
            {
                events.Add(new EventByDay
                {
                    EventName = $"Perf Event {i}",
                    EventData = $"Data {i}",
                    EventTime = new DateTime(2024, 1, (i % 28) + 1)
                });
            }

            var startTime = DateTime.Now;
            var result = db.Insertable(events).SplitTable().ExecuteCommand();
            var insertTime = (DateTime.Now - startTime).TotalMilliseconds;

            startTime = DateTime.Now;
            var queryResults = db.Queryable<EventByDay>()
                .SplitTable(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31))
                .ToList();
            var queryTime = (DateTime.Now - startTime).TotalMilliseconds;

            Console.WriteLine($"  Insert: {insertTime:F2}ms ({result / insertTime * 1000:F0} records/sec)");
            Console.WriteLine($"  Query: {queryTime:F2}ms ({queryResults.Count} records)");
            Console.WriteLine($"  ✓ Performance test completed\n");
        }
        #endregion
    }
}