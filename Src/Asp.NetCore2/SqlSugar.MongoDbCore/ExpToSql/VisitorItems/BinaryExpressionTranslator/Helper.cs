using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb
{
    public partial class BinaryExpressionTranslator
    {

        private static bool IsEq(string op)
        {
            return op == "$eq";
        } 
        private void OutParameters(BinaryExpression expr, out BsonValue field, out BsonValue value, out bool leftIsMember, out bool rightIsMember, out string op)
        {
            var leftVisitor = new ExpressionVisitor(_context, new ExpressionVisitorContext());
            var rightVisitor = new ExpressionVisitor(_context, new ExpressionVisitorContext());

            field = leftVisitor.Visit(expr.Left);
            value = rightVisitor.Visit(expr.Right);

            leftIsMember = leftVisitor.visitorContext?.ExpType == typeof(MemberExpression);
            rightIsMember = rightVisitor.visitorContext?.ExpType == typeof(MemberExpression);

            op = GetComparisonOperator(expr);
        }

        public static string GetComparisonOperator(BinaryExpression expr)
        {
            var type = expr.NodeType;
            return GetComparisonType(type);
        }

        public static string GetComparisonType(ExpressionType type)
        {
            return type switch
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

        private static string GetCalculationType(ExpressionType nodeType)
        {
            return nodeType switch
            {
                ExpressionType.Add => "$add",
                ExpressionType.Subtract => "$subtract",
                ExpressionType.Multiply => "$multiply",
                ExpressionType.Divide => "$divide",
                ExpressionType.Modulo => "$mod",
                _ => throw new NotSupportedException($"Unsupported calculation operation: {nodeType}")
            };
        }
    }
}
