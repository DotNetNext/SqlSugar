using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net.Http.Headers;
using System.Text;

namespace MongoDb.Ado.data
{
    public class DbDataReaderFactory
    {
        public readonly static Dictionary<string, IQueryHandler> Items = new Dictionary<string, IQueryHandler>(StringComparer.OrdinalIgnoreCase)
            {
                { "find", new QueryFindHandler() },
                { "aggregate", new QueryAggregateHandler() },
            };
        public DbDataReader Handle(string operation, IMongoCollection<BsonDocument> collection, string json)
        {
            MongoDbMethodUtils.ValidateOperation(operation);
            var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonValue>(json);
            DbDataReaderFactory.Items.TryGetValue(operation, out var handler);
            if (handler==null)
            {
                ExecuteHandlerFactory.Handler(operation, json, collection,new HandlerContext());
                return new DataTable().CreateDataReader();
            }
            return handler.Handler(collection, doc);
        }

    }
}
