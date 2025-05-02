using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Ado.data 
{
    public class InsertManyHandlerAsync : IMongoOperationHandlerAsync
    {
        public async Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json)
        {
            var documents = ParseJsonArray(json);
            await collection.InsertManyAsync(documents);
            return documents.Count;
        }

        private List<BsonDocument> ParseJsonArray(string json)
        {
            if (json.TrimStart().StartsWith("["))
                return BsonSerializer.Deserialize<List<BsonDocument>>(json);
            return new List<BsonDocument> { BsonDocument.Parse(json) };
        }
    }
}
