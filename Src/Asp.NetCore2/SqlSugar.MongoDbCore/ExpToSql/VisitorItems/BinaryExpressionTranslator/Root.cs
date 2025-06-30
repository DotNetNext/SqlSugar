using Dm.parser;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public partial class BinaryExpressionTranslator
    {
        MongoNestedTranslatorContext _context;
        ExpressionVisitorContext _visitorContext;
        public BinaryExpressionTranslator(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            _context = context;
            _visitorContext = visitorContext;
        }

        public BsonDocument Extract(BinaryExpression expr)
        {
            if (IsLogicalExpression(expr))
                return LogicalBinaryExpression(expr);
            else
                return FieldComparisonOrCalculationExpression(expr);
        }

        private static bool IsLogicalExpression(BinaryExpression expr)
        {
            return expr.NodeType == ExpressionType.AndAlso || expr.NodeType == ExpressionType.OrElse;
        }

        private static bool IsRightValue(bool leftIsMember, bool rightIsMember, string op)
        {
            return  rightIsMember && !leftIsMember;
        }

        private static bool IsLeftValue(bool leftIsMember, bool rightIsMember, string op)
        {
            return  leftIsMember && !rightIsMember;
        }
    }

}
