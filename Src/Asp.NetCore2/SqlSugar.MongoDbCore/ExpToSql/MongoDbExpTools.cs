using MongoDb.Ado.data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public class MongoDbExpTools
    {
        /// <summary>
        /// 将表达式转换成 FilterDefinition，并返回对应的 BsonDocument
        /// </summary>
        public static BsonDocument GetFilterBson<TDocument>(Expression<Func<TDocument, bool>> predicate)
        { 
            // 1. 构造 FilterDefinition
            var filter = Builders<TDocument>.Filter.Where(predicate);
            var serializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var registry = BsonSerializer.SerializerRegistry;

            // 3. 构造 RenderArgs<TDocument>
            var args = new RenderArgs<TDocument>(serializer, registry);

            var bson = filter.Render(new RenderArgs<TDocument>(serializer, registry)); 
            return bson;
        }
        public static Expression GetRootObject(Expression expr)
        {
            while (true)
            {
                switch (expr)
                {
                    case MemberExpression memberExpr:
                        expr = memberExpr.Expression;
                        break;

                    case MethodCallExpression methodCallExpr:
                        expr = methodCallExpr.Object ??
                               (methodCallExpr.Arguments.Count > 0 ? methodCallExpr.Arguments[0] : null);
                        break;

                    case UnaryExpression unaryExpr:
                        expr = unaryExpr.Operand;
                        break;

                    default:
                        return expr;
                }
            }
        }
        public static List<KeyValuePair<string, Expression>> ExtractIfElseEnd(MethodCallExpression expression)
        {
            var result = new List<KeyValuePair<string, Expression>>();
            Visit(expression, result);
            result.Reverse();
            return result;
        }

        private static void Visit(Expression exp, List<KeyValuePair<string, Expression>> result)
        {
            if (exp == null) return;

            if (exp is MethodCallExpression methodCall)
            {
                var methodName = methodCall.Method.Name.ToUpperInvariant();

                if (methodName == "IF" || methodName == "ELSEIF")
                {
                    // IF/ELSEIF 的表达式一般在第一个参数
                    result.Add(new KeyValuePair<string, Expression>(methodName, methodCall.Arguments[0]));
                }
                else if (methodName == "RETURN")
                {
                    // END 的默认值一般在第一个参数
                    result.Add(new KeyValuePair<string, Expression>("RETURN", methodCall.Arguments[0]));
                }
                else if (methodName == "END")
                {
                    // END 的默认值一般在第一个参数
                    result.Add(new KeyValuePair<string, Expression>("END", methodCall.Arguments[0]));
                }

                // 递归：链式调用的对象部分
                Visit(methodCall.Object, result);

                // 递归：每个参数（有时候Return里嵌套也可能有IF）
                foreach (var arg in methodCall.Arguments)
                {
                    Visit(arg, result);
                }
            }
        }
        public static bool IsFieldNameJson(string trimmed)
        {
            return trimmed.StartsWith("{ \""+UtilConstants.FieldName+"\" : ");
        }
        public static bool IsFieldNameJson(BsonDocument doc)
        {
            if (doc.Contains(UtilConstants.FieldName))
                return true;
            return false;
        }
        public static string CustomToString(object value)
        {
            if (value == null||value==DBNull.Value)
                return null;

            // 处理数字类型（忽略系统语言）
            if (value is IConvertible)
            {
                if (value is double || value is float || value is decimal || value is int || value is long|| value is uint || value is ulong)
                {
                    return Convert.ToDecimal(value).ToString(CultureInfo.InvariantCulture);
                } 
                else if (value is bool boolValue)
                {
                    return boolValue?"1":"0";
                }
            }

            // 处理时间类型
            if (value is TimeSpan ts)
            {
                return ts.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            }

            // 处理时间类型
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff");
            }

            // 处理 DateTimeOffset 类型（返回 UTC 时间，存储时区偏移量）
            if (value is DateTimeOffset)
            {
                var dateTimeOffset = (DateTimeOffset)value;

                // 存储 UTC 时间
                string utcTime = dateTimeOffset.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                // 如果需要存储时区偏移量，可以将偏移量作为额外字段
                string timezoneOffset = dateTimeOffset.Offset.ToString();

                // 返回两个字段，UTC 时间和时区偏移量（可以根据实际需求组合）
                return $"{utcTime} UTC{timezoneOffset}";
            }

            // 默认的ToString
            return value.ToString();
        }

        public static bool IsField(Expression expr)
        {
            // 如果是字段或属性访问（例如 x.Name）
            if (expr is MemberExpression)
                return true;

            // 如果是类型转换（例如 (object)x.Name），递归判断内部表达式
            if (expr is UnaryExpression unaryExpr &&
                (unaryExpr.NodeType == ExpressionType.Convert || unaryExpr.NodeType == ExpressionType.ConvertChecked))
            {
                return IsField(unaryExpr.Operand);
            }

            return false;
        }
         
        public static bool GetIsMemember(Expression expr)
        {
            return expr is MemberExpression member && member.Expression is ParameterExpression;
        }
        internal static Expression RemoveConvert(Expression item)
        {
            for (int i = 0; i < 10; i++)
            {
                if ((item is UnaryExpression) && (item as UnaryExpression).NodeType == ExpressionType.Convert)
                {
                    item = (item as UnaryExpression).Operand;
                }
                else
                {
                    break;
                }
            }
            return item;
        }
    }
}
