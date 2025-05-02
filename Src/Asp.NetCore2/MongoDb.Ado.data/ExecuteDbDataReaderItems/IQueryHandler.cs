using MongoDB.Bson;
using MongoDB.Driver;
using System.Data.Common;

namespace MongoDb.Ado.data
{
    public interface IQueryHandler
    {
        DbDataReader Find(IMongoCollection<BsonDocument> collection, BsonValue doc);
    }
}