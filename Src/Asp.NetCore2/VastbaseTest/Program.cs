using SqlSugar;
using System;

namespace OrmTest
{
    
    public class Program
    {
        static void Main(string[] args)
        {
            var db = DbHelper.GetNewDb();
 
            db.CodeFirst.InitTables<DataType1>();
            db.Insertable(new DataType1() { Id = 1,id2=Convert.ToDecimal(1.2),id3="aa" }).ExecuteCommand();
            var list=db.Queryable<DataType1>().ToList();
     
        }
    }

    public class DataType1 
    {
        [SugarColumn(ColumnDataType ="integer")]
        public int Id { get; set; }
        [SugarColumn(ColumnDataType ="money",IsNullable =true)]
        public decimal id2 { get; set; }
        [SugarColumn(ColumnDataType = "varchar2", IsNullable = true)]
        public string id3 { get; set; }
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
        public readonly static string Connection = "PORT=5410;DATABASE=sqlsugar_test;HOST=116.63.182.54;PASSWORD=Test@123456abc;USER ID=test;No Reset On Close=true";

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
                DbType = DbType.Vastbase,
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