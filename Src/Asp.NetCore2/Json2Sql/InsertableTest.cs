using SqlSugar;
using System;
using System.Collections.Generic;

namespace Test
{
    partial class Program
    {
 
        private static void Insetable01(JsonClient jsonToSqlClient)
        {
            jsonToSqlClient.Context.Queryable<object>().AS("order").Max<int>("id");
            Demo1(jsonToSqlClient);
            Demo2(jsonToSqlClient);
            Demo3(jsonToSqlClient);
        }

        private static void Demo1(JsonClient jsonToSqlClient)
        {
            var json = @"
                {
	                ""Table"":""order"",
                      Columns:{name:""{string}:1"",price:""{decimal}:1""} 
                }
                ";
            var x1 = jsonToSqlClient.Insertable(json).ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
        private static void Demo2(JsonClient jsonToSqlClient)
        {
            var json = @"
                {
	                ""Table"":""order"",
                      Columns:[ {name:""{string}:2"",price:""{decimal}:2""} , {name:""{string}:1"",price:""{decimal}:1""}  ]
                }
                ";
            var x1 = jsonToSqlClient.Insertable(json).ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
        private static void Demo3(JsonClient jsonToSqlClient)
        {
            var json = @"
                {
	                ""Table"":""order"",
                      Identity:""id"",
                      Columns:  {name:""{string}:2"",price:""{decimal}:2""}  
                }
                ";
            var x1 = jsonToSqlClient.Insertable(json).ToResult();
           
        }
    }
}
