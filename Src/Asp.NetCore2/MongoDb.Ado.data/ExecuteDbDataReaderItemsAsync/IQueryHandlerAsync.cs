using MongoDB.Bson;
using MongoDB.Driver;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDb.Ado.data
{
    public interface IQueryHandlerAsync
    {
        CancellationToken token { get; set; }

        Task<DbDataReader> HandlerAsync(IMongoCollection<BsonDocument> collection, BsonValue doc);
    }
}