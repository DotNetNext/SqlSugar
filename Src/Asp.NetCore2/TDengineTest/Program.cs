 
using System;
using TDengineTest;

namespace OrmTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //需要安装：客户端SDK
            //https://docs.taosdata.com/connector/#%E5%AE%89%E8%A3%85%E5%AE%A2%E6%88%B7%E7%AB%AF%E9%A9%B1%E5%8A%A8
           
            //orm用例
            ORMTest.Init();

            //原生ado用例
            AdoDemo.Init();
         
            
            Console.WriteLine("all successfully.");
            Console.ReadKey();
        }

 
    }
}
