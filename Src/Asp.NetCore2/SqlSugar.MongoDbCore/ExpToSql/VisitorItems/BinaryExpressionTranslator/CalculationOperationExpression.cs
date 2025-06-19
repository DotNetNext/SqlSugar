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
            if (IsStringAdd(value, operation))
                return StringAddCalculation(field, value, leftIsMember, rightIsMember, out operation);
            else
                return GetCommonCalculation(field, value, operation);
        } 
        private  BsonDocument GetCommonCalculation(BsonValue field, BsonValue value, string operation)
        {
            if (_visitorContext?.IsText == true)
            {
                // 三元条件格式: { "$gt": ["$Age", 0] }
                return new BsonDocument
                {
                    { operation, new BsonArray { "$" + field, value } }
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
