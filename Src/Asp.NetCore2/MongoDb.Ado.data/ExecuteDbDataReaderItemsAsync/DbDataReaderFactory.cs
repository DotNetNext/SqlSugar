using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Ado.data
{
    public class DbDataReaderFactoryAsync
    {
        public readonly static Dictionary<string, IQueryHandlerAsync> Items = new Dictionary<string, IQueryHandlerAsync>(StringComparer.OrdinalIgnoreCase)
            {
                { "find", new QueryFindHandlerAsync() },
                { "aggregate", new QueryFindHandlerAsync() },
            };
        public async Task<DbDataReader> Handle(string operation, IMongoCollection<BsonDocument> collection, string json)
        {
            var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonValue>(json);
            DbDataReaderFactoryAsync.Items.TryGetValue(operation, out var handler);
            if (handler == null)
            {
                await  ExecuteHandlerFactoryAsync.HandlerAsync(operation, json, collection);
                return new DataTable().CreateDataReader();
            }
            return await handler.HandlerAsync(collection, doc);
        }

    }
}
