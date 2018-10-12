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
            //    /***Unit Test***/
            new Select(1).Init();
            new Field(1).Init();
            new Where(1).Init();
            new Method(1).Init();
            new JoinQuery(1).Init();
            new SingleQuery(1).Init();
            new SelectQuery(1).Init();
            new AutoClose(1).Init();
            new Insert(1).Init();
            new Delete(1).Init();
            new Update(1).Init();
            new Mapping(1).Init();
            new DataTest(1).Init();
            new EnumTest(1).Init();
            /***Performance Test***/
            new SqlSugarPerformance(100).Select();

            /***Demo***/
            Demo.Query.Init();
            Demo.Insert.Init();
            Demo.Delete.Init();
            Demo.Update.Init();
            Demo.DbFirst.Init();
            Demo.JoinSql.Init();
            Demo.Filter.Init();
            Demo.ComplexModel.Init();
            Demo.CodeFirst.Init();
            Demo.Aop.Init();
            Demo.MasterSlave.Init();
            Demo.SharedConnection.Init();
            Demo.ExtSqlFun.Init();
            Demo.QueryableView.Init();
            Demo.AttributeDemo.Init();
        }
    }
}
