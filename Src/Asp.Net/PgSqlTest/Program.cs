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
            Demo.CodeFirst.Init();
            Demo.DbFirst.Init();
            Demo.Aop.Init();
            Demo.Query.Init();
            Demo.Insert.Init();
            Demo.Delete.Init();
            Demo.Update.Init();
            Demo.MasterSlave.Init();
            Demo.SharedConnection.Init();
            Demo.ExtSqlFun.Init();
            //Demo.QueryableView.Init();
            Demo.AttributeDemo.Init();
            Demo.Mapper.Init();
            Demo.ExtEntity.Init();
            Demo.Queue.Init();

        }
    }
}
