using Newtonsoft.Json.Linq;
using SqlSugar.MongoDbCore.ExpToSql.Context;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDbCore.ExpToSql.VisitorItems 
{
    public  class ValueExtractor
    {
        MongoNestedTranslatorContext _context;
        public ValueExtractor(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            _context = context;
        }
        public  JToken Extract(ConstantExpression expr)
        { 
            return JToken.FromObject(expr.Value);
        }

        public static JToken GetValue(Expression expr)
        {
            var lambda = Expression.Lambda(expr);
            var compiled = lambda.Compile();
            return JToken.FromObject(compiled.DynamicInvoke());
        }
    }
}
