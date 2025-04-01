using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using SqlSugar;
using static System.Net.Mime.MediaTypeNames;
namespace OrmTest
{

    public class Unitdfafassfa
    {

        public  static void Init()



        {

            var db = NewUnitTest.Db;





            db.Aop.OnLogExecuting = (x, y) =>

            {

                Console.WriteLine(UtilMethods.GetNativeSql(x, y));

            };

            //建表 

            db.CodeFirst.InitTables<dc_test_book>();

            db.CodeFirst.InitTables<dc_test_order>();

            db.CodeFirst.InitTables<dc_test_rbo>();

            //清空表

            db.DbMaintenance.TruncateTable<dc_test_book>();

            db.DbMaintenance.TruncateTable<dc_test_order>();

            db.DbMaintenance.TruncateTable<dc_test_rbo>();

            var insertdata1 = new List<dc_test_book>() { new dc_test_book() { BookId = 1, Monery = 100, UserType = "VIP贵宾", BookName = "三国演义" }, new dc_test_book() { BookId = 1, Monery = 200, UserType = "普通用户", BookName = "三国演义" } };

            var insertdata2 = new List<dc_test_order>() { new dc_test_order() { BookId = 1, OrderNumber = "A早1122", Rbo = "小明" } };

            var insertdata3 = new List<dc_test_rbo>() { new dc_test_rbo() { Rbo = "小明", Type = "VIP贵宾" } };

            //插入测试数据



            var result = db.Insertable(insertdata1).ExecuteCommand();//用例代码

            result = db.Insertable(insertdata2).ExecuteCommand();//用例代码

            result = db.Insertable(insertdata3).ExecuteCommand();//用例代码

            var data = db.Queryable<dc_test_order>().InnerJoin<dc_test_rbo>((a, b) => a.Rbo == b.Rbo).Select((a, b) => new

            {

                OrderNumber = a.OrderNumber,

                OrderDetails = SqlFunc.Subqueryable<dc_test_book>().Where(c => c.BookId == a.BookId && c.UserType == b.Type)

                .ToList(c => new OrderDetails()
                {

                    BookName = c.BookName,

                    Monery = c.Monery,

                    OrderNumber = a.OrderNumber,

                    RboType = b.Type,

                    Rbo = b.Rbo

                })

            }).ToList();

            Console.WriteLine($"订单详情有:{data.First().OrderDetails.Count()}条");



            Console.WriteLine("用例跑完"); 



        }



        //建类



        public class dc_test_book

        {

            [SugarColumn(ColumnName = "book_id")]

            public int BookId { get; set; }



            [SugarColumn(ColumnDataType = "float", ColumnName = "monery")]

            public float Monery { get; set; }

            [SugarColumn(ColumnDataType = "nvarchar(50)", ColumnName = "user_type")]

            public string UserType { get; set; }

            [SugarColumn(ColumnDataType = "nvarchar(50)", ColumnName = "book_name")]

            public string BookName { get; set; }

        }

        public class dc_test_order

        {

            [SugarColumn(ColumnDataType = "nvarchar(50)", ColumnName = "order_number")]

            public string OrderNumber { get; set; }

            [SugarColumn(ColumnDataType = "nvarchar(50)", ColumnName = "rbo")]

            public string Rbo { get; set; }

            [SugarColumn(ColumnName = "book_id")]

            public int BookId { get; set; }

        }

        public class dc_test_rbo

        {

            [SugarColumn(ColumnDataType = "nvarchar(50)", ColumnName = "rbo")]

            public string Rbo { get; set; }



            [SugarColumn(ColumnDataType = "nvarchar(50)", ColumnName = "type")]

            public string Type { get; set; }

        }

        public class OrderDetails
        {

            public string OrderNumber { get; set; }

            public string Rbo { get; set; }

            public string RboType { get; set; }

            public string BookName { get; set; }

            public float Monery { get; set; }

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

            public readonly static string Connection = "";



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

                    DbType = SqlSugar.DbType.SqlServer,

                    ConnectionString = Connection,

                    LanguageType = LanguageType.Default//Set language



                },

                it =>
                {

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



}