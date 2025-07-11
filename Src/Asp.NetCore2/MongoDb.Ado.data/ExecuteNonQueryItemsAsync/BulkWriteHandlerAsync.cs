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
    public class BulkWriteHandlerAsync : IMongoOperationHandlerAsync
    {
        public HandlerContext context { get; set; }
        public CancellationToken token { get; set; }
        public string operation { get; set; }
        public async Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json)
        {
            var documents = ParseJsonArray(json);
            var bulkOps = new List<WriteModel<BsonDocument>>();

            foreach (var doc in documents)
            {
                var filter = doc["filter"].AsBsonDocument;
                var update = doc["update"].AsBsonDocument;

                var op = new UpdateManyModel<BsonDocument>(filter, update);
                bulkOps.Add(op);
            }
            if (bulkOps.Count == 0) return 0;
            if (context.IsAnyServerSession)
            {
                var result = await collection.BulkWriteAsync(context.ServerSession,bulkOps);
                return (int)result.ModifiedCount;
            }
            else
            {
                var result = await collection.BulkWriteAsync(bulkOps);
                return (int)result.ModifiedCount;
            }
        }

        private List<BsonDocument> ParseJsonArray(string json)
        {
            if (json.TrimStart().StartsWith("["))
                return BsonSerializer.Deserialize<List<BsonDocument>>(json);
            return new List<BsonDocument> { BsonDocument.Parse(json) };
        }
    }

}
