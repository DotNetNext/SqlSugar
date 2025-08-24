using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public partial class MethodCallExpressionTractor
    {

        private BsonValue HandleComplexAnyExpression(MethodCallExpression methodCallExpression)
        {
            // 处理 it.Book.Any(s => s.Price == it.Age) 这种主表字段关联的 Any 表达式
            // 参数1: 集合字段 it.Book
            // 参数2: Lambda 表达式 s => s.Price == it.Age

            var memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
            var lambdaExpression = methodCallExpression.Arguments[1] as LambdaExpression;
            var firstParameterName = lambdaExpression.Parameters.FirstOrDefault().Name;

            // 获取集合字段名
            var collectionField = MongoNestedTranslator.TranslateNoFieldName(
                memberExpression,
                _context,
                new ExpressionVisitorContext { IsText = true }
            )?.ToString();

            // 处理 Lambda 表达式体
            var body = lambdaExpression.Body;

            // 支持多种比较操作
            if (body is BinaryExpression binaryExpr)
            {
                // 左右表达式
                var left = binaryExpr.Left;
                var right = binaryExpr.Right;
                if (ExpressionTool.GetParameters(right).Any(s => s.Name == firstParameterName))
                {
                    left = binaryExpr.Right;
                    right = binaryExpr.Left;
                }
                string leftField = MongoNestedTranslator.TranslateNoFieldName(left, _context, new ExpressionVisitorContext { IsText = true })?.ToString();
                string rightField = MongoNestedTranslator.TranslateNoFieldName(right, _context, new ExpressionVisitorContext { IsText = true })?.ToString();

                // 映射表达式类型到Mongo操作符
                string mongoOperator = binaryExpr.NodeType switch
                {
                    ExpressionType.Equal => "$eq",
                    ExpressionType.NotEqual => "$ne",
                    ExpressionType.GreaterThan => "$gt",
                    ExpressionType.GreaterThanOrEqual => "$gte",
                    ExpressionType.LessThan => "$lt",
                    ExpressionType.LessThanOrEqual => "$lte",
                    _ => null
                };

                if (mongoOperator != null)
                {
                    var mapDoc = new BsonDocument
                    {
                        { "input", $"${collectionField}" },
                        { "as", "b" },
                        { "in", new BsonDocument(mongoOperator, new BsonArray { $"$$b.{leftField}", $"${rightField}" }) }
                    };
                    var anyElementTrueDoc = new BsonDocument("$expr", new BsonDocument("$anyElementTrue", new BsonDocument("$map", mapDoc)));
                    return anyElementTrueDoc;
                }
            }
            return null;
        }

    }
}
