using MongoDb.Ado.data;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbTest.DBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MongoDbTest
{
    public  class AdoTest
    {
        public static void Init()
        {
            MongoClientTest();
            MongoDbConnectionTest();
            MongoDbCommandTest();
            MongoDbCommandTestAsync().GetAwaiter().GetResult();
        }

        #region MongoDbCommandTest
        private static void MongoDbCommandTest()
        {
            DataReaderTest();
            DataTableTest();
            ExecuteScalarTest();
            ExecuteNonQueryTest();
        }
        private static void ExecuteNonQueryTest()
        {
            //ExecuteNonQuery insert
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(
                    "insert b { name: \"John\", age: 31 }",
                    connection);
                var value = mongoDbCommand.ExecuteNonQuery();
                connection.Close();
            }
            //ExecuteNonQuery insertMany
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(" insertMany b  [{ name: \"John\", age: 31 }, { name: \"Alice\", age: 25 }, { name: \"Bob\", age: 30 }  ]  ", connection);
                var value = mongoDbCommand.ExecuteNonQuery();
                connection.Close();
            }
            //ExecuteNonQuery update
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(
                   @"update b {
                              ""filter"": { ""name"": ""John"" },
                              ""update"": { ""$set"": { ""age"": 32 } }
                            }",
                    connection);
                var value = mongoDbCommand.ExecuteNonQuery();
                connection.Close();
            }
            //ExecuteNonQuery updateMany
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(
                       @"updateMany b [{
                              ""filter"": { ""name"": ""John"" },
                              ""update"": { ""$set"": { ""age"": 32 } }
                            }]",
                    connection);
                var value = mongoDbCommand.ExecuteNonQuery();
                connection.Close();
            }
            //ExecuteNonQuery delete
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(
                      "delete  b {\"filter\":{ name: \"John\" }}",
                    connection);
                var value = mongoDbCommand.ExecuteNonQuery();
                connection.Close();
            }
            //ExecuteNonQuery delete
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(
                    "delete   b {\"filter\":{ name: \"John\" }}",
                    connection);
                var value = mongoDbCommand.ExecuteNonQuery();
                connection.Close();
            }
            //ExecuteNonQuery deleteMany
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(
                    "deleteMany  b [{\"filter\":{ name: \"John\" }}]",
                    connection);
                var value = mongoDbCommand.ExecuteNonQuery();
                connection.Close();
            }
            //ExecuteNonQuery Find
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(
                    " find b { age: { $gt: 31 } }",
                    connection);
                var value = mongoDbCommand.ExecuteNonQuery();
                connection.Close();
            }
        }
        private static void DataTableTest()
        {
            //datatable
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                ////SELECT *  FROM b ORDER BY age DESC OFFSET 1 ROWS FETCH NEXT 2 ROWS ONLY; 
                MongoDbCommand mongoDbCommand = new MongoDbCommand(" aggregate b [\r\n  { \"$sort\": { \"age\": -1 } },\r\n  { \"$skip\": 1 },\r\n  { \"$limit\": 2 }\r\n] ]", connection);
                MongoDbDataAdapter mongoDbDataAdapter = new MongoDbDataAdapter();
                mongoDbDataAdapter.SelectCommand = mongoDbCommand;
                DataTable dt = new DataTable();
                mongoDbDataAdapter.Fill(dt);
                connection.Close();
            }
            //dataset
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                ////SELECT *  FROM b ORDER BY age DESC OFFSET 1 ROWS FETCH NEXT 2 ROWS ONLY; 
                MongoDbCommand mongoDbCommand = new MongoDbCommand(" aggregate b [\r\n  { \"$sort\": { \"age\": -1 } },\r\n  { \"$skip\": 1 },\r\n  { \"$limit\": 2 }\r\n] ]", connection);
                MongoDbDataAdapter mongoDbDataAdapter = new MongoDbDataAdapter();
                mongoDbDataAdapter.SelectCommand = mongoDbCommand;
                DataSet ds = new DataSet();
                mongoDbDataAdapter.Fill(ds);
                connection.Close();
            }
        }
        private static void ExecuteScalarTest()
        {
            //ExecuteScalar

            var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
            connection.Open();
            MongoDbCommand mongoDbCommand = new MongoDbCommand(" find b { age: { $gt: 31 } }", connection);
            var value = mongoDbCommand.ExecuteScalar();
            connection.Close();
        } 
        private static void DataReaderTest()
        {
            //ExecuteReader single query 1
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                //对应的SQL: SELECT * FROM b  WHERE age > 18;
                MongoDbCommand mongoDbCommand = new MongoDbCommand(" find  b { age: { $gt: 32 } } ", connection);
                using (var reader = mongoDbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString("name");
                        var age = reader.GetInt32("age");
                    }
                }
                connection.Close();
            }
            //ExecuteReader single query 2
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                //对应的SQL: SELECT name, age, _id FROM b  WHERE age > 18;
                MongoDbCommand mongoDbCommand = new MongoDbCommand(" find b [{ age: { $gt: 18 } }, {  name: 1, age: 1 }]", connection);
                using (var reader = mongoDbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString("name");
                        var age = reader.GetInt32("age");
                    }
                }
                connection.Close();
            }
            //ExecuteReader single query 3
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                //SELECT *  FROM b ORDER BY age DESC OFFSET 1 ROWS FETCH NEXT 2 ROWS ONLY; 
                MongoDbCommand mongoDbCommand = new MongoDbCommand(" aggregate b [\r\n  { \"$sort\": { \"age\": -1 } },\r\n  { \"$skip\": 1 },\r\n  { \"$limit\": 2 }\r\n] ]", connection);
                using (var reader = mongoDbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString("name");
                        var age = reader.GetInt32("age");
                    }
                }
                connection.Close();
            }
            //ExecuteReader join query
            {
                //SELECT b1._id, b1.a_id, b2.some_field AS joined_field
                // FROM b AS b1
                //LEFT JOIN b AS b2 ON b1.a_id = b2._id
                //WHERE b2.some_field > 100
                //ORDER BY b2.some_field DESC
                // LIMIT 20 OFFSET 10;
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                connection.Open();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(@" aggregate b [
                  {
                    ""$lookup"": {
                      ""from"": ""b"",
                      ""localField"": ""a_id"",
                      ""foreignField"": ""_id"",
                      ""as"": ""joined_docs""
                    }
                  },
                  {
                    ""$unwind"": ""$joined_docs""
                  },
                  {
                    ""$match"": {
                      ""joined_docs.some_field"": { ""$gt"": 100 }
                    }
                  },
                  {
                    ""$project"": {
                      ""_id"": 1,
                      ""a_id"": 1,
                      ""joined_field"": ""$joined_docs.some_field""
                    }
                  }
                 ])", connection);
                using (var reader = mongoDbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString("name");
                        var age = reader.GetInt32("age");
                    }
                }
                connection.Close();
            }
        }
        #endregion

        #region MongoDbCommandTestAsync
        private static async Task MongoDbCommandTestAsync()
        {
            await DataReaderTestAsync();
            await DataTableTestAsync();
            await ExecuteScalarTestAsync();
            await ExecuteNonQueryTestAsync();
        }

        private static async Task ExecuteNonQueryTestAsync()
        {
            // ExecuteNonQueryAsync insert
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                await connection.OpenAsync();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(
                    "insert b { name: \"John\", age: 31 }",
                    connection);
                var value = await mongoDbCommand.ExecuteNonQueryAsync();
                connection.Close();
            }
            // ExecuteNonQueryAsync insertMany
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                await connection.OpenAsync();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(
                    "insertMany b [{ name: \"John\", age: 31 }, { name: \"Alice\", age: 25 }, { name: \"Bob\", age: 30 }]",
                    connection);
                var value = await mongoDbCommand.ExecuteNonQueryAsync();
                connection.Close();
            }
            // ExecuteNonQueryAsync update
            {
                var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
                await connection.OpenAsync();
                MongoDbCommand mongoDbCommand = new MongoDbCommand(
                    @"update b {
                ""filter"": { ""name"": ""John"" },
                ""update"": { ""$set"": { ""age"": 32 } }
            }",
                    connection);
                var value = await mongoDbCommand.ExecuteNonQueryAsync();
                connection.Close();
            }
            // 其他类似的异步测试方法...
        }

        private static async Task ExecuteScalarTestAsync()
        {
            var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
            await connection.OpenAsync();
            MongoDbCommand mongoDbCommand = new MongoDbCommand("find b { age: { $gt: 31 } }", connection);
            var value = await mongoDbCommand.ExecuteScalarAsync();
            connection.Close();
        }

        private static async Task DataReaderTestAsync()
        {
            var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
            await connection.OpenAsync();
            MongoDbCommand mongoDbCommand = new MongoDbCommand("find b { age: { $gt: 32 } }", connection);
            using (var reader = await mongoDbCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var name = reader.GetString("name");
                    var age = reader.GetInt32("age");
                }
            }
            connection.Close();
        }

        private static async Task DataTableTestAsync()
        {
            var connection = new MongoDbConnection(DbHelper.SqlSugarConnectionString);
            await connection.OpenAsync();
            MongoDbCommand mongoDbCommand = new MongoDbCommand(
                "aggregate b [ { \"$sort\": { \"age\": -1 } }, { \"$skip\": 1 }, { \"$limit\": 2 } ]",
                connection);
            MongoDbDataAdapter mongoDbDataAdapter = new MongoDbDataAdapter();
            mongoDbDataAdapter.SelectCommand = mongoDbCommand;
            DataTable dt = new DataTable();
            await Task.Run(() => mongoDbDataAdapter.Fill(dt)); // 模拟异步操作
            connection.Close();
        }
        #endregion

        private static void MongoDbConnectionTest()
        {
           var db= new MongoDbConnection(DbHelper.SqlSugarConnectionString);

           var database= db.GetDatabase();
            var collections = database.GetCollection<BsonDocument>("b");
            // 插入一个文档，MongoDB 会创建数据库和集合
            var document = new BsonDocument { { "name", "bbbbbb" }, { "age", 40 } };
            collections.InsertOne(document);
            var list = collections.AsQueryable<BsonDocument>().ToList();
        }

        private static void MongoClientTest()
        {
            //开发中
            var client = new MongoClient(DbHelper.ConnectionString);
            var database = client.GetDatabase("SqlSugarDb");
            // 获取当前数据库中的所有集合
            var collections = database.GetCollection<BsonDocument>("a");
            // 插入一个文档，MongoDB 会创建数据库和集合
            var document = new BsonDocument { { "name", "aaaa" }, { "age", 30 } };
            collections.InsertOne(document);
            var list = collections.AsQueryable<BsonDocument>().ToList();
        }
    }
}
