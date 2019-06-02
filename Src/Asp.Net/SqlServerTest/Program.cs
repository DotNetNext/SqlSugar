using OrmTest.PerformanceTesting;
using OrmTest.UnitTest;
using System;

namespace OrmTest
{
    class Program
    {
        /// <summary>
        /// Set up config.cs file and start directly F5
        /// 设置Config.cs文件直接F5启动例子
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
 
            //Demo
            Demo0_SqlSugarClient.Init();
            Demo1_Queryable.Init();
            Demo2_Updateable.Init();
            Democ_GobalFilter.Init();
            DemoD_DbFirst.Init();
            DemoE_CodeFirst.Init();
            Demo5_SqlQueryable.Init();
            Demo6_Queue.Init();

            //Unit test
            //NewUnitTest.Init();

            //Rest Data
            NewUnitTest.RestData();

            Console.WriteLine("all successfully.");
            Console.ReadKey();
        }

 
    }
}
