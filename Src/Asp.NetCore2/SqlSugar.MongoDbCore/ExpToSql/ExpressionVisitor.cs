using MongoDB.Bson;
using Newtonsoft.Json.Linq;  
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public class ExpressionVisitor
    {
        private MongoNestedTranslatorContext context;
        public ExpressionVisitorContext visitorContext;

        public ExpressionVisitor(MongoNestedTranslatorContext context, ExpressionVisitorContext expressionVisitorContext)
        {
            this.context = context;
            this.visitorContext = expressionVisitorContext;
        }

        public ExpressionVisitor(MongoNestedTranslatorContext context)
        {
            this.context = context;
        }

        public BsonValue Visit(Expression expr)
        {
            expr = MongoDbExpTools.RemoveConvert(expr);

            if (IsNegateExpression(expr))
                return NegateExpressionHandler(ref expr);
            else if (IsNotExpression(expr))
                return NotExpressionHandler(expr);

            switch (expr)
            {
                case BinaryExpression binary:
                    return new BinaryExpressionTranslator(context, visitorContext).Extract(binary);
                case MemberExpression member:
                    return new FieldPathExtractor(context, visitorContext).Extract(member);
                case ConstantExpression constant:
                    return new ValueExtractor(context, visitorContext).Extract(constant);
                case UnaryExpression unary:
                    return this.Visit(unary);
                case LambdaExpression lambda:
                    return this.Visit(lambda.Body);
                case MethodCallExpression call:
                    return new MethodCallExpressionTractor(context, visitorContext).Extract(call);
                case MemberInitExpression newMemberExp:
                    return new MemberInitExpressionTractor(context, visitorContext).Extract(newMemberExp);
                case NewExpression newNewExpression:
                    return new NewExpressionTractor(context, visitorContext).Extract(newNewExpression);
                case ConditionalExpression conditionalExpression:
                    return new ConditionalExpressionTractor(context, visitorContext).Extract(conditionalExpression);
                case ParameterExpression parameterExpression:
                    return new ParameterExpressionTractor(context, visitorContext).Extract(parameterExpression);
                default:
                    throw new NotSupportedException($"Unsupported expression: {expr.NodeType}");
            }
        }

        private BsonValue NotExpressionHandler(Expression expr)
        {
            // 处理一元Not表达式
            var operand = (expr as UnaryExpression).Operand;
            var value = new ExpressionVisitor(context, visitorContext).Visit(operand);
            // 如果是布尔类型成员，直接取反
            if (value is BsonDocument doc && doc.ElementCount == 1)
            {
                // 处理如 { field: value } 变为 { field: { $not: value } }
                var element = doc.GetElement(0);
                return new BsonDocument(element.Name, new BsonDocument("$not", element.Value));
            }
            else if (value is BsonBoolean booleanValue)
            {
                // 直接对常量布尔值取反
                return new BsonBoolean(!booleanValue.Value);
            }
            else
            {
                // 其他情况用$not包裹
                return new BsonDocument("$not", value);
            }
        }

        private static bool IsNotExpression(Expression expr)
        {
            return expr.NodeType == ExpressionType.Not;
        }

        private BsonValue NegateExpressionHandler(ref Expression expr)
        {
            expr = (expr as UnaryExpression).Operand;
            var value = new ExpressionVisitor(context, visitorContext).Visit(expr);
            var isMemember = MongoDbExpTools.GetIsMemember(expr);
            if (isMemember)
                value = UtilMethods.GetMemberName(value);
            return new BsonDocument("$multiply", new BsonArray { -1, value });
        }

        private static bool IsNegateExpression(Expression expr)
        {
            return expr.NodeType == ExpressionType.Negate;
        }
    } 
}
