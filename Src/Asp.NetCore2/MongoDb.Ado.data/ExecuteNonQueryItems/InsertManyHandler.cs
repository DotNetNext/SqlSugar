using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MongoDb.Ado.data 
{
    public class InsertManyHandler : IMongoOperationHandler
    {
        public HandlerContext context { get; set; }
        public string operation { get; set; }
        public int Handle(IMongoCollection<BsonDocument> collection, string json)
        {
            var documents = ParseJsonArray(json);
            if (context.IsAnyServerSession)
            {
                collection.InsertMany(context.ServerSession,documents);
            }
            else
            {
                collection.InsertMany(documents);
            }
            var objectIds = documents.Select(it=>it["_id"].AsObjectId.ToString()).ToArray();
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
