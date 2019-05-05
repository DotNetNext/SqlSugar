using OrmTest.PerformanceTesting;
using OrmTest.UnitTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class OldTestMain
    {
        public static void Init()
        {
            /***BUG repair test***/
            new BugTest.Bug1().Init();
            new BugTest.Bug2().Init();

            /***Unit Test***/
            new SqlRemark(2).Init();
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
            new SqlSugarPerformance(10).Select();

            /***Demo***/
            Demo.Insert.Init();
            Demo.Query.Init();
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
            Demo.Mapper.Init();
            Demo.ExtEntity.Init();
            Demo.VersionValidation.Init();
            Demo.Delete.Init();
            Demo.InsertOrUpdate.Init();
            Demo.Debugger.Init();
            Demo.Queue.Init();
        }
    }
}
