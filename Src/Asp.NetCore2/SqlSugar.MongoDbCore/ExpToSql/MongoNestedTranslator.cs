using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;

namespace SqlSugar.MongoDb 
{ 
    public static class MongoNestedTranslator
    {
        public static BsonValue TranslateNoFieldName(Expression expr, MongoNestedTranslatorContext context,ExpressionVisitorContext visContext=null)
        {
            return new ExpressionVisitor(context) {  visitorContext= visContext }.Visit(expr);
        }
        public static BsonDocument Translate(Expression expr, MongoNestedTranslatorContext context)
        {
            var result = new ExpressionVisitor(context).Visit(expr);
            if (result is BsonString bs) 
            {
                result = new BsonDocument(UtilConstants.FieldName, bs);
            }
            return (BsonDocument)result;
        }
    }
}
