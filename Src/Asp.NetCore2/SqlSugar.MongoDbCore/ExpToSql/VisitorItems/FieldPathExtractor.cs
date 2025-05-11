using MongoDB.Bson;
using Newtonsoft.Json.Linq; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb
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
            if (ExpressionTool.GetParameters(expr).Count == 0) 
            { 
                var value = ExpressionTool.GetMemberValue(oldMember.Member, oldExp);
                return BsonValue.Create(value);
            } 
            var parts = new Stack<string>();

            while (expr is MemberExpression member)
            {
                parts.Push(member.Member.Name);
                expr = member.Expression!;
            }

            if (_visitorContext != null)
            {
                _visitorContext.ExpType = typeof(MemberExpression);
            }
            if (parts.Count == 1&& expr is ParameterExpression parameter) 
            {
                if (_context?.context != null)
                {
                    var entityInfo = _context.context.EntityMaintenance.GetEntityInfo(parameter.Type);
                    var columnInfo = entityInfo.Columns.FirstOrDefault(s => s.PropertyName == parts.First());
                    if (columnInfo != null)
                    {
                        return BsonValue.Create(columnInfo.DbColumnName);
                    }
                }
            }
            return BsonValue.Create(string.Join(".", parts));
        }
    }

}
