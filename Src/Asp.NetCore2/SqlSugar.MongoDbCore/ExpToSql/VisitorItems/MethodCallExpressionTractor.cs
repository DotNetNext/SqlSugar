using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
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
                    if (IsComplexAnyExpression(methodCallExpression))
                    {
                        return HandleComplexAnyExpression(methodCallExpression);
                    }
                    else
                    {
                        return ProcessAnyExpression(methodCallExpression);
                    }
                }
                return result;
            }
        }

        private BsonValue HandleComplexAnyExpression(MethodCallExpression methodCallExpression)
        {
            // 处理 it.Book.Any(s => s.Price == it.Age) 这种主表字段关联的 Any 表达式
            // 参数1: 集合字段 it.Book
            // 参数2: Lambda 表达式 s => s.Price == it.Age

            var memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
            var lambdaExpression = methodCallExpression.Arguments[1] as LambdaExpression;

            // 获取集合字段名
            var collectionField = MongoNestedTranslator.TranslateNoFieldName(
                memberExpression,
                _context,
                new ExpressionVisitorContext { IsText = true }
            )?.ToString();

            // 处理 Lambda 表达式体
            var body = lambdaExpression.Body;

            // 只处理 s => s.Prop == it.Prop 这种表达式
            if (body is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.Equal)
            {
                // 左右表达式
                var left = binaryExpr.Left;
                var right = binaryExpr.Right;

                // 判断左侧是否为子对象属性，右侧是否为主对象属性
                string leftField = MongoNestedTranslator.TranslateNoFieldName(left, _context, new ExpressionVisitorContext { IsText = true })?.ToString();
                string rightField = MongoNestedTranslator.TranslateNoFieldName(right, _context, new ExpressionVisitorContext { IsText = true })?.ToString();

                // 构造 $map 和 $anyElementTrue
                var mapDoc = new BsonDocument
                                    {
                                        { "input", $"${collectionField}" },
                                        { "as", "b" },
                                        { "in", new BsonDocument("$eq", new BsonArray { $"$$b.{leftField}", $"${rightField}" }) }
                                    };
                var anyElementTrueDoc = new BsonDocument("$anyElementTrue", new BsonDocument("$map", mapDoc));
                return anyElementTrueDoc;
            }
            else 
            {
                return null;
            }
        }

        private static bool IsComplexAnyExpression(MethodCallExpression methodCallExpression)
        {
            return methodCallExpression.Arguments.Count == 2 && ExpressionTool.GetParameters(methodCallExpression.Arguments[1]).Select(s => s.Name).Distinct().Count() == 2;
        }

        private BsonValue ProcessAnyExpression(MethodCallExpression methodCallExpression)
        {
            // 处理 it.xx.Any(s => s.id == 1) 这种表达式
            // memberExpression: it.xx
            // whereExpression: s => s.id == 1
            var memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
            var whereExpression = methodCallExpression.Arguments[1];

            // 获取集合字段名
            var collectionField = MongoNestedTranslator.TranslateNoFieldName(
                memberExpression,
                _context,
                new ExpressionVisitorContext { IsText = true }
            );

            // 生成 $elemMatch 查询
            var elemMatch = MongoNestedTranslator.TranslateNoFieldName(
                whereExpression,
                _context,
                new ExpressionVisitorContext { IsText = true }
            );

            var bson = new BsonDocument
                        {
                            { collectionField?.ToString(), new BsonDocument("$elemMatch", elemMatch) }
                        };
            return bson;
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

        private static bool IsAnyMethodCall(MethodCallExpression methodCallExpression, string name)
        {
            return name == "Any" && methodCallExpression.Arguments.Count == 2;
        }
         
        private static bool IsCountJson(MethodCallExpression methodCallExpression, string name)
        {
            return name == "Count" && methodCallExpression?.Arguments?.FirstOrDefault() is MemberExpression m;
        } 
    }
}
