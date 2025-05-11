using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public partial class BinaryExpressionTranslator
    { 
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

    }
}
