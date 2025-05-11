using MongoDB.Bson;
using Newtonsoft.Json.Linq; 
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public class ValueExtractor
    {
        MongoNestedTranslatorContext _context;

        public ValueExtractor(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            _context = context;
        }

        public BsonValue Extract(ConstantExpression expr)
        {
            return BsonValue.Create(expr.Value);
        } 
    }
}
