using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public partial class BinaryExpressionTranslator: BaseCommonExpression
    { 
      private BsonDocument LogicalBinaryExpression(BinaryExpression expr)
        {
            string logicOp = expr.NodeType == ExpressionType.AndAlso ? "$and" : "$or";
            var leftExp = AdjustExpression(expr.Left);
            var left = new ExpressionVisitor(_context).Visit(leftExp);
            var rightExp = AdjustExpression(expr.Right);
            var right = new ExpressionVisitor(_context).Visit(rightExp);
            var isJoinByExp=base.IsJoinByExp(_context);
            if (isJoinByExp)
            {
                if (left is BsonDocument leftDoc && leftDoc.Contains("$expr"))
                {
                    left = leftDoc["$expr"];
                }
                if (right is BsonDocument rightDoc && rightDoc.Contains("$expr"))
                {
                    right = rightDoc["$expr"];
                }
            }
            var arr = new BsonArray();
            AddNestedLogic(arr, left, logicOp);
            AddNestedLogic(arr, right, logicOp);

            return new BsonDocument { { logicOp, arr } };
        }

        private static Expression AdjustExpression(Expression exp)
        {
            if (exp is MemberExpression m && m.Type == UtilConstants.BoolType && ExpressionTool.GetParameters(exp).Count > 0)
            { 
                exp = Expression.Equal(exp, Expression.Constant(true, typeof(bool)));
            } 
            return exp;
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

    }
}
