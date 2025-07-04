using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.MongoDb
{
    public class MongoDbBase
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [SugarColumn(IsPrimaryKey =true,IsOnlyIgnoreInsert =true,ColumnName ="_id")]
        public string Id { get; set; }
    }
}
