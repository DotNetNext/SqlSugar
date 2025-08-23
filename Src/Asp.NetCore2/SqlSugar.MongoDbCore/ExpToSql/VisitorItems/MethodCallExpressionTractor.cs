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
                    if (IsComplexAnyExpression(methodCallExpression, out AnyArgModel anyArgModel))
                    {
                        return HandleComplexAnyExpression(methodCallExpression);
                    }
                    else if (IsUnresolvableAnyExpression(anyArgModel)) 
                    {
                        return UnresolvablelexAnyExpression(methodCallExpression);
                    }
                    else if (IsSimpleValueListAny(methodCallExpression))
                    {
                        return HandleSimpleValueListAny(methodCallExpression);
                    }
                    else
                    {
                        return ProcessAnyExpression(methodCallExpression);
                    }

                }
                else if (IsAnyMethodCallEmpty(methodCallExpression, name)) 
                {
                        // 处理 it.xx.Any()，即判断 JSON 数组字段是否非空
                        var memberExpression = methodCallExpression.Arguments.FirstOrDefault() as MemberExpression;
                        if (memberExpression != null)
                        {
                            // 获取集合字段名
                            var collectionField = MongoNestedTranslator.TranslateNoFieldName(
                                memberExpression,
                                _context,
                                new ExpressionVisitorContext { IsText = true }
                            );

                            // 构造 $size > 0 查询
                            var bson = new BsonDocument
                            {
                                { "$expr", new BsonDocument("$gt", new BsonArray { new BsonDocument("$size", UtilMethods.GetMemberName(collectionField)), 0 }) }
                            };
                            result = bson;
                        }
                    }
                {
                }
                return result;
            }
        }

        private BsonValue UnresolvablelexAnyExpression(MethodCallExpression methodCallExpression)
        {
            BsonValue bsonValue = null;
            return null;
        }

        private BsonValue HandleSimpleValueListAny(MethodCallExpression methodCallExpression)
        {
            var memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
            var lambdaExpression = methodCallExpression.Arguments[1] as LambdaExpression;
            var paramName = lambdaExpression.Parameters.FirstOrDefault()?.Name;

            // 集合字段名
            var collectionField = MongoNestedTranslator.TranslateNoFieldName(
                memberExpression,
                _context,
                new ExpressionVisitorContext { IsText = true }
            )?.ToString();

            // Lambda 表达式体
            var body = lambdaExpression.Body;
            if (body is BinaryExpression binaryExpr)
            {
                // 只处理 s == value 这种简单表达式
                string mongoOperator = binaryExpr.NodeType switch
                {
                    ExpressionType.Equal => "$in",
                    ExpressionType.NotEqual => "$nin",
                    _ => null
                };
                if (mongoOperator != null)
                {
                    // s == value，左边是参数，右边是常量
                    var left = binaryExpr.Left;
                    var right = binaryExpr.Right;
                    if (left is ParameterExpression && right is ConstantExpression constant)
                    {
                        var value = UtilMethods.MyCreate(constant.Value);
                        var bson = new BsonDocument
                                    {
                                        { collectionField, new BsonDocument(mongoOperator, new BsonArray { value }) }
                                    };
                        return bson;
                    }
                    // s == it.xx 这种情况
                    if (left is ParameterExpression && right != null)
                    {
                        var valueExpr = MongoNestedTranslator.TranslateNoFieldName(
                            right,
                            _context,
                            new ExpressionVisitorContext { IsText = true }
                        );
                        var bson = new BsonDocument
                                    {
                                        { collectionField, new BsonDocument(mongoOperator, new BsonArray { valueExpr }) }
                                    };
                        return bson;
                    }
                }
            }
            return null;
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
        private BsonValue HandleComplexAnyExpression(MethodCallExpression methodCallExpression)
        {
            // 处理 it.Book.Any(s => s.Price == it.Age) 这种主表字段关联的 Any 表达式
            // 参数1: 集合字段 it.Book
            // 参数2: Lambda 表达式 s => s.Price == it.Age

            var memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
            var lambdaExpression = methodCallExpression.Arguments[1] as LambdaExpression;
            var firstParameterName = lambdaExpression.Parameters.FirstOrDefault().Name;

            // 获取集合字段名
            var collectionField = MongoNestedTranslator.TranslateNoFieldName(
                memberExpression,
                _context,
                new ExpressionVisitorContext { IsText = true }
            )?.ToString();

            // 处理 Lambda 表达式体
            var body = lambdaExpression.Body;

            // 支持多种比较操作
            if (body is BinaryExpression binaryExpr)
            {
                // 左右表达式
                var left = binaryExpr.Left;
                var right = binaryExpr.Right;
                if (ExpressionTool.GetParameters(right).Any(s => s.Name == firstParameterName))
                {
                    left = binaryExpr.Right;
                    right = binaryExpr.Left;
                }
                string leftField = MongoNestedTranslator.TranslateNoFieldName(left, _context, new ExpressionVisitorContext { IsText = true })?.ToString();
                string rightField = MongoNestedTranslator.TranslateNoFieldName(right, _context, new ExpressionVisitorContext { IsText = true })?.ToString();

                // 映射表达式类型到Mongo操作符
                string mongoOperator = binaryExpr.NodeType switch
                {
                    ExpressionType.Equal => "$eq",
                    ExpressionType.NotEqual => "$ne",
                    ExpressionType.GreaterThan => "$gt",
                    ExpressionType.GreaterThanOrEqual => "$gte",
                    ExpressionType.LessThan => "$lt",
                    ExpressionType.LessThanOrEqual => "$lte",
                    _ => null
                };

                if (mongoOperator != null)
                {
                    var mapDoc = new BsonDocument
                    {
                        { "input", $"${collectionField}" },
                        { "as", "b" },
                        { "in", new BsonDocument(mongoOperator, new BsonArray { $"$$b.{leftField}", $"${rightField}" }) }
                    };
                    var anyElementTrueDoc = new BsonDocument("$expr", new BsonDocument("$anyElementTrue", new BsonDocument("$map", mapDoc)));
                    return anyElementTrueDoc;
                }
            }
            return null;
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
                if (MongoDbExpTools.RemoveConvert(b.Left) is MemberExpression  lm== false|| leftCount != 1) 
                    return false;
                if (MongoDbExpTools.RemoveConvert(b.Right) is MemberExpression  lr== false || rightCount != 1)
                    return false;
                anyArgModel.IsBinary = true;
                anyArgModel.LamdaExpression = l;
                anyArgModel.Left = lm;
                anyArgModel.LeftCount = leftCount;
                anyArgModel.RightCount = rightCount;
                anyArgModel.Right = lr;
                anyArgModel.NodeType = b.NodeType;
            }
            return true;
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

        private bool IsUnresolvableAnyExpression(AnyArgModel anyArgModel)
        {
            var leftIsUn = !(anyArgModel.Left is MemberExpression) && anyArgModel.LeftCount >= 1;
            var rightIsUn = !(anyArgModel.Right is MemberExpression) && anyArgModel.RightCount >= 1;
            if (leftIsUn|| rightIsUn) 
            {
                return true;
            }
            return false;
        } 
        private static bool IsAnyMethodCall(MethodCallExpression methodCallExpression, string name)
        {
            return name == "Any" && methodCallExpression.Arguments.Count == 2;
        }
        private static bool IsAnyMethodCallEmpty(MethodCallExpression methodCallExpression, string name)
        {
            return name == "Any" && methodCallExpression.Arguments.Count ==1;
        }

        private static bool IsCountJson(MethodCallExpression methodCallExpression, string name)
        {
            return name == "Count" && methodCallExpression?.Arguments?.FirstOrDefault() is MemberExpression m;
        } 
    }
}
