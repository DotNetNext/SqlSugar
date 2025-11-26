using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class UEntityModels
    {
        public static void Init()
        {
            Console.WriteLine("\n========== UEntityModels Test Suite ==========");
            Console.WriteLine("Testing SqlSugar Entity Models\n");
            
            // PageModel Tests
            Console.WriteLine("--- PageModel Tests ---");
            Test_PageModel_DefaultValues();
            Test_PageModel_SetProperties();
            Test_PageModel_Pagination();
            
            // DbColumnInfo Tests
            Console.WriteLine("\n--- DbColumnInfo Tests ---");
            Test_DbColumnInfo_DefaultValues();
            Test_DbColumnInfo_AllProperties();
            Test_DbColumnInfo_PrimaryKey();
            Test_DbColumnInfo_Identity();
            Test_DbColumnInfo_Nullable();
            Test_DbColumnInfo_WithType();
            
            // EntityInfo Tests
            Console.WriteLine("\n--- EntityInfo Tests ---");
            Test_EntityInfo_DefaultDbTableName();
            Test_EntityInfo_CustomDbTableName();
            Test_EntityInfo_WithColumns();
            Test_EntityInfo_DisabledFlags();
            
            // EntityColumnInfo Tests
            Console.WriteLine("\n--- EntityColumnInfo Tests ---");
            Test_EntityColumnInfo_BasicProperties();
            Test_EntityColumnInfo_IgnoreFlags();
            Test_EntityColumnInfo_IndexGroups();
            Test_EntityColumnInfo_ServerTime();
            
            // ConditionalModel Tests
            Console.WriteLine("\n--- ConditionalModel Tests ---");
            Test_ConditionalModel_DefaultType();
            Test_ConditionalModel_SetProperties();
            Test_ConditionalModel_CreateList();
            Test_ConditionalCollections();
            Test_ConditionalTree();
            
            // CacheKey Tests
            Console.WriteLine("\n--- CacheKey Tests ---");
            Test_CacheKey_ToString();
            Test_CacheKey_WithAppendKey();
            Test_CacheKey_MultipleTables();
            
            // DiffLogModel Tests
            Console.WriteLine("\n--- DiffLogModel Tests ---");
            Test_DiffLogModel_BasicProperties();
            Test_DiffLogTableInfo();
            Test_DiffLogColumnInfo();
            Test_DiffLogModel_WithData();
            
            // ConnectionConfig Tests
            Console.WriteLine("\n--- ConnectionConfig Tests ---");
            Test_ConnectionConfig_BasicProperties();
            Test_ConnectionConfig_DbType();
            Test_ConnectionConfig_AutoClose();
            Test_ConnectionConfig_InitKeyType();
            Test_ConnectionConfig_SlaveConfigs();
            
            // AopEvents Tests
            Console.WriteLine("\n--- AopEvents Tests ---");
            Test_AopEvents_OnError();
            Test_AopEvents_OnLogExecuting();
            Test_AopEvents_DataExecuting();
            
            // SlaveConnectionConfig Tests
            Console.WriteLine("\n--- SlaveConnectionConfig Tests ---");
            Test_SlaveConnectionConfig_Basic();
            Test_SlaveConnectionConfig_HitRate();
            
            Console.WriteLine("\n========== All UEntityModels Tests Passed! ==========\n");
        }

        #region PageModel Tests
        
        private static void Test_PageModel_DefaultValues()
        {
            var model = new PageModel();
            
            if (model.PageIndex != 0 || model.PageSize != 0 || model.TotalCount != 0)
                throw new Exception("Test_PageModel_DefaultValues failed");
            Console.WriteLine("  ✓ Test_PageModel_DefaultValues passed");
        }
        
        private static void Test_PageModel_SetProperties()
        {
            var model = new PageModel
            {
                PageIndex = 1,
                PageSize = 20,
                TotalCount = 100
            };
            
            if (model.PageIndex != 1 || model.PageSize != 20 || model.TotalCount != 100)
                throw new Exception("Test_PageModel_SetProperties failed");
            Console.WriteLine("  ✓ Test_PageModel_SetProperties passed");
        }
        
        private static void Test_PageModel_Pagination()
        {
            var model = new PageModel { PageIndex = 3, PageSize = 10, TotalCount = 95 };
            int totalPages = (int)Math.Ceiling((double)model.TotalCount / model.PageSize);
            
            if (totalPages != 10)
                throw new Exception("Test_PageModel_Pagination failed");
            Console.WriteLine("  ✓ Test_PageModel_Pagination passed");
        }
        
        #endregion

        #region DbColumnInfo Tests
        
        private static void Test_DbColumnInfo_DefaultValues()
        {
            var info = new DbColumnInfo();
            
            if (info.IsNullable != false || info.IsIdentity != false || info.IsPrimarykey != false)
                throw new Exception("Test_DbColumnInfo_DefaultValues failed");
            Console.WriteLine("  ✓ Test_DbColumnInfo_DefaultValues passed");
        }
        
        private static void Test_DbColumnInfo_AllProperties()
        {
            var info = new DbColumnInfo
            {
                TableName = "Users",
                TableId = 1,
                DbColumnName = "Id",
                PropertyName = "Id",
                DataType = "int",
                Length = 4,
                ColumnDescription = "Primary key",
                DefaultValue = "0",
                DecimalDigits = 0,
                Scale = 0
            };
            
            if (info.TableName != "Users" || info.DbColumnName != "Id")
                throw new Exception("Test_DbColumnInfo_AllProperties failed");
            Console.WriteLine("  ✓ Test_DbColumnInfo_AllProperties passed");
        }
        
        private static void Test_DbColumnInfo_PrimaryKey()
        {
            var info = new DbColumnInfo { IsPrimarykey = true };
            
            if (!info.IsPrimarykey)
                throw new Exception("Test_DbColumnInfo_PrimaryKey failed");
            Console.WriteLine("  ✓ Test_DbColumnInfo_PrimaryKey passed");
        }
        
        private static void Test_DbColumnInfo_Identity()
        {
            var info = new DbColumnInfo { IsIdentity = true };
            
            if (!info.IsIdentity)
                throw new Exception("Test_DbColumnInfo_Identity failed");
            Console.WriteLine("  ✓ Test_DbColumnInfo_Identity passed");
        }
        
        private static void Test_DbColumnInfo_Nullable()
        {
            var info = new DbColumnInfo { IsNullable = true };
            
            if (!info.IsNullable)
                throw new Exception("Test_DbColumnInfo_Nullable failed");
            Console.WriteLine("  ✓ Test_DbColumnInfo_Nullable passed");
        }
        
        private static void Test_DbColumnInfo_WithType()
        {
            var info = new DbColumnInfo
            {
                PropertyType = typeof(string),
                IsArray = true,
                IsJson = true
            };
            
            if (info.PropertyType != typeof(string) || !info.IsArray || !info.IsJson)
                throw new Exception("Test_DbColumnInfo_WithType failed");
            Console.WriteLine("  ✓ Test_DbColumnInfo_WithType passed");
        }
        
        #endregion

        #region EntityInfo Tests
        
        private static void Test_EntityInfo_DefaultDbTableName()
        {
            var info = new EntityInfo { EntityName = "User" };
            
            if (info.DbTableName != "User")
                throw new Exception("Test_EntityInfo_DefaultDbTableName failed");
            Console.WriteLine("  ✓ Test_EntityInfo_DefaultDbTableName passed");
        }
        
        private static void Test_EntityInfo_CustomDbTableName()
        {
            var info = new EntityInfo
            {
                EntityName = "User",
                DbTableName = "tbl_users"
            };
            
            if (info.DbTableName != "tbl_users")
                throw new Exception("Test_EntityInfo_CustomDbTableName failed");
            Console.WriteLine("  ✓ Test_EntityInfo_CustomDbTableName passed");
        }
        
        private static void Test_EntityInfo_WithColumns()
        {
            var info = new EntityInfo
            {
                EntityName = "Order",
                Columns = new List<EntityColumnInfo>
                {
                    new EntityColumnInfo { PropertyName = "Id", IsPrimarykey = true },
                    new EntityColumnInfo { PropertyName = "Name" }
                }
            };
            
            if (info.Columns.Count != 2)
                throw new Exception("Test_EntityInfo_WithColumns failed");
            Console.WriteLine("  ✓ Test_EntityInfo_WithColumns passed");
        }
        
        private static void Test_EntityInfo_DisabledFlags()
        {
            var info = new EntityInfo
            {
                IsDisabledDelete = true,
                IsDisabledUpdateAll = true
            };
            
            if (!info.IsDisabledDelete || !info.IsDisabledUpdateAll)
                throw new Exception("Test_EntityInfo_DisabledFlags failed");
            Console.WriteLine("  ✓ Test_EntityInfo_DisabledFlags passed");
        }
        
        #endregion

        #region EntityColumnInfo Tests
        
        private static void Test_EntityColumnInfo_BasicProperties()
        {
            var info = new EntityColumnInfo
            {
                PropertyName = "Name",
                DbColumnName = "user_name",
                Length = 100,
                IsNullable = true
            };
            
            if (info.PropertyName != "Name" || info.DbColumnName != "user_name")
                throw new Exception("Test_EntityColumnInfo_BasicProperties failed");
            Console.WriteLine("  ✓ Test_EntityColumnInfo_BasicProperties passed");
        }
        
        private static void Test_EntityColumnInfo_IgnoreFlags()
        {
            var info = new EntityColumnInfo
            {
                IsIgnore = true,
                IsOnlyIgnoreInsert = true,
                IsOnlyIgnoreUpdate = true
            };
            
            if (!info.IsIgnore || !info.IsOnlyIgnoreInsert || !info.IsOnlyIgnoreUpdate)
                throw new Exception("Test_EntityColumnInfo_IgnoreFlags failed");
            Console.WriteLine("  ✓ Test_EntityColumnInfo_IgnoreFlags passed");
        }
        
        private static void Test_EntityColumnInfo_IndexGroups()
        {
            var info = new EntityColumnInfo
            {
                IndexGroupNameList = new[] { "idx_name", "idx_composite" },
                UIndexGroupNameList = new[] { "uix_email" }
            };
            
            if (info.IndexGroupNameList.Length != 2 || info.UIndexGroupNameList.Length != 1)
                throw new Exception("Test_EntityColumnInfo_IndexGroups failed");
            Console.WriteLine("  ✓ Test_EntityColumnInfo_IndexGroups passed");
        }
        
        private static void Test_EntityColumnInfo_ServerTime()
        {
            var info = new EntityColumnInfo
            {
                InsertServerTime = true,
                UpdateServerTime = true,
                InsertSql = "GETDATE()",
                UpdateSql = "GETDATE()"
            };
            
            if (!info.InsertServerTime || !info.UpdateServerTime)
                throw new Exception("Test_EntityColumnInfo_ServerTime failed");
            Console.WriteLine("  ✓ Test_EntityColumnInfo_ServerTime passed");
        }
        
        #endregion

        #region ConditionalModel Tests
        
        private static void Test_ConditionalModel_DefaultType()
        {
            var model = new ConditionalModel();
            
            if (model.ConditionalType != ConditionalType.Equal)
                throw new Exception("Test_ConditionalModel_DefaultType failed");
            Console.WriteLine("  ✓ Test_ConditionalModel_DefaultType passed");
        }
        
        private static void Test_ConditionalModel_SetProperties()
        {
            var model = new ConditionalModel
            {
                FieldName = "Status",
                FieldValue = "Active",
                ConditionalType = ConditionalType.Like
            };
            
            if (model.FieldName != "Status" || model.ConditionalType != ConditionalType.Like)
                throw new Exception("Test_ConditionalModel_SetProperties failed");
            Console.WriteLine("  ✓ Test_ConditionalModel_SetProperties passed");
        }
        
        private static void Test_ConditionalModel_CreateList()
        {
            var list = ConditionalModel.Create(
                new ConditionalModel { FieldName = "A" },
                new ConditionalModel { FieldName = "B" }
            );
            
            if (list.Count != 2)
                throw new Exception("Test_ConditionalModel_CreateList failed");
            Console.WriteLine("  ✓ Test_ConditionalModel_CreateList passed");
        }
        
        private static void Test_ConditionalCollections()
        {
            var collection = new ConditionalCollections
            {
                ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>
                {
                    new KeyValuePair<WhereType, ConditionalModel>(
                        WhereType.And,
                        new ConditionalModel { FieldName = "Id", FieldValue = "1" }
                    )
                }
            };
            
            if (collection.ConditionalList.Count != 1)
                throw new Exception("Test_ConditionalCollections failed");
            Console.WriteLine("  ✓ Test_ConditionalCollections passed");
        }
        
        private static void Test_ConditionalTree()
        {
            var tree = new ConditionalTree
            {
                ConditionalList = new List<KeyValuePair<WhereType, IConditionalModel>>
                {
                    new KeyValuePair<WhereType, IConditionalModel>(
                        WhereType.Or,
                        new ConditionalModel { FieldName = "Name" }
                    )
                }
            };
            
            if (tree.ConditionalList.Count != 1)
                throw new Exception("Test_ConditionalTree failed");
            Console.WriteLine("  ✓ Test_ConditionalTree passed");
        }
        
        #endregion

        #region CacheKey Tests
        
        private static void Test_CacheKey_ToString()
        {
            var key = new CacheKey
            {
                Tables = new List<string> { "Users" },
                IdentificationList = new List<string> { "query1" }
            };
            
            var result = key.ToString();
            if (!result.Contains("SqlSugarDataCache") || !result.Contains("Users"))
                throw new Exception("Test_CacheKey_ToString failed");
            Console.WriteLine("  ✓ Test_CacheKey_ToString passed");
        }
        
        private static void Test_CacheKey_WithAppendKey()
        {
            var key = new CacheKey
            {
                Tables = new List<string> { "Orders" },
                IdentificationList = new List<string> { "id1" },
                AppendKey = "_custom"
            };
            
            var result = key.ToString();
            if (!result.EndsWith("_custom"))
                throw new Exception("Test_CacheKey_WithAppendKey failed");
            Console.WriteLine("  ✓ Test_CacheKey_WithAppendKey passed");
        }
        
        private static void Test_CacheKey_MultipleTables()
        {
            var key = new CacheKey
            {
                Tables = new List<string> { "Users", "Orders", "Products" },
                IdentificationList = new List<string> { "complex" }
            };
            
            var result = key.ToString();
            if (!result.Contains("Users") || !result.Contains("Orders") || !result.Contains("Products"))
                throw new Exception("Test_CacheKey_MultipleTables failed");
            Console.WriteLine("  ✓ Test_CacheKey_MultipleTables passed");
        }
        
        #endregion

        #region DiffLogModel Tests
        
        private static void Test_DiffLogModel_BasicProperties()
        {
            var model = new DiffLogModel
            {
                Sql = "UPDATE Users SET Name = @p0",
                DiffType = DiffType.update,
                Time = TimeSpan.FromMilliseconds(50)
            };
            
            if (model.Sql == null || model.DiffType != DiffType.update)
                throw new Exception("Test_DiffLogModel_BasicProperties failed");
            Console.WriteLine("  ✓ Test_DiffLogModel_BasicProperties passed");
        }
        
        private static void Test_DiffLogTableInfo()
        {
            var info = new DiffLogTableInfo
            {
                TableName = "Users",
                TableDescription = "User table",
                Columns = new List<DiffLogColumnInfo>()
            };
            
            if (info.TableName != "Users")
                throw new Exception("Test_DiffLogTableInfo failed");
            Console.WriteLine("  ✓ Test_DiffLogTableInfo passed");
        }
        
        private static void Test_DiffLogColumnInfo()
        {
            var info = new DiffLogColumnInfo
            {
                ColumnName = "Id",
                ColumnDescription = "Primary Key",
                Value = 1,
                IsPrimaryKey = true
            };
            
            if (info.ColumnName != "Id" || !info.IsPrimaryKey)
                throw new Exception("Test_DiffLogColumnInfo failed");
            Console.WriteLine("  ✓ Test_DiffLogColumnInfo passed");
        }
        
        private static void Test_DiffLogModel_WithData()
        {
            var model = new DiffLogModel
            {
                BeforeData = new List<DiffLogTableInfo>
                {
                    new DiffLogTableInfo { TableName = "Users" }
                },
                AfterData = new List<DiffLogTableInfo>
                {
                    new DiffLogTableInfo { TableName = "Users" }
                },
                BusinessData = new { UserId = 1 }
            };
            
            if (model.BeforeData.Count != 1 || model.AfterData.Count != 1)
                throw new Exception("Test_DiffLogModel_WithData failed");
            Console.WriteLine("  ✓ Test_DiffLogModel_WithData passed");
        }
        
        #endregion

        #region ConnectionConfig Tests
        
        private static void Test_ConnectionConfig_BasicProperties()
        {
            var config = new ConnectionConfig
            {
                ConfigId = "main",
                ConnectionString = "Server=localhost;Database=test"
            };
            
            if (config.ConfigId.ToString() != "main")
                throw new Exception("Test_ConnectionConfig_BasicProperties failed");
            Console.WriteLine("  ✓ Test_ConnectionConfig_BasicProperties passed");
        }
        
        private static void Test_ConnectionConfig_DbType()
        {
            var config = new ConnectionConfig { DbType = DbType.SqlServer };
            
            if (config.DbType != DbType.SqlServer)
                throw new Exception("Test_ConnectionConfig_DbType failed");
            Console.WriteLine("  ✓ Test_ConnectionConfig_DbType passed");
        }
        
        private static void Test_ConnectionConfig_AutoClose()
        {
            var config = new ConnectionConfig { IsAutoCloseConnection = true };
            
            if (!config.IsAutoCloseConnection)
                throw new Exception("Test_ConnectionConfig_AutoClose failed");
            Console.WriteLine("  ✓ Test_ConnectionConfig_AutoClose passed");
        }
        
        private static void Test_ConnectionConfig_InitKeyType()
        {
            var config = new ConnectionConfig { InitKeyType = InitKeyType.SystemTable };
            
            if (config.InitKeyType != InitKeyType.SystemTable)
                throw new Exception("Test_ConnectionConfig_InitKeyType failed");
            Console.WriteLine("  ✓ Test_ConnectionConfig_InitKeyType passed");
        }
        
        private static void Test_ConnectionConfig_SlaveConfigs()
        {
            var config = new ConnectionConfig
            {
                SlaveConnectionConfigs = new List<SlaveConnectionConfig>
                {
                    new SlaveConnectionConfig { ConnectionString = "slave1" },
                    new SlaveConnectionConfig { ConnectionString = "slave2" }
                }
            };
            
            if (config.SlaveConnectionConfigs.Count != 2)
                throw new Exception("Test_ConnectionConfig_SlaveConfigs failed");
            Console.WriteLine("  ✓ Test_ConnectionConfig_SlaveConfigs passed");
        }
        
        #endregion

        #region AopEvents Tests
        
        private static void Test_AopEvents_OnError()
        {
            bool errorCalled = false;
            var events = new AopEvents
            {
                OnError = (ex) => { errorCalled = true; }
            };
            
            events.OnError?.Invoke(new SqlSugarException("test"));
            
            if (!errorCalled)
                throw new Exception("Test_AopEvents_OnError failed");
            Console.WriteLine("  ✓ Test_AopEvents_OnError passed");
        }
        
        private static void Test_AopEvents_OnLogExecuting()
        {
            string capturedSql = null;
            var events = new AopEvents
            {
                OnLogExecuting = (sql, pars) => { capturedSql = sql; }
            };
            
            events.OnLogExecuting?.Invoke("SELECT * FROM Users", null);
            
            if (capturedSql != "SELECT * FROM Users")
                throw new Exception("Test_AopEvents_OnLogExecuting failed");
            Console.WriteLine("  ✓ Test_AopEvents_OnLogExecuting passed");
        }
        
        private static void Test_AopEvents_DataExecuting()
        {
            bool dataCalled = false;
            var events = new AopEvents
            {
                DataExecuting = (obj, model) => { dataCalled = true; }
            };
            
            events.DataExecuting?.Invoke(new object(), null);
            
            if (!dataCalled)
                throw new Exception("Test_AopEvents_DataExecuting failed");
            Console.WriteLine("  ✓ Test_AopEvents_DataExecuting passed");
        }
        
        #endregion

        #region SlaveConnectionConfig Tests
        
        private static void Test_SlaveConnectionConfig_Basic()
        {
            var config = new SlaveConnectionConfig
            {
                ConnectionString = "Server=slave;Database=test"
            };
            
            if (config.ConnectionString == null)
                throw new Exception("Test_SlaveConnectionConfig_Basic failed");
            Console.WriteLine("  ✓ Test_SlaveConnectionConfig_Basic passed");
        }
        
        private static void Test_SlaveConnectionConfig_HitRate()
        {
            var config = new SlaveConnectionConfig
            {
                HitRate = 10
            };
            
            if (config.HitRate != 10)
                throw new Exception("Test_SlaveConnectionConfig_HitRate failed");
            Console.WriteLine("  ✓ Test_SlaveConnectionConfig_HitRate passed");
        }
        
        #endregion
    }
}
