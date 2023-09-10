using SqlSugar;
using System;
using System.Collections.Generic;

namespace Test
{
    partial class Program
    {
        static void Main(string[] args)
        {
            JsonClient jsonToSqlClient = new JsonClient();
            jsonToSqlClient.Context = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                ConnectionString = "server=.;uid=sa;pwd=sasa;database=SQLSUGAR4XTEST"
            });
            //jsonToSqlClient.Context = new SqlSugarClient(new ConnectionConfig()
            //{
            //    DbType = DbType.MySql,
            //    IsAutoCloseConnection = true,
            //    ConnectionString = "server=localhost;Database=SqlSugar4xTest;Uid=root;Pwd=haosql"
            //}); ;
            //TestHelper.InitDatabase(jsonToSqlClient);

            jsonToSqlClient.Context.Aop.OnLogExecuted = (sql, p) =>
            {
                Console.WriteLine(UtilMethods.GetNativeSql(sql,p));
            };

            Insetable01(jsonToSqlClient);

            Description(jsonToSqlClient);
            FuncText(jsonToSqlClient);
            OrderByTest(jsonToSqlClient);
            GroupByTest(jsonToSqlClient);
            SelectTest(jsonToSqlClient);
            JoinTest(jsonToSqlClient);
            WhereTest(jsonToSqlClient);
            PageTest(jsonToSqlClient);
            PageTest2(jsonToSqlClient);
            PageTest3(jsonToSqlClient);

            Updateable01(jsonToSqlClient);
            Deleteable01(jsonToSqlClient);

            DynamicLinq(jsonToSqlClient.Context);
            Console.WriteLine();
        }
    }
}
