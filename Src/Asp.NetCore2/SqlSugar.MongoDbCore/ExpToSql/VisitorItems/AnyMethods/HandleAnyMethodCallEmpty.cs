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
        private BsonValue HandleAnyMethodCallEmpty(MethodCallExpression methodCallExpression, BsonValue result)
        {
            // 处理 it.xx.Any()，即判断 JSON 数组字段是否非空
            var memberExpression = methodCallExpression.Arguments.FirstOrDefault() as MemberExpression;
            if (memberExpression != null)
            {
                // 获取集合字段名
                var collectionField = MongoNestedTranslator.TranslateNoFieldName(
                    memberExpression,
                    _context,
                    new ExpressionVisitorContext { IsText = true }
                );

                // 构造 $size > 0 查询
                var bson = new BsonDocument
                            {
                                { "$expr", new BsonDocument("$gt", new BsonArray { new BsonDocument("$size", UtilMethods.GetMemberName(collectionField)), 0 }) }
                            };
                result = bson;
            }

            return result;
        }

    }
}
