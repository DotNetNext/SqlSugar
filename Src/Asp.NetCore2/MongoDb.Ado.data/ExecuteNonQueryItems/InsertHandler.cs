using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Ado.data 
{
    public class InsertHandler : IMongoOperationHandler
    {
        public HandlerContext context { get; set; }
        public string operation { get; set; }
        public int Handle(IMongoCollection<BsonDocument> collection, string json)
        {
            var doc = BsonDocument.Parse(json);
            collection.InsertOne(doc);
            var objectId = doc["_id"].AsObjectId.ToString();
            context.ids = new string[] { objectId };
            return 1;
        }
    }
}
