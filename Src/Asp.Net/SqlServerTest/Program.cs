using OrmTest.PerformanceTesting;
using OrmTest.UnitTest;
using System;

namespace OrmTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //OldTestMain.Init();

            Demo1_SqlSugarClient.Init();
            Democ_GobalFilter.Init();
            DemoD_DbFirst.Init();

            Console.WriteLine("all successfully.");
            Console.ReadKey();
        }

 
    }
}
