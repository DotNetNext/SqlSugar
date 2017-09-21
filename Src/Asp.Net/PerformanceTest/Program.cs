using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerformanceTest.Items;

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
            WarmUp wu = new WarmUp();//预热处理
 
            new SelectBigData().Init();
       
            Console.ReadKey();
        }
    }
}
