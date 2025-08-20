using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public partial class BinaryExpressionTranslator
    {
        MongoNestedTranslatorContext _context;
        ExpressionVisitorContext _visitorContext;
        public BinaryExpressionTranslator(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            _context = context;
            _visitorContext = visitorContext;
        }

        public BsonDocument Extract(BinaryExpression expr)
        {
            if (IsValidJsonEqualityExpression(expr))
                return JsonEqualityExpression(expr);
            else if (IsLogicalExpression(expr))
                return LogicalBinaryExpression(expr);
            else
                return FieldComparisonOrCalculationExpression(expr);
        }

        private BsonDocument JsonEqualityExpression(BinaryExpression expr)
        {
            var fieldName = new ExpressionVisitor(_context).Visit(expr.Left); 

            // 步骤3：获取右侧常量值
            object rightValue = ExpressionTool.DynamicInvoke(expr.Right);

            var builder=InstanceFactory.GetInsertBuilder(this._context.context.CurrentConnectionConfig);
            var json=builder.SerializeObjectFunc(rightValue); 
            return new BsonDocument(fieldName+"",UtilMethods.ParseJsonObject(json));
        }

        private bool IsValidJsonEqualityExpression(BinaryExpression expr)
        {
            return this._context?.context != null && expr.NodeType == ExpressionType.Equal && UtilMethods.IsJsonMember(expr.Left, this._context?.context) && ExpressionTool.GetParameters(expr.Right).Count == 0;
        }

        private static bool IsLogicalExpression(BinaryExpression expr)
        {
            return expr.NodeType == ExpressionType.AndAlso || expr.NodeType == ExpressionType.OrElse;
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
