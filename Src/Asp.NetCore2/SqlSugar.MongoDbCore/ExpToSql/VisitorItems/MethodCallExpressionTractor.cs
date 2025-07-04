using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using static Dm.net.buffer.ByteArrayBuffer;
using System.Collections;
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
                return UtilMethods.MyCreate(ExpressionTool.DynamicInvoke(expr));
            }
            else
            {
                var methodCallExpression = expr as MethodCallExpression;
                var name = methodCallExpression.Method.Name;
                name = TransformMethodName(methodCallExpression, name);
                BsonValue result = null;
                var context = new MongoDbMethod() { context = this._context };
                MethodCallExpressionModel model = new MethodCallExpressionModel();
                var args = methodCallExpression.Arguments;
                model.Args = new List<MethodCallExpressionArgs>();
                model.DataObject = methodCallExpression.Object;
                foreach (var item in args)
                {
                    model.Args.Add(new MethodCallExpressionArgs() { MemberValue = item });
                }
                if (typeof(IDbMethods).GetMethods().Any(it => it.Name == name))
                {
                    if (name == nameof(ToString))
                    {
                        var funcString = context.ToString(model);
                        result = BsonDocument.Parse(funcString);
                    }
                    else if (name.StartsWith("To"))
                    {
                        var value = context.GetType().GetMethod(name).Invoke(context, new object[] { model });
                        result = BsonDocument.Parse(value?.ToString());
                    }
                    else if (name.StartsWith("Aggregate"))
                    {
                        var value = context.GetType().GetMethod(name).Invoke(context, new object[] { model });
                        result = UtilMethods.MyCreate(value?.ToString());
                    }
                    else
                    {
                        var methodInfo = context.GetType().GetMethod(name);
                        var funcString = methodInfo.Invoke(context, new object[] { model });
                        result = BsonDocument.Parse(funcString?.ToString());
                    }
                }
                else if (name.StartsWith("Add"))
                {
                    // 根据方法名推断DateType
                    DateType dateType = DateType.Minute;
                    if (name.Equals("AddDays", StringComparison.OrdinalIgnoreCase))
                        dateType = DateType.Day;
                    else if (name.Equals("AddHours", StringComparison.OrdinalIgnoreCase))
                        dateType = DateType.Hour;
                    else if (name.Equals("AddMinutes", StringComparison.OrdinalIgnoreCase))
                        dateType = DateType.Minute;
                    else if (name.Equals("AddSeconds", StringComparison.OrdinalIgnoreCase))
                        dateType = DateType.Second;
                    else if (name.Equals("AddMilliseconds", StringComparison.OrdinalIgnoreCase))
                        dateType = DateType.Millisecond;
                    else if (name.Equals("AddMonths", StringComparison.OrdinalIgnoreCase))
                        dateType = DateType.Month;
                    else if (name.Equals("AddYears", StringComparison.OrdinalIgnoreCase))
                        dateType = DateType.Year;
                    // 追加DateType参数
                    model.Args.Add(new MethodCallExpressionArgs() { MemberValue = dateType });
                    var value = context.DateAddByType(model);
                    result = BsonDocument.Parse(value?.ToString());
                }
                return result;
            }
        }

        private static string TransformMethodName(MethodCallExpression methodCallExpression, string name)
        {
            if (name == "ToDateTime")
                name = "ToDate";
            if (name == "Contains" && methodCallExpression.Arguments.Count == 1
                && methodCallExpression?.Object?.Type != null
                && ExpressionTool.GetParameters(methodCallExpression.Object).Count == 0
                && typeof(IEnumerable).IsAssignableFrom(methodCallExpression.Object.Type))
            {
                name = "ContainsArray";
            }
            else if (name == "Contains" && methodCallExpression.Arguments.Count ==2
               && methodCallExpression.Arguments.FirstOrDefault().Type.IsArray
               && ExpressionTool.GetParameters(methodCallExpression.Arguments.FirstOrDefault()).Count == 0
               )
            {
                name = "ContainsArray";
            }
            else if (name == "Contains" && methodCallExpression.Arguments.Count == 1
              && methodCallExpression?.Object!=null
              &&UtilMethods.IsCollectionOrArrayButNotByteArray(methodCallExpression.Object.Type)
              && ExpressionTool.GetParameters(methodCallExpression?.Object).Count() >0
              && ExpressionTool.GetParameters(methodCallExpression.Arguments.FirstOrDefault()).Count == 0) 
            { 
                name = "JsonArrayAny";
            }
            return name;
        }
    }
}
