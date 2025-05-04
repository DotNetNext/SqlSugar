using Newtonsoft.Json.Linq;
using SqlSugar.MongoDbCore.ExpToSql.VisitorItems;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDbCore 
{
    public static class ExpressionVisitor
    {
        public static JToken Visit(Expression expr)
        {
            expr = MongoDbExpTools.RemoveConvert(expr);
            switch (expr)
            {
                case BinaryExpression binary:
                    return BinaryExpressionTranslator.Translate(binary);
                case MemberExpression member:
                    return FieldPathExtractor.GetFieldPath(member);
                case ConstantExpression constant:
                    return ValueExtractor.GetValue(constant);
                case UnaryExpression unary:
                    return Visit(unary);
                case LambdaExpression lambda:
                    return Visit((lambda as LambdaExpression).Body);
                default:
                    throw new NotSupportedException($"Unsupported expression: {expr.NodeType}");
            }
        }
    }

}
