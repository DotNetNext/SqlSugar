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

            // 从表达式中动态获取嵌套集合字段名
            string nestedCollectionField = null;
            if (lambda.Body is MethodCallExpression innerAnyCall)
            {
                if (innerAnyCall.Arguments.Count > 0 && innerAnyCall.Arguments[0] is MemberExpression nestedMember)
                {
                    nestedCollectionField = nestedMember.Member.Name;
                }
                else if (innerAnyCall.Object is MemberExpression nestedMemberObj)
                {
                    nestedCollectionField = nestedMemberObj.Member.Name;
                }
            }

            // fallback
            if (string.IsNullOrEmpty(nestedCollectionField))
            {
                throw new Exception("Expressions are not supported." + lambda.ToString());
            }

            // 生成 $expr 查询，兼容 null 和空集合
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
                                                            { "$and", new BsonArray
                                                                {
                                                                    new BsonDocument
                                                                    {
                                                                        { "$isArray", $"$${paramName}.{nestedCollectionField}" }
                                                                    },
                                                                    new BsonDocument
                                                                    {
                                                                        { "$gt", new BsonArray
                                                                            {
                                                                                new BsonDocument
                                                                                {
                                                                                    { "$size", new BsonDocument
                                                                                        {
                                                                                            { "$ifNull", new BsonArray
                                                                                                {
                                                                                                    $"$${paramName}.{nestedCollectionField}",
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
