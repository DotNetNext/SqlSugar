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

            //Demo
            Demo1_SqlSugarClient.Init();
            Demo1_Queryable.Init();
            Democ_GobalFilter.Init();
            DemoD_DbFirst.Init();
            DemoE_CodeFirst.Init();

            //Unit test
            NewUnitTest.Init();

            Console.WriteLine("all successfully.");
            Console.ReadKey();
        }

 
    }
}
