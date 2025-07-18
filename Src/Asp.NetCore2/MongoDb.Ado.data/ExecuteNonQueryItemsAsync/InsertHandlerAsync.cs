using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDb.Ado.data 
{
    public class InsertHandlerAsync : IMongoOperationHandlerAsync
    {
        public HandlerContext context { get; set; }
        public CancellationToken token { get; set; }
        public string operation { get; set; }
        public async Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json)
        {
            var doc = BsonDocument.Parse(json);
            if (context.IsAnyServerSession)
            {
                await collection.InsertOneAsync(context.ServerSession,doc, null, token);
            }
            else
            {
                await collection.InsertOneAsync(doc, null, token);
            }
            var objectId = doc["_id"].IsObjectId? doc["_id"].AsObjectId.ToString() : doc["_id"].ToString();
            context.ids = new string[] { objectId };
            return 1;
        }
    }
}
