using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;

namespace SqlSugar.MongoDbCore 
{ 
    public static class MongoNestedTranslator
    {
        public static BsonDocument Translate(Expression expr, MongoNestedTranslatorContext context)
        {
            var result = new ExpressionVisitor(context).Visit(expr);
            if (result is BsonString bs) 
            {
                result = new BsonDocument("fieldName", bs);
            }
            return (BsonDocument)result;
        }
    }
}
