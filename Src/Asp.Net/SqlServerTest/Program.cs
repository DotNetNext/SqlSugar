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
using OrmTest.PerformanceTesting;

namespace OrmTest
{
    class Program
    {
        static void Main(string[] args)
        {
           
            Demo.VersionValidation.Init();
        }
    }
}
