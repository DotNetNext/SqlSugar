using SqlSugar;
using System;

namespace OrmTest
{
    
    public class Program
    {
        static void Main(string[] args)
        {
            //Each example will automatically create a table and can run independently.
            //每个例子都会自动建表 并且可以独立运行   

            _1_CodeFirst.Init();
            _2_DbFirst.Init();
            _3_EasyQuery.Init();
            _4_JoinQuery.Init();
            _4_Subquery.Init();
            _5_PageQuery.Init();
            _6_NavQuery.Init();
            _7_GroupQuery.Init();
            _8_Insert.Init();
            _9_Update.Init();
            _a1_Delete.Init();
            _a2_Sql.Init();
            _a3_Merge.Init(); 
            _a4_SplitTable.Init();
            _a5_GridSave.Init();
            _a6_SqlPage.Init();
            _a7_JsonType.Init();
            _a8_SelectReturnType.Init();
        }
    }

    /// <summary>
    /// Helper class for database operations
    /// 数据库操作的辅助类
    /// </summary>
    public class DbHelper
    {
        /// <summary>
        /// Database connection string
        /// 数据库连接字符串
        /// </summary>
        public readonly static string Connection = "server=localhost;Database=SqlSugar5xTest;Uid=root;Pwd=123456;AllowLoadLocalInfile=true";

        /// <summary>
        /// Get a new SqlSugarClient instance with specific configurations
        /// 获取具有特定配置的新 SqlSugarClient 实例
        /// </summary>
        /// <returns>SqlSugarClient instance</returns>
        public static SqlSugarClient GetNewDb()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                IsAutoCloseConnection = true,
                DbType = DbType.MySql,
                ConnectionString = Connection,
                LanguageType=LanguageType.Default//Set language

            },
            it => {
                // Logging SQL statements and parameters before execution
                // 在执行前记录 SQL 语句和参数
                it.Aop.OnLogExecuting = (sql, para) =>
                {
                    Console.WriteLine(UtilMethods.GetNativeSql(sql, para));
                };
            });
            return db;
        }
    }
}