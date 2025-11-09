using SqlSugar;
using System;

namespace PerformanceBenchmarks
{
    /// <summary>
    /// Configuration helper for benchmark tests
    /// 基准测试的配置辅助类
    /// </summary>
    public class BenchmarkConfig
    {
        /// <summary>
        /// Database connection string for SQL Server
        /// SQL Server 数据库连接字符串
        /// </summary>
        public const string SqlServerConnection = "server=.;uid=sa;pwd=sasa;database=SqlSugarBenchmark;Encrypt=True;TrustServerCertificate=True";

        /// <summary>
        /// Database connection string for MySQL
        /// MySQL 数据库连接字符串
        /// </summary>
        public const string MySqlConnection = "server=localhost;Database=SqlSugarBenchmark;Uid=root;Pwd=123456";

        /// <summary>
        /// Number of iterations for bulk operations
        /// 批量操作的迭代次数
        /// </summary>
        public const int BulkIterations = 1000;

        /// <summary>
        /// Number of iterations for single operations
        /// 单个操作的迭代次数
        /// </summary>
        public const int SingleIterations = 100;

        /// <summary>
        /// Get a new SqlSugarClient instance for SQL Server
        /// 获取 SQL Server 的新 SqlSugarClient 实例
        /// </summary>
        /// <param name="enableLog">Enable SQL logging / 启用 SQL 日志</param>
        /// <returns>SqlSugarClient instance</returns>
        public static SqlSugarClient GetSqlServerDb(bool enableLog = false)
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                IsAutoCloseConnection = true,
                DbType = DbType.SqlServer,
                ConnectionString = SqlServerConnection,
                LanguageType = LanguageType.Default
            });

            if (enableLog)
            {
                db.Aop.OnLogExecuting = (sql, para) =>
                {
                    Console.WriteLine(UtilMethods.GetNativeSql(sql, para));
                };
            }

            return db;
        }

        /// <summary>
        /// Get a new SqlSugarClient instance for MySQL
        /// 获取 MySQL 的新 SqlSugarClient 实例
        /// </summary>
        /// <param name="enableLog">Enable SQL logging / 启用 SQL 日志</param>
        /// <returns>SqlSugarClient instance</returns>
        public static SqlSugarClient GetMySqlDb(bool enableLog = false)
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                IsAutoCloseConnection = true,
                DbType = DbType.MySql,
                ConnectionString = MySqlConnection,
                LanguageType = LanguageType.Default
            });

            if (enableLog)
            {
                db.Aop.OnLogExecuting = (sql, para) =>
                {
                    Console.WriteLine(UtilMethods.GetNativeSql(sql, para));
                };
            }

            return db;
        }

        /// <summary>
        /// Initialize database and create tables
        /// 初始化数据库并创建表
        /// </summary>
        public static void InitializeDatabase()
        {
            var db = GetSqlServerDb();
            
            // Create database if not exists
            // 如果数据库不存在则创建
            db.DbMaintenance.CreateDatabase();

            // Initialize all benchmark tables
            // 初始化所有基准测试表
            db.CodeFirst.InitTables<Entities.BenchmarkOrder>();
            db.CodeFirst.InitTables<Entities.BenchmarkCustomer>();
            db.CodeFirst.InitTables<Entities.BenchmarkProduct>();
            db.CodeFirst.InitTables<Entities.BenchmarkOrderItem>();
        }

        /// <summary>
        /// Clean up database
        /// 清理数据库
        /// </summary>
        public static void CleanupDatabase()
        {
            var db = GetSqlServerDb();
            
            // Truncate all tables
            // 清空所有表
            db.DbMaintenance.TruncateTable<Entities.BenchmarkOrder>();
            db.DbMaintenance.TruncateTable<Entities.BenchmarkCustomer>();
            db.DbMaintenance.TruncateTable<Entities.BenchmarkProduct>();
            db.DbMaintenance.TruncateTable<Entities.BenchmarkOrderItem>();
        }
    }
}
