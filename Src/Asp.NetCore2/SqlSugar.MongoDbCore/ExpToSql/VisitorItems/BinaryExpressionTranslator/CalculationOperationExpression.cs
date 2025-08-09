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
        private BsonDocument GetCalculationOperation(BsonValue field, ExpressionType nodeType, BsonValue value, bool leftIsMember, bool rightIsMember)
        {
            string operation = GetCalculationType(nodeType);
            if (!leftIsMember && rightIsMember&& IsStringAdd(field, operation)) 
                return StringAddCalculation(value, field, leftIsMember, rightIsMember, out operation);
            else if (!rightIsMember&&leftIsMember&&IsStringAdd(value, operation))
                return StringAddCalculation(field, value, leftIsMember, rightIsMember, out operation);
            else
                return GetCommonCalculation(field, value, operation,leftIsMember,rightIsMember);
        } 
        private  BsonDocument GetCommonCalculation(BsonValue field, BsonValue value, string operation, bool leftIsMember, bool rightIsMember)
        {
            if (_visitorContext?.IsText == true)
            {
                var leftValue = field;
                var rightValue = value;
                if (leftIsMember)
                {
                    leftValue = UtilMethods.GetMemberName(leftValue);
                }
                if (rightIsMember)
                {
                    rightValue = UtilMethods.GetMemberName(rightValue);
                }
                return new BsonDocument
                {
                    { operation, new BsonArray { leftValue, rightValue } }
                };
            }
            else if (operation == "$add") 
            {
                return new BsonDocument
                {
                   {operation, new BsonArray {UtilMethods.GetMemberName(field.ToString()), value } }
                };
            }
            else
            {
                return new BsonDocument
                {
                   { field.ToString(), new BsonDocument { { operation, value } } }
                };
            }
        }

        private static BsonDocument StringAddCalculation(BsonValue field, BsonValue value, bool leftIsMember, bool rightIsMember, out string operation)
        {
            operation = "$concat";
            return new BsonDocument
                {
                    { operation, new BsonArray { UtilMethods.GetBsonValue(leftIsMember, field), UtilMethods.GetBsonValue(rightIsMember, value) } }
                };
            ;
        } 
        private static bool IsStringAdd(BsonValue value, string operation)
        {
            return operation == "$add" && value.BsonType == BsonType.String;
        }
    }
}
