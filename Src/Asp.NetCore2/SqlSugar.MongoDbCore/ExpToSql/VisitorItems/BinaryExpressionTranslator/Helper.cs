using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb
{
    public partial class BinaryExpressionTranslator
    { 
        private void OutParameters(BinaryExpression expr, out BsonValue field, out BsonValue value, out bool leftIsMember, out bool rightIsMember, out string op)
        {
            var leftVisitor = new ExpressionVisitor(_context, new ExpressionVisitorContext());
            var rightVisitor = new ExpressionVisitor(_context, new ExpressionVisitorContext());

            field = leftVisitor.Visit(expr.Left);
            value = rightVisitor.Visit(expr.Right);

            leftIsMember = leftVisitor.visitorContext?.ExpType == typeof(MemberExpression);
            rightIsMember = rightVisitor.visitorContext?.ExpType == typeof(MemberExpression);

            op = expr.NodeType switch
            {
                ExpressionType.Equal => "$eq",
                ExpressionType.NotEqual => "$ne",
                ExpressionType.GreaterThan => "$gt",
                ExpressionType.GreaterThanOrEqual => "$gte",
                ExpressionType.LessThan => "$lt",
                ExpressionType.LessThanOrEqual => "$lte",
                _ => null
            };
        } 
    }
}
