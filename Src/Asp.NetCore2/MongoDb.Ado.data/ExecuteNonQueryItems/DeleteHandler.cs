using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Ado.data 
{
    public class DeleteHandler : IMongoOperationHandler
    {
        public string operation { get; set; }
        public HandlerContext context { get; set; }

        public int Handle(IMongoCollection<BsonDocument> collection, string json)
        {
            var doc = BsonDocument.Parse(json);
            var filter = doc["filter"].AsBsonDocument;
            if (context.IsAnyServerSession)
            {
                var result = collection.DeleteOne(context.ServerSession,filter); 
                return (int)result.DeletedCount;
            }
            else
            {
                var result = collection.DeleteOne(filter); 
                return (int)result.DeletedCount;
            }
        }
    }
}
