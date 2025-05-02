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
        public async Task<DbDataReader> HandleAsync(string operation, IMongoCollection<BsonDocument> collection, string json)
        {
            var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonValue>(json);
            IQueryHandlerAsync queryHandler = null;
            if (operation == "find")
            {
                queryHandler = new QueryFindHandlerAsync();
            }
            else if (operation == "aggregate")
            {
                queryHandler = new QueryAggregateHandlerAsync();
            }
            else 
            {
                await ExecuteHandlerFactoryAsync.HandlerAsync(operation,json, collection);
                return new DataTable().CreateDataReader();
            }
            return await queryHandler.HandlerAsync(collection, doc);
        }

    }
}
