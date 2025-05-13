using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Ado.data 
{
    public class UpdateHandlerAsync : IMongoOperationHandlerAsync
    {
        public string operation { get; set; }
        public async Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json)
        {
            var doc = BsonDocument.Parse(json);
            var filter = doc["filter"].AsBsonDocument;
            var update = doc["update"].AsBsonDocument;
            var result =await collection.UpdateOneAsync(filter, update);
            return (int)result.ModifiedCount;
        }
    }
}
