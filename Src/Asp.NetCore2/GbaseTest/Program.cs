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
            Demo4_Deleteable.Init();
            Demo2_Updateable.Init();
            Demo7_Ado.Init();
            DemoE_CodeFirst.Init();
            DemoD_DbFirst.Init();
            Console.WriteLine("Hello World!");
        }
    }
}
