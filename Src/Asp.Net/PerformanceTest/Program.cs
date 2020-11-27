using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerformanceTest.TestItems;
using SqlSugar;

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
            InitData();

            var type = DemoType.GetById;
            var ormType = OrmType.SqlSugar;
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
                case DemoType.Insert:
                    new TestInsert().Init(ormType);
                    break;
                case DemoType.Like:
                    new TestLike().Init(ormType);
                    break;
                default:
                    break;
            }
            Console.ReadKey();
        }

        private static void InitData()
        {
            SqlSugarClient conn = Config.GetSugarConn();
            conn.CurrentConnectionConfig.InitKeyType = InitKeyType.Attribute;
            conn.DbMaintenance.CreateDatabase();//创建库
            conn.CodeFirst.InitTables<Test>();
            List<Test> test = new List<Test>();
            if (conn.Queryable<Test>().Count() < 100000)
            {
                Console.WriteLine("初始化数据");

                for (int i = 0; i < 100000; i++)
                {
                    test.Add(new Test()
                    {
                        F_Bool = true,
                        F_Byte = 0,
                        F_DateTime = DateTime.Now,
                        F_Decimal = 1,
                        F_Double = 11,
                        F_Float = 11,
                        F_Guid = Guid.Empty,
                        F_String = "abc",
                        F_Int16 = 1,
                        F_Int32 = 1,
                        F_Int64 = 1

                    });
                }
            }
            conn.Insertable(test).ExecuteCommand();
        }

        enum DemoType
        {
            GetAll,
            GetById,
            GetSql,
            Insert,
            Like
        }
    }
}
