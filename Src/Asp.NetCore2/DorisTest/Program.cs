using DorisTest;
using SqlSugar;
using System;

namespace OrmTest
{

    public class Program
    {
        static void Main(string[] args)
        { 
            var db = DbHelper.GetNewDb();
          var xx=  db.Ado.GetDataTable(@"SELECT
                                    0 as TableId,
                                    TABLE_NAME as TableName, 
                                    column_name AS DbColumnName,
                                    CASE WHEN  COLUMN_TYPE NOT LIKE '(' THEN COLUMN_TYPE ELSE  left(COLUMN_TYPE,LOCATE('(',COLUMN_TYPE)-1) END   AS DataType,
                                    CAST(SUBSTRING(COLUMN_TYPE,LOCATE('(',COLUMN_TYPE)+1,LOCATE(')',COLUMN_TYPE)-LOCATE('(',COLUMN_TYPE)-1) AS  decimal(18,0) ) AS Length,
                                    column_default  AS  `DefaultValue`,
                                    column_comment  AS  `ColumnDescription`,
                                    CASE WHEN COLUMN_KEY = 'PRI'
                                    THEN true ELSE false END AS `IsPrimaryKey`,
                                    CASE WHEN EXTRA='auto_increment' THEN true ELSE false END as IsIdentity,
                                    CASE WHEN is_nullable = 'YES'
                                    THEN true ELSE false END AS `IsNullable`,
                                    numeric_scale as Scale,
                                    numeric_scale as DecimalDigits,
                                    LOCATE(  'unsigned',COLUMN_type   ) >0  as IsUnsigned
                                    FROM
                                    Information_schema.columns where TABLE_NAME='LogEntity1' and  TABLE_SCHEMA=(select database()) ORDER BY ordinal_position");
            db.CodeFirst.InitTables<LogEntity>();
            db.CodeFirst.InitTables<Student11>();

            db.Insertable(new Student11() { Id = 1, Age = 1, Name = "a" })
                .ExecuteCommand();
            var list=db.Queryable<Student11>().ToList();
            //var rows = db.Updateable(list.First()).ExecuteCommand();
            db.Deleteable(list).ExecuteCommand();
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
        public readonly static string Connection = "server=39.170.74.19;port=9030 ;Database=test; Uid=root;Pwd=lq!1q!;Pooling=false;";

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
                it.Aop.OnError = ex =>
                {
                };
            });
            return db;
        }
    }
}