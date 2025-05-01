using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Ado.data 
{
    public interface IMongoOperationHandler
    {
        int Handle(IMongoCollection<BsonDocument> collection, string json);
    }
}
