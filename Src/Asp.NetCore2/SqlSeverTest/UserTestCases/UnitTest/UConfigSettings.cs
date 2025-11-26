using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class UConfigSettings
    {
        public static void Init()
        {
            Console.WriteLine("\n========== UConfigSettings Test Suite ==========");
            Console.WriteLine("Testing SqlSugar Config and Settings Classes\n");
            
            // ConnMoreSettings Tests
            Console.WriteLine("--- ConnMoreSettings Tests ---");
            Test_ConnMoreSettings_DefaultValues();
            Test_ConnMoreSettings_WithNoLock();
            Test_ConnMoreSettings_PostgresSettings();
            Test_ConnMoreSettings_CacheSettings();
            Test_ConnMoreSettings_CodeFirstSettings();
            Test_ConnMoreSettings_OracleSettings();
            Test_ConnMoreSettings_ClickHouseSettings();
            
            // DbTableInfo Tests
            Console.WriteLine("\n--- DbTableInfo Tests ---");
            Test_DbTableInfo_BasicProperties();
            Test_DbTableInfo_ObjectType_Table();
            Test_DbTableInfo_ObjectType_View();
            
            // RazorTableInfo Tests
            Console.WriteLine("\n--- RazorTableInfo Tests ---");
            Test_RazorTableInfo_BasicProperties();
            Test_RazorTableInfo_WithColumns();
            Test_RazorTableInfo_EmptyColumns();
            
            // RazorColumnInfo Tests
            Console.WriteLine("\n--- RazorColumnInfo Tests ---");
            Test_RazorColumnInfo_AllProperties();
            Test_RazorColumnInfo_PrimaryKey();
            Test_RazorColumnInfo_Identity();
            
            // DbResult Tests
            Console.WriteLine("\n--- DbResult Tests ---");
            Test_DbResult_Success();
            Test_DbResult_Failure();
            Test_DbResult_WithData();
            Test_DbResult_GenericType();
            
            // SugarParameter Tests
            Console.WriteLine("\n--- SugarParameter Tests ---");
            Test_SugarParameter_BasicConstructor();
            Test_SugarParameter_WithValue();
            Test_SugarParameter_WithDirection();
            Test_SugarParameter_NullValue();
            Test_SugarParameter_TypeName();
            
            // JoinQueryInfo Tests
            Console.WriteLine("\n--- JoinQueryInfo Tests ---");
            Test_JoinQueryInfo_BasicProperties();
            Test_JoinQueryInfo_JoinTypes();
            Test_JoinQueryInfo_TableShortName();
            
            // SchemaInfo Tests
            Console.WriteLine("\n--- SchemaInfo Tests ---");
            Test_SchemaInfo_BasicProperties();
            Test_SchemaInfo_WithTables();
            
            // QueueItem Tests
            Console.WriteLine("\n--- QueueItem Tests ---");
            Test_QueueItem_BasicProperties();
            Test_QueueItem_WithParameters();
            
            // StackTraceInfo Tests
            Console.WriteLine("\n--- StackTraceInfo Tests ---");
            Test_StackTraceInfo_BasicProperties();
            Test_StackTraceInfo_MethodInfo();
            
            // SqlFilter Tests
            Console.WriteLine("\n--- SqlFilter Tests ---");
            Test_SqlFilter_BasicProperties();
            Test_SqlFilter_FilterValue();
            
            Console.WriteLine("\n========== All UConfigSettings Tests Passed! ==========\n");
        }

        #region ConnMoreSettings Tests
        
        private static void Test_ConnMoreSettings_DefaultValues()
        {
            var settings = new ConnMoreSettings();
            
            if (settings.IsAutoRemoveDataCache != false)
                throw new Exception("Test_ConnMoreSettings_DefaultValues failed");
            if (settings.PgSqlIsAutoToLower != true)
                throw new Exception("Test_ConnMoreSettings_DefaultValues failed - PgSqlIsAutoToLower");
            if (settings.IsAutoToUpper != true)
                throw new Exception("Test_ConnMoreSettings_DefaultValues failed - IsAutoToUpper");
            Console.WriteLine("  ✓ Test_ConnMoreSettings_DefaultValues passed");
        }
        
        private static void Test_ConnMoreSettings_WithNoLock()
        {
            var settings = new ConnMoreSettings
            {
                IsWithNoLockQuery = true,
                IsWithNoLockSubquery = true,
                DisableWithNoLockWithTran = true
            };
            
            if (!settings.IsWithNoLockQuery || !settings.IsWithNoLockSubquery)
                throw new Exception("Test_ConnMoreSettings_WithNoLock failed");
            Console.WriteLine("  ✓ Test_ConnMoreSettings_WithNoLock passed");
        }
        
        private static void Test_ConnMoreSettings_PostgresSettings()
        {
            var settings = new ConnMoreSettings
            {
                PgSqlIsAutoToLower = false,
                PgSqlIsAutoToLowerCodeFirst = false,
                PgSqlIsAutoToLowerSchema = false,
                EnableILike = true,
                PostgresIdentityStrategy = PostgresIdentityStrategy.Identity
            };
            
            if (settings.PgSqlIsAutoToLower || !settings.EnableILike)
                throw new Exception("Test_ConnMoreSettings_PostgresSettings failed");
            Console.WriteLine("  ✓ Test_ConnMoreSettings_PostgresSettings passed");
        }
        
        private static void Test_ConnMoreSettings_CacheSettings()
        {
            var settings = new ConnMoreSettings
            {
                IsAutoRemoveDataCache = true,
                DefaultCacheDurationInSeconds = 300
            };
            
            if (!settings.IsAutoRemoveDataCache || settings.DefaultCacheDurationInSeconds != 300)
                throw new Exception("Test_ConnMoreSettings_CacheSettings failed");
            Console.WriteLine("  ✓ Test_ConnMoreSettings_CacheSettings passed");
        }
        
        private static void Test_ConnMoreSettings_CodeFirstSettings()
        {
            var settings = new ConnMoreSettings
            {
                SqlServerCodeFirstNvarchar = true,
                SqliteCodeFirstEnableDefaultValue = true,
                SqliteCodeFirstEnableDescription = true,
                SqliteCodeFirstEnableDropColumn = true,
                EnableCodeFirstUpdatePrecision = true
            };
            
            if (!settings.SqlServerCodeFirstNvarchar || !settings.SqliteCodeFirstEnableDefaultValue)
                throw new Exception("Test_ConnMoreSettings_CodeFirstSettings failed");
            Console.WriteLine("  ✓ Test_ConnMoreSettings_CodeFirstSettings passed");
        }
        
        private static void Test_ConnMoreSettings_OracleSettings()
        {
            var settings = new ConnMoreSettings
            {
                OracleCodeFirstNvarchar2 = true,
                EnableOracleIdentity = true
            };
            
            if (!settings.OracleCodeFirstNvarchar2 || !settings.EnableOracleIdentity)
                throw new Exception("Test_ConnMoreSettings_OracleSettings failed");
            Console.WriteLine("  ✓ Test_ConnMoreSettings_OracleSettings passed");
        }
        
        private static void Test_ConnMoreSettings_ClickHouseSettings()
        {
            var settings = new ConnMoreSettings
            {
                ClickHouseEnableFinal = true,
                EnableJsonb = true
            };
            
            if (!settings.ClickHouseEnableFinal || !settings.EnableJsonb)
                throw new Exception("Test_ConnMoreSettings_ClickHouseSettings failed");
            Console.WriteLine("  ✓ Test_ConnMoreSettings_ClickHouseSettings passed");
        }
        
        #endregion

        #region DbTableInfo Tests
        
        private static void Test_DbTableInfo_BasicProperties()
        {
            var info = new DbTableInfo
            {
                Name = "Users",
                Description = "User table"
            };
            
            if (info.Name != "Users" || info.Description != "User table")
                throw new Exception("Test_DbTableInfo_BasicProperties failed");
            Console.WriteLine("  ✓ Test_DbTableInfo_BasicProperties passed");
        }
        
        private static void Test_DbTableInfo_ObjectType_Table()
        {
            var info = new DbTableInfo { DbObjectType = DbObjectType.Table };
            
            if (info.DbObjectType != DbObjectType.Table)
                throw new Exception("Test_DbTableInfo_ObjectType_Table failed");
            Console.WriteLine("  ✓ Test_DbTableInfo_ObjectType_Table passed");
        }
        
        private static void Test_DbTableInfo_ObjectType_View()
        {
            var info = new DbTableInfo { DbObjectType = DbObjectType.View };
            
            if (info.DbObjectType != DbObjectType.View)
                throw new Exception("Test_DbTableInfo_ObjectType_View failed");
            Console.WriteLine("  ✓ Test_DbTableInfo_ObjectType_View passed");
        }
        
        #endregion

        #region RazorTableInfo Tests
        
        private static void Test_RazorTableInfo_BasicProperties()
        {
            var info = new RazorTableInfo
            {
                DbTableName = "tbl_orders",
                ClassName = "Order",
                Description = "Order table"
            };
            
            if (info.DbTableName != "tbl_orders" || info.ClassName != "Order")
                throw new Exception("Test_RazorTableInfo_BasicProperties failed");
            Console.WriteLine("  ✓ Test_RazorTableInfo_BasicProperties passed");
        }
        
        private static void Test_RazorTableInfo_WithColumns()
        {
            var info = new RazorTableInfo
            {
                DbTableName = "Products",
                Columns = new List<RazorColumnInfo>
                {
                    new RazorColumnInfo { DbColumnName = "Id", IsPrimarykey = true },
                    new RazorColumnInfo { DbColumnName = "Name", DataType = "varchar" }
                }
            };
            
            if (info.Columns.Count != 2)
                throw new Exception("Test_RazorTableInfo_WithColumns failed");
            Console.WriteLine("  ✓ Test_RazorTableInfo_WithColumns passed");
        }
        
        private static void Test_RazorTableInfo_EmptyColumns()
        {
            var info = new RazorTableInfo
            {
                DbTableName = "Empty",
                Columns = new List<RazorColumnInfo>()
            };
            
            if (info.Columns.Count != 0)
                throw new Exception("Test_RazorTableInfo_EmptyColumns failed");
            Console.WriteLine("  ✓ Test_RazorTableInfo_EmptyColumns passed");
        }
        
        #endregion

        #region RazorColumnInfo Tests
        
        private static void Test_RazorColumnInfo_AllProperties()
        {
            var info = new RazorColumnInfo
            {
                DbColumnName = "Email",
                DataType = "nvarchar",
                Length = 255,
                ColumnDescription = "User email",
                DefaultValue = "",
                IsNullable = true
            };
            
            if (info.DbColumnName != "Email" || info.Length != 255)
                throw new Exception("Test_RazorColumnInfo_AllProperties failed");
            Console.WriteLine("  ✓ Test_RazorColumnInfo_AllProperties passed");
        }
        
        private static void Test_RazorColumnInfo_PrimaryKey()
        {
            var info = new RazorColumnInfo
            {
                DbColumnName = "Id",
                IsPrimarykey = true,
                IsIdentity = true
            };
            
            if (!info.IsPrimarykey || !info.IsIdentity)
                throw new Exception("Test_RazorColumnInfo_PrimaryKey failed");
            Console.WriteLine("  ✓ Test_RazorColumnInfo_PrimaryKey passed");
        }
        
        private static void Test_RazorColumnInfo_Identity()
        {
            var info = new RazorColumnInfo { IsIdentity = true };
            
            if (!info.IsIdentity)
                throw new Exception("Test_RazorColumnInfo_Identity failed");
            Console.WriteLine("  ✓ Test_RazorColumnInfo_Identity passed");
        }
        
        #endregion

        #region DbResult Tests
        
        private static void Test_DbResult_Success()
        {
            var result = new DbResult<int>
            {
                IsSuccess = true,
                Data = 100
            };
            
            if (!result.IsSuccess || result.Data != 100)
                throw new Exception("Test_DbResult_Success failed");
            Console.WriteLine("  ✓ Test_DbResult_Success passed");
        }
        
        private static void Test_DbResult_Failure()
        {
            var result = new DbResult<string>
            {
                IsSuccess = false,
                ErrorMessage = "Operation failed"
            };
            
            if (result.IsSuccess || result.ErrorMessage != "Operation failed")
                throw new Exception("Test_DbResult_Failure failed");
            Console.WriteLine("  ✓ Test_DbResult_Failure passed");
        }
        
        private static void Test_DbResult_WithData()
        {
            var result = new DbResult<List<int>>
            {
                IsSuccess = true,
                Data = new List<int> { 1, 2, 3 }
            };
            
            if (result.Data.Count != 3)
                throw new Exception("Test_DbResult_WithData failed");
            Console.WriteLine("  ✓ Test_DbResult_WithData passed");
        }
        
        private static void Test_DbResult_GenericType()
        {
            var result = new DbResult<object>
            {
                IsSuccess = true,
                Data = new { Id = 1, Name = "Test" }
            };
            
            if (!result.IsSuccess)
                throw new Exception("Test_DbResult_GenericType failed");
            Console.WriteLine("  ✓ Test_DbResult_GenericType passed");
        }
        
        #endregion

        #region SugarParameter Tests
        
        private static void Test_SugarParameter_BasicConstructor()
        {
            var param = new SugarParameter("@id", 1);
            
            if (param.ParameterName != "@id")
                throw new Exception("Test_SugarParameter_BasicConstructor failed");
            Console.WriteLine("  ✓ Test_SugarParameter_BasicConstructor passed");
        }
        
        private static void Test_SugarParameter_WithValue()
        {
            var param = new SugarParameter("@name", "test");
            
            if (param.Value.ToString() != "test")
                throw new Exception("Test_SugarParameter_WithValue failed");
            Console.WriteLine("  ✓ Test_SugarParameter_WithValue passed");
        }
        
        private static void Test_SugarParameter_WithDirection()
        {
            var param = new SugarParameter("@output", null, true);
            
            if (!param.IsOutput)
                throw new Exception("Test_SugarParameter_WithDirection failed");
            Console.WriteLine("  ✓ Test_SugarParameter_WithDirection passed");
        }
        
        private static void Test_SugarParameter_NullValue()
        {
            var param = new SugarParameter("@nullable", null);
            
            if (param.ParameterName != "@nullable")
                throw new Exception("Test_SugarParameter_NullValue failed");
            Console.WriteLine("  ✓ Test_SugarParameter_NullValue passed");
        }
        
        private static void Test_SugarParameter_TypeName()
        {
            var param = new SugarParameter("@json", "{}", "json");
            
            if (param.TypeName != "json")
                throw new Exception("Test_SugarParameter_TypeName failed");
            Console.WriteLine("  ✓ Test_SugarParameter_TypeName passed");
        }
        
        #endregion

        #region JoinQueryInfo Tests
        
        private static void Test_JoinQueryInfo_BasicProperties()
        {
            var info = new JoinQueryInfo
            {
                TableName = "Orders"
            };
            
            if (info.TableName != "Orders")
                throw new Exception("Test_JoinQueryInfo_BasicProperties failed");
            Console.WriteLine("  ✓ Test_JoinQueryInfo_BasicProperties passed");
        }
        
        private static void Test_JoinQueryInfo_JoinTypes()
        {
            var leftJoin = new JoinQueryInfo { JoinType = JoinType.Left };
            var rightJoin = new JoinQueryInfo { JoinType = JoinType.Right };
            var innerJoin = new JoinQueryInfo { JoinType = JoinType.Inner };
            
            if (leftJoin.JoinType != JoinType.Left || rightJoin.JoinType != JoinType.Right)
                throw new Exception("Test_JoinQueryInfo_JoinTypes failed");
            Console.WriteLine("  ✓ Test_JoinQueryInfo_JoinTypes passed");
        }
        
        private static void Test_JoinQueryInfo_TableShortName()
        {
            var info = new JoinQueryInfo
            {
                TableName = "OrderItems",
                ShortName = "oi"
            };
            
            if (info.ShortName != "oi")
                throw new Exception("Test_JoinQueryInfo_TableShortName failed");
            Console.WriteLine("  ✓ Test_JoinQueryInfo_TableShortName passed");
        }
        
        #endregion

        #region SchemaInfo Tests
        
        private static void Test_SchemaInfo_BasicProperties()
        {
            var info = new SchemaInfo
            {
                SchemaName = "dbo"
            };
            
            if (info.SchemaName != "dbo")
                throw new Exception("Test_SchemaInfo_BasicProperties failed");
            Console.WriteLine("  ✓ Test_SchemaInfo_BasicProperties passed");
        }
        
        private static void Test_SchemaInfo_WithTables()
        {
            var info = new SchemaInfo
            {
                SchemaName = "public",
                Tables = new List<DbTableInfo>
                {
                    new DbTableInfo { Name = "Users" },
                    new DbTableInfo { Name = "Orders" }
                }
            };
            
            if (info.Tables.Count != 2)
                throw new Exception("Test_SchemaInfo_WithTables failed");
            Console.WriteLine("  ✓ Test_SchemaInfo_WithTables passed");
        }
        
        #endregion

        #region QueueItem Tests
        
        private static void Test_QueueItem_BasicProperties()
        {
            var item = new QueueItem
            {
                Sql = "INSERT INTO Users VALUES (@id, @name)"
            };
            
            if (item.Sql == null)
                throw new Exception("Test_QueueItem_BasicProperties failed");
            Console.WriteLine("  ✓ Test_QueueItem_BasicProperties passed");
        }
        
        private static void Test_QueueItem_WithParameters()
        {
            var item = new QueueItem
            {
                Sql = "SELECT * FROM Users WHERE Id = @id",
                Parameters = new SugarParameter[] { new SugarParameter("@id", 1) }
            };
            
            if (item.Parameters.Length != 1)
                throw new Exception("Test_QueueItem_WithParameters failed");
            Console.WriteLine("  ✓ Test_QueueItem_WithParameters passed");
        }
        
        #endregion

        #region StackTraceInfo Tests
        
        private static void Test_StackTraceInfo_BasicProperties()
        {
            var info = new StackTraceInfo
            {
                FileName = "Test.cs",
                Line = 100
            };
            
            if (info.FileName != "Test.cs" || info.Line != 100)
                throw new Exception("Test_StackTraceInfo_BasicProperties failed");
            Console.WriteLine("  ✓ Test_StackTraceInfo_BasicProperties passed");
        }
        
        private static void Test_StackTraceInfo_MethodInfo()
        {
            var info = new StackTraceInfo
            {
                MethodName = "TestMethod",
                ClassName = "TestClass"
            };
            
            if (info.MethodName != "TestMethod" || info.ClassName != "TestClass")
                throw new Exception("Test_StackTraceInfo_MethodInfo failed");
            Console.WriteLine("  ✓ Test_StackTraceInfo_MethodInfo passed");
        }
        
        #endregion

        #region SqlFilter Tests
        
        private static void Test_SqlFilter_BasicProperties()
        {
            var filter = new SqlFilterItem
            {
                FilterName = "TenantFilter"
            };
            
            if (filter.FilterName != "TenantFilter")
                throw new Exception("Test_SqlFilter_BasicProperties failed");
            Console.WriteLine("  ✓ Test_SqlFilter_BasicProperties passed");
        }
        
        private static void Test_SqlFilter_FilterValue()
        {
            var filter = new SqlFilterItem
            {
                FilterName = "SoftDelete",
                FilterValue = (db) => new SqlFilterResult { Sql = "IsDeleted = 0" }
            };
            
            if (filter.FilterName != "SoftDelete")
                throw new Exception("Test_SqlFilter_FilterValue failed");
            Console.WriteLine("  ✓ Test_SqlFilter_FilterValue passed");
        }
        
        #endregion
    }
}
