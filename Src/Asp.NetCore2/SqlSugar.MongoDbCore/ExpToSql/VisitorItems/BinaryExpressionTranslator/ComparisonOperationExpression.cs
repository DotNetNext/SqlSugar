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
        private BsonDocument GetComparisonOperation(BinaryExpression expr, BsonValue field, BsonValue value, bool leftIsMember, bool rightIsMember, string op)
        {
            var isLeftValue = IsLeftValue(leftIsMember, rightIsMember, op);
            var isRightValue = IsRightValue(leftIsMember, rightIsMember, op);
            var isKeyValue = isLeftValue || isRightValue;
            if (isKeyValue)
                return ComparisonKeyValue(expr, field, value, op, isLeftValue);
            else
                return ComparisonNotKeyValue(field, value, op);
        }

        private static BsonDocument ComparisonNotKeyValue(BsonValue field, BsonValue value, string op)
        {
            var leftKey = field.ToString();
            return new BsonDocument
                   {
                       { leftKey, new BsonDocument { { op, value } } }
                   };
        }

        private BsonDocument ComparisonKeyValue(BinaryExpression expr, BsonValue field, BsonValue value, string op, bool isLeftValue)
        {
            string leftValue = string.Empty;
            BsonValue rightValue = null;
            MemberExpression expression;
            if (isLeftValue)
            {
                leftValue = field.ToString();
                rightValue = value;
                expression = expr.Left as MemberExpression;
            }
            else
            {
                leftValue = value.ToString();
                rightValue = field;
                expression = expr.Right as MemberExpression;
            }
            if (expression != null)
            {
                if (expression.Expression is ParameterExpression parameter)
                {
                    if (_context?.context != null)
                    {
                        var entityInfo = _context.context.EntityMaintenance.GetEntityInfo(parameter.Type);
                        var columnInfo = entityInfo.Columns.FirstOrDefault(s => s.PropertyName == leftValue || s.DbColumnName == leftValue);
                        if (columnInfo != null)
                        {
                            leftValue = columnInfo.DbColumnName;
                            if (columnInfo.IsPrimarykey)
                            {
                                rightValue = BsonValue.Create(ObjectId.Parse(value + ""));
                            }
                        }
                    }
                }
            }
            if (op == "$eq")
            {
                return new BsonDocument { { leftValue, rightValue } };
            }
            else
            {
                return new BsonDocument
                    {
                        { leftValue, new BsonDocument { { op, rightValue } } }
                    };
            }
        }
    }
}
