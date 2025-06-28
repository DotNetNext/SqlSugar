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
            if (expr.NodeType == ExpressionType.Negate) 
            {
                expr=(expr as UnaryExpression).Operand; 
                var value=new ExpressionVisitor(context,visitorContext).Visit(expr);
                var isMemember = MongoDbExpTools.GetIsMemember(expr);
                if (isMemember)
                    value = $"${value}";
                return new BsonDocument("$multiply", new BsonArray { -1, value });
            }
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
    } 
}
