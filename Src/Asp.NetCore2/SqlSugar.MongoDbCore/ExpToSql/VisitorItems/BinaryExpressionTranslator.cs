using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDbCore
{
    public class BinaryExpressionTranslator
    {
        MongoNestedTranslatorContext _context;

        public BinaryExpressionTranslator(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            _context = context;
        }

        public BsonDocument Extract(BinaryExpression expr)
        {
            if (expr.NodeType == ExpressionType.AndAlso || expr.NodeType == ExpressionType.OrElse)
            {
                return LogicalBinaryExpression(expr);
            }

            return FieldComparisonExpression(expr);
        }

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

        private BsonDocument FieldComparisonExpression(BinaryExpression expr)
        {
            BsonValue field, value;
            bool leftIsMember, rightIsMember;
            string op;
            OutParameters(expr, out field, out value, out leftIsMember, out rightIsMember, out op);
            if (op == null)
            {
                return GetCalculationOperation(field, expr.NodeType, value);
            }
            else
            {
                return GetComparisonOperation(expr, field, value, leftIsMember, rightIsMember, op);
            }
        }

        private void OutParameters(BinaryExpression expr, out BsonValue field, out BsonValue value, out bool leftIsMember, out bool rightIsMember, out string op)
        {
            var left = new ExpressionVisitor(_context, new ExpressionVisitorContext());
            var right = new ExpressionVisitor(_context, new ExpressionVisitorContext());
            field = left.Visit(expr.Left);
            value = right.Visit(expr.Right);
            leftIsMember = false;
            rightIsMember = false;
            if (left?.visitorContext?.ExpType == typeof(MemberExpression))
            {
                leftIsMember = true;
            }

            if (right?.visitorContext?.ExpType == typeof(MemberExpression))
            {
                rightIsMember = true;
            }

            op = expr.NodeType switch
            {
                ExpressionType.Equal => "$eq",
                ExpressionType.NotEqual => "$ne",
                ExpressionType.GreaterThan => "$gt",
                ExpressionType.GreaterThanOrEqual => "$gte",
                ExpressionType.LessThan => "$lt",
                ExpressionType.LessThanOrEqual => "$lte",
                _ => null
            };
        }

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
                            var columnInfo = entityInfo.Columns.FirstOrDefault(s => s.PropertyName == leftValue);
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
                return new BsonDocument { { leftValue, rightValue } };
            }
            else
            {
                return new BsonDocument
                    {
                        { field.ToString(), new BsonDocument { { op, value } } }
                    };
            }
        }

        private BsonDocument GetCalculationOperation(BsonValue field, ExpressionType nodeType, BsonValue value)
        {
            string operation = nodeType switch
            {
                ExpressionType.Add => "$add",
                ExpressionType.Subtract => "$subtract",
                ExpressionType.Multiply => "$multiply",
                ExpressionType.Divide => "$divide",
                ExpressionType.Modulo => "$mod",
                _ => throw new NotSupportedException($"Unsupported calculation operation: {nodeType}")
            };

            return new BsonDocument
    {
        { field.ToString(), new BsonDocument { { operation, value } } }
    };
        }

        private static bool IsRightValue(bool leftIsMember, bool rightIsMember, string op)
        {
            return  rightIsMember && !leftIsMember;
        }

        private static bool IsLeftValue(bool leftIsMember, bool rightIsMember, string op)
        {
            return  leftIsMember && !rightIsMember;
        }
    }

}
