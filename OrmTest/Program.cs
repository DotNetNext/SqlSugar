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

namespace OrmTest
{

    class Program
    {
        static void Main(string[] args)
        {
            //Expression To Sql Unit Test
            int eachCount = 1;
            new OrmTest.ExpressionTest.Select(eachCount).Init();
            new OrmTest.ExpressionTest.Field(eachCount).Init();
            new OrmTest.ExpressionTest.Where(eachCount).Init();
        }
    }
}
