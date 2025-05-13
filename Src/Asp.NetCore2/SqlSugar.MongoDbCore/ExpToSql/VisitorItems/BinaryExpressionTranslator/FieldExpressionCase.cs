using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb
{
    public partial class BinaryExpressionTranslator
    {
        private BsonDocument FieldComparisonOrCalculationExpression(BinaryExpression expr)
        {
            OutParameters(expr, out var field, out var value, out var leftIsMember, out var rightIsMember, out var op);
            return op == null
                ? GetCalculationOperation(field, expr.NodeType, value, leftIsMember, rightIsMember)
                : GetComparisonOperation(expr, field, value, leftIsMember, rightIsMember, op);
        }
    }
}
