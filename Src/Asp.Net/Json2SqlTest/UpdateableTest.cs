using SqlSugar;
using System;
using System.Collections.Generic;

namespace Test
{
    partial class Program
    {
 
        private static void Updateable01(JsonClient jsonToSqlClient)
        {
            jsonToSqlClient.Context.Queryable<object>().AS("order").Max<int>("id");
            Demo11(jsonToSqlClient);
            Demo22(jsonToSqlClient);
            Demo33(jsonToSqlClient);
            Demo44(jsonToSqlClient);
        }

        private static void Demo11(JsonClient jsonToSqlClient)
        {
            var json = @"
                {
	                ""Table"":""order"",
                      Columns: { id:""{int}:1"" ,name:""{string}:1"" },
                      WhereColumns:[""id""]
                }
                ";
            var x1 = jsonToSqlClient.Updateable(json).ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
        private static void Demo22(JsonClient jsonToSqlClient)
        {
            var json = @"
                {
	                ""Table"":""order"",
                      Columns:[ {id:2,name:""{string}:2"",price:""{decimal}:2""}  , {id:1,name:""{string}:1"",price:""{decimal}:1""}  ],
                      WhereColumns:[""id""]                
                }
                ";
            var x1 = jsonToSqlClient.Updateable(json).ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }

        private static void Demo33(JsonClient jsonToSqlClient)
        {
            var json = @"
                {
	                ""Table"":""order"",
                      Columns: {name:""{string}:2"",price:""{decimal}:2""}  ,
                      Where:[""id"",""="",""{int}:11""]                
                }
                ";
            var x1 = jsonToSqlClient.Updateable(json).ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
        private static void Demo44(JsonClient jsonToSqlClient)
        {
            var json = @"
                {
	                ""Table"":""order"",
                      Columns: {name:""{string}:2"",price:""{decimal}:2""}  ,
                      Where:[""id"",""="",""{int}:11""]                
                }
                ";
            var x1 = jsonToSqlClient.Updateable(json).ToSqlList();
            // var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
    }
}
