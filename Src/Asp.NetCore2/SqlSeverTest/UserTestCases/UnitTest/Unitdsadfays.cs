using SqlSeverTest.UserTestCases;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OrmTest._6_NavQuery;

namespace OrmTest
{
    public class Unitdsadfays
    {
        public static void Init()
        {
            var client = new SqlSugarClient(
              new List<ConnectionConfig>() {
                    new ConnectionConfig {
                        ConfigId   = 1
                      , DbLinkName = "dbo"
                      , DbType                = DbType.SqlServer
                      , IsAutoCloseConnection = true,
                        ConnectionString=Config.ConnectionString
                    }
                  , new ConnectionConfig {
                        ConfigId   = 2
                      , DbLinkName = "dbo"
                      , DbType                = DbType.SqlServer
                      , IsAutoCloseConnection = true,
                        ConnectionString=Config.ConnectionString
                    }});
            client.CodeFirst.InitTables<Tb1, Tb2>();

            client.GetConnection("1")
               .Aop.OnLogExecuting=
            client.GetConnection("2")
                .Aop.OnLogExecuting = (x, y) => Console.WriteLine(x);

            var sql1 =   
                client
                   .QueryableWithAttr<Tb1>()
               .Where(
            a => a.Code
                      == SqlFunc.Subqueryable<Tb2>().AsWithAttr().Where(b => b.Code > 0).Where(b => b.Name > 0).GroupBy(b => b.Code).Select(b => b.Code)
                )
               .ToList() ;

            Console.WriteLine(sql1);
            var sql2 =
                client
                .QueryableWithAttr<Tb1>()
                   .Where(
                a => a.Code == SqlFunc.Subqueryable<Tb2>().AsWithAttr().Where(b => (b.Code > 0) && (b.Name > 0)).GroupBy(b => b.Code).Select(b => b.Code)
                    )
                  .ToList();

            var sql3=
             client
             .QueryableWithAttr<Tb1>()
                .Where(
             a => a.Code == SqlFunc.Subqueryable<Tb2>().AsWithAttr().Where(b => b.Code==a.Code).GroupBy(b => b.Code).Select(b => b.Code)
                 )
       .ToList();

        }
    }


    [Tenant(1)]
    [SugarTable("UnitTTT1aaa")]
    public class Tb1
    {
        public long Code { get; set; }
        public long Name { get; set; }
    }
    [Tenant(2)]
    [SugarTable("UnitTTT1aaa222")]
    public class Tb2
    {
        public long Code { get; set; }

        public long Name { get; set; }
    }
}

