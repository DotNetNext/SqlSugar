using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Ado.data
{
    public class ExecuteScalarHandlerAsync
    {
        public async Task<object> HandleAsync(string operation, IMongoCollection<BsonDocument> collection, string json)
        {
            using (var dbReader = await new DbDataReaderFactoryAsync().HandleAsync(operation, collection, json))
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
