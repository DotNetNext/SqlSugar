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
        private BsonValue CallAnyExpression(MethodCallExpression methodCall, AnyArgModel anyArgModel)
        {
            var nodeType = anyArgModel.NodeType;
            // 获取集合字段名
            var collectionField = MongoNestedTranslator.TranslateNoFieldName(
                methodCall.Arguments[0],
                _context,
                new ExpressionVisitorContext { IsText = true }
            )?.ToString();

            var leftExpr = anyArgModel.Left as MethodCallExpression;
            var rightExpr = anyArgModel.Right;

            // 处理左边表达式（如 $toString: "$$b.Age"）
            BsonValue leftValue;
            if (leftExpr != null)
            {
                leftValue = MongoNestedTranslator.TranslateNoFieldName(
                    leftExpr,
                    _context,
                    new ExpressionVisitorContext { IsText = true }
                );
            }
            else
            {
                leftValue = MongoNestedTranslator.TranslateNoFieldName(
                    anyArgModel.Left,
                    _context,
                    new ExpressionVisitorContext { IsText = true }
                );
            }

            // 处理右边表达式（如常量 "99"）
            BsonValue rightValue;
            if (rightExpr is ConstantExpression constantExpr)
            {
                rightValue = UtilMethods.MyCreate(constantExpr.Value);
            }
            else
            {
                rightValue = MongoNestedTranslator.TranslateNoFieldName(
                    rightExpr,
                    _context,
                    new ExpressionVisitorContext { IsText = true }
                );
            }

            // 映射表达式类型到Mongo操作符
            string mongoOperator = nodeType switch
            {
                ExpressionType.Equal => "$eq",
                ExpressionType.NotEqual => "$ne",
                ExpressionType.GreaterThan => "$gt",
                ExpressionType.GreaterThanOrEqual => "$gte",
                ExpressionType.LessThan => "$lt",
                ExpressionType.LessThanOrEqual => "$lte",
                _ => "$eq"
            };

            if (leftExpr.Arguments.Any() && leftExpr.Arguments[0] is MemberExpression member && ExpressionTool.GetParameters(member).Count() > 0)
            {
                var leftMember = MongoNestedTranslator.TranslateNoFieldName(
                       member,
                       _context,
                       new ExpressionVisitorContext { IsText = true }
                   );
                var leftMemberStr = UtilMethods.GetMemberName(leftMember.ToString()).ToString();
                var newValue = "$$b." + leftMember.ToString();
                leftValue = leftValue?.ToString().Replace(leftMemberStr, newValue);
                leftValue = UtilMethods.ParseJsonObject(leftValue);
            }
            // 构造 $map
            var mapDoc = new BsonDocument
            {
                { "input", $"${collectionField}" },
                { "as", "b" },
                { "in", new BsonDocument(mongoOperator, new BsonArray { leftValue, rightValue }) }
            };

            // 构造 $expr/$anyElementTrue
            var exprDoc = new BsonDocument
            {
                { "$anyElementTrue", new BsonDocument("$map", mapDoc) }
            };

            var bson = new BsonDocument
            {
                { "$expr", exprDoc }
            };

            return bson;
        }

    }
}
