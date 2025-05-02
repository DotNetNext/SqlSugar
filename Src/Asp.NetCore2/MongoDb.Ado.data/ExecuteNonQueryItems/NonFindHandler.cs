using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Ado.data 
{
    public class NonFindHandler : IMongoOperationHandler
    {
        public int Handle(IMongoCollection<BsonDocument> collection, string json)
        {
            var filter = string.IsNullOrWhiteSpace(json) ? FilterDefinition<BsonDocument>.Empty : BsonDocument.Parse(json);
            var result = collection.Find(filter).FirstOrDefault();
            return 0; // 查询不改变数据库
        }
    }
}
