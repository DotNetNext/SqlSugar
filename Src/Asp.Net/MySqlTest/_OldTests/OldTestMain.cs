using OrmTest;
using OrmTest.UnitTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlTest
{
    public  class OldTestMain
    {
        public static void Init() {


            ////    /***Unit Test***/
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

            //    /***Performance Test***/
            //    new SqlSugarPerformance(100).Select();

            /***Demo***/
            OrmTest.Demo.Query.Init();
            OrmTest.Demo.Insert.Init();
            OrmTest.Demo.Delete.Init();
            OrmTest.Demo.Update.Init();
            OrmTest.Demo.DbFirst.Init();
            OrmTest.Demo.JoinSql.Init();
            OrmTest.Demo.Filter.Init();
            OrmTest.Demo.ComplexModel.Init();
            OrmTest.Demo.CodeFirst.Init(); ;
            OrmTest.Demo.MasterSlave.Init(); ;
            OrmTest.Demo.Queue.Init(); ;
            OrmTest.Demo.Encode.Init();
        }

    }
}
