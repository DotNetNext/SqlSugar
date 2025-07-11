using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Ado.data 
{
    public class NonFindHandler : IMongoOperationHandler
    {
        public HandlerContext context { get; set; }
        public string operation { get; set; }
        public int Handle(IMongoCollection<BsonDocument> collection, string json)
        {
            using (var dr = new DbDataReaderFactory().Handle(operation, collection, json,null))
            {
                if (dr.Read())
                {

                }
            }
            return 0; // 查询不改变数据库
        }
    }
}
