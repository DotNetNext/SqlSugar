using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System.Collections;
using System.Reflection;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using System.Diagnostics.CodeAnalysis;
using Dm.util;
namespace SqlSugar.MongoDb 
{
    public partial class MethodCallExpressionTractor
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
                var args =methodCallExpression.Arguments;
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
                    else if (name.StartsWith("To") || name == nameof(SqlFunc.IIF))
                    {
                        var value = context.GetType().GetMethod(name).Invoke(context, new object[] { model });
                        result = BsonDocument.Parse(value?.ToString());
                    }
                    else if (name.StartsWith("Aggregate"))
                    {
                        var value = context.GetType().GetMethod(name).Invoke(context, new object[] { model });
                        result = UtilMethods.MyCreate(value?.ToString());
                    }
                    else if (name.StartsWith(nameof(SqlFunc.Equals)))
                    {

                        var left = model.DataObject;
                        var right = model.Args[0].MemberValue;
                        var exp = Expression.Equal(left as Expression, right as Expression);
                        var resultValue = new ExpressionVisitor(_context, new ExpressionVisitorContext()).Visit(exp);
                        result = new ExpressionVisitor(_context, new ExpressionVisitorContext()).Visit(exp);
                        return result;
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
                else if (name == "End")
                {
                    var ifConditions = MongoDbExpTools.ExtractIfElseEnd(methodCallExpression);
                    return BuildMongoSwitch(ifConditions);
                }
                else if (IsCountJson(methodCallExpression, name))
                {
                    // 处理 it.xx.Count() 其中 xx 为 JSON 数组字段
                    var memberExpression = methodCallExpression.Arguments.FirstOrDefault() as MemberExpression;
                    if (memberExpression != null)
                    {
                        // 获取集合字段名
                        var collectionField = MongoNestedTranslator.TranslateNoFieldName(
                            memberExpression,
                            _context,
                            new ExpressionVisitorContext { IsText = true }
                        );

                        // 构造 $size 查询
                        var bson = new BsonDocument
                            {
                                { "$size", UtilMethods.GetMemberName(collectionField) }
                            };
                        result = bson;
                    }
                }
                else if (IsAnyMethodCall(methodCallExpression, name))
                {
                    if (IsComplexAnyExpression(methodCallExpression, out AnyArgModel anyArgModel))
                    {
                        return HandleComplexAnyExpression(methodCallExpression);
                    }
                    else if (IsSimpleValueListAny(methodCallExpression))
                    {
                        return HandleSimpleValueListAny(methodCallExpression);
                    } 
                    else if (IsCallAnyExpression(anyArgModel))
                    {
                        return CallAnyExpression(methodCallExpression,anyArgModel);
                    }
                    else
                    {
                        return ProcessAnyExpression(methodCallExpression);
                    }

                }
                else if (IsAnyMethodCallEmpty(methodCallExpression, name))
                {
                    result = HandleAnyMethodCallEmpty(methodCallExpression, result);
                }
                return result;
            }
        }

    
        public static BsonValue TranslateMethodCall(MethodCallExpression methodCall, Type documentType,string name)
        {
            var type = typeof(MongoDbExpTools);

            // 获取泛型方法定义
            var method = type.GetMethod("GetFilterBson", BindingFlags.Public | BindingFlags.Static);
            if (method == null)
                throw new InvalidOperationException("找不到 GetFilterBson 方法");

            // 构造泛型方法
            var genericMethod = method.MakeGenericMethod(documentType);

            // 调用方法 (假设 methodCall 就是 Expression)
            var lambda = Expression.Lambda(methodCall, ExpressionTool.GetParameters(methodCall).First());
            var result = genericMethod.Invoke(null, new object[] { lambda });

            return (BsonValue)result;
        }
        private static bool IsSimpleValueListAny(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Arguments.Count == 2)
            {
                var memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
                var lambdaExpression = methodCallExpression.Arguments[1] as LambdaExpression;
                if (memberExpression != null && lambdaExpression != null)
                {
                    var memberType = memberExpression.Type;
                    if (typeof(IEnumerable<string>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<int>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<long>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<double>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<float>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<Guid>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<decimal>).IsAssignableFrom(memberType))
                    {
                        // 判断 lambda 是否是 s => s == value 或 s => s != value
                        if (lambdaExpression.Body is BinaryExpression binaryExpr)
                        {
                            if ((binaryExpr.Left is ParameterExpression && binaryExpr.Right is ConstantExpression) ||
                                (binaryExpr.Left is ParameterExpression && binaryExpr.Right != null))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        private static bool IsComplexAnyExpression(MethodCallExpression methodCallExpression, out AnyArgModel anyArgModel)
        {

            anyArgModel=new AnyArgModel() { IsBinary = false };

            if (methodCallExpression.Arguments.Count != 2)
                return false;
            var isTwoMemeber=ExpressionTool.GetParameters(methodCallExpression.Arguments[1]).Select(s => s.Name).Distinct().Count() == 2;

            if (!isTwoMemeber)
            {
                if (methodCallExpression.Arguments[1] is LambdaExpression la && la.Body is BinaryExpression bin) 
                {
                    var leftCount = ExpressionTool.GetParameters(MongoDbExpTools.RemoveConvert(bin.Left)).Count();
                    var rightCount = ExpressionTool.GetParameters(MongoDbExpTools.RemoveConvert(bin.Right)).Count();
                    anyArgModel.IsBinary = true;
                    anyArgModel.LamdaExpression = la;
                    anyArgModel.Left = MongoDbExpTools.RemoveConvert(bin.Left);
                    anyArgModel.LeftCount = leftCount;
                    anyArgModel.RightCount = rightCount;
                    anyArgModel.Right = MongoDbExpTools.RemoveConvert(bin.Right);
                    anyArgModel.NodeType = bin.NodeType;
                }
                return false;
            }
            if (methodCallExpression.Arguments[1] is LambdaExpression l&&l.Body is BinaryExpression b) 
            {
                var leftCount = ExpressionTool.GetParameters(MongoDbExpTools.RemoveConvert(b.Left)).Count();
                var rightCount = ExpressionTool.GetParameters(MongoDbExpTools.RemoveConvert(b.Right)).Count(); 
                anyArgModel.LamdaExpression = l;
                anyArgModel.Left = MongoDbExpTools.RemoveConvert(b.Left);
                anyArgModel.LeftCount = leftCount;
                anyArgModel.RightCount = rightCount;
                anyArgModel.Right = MongoDbExpTools.RemoveConvert(b.Right);
                anyArgModel.NodeType = b.NodeType;
                if (leftCount != 1) 
                    return false;
                if (rightCount != 1)
                    return false;
                anyArgModel.IsBinary = true;
            }
            return true;
        }

      
        public BsonValue BuildMongoSwitch(List<KeyValuePair<string, Expression>> ifConditions)
        {
            if (ifConditions == null || ifConditions.Count < 3)
                throw new ArgumentException("Insufficient conditions");

            var branches = new BsonArray();
            var context = _context;

            int i = 0;
            while (i < ifConditions.Count - 1)
            {
                var key = ifConditions[i].Key;

                if (key == "IF" || key == "ELSEIF")
                {
                    // 取当前条件
                    var caseExpr = MongoNestedTranslator.TranslateNoFieldName(
                        ifConditions[i].Value,
                        context,
                        new ExpressionVisitorContext { IsText = true }
                    );

                    // 取下一个RETURN
                    if (i + 1 >= ifConditions.Count)
                        throw new InvalidOperationException("Lacking RETURN");

                    var returnPair = ifConditions[i + 1];
                    if (returnPair.Key != "RETURN")
                        throw new InvalidOperationException("IF/ELSEIF Must follow RETURN");

                    var thenExpr = MongoNestedTranslator.TranslateNoFieldName(
                        returnPair.Value,
                        context,
                        new ExpressionVisitorContext { IsText = true }
                    );
                    if (MongoDbExpTools.GetIsMemember(returnPair.Value))
                    {
                        thenExpr = UtilMethods.GetMemberName(thenExpr);
                    } 
                    branches.Add(new BsonDocument
                    {
                        { "case", caseExpr },
                        { "then", thenExpr }
                    }); 
                    i += 2;  // 跳过 RETURN
                }
                else if (key == "END")
                {
                    break;
                }
                else
                {
                    throw new InvalidOperationException($"Not supported Key: {key}");
                }
            }

            // END 的值作为 default
            var defaultValue = MongoNestedTranslator.TranslateNoFieldName(
                ifConditions.Last().Value,
                context,
                new ExpressionVisitorContext { IsText = true }
            ); 
            var switchDoc = new BsonDocument
            {
                { "$switch", new BsonDocument
                    {
                        { "branches", branches },
                        { "default", defaultValue }
                    }
                }
            }; 
            return switchDoc;
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

    
        private static bool IsCountJson(MethodCallExpression methodCallExpression, string name)
        {
            return name == "Count" && methodCallExpression?.Arguments?.FirstOrDefault() is MemberExpression m;
        } 
    }
}
