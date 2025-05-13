using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Ado.data 
{
    public class InsertHandlerAsync : IMongoOperationHandlerAsync
    {
        public string operation { get; set; }
        public async Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json)
        {
            var doc = BsonDocument.Parse(json);
            await collection.InsertOneAsync(doc);
            return 1;
        }
    }
}
