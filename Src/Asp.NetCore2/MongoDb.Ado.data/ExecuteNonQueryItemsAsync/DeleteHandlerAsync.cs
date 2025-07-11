﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDb.Ado.data 
{
    public class DeleteHandlerAsync : IMongoOperationHandlerAsync
    {
        public HandlerContext context { get; set; }
        public CancellationToken token { get; set; }
        public string operation { get; set; }
        public async Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json)
        {
            var doc = BsonDocument.Parse(json);
            var filter = doc["filter"].AsBsonDocument;
            if (context.IsAnyServerSession)
            {
                var result = await collection.DeleteOneAsync(context.ServerSession,filter,null,token);
                return (int)result.DeletedCount;
            }
            else
            {
                var result = await collection.DeleteOneAsync(filter, token);
                return (int)result.DeletedCount;
            }
        }
    }
}
