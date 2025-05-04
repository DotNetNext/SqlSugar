using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDbCore.ExpToSql.VisitorItems 
{
    public static class BinaryExpressionTranslator
    {
        public static JToken Translate(BinaryExpression expr)
        {
            if (expr.NodeType == ExpressionType.AndAlso || expr.NodeType == ExpressionType.OrElse)
            {
                return LogicalBinaryExpression(expr);
            }

            return FieldComparisonExpression(expr);
        }

        private static JToken LogicalBinaryExpression(BinaryExpression expr)
        {
            string logicOp = expr.NodeType == ExpressionType.AndAlso ? "$and" : "$or";

            var left = ExpressionVisitor.Visit(expr.Left);
            var right = ExpressionVisitor.Visit(expr.Right);

            var arr = new JArray();
            AddNestedLogic(arr, left, logicOp);
            AddNestedLogic(arr, right, logicOp);

            return new JObject { [logicOp] = arr };
        }

        private static void AddNestedLogic(JArray arr, JToken token, string logicOp)
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

        private static JToken FieldComparisonExpression(BinaryExpression expr)
        {
            string field = FieldPathExtractor.GetFieldPath(expr.Left);
            JToken value = ExpressionVisitor.Visit(expr.Right);

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
