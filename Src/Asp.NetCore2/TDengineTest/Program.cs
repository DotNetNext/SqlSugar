 
using System;
using TDengineTest;

namespace OrmTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //orm用例
            ORMTest.Init();
            //原生ado用例
            AdoDemo.Init();
         
            
            Console.WriteLine("all successfully.");
            Console.ReadKey();
        }

 
    }
}
