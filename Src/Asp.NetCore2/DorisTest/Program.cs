using SqlSugar;
using System;

namespace OrmTest
{

    public class Program
    {
        static void Main(string[] args)
        {

            var db = DbHelper.GetNewDb();
            db.BeginTran();

             var XX=  db.Ado.SqlQuery<DbTableInfo>("Select TABLE_NAME as Name,TABLE_COMMENT as Description from information_schema.tables\r\n                         where  TABLE_SCHEMA=(select database())  AND TABLE_TYPE='BASE TABLE'");
             db.CommitTran();
             db.BeginTran();

            var XX2 = db.Ado.SqlQuery<DbTableInfo>("Select TABLE_NAME as Name,TABLE_COMMENT as Description from information_schema.tables\r\n                         where  TABLE_SCHEMA=(select database())  AND TABLE_TYPE='BASE TABLE'");
            db.CommitTran();
            db.CodeFirst.InitTables<Student11>();
        }
    } 
    public class Student11
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true )]
        public int Id { get; set; } // 学生ID (Student ID)
        public string Name { get; set; } // 学生姓名 (Student Name)
        public int Age { get; set; } // 学生年龄 (Student Age)
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
        public readonly static string Connection = "data source=139.170.74.9;port=9030;database=test;user id=root;password=1q!1q!;pooling=true;charset=utf8;Pooling=false";

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
                DbType = DbType.Doris,
                ConnectionString = Connection,
                LanguageType = LanguageType.Default//Set language

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