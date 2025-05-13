﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Ado.data
{
    public class NonFindHandlerAsync : IMongoOperationHandlerAsync
    {
        public string operation { get; set; }

        public async Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json)
        {
            using (var dr = await new DbDataReaderFactoryAsync().HandleAsync(operation, collection, json))
            {
                if (dr.Read())
                {

                }
            }
            return 0; // 查询不改变数据库
        }
    }
}
