using OrmTest;
using System;

namespace GbaseTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Demo0_SqlSugarClient.Init();
            Demo1_Queryable.Init();
            Demo3_Insertable.Init();
            Console.WriteLine("Hello World!");
        }
    }
}
