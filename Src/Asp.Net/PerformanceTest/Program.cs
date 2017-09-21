using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerformanceTest.TestItems;

namespace PerformanceTest
{
    class Program
    {
     
        /// <summary>
        /// SqlSugar与Dapper的性能比较
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //new TestGetAll().Init();
            new TestGetById().Init();
            Console.ReadKey();
        }
    }
}
