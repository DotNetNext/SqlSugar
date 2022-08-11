using SqlSugar;
using System;
using System.Collections.Generic;

namespace Test
{
    partial class Program
    {
 
        private static void Deleteable01(JsonClient jsonToSqlClient)
        {
            jsonToSqlClient.Context.Queryable<object>().AS("order").Max<int>("id");
            Demo111(jsonToSqlClient);
            Demo222(jsonToSqlClient);
            Demo333(jsonToSqlClient);
        }

        private static void Demo111(JsonClient jsonToSqlClient)
        {
            var json = @"
                {
	                ""Table"":""order"",
                      Where:[ ""id"","" = "",""{int}:1"" ]
                }
                ";
            var x1 = jsonToSqlClient.Deleteable(json).ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
        private static void Demo222(JsonClient jsonToSqlClient)
        {
            var json = @"
                {
	                ""Table"":""order"",
                      Where:[ ""id"","" = "",""{int}:1"" ]
                }
                ";
            var x1 = jsonToSqlClient.Deleteable(json).ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
        private static void Demo333(JsonClient jsonToSqlClient)
        {
            var json = @"
                {
	                ""Table"":""order"" 
                }
                ";
            var x1 = jsonToSqlClient.Deleteable(json).ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
    }
}
