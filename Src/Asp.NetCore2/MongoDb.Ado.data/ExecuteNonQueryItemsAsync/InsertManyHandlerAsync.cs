using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace MongoDb.Ado.data 
{
    public class InsertManyHandlerAsync : IMongoOperationHandlerAsync
    {
        public HandlerContext context { get; set; }
        public CancellationToken token { get; set; }
        public string operation { get; set; }
        public async Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json)
        {
            var documents = ParseJsonArray(json);
            if (context.IsAnyServerSession)
            {
                await collection.InsertManyAsync(context.ServerSession,documents, null, token);
            }
            else
            {
                await collection.InsertManyAsync(documents, null, token);
            }
            var objectIds = documents.Select(it => it["_id"].AsObjectId.ToString()).ToArray();
            context.ids = objectIds;
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
