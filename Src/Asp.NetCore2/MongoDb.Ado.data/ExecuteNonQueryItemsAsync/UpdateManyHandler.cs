using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MongoDb.Ado.data 
{
    public class UpdateManyHandlerAsync : IMongoOperationHandlerAsync
    {
        public CancellationToken token { get; set; }
        public string operation { get; set; }
        public async  Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json)
        {
            var documents = ParseJsonArray(json);
            int total = 0;
            foreach (var doc in documents)
            {
                var filter = doc["filter"].AsBsonDocument;
                var update = doc["update"].AsBsonDocument;
                var result =await  collection.UpdateManyAsync(filter, update,null,token);
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
