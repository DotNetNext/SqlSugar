using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlSugar.MongoDb 
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
            else if (this._context.resolveType == ResolveExpressType.ArraySingle) 
            {
                return WhereColumn(expr);
            }
            HandleExpressionError(expr);
            return null;
        }

        private BsonValue WhereColumn(Expression expr)
        {
            if (expr is NewExpression newExpr)
            {
                var bsonArray = new BsonArray();

                // 遍历构造函数参数对应的成员
                foreach (var arg in newExpr.Arguments)
                {
                    if (arg is MemberExpression memberExpr)
                    {
                        bsonArray.Add(memberExpr.Member.Name);
                    }
                    else if (arg is UnaryExpression unary && unary.Operand is MemberExpression innerMember)
                    {
                        // 处理装箱后的成员访问（如 object 包裹）
                        bsonArray.Add(innerMember.Member.Name);
                    }
                    else
                    {
                        HandleExpressionError(expr);
                    }
                } 
                return new BsonDocument(UtilConstants.FieldName,bsonArray);
            } 
            HandleExpressionError(expr);
            return null;
        }

        private static void HandleExpressionError(Expression expr)
        {
            throw new Exception(expr.ToString() + " error");
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

                var visExp = exp.Arguments[exp.Members.IndexOf(member)];
                // 使用 ExpressionVisitor 访问表达式
                var json = new ExpressionVisitor(_context, _visitorContext).Visit(visExp);

                SetProjectionValue(json, fieldName, projectionDocument, visExp);
            }
            projectionDocument["_id"] = 0;
            return projectionDocument;
        }

        private static void SetProjectionValue(BsonValue json, string fieldName, BsonDocument projectionDocument, Expression visExp)
        {
            var jsonString = json.ToJson(UtilMethods.GetJsonWriterSettings());
            if (jsonString.StartsWith("{") && jsonString.EndsWith("}"))
            {
                projectionDocument[fieldName] = json;
            }
            else if (ExpressionTool.GetParameters(visExp).Count == 0) 
            {
                projectionDocument[fieldName] =  jsonString ;
            }
            else
            {
                projectionDocument[fieldName] = "$" + jsonString.TrimStart('\"').TrimEnd('\"');
            }
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
                    setDocument[fieldName] =  UtilMethods.MyCreate(fieldValue);
                }
            }

            return new BsonDocument("$set", setDocument);
        }
    }
}
