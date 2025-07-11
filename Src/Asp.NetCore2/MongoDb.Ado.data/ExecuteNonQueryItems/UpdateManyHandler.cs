﻿using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Ado.data 
{
    public class UpdateManyHandler : IMongoOperationHandler
    {
        public HandlerContext context { get; set; }
        public string operation { get; set; }
        public int Handle(IMongoCollection<BsonDocument> collection, string json)
        {
            var documents = ParseJsonArray(json);
            int total = 0;
            foreach (var doc in documents)
            {
                var filter = doc["filter"].AsBsonDocument;
                var update = doc["update"].AsBsonDocument;
                if (context.IsAnyServerSession)
                {
                    var result = collection.UpdateMany(context.ServerSession,filter, update);
                    total += (int)result.ModifiedCount;
                }
                else
                {
                    var result = collection.UpdateMany(filter, update);
                    total += (int)result.ModifiedCount;
                }
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
