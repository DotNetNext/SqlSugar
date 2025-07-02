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
                result = TranslateStringToBson(bs);
            else if (result is BsonBoolean bb)
                result = TranslateBooleanToBson(bb);
            return (BsonDocument)result;
        }

        private static BsonValue TranslateStringToBson(BsonString bs)
        {
            return new BsonDocument(UtilConstants.FieldName, bs);
        }

        private static BsonValue TranslateBooleanToBson(BsonBoolean bb)
        {
            BsonValue result;
            if (bb == true)
            {
                result = new BsonDocument("$expr", new BsonDocument("$eq", new BsonArray { 1, 1 }));
            }
            else
            {
                result = new BsonDocument("$expr", new BsonDocument("$eq", new BsonArray { 1, 0 }));
            }

            return result;
        }
    }
}
