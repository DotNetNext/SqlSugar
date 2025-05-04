using MongoDB.Bson;
using Newtonsoft.Json.Linq; 
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDbCore
{
    public class FieldPathExtractor
    {
        MongoNestedTranslatorContext _context;
        ExpressionVisitorContext _visitorContext;

        public FieldPathExtractor(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            _context = context;
            _visitorContext = visitorContext;
        }

        public BsonValue Extract(Expression expr)
        {
            var oldExp = expr;
            var oldMember = expr as MemberExpression;
            var parts = new Stack<string>();

            while (expr is MemberExpression member)
            {
                parts.Push(member.Member.Name);
                expr = member.Expression!;
            }

            if (expr is ConstantExpression constantExpression)
            {
                var value = ExpressionTool.GetMemberValue(oldMember.Member, oldExp);
                return BsonValue.Create(value);
            }

            if (_visitorContext != null)
            {
                _visitorContext.ExpType = typeof(MemberExpression);
            }
            return BsonValue.Create(string.Join(".", parts));
        }
    }

}
