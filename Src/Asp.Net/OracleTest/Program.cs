using OrmTest.PerformanceTesting;
using OrmTest.UnitTest;
using System;

namespace OrmTest
{
    class Program
    {
        static void Main(string[] args)
        {
            OldTestMain.Init();
 
            //Unit test
            NewUnitTest.Init();

            Console.WriteLine("all successfully.");
            Console.ReadKey();
        }

 
    }
}
