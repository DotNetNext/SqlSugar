using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Ado.data 
{
    public class QueryFindHandlerAsync : IQueryHandlerAsync
    {
        public async Task<DbDataReader> HandlerAsync(IMongoCollection<BsonDocument> collection, BsonValue doc)
        {
            BsonDocument filter;
            BsonDocument projection = null;

            if (doc.IsBsonArray)
            {
                var array = doc.AsBsonArray;
                filter = array.Count > 0 ? array[0].AsBsonDocument : new BsonDocument();
                if (array.Count > 1)
                    projection = array[1].AsBsonDocument;
            }
            else if (doc.IsBsonDocument)
            {
                filter = doc.AsBsonDocument;
            }
            else
            {
                throw new ArgumentException("Invalid JSON format for MongoDB find operation.");
            }

            var findFluent = collection.Find(filter);

            if (projection != null)
                findFluent = findFluent.Project<BsonDocument>(projection);

            var cursor =await findFluent.ToListAsync();    
            return new MongoDbBsonDocumentDataReader(cursor); // 你要确保这个类支持逐行读取 BsonDocument
        }
    }
}
