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
        public object Handle(IMongoCollection<BsonDocument> collection, string json)
        {
            var filter = string.IsNullOrWhiteSpace(json) ? FilterDefinition<BsonDocument>.Empty : BsonDocument.Parse(json);
            // 执行查询并限制返回一条记录
            var document = collection.Find(filter).FirstOrDefault();

            // 如果查询到结果且文档非空，则获取第一个字段的值
            if (document != null && document.Elements.Any())
            {
                var firstElement = document.Elements.First();  // 获取第一个字段（列）
                return firstElement.Value;  // 返回该字段的值
            } 
            return null; //  
        } 
    }
}
