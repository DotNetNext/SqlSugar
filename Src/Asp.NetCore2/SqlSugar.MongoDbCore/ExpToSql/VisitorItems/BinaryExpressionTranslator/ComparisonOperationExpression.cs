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
            string leftValue = "";
            BsonValue rightValue = "";
            if (IsLeftValue(leftIsMember, rightIsMember, op) || IsRightValue(leftIsMember, rightIsMember, op))
            {
                MemberExpression expression;
                if (IsLeftValue(leftIsMember, rightIsMember, op))
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
            else
            {
                return new BsonDocument
                    {
                        { field.ToString(), new BsonDocument { { op, value } } }
                    };
            }
        }

    }
}
