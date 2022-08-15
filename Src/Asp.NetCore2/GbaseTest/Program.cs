using System;

namespace GbaseTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
           var odbc= new System.Data.Odbc.OdbcConnection(@"Driver={GBase ODBC 8.3 DRIVER};Server=localhost;port=19088;Database=;Protocol=onsoctcp;Uid=gbasedbt;Pwd=GBase123;");
            odbc.Open();
            Console.WriteLine("Hello World!");
        }
    }
}
