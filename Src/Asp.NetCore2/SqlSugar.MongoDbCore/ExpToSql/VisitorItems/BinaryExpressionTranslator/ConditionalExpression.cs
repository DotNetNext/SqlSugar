using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public class ConditionalExpressionTractor
    {
        private MongoNestedTranslatorContext context;
        private ExpressionVisitorContext visitorContext;

        public ConditionalExpressionTractor(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            this.context = context;
            this.visitorContext = visitorContext;
        }

        internal BsonValue Extract(Expression expr)
        {
            var exp = expr as ConditionalExpression;
            if (exp == null)
            {
                throw new ArgumentException("表达式不是ConditionalExpression", nameof(expr));
            }
            if (!ExpressionTool.GetParameters(exp.Test).Any())
            {
                var isOk = (bool)ExpressionTool.DynamicInvoke(exp.Test);
                var tranExp = isOk ? exp.IfTrue : exp.IfFalse;
                return MongoNestedTranslator.Translate(tranExp, context);
            }
            else
            {
                var testValue = MongoNestedTranslator.TranslateNoFieldName(exp.Test, context, new ExpressionVisitorContext() { IsText=true });
                var ifTrueValue = MongoNestedTranslator.TranslateNoFieldName(exp.IfTrue, context, new ExpressionVisitorContext() { IsText = true });
                var ifFalseValue = MongoNestedTranslator.TranslateNoFieldName(exp.IfFalse, context, new ExpressionVisitorContext() { IsText = true });
                if (MongoDbExpTools.GetIsMemember(exp.IfTrue)) 
                {
                    ifTrueValue = UtilMethods.GetMemberName(ifTrueValue);
                }
                if (MongoDbExpTools.GetIsMemember(exp.IfFalse))
                {
                    ifFalseValue = UtilMethods.GetMemberName(ifFalseValue);
                }
                // MongoDB的$cond操作符
                var condDoc = new BsonDocument
                {
                    { "$cond", new BsonArray { testValue,ifTrueValue, ifFalseValue } }
                };
                return condDoc;
            }
        }
    }
}
