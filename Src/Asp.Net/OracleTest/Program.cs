﻿using System;
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

            /***Unit Test***/
            OrmTest.Demo.CodeFirst.Init();
            OrmTest.Demo.Insert.Init();
            new SelectQuery(1).Init();
            new Select(1).Init();
            new Field(1).Init();

            /***Demo***/
            OrmTest.Demo.Query.Init();
            //OrmTest.Demo.Delete.Init();
            OrmTest.Demo.Update.Init();
            OrmTest.Demo.DbFirst.Init();
            OrmTest.Demo.JoinSql.Init();
            OrmTest.Demo.Filter.Init();
            OrmTest.Demo.ComplexModel.Init();
        }
    }
}
