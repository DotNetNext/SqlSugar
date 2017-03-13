using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SqlSugar;
using OrmTest.Models;
using System.Data.SqlClient;
using OrmTest.UnitTest;
namespace OrmTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Unit Test
            int eachCount = 10;
            //new Field(eachCount).Init();
            //new Where(eachCount).Init();
            //new Method(eachCount).Init();
            //new JoinQuery(eachCount).Init();
            //new SingleQuery(eachCount).Init();
            new SelectQuery(eachCount).Init();
        }
    }
}
