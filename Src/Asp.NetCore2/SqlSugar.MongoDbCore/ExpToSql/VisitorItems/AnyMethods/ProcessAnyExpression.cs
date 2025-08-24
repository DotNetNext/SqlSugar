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
        private BsonValue ProcessAnyExpression(MethodCallExpression methodCallExpression)
        {
            // 处理 it.xx.Any(s => s.id == 1) 这种表达式
            // memberExpression: it.xx
            // whereExpression: s => s.id == 1
            var memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
            var whereExpression = methodCallExpression.Arguments[1];

            var isAnyAny = whereExpression is LambdaExpression l && l.Body is MethodCallExpression call && call.Method.Name == "Any" && ExpressionTool.GetParameters(call).Count() > 0;

            if (isAnyAny)
            {
                return CreateAnyAnyExpression(memberExpression, whereExpression);
            }
            else
            {
                // 获取集合字段名
                var collectionField = MongoNestedTranslator.TranslateNoFieldName(
                    memberExpression,
                    _context,
                    new ExpressionVisitorContext { IsText = true }
                );

                // 生成 $elemMatch 查询
                var elemMatch = MongoNestedTranslator.TranslateNoFieldName(
                    whereExpression,
                    _context,
                    new ExpressionVisitorContext { IsText = true }
                );

                var bson = new BsonDocument
                        {
                            { collectionField?.ToString(), new BsonDocument("$elemMatch", elemMatch) }
                        };
                return bson;
            }
        }

        private BsonValue CreateAnyAnyExpression(MemberExpression memberExpression, Expression whereExpression)
        {
            // 获取集合字段名
            var collectionField2 = MongoNestedTranslator.TranslateNoFieldName(
                memberExpression,
                _context,
                new ExpressionVisitorContext { IsText = true }
            )?.ToString();

            // 获取 where lambda
            var lambda = (LambdaExpression)whereExpression;
            var paramName = lambda.Parameters[0].Name;

            // 这里假设嵌套集合字段为 book，实际应根据表达式动态获取
            // 生成 $expr 查询，增加 $ifNull 逻辑
            var expr = new BsonDocument
                {
                    {
                        "$expr", new BsonDocument
                        {
                            {
                                "$gt", new BsonArray
                                {
                                    new BsonDocument
                                    {
                                        {
                                            "$size", new BsonDocument
                                            {
                                                {
                                                    "$filter", new BsonDocument
                                                    {
                                                        { "input", $"${collectionField2}" },
                                                        { "as", paramName },
                                                        { "cond", new BsonDocument
                                                            {
                                                                { "$gt", new BsonArray
                                                                    {
                                                                        new BsonDocument
                                                                        {
                                                                            { "$size", new BsonDocument
                                                                                {
                                                                                    { "$ifNull", new BsonArray
                                                                                        {
                                                                                            $"$${paramName}.book",
                                                                                            new BsonArray()
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        },
                                                                        0
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    0
                                }
                            }
                        }
                    }
                };
            return expr;
        }
    }
}
