using SqlSugar;
using System;
using System.Collections.Generic;

namespace Test
{
    partial class Program
    {
        #region Queryable
        private static void FuncText(JsonClient jsonToSqlClient)
        {
            jsonToSqlClient.Context.Queryable<object>().AS("order").Max<int>("id");
            var json = @"
                {
	                ""Table"":""order"",
                      Select:[ [{SqlFunc_AggregateMin:[""id""]},""id""], [{SqlFunc_GetDate:[]},""Date""] ]
                }
                ";
            var x1 = jsonToSqlClient.Queryable(json).ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
        private static void WhereTest(JsonClient jsonToSqlClient)
        {
            jsonToSqlClient.Context.Queryable<object>().AS("order").Max<int>("id");
            var json = @"
                {
	                ""Table"":""order"",
                      Where:[  ""name"",""="", {  SqlFunc_ToString:[""{string}:xxx""] } ],
                      Select:[ [{SqlFunc_AggregateMin:[""id""]},""id""], [{SqlFunc_GetDate:[]},""Date""] ]
                }
                ";
            var x1 = jsonToSqlClient.Queryable(json).ToSqlList();
            var json2 = @"
                {
	                ""Table"":""order"",
                      Where: [{ ""FieldName"":""id"",""ConditionalType"":""0"",""FieldValue"":""1""}],
                      Select:[ 
                                 [{SqlFunc_GetDate:[]},""Date""] ,
                                 [""Name"",""Name""],
                                 [""Id""],
                                 ""price""
                              ]
                }
                ";
            var x2 = jsonToSqlClient.Queryable(json2).ToSqlList(); 

            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
            var list2 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x2[0].Sql, x2[0].Parameters);
        }
        private static void JoinTest(JsonClient jsonToSqlClient)
        {
            var onList =
                new ObjectFuncModel()
                {
                    FuncName = "Equals",
                    Parameters = new List<object>{
                       "d.orderid","o.id"
                     }

                };

            var selectItems = new List<SelectModel>() {
                                        new SelectModel()
                                        {
                                            AsName = "id",
                                            FiledName = "o.id"
                                         }
            };
            var x = jsonToSqlClient.Context.Queryable<object>()
                           .AS("order", "o")
                           .AddJoinInfo("orderdetail", "d", onList, JoinType.Left)
                           .Select(selectItems)
                           .ToList();
            var json = @"
{
	""Table"":[ ""order"",""o""],
    ""LeftJoin01"": [""orderdetail"", ""d"", [  ""d.orderid"","">"",""o.id""  ]],
    ""Select"":[""o.id"" ,[""d.itemid"",""newitemid""]]
}
";
            var x1 = jsonToSqlClient.Queryable(json).ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
        private static void JoinTest2(JsonClient jsonToSqlClient)
        {
            var onList =
                new ObjectFuncModel()
                {
                    FuncName = "Format",
                    Parameters = new List<object>{
                       "d.orderid","=","{int}:1"
                     }

                };
            var onList2 =
                 new ObjectFuncModel()
                 {
                     FuncName = "Format",
                     Parameters = new List<object>{
                               "c.orderid","=","{int}:1"
                      }

                 };
            var selectItems = new List<SelectModel>() {
                                        new SelectModel()
                                        {
                                            AsName = "id",
                                            FiledName = "o.id"
                                         }
            };
            var x = jsonToSqlClient.Context.Queryable<object>()
                           .AS("order", "o")
                           .AddJoinInfo("orderdetail", "d", onList, JoinType.Left)
                            .AddJoinInfo("orderdetail", "c", onList2, JoinType.Left)
                           .Select(selectItems)
                           .ToList(); 
        }
        private static void GroupByTest(JsonClient jsonToSqlClient)
        {
            jsonToSqlClient.Context.Queryable<object>()
                     .AS("order").GroupBy(new List<GroupByModel> {
                 new GroupByModel(){
                  FieldName="id"
                 } }).Select("iD").ToList();

            var json = @"
{
	""Table"":  ""order"" ,
      GroupBy:[""name""],
      Having: [{SqlFunc_AggregateAvg:[""id""]},"">"",""{int}:1"" ],
      Select:[ [{SqlFunc_AggregateAvg:[""id""]},""id""],""name"" ]
}
";
            var x1 = jsonToSqlClient.Queryable(json).ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
        private static void OrderByTest(JsonClient jsonToSqlClient)
        {
            jsonToSqlClient.Context.Queryable<object>()
                .AS("order").OrderBy(new List<OrderByModel> {
                 new OrderByModel(){
                  FieldName="id",
                   OrderByType=OrderByType.Desc
                 },
                  new OrderByModel(){
                  FieldName="name",
                   OrderByType=OrderByType.Asc
                 }
                }).ToList();


            var x1 = jsonToSqlClient.Queryable("{Table:\"order\",OrderBy:[{FieldName:\"id\"},{FieldName:\"name\",OrderByType:\"desc\"}]}").ToSqlList();
            var list = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);


            var x2 = jsonToSqlClient.Queryable("{Table:\"order\",OrderBy:[[\"id\",\"desc\"],\"name\"]}").ToSqlList();
            var list1 = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x2[0].Sql, x2[0].Parameters);
        }
        private static void PageTest(JsonClient jsonToSqlClient)
        {
            var x1 = jsonToSqlClient.Queryable("{Table:\"order\",OrderBy:[ [\"id\",\"desc\"] ],PageNumber:1,PageSize:8}").ToSqlList();
            var list = jsonToSqlClient.Context.Ado.SqlQuery<dynamic>(x1[0].Sql, x1[0].Parameters);
        }
        private static void SelectTest(JsonClient jsonToSqlClient)
        {
            var list = new List<SelectModel>() {
                new SelectModel()
                {
                    AsName = "id1",
                    FiledName = "id"
                },
                  new SelectModel()
                  {

                    FiledName = "id"
                   }
                 };
            jsonToSqlClient.Context
                .Queryable<object>()
                .AS("order").Select(list).ToList();
        }
        private static void Description(JsonClient jsonToSqlClient)
        {
            jsonToSqlClient.Context.Queryable<object>().AS("order").Max<int>("id");
            var json = @"
                {
	                ""Table"":""order"",
                      PageNumber:""1"",
                      PageSize:""100""
                }
                ";
            var x1 = jsonToSqlClient.Queryable(json).ToResult();
            var result = jsonToSqlClient.Context.Utilities.SerializeObject(x1);
        }
        private static void PageTest2(JsonClient jsonToSqlClient)
        {
            var json = "{Table:\"order\",OrderBy:[ [\"id\",\"desc\"] ],Where:[\"name\",\"=\",\"{string}:a\"  ],PageNumber:1,PageSize:8}";

            var tableNames = jsonToSqlClient.GetTableNameList(json);//通过JSON获取JSON所有表
            var configs = GetConfigByUser(tableNames);//通过表获取行列过滤备注等信息

             var sqlList = jsonToSqlClient
                .Queryable(json) 
                .UseAuthentication(configs)//查询启用行列过滤 
                .ShowDesciption()//查询返回备注
                .ToResult();

        }
        private static void PageTest3(JsonClient jsonToSqlClient)
        {
            var json = "{Table:\"order\",OrderBy:[ [\"id\",\"desc\"] ]," +
                "Where:[\"name\",\"=\",\"{string}:a\"  ]," +
                "PageNumber:1,PageSize:8," +
                "Select:[\"id\",\"name\",\"price\"]}";

            var tableNames = jsonToSqlClient.GetTableNameList(json);//通过JSON获取JSON所有表
            var configs = GetConfigByUser(tableNames);//通过表获取行列过滤备注等信息

            var sqlList = jsonToSqlClient
               .Queryable(json)
               .UseAuthentication(configs)//查询启用行列过滤 
               .ShowDesciption()//查询返回备注
               .ToResult();

        }
        private static List<JsonTableConfig> GetConfigByUser(List<string> tableNames)
        {

            JsonTableConfig config = new JsonTableConfig()
            {
                TableName = "order",
                Columns = new List<JsonColumnConfig>()
                  {
                      new JsonColumnConfig(){  Name="id",Description="编号"   },
                      new JsonColumnConfig(){  Name="Name",Description="名称" , Validate="required", ValidateMessage="{Description}不能为空" },
                      new JsonColumnConfig(){  Name="Name",Description="名称" , Validate="unique", ValidateMessage="{Description}已存在" }
                  },
                Conditionals = new List<IConditionalModel>()
                {
                       new ConditionalModel(){ FieldName="id", ConditionalType= ConditionalType.Equal, FieldValue="1" }
                }
            };
            return new List<JsonTableConfig>() { config };
        }


        #endregion
    }
}
