using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDb.Ado.data
{
    public class ExecuteScalarHandler
    {
        public object Handle(string operation, IMongoCollection<BsonDocument> collection, string json, HandlerContext context)
        {
            using (var dbReader = new DbDataReaderFactory().Handle(operation, collection, json,context))
            {
                if (dbReader.Read())
                {
                   return dbReader.GetValue(0);
                } 
                return null; //  
            }
        }
    }
}
