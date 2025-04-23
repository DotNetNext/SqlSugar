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
            //说明：
            //DuckDB和Sqlite差不多不需要安装直接调试和运行

            //注册自定义信息扔在程序启动时
            InstanceFactory.CustomDbName = "DuckDB";
            InstanceFactory.CustomDllName = "SqlSugar.DuckDBCore";
            InstanceFactory.CustomNamespace = "SqlSugar.DuckDB";
            InstanceFactory.CustomAssemblies = new System.Reflection.Assembly[] {
            typeof(SqlSugar.DuckDB.DuckDBProvider).Assembly };

            //执行Demo
            Demo.Init(); 
            Console.WriteLine("all successfully.");
            Console.ReadKey();
        }

 
    }
}
