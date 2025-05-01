using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Ado.data 
{
    public class UpdateManyHandler : IMongoOperationHandler
    {
        public int Handle(IMongoCollection<BsonDocument> collection, string json)
        {
            var documents = ParseJsonArray(json);
            int total = 0;
            foreach (var doc in documents)
            {
                var filter = doc["filter"].AsBsonDocument;
                var update = doc["update"].AsBsonDocument;
                var result = collection.UpdateMany(filter, update);
                total += (int)result.ModifiedCount;
            }
            return total;
        }

        private List<BsonDocument> ParseJsonArray(string json)
        {
            if (json.TrimStart().StartsWith("["))
                return BsonSerializer.Deserialize<List<BsonDocument>>(json);
            return new List<BsonDocument> { BsonDocument.Parse(json) };
        }
    }

}
