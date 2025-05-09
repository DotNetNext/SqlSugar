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
            throw new Exception("");
        }

        private static BsonValue Update(Expression expr)
        {
            var exp = expr as MemberInitExpression;
            var setDocument = new BsonDocument();
            foreach (var binding in exp.Bindings)
            {
                if (binding is MemberAssignment assignment)
                {
                    var fieldName = assignment.Member.Name;
                    var fieldValue = Expression.Lambda(assignment.Expression).Compile().DynamicInvoke();
                    setDocument[fieldName] = BsonValue.Create(fieldValue);
                }
            }
            return new BsonDocument("$set", setDocument);
        }
    }
}
