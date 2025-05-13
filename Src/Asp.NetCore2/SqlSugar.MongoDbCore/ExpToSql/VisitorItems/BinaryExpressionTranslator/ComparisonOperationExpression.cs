﻿using Dm;
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
            string leftValue = isLeftValue ? field.ToString() : value.ToString();
            BsonValue rightValue = isLeftValue ?   value: field;
            var expression = isLeftValue ? expr.Left as MemberExpression : expr.Right as MemberExpression;
            EntityColumnInfo CurrentColumnInfo = null;
            leftValue = GetLeftValue(leftValue, expression, ref CurrentColumnInfo);
            rightValue = GetRightValue(CurrentColumnInfo, rightValue);
            if (IsEq(op)) 
                return GetEqResult(leftValue, rightValue); 
            else 
                return GetOtherResult(op, leftValue, rightValue); 
        }

        private static BsonDocument GetOtherResult(string op, string leftValue, BsonValue rightValue)
        {
            return new BsonDocument
                    {
                        { leftValue, new BsonDocument { { op, rightValue } } }
                    };
        }

        private static BsonDocument GetEqResult(string leftValue, BsonValue rightValue)
        {
            return new BsonDocument { { leftValue, rightValue } };
        }

        private string GetLeftValue(string leftValue, MemberExpression expression, ref EntityColumnInfo CurrentColumnInfo)
        {
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
                            CurrentColumnInfo = columnInfo;
                        }
                    }
                }
            }
            return leftValue;
        }

        private BsonValue GetRightValue(EntityColumnInfo  entityColumnInfo, BsonValue rightValue)
        {
            if (entityColumnInfo?.IsPrimarykey==true) 
            {
                rightValue=ObjectId.Parse(rightValue?.ToString());
            }
            return rightValue;
        }
    }
}
