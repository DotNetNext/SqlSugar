using KdbndpTest.OracleDemo;
using KdbndpTest.SqlServerDemo;
using System;

namespace OrmTest
{
    class Program
    {
        static void Main(string[] args)
        {

            //Oracle模式DEMO
            OracleDemo.Init();

            //SqlServer模式DEMO
            SqlServer.Init();

            //Unit test
            //NewUnitTest.Init();

            //Rest Data
            NewUnitTest.RestData();

            Console.WriteLine("all successfully.");
            Console.ReadKey();
        }

 
    }
}
