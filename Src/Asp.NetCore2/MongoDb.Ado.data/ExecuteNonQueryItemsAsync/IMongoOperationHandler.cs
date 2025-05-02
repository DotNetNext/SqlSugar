using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Ado.data 
{
    public interface IMongoOperationHandlerAsync
    {
        Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json);
    }
}
