using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDbCore 
{
    public class MemberInitExpressionTractor 
    {
        MongoNestedTranslatorContext _context;
        ExpressionVisitorContext _visitorContext;

        public MemberInitExpressionTractor(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
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
            else if(this._context.resolveType == ResolveExpressType.SelectSingle)
            {
                return Select(expr);
            }
            throw new NotSupportedException(this._context.resolveType+"");
        }

        private BsonValue Select(Expression expr)
        {
            var exp = expr as MemberInitExpression;
            var projectionDocument = new BsonDocument();

            // Iterate over the bindings in the MemberInitExpression
            foreach (var binding in exp.Bindings)
            {
                if (binding is MemberAssignment assignment)
                {
                    var fieldName = assignment.Member.Name; // 原字段名

                    // 将原字段名动态转换为 新字段名（例如：name -> name1）
                    var newFieldName = fieldName ;

                    // 将字段投影为 "新字段名" : "$原字段名"
                    projectionDocument[newFieldName] = $"${fieldName}";
                }
            } 
            return projectionDocument;
        }

        private  BsonValue Update(Expression expr)
        {
            var exp = expr as MemberInitExpression;
            var setDocument = new BsonDocument();
            foreach (var binding in exp.Bindings)
            {
                if (binding is MemberAssignment assignment)
                {
                    var fieldName = assignment.Member.Name; 
                    var fieldValue =new ExpressionVisitor(_context,_visitorContext).Visit(Expression.Lambda(assignment.Expression));
                    if (assignment.Expression is MemberExpression && ExpressionTool.GetParameters(assignment.Expression).Count > 0)
                    {
                        setDocument[fieldName] = new BsonDocument("$getField", fieldValue); 
                    }
                    else
                    {
                        setDocument[fieldName] = BsonValue.Create(fieldValue);
                    }
                }
            }
            return new BsonDocument("$set", setDocument);
        }
    }
}
