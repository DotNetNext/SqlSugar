﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDb.Ado.data 
{
    public class UpdateHandlerAsync : IMongoOperationHandlerAsync
    {
        public HandlerContext context { get; set; }
        public CancellationToken token { get; set; }
        public string operation { get; set; }
        public async Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json)
        {
            var doc = BsonDocument.Parse(json);
            var filter = doc["filter"].AsBsonDocument;
            var update = doc["update"].AsBsonDocument;
            if (context.IsAnyServerSession)
            {
                var result = await collection.UpdateOneAsync(context.ServerSession,filter, update, null, token);
                return (int)result.ModifiedCount;
            }
            else
            {
                var result = await collection.UpdateOneAsync(filter, update, null, token);
                return (int)result.ModifiedCount;
            }
        }
    }
}
