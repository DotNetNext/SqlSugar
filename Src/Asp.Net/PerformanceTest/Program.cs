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
        /// 分开测试比较公平
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //new TestGetAll().Init();
            //new TestGetById().Init();
            new TestSql().Init(OrmType.SqlSugar);
            Console.ReadKey();
        }
    }
}
