using MongoDb.Ado.data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public  class AdoTest
    {
        public static void Init()
        {
            MongoClientTest();
            MongoDbConnectionTest();
        }

        private static void MongoDbConnectionTest()
        {
           var db= new MongoDbConnection(DbHelper.SqlSugarConnectionString);

           var database= db.GetDatabase();
            var collections = database.GetCollection<BsonDocument>("b");
            // 插入一个文档，MongoDB 会创建数据库和集合
            var document = new BsonDocument { { "name", "bbbbbb" }, { "age", 30 } };
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
