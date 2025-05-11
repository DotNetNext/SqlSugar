using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlSugar.MongoDbCore
{
    public class NewExpressionTractor
    {
        MongoNestedTranslatorContext _context;
        ExpressionVisitorContext _visitorContext;

        public NewExpressionTractor(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            _context = context;
            _visitorContext = visitorContext;
        }

        public BsonValue Extract(Expression expr)
        {
            if (this._context.resolveType == ResolveExpressType.Update)
            {
                return Update(expr);
            }
            else if (this._context.resolveType == ResolveExpressType.SelectSingle)
            {
                return Select(expr);
            }
            throw new NotSupportedException(this._context.resolveType + "");
        }

        private BsonValue Select(Expression expr)
        {

            var exp = expr as NewExpression ?? throw new InvalidOperationException("Expression must be NewExpression");
            var projectionDocument = new BsonDocument();

            // 遍历 NewExpression 的成员
            foreach (var member in exp.Members)
            {
                // 获取字段名
                var fieldName = member.Name;

                // 使用 ExpressionVisitor 访问表达式
                var json = new ExpressionVisitor(_context, _visitorContext).Visit(exp.Arguments[exp.Members.IndexOf(member)]);

                // 构建 MongoDB 的投影文档
                projectionDocument[fieldName] = "$" + json.ToString();
            }
            projectionDocument["_id"] = 0;
            return projectionDocument;
        }

        private BsonValue Update(Expression expr)
        {
            var exp = expr as NewExpression ?? throw new InvalidOperationException("Expression must be NewExpression");
            var setDocument = new BsonDocument();

            for (int i = 0; i < exp.Members.Count; i++)
            {
                var fieldName = exp.Members[i].Name;
                var assignmentExpr = exp.Arguments[i];
                var fieldValue = new ExpressionVisitor(_context, _visitorContext).Visit(Expression.Lambda(assignmentExpr));

                if (assignmentExpr is MemberExpression && ExpressionTool.GetParameters(assignmentExpr).Count > 0)
                {
                    setDocument[fieldName] = new BsonDocument("$getField", fieldValue);
                }
                else
                {
                    setDocument[fieldName] = BsonValue.Create(fieldValue);
                }
            }

            return new BsonDocument("$set", setDocument);
        }
    }
}
