using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Ado.data 
{
    public class InsertManyHandler : IMongoOperationHandler
    {
        public int Handle(IMongoCollection<BsonDocument> collection, string json)
        {
            var documents = ParseJsonArray(json);
            collection.InsertMany(documents);
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
