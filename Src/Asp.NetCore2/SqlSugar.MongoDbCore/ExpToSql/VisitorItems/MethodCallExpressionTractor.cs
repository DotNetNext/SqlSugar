using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
namespace SqlSugar.MongoDb 
{
    public class MethodCallExpressionTractor
    {
        MongoNestedTranslatorContext _context;
        ExpressionVisitorContext _visitorContext;
        public MethodCallExpressionTractor(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            _context = context;
            _visitorContext = visitorContext; 
        }
        public BsonValue Extract(Expression expr)
        {
            if (ExpressionTool.GetParameters(expr).Count == 0)
            {
                return  UtilMethods.MyCreate(ExpressionTool.DynamicInvoke(expr));
            }
            else
            {
                var methodCallExpression = expr as MethodCallExpression;
                var name = methodCallExpression.Method.Name;
                BsonValue result = null;
                if (typeof(IDbMethods).GetMethods().Any(it => it.Name == name))
                {
                    var context = new MongoDbMethod() {  context=this._context};
                    MethodCallExpressionModel model = new MethodCallExpressionModel();
                    var args= methodCallExpression.Arguments; 
                    model.Args = new List<MethodCallExpressionArgs>();
                    model.DataObject = methodCallExpression.Object;
                    foreach (var item in args)
                    {
                        model.Args.Add(new MethodCallExpressionArgs() { MemberValue = item });
                    }
                    if (name == nameof(ToString))
                    {
                        var funcString = context.ToString(model);
                        result = BsonDocument.Parse(funcString);
                    }
                    else
                    {
                        var funcString = context.GetType().GetMethod(name).Invoke(context, new object[] { model });
                        result = UtilMethods.MyCreate(funcString);
                    }
                }
                return result;
            }
        }
    }
}
