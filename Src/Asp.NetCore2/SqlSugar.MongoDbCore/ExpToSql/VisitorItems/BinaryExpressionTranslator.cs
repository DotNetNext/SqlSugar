using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq; 
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDbCore
{
    public class BinaryExpressionTranslator
    {
        MongoNestedTranslatorContext _context;

        public BinaryExpressionTranslator(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            _context = context;
        }

        public BsonDocument Extract(BinaryExpression expr)
        {
            if (expr.NodeType == ExpressionType.AndAlso || expr.NodeType == ExpressionType.OrElse)
            {
                return LogicalBinaryExpression(expr);
            }

            return FieldComparisonExpression(expr);
        }

        private BsonDocument LogicalBinaryExpression(BinaryExpression expr)
        {
            string logicOp = expr.NodeType == ExpressionType.AndAlso ? "$and" : "$or";

            var left = new ExpressionVisitor(_context).Visit(expr.Left);
            var right = new ExpressionVisitor(_context).Visit(expr.Right);

            var arr = new BsonArray();
            AddNestedLogic(arr, left, logicOp);
            AddNestedLogic(arr, right, logicOp);

            return new BsonDocument { { logicOp, arr } };
        }

        private void AddNestedLogic(BsonArray arr, BsonValue token, string logicOp)
        {
            if (token is BsonDocument obj && obj.Contains(logicOp))
            {
                var nestedArr = obj[logicOp].AsBsonArray;
                arr.AddRange(nestedArr);
            }
            else
            {
                arr.Add(token);
            }
        }

        private BsonDocument FieldComparisonExpression(BinaryExpression expr)
        {
            var left = new ExpressionVisitor(_context, new ExpressionVisitorContext());
            var right = new ExpressionVisitor(_context, new ExpressionVisitorContext());
            BsonValue field = left.Visit(expr.Left);
            BsonValue value = right.Visit(expr.Right);
            bool leftIsMember = false;
            bool rightIsMember = false;

            if (left?.visitorContext?.ExpType == typeof(MemberExpression))
            {
                leftIsMember = true;
            }

            if (right?.visitorContext?.ExpType == typeof(MemberExpression))
            {
                rightIsMember = true;
            }

            string op = expr.NodeType switch
            {
                ExpressionType.Equal => value.IsBsonNull ? "$eq" : null,
                ExpressionType.NotEqual => value.IsBsonNull ? "$ne" : "$ne",
                ExpressionType.GreaterThan => "$gt",
                ExpressionType.GreaterThanOrEqual => "$gte",
                ExpressionType.LessThan => "$lt",
                ExpressionType.LessThanOrEqual => "$lte",
                _ => throw new NotSupportedException($"Unsupported binary op: {expr.NodeType}")
            };

            if (op == null && leftIsMember && !rightIsMember)
                return new BsonDocument { { field.ToString(), value } };
            else if (op == null && rightIsMember && !leftIsMember)
                return new BsonDocument { { value.ToString(), field } };

            return new BsonDocument
        {
            { field.ToString(), new BsonDocument { { op, value } } }
        };
        }
    }

}
