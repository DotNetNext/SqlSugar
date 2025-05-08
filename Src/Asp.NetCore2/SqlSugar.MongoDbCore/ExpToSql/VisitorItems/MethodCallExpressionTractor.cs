using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDbCore 
{
    public class MethodCallExpressionTractor
    {
        MongoNestedTranslatorContext _context;
        ExpressionVisitorContext _visitorContext;
        public MethodCallExpressionTractor(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            _context = context;
            _visitorContext = visitorContext; 
        }
        public BsonValue Extract(Expression expr)
        {
            if (ExpressionTool.GetParameters(expr).Count == 0)
            {
                return BsonValue.Create(ExpressionTool.DynamicInvoke(expr));
            }
            return null;
        }
    }
}
