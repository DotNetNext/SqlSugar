using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace MongoDb.Ado.data
{
    public class QueryAggregateHandler : IQueryHandler
    {
        public DbDataReader Handler(IMongoCollection<BsonDocument> collection, BsonValue doc)
        {
            // 解析 JSON 字符串为 BsonArray
            var pipeline =  doc.AsBsonArray; ;

            // 构建聚合管道
            var aggregateFluent = collection.Aggregate<BsonDocument>(pipeline.Select(stage => new BsonDocument(stage.AsBsonDocument)).ToArray());

            // 执行聚合查询并返回 DbDataReader
            var cursor = aggregateFluent.ToList(); 
            var result= new MongoDbBsonDocumentDataReader(cursor);
            return result;
        }
    }
}
