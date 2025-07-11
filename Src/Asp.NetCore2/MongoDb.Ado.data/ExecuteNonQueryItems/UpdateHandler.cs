using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Ado.data 
{
    public class UpdateHandler : IMongoOperationHandler
    {
        public HandlerContext context { get; set; }
        public string operation { get; set; }
        public int Handle(IMongoCollection<BsonDocument> collection, string json)
        {
            var doc = BsonDocument.Parse(json);
            var filter = doc["filter"].AsBsonDocument;
            var update = doc["update"].AsBsonDocument;
            if (context.IsAnyServerSession) 
            {
                var result = collection.UpdateOne(context.ServerSession,filter, update);
                return (int)result.ModifiedCount;
            }
            else
            {
                var result = collection.UpdateOne(filter, update);
                return (int)result.ModifiedCount;
            }
          
        }
    }
}
