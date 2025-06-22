using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public class MongoDbExpTools
    {

        public static bool IsFieldNameJson(string trimmed)
        {
            return trimmed.StartsWith("{ \"fieldName\" : ");
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
