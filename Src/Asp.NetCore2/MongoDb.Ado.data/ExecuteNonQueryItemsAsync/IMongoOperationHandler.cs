using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDb.Ado.data 
{
    public interface IMongoOperationHandlerAsync
    { 
        string operation { get; set; }
        CancellationToken token { get; set; }
        HandlerContext context { get; set; }

        Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json);
    }
}
