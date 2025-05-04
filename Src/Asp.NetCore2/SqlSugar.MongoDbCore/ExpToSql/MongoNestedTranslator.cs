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
            return (BsonDocument)new ExpressionVisitor(context).Visit(expr);
        }
    }
}
