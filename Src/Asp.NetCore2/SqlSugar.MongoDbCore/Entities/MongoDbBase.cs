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
    public class MongoDbBaseLong
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, ColumnName = "_id")]
        public long Id { get; set; }
    }
    public class MongoDbBaseString
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, ColumnName = "_id")]
        public string Id { get; set; }
    }
    public class MongoDbBaseGuid
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, ColumnName = "_id")]
        public Guid Id { get; set; }
    }
}
