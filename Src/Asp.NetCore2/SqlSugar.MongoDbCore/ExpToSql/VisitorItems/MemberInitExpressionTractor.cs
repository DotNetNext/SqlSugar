using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
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
            var exp = expr as MemberInitExpression ?? throw new InvalidOperationException("Expression must be NewExpression");
            var projectionDocument = new BsonDocument();

            // Iterate over the bindings of the NewExpression
            foreach (MemberBinding binding in exp.Bindings)
            {
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new NotSupportedException("Only MemberAssignments are supported.");
                }

                // Cast to MemberAssignment to access the value of the field
                MemberAssignment memberAssignment = (MemberAssignment)binding;

                // Extract the field name (the name of the property or field)
                var fieldName = binding.Member.Name;

                // Build the projection document with the field name and its reference in MongoDB
                var json=new ExpressionVisitor(_context, _visitorContext).Visit(memberAssignment.Expression);
                projectionDocument[fieldName] = "$" + json.ToString();
            }
            projectionDocument["_id"] = 0;
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
