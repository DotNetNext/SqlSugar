using SqlSugar;
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
            //Set Custom  Db
            InstanceFactory.CustomDbName = "MySql";
            InstanceFactory.CustomDllName = "SqlSugar.MySqlConnectorCore";
            InstanceFactory.CustomNamespace = "SqlSugar.MySqlConnector";

            Demo0_SqlSugarClient.Init();
            Demo1_Queryable.Init();
            Demo2_Updateable.Init();
            Demo3_Insertable.Init();
            Demo4_Deleteable.Init();
            DemoD_DbFirst.Init();
            Console.WriteLine("all successfully.");
            Console.ReadKey();
        }

 
    }
}
