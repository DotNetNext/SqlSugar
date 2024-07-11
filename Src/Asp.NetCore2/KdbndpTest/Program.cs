using KdbndpTest.OracleDemo;
using KdbndpTest.SqlServerDemo;
using System;

namespace OrmTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //可以查看安装的模式
            //show database_mode;

            //Oracle模式DEMO 【默认模式：语法同时支持PGSQL】
            OracleDemo.Init();

            //SqlServer模式DEMO
            //SqlServerDemo.Init();

            //MySql模式DEMO
            //MySqlDemo.Init();

            //PostgreSQL模式DEMO
            //PgSqlDemo.Init();

            //Unit test
            //NewUnitTest.Init();


            Console.WriteLine("all successfully.");
            Console.ReadKey();
        }

 
    }
}
