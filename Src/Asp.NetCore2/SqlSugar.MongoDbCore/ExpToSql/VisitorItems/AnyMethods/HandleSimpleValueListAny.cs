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

        private BsonValue HandleSimpleValueListAny(MethodCallExpression methodCallExpression)
        {
            var memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
            var lambdaExpression = methodCallExpression.Arguments[1] as LambdaExpression;
            var paramName = lambdaExpression.Parameters.FirstOrDefault()?.Name;

            // 集合字段名
            var collectionField = MongoNestedTranslator.TranslateNoFieldName(
                memberExpression,
                _context,
                new ExpressionVisitorContext { IsText = true }
            )?.ToString();

            // Lambda 表达式体
            var body = lambdaExpression.Body;
            if (body is BinaryExpression binaryExpr)
            {
                // 只处理 s == value 这种简单表达式
                string mongoOperator = binaryExpr.NodeType switch
                {
                    ExpressionType.Equal => "$in",
                    ExpressionType.NotEqual => "$nin",
                    _ => null
                };
                if (mongoOperator != null)
                {
                    // s == value，左边是参数，右边是常量
                    var left = binaryExpr.Left;
                    var right = binaryExpr.Right;
                    if (left is ParameterExpression && right is ConstantExpression constant)
                    {
                        var value = UtilMethods.MyCreate(constant.Value);
                        var bson = new BsonDocument
                                    {
                                        { collectionField, new BsonDocument(mongoOperator, new BsonArray { value }) }
                                    };
                        return bson;
                    }
                    // s == it.xx 这种情况
                    if (left is ParameterExpression && right != null)
                    {
                        var valueExpr = MongoNestedTranslator.TranslateNoFieldName(
                            right,
                            _context,
                            new ExpressionVisitorContext { IsText = true }
                        );
                        var bson = new BsonDocument
                                    {
                                        { collectionField, new BsonDocument(mongoOperator, new BsonArray { valueExpr }) }
                                    };
                        return bson;
                    }
                }
            }
            return null;
        }

    }
}
