using Newtonsoft.Json.Linq;
using SqlSugar.MongoDbCore.ExpToSql.VisitorItems;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDbCore 
{
    public  class ExpressionVisitor
    {
        private MongoNestedTranslatorContext context;

        public ExpressionVisitor(MongoNestedTranslatorContext context)
        {
            this.context = context;
        }

        public  JToken Visit(Expression expr)
        {
            expr = MongoDbExpTools.RemoveConvert(expr);
            switch (expr)
            {
                case BinaryExpression binary:
                    return new BinaryExpressionTranslator(context).Extract(binary);
                case MemberExpression member:
                    return new FieldPathExtractor(context).Extract(member);
                case ConstantExpression constant:
                    return new ValueExtractor(context).Extract(constant);
                case UnaryExpression unary:
                    return this.Visit(unary);
                case LambdaExpression lambda:
                    return this.Visit((lambda as LambdaExpression).Body);
                default:
                    throw new NotSupportedException($"Unsupported expression: {expr.NodeType}");
            }
        }
    }

}
