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
        /// 注意注意注意注意注意：分开测试比较公平,并且请在Realse模式下启动程序（SqlSugar直接引用的是项目）
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //new TestGetAll().Init(OrmType.Dapper);
            //new TestGetById().Init(OrmType.EF);
            new TestSql().Init(OrmType.SqlSugar);
            Console.ReadKey();
        }
    }
}
