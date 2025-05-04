using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDbCore.ExpToSql.VisitorItems 
{
    public  class BinaryExpressionTranslator
    {
        MongoNestedTranslatorContext _context;

        public BinaryExpressionTranslator(MongoNestedTranslatorContext context)
        {
            _context = context;
        }

        public  JToken Extract(BinaryExpression expr)
        {
            if (expr.NodeType == ExpressionType.AndAlso || expr.NodeType == ExpressionType.OrElse)
            {
                return LogicalBinaryExpression(expr);
            }

            return FieldComparisonExpression(expr);
        }

        private  JToken LogicalBinaryExpression(BinaryExpression expr)
        {
            string logicOp = expr.NodeType == ExpressionType.AndAlso ? "$and" : "$or";

            var left = new ExpressionVisitor(_context).Visit(expr.Left);
            var right =new ExpressionVisitor(_context).Visit(expr.Right);

            var arr = new JArray();
            AddNestedLogic(arr, left, logicOp);
            AddNestedLogic(arr, right, logicOp);

            return new JObject { [logicOp] = arr };
        }

        private  void AddNestedLogic(JArray arr, JToken token, string logicOp)
        {
            if (token is JObject obj && obj.TryGetValue(logicOp, out var nested) && nested is JArray nestedArr)
            {
                arr.Merge(nestedArr);
            }
            else
            {
                arr.Add(token);
            }
        }

        private  JToken FieldComparisonExpression(BinaryExpression expr)
        {
            string field = new FieldPathExtractor(_context).Extract(expr.Left);
            JToken value = new ExpressionVisitor(_context).Visit(expr.Right);

            string op = expr.NodeType switch
            {
                ExpressionType.Equal => value.Type == JTokenType.Null ? "$eq" : null,
                ExpressionType.NotEqual => value.Type == JTokenType.Null ? "$ne" : "$ne",
                ExpressionType.GreaterThan => "$gt",
                ExpressionType.GreaterThanOrEqual => "$gte",
                ExpressionType.LessThan => "$lt",
                ExpressionType.LessThanOrEqual => "$lte",
                _ => throw new NotSupportedException($"Unsupported binary op: {expr.NodeType}")
            };

            if (op == null)
                return new JObject { [field] = value };

            return new JObject
            {
                [field] = new JObject { [op] = value }
            };
        }
    }

}
