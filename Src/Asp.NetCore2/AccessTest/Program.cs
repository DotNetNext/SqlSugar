using System;

namespace OrmTest
{
    class Program
    {
        /// <summary>
        /// Set up config.cs file and start directly F5
        /// 设置Config.cs文件直接F5启动例子
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            //注意: Cpu X86  ，生成的目录要在x86里面
            Demo0_SqlSugarClient.Init();
            Demo1_Queryable.Init();
            Demo2_Updateable.Init();
            Demo3_Insertable.Init();
            Demo4_Deleteable.Init();
            Demo5_SqlQueryable.Init();
            Demo7_Ado.Init();
            DemoD_DbFirst.Init();
            Console.WriteLine("all successfully.");
            Console.ReadKey();
        }

 
    }
}
