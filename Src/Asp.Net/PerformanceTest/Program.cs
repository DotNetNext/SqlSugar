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
            var type = DemoType.GetAll;
            var ormType = OrmType.Dapper;
            switch (type)
            {
                case DemoType.GetAll:
                    new TestGetAll().Init(ormType);
                    break;
                case DemoType.GetById:
                    new TestGetById().Init(ormType);
                    break;
                case DemoType.GetSql:
                    new TestGetSql().Init(ormType);
                    break;
                default:
                    break;
            }
            Console.ReadKey();
        }
        enum DemoType
        {
            GetAll,
            GetById,
            GetSql
        }
    }
}
