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
        public DbDataReader Handle(string operation, IMongoCollection<BsonDocument> collection, string json)
        {
            var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonValue>(json);
            IQueryHandler queryHandler = null;
            if (operation == "find")
            {
                queryHandler = new QueryFindHandler();
            }
            else if (operation == "aggregate")
            {
                queryHandler = new QueryAggregateHandler();
            }
            else 
            {
                ExecuteHandlerFactory.Handler(operation,json, collection);
                return new DataTable().CreateDataReader();
            }
            return queryHandler.Find(collection, doc);
        }

    }
}
